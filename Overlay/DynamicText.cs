using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Overlay
{
    /// <summary>
    /// Class to handle dynamic text
    /// </summary>
    class DynamicText
    {
        private struct mapSection {
            public int dynamicMode;
            public string content;
        }
        private static Dictionary<string, Func<string>> getters = new Dictionary<string,Func<string>>();

        private List<mapSection> map = new List<mapSection>();

        /// <summary>
        /// Registers a variable getter for the dynamic text(s)
        /// </summary>
        /// <param name="name">Name of the getter/variable</param>
        /// <param name="getter">Getter function</param>
        public static void registerVarGetter(string name, Func<string> getter)
        {
            getters.Add(name, getter);
        }

        /// <summary>
        /// Parses the dynamic text descriptors into proper dynamic text
        /// </summary>
        /// <param name="dynamicText">Text to parse</param>
        public void interpret(string dynamicText)
        {
            map.Clear();
            var ccont = new StringBuilder();
            bool inVar = false;
            var specifier = new StringBuilder();
            foreach (char c in dynamicText)
            {
                if (c == '$' && !inVar)
                {
                    inVar = true;
                }
                else if (!inVar)
                {
                    ccont.Append(c);
                }
                else
                {
                    if (specifier.Length == 0 && c == '$')
                    {
                        ccont.Append('$');
                        inVar = false;
                        continue;
                    } 
                    else if (specifier.Length == 0)
                    {
                        map.Add( new mapSection
                        {
                            dynamicMode = 0,
                            content = ccont.ToString()
                        });
                        ccont.Clear();
                    }
                    if (char.IsWhiteSpace(c))
                    {
                        map.Add(new mapSection
                        {
                            dynamicMode = 1,
                            content = specifier.ToString()
                        });
                        specifier.Clear();
                        ccont.Append(c);
                        inVar = false;
                        continue;
                    }
                    specifier.Append(c);

                }
            }
            if (inVar)
            {
                map.Add(new mapSection
                {
                    dynamicMode = 1,
                    content = specifier.ToString()
                });
            } 
            else
            {
                map.Add(new mapSection
                {
                    dynamicMode = 0,
                    content = ccont.ToString()
                });
            }
        }

        /// <summary>
        /// Generates the current text
        /// </summary>
        /// <returns>String representing the current state of the dynamic text</returns>
        public string frame()
        {
            try
            {
                var output = new StringBuilder();
                foreach (var section in map)
                {
                    if (section.dynamicMode == 0)
                    {
                        output.Append(section.content);
                    }
                    else
                    {
                        output.Append(dynamicHandler(section));
                    }
                }
                return output.ToString();
            }
            catch
            {
                return "";
            }
        }

        private string dynamicHandler(mapSection m)
        {
            if (getters.ContainsKey(m.content))
            {
                return getters[m.content]();
            } else
            {
                return String.Format("${0}", m.content);
            }
        }
    }
}
