using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_127
{
    class EntitlementBlockCipher
    {
        private static UInt32[] entitlement_key_dec;
        private static UInt32[] entitlement_tables_dec;

        private static void SetupEntitlementTables()
        {
            if (entitlement_tables_dec == null)
            {
                var aes = new AesManaged();
                aes.KeySize = 256;
                aes.Key = System.Convert.FromBase64String("evXOTgIkGJ6xoPeHCmo8drsQFB3CaAHqN9FC2W/2JnU=");
                aes.IV = System.Convert.FromBase64String("WOdCEkBR10NK5+Dbv8icIw==");
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                var tableStream = new MemoryStream(EntitlementTables.entitlement_tables);

                var decryptor = new CryptoStream(tableStream, aes.CreateDecryptor(), CryptoStreamMode.Read);

                var decryptorBR = new BinaryReader(decryptor);

                List<UInt32> ent_tbl = new List<UInt32>(EntitlementTables.entitlement_tables.Length / 4);
                for (int i = 0; i < (EntitlementTables.entitlement_tables.Length / 4); i++)
                {
                    ent_tbl.Add(decryptorBR.ReadUInt32());
                }

                entitlement_tables_dec = ent_tbl.ToArray();

                var keyStream = new MemoryStream(EntitlementTables.entitlement_key);

                decryptor = new CryptoStream(keyStream, aes.CreateDecryptor(), CryptoStreamMode.Read);

                decryptorBR = new BinaryReader(decryptor);

                List<UInt32> ent_key = new List<UInt32>(EntitlementTables.entitlement_key.Length / 4);
                for (int i = 0; i < (EntitlementTables.entitlement_key.Length / 4); i++)
                {
                    ent_key.Add(decryptorBR.ReadUInt32());
                }

                entitlement_key_dec = ent_key.ToArray();
            }
        }
        public int block_size()
        {
            return 16;
        }
        private static UInt32[] GetSubkey(int idx)
        {
            UInt32[] keyUints = entitlement_key_dec;
            return new ArraySegment<UInt32>(keyUints,4*idx, 4).Array;
        } 
        private static UInt32[] GetDecryptTable(int idx)
        {
            UInt32[] dtUints = entitlement_tables_dec;
            return new ArraySegment<UInt32>(dtUints, 16*256*idx, 16*256).Array;
        }
        private static UInt32[] GetDecryptBytes(UInt32[] tables, int idx) 
        {
            return new ArraySegment<UInt32>(tables, 256*idx, 256).Array;
        }
        public static byte[] decrypt_n(byte[] inp, int blocks)
        {
            SetupEntitlementTables();

            byte[] outBuf;
            List<byte> output = new List<byte>(16 * blocks);

            for (int i = 0; i < blocks; i++)
            {
                byte[] inBuf = new ArraySegment<byte>(inp, 16 * i, 16).Array;

                outBuf = DecryptRoundA(inBuf, GetSubkey(0), GetDecryptTable(0));
                outBuf = DecryptRoundA(outBuf, GetSubkey(1), GetDecryptTable(1));

                for (int j = 2; j <= 15; j++)
                {
                    outBuf = DecryptRoundB(outBuf, GetSubkey(j), GetDecryptTable(j));
                }

                outBuf = DecryptRoundA(outBuf, GetSubkey(16), GetDecryptTable(16));

                output.AddRange(outBuf);
            }
            return output.ToArray();
        }

        private static byte[] DecryptRoundA(byte[] inp, UInt32[] key, UInt32[] table)
        {
            List<byte> outp = new List<byte>(16);
            UInt32 x1 = GetDecryptBytes(table, 0)[inp[0]] ^
                GetDecryptBytes(table, 1)[inp[1]] ^
                GetDecryptBytes(table, 2)[inp[2]] ^
                GetDecryptBytes(table, 3)[inp[3]] ^
                key[0];
            UInt32 x2 = GetDecryptBytes(table, 4)[inp[4]] ^
                GetDecryptBytes(table, 5)[inp[5]] ^
                GetDecryptBytes(table, 6)[inp[6]] ^
                GetDecryptBytes(table, 7)[inp[7]] ^
                key[1];

            UInt32 x3 = GetDecryptBytes(table, 8)[inp[8]] ^
                GetDecryptBytes(table, 9)[inp[9]] ^
                GetDecryptBytes(table, 10)[inp[10]] ^
                GetDecryptBytes(table, 11)[inp[11]] ^
                key[2];

            UInt32 x4 = GetDecryptBytes(table, 12)[inp[12]] ^
                GetDecryptBytes(table, 13)[inp[13]] ^
                GetDecryptBytes(table, 14)[inp[14]] ^
                GetDecryptBytes(table, 15)[inp[15]] ^
                key[3];

            outp.AddRange(BitConverter.GetBytes(x1));
            outp.AddRange(BitConverter.GetBytes(x2));
            outp.AddRange(BitConverter.GetBytes(x3));
            outp.AddRange(BitConverter.GetBytes(x4));

            return outp.ToArray();
        }
        private static byte[] DecryptRoundB(byte[] inp, UInt32[] key, UInt32[] table)
        {
            UInt32 x1 = GetDecryptBytes(table, 0)[inp[0]] ^
                GetDecryptBytes(table, 7)[inp[7]] ^
                GetDecryptBytes(table, 10)[inp[10]] ^
                GetDecryptBytes(table, 13)[inp[13]] ^
                key[0];

            UInt32 x2 = GetDecryptBytes(table, 1)[inp[1]] ^
                GetDecryptBytes(table, 4)[inp[4]] ^
                GetDecryptBytes(table, 11)[inp[11]] ^
                GetDecryptBytes(table, 14)[inp[14]] ^
                key[1];

            UInt32 x3 = GetDecryptBytes(table, 2)[inp[2]] ^
                GetDecryptBytes(table, 5)[inp[5]] ^
                GetDecryptBytes(table, 8)[inp[8]] ^
                GetDecryptBytes(table, 15)[inp[15]] ^
                key[2];

            UInt32 x4 = GetDecryptBytes(table, 3)[inp[3]] ^
                GetDecryptBytes(table, 6)[inp[6]] ^
                GetDecryptBytes(table, 9)[inp[9]] ^
                GetDecryptBytes(table, 12)[inp[12]] ^
                key[3];

            byte[] outp = new byte[16];

            outp[0] = (byte)((x1 >> 0) & 0xFF);
            outp[1] = (byte)((x1 >> 8) & 0xFF);
            outp[2] = (byte)((x1 >> 16) & 0xFF);
            outp[3] = (byte)((x1 >> 24) & 0xFF);
            outp[4] = (byte)((x2 >> 0) & 0xFF);
            outp[5] = (byte)((x2 >> 8) & 0xFF);
            outp[6] = (byte)((x2 >> 16) & 0xFF);
            outp[7] = (byte)((x2 >> 24) & 0xFF);
            outp[8] = (byte)((x3 >> 0) & 0xFF);
            outp[9] = (byte)((x3 >> 8) & 0xFF);
            outp[10] = (byte)((x3 >> 16) & 0xFF);
            outp[11] = (byte)((x3 >> 24) & 0xFF);
            outp[12] = (byte)((x4 >> 0) & 0xFF);
            outp[13] = (byte)((x4 >> 8) & 0xFF);
            outp[14] = (byte)((x4 >> 16) & 0xFF);
            outp[15] = (byte)((x4 >> 24) & 0xFF);

            return outp;
        }
        public static string name()  
        {
            return "RAEA(tm) (c) OpenIV-Putin-Team";
        }
    }
}
