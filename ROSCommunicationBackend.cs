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

/*
 * This file is based on SCUIStub/LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# by @dr490n/@jaredtb  
 */
namespace Project_127 {
    public class ROSCommunicationBackend
    {
        private static HttpClient httpClient = new HttpClient();
        public static string btoa(byte[] data)
        {
            return System.Convert.ToBase64String(data);
        }
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

        private static byte[] BuildPostString(Dictionary<string,string> form)
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

        public static async Task<bool> Login(string ticket, string sessionKey, string sessionTicket, UInt64 RockstarID)
        {
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
            validateResponse v = await Validate(ticket, sessionKey, sessionTicket, machineHash);
            if (!v.error)
            {
                //string res = EntitlementDecrypt(v.text); // Not actual entitlements (yet)
                //MessageBox.Show(res);
                MessageBox.Show(v.text);
                return true;
            }
            else
            {
                MessageBox.Show(v.text); // Show Error
            }

            return false;
        }

        private static async Task<validateResponse> Validate(string t, string sk, string st, byte[] mh)
        {
            validateResponse v = new validateResponse();

            var RNG = new RNGCryptoServiceProvider();
            var challenge = new byte[8];
            RNG.GetBytes(challenge);
            byte[] reqBody = EncryptROSData(BuildPostString(
                        new Dictionary<string, string>{
                            { "ticket", t },
                            { "titleId", "11" },
                        }), sk);

            var req = new HttpRequestMessage
            {
                RequestUri = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/app.asmx/GetTitleAccessToken"),
                Method = HttpMethod.Post,
            };
            req.Headers.Add("Host", "prod.ros.rockstargames.com");
            //req.Headers.Add("Accept", "*/*");
            //req.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");
            req.Headers.TryAddWithoutValidation("ros-Challenge", btoa(challenge));
            req.Headers.TryAddWithoutValidation("ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", req.RequestUri.AbsolutePath, sk, st)));
            req.Headers.TryAddWithoutValidation("ros-SecurityFlags", "239");
            req.Headers.TryAddWithoutValidation("ros-SessionTicket", st);
            req.Headers.TryAddWithoutValidation("User-Agent", GetROSVersionString());
            req.Headers.ConnectionClose = true;
            req.Headers.ExpectContinue = false;

            req.Content = new ByteArrayContent(reqBody);

            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var reqInfo = string.Format("{0}, {1}, {2}", req.RequestUri, req.Headers, GetROSVersionString());

            var res = await httpClient.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                v.error = true;
                v.text = String.Format("Error {0}: {1}\n{2}",
                    (int)res.StatusCode, res.StatusCode, res.Content.ReadAsStringAsync().Result);
                v.status = (int)res.StatusCode;
                return v;
            }
            else
            {
                byte[] rawResp = res.Content.ReadAsByteArrayAsync().Result;
                string xmldoc = Encoding.UTF8.GetString(DecryptROSData(rawResp, rawResp.Length, sk));
                var xml = new XPathDocument(new StringReader(xmldoc));
                var nav = xml.CreateNavigator();

                if (NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Status']", 0) == 0)
                {
                    v.error = true;
                    v.text = String.Format("Could not get title access token "
                        + "from the Social Club. Error code: \n{0}/{1}",
                        NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@Code", "[unknown]"),
                        NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@CodeEx", "[unknown]")
                        );
                    v.status = -1;
                    return v;
                }
                else
                {
                    var acctoken = NavGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Result']", "");
                    if (acctoken == "")
                    {
                        return v; // Unknown Error
                    }

                    // req2.Headers.TryAddWithoutValidation("locale", "en-US");
                    //req2.Headers.TryAddWithoutValidation("machineHash", btoa(mh));
                    var targetURI = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/entitlements.asmx/GetEntitlementBlock");
                    var LKey = new List<byte>();
                    byte[] reqBody2 = EncryptROSData(BuildPostString(
                        new Dictionary<string, string>{
                            { "ticket", t },
                            { "titleAccessToken", acctoken },
                            { "locale", "en-US"},
                            { "machineHash", btoa(mh)},
                        }), sk);
                    LKey.Add(Settings.EnablePreOrderBonus ? (byte)1 : (byte)0);

                    byte[] reqHeaders = HeaderBuild(
                        new Dictionary<string, string>{
                            { "ros-Challenge", btoa(challenge)},
                            { "ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", targetURI.AbsolutePath, sk, st)) },
                            { "ros-SecurityFlags", "239"},
                            { "ros-SessionTicket", st},
                        });
                    LKey.AddRange(BitConverter.GetBytes((UInt16)sk.Length));
                    LKey.AddRange(Encoding.UTF8.GetBytes(sk));

                    LKey.AddRange(BitConverter.GetBytes((UInt16)reqHeaders.Length));
                    LKey.AddRange(reqHeaders);

                    LKey.AddRange(BitConverter.GetBytes((UInt16)reqBody2.Length));
                    LKey.AddRange(reqBody2);

                    var launcBin = LKey.ToArray();
                    using (var b = new BinaryWriter(File.Open("launc.dat", FileMode.Create)))
                    {
                        b.Write(launcBin);
                    }
                    v.error = false;
                    v.status = 200;
                    v.text = "Launcher token data written";
                    return v;
                    /*var req2 = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://prod.ros.rockstargames.com/launcher/11/launcherservices/entitlements.asmx/GetEntitlementBlock"),
                        Method = HttpMethod.Post,
                    };
                    req2.Headers.Add("Host", "prod.ros.rockstargames.com");
                    req2.Headers.TryAddWithoutValidation("ros-Challenge", btoa(challenge));
                    req2.Headers.TryAddWithoutValidation("ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", req2.RequestUri.AbsolutePath, sk, st)));
                    req2.Headers.TryAddWithoutValidation("ros-SecurityFlags", "239");
                    req2.Headers.TryAddWithoutValidation("ros-SessionTicket", st);
                    req2.Headers.TryAddWithoutValidation("User-Agent", GetROSVersionString());
                    req2.Headers.ConnectionClose = true;
                    req2.Headers.ExpectContinue = false;
                    

                    req2.Content = new ByteArrayContent(reqBody2);

                    req2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    res = await httpClient.SendAsync(req2);
                    
                    if (!res.IsSuccessStatusCode)
                    {
                        v.error = true;
                        v.text = String.Format("Error {0}: {1}\r\n{2}",
                            (int)res.StatusCode, res.StatusCode, res.Content.ReadAsStringAsync().Result);
                        v.status = (int)res.StatusCode + 1000;
                        return v;
                    }
                    else
                    {
                        rawResp = res.Content.ReadAsByteArrayAsync().Result;
                        string exmldoc = Encoding.UTF8.GetString(DecryptROSData(rawResp, rawResp.Length, sk));
                        var exml = new XPathDocument(new StringReader(exmldoc));
                        var enav = exml.CreateNavigator();

                        if (NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Status']", 0) == 0) //you are here
                        {
                            v.error = true;
                            v.text = String.Format("Could not get entitlement block "
                                + "from the Social Club. Error code: \n{0}/{1}",
                                NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Error']/@Code", "[unknown]"),
                                NavGetOrDefault(enav, "//*[local-name()='Response']/*[local-name()='Error']/@CodeEx", "[unknown]")
                                );
                            v.status = -1001;
                            return v;
                        }
                        else
                        {
                            v.error = false;
                            v.text = EntitlementHandler(enav);
                            v.status = (v.text != "")? 0 : -1002;
                            return v;
                        }
                    }*/

                    return v;
                }
            }
            return v;
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
                m_rc4Key = new ArraySegment<byte>(platformStr,1,32).ToArray();
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


        private class validateResponse
        {
            public bool error = true;
            public int status = int.MinValue;
            public string text;
        }
    }
}
