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
 * This file is based on SCUIStub from the CitizenFX Project - http://citizen.re/
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
            byte[] encblock = data.Skip(16).ToArray();
            byte[] fullData = RC4.Decrypt(rc4Key, encblock);
            List<byte> result = new List<byte>();

            // Block handling:
            int start = 4;
            while (start < (size - 16))
            {
                // Find the end
                int end = Math.Min(size - 16, start + blockSize);

                // We ignore the hash at the end of blocks (the decryption corrupted it anyways)
                end -= 20; 

                int len = end - start;

                // Grab deciphered block
                ArraySegment<byte> block = new ArraySegment<byte>(fullData, start, len);

                // Push to result
                result.AddRange(block.ToArray());

                // Move the start for next iteration
                start += blockSize;
            }

            return result.ToArray();

        }

        /*HeadersHmac(const std::vector<uint8_t, TAlloc>& challenge, const char* method, const char* path, const std::string& sessionKey, const std::string& sessionTicket)
        {
        auto hmac = std::unique_ptr<Botan::MessageAuthenticationCode>(Botan::get_mac("HMAC(SHA1)")->clone());

        ROSCryptoState cryptoState;

        // set the key
        uint8_t hmacKey[16];

        // xor the RC4 key with the platform key (and optionally the session key)
        auto rc4Xor = Botan::base64_decode(sessionKey);

        for (int i = 0; i < sizeof(hmacKey); i++)
        {
            hmacKey[i] = rc4Xor[i] ^ cryptoState.GetXorKey()[i];
        }

        hmac->set_key(hmacKey, sizeof(hmacKey));

        // method
        hmac->update(method);
        hmac->update(0);

        // path
        hmac->update(path);
        hmac->update(0);

        // ros-SecurityFlags
        hmac->update("239");
        hmac->update(0);

        // ros-SessionTicket
        hmac->update(sessionTicket);
        hmac->update(0);

        // ros-Challenge
        hmac->update(Botan::base64_encode(challenge));
        hmac->update(0);

        // platform hash key
        hmac->update(cryptoState.GetHashKey(), 16);

        // set the request header
        auto hmacValue = hmac->final();

        return hmacValue;
        }*/
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
            validateResponse v = Validate(ticket, sessionKey, sessionTicket, machineHash);
            if (!v.error)
            {
                //string res = EntitlementDecrypt(v.text); // Not actual entitlements (yet)
                //MessageBox.Show(res);
                return true;
            }
            else
            {
                MessageBox.Show(v.text); // Show Error
            }

            return false;
        }

        private static validateResponse Validate(string t, string sk, string st, byte[] mh)
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
                RequestUri = new Uri("http://192.81.241.100/launcher/11/launcherservices/app.asmx/GetTitleAccessToken"),
                Method = HttpMethod.Post,
            };
            req.Headers.Add("Host", "prod.ros.rockstargames.com");
            req.Headers.Add("Accept", "*/*");
            req.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");
            req.Headers.TryAddWithoutValidation("ros-Challenge", btoa(challenge));
            req.Headers.TryAddWithoutValidation("ros-HeadersHmac", btoa(HeadersHmac(challenge, "POST", "/launcher/11/launcherservices/app.asmx/GetTitleAccessToken", sk, st)));
            req.Headers.TryAddWithoutValidation("ros-SecurityFlags", "239");
            req.Headers.TryAddWithoutValidation("ros-SessionTicket", st);
            req.Headers.TryAddWithoutValidation("User-Agent", GetROSVersionString());
            req.Headers.Remove("Connection");
            req.Headers.Remove("Expect");
            req.Headers.ConnectionClose = true;
            req.Headers.ExpectContinue = false;

            req.Content = new ByteArrayContent(reqBody);

            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var reqInfo = string.Format("{0}, {1}, {2}", req.RequestUri, req.Headers, GetROSVersionString());
            System.Windows.Forms.MessageBox.Show(reqInfo);

            var res = httpClient.SendAsync(req).Result;
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

                if (navGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Status']", 0) == 0)
                {
                    v.error = true;
                    v.text = String.Format("Could not get title access token "
                        + "from the Social Club. Error code: \n{0}/{1}",
                        navGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@Code", "[unknown]"),
                        navGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Error']/@CodeEx", "[unknown]")
                        );
                    v.status = -1;
                    return v;
                }
                else
                {
                    v.text = navGetOrDefault(nav, "//*[local-name()='Response']/*[local-name()='Result']", "");
                    v.error = v.text == "";
                    v.status = (int)res.StatusCode;
                    return v;
                }
            }
            return new validateResponse();
        }

        private static string EntitlementDecrypt(string encBlock)
        {
            byte[] blob = atob(encBlock);
            var blobseg = new ArraySegment<byte>(blob, 4, blob.Length - 4);
            byte[] decBlock = EntitlementBlockCipher.decrypt_n(blobseg.ToArray(), blobseg.Count / 16);
            return Encoding.UTF8.GetString(decBlock);
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

        private static string navGetOrDefault(XPathNavigator x, string p, string d)
        {
            return x.SelectSingleNode(p) != null ? x.SelectSingleNode(p).Value : d;
        }
        private static int navGetOrDefault(XPathNavigator x, string p, int d)
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