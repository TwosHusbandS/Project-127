using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Auth
{
    class MTLInterface
    {
        private static readonly byte[] match_pattern = { 0x84, 0xC0, 0x75, 0x19, 0xFF, 0xC7, 0x83, 0xFF,
        0x01, 0x7C, 0xD8 };

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
             ProcessAccessFlags processAccess,
             bool bInheritHandle,
             int processId
        );

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static int patternSearch(byte[] pattern, byte[] blob)
        {
            int patternLength = pattern.Length;
            int totalLength = blob.Length;
            byte firstByte = pattern[0];
            byte[] matchHolder = new byte[patternLength];
            for (int i = 0; i < totalLength; i++)
            {
                if (blob[i] == firstByte)
                {
                    Array.Copy(blob, i, matchHolder, 0, patternLength);
                    if (matchHolder.SequenceEqual<byte>(pattern))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static ROSCommunicationBackend.sessionContainer GetMTLSession()
        {
            var launcherProcs = Process.GetProcessesByName("Launcher");
            if (launcherProcs.Length == 0)
            {
                return null;
            }
            Process launcherProcess = launcherProcs[0];
            foreach (var p in launcherProcs)
            {
                if (p.MainModule.FileName == @"C:\Program Files\Rockstar Games\Launcher\Launcher.exe")
                {
                    launcherProcess = p;
                    break;
                }
            }
            var pid = launcherProcess.Id;
            var hProc = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead |
                ProcessAccessFlags.VirtualMemoryWrite | ProcessAccessFlags.VirtualMemoryOperation, false, pid);
            var modules = launcherProcess.Modules;
            ProcessModule socialClub = null;
            foreach (ProcessModule module in modules)
            {
                if (module.ModuleName == "socialclub.dll")
                {
                    socialClub = module;
                }
            }
            if (socialClub == null)
            {
                return null;
            }
            byte[] scmem = new byte[socialClub.ModuleMemorySize];
            int nread = 0;
            ReadProcessMemory(hProc, socialClub.BaseAddress, scmem, socialClub.ModuleMemorySize, ref nread);
            //FindRVA
            var offset = patternSearch(match_pattern, scmem);
            offset -= 35;
            var exeioffset = BitConverter.ToUInt32(scmem, offset);
            var blob_offset = offset + 4 + exeioffset;
            var blob = new ArraySegment<byte>(scmem, (int)blob_offset, 16384);
            //var posixtime = BitConverter.ToInt32(blob.Skip(0x3170).Take(4).ToArray(), 0); //Broklen
            var posixtime = (int)ROSCommunicationBackend.GetPosixTime() + 24 * 3600;
            var RockstarID = BitConverter.ToUInt64(blob.Skip(0xEE8).Take(8).ToArray(), 0);
            var sessKey = blob.Skip(0x1108).Take(16).ToArray();
            var ticket = Encoding.UTF8.GetString(blob.Skip(0xAF0).TakeWhile(a => a != 0).ToArray());
            var sessTicket = Encoding.UTF8.GetString(blob.Skip(0xCF0).TakeWhile(a => a != 0).ToArray());
            var rockstarNick = Encoding.UTF8.GetString(blob.Skip(0xE9F).TakeWhile(a => a != 0).ToArray());
            var countryCode = Encoding.UTF8.GetString(blob.Skip(0xE0C).TakeWhile(a => a != 0).ToArray());

            if (/*posixtime == 0 ||*/ rockstarNick == "")
            {
                return null;
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
                ticket,
                Convert.ToBase64String(sessKey),
                sessTicket,
                machineHash,
                posixtime,
                rockstarNick
                );
            //strcpy((char*)ticket, (char*)(blob + 0xAF0));
            //strcpy((char*)sessticket, (char*)(blob + 0xCF0));
            //strcpy((char*)rockstarNick, (char*)(blob + 0xE9F));
            //strcpy((char*)countryCode, (char*)(blob + 0xE0C));
        }


    }
}
