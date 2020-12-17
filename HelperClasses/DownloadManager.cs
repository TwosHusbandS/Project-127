using Microsoft.Win32;
using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.XPath;

namespace Project_127.HelperClasses
{
	class DownloadManager
	{
        private XPathNavigator nav;
        private Dictionary<string, XPathNavigator> availableSubassemblies;
        private Dictionary<string, subassemblyInfo> _installedSubassemblies;
        private static string Project127Files
        {
            get
            {
                return System.IO.Path.Combine(LauncherLogic.ZIPFilePath, @"Project_127_Files\");
            }
        }
                
        private Dictionary<string, subassemblyInfo> installedSubassemblies
        {
            get
            {
                return _installedSubassemblies;
            }
            set
            {
                _installedSubassemblies = value;
            }
        }

        private static JavaScriptSerializer json = new JavaScriptSerializer();
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
                var s = availableSubassemblies[subassemblyName];
                var subInfo = new subassemblyInfo();
                Version vinfo;
                try
                {
                    vinfo = new Version(s.GetAttribute("version", ""));
                }
                catch
                {
                    vinfo = new Version(0, 0);
                }
                subInfo.version = vinfo;
                if (s.GetAttribute("type","").ToLower() == "zip")
                {
                    var mirrors = s.Select("./mirror");
                    var succeeded = false;
                    foreach (XPathNavigator zipMirror in mirrors)
                    {
                        var zipdlpath = System.IO.Path.Combine(Globals.ProjectInstallationPath, "dl.zip");
                        string link = zipMirror.Value;
                        new PopupDownload(link, zipdlpath, "Downloading zip...").ShowDialog();
                        var zipmd5 = HelperClasses.FileHandling.GetHashFromFile(zipdlpath);
                        succeeded = s.SelectSingleNode("./hash").Value.ToLower() == zipmd5;
                        if (!succeeded)
                        {
                            continue;
                        }
                        new PopupProgress(PopupProgress.ProgressTypes.ZIPFile, zipdlpath).ShowDialog();
                        break;
                    }
                    if (succeeded)
                    {
                        installedSubassemblies.Add(subassemblyName, subInfo);
                        return true;
                    }
                }
                else if (s.GetAttribute("type", "").ToLower() == "common")
                {
                    subInfo.common = true;
                    var cfiles = s.Select("./file");
                    foreach (XPathNavigator file in cfiles)
                    {
                        subInfo.files.Add(new subAssemblyFile { name = file.GetAttribute("name", ""), available = false });
                    }
                    installedSubassemblies.Add(subassemblyName, subInfo);
                    return true;

                }
                var reqs = s.Select("./requires/subassembly");
                foreach (XPathNavigator req in reqs)
                {
                    await getSubassembly(req.GetAttribute("target", ""));
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
        public void delSubassembly(string subassemblyName)
        {
            if (!installedSubassemblies.ContainsKey(subassemblyName))
            {
                return;
            }
            var sa = installedSubassemblies[subassemblyName];
            var sar = availableSubassemblies[subassemblyName].GetAttribute("root", "");
            if (sa.common)
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
        }

        private subAssemblyFile getSubassemblyFile(string path, XPathNavigator fileEntry)
        {
            var filename = fileEntry.GetAttribute("name", "");
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
                new PopupDownload(link, fullPath, "Downloading " + filename).ShowDialog();
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
                    var mirrors = file.Select("//mirror");
                    foreach (XPathNavigator mirror in mirrors)
                    {
                        string link = mirror.Value;
                        new PopupDownload(link, fullPath, "Downloading " + filename).ShowDialog();
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
                if (new Version(sa.GetAttribute("version","")) != sai.version)
                {
                    return false;
                }
            }
            catch { }
            Dictionary<string, string> hashdict;
            if (sai.common)
            {
                var files = sa.Select("//files");
                hashdict = new Dictionary<string, string>();
                foreach (XPathNavigator file in files)
                {
                    hashdict.Add(file.GetAttribute("name", ""), file.SelectSingleNode("//hash").Value);
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
        public DownloadManager(string xmlLocation)
        {
            XPathDocument xml = new XPathDocument(xmlLocation);//);
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
                _installedSubassemblies = json.Deserialize<Dictionary<string, subassemblyInfo>>(HelperClasses.RegeditHandler.GetValue("DownloadManagerInstalledSubassemblies"));
            }
            else
            {
                installedSubassemblies = new Dictionary<string, subassemblyInfo>();
            }

        }
       
        private class subassemblyInfo
        {
            public Version version;
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
        
        public Dictionary<string,string> hfTree(XPathNavigator x, string root)
        {
            var hashdict = new Dictionary<string, string>();
            var files = x.Select("./file");
            var folders = x.Select("./folder");
            foreach (XPathNavigator file in files)
            {
                hashdict[root+"\\"+file.GetAttribute("name","")] = file.SelectSingleNode("//hash").Value;
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
