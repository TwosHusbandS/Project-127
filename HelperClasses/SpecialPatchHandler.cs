using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Project_127.HelperClasses
{
    public class SpecialPatchHandler
    {
        private static bool patchesUpdated = true;
        private static byte[] _patchBlob = null;

        /// <summary>
        /// Bool determining whether or not the patcher runs/whether patches are applied
        /// </summary>
        public static bool PatcherEnabled
        {
            get
            {
                return MySettings.Settings.SpecialPatcherEnabled && patcherEnabled;
            }
            set
            {
                patcherEnabled = value;
            }
        }

        /// <summary>
        /// Toggles the patcher
        /// </summary>
        public static void patcherToggle()
        {
            patcherEnabled = !patcherEnabled;
        }

        private static bool patcherEnabled = true;

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
                if (PatcherEnabled)
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
        private static Dictionary<string, cachedPatch> activePatches = new Dictionary<string, cachedPatch>();
        private static Dictionary<string, cachedPatch> cachedPatches = new Dictionary<string, cachedPatch>();

        private static void generatePatchBlob()
        {
            List<cachedPatch> patches = new List<cachedPatch>();
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
        private struct cachedPatch
        {
            public UInt32 RVA;
            public byte[] content;
            public byte sequence;
        }

        /// <summary>
        /// Adds a patch to the cache
        /// </summary>
        /// <param name="name">Name of the new patch</param>
        /// <param name="RVA">RVA of the patch</param>
        /// <param name="content">Content of the patch</param>
        /// <returns>Bool indicating creation success</returns>
        internal static bool cachePatch(string name, UInt32 RVA, byte[] content)
        {
            try
            {
                cachedPatches.Add(name, new cachedPatch
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
        /// Adds a patch to the cache
        /// </summary>
        /// <param name="p">Patch to add</param>
        /// <returns>Bool indicating creation success</returns>
        public static bool cachePatch(patch p)
        {
            return cachePatch(p.Name, p.RVA, p.Content);
        }

        /// <summary>
        /// Removes a patch from cache
        /// </summary>
        /// <param name="name">Name of the patch to remove</param>
        /// <returns>Bool indicating removal success</returns>
        internal static bool removeCachedPatch(string name)
        {
            if (isPatchEnabled(name))
            {
                disableCachedPatch(name);
            }
            return cachedPatches.Remove(name);
        }

        /// <summary>
        /// Enables a certain cached patch
        /// </summary>
        /// <param name="name">Name of patch to enable</param>
        /// <returns>Bool indicating success</returns>
        public static bool enableCachedPatch(string name)
        {
            patchesUpdated = true;
            if (cachedPatches.ContainsKey(name) && !activePatches.ContainsKey(name))
            {
                activePatches.Add(name, cachedPatches[name]);
                return true;
            }
            else if (activePatches.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disables a certain cached patch
        /// </summary>
        /// <param name="name">Name of patch to disable</param>
        /// <returns>Bool indicating success</returns>
        public static bool disableCachedPatch(string name)
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
        /// Checks if a certain patch exists in cache and is enabled
        /// </summary>
        /// <param name="name">Patch Name</param>
        /// <returns>Bool indicating whether patch exists and is enabled</returns>
        public static bool isPatchEnabled(string name)
        {
            return activePatches.ContainsKey(name);
        }

        internal static bool isPatchCached(string name)
        {
            return cachedPatches.ContainsKey(name);
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
        /// Enables/Activates all cached patches
        /// </summary>
        public static void enableAllCached()
        {
            patchesUpdated = true;
            activePatches = cachedPatches.ToDictionary(e => e.Key, e => e.Value);
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
        /// Returns a list of cached patches
        /// </summary>
        /// <returns>List of patch names</returns>
        public static List<string> getCachedPatches()
        {
            return new List<string>(cachedPatches.Keys);
        }

        /// <summary>
        /// Returns a tuple of patch info for a cached patch
        /// </summary>
        /// <param name="name">Name of the patch</param>
        /// <returns>Tuple of patch info (RVA, Content)</returns>
        public static Tuple<UInt64, Byte[]> getPatchInfo(string name)
        {
            if (cachedPatches.ContainsKey(name))
            {
                return new Tuple<UInt64, Byte[]>(cachedPatches[name].RVA, cachedPatches[name].content);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Returns status of all cached patches
        /// </summary>
        /// <returns>List of patch statuses (name, active)(</returns>
        public static List<Tuple<string,bool>> getCachedPatchStatus()
        {
            var output = new List<Tuple<string, bool>>();
            foreach (var i in cachedPatches.Keys)
            {
                output.Add(new Tuple<string, bool>(i, activePatches.ContainsKey(i)));
            }
            return output;
        }

        public class patch
        {

            private static JavaScriptSerializer json = new JavaScriptSerializer();
            /// <summary>
            /// Contructs and empty patch object
            /// </summary>
            public patch() { }

            /// <summary>
            /// Constructs a patch object
            /// </summary>
            /// <param name="Name">Patch Name</param>
            /// <param name="RVA">Patch RVA</param>
            /// <param name="KeyBind">Patch KeyBind</param>
            /// <param name="DefaultEnabled">Patch Default Enable Status</param>
            /// <param name="Content">Patch Content</param>
            public patch(string Name, UInt32 RVA, System.Windows.Forms.Keys KeyBind, bool DefaultEnabled, byte[] Content)
            {
                this.Name = Name;
                this.RVA = RVA;
                this.KeyBind = KeyBind;
                this.DefaultEnabled = DefaultEnabled;
                this.Content = Content;
            }

            /// <summary>
            /// JSON CONSTRUCTOR
            /// </summary>
            private patch(jsonSerialPatch p, bool use_enabled = true)
            {
                this.Name = p.Name;
                this.RVA = p.RVA;
                this.KeyBind = (System.Windows.Forms.Keys)p.KeyBind;
                this.DefaultEnabled = p.DefaultEnabled;
                this.Content = p.Content;
                if (use_enabled)
                {
                    this.Enabled = p.Enabled;
                }
            }

            /// <summary>
            /// Patch KeyBind
            /// </summary>
            public System.Windows.Forms.Keys KeyBind { get; protected set; } = System.Windows.Forms.Keys.None;

            /// <summary>
            /// Patch KeyBind as string
            /// </summary>
            public string stringKeyBind
            {
                get
                {
                    return KeyBind.ToString();
                }
            }

            /// <summary>
            /// Bool indicating whether patch is default enabled
            /// </summary>
            public bool DefaultEnabled { get; protected set; } = false;

            /// <summary>
            /// Patch Name
            /// </summary>
            public string Name { get; protected set; } = "";

            /// <summary>
            /// Patch RVA
            /// </summary>
            public UInt32 RVA { get; protected set; }

            /// <summary>
            /// Patch RVA as hex string
            /// </summary>
            public string hexRVA
            {
                get
                {
                    return RVA.ToString("X").ToLower();
                }
            }

            /// <summary>
            /// Patch Content
            /// </summary>
            public byte[] Content { get; protected set; }

            /// <summary>
            /// Patch content as hex string
            /// </summary>
            public string hexContent { 
                get
                {
                    if (Content != null) 
                    {
                        return BitConverter.ToString(Content).Replace("-","").ToLower();
                    }
                    else
                    {
                        return "";
                    }
                }
                    
            }

            public bool Enabled
            {
                get
                {
                    return isPatchEnabled(Name);
                }
                set
                {
                    if (value)
                    {
                        if (!isPatchCached(Name))
                        {
                            cachePatch(this);
                        }
                        enableCachedPatch(Name);
                    }
                    else
                    {
                        disableCachedPatch(Name);
                    }
                }
            }

            private static Dictionary<string,patch> patches
            {
                get
                {
                    var output = new Dictionary<string, patch>();
                    try
                    {
                        var jsonpatches = json.Deserialize<List<jsonSerialPatch>>(MySettings.Settings.SpecialPatcherPatches);
                        foreach (var p in jsonpatches)
                        {
                            if (!isPatchCached(p.Name))
                            {
                                output.Add(p.Name, new patch(p));
                            }
                            else
                            {
                                output.Add(p.Name, new patch(p, false));
                            }
                            
                        }
                    }
                    catch { }
                    return output;
                }
                set
                {
                    var jsonpatches = new List<jsonSerialPatch>();
                    foreach (var p in value.Values)
                    {
                        jsonpatches.Add(p.toJsonSerialPatch());
                    }
                    MySettings.Settings.SpecialPatcherPatches = json.Serialize(jsonpatches);
                }
            }

            private jsonSerialPatch toJsonSerialPatch()
            {
                return new jsonSerialPatch
                {
                    Content = Content,
                    DefaultEnabled = DefaultEnabled,
                    Enabled = Enabled,
                    KeyBind = (int)KeyBind,
                    Name = Name,
                    RVA = RVA
                };
            }


            /// <summary>
            /// Gets patch by name
            /// </summary>
            /// <param name="name">Patch Name</param>
            /// <returns>Patch by that name (null if not found)</returns>
            public static patch GetPatch(string name)
            {
                patch p = null;
                patches.TryGetValue(name, out p);
                return p;
            }

            /// <summary>
            /// Gets all patches
            /// </summary>
            /// <returns>List of all patches</returns>
            public static List<patch> GetPatches()
            {
                return patches.Values.ToList();
            }

            private static System.Collections.ObjectModel.ObservableCollection<patch> patchesObservable = null;

            /// <summary>
            /// Observablecollection of all patches
            /// </summary>
            public static System.Collections.ObjectModel.ObservableCollection<patch> PatchesObservable
            {
                get
                {
                    if (patchesObservable == null)
                    {
                        patchesObservable = new System.Collections.ObjectModel.ObservableCollection<patch>(GetPatches());
                    }
                    return patchesObservable;
                }
            }

            /// <summary>
            /// Function to check if a patch by a specified name exits
            /// </summary>
            /// <param name="name">Name to check</param>
            /// <returns>Bool indicating if the patch exists (true = name exists)</returns>
            public static bool nameExists(string name)
            {
                return patches.ContainsKey(name);
            }

            /// <summary>
            /// Function to update patch on changes
            /// </summary>
            public void update()
            {
                if(nameExists(Name)){
                    if (isPatchCached(Name))
                    {
                        removeCachedPatch(Name);
                    }
                    if (DefaultEnabled)
                    {
                        cachePatch(this);
                        enableCachedPatch(this.Name);
                    }
                    var item = patchesObservable.FirstOrDefault(i => i.Name == Name);
                    var idx = patchesObservable.IndexOf(item);
                    patchesObservable[idx] = this;

                }
                else
                {
                    if (patchesObservable != null)
                    {
                        patchesObservable.Add(this);
                    }
                    
                }
                var p = patches;
                p[this.Name] = this;
                patches = p;
                patchKeys = null;
                //new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, JsonSerializer.Serialize<patch>(this)).ShowDialog();
            }

            private static Dictionary<System.Windows.Forms.Keys, List<patch>> patchKeys = null;

            /// <summary>
            /// Dictionary of patch keybinds
            /// </summary>
            public static Dictionary<System.Windows.Forms.Keys, List<patch>> PatchKeys
            {
                get
                {
                    if (patchKeys == null)
                    {
                        patchKeys = new Dictionary<System.Windows.Forms.Keys, List<patch>>();
                        foreach (var p in patches.Values)
                        {
                            if (p.KeyBind != System.Windows.Forms.Keys.None)
                            {
                                if (!patchKeys.ContainsKey(p.KeyBind))
                                {
                                    patchKeys[p.KeyBind] = new List<patch>();
                                    
                                }
                                patchKeys[p.KeyBind].Add(p);
                            }
                        }
                    }
                    return patchKeys;
                }
                
            }

            /// <summary>
            /// Function to delete a patch
            /// </summary>
            /// <param name="name">Name of patch to delete</param>
            /// <returns>Bool indicating deletion success</returns>
            public static bool deletePatch(string name)
            {
                if (!nameExists(name))
                {
                    return false;
                }
                if (isPatchCached(name))
                {
                    removeCachedPatch(name);
                }
                //new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOk, "Not Implemented!").ShowDialog();
                var item = patchesObservable.FirstOrDefault(i => i.Name == name);
                patchesObservable.Remove(item);
                var p = patches;
                p.Remove(name);
                patches = p;
                return true;
            }

            /// <summary>
            /// Clears cached patches.
            /// </summary>
            public static void clearCache()
            {
                var cachedps = cachedPatches.Keys.ToArray();
                foreach(var p in cachedps)
                {
                    removeCachedPatch(p);
                }
            }

            /// <summary>
            /// Forces observable collection reload
            /// </summary>
            public static void reloadObservable()
            {
                if (patchesObservable == null)
                {
                    return;
                }
                patchesObservable.Clear();
                foreach (var patch in GetPatches())
                {
                    patchesObservable.Add(patch);
                }

            }

            public struct jsonSerialPatch
            {
                public string Name;
                public UInt32 RVA;
                public int KeyBind;
                public bool DefaultEnabled;
                public byte[] Content;
                public bool Enabled;
            }
        }

        /// <summary>
        /// Check for existance of scripthook & copy it if necessary
        /// </summary>
        public static void checkCopyScripthook()
        {
            if (!MySettings.Settings.SpecialPatcherEnabled)
            {
                return;
            }
            var targetPath = System.IO.Path.Combine(
                 MySettings.Settings.GTAVInstallationPath,
                "P127_ASMPATCHER_SCRIPTHOOK.dll"
            );
            if (!System.IO.File.Exists(targetPath))
            {
                try
                {
                    System.IO.File.Copy("P127_ASMPATCHER_SCRIPTHOOK.dll", targetPath);
                }
                catch
                {
                    new Popups.Popup(
                        Popups.Popup.PopupWindowTypes.PopupOkError,
                        "Failed to copy scripthook. ASM patcher will not load"
                        ).ShowDialog();
                }
            }
        }
    }
    
}
