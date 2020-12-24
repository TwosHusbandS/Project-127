using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Auth
{
    class MTLAuth
    {
        [DllImport("MTLInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool retrieveMtlData(byte[] ticket, byte[] sessticket,
            byte[] sessionKey, byte[] rockstarNick, byte[] countryCode,
            ref Int32 posixtime, ref UInt64 rockstarID);
        public static ROSCommunicationBackend.sessionContainer GetMTLSession()
        {
            byte[] ticket = new byte[256], sessionTicket = new byte[128], 
                rockstarNick = new byte[32], countryCode = new byte[4], 
                sessKey = new byte[16];
            Int32 posixtime = new Int32();
            UInt64 RockstarID = new UInt64();
            var success = retrieveMtlData(ticket, sessionTicket, sessKey,
                rockstarNick, countryCode, ref posixtime, ref RockstarID);
            if (!success)
            {
                return new ROSCommunicationBackend.sessionContainer(null, null,
                    null, null, 0, null);
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
            return new ROSCommunicationBackend.sessionContainer(
                Encoding.UTF8.GetString(ticket).TrimEnd('\0'),
                Convert.ToBase64String(sessKey).TrimEnd('\0'),
                Encoding.UTF8.GetString(sessionTicket).TrimEnd('\0'),
                machineHash,
                posixtime,
                Encoding.UTF8.GetString(rockstarNick).TrimEnd('\0')
                );
        }
    }
}
