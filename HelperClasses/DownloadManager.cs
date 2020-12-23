using Microsoft.Win32;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.XPath;

namespace Project_127.HelperClasses
{
	public class DownloadManager
	{
        private XPathNavigator nav;
        private Dictionary<string, XPathNavigator> availableSubassemblies;
        private Dictionary<string, subassemblyInfo> installedSubassemblies;

        private static string Project127Files
        {
            get
            {
                return System.IO.Path.Combine(LauncherLogic.ZIPFilePath, @"Project_127_Files\");
            }
        }

        private void updateInstalled()
        {
            HelperClasses.RegeditHandler.SetValue("DownloadManagerInstalledSubassemblies", json.Serialize(installedSubassemblies));
        }        

        private static JavaScriptSerializer json = new JavaScriptSerializer();

        /// <summary>
        /// Function to install subassemblies
        /// </summary>
        /// <param name="subassemblyName">Name of subassembly to be installed</param>
        /// <param name="reinstall">Determines whether or not reinstall is enabled</param>
        /// <returns>Boolean indicating whether install succeded or not</returns>
        public async Task<bool> getSubassembly(string subassemblyName, bool reinstall = false)
        {
            if (!availableSubassemblies.ContainsKey(subassemblyName))
            {
                return false;
            } 
            else if (installedSubassemblies.ContainsKey(subassemblyName) && !reinstall)
            {
                return true;
            }
            else
            {
                if (reinstall && isInstalled(subassemblyName)) 
                {
                    delSubassembly(subassemblyName, true);
                }
                var s = availableSubassemblies[subassemblyName];
                var subInfo = new subassemblyInfo();
                Version vinfo;
                try
                {
                    vinfo = new Version(s.GetAttribute("version", ""));
                }
                catch
                {
                    vinfo = new Version(0, 0, 0, 1);
                }
                subInfo.version = vinfo.ToString();
                var reqs = s.Select("./requires/subassembly");
                foreach (XPathNavigator req in reqs)
                {
                    await getSubassembly(req.GetAttribute("target", ""));
                }
                if (s.GetAttribute("type", "").ToLower() == "zip")
                {
                    var mirrors = s.Select("./mirror");
                    var succeeded = false;
                    foreach (XPathNavigator zipMirror in mirrors)
                    {
                        var zipdlpath = System.IO.Path.Combine(Globals.ProjectInstallationPath, "dl.zip");
                        string link = zipMirror.Value;
                        new PopupDownload(link, zipdlpath, "Zip").ShowDialog();
                        var zipmd5 = HelperClasses.FileHandling.GetHashFromFile(zipdlpath);
                        succeeded = s.SelectSingleNode("./hash").Value.ToLower() == zipmd5;
                        if (!succeeded)
                        {
                            continue;
                        }
                        new PopupProgress(PopupProgress.ProgressTypes.ZIPFile, zipdlpath).ShowDialog();
						HelperClasses.FileHandling.DeleteFolder(zipdlpath);
                        break;
                    }
                    if (succeeded)
                    {
                        if (installedSubassemblies.ContainsKey(subassemblyName))
                        {
                            installedSubassemblies.Remove(subassemblyName);
                        }
                        installedSubassemblies.Add(subassemblyName, subInfo);
                        updateInstalled();
                        return true;
                    }
                }
                else if (s.GetAttribute("type", "").ToLower() == "common")
                {
                    subInfo.common = true;
                    var cfiles = s.Select("./file");
                    if (reinstall)
                    {
                        var installedFiles = new List<string>();
                        foreach (var file in installedSubassemblies[subassemblyName].files)
                        {
                            installedFiles.Add(file.name);
                            subInfo.files.Add(file);
                        }
                        var fileInfo = new Dictionary<string, XPathNavigator>();
                        foreach (XPathNavigator file in cfiles)
                        {
                            if (!installedFiles.Contains(file.GetAttribute("name", "")))
                            {
                                subInfo.files.Add(new subAssemblyFile { name = file.GetAttribute("name", ""), available = false });
                            }
                            fileInfo.Add(file.GetAttribute("name", ""), file);
                        }
                        foreach (var file in subInfo.files)
                        {
                            if (file.paths.Count > 0)
                            {
                                var fileEntry = fileInfo[file.name];
                                var succeeded = false;
                                var mirrors = fileEntry.Select("./mirror");
                                var fullPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                                foreach (XPathNavigator mirror in mirrors)
                                {
                                    string link = mirror.Value;
                                    new PopupDownload(link, fullPath, file.name).ShowDialog();
                                    var md5hash = HelperClasses.FileHandling.GetHashFromFile(fullPath);
                                    succeeded = fileEntry.SelectSingleNode("./hash").Value.ToLower() == md5hash;
                                    if (succeeded)
                                    {
                                        break;
                                    }
                                }
                                if (!succeeded)
                                {
                                    return false;
                                }
                                foreach (var path in file.paths)
                                {
                                    var outfullpath = System.IO.Path.Combine(Project127Files, path);
                                    System.IO.File.Copy(fullPath, outfullpath, true);
                                }
                                System.IO.File.Delete(fullPath);
                            }
                        }
                        return true;
                    }
                    foreach (XPathNavigator file in cfiles)
                    {
                        subInfo.files.Add(new subAssemblyFile { name = file.GetAttribute("name", ""), available = false });
                    }
                    installedSubassemblies.Add(subassemblyName, subInfo);
                    updateInstalled();
                    return true;

                }
                string root = s.GetAttribute("root", "");
                string rootFullPath = System.IO.Path.Combine(Project127Files, root);
                if (!System.IO.Directory.Exists(rootFullPath))
                {
                    System.IO.Directory.CreateDirectory(rootFullPath);
                }
                var files = s.Select("./file");
                var folders = s.Select("./folder");
                foreach (XPathNavigator fileEntry in files)
                {
                    var saf = getSubassemblyFile(root, fileEntry);
                    if (saf == null)
                    {
                        delSubassemblyFiles(subInfo.files);
                        return false;
                    }
                    subInfo.files.Add(saf);

                }
                foreach (XPathNavigator folderEntry in folders)
                {
                    var fol = getSubassemblyFolder(root, folderEntry);
                    subInfo.files.AddRange(fol.Key);
                    if (!fol.Value)
                    {
                        delSubassemblyFiles(subInfo.files);
                        return false;
                    }
                }
                installedSubassemblies.Add(subassemblyName, subInfo);
                updateInstalled();
                return true;
            }
        }
        private void delSubassemblyFile(subAssemblyFile f)
        {
            if (f.linked)
            {
                foreach(var sa in installedSubassemblies.Values)
                {
                    if (!sa.common)
                    {
                        continue;
                    }
                    foreach (var file in sa.files)
                    {
                        if (file.paths.Contains(f.paths[0]))
                        {
                            file.paths.Remove(f.paths[0]);
                            if (file.paths.Count == 0)
                            {
                                file.available = false;
                            }
                        }
                    }
                }
            }
            foreach (var path in f.paths)
            {
                var fullPath = System.IO.Path.Combine(Project127Files, path);
                System.IO.File.Delete(fullPath);
            }
        }
        private void delSubassemblyFiles(List<subAssemblyFile> L)
        {
            foreach (var f in L)
            {
                delSubassemblyFile(f);
            }
        }

        /// <summary>
        /// Function to uninstall subassemblies
        /// </summary>
        /// <param name="subassemblyName">Name of subassembly to be remove</param>
        /// <returns>Boolean indicating whether uninstall succeded or not</returns>
        public void delSubassembly(string subassemblyName)
        {
            delSubassembly(subassemblyName, false);
        }

        private void delSubassembly(string subassemblyName, bool reqBySupress)
        {
            if (!installedSubassemblies.ContainsKey(subassemblyName))
            {
                return;
            }
            var sa = installedSubassemblies[subassemblyName];
            var sar = availableSubassemblies[subassemblyName].GetAttribute("root", "");
            if (sa.common && !reqBySupress)
            {
                foreach(XPathNavigator saa in availableSubassemblies.Values)
                {
                    var reqs = saa.Select("./requires/subassembly");
                    foreach (XPathNavigator req in reqs)
                    {
                        if (req.GetAttribute("target","") == subassemblyName)
                        {
                            delSubassembly(saa.GetAttribute("name", ""));
                        }
                    }
                }
            }
            delSubassemblyFiles(sa.files);
            if (sar != "")
            {
                var rootFullPath = System.IO.Path.Combine(Project127Files, sar);
                System.IO.Directory.Delete(rootFullPath, true);
            }
            installedSubassemblies.Remove(subassemblyName);
            updateInstalled();
        }

        private subAssemblyFile getSubassemblyFile(string path, XPathNavigator fileEntry)
        {
            var filename = fileEntry.GetAttribute("name", "");
            path.TrimEnd("\\");
            var relPath = path + "\\" + filename;
            var fullPath = System.IO.Path.Combine(Project127Files, relPath);
            if (fileEntry.GetAttribute("linked", "").ToLower() == "true")
            {
                return linkedGetManager(path, fileEntry);
            }
            var succeeded = false;
            var mirrors = fileEntry.Select("./mirror");
            foreach (XPathNavigator mirror in mirrors)
            {
                string link = mirror.Value;
                new PopupDownload(link, fullPath, filename).ShowDialog();
                var md5hash = HelperClasses.FileHandling.GetHashFromFile(fullPath);
                succeeded = fileEntry.SelectSingleNode("./hash").Value.ToLower() == md5hash;
                if (succeeded)
                {
                    break;
                }
            }
            if (!succeeded)
            {
                return null;
            }
            else
            {
                return new subAssemblyFile
                {
                    name = filename,
                    paths = new List<string>(new string[] { relPath })
                };
            }
        }
        
        private subAssemblyFile linkedGetManager(string path, XPathNavigator fileEntry)
        {
            var filename = fileEntry.GetAttribute("name", "");
            path.TrimEnd("\\");
            var relPath = path + "\\" + filename;
            var fullPath = System.IO.Path.Combine(Project127Files, relPath);
            var from = fileEntry.GetAttribute("subassembly", "");
            var froma = availableSubassemblies[from];
            var fromi = installedSubassemblies[from];
            var files = froma.Select("./file");

            foreach (var file in fromi.files)
            {
                if (file.name == filename)
                {
                    if (file.available)
                    {
                        var fpa = file.paths[0];
                        var fpafull = System.IO.Path.Combine(Project127Files, fpa);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.Copy(fpafull, fullPath);
                        return new subAssemblyFile
                        {
                            name = filename,
                            linked = true,
                            paths = new List<string>(new string[] { relPath })
                        };
                    }
                    else
                    {
                        break;
                    }
                }  
            }
            var succeeded = false;
            foreach (XPathNavigator file in files)
            {
                if (file.GetAttribute("name","") == filename)
                {
                    var mirrors = file.Select("./mirror");
                    foreach (XPathNavigator mirror in mirrors)
                    {
                        string link = mirror.Value;
                        new PopupDownload(link, fullPath, filename).ShowDialog();
                        var md5hash = HelperClasses.FileHandling.GetHashFromFile(fullPath);
                        succeeded = file.SelectSingleNode("./hash").Value.ToLower() == md5hash;
                        if (succeeded)
                        {
                            break;
                        }
                    }
                    if (succeeded)
                    {
                        foreach (var filei in fromi.files)
                        {
                            if (filei.name == filename)
                            {
                                filei.available = true;
                                filei.paths.Add(relPath);
                                break;
                            }
                        }
                        return new subAssemblyFile
                        {
                            name = filename,
                            linked = true,
                            paths = new List<string>(new string[] { relPath })
                        };
                    }
                    break;
                }
            }
            return null;
        }
        private KeyValuePair<List<subAssemblyFile>, bool> getSubassemblyFolder(string path, XPathNavigator folderEntry)
        {
            var outp = new List<subAssemblyFile>();
            path.TrimEnd("\\");
            path = path + "\\" +folderEntry.GetAttribute("name", "");
            var files = folderEntry.Select("./file");
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Project127Files, path));
            foreach (XPathNavigator file in files)
            {
                var saf = getSubassemblyFile(path, file);
                if (saf == null)
                {
                    return new KeyValuePair<List<subAssemblyFile>,bool>(outp,false);
                }
                outp.Add(saf);
            }
            var folders = folderEntry.Select("./folder");
            foreach (XPathNavigator folder in folders)
            {
                var gfo = getSubassemblyFolder(path, folder);
                outp.AddRange(gfo.Key);
                if (!gfo.Value)
                {
                    return new KeyValuePair<List<subAssemblyFile>, bool>(outp, false);
                }
            }
            return new KeyValuePair<List<subAssemblyFile>, bool>(outp, true);
        }

        /// <summary>
        /// Function to verify the hashes of a subassembly
        /// </summary>
        /// <param name="subassemblyName">Subassembly to verify</param>
        /// <returns>Boolean indicating whether all hashes matched</returns>
        public async Task<bool> verifySubassembly(string subassemblyName)
        {
            if (!availableSubassemblies.ContainsKey(subassemblyName))
            {
                return false;
            } 
            else if (!installedSubassemblies.ContainsKey(subassemblyName))
            {
                return false;
            }
            var sa = availableSubassemblies[subassemblyName];
            var sai = installedSubassemblies[subassemblyName];
            try
            {
                if (new Version(sa.GetAttribute("version","")) != new Version(sai.version))
                {
                    return false;
                }
                else if (sa.GetAttribute("type", "") == "zip")
                {
                    return true;
                }
            }
            catch { }
            Dictionary<string, string> hashdict;
            if (sai.common)
            {
                var files = sa.Select(".//files");
                hashdict = new Dictionary<string, string>();
                foreach (XPathNavigator file in files)
                {
                    hashdict.Add(file.GetAttribute("name", ""), file.SelectSingleNode("./hash").Value);
                }
                foreach (var file in sai.files)
                {
                    foreach (var path in file.paths)
                    {
                        var fullpath = System.IO.Path.Combine(Project127Files, path);
                        var fileHash = HelperClasses.FileHandling.GetHashFromFile(fullpath);
                        if (hashdict[file.name].ToLower() != fileHash)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            hashdict = hfTree(sa, sa.GetAttribute("root", ""));
            foreach (var file in sai.files)
            {
                if (file.linked)
                {
                    continue;
                }
                foreach (var filepath in file.paths)
                {
                    var fullpath = System.IO.Path.Combine(Project127Files, filepath);
                    var fileHash = HelperClasses.FileHandling.GetHashFromFile(fullpath);
                    if (!hashdict.ContainsKey(filepath))
                    {
                        continue;
                    }
                    if (hashdict[filepath].ToLower() != fileHash)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Function to generate a list of installed subassemblies
        /// </summary>
        /// <returns>A list of names of all installed subassemblies</returns>
        public List<string> getInstalled()
        {
            return new List<string>(installedSubassemblies.Keys);
        }


		/// <summary>
		/// Function to check if a single subassembly is installed
		/// </summary>
		/// <param name="subassembly"></param>
		/// <returns>Bool if specicic subassembly is installed</returns>
		public bool isInstalled(string subassembly)
		{
			return (getInstalled().Contains(subassembly));
		}

        /// <summary>
        /// Function to get a the version of an installed subassembly
        /// </summary>
        /// <returns>The version of the installed subassembly (or 0.0 if n/a)</returns>
        public Version getVersion(string subassembly)
        {
            try
            {
                return new Version(installedSubassemblies[subassembly].version);
            }
            catch
            {
                return new Version(0, 0);
            }
        }

        /// <summary>
        /// Function to check whether a given subassembly has an available update
        /// </summary>
        /// <param name="subassemblyName">Subassembly to check</param>
        /// <returns>Boolean indicating whether or not there is an update</returns>
        public bool isUpdateAvalailable(string subassemblyName)
        {
            if (!availableSubassemblies.ContainsKey(subassemblyName))
            {
                return false;
            }
            else if (!installedSubassemblies.ContainsKey(subassemblyName))
            {
                return true;
            }
            var cver = getVersion(subassemblyName);
            var avers = availableSubassemblies[subassemblyName].GetAttribute("version", "");
            Version aver;
            try
            {
                aver = new Version(avers);
            }
            catch
            {
                return false;
            }
            if (cver < aver)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function to retrieve a list of installed subassemblies which have an available update
        /// </summary>
        /// <returns>List of subassemblies with an available update</returns>
        public List<string> availableUpdates()
        {
            var outp = new List<string>();
            foreach (var s in installedSubassemblies.Keys)
            {
                if (isUpdateAvalailable(s))
                {
                    outp.Add(s);
                }
            }
            return outp;
        }

        /// <summary>
        /// Function to update installed subassemblies
        /// </summary>
        /// <param name="subassemblyName">Name of subassembly to update</param>
        /// <param name="checkVersion"> Boolean indicating whether or not to verify an update is available</param>
        /// <returns></returns>
        public async Task<bool> updateSubssembly(string subassemblyName, bool checkVersion = true)
        {
            if (checkVersion && !isUpdateAvalailable(subassemblyName))
            {
                return true;
            }
            if (!availableSubassemblies.ContainsKey(subassemblyName) || 
                !installedSubassemblies.ContainsKey(subassemblyName))
            {
                return false;
            }
            var sa = availableSubassemblies[subassemblyName];
            var sai = installedSubassemblies[subassemblyName];
            var reqs = sa.Select("./requires/subassembly");
            foreach (XPathNavigator req in reqs)
            {
                await updateSubssembly(req.GetAttribute("target", ""));
            }
            
            return await getSubassembly(subassemblyName, true);
        }

        /// <summary>
        /// Function to set the version of an installed subassembly
        /// Can be used to "install" packages from zip import
        /// </summary>
        /// <returns>The version of the installed subassembly</returns>
        public void setVersion(string subassemblyName, Version version)
        {
            if (installedSubassemblies.ContainsKey(subassemblyName))
            {
                installedSubassemblies[subassemblyName].version = version.ToString();
            }
            else
            {
                var s = new subassemblyInfo {version = version.ToString() };
                installedSubassemblies.Add(subassemblyName, s);
            }
            updateInstalled();
        }

        public DownloadManager(string xmls = null)
        {
            if (xmls == null)
            {
                xmls = HelperClasses.FileHandling.GetStringFromURL(Globals.URL_DownloadManager);
            }
            XPathDocument xml = null;
            if (!string.IsNullOrEmpty(xmls))
            {
                xml = new XPathDocument(new System.IO.StringReader(xmls));//);
            }
            else
            {
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Download Manager unable to fetch xml").ShowDialog();
                xml = new XPathDocument(new System.IO.StringReader("<targets/>"));
            }
            nav = xml.CreateNavigator();
            var subassemblyEntries = nav.Select("/targets/subassembly");
            availableSubassemblies = new Dictionary<string, XPathNavigator>();
            foreach (XPathNavigator s in subassemblyEntries)
            {
                var name = s.GetAttribute("name", "");
                availableSubassemblies.Add(name, s);
            }
            if (HelperClasses.RegeditHandler.DoesValueExists("DownloadManagerInstalledSubassemblies"))
            {
                installedSubassemblies = json.Deserialize<Dictionary<string, subassemblyInfo>>(HelperClasses.RegeditHandler.GetValue("DownloadManagerInstalledSubassemblies"));
            }
            else
            {
                installedSubassemblies = new Dictionary<string, subassemblyInfo>();
            }

        }

        private async Task<bool> VerifyUrlExists(string url)
        {
            return HelperClasses.FileHandling.URLExists(url);
        }
       
        private class subassemblyInfo
        {
            public string version = "0.0";
            public bool common = false;
            
            public List<subAssemblyFile> files = new List<subAssemblyFile>();

            public string extentedAttributes = ""; //Future-proofing
        }
        private class subAssemblyFile
        {
            public bool available = false;
            public bool linked = false;

            public string name;
            public List<string> paths = new List<string>();

            public string extentedAttributes = ""; //Future-proofing
        }
        
        private Dictionary<string,string> hfTree(XPathNavigator x, string root)
        {
            var hashdict = new Dictionary<string, string>();
            var files = x.Select("./file");
            var folders = x.Select("./folder");
            foreach (XPathNavigator file in files)
            {
                if (file.GetAttribute("linked","").ToLower() == "true")
                {
                    continue;
                }
                hashdict[root+"\\"+file.GetAttribute("name","")] = file.SelectSingleNode("./hash").Value;
            }
            foreach (XPathNavigator folder in folders)
            {
                var shf = hfTree(folder, root + folder.GetAttribute("name", ""));
                foreach (KeyValuePair<string,string> fileHashPair in shf)
                {
                    hashdict.Add(fileHashPair.Key, fileHashPair.Value); 
                }
            }
            return hashdict;
        }
            
    }
}
