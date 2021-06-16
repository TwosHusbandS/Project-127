using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

        private Process _baseprocess = null;

        private bool procChanged = false;
        private Process BaseProcess
        {
            get
            {
                if (_baseprocess == null || _baseprocess.HasExited)
                {
                    procChanged = true;
                    _baseprocess = null;
                    try
                    {
                        _baseprocess = Process.GetProcessesByName("GTA5")[0];
                    }
                    catch
                    {
                        return null;
                    }
                }
                return _baseprocess;
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
                if (_prochandle != IntPtr.Zero && !procChanged)
                {
                    return _prochandle;
                }
                else if (BaseProcess != null || procChanged)
                {
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

        private Dictionary<string, IntPtr> baseTable = new Dictionary<string, IntPtr>();

        private IntPtr getModuleBase(string modulename)
        {
            if (BaseProcess == null)
            {
                throw new Exception();
            }
            var modules = BaseProcess.Modules;
            if (modules.Count != baseTable.Count)
            {
                baseTable.Clear();
                foreach (ProcessModule module in modules)
                {
                    baseTable.Add(System.IO.Path.GetFileNameWithoutExtension(module.ModuleName), module.BaseAddress);
                    baseTable.Add(module.ModuleName, module.BaseAddress);
                }
            }
            IntPtr modBase;
            baseTable.TryGetValue(modulename, out modBase);
            return modBase;
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

        public static List<pointerPath> pointerPathParse(string file)
        {
            var pathRGX = new Regex(@"([a-zA-Z]+[0-9]*)\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*(?:""([^""]+)""\s*,\s*)?((?:(?:(?:0x[a-zA-Z0-9]+)|(?:0b[01]+)|(?:[0-9]+))\s*,\s*)*(?:(?:0x[a-zA-Z0-9]+)|(?:0b[01]+)|(?:[0-9]+))\s*);\s*");
            var mlcRGX = new Regex(@"/\*.*?\*/");
            var numRGX = new Regex(@"(?:(?:0x[a-zA-Z0-9]+)|(?:0b[01]+)|(?:[0-9]+))");
            var lines = file.Split('\n');
            var commentProcessedLines = new List<string>();
            foreach(var line in lines)
            {
                commentProcessedLines.Add(line.Split(new string[] { "//" }, StringSplitOptions.None)[0]);
            }
            var fileNoComments = string.Join("\n", commentProcessedLines.ToArray());
            fileNoComments = mlcRGX.Replace(fileNoComments, "");
            var pointerPaths = pathRGX.Matches(fileNoComments);
            var output = new List<pointerPath>();
            foreach (Match pp in pointerPaths)
            {
                var ppo = new pointerPath();
                ppo.Type = pp.Groups[1].ToString();
                ppo.Name = pp.Groups[2].ToString();
                ppo.BaseModule = Regex.Unescape(pp.Groups[3].ToString());
                string offsetlist = pp.Groups[4].ToString();
                var offsetsstring = numRGX.Matches(offsetlist);
                var offsets = new List<int>();
                foreach (Match num in offsetsstring)
                {
                    if (num.Value.Substring(0,2) == "0x")
                    {
                        offsets.Add(Convert.ToInt32(num.Value.Substring(2), 16));
                    }
                    else if (num.Value.Substring(0, 2) == "0b")
                    {
                        offsets.Add(Convert.ToInt32(num.Value.Substring(2), 2));
                    }
                    else
                    {
                        offsets.Add(Convert.ToInt32(num.Value.Substring(2)));
                    }
                }
                ppo.offsets = offsets;
                output.Add(ppo);
            }
            return output;
        }

        public class pointerPath
        {
            public pointerPath() { }

            public string Name { get; set; }

            public string Type
            {
                get
                {
                    return subtype+(subtypemax != null? subtypemax.ToString() : "");
                }
                set
                {
                    var typeRGX = new Regex("([a-z]+)([0-9]*)");
                    var match = typeRGX.Match(value);
                    subtype = match.Groups[1].ToString();
                    if (match.Groups[2].ToString() != "")
                    {
                        subtypemax = Convert.ToInt32(match.Groups[2].ToString());
                    }
                    else
                    {
                        subtypemax = null;
                    }
                }
            }
            public Type resultantType {
                get
                {
                    return stringToTypeMapping(subtype);
                }
            }

            private static Type stringToTypeMapping(string typename)
            {
                switch (typename)
                {
                    case "sbyte":
                        return typeof(sbyte);
                    case "byte":
                        return typeof(byte);
                    case "short":
                        return typeof(short);
                    case "ushort":
                        return typeof(ushort);
                    case "int":
                        return typeof(int);
                    case "uint":
                        return typeof(uint);
                    case "long":
                        return typeof(long);
                    case "ulong":
                        return typeof(ulong);
                    case "float":
                        return typeof(float);
                    case "double":
                        return typeof(double);
                    case "bool":
                        return typeof(bool);
                    case "string":
                        return typeof(string);
                    case "bytearray":
                        return typeof(Byte[]);
                    default:
                        return typeof(void);
                }
            }

            private static object autoConverterMapping(byte[] raw, string type)
            {
                switch (type)
                {
                    case "sbyte":
                        return (sbyte)raw[0];
                    case "byte":
                        return raw[0];
                    case "short":
                        return BitConverter.ToInt16(raw, 0);
                    case "ushort":
                        return BitConverter.ToUInt16(raw, 0);
                    case "int":
                        return BitConverter.ToInt32(raw, 0);
                    case "uint":
                        return BitConverter.ToUInt32(raw, 0);
                    case "long":
                        return BitConverter.ToInt64(raw, 0);
                    case "ulong":
                        return BitConverter.ToUInt64(raw, 0);
                    case "float":
                        return BitConverter.ToSingle(raw, 0);
                    case "double":
                        return BitConverter.ToDouble(raw, 0);
                    case "bool":
                        return BitConverter.ToBoolean(raw, 0);
                    case "string":
                        return Encoding.UTF8.GetString(raw.TakeWhile(a => a != '\0').ToArray());
                    default:
                        return raw;
                }
            }

            private string subtype;
            private int? subtypemax;

            private string baseModule = null;

            public string BaseModule
            {
                get
                {
                    return baseModule;
                }
                set
                {
                    if (value != "")
                    {
                        baseModule = value;
                    }
                    else
                    {
                        baseModule = null;
                    }
                }
            }

            public List<int> offsets { get; set; }

            public Tuple<Type,object> evaluate(ASPointerPath PathEvaluator = null)
            {
                if (PathEvaluator == null)
                {
                    try
                    {
                        PathEvaluator = Globals.GTAPointerPathHandler;
                    }
                    catch
                    {
                        return null;
                    }
                }
                var typeTarget = resultantType;
                byte[] resultRaw;
                int typeSize;
                if (typeTarget != typeof(string) && typeTarget != typeof(Byte[]))
                {
                    typeSize = Marshal.SizeOf(typeTarget);
                }
                else
                {
                    typeSize = subtypemax ?? 255;
                }
                
                if (BaseModule == null)
                {
                    resultRaw = PathEvaluator.EvalPointerPath(typeSize, offsets);
                }
                else
                {
                    resultRaw = PathEvaluator.EvalPointerPath(BaseModule, typeSize, offsets);
                }

                return new Tuple<Type, object>(typeTarget, autoConverterMapping(resultRaw, subtype));
            }
        }

    }
}
