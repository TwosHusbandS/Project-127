using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
//using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Documents;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Security.Permissions;
using System.Configuration;
using System.Text.RegularExpressions;

/*
 * This file is based on partially on SCUIStub/LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# for project by @dr490n/@jaredtb  
 */
namespace Project_127 {


    public class ROSCommunicationBackend
    {

        private static byte[] OESFDR = { 0x45, 0xE6, 0xA5, 0x0D, 0xAA, 0xE4, 0x56, 0x2A, 0x5E, 0xAC, 0x05 };
        private static sessionContainer session = null;

        private static UInt32 s1= 0x55415644;


        private static byte laflags; //Launch Attribute Flags


        /// <summary>
        /// Sets a Launch Attribute Flag
        /// </summary>
        /// <param name="flag">Flag to set</param>
        /// <param name="state">State of flag</param>
        public static void setFlag(Flags flag, bool state)
        {
            setFlag((int)flag, state);
        }

        /// <summary>
        /// Sets a Launch Attribute Flag
        /// </summary>
        /// <param name="flag">Flag to set</param>
        /// <param name="state">State of flag</param>
        public static void setFlag(int flag, bool state)
        {
            if (flag > 7)
            {
                return;
            }
            if (state)
            {
                laflags = (byte)(laflags | 1 << flag);
            }
            else
            {
                laflags = (byte)(laflags & (0xFF ^ (1 << flag)));
            }

        }

        private static HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Binary to ascii (Base64 Encode)
        /// </summary>
        /// <param name="data">Data to encode</param>
        /// <returns>Encoded binary data</returns>
        public static string btoa(byte[] data)
        {
            return System.Convert.ToBase64String(data);
        }

        /// <summary>
        /// Ascii to binary (Base64 Decode)
        /// </summary>
        /// <param name="data">Data to decode</param>
        /// <returns>Decoded binary data</returns>
        public static byte[] atob(string data)
        {
            return System.Convert.FromBase64String(data);
        }

        private const string ROS_PLATFORM_KEY_LAUNCHER = "C6fU6TQTPgUVmy3KIB5g8ElA7DrenVpGogSZjsh+lqJaQHqv4Azoctd/v4trfX6cBjbEitUKBG/hRINF4AhQqcg=";

        private static byte[] EncryptROSData(byte[] data, string sessionKey = "")
        {
            // Initialize state
            var state = new ROSCryptoState();
            List<byte> output = new List<byte>();

            // Decode session key (if applicable)
            bool hasSecurity = !String.IsNullOrEmpty(sessionKey);

            byte[] sessKey = new byte[16];
            if (hasSecurity)
            {
                byte[] keyData = atob(sessionKey);
                sessKey = new ArraySegment<byte>(keyData, 0, 16).ToArray();

            }

            // Create a random RC4 key/cipher
            byte[] rc4Key = new byte[16];
            var RNG = new RNGCryptoServiceProvider();
            RNG.GetBytes(rc4Key);

            // XOR the key because the future of encryption is XOR /s
            for (int i = 0; i < 16; i++)
            {
                byte thisChar = (byte)(rc4Key[i] ^ state.GetXorKey()[i]);
                output.Add(thisChar);

                if (hasSecurity)
                {
                    rc4Key[i] ^= sessKey[i];
                }
            }


            // Push RC4 encypted data to output
            output.AddRange(RC4.Encrypt(rc4Key, data));
            List<byte> tempContent = new List<byte>(output);

            // Prepare to hash
            tempContent.AddRange(state.GetHashKey());

            // Hash it
            byte[] hashData;
            if (!hasSecurity)
            {
                hashData = new SHA1Managed().ComputeHash(tempContent.ToArray());
            }
            else
            {
                var hashFunc = new HMACSHA1();
                hashFunc.Key = rc4Key;
                hashData = hashFunc.ComputeHash(tempContent.ToArray());
            }

            // Append hash to output
            output.AddRange(hashData);
            return output.ToArray();
        }

        private static byte[] DecryptROSData(byte[] data, int size, string sessionKey)
        {
            // Initialize state
            ROSCryptoState state = new ROSCryptoState();

            // Initialize key container
            byte[] rc4Key = new byte[16];
            bool hasSecurity = !String.IsNullOrEmpty(sessionKey); //HaS sEcUrItY, LOL

            byte[] sessKey = new byte[16];

            // Decode sesskey (if applicable)
            if (hasSecurity)
            {
                sessKey = atob(sessionKey);
            }

            // Decrypt the RC4 key
            for (int i = 0; i < 16; i++)
            {
                rc4Key[i] = (byte)(data[i] ^ state.GetXorKey()[i]);

                if (hasSecurity)
                {
                    rc4Key[i] ^= sessKey[i];
                }
            }

            // Read in blocksize
            byte[] blockSizeData = RC4.Decrypt(rc4Key, new ArraySegment<byte>(data, 16, 4).ToArray());

            // Sadly, bitconverter doesn't have an explicit from Big Endian
            int blockSize = (blockSizeData[0] << 24) + (blockSizeData[1] << 16) +
                (blockSizeData[2] << 8) + blockSizeData[1] + 20;
            byte[] encblock = data;//.ToArray();

            List<byte> encresult = new List<byte>();

            encresult.AddRange(new ArraySegment<byte>(encblock, 16, 4));
            // Block handling:
            int start = 20;
            while (start < size)
            {
                // Find the end
                int end = Math.Min(size, start + blockSize);

                // We ignore the hash at the end of blocks 
                end -= 20;

                int len = end - start;

                // Grab data block
                ArraySegment<byte> block = new ArraySegment<byte>(encblock, start, len);

                // Push to result
                encresult.AddRange(block);

                // Move the start for next iteration
                start += blockSize;
            }

            var result = RC4.Decrypt(rc4Key, encresult.ToArray());

            return result.Skip(4).ToArray();

        }

        private static byte[] HeadersHmac(byte[] challenge, string method, string path, string sessionKey, string sessionTicket)
        {
            var HMAC = new HMACSHA1();

            ROSCryptoState state = new ROSCryptoState();

            byte[] hmacKey = new byte[16];

            var rc4Xor = atob(sessionKey);

            for (int i = 0; i < hmacKey.Length; i++)
            {
                hmacKey[i] = (byte)(rc4Xor[i] ^ state.GetXorKey()[i]);
            }

            HMAC.Key = hmacKey;
            List<byte> hmacBuff = new List<byte>();

            // Method
            hmacBuff.AddRange(Encoding.ASCII.GetBytes(method));
            hmacBuff.Add(0);

            // Path
            hmacBuff.AddRange(Encoding.ASCII.GetBytes(path));
            hmacBuff.Add(0);

            // ros-SecurityFlags
            hmacBuff.AddRange(Encoding.ASCII.GetBytes("239"));
            hmacBuff.Add(0);

            // ros-SessionTicket
            hmacBuff.AddRange(Encoding.ASCII.GetBytes(sessionTicket));
            hmacBuff.Add(0);

            // ros-Challenge
            hmacBuff.AddRange(Encoding.ASCII.GetBytes(btoa(challenge)));
            hmacBuff.Add(0);

            // Platform hash key
            hmacBuff.AddRange(state.GetHashKey());

            return HMAC.ComputeHash(hmacBuff.ToArray());
        }


        private static string GetROSVersionString()
        {
            byte[] baseString = Encoding.ASCII.GetBytes(String.Format("e={0},t={1},p={2},v={3}", 1, "launcher", "pcros", 11));
            List<byte> xorBuff = new List<byte>();
            xorBuff.AddRange(BitConverter.GetBytes(0xC5C5C5C5));
            for (int i = 0; i < baseString.Length; i++)
            {
                xorBuff.Add((byte)(baseString[i] ^ 0xC5));
            }
            var base64str = btoa(xorBuff.ToArray());

            return String.Format("ros {0}", base64str);
        }

        private static byte[] BuildPostString(Dictionary<string, string> form)
        {
            List<byte> retval = new List<byte>();
            foreach (KeyValuePair<string, string> field in form)
            {
                string key = System.Net.WebUtility.UrlEncode(field.Key);
                string value = System.Net.WebUtility.UrlEncode(field.Value);
                retval.AddRange(Encoding.UTF8.GetBytes(key));
                retval.AddRange(Encoding.UTF8.GetBytes("="));
                retval.AddRange(Encoding.UTF8.GetBytes(value));
                retval.AddRange(Encoding.UTF8.GetBytes("&"));
            }
            retval.RemoveAt(retval.Count - 1);
            return retval.ToArray();
        }
        private static byte[] HeaderBuild(Dictionary<string, string> headers)
        {
            List<byte> retval = new List<byte>();
            foreach (KeyValuePair<string, string> field in headers)
            {
                retval.AddRange(Encoding.UTF8.GetBytes(field.Key));
                retval.AddRange(Encoding.UTF8.GetBytes(": "));
                retval.AddRange(Encoding.UTF8.GetBytes(field.Value));
                retval.AddRange(Encoding.UTF8.GetBytes("\r\n"));
            }
            return retval.ToArray();
        }

        /// <summary>
        /// Generates the persistent session from aquired session attributes
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <param name="sessionKey">Session Key</param>
        /// <param name="sessionTicket">Session Ticket</param>
        /// <param name="RockstarID">Rockstar ID</param>
        /// <param name="ctime">Current Time</param>
        /// <param name="nick">User Nickname</param>
        /// <returns></returns>
        public static async Task<bool> Login(string ticket, string sessionKey, string sessionTicket, UInt64 RockstarID, Int64 ctime = 0, string nick = "")
        {
            HelperClasses.Logger.Log("Verifying Ownership...");
            if (ctime == 0)
            {
                ctime = GetPosixTime();
            }
            var RNG = new RNGCryptoServiceProvider();
            byte[] machineHash = new byte[32];
            RNG.GetBytes(machineHash);
            UInt64 IDSegment = RockstarID ^ 0xDEADCAFEBABEFEED;
            byte[] IDSegmentBytes = BitConverter.GetBytes(IDSegment);
            for (int i = 4; i < IDSegmentBytes.Length + 4; i++)
            {
                machineHash[i] = IDSegmentBytes[i - 4];
            }
            //Validate();
            HelperClasses.Logger.Log("Generating session...");
            session = new sessionContainer(ticket, sessionKey, sessionTicket, machineHash, ctime, nick);
            HelperClasses.Logger.Log("Session genned");
            var res = await GenToken();
            if (!res.error)
            {
                //string res = EntitlementDecrypt(v.text); // Not actual entitlements (yet)
                //MessageBox.Show(res);
                //MessageBox.Show(v.text);
                HelperClasses.Logger.Log("Initial token generation sucess, ownership verification successful");
                return true;
            }
            else
            {
                HelperClasses.Logger.Log("Initial token generation failed;");
                MessageBox.Show(res.text); // Show Error
            }

            return false;
        }

        /// <summary>
        /// Generates a token for the GTA Launcher
        /// </summary>
        public static async Task<tokenGenResponse> GenToken()
        {
            tokenGenResponse tgr = new tokenGenResponse();

            if (!session.isValid())
            {
                tgr.status = 0;
                tgr.text = "Session Expired";
                return tgr;
            }

            var RNG = new RNGCryptoServiceProvider();
            var challenge = new byte[8];
            RNG.GetBytes(challenge);
            byte[] reqBody = EncryptROSData(BuildPostString(
                        new Dictionary<string, string>{
                            { "ticket", session.ticket },
                            { "titleId", "11" },
                        }), session.sessionKey);

            var req = new HttpRequestMessage
            {
                RequestUri = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/app.asmx/GetTitleAccessToken"),
                Method = HttpMethod.Post,
            };
            req.Headers.Add("Host", "prod.ros.rockstargames.com");
            //req.Headers.Add("Accept", "*/*");
            //req.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");
            req.Headers.TryAddWithoutValidation("ros-Challenge", btoa(challenge));
            req.Headers.TryAddWithoutValidation("ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", req.RequestUri.AbsolutePath, session.sessionKey, session.sessionTicket)));
            req.Headers.TryAddWithoutValidation("ros-SecurityFlags", "239");
            req.Headers.TryAddWithoutValidation("ros-SessionTicket", session.sessionTicket);
            req.Headers.TryAddWithoutValidation("User-Agent", GetROSVersionString());
            req.Headers.ConnectionClose = true;
            req.Headers.ExpectContinue = false;

            req.Content = new ByteArrayContent(reqBody);

            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var reqInfo = string.Format("{0}, {1}, {2}", req.RequestUri, req.Headers, GetROSVersionString());

            var res = await httpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                tgr.error = true;
                tgr.text = String.Format("Error {0}: {1}\n{2}",
                    (int)res.StatusCode, res.StatusCode, res.Content.ReadAsStringAsync().Result);
                tgr.status = (int)res.StatusCode;
                return tgr;
            }
            else
            {
                byte[] rawResp = res.Content.ReadAsByteArrayAsync().Result;
                string xmldoc = Encoding.UTF8.GetString(DecryptROSData(rawResp, rawResp.Length, session.sessionKey));
                var xml = new XPathDocument(new StringReader(xmldoc));
                var nav = xml.CreateNavigator();

                if (NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Status']", 0) == 0)
                {
                    tgr.error = true;
                    tgr.text = String.Format("Could not get title access token "
                        + "from the Social Club. Error code: \n{0}/{1}",
                        NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@Code", "[unknown]"),
                        NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@CodeEx", "[unknown]")
                        );
                    tgr.status = -1;
                    return tgr;
                }
                else
                {
                    var acctoken = NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Result']", "");
                    if (acctoken == "")
                    {
                        return tgr; // Unknown Error
                    }

                    //req2.Headers.TryAddWithoutValidation("locale", "en-US");
                    //req2.Headers.TryAddWithoutValidation("machineHash", btoa(mh));
                    var targetURI = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/entitlements.asmx/GetEntitlementBlock");
                    var LKey = new List<byte>();
                    byte[] reqBody2 = EncryptROSData(BuildPostString(
                        new Dictionary<string, string>{
                            { "ticket", session.ticket },
                            { "titleAccessToken", acctoken },
                            { "locale", "en-US"},
                            { "machineHash", btoa(session.machineHash)},
                        }), session.sessionKey);
                    setFlag(Flags.preorder, Settings.EnablePreOrderBonus);
                    LKey.Add(laflags);

                    byte[] reqHeaders = HeaderBuild(
                        new Dictionary<string, string>{
                            { "ros-Challenge", btoa(challenge)},
                            { "ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", targetURI.AbsolutePath, session.sessionKey, session.sessionTicket)) },
                            { "ros-SecurityFlags", "239"},
                            { "ros-SessionTicket", session.sessionTicket},
                        });
                    LKey.AddRange(BitConverter.GetBytes((UInt16)session.sessionKey.Length));
                    LKey.AddRange(Encoding.UTF8.GetBytes(session.sessionKey));

                    LKey.AddRange(BitConverter.GetBytes((UInt16)reqHeaders.Length));
                    LKey.AddRange(reqHeaders);

                    LKey.AddRange(BitConverter.GetBytes((UInt16)reqBody2.Length));
                    LKey.AddRange(reqBody2);

                    LKey.AddRange(genLaunchExtension());
					//LKey.Add(0);
					//LKey.Add(0);


                    var launcBin = LKey.ToArray();
                    var outdir = Settings.GTAVInstallationPath;
                    if (!outdir.EndsWith("\\"))
                    {
                        outdir += "\\";
                    }

                    using (var b = new BinaryWriter(File.Open(outdir + "launc.dat", FileMode.Create)))
                    {
                        b.Write(launcBin);
                    }
                    tgr.error = false;
                    tgr.status = 200;
                    tgr.text = "Launcher token data written";

                    var req2 = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/entitlements.asmx/GetEntitlementBlock"),
                        Method = HttpMethod.Post,
                    };
                    req2.Headers.Add("Host", "prod.ros.rockstargames.com");
                    req2.Headers.TryAddWithoutValidation("ros-Challenge", btoa(challenge));
                    req2.Headers.TryAddWithoutValidation("ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", req2.RequestUri.AbsolutePath, session.sessionKey, session.sessionTicket)));
                    req2.Headers.TryAddWithoutValidation("ros-SecurityFlags", "239");
                    req2.Headers.TryAddWithoutValidation("ros-SessionTicket", session.sessionTicket);
                    req2.Headers.TryAddWithoutValidation("User-Agent", GetROSVersionString());
                    req2.Headers.ConnectionClose = true;
                    req2.Headers.ExpectContinue = false;

                    req2.Content = new ByteArrayContent(reqBody2);
                    req2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    res = await httpClient.SendAsync(req2);

                    if (!res.IsSuccessStatusCode)
                    {
                        tgr.error = true;
                        tgr.text = String.Format("Ownership Verification Error {0}: {1}\r\n{2}",
                            (int)res.StatusCode, res.StatusCode, res.Content.ReadAsStringAsync().Result);
                        tgr.status = (int)res.StatusCode + 1000;
                    }
                    else
                    {
                        rawResp = res.Content.ReadAsByteArrayAsync().Result;
                        string exmldoc = Encoding.UTF8.GetString(DecryptROSData(rawResp, rawResp.Length, session.sessionKey));
                        var exml = new XPathDocument(new StringReader(exmldoc));
                        var enav = exml.CreateNavigator();
                        if (NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Status']", 0) == 0) //you are here
                        {
                            tgr.error = true;
                            tgr.text = String.Format("Ownership error: could not get entitlement block "
                                + "from the Social Club. Error code: \n{0}/{1}",
                                NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Error']/@Code", "[unknown]"),
                                NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Error']/@CodeEx", "[unknown]")
                                );
                            tgr.status = -1001;
                            session.destroy();
                        }

                    }
                    return tgr;

                }
            }
            return tgr;
        }

        /// <summary>
        /// Returns wheter a session is valid (not expired & logged in)
        /// </summary>

        public static bool SessionValid
        {
            get
            {
                if (session == null)
                {
                    return false;
                }
                return session.isValid();
            }
        }

        /// <summary>
        /// Destroys/Logs out session
        /// </summary>
        public static void SessionDestroy()
        {
            session.destroy();
        }
        private static string EntitlementHandler(XPathNavigator nav)
        {
            // PATH "Response.Result.Data"
            var encEntBlock = NavGetOrDefault(nav,
                "//*[local-name()='Response']/*[local-name()='Result']/*[local-name()='Data']",
                "");
            if (encEntBlock == "")
            {
                return "";
            }
            //var decEntBlock = EntitlementDecrypt(encEntBlock);

            var eb = new EntitlementBlock(EntitlementDecrypt(encEntBlock));

            return eb.GetXml();
        }

        private static byte[] EntitlementDecrypt(string encBlock)
        {
            byte[] blob = atob(encBlock);
            byte[] decBlock = EntitlementBlockCipher.DecryptEntitlementBlock(blob);
            return decBlock;
        }

        private class ROSCryptoState
        {
            private byte[] m_rc4Key;
            private byte[] m_xorKey;
            private byte[] m_hashKey;
            public ROSCryptoState()
            {
                byte[] platformStr = atob(ROS_PLATFORM_KEY_LAUNCHER);
                m_rc4Key = new ArraySegment<byte>(platformStr, 1, 32).ToArray();
                m_xorKey = new ArraySegment<byte>(platformStr, 33, 16).ToArray();
                m_hashKey = new ArraySegment<byte>(platformStr, 49, 16).ToArray();

                m_xorKey = RC4.Decrypt(m_rc4Key, m_xorKey);
                m_hashKey = RC4.Decrypt(m_rc4Key, m_hashKey);

            }
            public byte[] GetXorKey()
            {
                return m_xorKey;
            }
            public byte[] GetHashKey()
            {
                return m_hashKey;
            }
        }

        private static string NavGetOrDefault(XPathNavigator x, string p, string d)
        {
            return x.SelectSingleNode(p) != null ? x.SelectSingleNode(p).Value : d;
        }
        private static int NavGetOrDefault(XPathNavigator x, string p, int d)
        {
            return x.SelectSingleNode(p) != null ? int.Parse(x.SelectSingleNode(p).Value) : d;
        }

        /// <summary>
        /// Response class for the GenToken method
        /// </summary>
        public class tokenGenResponse
        {
            public bool error = true;
            public int status = int.MinValue;
            public string text;
        }

        private class sessionContainer
        {
            private string tick;
            private string sesskey;
            private string sesstick;
            private byte[] mhash;
            private Int64 expiration;
            private string nickname;
            public sessionContainer(string t, string sk, string st, byte[] mh, Int64 ctime, string nick)
            {
                update(t, sk, st, mh, ctime, nick);
                //store();
            }
            public void update(string t, string sk, string st, byte[] mh, Int64 ctime, string nick)
            {
                tick = t;
                sesskey = sk;
                sesstick = st;
                mhash = mh;
                expiration = ctime + 86399;
                nickname = nick;
#if DEBUG
                if (Settings.InGameName == "HiMomImOnYoutube")
                {
                    addLaunchExtension("ingameNick", "1337haxx0r");
                }
#else
                if (Settings.InGameName != "HiMomImOnYoutube")
                {
                    addLaunchExtension("ingameNick", Settings.InGameName);
                }
#endif
                try
                {
                    if (nick == "gta5downgrade")
                    {
                        addLaunchExtension("ingameNick", "LiterallyAnybody");
                    }
                    else if ((nick+ "=") == btoa(OESFDR))
                    {
                        addLaunchExtension("specUser", 
                            Encoding.UTF8.GetString(
                                BitConverter.GetBytes(s1)
                                ));
                    }
                } finally { }
            }
            /*public sessionContainer()
            {
                load();
            }*/

            public bool isValid()
            {
                return (expiration > GetPosixTime());
            }
            public void destroy()
            {
                expiration = 0; //For the moment
            }
            /*private void load() //Possibly maybe persitence accross launches
            {

            }
            private void store()
            {
                var sess = new List<byte>();
                sess.AddRange(BitConverter.GetBytes((UInt16)tick.Length));
                sess.AddRange(Encoding.UTF8.GetBytes(tick));

                sess.AddRange(BitConverter.GetBytes((UInt16)sesskey.Length));
                sess.AddRange(Encoding.UTF8.GetBytes(sesskey));

                sess.AddRange(BitConverter.GetBytes((UInt16)sesstick.Length));
                sess.AddRange(Encoding.UTF8.GetBytes(sesstick));

                sess.AddRange(BitConverter.GetBytes((UInt16)mhash.Length));
                sess.AddRange(mhash);

                sess.AddRange(BitConverter.GetBytes((UInt16)8));
                sess.AddRange(BitConverter.GetBytes(expiration));



                using (var b = new BinaryWriter(File.Open("sess.dat", FileMode.Create)))
                {
                    b.Write(sess.ToArray());
                }
            }*/
            public string ticket
            {
                get
                {
                    return tick;
                }
            }
            public string sessionKey
            {
                get
                {
                    return sesskey;
                }
            }
            public string sessionTicket
            {
                get
                {
                    return sesstick;
                }
            }
            public byte[] machineHash
            {
                get
                {
                    return mhash;
                }
            }
            public string Nick
            {
                get
                {
                    return nickname;
                }
            }
        }

        private static Int64 GetPosixTime()
        {
            return (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// Enum defining the current Launch Attribute Flags
        /// </summary>
        public enum Flags
        {
            preorder,
            RES1,
            RES2,
            RES3,
            RES4,
            RES5,
            RES6,
            RES7,

        }
        private static string launchExtensionRaw;


        /// <summary>
        /// Adds special launch extension parameters
        /// Parameters include:
        /// saveDirOverride: Overrides the default save dir
        /// profileIdOverride: Overrides the default profile ID
        /// ingameNick: Overrides the default ingame nickname
        /// </summary>
        /// <param name="key">Parameter to set</param>
        /// <param name="value">Value of parameter</param>
        private static void addLaunchExtension(string key, string value, bool checkSpec = true)
        {
            var checkRegex = new Regex("^[a-zA-Z0-9+/._-]*$");
            if (!checkRegex.IsMatch(value) && checkSpec)
            {
                value = btoa(Encoding.UTF8.GetBytes(value));
            }
            if (launchExtensionRaw != "")
            {
                launchExtensionRaw += "&";
            }
            launchExtensionRaw += String.Format("{0}={1}", key, value);
        }
        /// <summary>
        /// Adds special launch extension parameters
        /// </summary>
        /// <param name="key">Parameter to set</param>
        /// <param name="value">Value of parameter</param>
        private static void addLaunchExtension(string key, byte[] value)
        {
            addLaunchExtension(key, btoa(value));
        }
        /// <summary>
        /// Clears all launch extension parameters
        /// </summary>
        private void clearLaunchExtension()
        {
            launchExtensionRaw = "";
        }

        private static List<byte> genLaunchExtension()
        {
            if (launchExtensionRaw == "" || launchExtensionRaw == null)
            {
                var ret = new List<byte>();
                ret.Add(0);
                ret.Add(0);
                return ret;
            } else
            {
                var lab = Encoding.UTF8.GetBytes(launchExtensionRaw);
                var ret = new List<byte>(lab.Length + 2);
                ret.AddRange(BitConverter.GetBytes((UInt16)lab.Length));
                ret.AddRange(lab);
                return ret;
            }
        }


        /// <summary>
        /// Get current logins rockstar nickname
        /// </summary>
        public static string sessionNickname //For future use
        {
            get
            {
                return session.Nick;
            }
        }
        
    }
}
