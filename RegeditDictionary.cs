using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Project_127
{
    class RegeditDictionary : IDictionary<string, string>
    {
        private RegistryKey RegistryKey;

        public RegeditDictionary(string registryKey) {
            try
            {
                this.RegistryKey = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\{registryKey}");
                if(registryKey == null)
                {
                    System.Windows.Forms.MessageBox.Show("regedit null", "wtf");
                }
            } catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.InnerException.StackTrace, e.InnerException.Message);
            }
        }

        public RegeditDictionary(string registryKey, Dictionary<string, string> copy)
        {
            try
            {
                this.RegistryKey = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\{registryKey}");
                if (registryKey == null)
                {
                    System.Windows.Forms.MessageBox.Show("regedit null", "wtf");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.InnerException.StackTrace, e.InnerException.Message);
            }
            Add(copy);
        }

        public string this[string key] { get => Get(key); set => Set(key, value); }

        public ICollection<string> Keys => GetKeys();

        public ICollection<string> Values => GetValues();

        public IDictionary<string, string> KeyValues => GetKeyValues();

        public int Count => KeyValues.Count;

        public bool IsReadOnly => false;

        public string Get(string key)
        {
            object value = RegistryKey.GetValue(key.ToString());
            if (value == null) return null;
            return value.ToString();
        }

        public void Set(string key, string value)
        {
            RegistryKey.SetValue(key, value);
        }

        public void Set(Dictionary<string, string> copy)
        {
            foreach (KeyValuePair<string, string> entry in copy)
            {
                Set(entry.Key, entry.Value);
            }
        }

        public void Add(string key, string value)
        {
            if(Get(key) == null)
            {
                Set(key, value);
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(Dictionary<string, string> copy)
        {
            foreach (KeyValuePair<string, string> entry in copy)
            {
                Add(entry.Key, entry.Value);
            }
        }

        public void Clear()
        {
            foreach(string key in Keys)
            {
                Remove(key);
            }
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return KeyValues.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            try
            {
                RegistryKey.DeleteValue(key, true);
                return true;
            } catch {
                return false;
            }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> GetKeyValues()
        {
            string[] keys = RegistryKey.GetValueNames();
            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            foreach(string key in keys)
            {
                keyValues.Add(key, Get(key));
            }
            return keyValues;
        }

        private List<string> GetKeys()
        {
            return GetKeyValues().Keys.ToList<string>();
        }

        private List<string> GetValues()
        {
            return GetKeyValues().Values.ToList<string>();
        }
    }
}
