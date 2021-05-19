using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
    public class SpecialPatchHandler
    {
        private static bool patchesUpdated = true;
        private static byte[] _patchBlob = null;
        public static byte[] patchBlob
        {
            get
            {
                if (patchesUpdated)
                {
                    generatePatchBlob();
                    patchesUpdated = false;
                }
                return _patchBlob;
            }
        }
        private static byte sequence_number = 0;
        private static Dictionary<string, patch> activePatches = new Dictionary<string, patch>();
        private static Dictionary<string, patch> patches = new Dictionary<string, patch>();
        public static void generatePatchBlob()
        {
            List<patch> patches = new List<patch>();
            patches.AddRange(activePatches.Values);
            patches = patches.OrderBy(x => x.RVA).ToList();
            List<byte> patchBlobTemp = new List<byte>();
            patchBlobTemp.AddRange(BitConverter.GetBytes(activePatches.Count));
            foreach (var i in patches)
            {
                patchBlobTemp.AddRange(BitConverter.GetBytes(i.RVA));
                patchBlobTemp.Add(sequence_number++);
                UInt16 datasize = (UInt16)i.data.Length;
                patchBlobTemp.AddRange(BitConverter.GetBytes(datasize));
                patchBlobTemp.AddRange(i.data);
            }
            _patchBlob = patchBlobTemp.ToArray();
        }
        private struct patch
        {
            public UInt32 RVA;
            public byte[] data;
        }

        public static bool addPatch(string name, UInt32 RVA, byte[] data)
        {
            try
            {
                patches.Add(name, new patch
                {
                    RVA = RVA,
                    data = data
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool enablePatch(string name)
        {
            patchesUpdated = true;
            if (patches.ContainsKey(name))
            {
                activePatches.Add(name, patches[name]);
                return true;
            }
            return false;
        }

        public static bool disablePatch(string name)
        {
            patchesUpdated = true;
            if (activePatches.ContainsKey(name))
            {
                activePatches.Remove(name);
                return true;
            }
            return false;
        }

        public static bool isPatchEnabled(string name)
        {
            return activePatches.ContainsKey(name);
        }

        public static void disableAll()
        {
            activePatches.Clear();
        }

        public static void enableAll()
        {
            activePatches = patches.ToDictionary(e => e.Key, e => e.Value);
        }

        public static bool patchesEnabled()
        {
            return activePatches.Count > 0;
        }
    }
}
