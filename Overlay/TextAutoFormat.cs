using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Overlay
{
    class TextAutoFormat
    {
        private struct mapSection {
            public int dynamicMode;
            public string content;
        }
        private static Dictionary<string, Func<string>> getters = new Dictionary<string,Func<string>>();

        private List<mapSection> map = new List<mapSection>();
        public static void registerVarGetter(string name, Func<string> getter)
        {
            getters.Add(name, getter);
        }

        public void interpret(string formatString)
        {
            map.Clear();
            var ccont = new StringBuilder();
            bool inVar = false;
            var specifier = new StringBuilder();
            foreach (char c in formatString)
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
