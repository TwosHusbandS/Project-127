using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
    /// <summary>
    /// Class to handle pointer paths similar to those in autosplitter
    /// </summary>
    public class ASPointerPath
    {
        /// <summary>
        /// Name of the base module
        /// </summary>
        public string BaseModuleName
        {
            get;
            private set;
        }

        private Process _baseprocess;
        private Process BaseProcess
        {
            get
            {
                try
                {
                    return Process.GetProcessesByName("GTA5")[0];
                }
                catch
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Indicates whether or not the target process could be found
        /// </summary>
        public bool processFound
        {
            get
            {
                return (BaseProcess != null);
            }
        }

        private IntPtr _prochandle = IntPtr.Zero;

        private IntPtr prochandle
        {
            get
            {
                if (_prochandle != IntPtr.Zero && BaseProcess.Id == _baseprocess.Id)
                {
                    return _prochandle;
                }
                else if (BaseProcess != null || BaseProcess.Id != _baseprocess.Id)
                {
                    _baseprocess = BaseProcess;
                    _prochandle = OpenProcess(ProcessAccessFlags.QueryInformation | 
                        ProcessAccessFlags.VirtualMemoryRead |
                        ProcessAccessFlags.VirtualMemoryWrite |
                        ProcessAccessFlags.VirtualMemoryOperation,
                        false, BaseProcess.Id);
                    return _prochandle;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }


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
       
        /// <summary>
        /// Constructor for ASPointerPath
        /// </summary>
        /// <param name="BaseModuleName">Base module name; used for locating the process</param>
        public ASPointerPath(string BaseModuleName)
        {
            this.BaseModuleName = BaseModuleName;
        }

        private IntPtr getModuleBase(string modulename)
        {
            var modules = BaseProcess.Modules;
            foreach (ProcessModule module in modules)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(module.ModuleName) == modulename)
                {
                    return module.BaseAddress;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Evaluates a pointer path
        /// </summary>
        /// <param name="sz">Number of bytes to read</param>
        /// <param name="path">Pointer path</param>
        /// <returns>The requested bytes</returns>
        public byte[] EvalPointerPath (int sz, IList<int> path)
        {
            return EvalPointerPath(BaseModuleName, sz, path);
            
        }

        /// <summary>
        /// Evaluates a pointer path
        /// </summary>
        /// <param name="modulename">Name of the target module</param>
        /// <param name="sz">Number of bytes to read</param>
        /// <param name="path">Pointer path</param>
        /// <returns>The requested bytes</returns>
        public byte[] EvalPointerPath(string modulename, int sz, IList<int> path)
        {
            //getBase addres
            IntPtr cbase = getModuleBase(modulename);
            if (cbase == IntPtr.Zero || prochandle == IntPtr.Zero)
            {
                return null;
            }
            
            foreach (var offset in path)
            {
                var addr = IntPtr.Add(cbase, offset);
                if (offset == path.Last())
                {
                    byte[] outp = new byte[sz];
                    int read = 0;
                    ReadProcessMemory(prochandle, addr, outp, sz, ref read);
                    return outp;
                }
                else
                {
                    var nbase = new byte[8];
                    int read = 0;
                    var stat = ReadProcessMemory(prochandle, addr, nbase, 8, ref read);
                    cbase = (IntPtr)BitConverter.ToUInt64(nbase, 0);
                }
            }
            return null;
        }

        /// <summary>
        /// Evaluates a pointer path (Int32)
        /// </summary>
        /// <param name="path">Pointer path</param>
        /// <returns>Int32 value at the given pointer path</returns>
        public Int32 EvalPointerPath_I32(IList<int> path)
        {
            return BitConverter.ToInt32(EvalPointerPath(sizeof(Int32), path), 0);
        }

        /// <summary>
        /// Evaluates a pointer path (Int32)
        /// </summary>
        /// <param name="modulename">Name of the target module</param>
        /// <param name="path">Pointer path</param>
        /// <returns>Int32 value at the given pointer path</returns>
        public Int32 EvalPointerPath_I32(string modulename, IList<int> path)
        {
            return BitConverter.ToInt32(EvalPointerPath(modulename, sizeof(Int32), path), 0);
        }

        /// <summary>
        /// Evaluates a pointer path (float32)
        /// </summary>
        /// <param name="path">Pointer path</param>
        /// <returns>float32 value at the given pointer path</returns>
        public float EvalPointerPath_fp32(IList<int> path)
        {
            return BitConverter.ToSingle(EvalPointerPath(sizeof(float), path), 0);
        }

        /// <summary>
        /// Evaluates a pointer path (float32)
        /// </summary>
        /// <param name="modulename">Name of the target module</param>
        /// <param name="path">Pointer path</param>
        /// <returns>float32 value at the given pointer path</returns>
        public float EvalPointerPath_fp32(string modulename, IList<int> path)
        {
            return BitConverter.ToSingle(EvalPointerPath(modulename, sizeof(float), path), 0);
        }

    }
}
