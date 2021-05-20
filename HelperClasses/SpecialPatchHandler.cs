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

        /// <summary>
        /// Bool determining whether or not the patcher runs/whether patches are applied
        /// </summary>
        public static bool patcherEnabled { get; set; } = false;

        /// <summary>
        /// The blob for EMU
        /// </summary>
        public static byte[] patchBlob
        {
            get
            {
                if (patchesUpdated)
                {
                    generatePatchBlob();
                    patchesUpdated = false;
                }
                if (patcherEnabled)
                {
                    return _patchBlob;
                }
                else
                {
                    return BitConverter.GetBytes(0);
                }
                
            }
        }
        private static byte last_sequence = 0;
        private static Dictionary<string, patch> activePatches = new Dictionary<string, patch>();
        private static Dictionary<string, patch> patches = new Dictionary<string, patch>();

        private static void generatePatchBlob()
        {
            List<patch> patches = new List<patch>();
            patches.AddRange(activePatches.Values);
            patches = patches.OrderBy(x => x.RVA).ToList();
            List<byte> patchBlobTemp = new List<byte>();
            patchBlobTemp.AddRange(BitConverter.GetBytes(activePatches.Count));
            foreach (var i in patches)
            {
                patchBlobTemp.AddRange(BitConverter.GetBytes(i.RVA));
                patchBlobTemp.Add(i.sequence);
                UInt16 datasize = (UInt16)i.content.Length;
                patchBlobTemp.AddRange(BitConverter.GetBytes(datasize));
                patchBlobTemp.AddRange(i.content);
            }
            _patchBlob = patchBlobTemp.ToArray();
        }
        private struct patch
        {
            public UInt32 RVA;
            public byte[] content;
            public byte sequence;
        }

        /// <summary>
        /// Adds a patch
        /// </summary>
        /// <param name="name">Name of the new patch</param>
        /// <param name="RVA">RVA of the patch</param>
        /// <param name="content">Content of the patch</param>
        /// <returns>Bool indicating creation success</returns>
        public static bool addPatch(string name, UInt32 RVA, byte[] content)
        {
            try
            {
                patches.Add(name, new patch
                {
                    RVA = RVA,
                    content = content,
                    sequence = ++last_sequence
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes/Deletes a patch
        /// </summary>
        /// <param name="name">Name of the patch to remove</param>
        /// <returns>Bool indicating removal success</returns>
        public static bool removePatch(string name)
        {
            return patches.Remove(name);
        }

        /// <summary>
        /// Enables a certain patch
        /// </summary>
        /// <param name="name">Name of patch to enable</param>
        /// <returns>Bool indicating success</returns>
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

        /// <summary>
        /// Disables a certain patch
        /// </summary>
        /// <param name="name">Name of patch to disable</param>
        /// <returns>Bool indicating success</returns>
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

        /// <summary>
        /// Checks if a certain patch exists and is enabled
        /// </summary>
        /// <param name="name">Patch Name</param>
        /// <returns>Bool indicating whether patch exists and is enabled</returns>
        public static bool isPatchEnabled(string name)
        {
            return activePatches.ContainsKey(name);
        }

        /// <summary>
        /// Disables/Deactivates all patches
        /// </summary>
        public static void disableAll()
        {
            patchesUpdated = true;
            activePatches.Clear();
        }

        /// <summary>
        /// Enables/Activates all patches
        /// </summary>
        public static void enableAll()
        {
            patchesUpdated = true;
            activePatches = patches.ToDictionary(e => e.Key, e => e.Value);
        }

        /// <summary>
        /// Checks if any patches are enabled
        /// </summary>
        /// <returns>Bool indicating if any patches are enabled</returns>
        public static bool patchesEnabled()
        {
            return activePatches.Count > 0;
        }

        /// <summary>
        /// Returns a list of available patches
        /// </summary>
        /// <returns>List of patch names</returns>
        public static List<string> getPatches()
        {
            return new List<string>(patches.Keys);
        }

        /// <summary>
        /// Returns a tuple of patch info
        /// </summary>
        /// <param name="name">Name of the patch</param>
        /// <returns>Tuple of patch info (RVA, Content)</returns>
        public static Tuple<UInt64, Byte[]> getPatchInfo(string name)
        {
            if (patches.ContainsKey(name))
            {
                return new Tuple<UInt64, Byte[]>(patches[name].RVA, patches[name].content);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns status of all patches
        /// </summary>
        /// <returns>List of patch statuses (name, active)(</returns>
        public static List<Tuple<string,bool>> getPatchStatus()
        {
            var output = new List<Tuple<string, bool>>();
            foreach (var i in patches.Keys)
            {
                output.Add(new Tuple<string, bool>(i, activePatches.ContainsKey(i)));
            }
            return output;
        }
    }
}
