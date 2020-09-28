using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
/*
 * This file is based on LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# by @dr490n/@jaredtb  
 */
namespace Project_127
{
    class EntitlementBlockCipher
    {

        // Stub to call dll
        [DllImport("ROSCrypto.dll", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool fnEntitlementDecrypt(byte[] input, int length);

        public static byte[] DecryptEntitlementBlock(byte[] data)
        {
            if (fnEntitlementDecrypt(data, data.Length))
            {
                return data.Skip(20).ToArray();
            } else
            {
                return null;
            }
        }
    }
}
