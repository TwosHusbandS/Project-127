using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
//using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Documents;

/*
 * This file is based on SCUIStub from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# by @dr490n/@jaredtb  
 */
public class ROSCommunicationBackend
{
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
            sessKey = new ArraySegment<byte>(keyData, 0, 16).Array;

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
            hashData = new HMACSHA1().ComputeHash(tempContent.ToArray());
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
        byte[] blockSizeData = RC4.Decrypt(rc4Key, new ArraySegment<byte>(data, 16, 4).Array);

        // Sadly, bitconverter doesn't have an explicit from Big Endian
        int blockSize = (blockSizeData[0] << 24) + (blockSizeData[1] << 16) +
            (blockSizeData[2] << 8) + blockSizeData[1];

        byte[] fullData = RC4.Decrypt(rc4Key, data);
        List<byte> result = new List<byte>();

        // Block handling:
        int start = 20;
        while (start < size)
        {
            // Find the end
            int end = Math.Min(size, start + blockSize);

            // We ignore the hash at the end of blocks (the decryption corrupted it anyways)
            end -= 20; 

            int len = end - start;

            // Grab deciphered block
            ArraySegment<byte> block = new ArraySegment<byte>(fullData, start, len);

            // Push to result
            result.AddRange(block.Array);

            // Move the start for next iteration
            start += blockSize;
        }

        return result.ToArray();

    }


    private class ROSCryptoState
    {
        private byte[] m_rc4Key;
        private byte[] m_xorKey;
        private byte[] m_hashKey;
        public ROSCryptoState()
        {
            byte[] platformStr = atob(ROS_PLATFORM_KEY_LAUNCHER);
            m_rc4Key = new ArraySegment<byte>(platformStr,1,32).Array;
            m_xorKey = new ArraySegment<byte>(platformStr, 33, 16).Array;
            m_hashKey = new ArraySegment<byte>(platformStr, 49, 16).Array;

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
}
