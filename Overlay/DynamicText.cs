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

        private List<KeyValuePair<string, List<mapSection>>> chapters = new List<KeyValuePair<string, List<mapSection>>>();

        private int currChapidx = 0;
        private List<mapSection> currChapMap 
        {
            get
            {
                return chapters[currChapidx].Value;
            }
        }

        private Dictionary<string, listenerData> listeners = new Dictionary<string, listenerData>();

        private void addListenerCondition(string listenerTarget, int chapter, string condition = null)
        {
            if (listenerTarget == null)
            {
                return;
            }
            else if (listeners.ContainsKey(listenerTarget))
            {
                if (condition != null)
                {
                    listeners[listenerTarget].conditionTarget[condition] = chapter;
                }
                else
                {
                    listeners[listenerTarget] = new listenerData { wildcardTarget = chapter };
                }
            }
            else if (!listeners.ContainsKey(listenerTarget))
            {
                if (condition != null)
                {
                    listeners[listenerTarget] = new listenerData
                    {
                        conditionTarget = new Dictionary<string, int>(),
                        wildcardTarget = -1
                    };
                    listeners[listenerTarget].conditionTarget[condition] = chapter;
                }
                else
                {
                    listeners[listenerTarget] = new listenerData { wildcardTarget = chapter };
                }
            }
        }

        /// <summary>
        /// Registers a variable getter for the dynamic text(s)
        /// </summary>
        /// <param name="name">Name of the getter/variable</param>
        /// <param name="getter">Getter function</param>
        public static void registerVarGetter(string name, Func<string> getter)
        {
            getters.Add(name, getter);
        }

        private enum specifierTypes
        {
            none,
            variable,
            label,
            listener,
            listenerTag
        }

        /// <summary>
        /// Parses the dynamic text descriptors into proper dynamic text
        /// </summary>
        /// <param name="dynamicText">Text to parse</param>
        public void parse(string dynamicText)
        {
            listeners.Clear();
            chapters.Clear();
            currChapidx = 0;
            var genMap = new List<mapSection>();
            string clabel = "base";
            genMap.Clear();
            var ccont = new StringBuilder();
            var specifier = new StringBuilder();
            string buffer = null;
            specifierTypes specifierType = specifierTypes.none;
            bool markerIsPrevChar = false;
            
            foreach (char c in dynamicText)
            {
                if (!markerIsPrevChar && specifierType == specifierTypes.none)
                {
                    if (c == '$')
                    {
                        markerIsPrevChar = true;
                    }
                    else
                    {
                        ccont.Append(c);
                    }
                }
                else if (specifierType == specifierTypes.none)
                {
                    if (markerIsPrevChar)
                    {
                        if (c == '$')
                        {
                            ccont.Append('$');
                            specifierType = specifierTypes.none;
                            markerIsPrevChar = false;
                        }
                        else if (c == ':')
                        {
                            specifierType = specifierTypes.label;
                            genMap.Add(new mapSection
                            {
                                dynamicMode = 0,
                                content = ccont.ToString()
                            });
                            ccont.Clear();
                            markerIsPrevChar = false;
                        }
                        else if (c == '@')
                        {
                            specifierType = specifierTypes.listener;
                            genMap.Add(new mapSection
                            {
                                dynamicMode = 0,
                                content = ccont.ToString()
                            });
                            ccont.Clear();
                            markerIsPrevChar = false;
                        }
                        else if (char.IsLetterOrDigit(c) || c == '_')
                        {
                            specifierType = specifierTypes.variable;
                            genMap.Add(new mapSection
                            {
                                dynamicMode = 0,
                                content = ccont.ToString()
                            });
                            ccont.Clear();
                            markerIsPrevChar = false;
                            specifier.Append(c);
                        }
                    }
                }
                else if (
                    specifierType == specifierTypes.label ||
                    specifierType == specifierTypes.listener ||
                    specifierType == specifierTypes.listenerTag
                    )
                {
                    if (c == '$')
                    {
                        if (!markerIsPrevChar)
                        {
                            markerIsPrevChar = true;
                        }
                        else
                        {
                            markerIsPrevChar = false;
                            specifier.Append(c);
                        }
                        
                    }
                    else if (specifierType == specifierTypes.listener && 
                        ((!char.IsLetterOrDigit(c) && (c != '_')) || markerIsPrevChar))
                    {
                        //Add lister if doesnt exist
                        if (markerIsPrevChar)
                        {
                            //wildcard (on any change)
                            addListenerCondition(specifier.ToString(), chapters.Count);
                            specifier.Clear();
                            specifierType = specifierTypes.none;
                            ccont.Append(c);
                        }
                        else
                        {
                            buffer = specifier.ToString();
                            specifier.Clear();
                            specifierType = specifierTypes.listenerTag;
                        }
                    }
                    else if (c == '\n' || markerIsPrevChar)
                    {
                        markerIsPrevChar = false;
                        if (specifierType == specifierTypes.label)
                        {
                            chapters.Add(new KeyValuePair<string, List<mapSection>>(clabel, genMap));
                            genMap = new List<mapSection>();
                            clabel = specifier.ToString();
                        } 
                        else if (specifierType == specifierTypes.listenerTag)
                        {
                            addListenerCondition(buffer, chapters.Count, specifier.ToString());
                        }
                        specifier.Clear();
                        specifierType = specifierTypes.none;
                        if (markerIsPrevChar && c != ',')
                        {
                            ccont.Append(c);
                        }
                    }
                    else
                    {
                        specifier.Append(c);
                    }
                }
                else if (specifierType == specifierTypes.variable)
                {
                    if (!char.IsLetterOrDigit(c) && c != '_')
                    {
                        genMap.Add(new mapSection
                        {
                            dynamicMode = 1,
                            content = specifier.ToString()
                        });
                        specifier.Clear();
                        if (c != '$')
                        {
                            ccont.Append(c);
                        }
                        specifierType = specifierTypes.none;
                        continue;
                    }
                    specifier.Append(c);
                }
                
            }
            if (specifierType == specifierTypes.variable)
            {
                genMap.Add(new mapSection
                {
                    dynamicMode = 1,
                    content = specifier.ToString()
                });
            } 
            else if (specifierType == specifierTypes.none)
            {
                genMap.Add(new mapSection
                {
                    dynamicMode = 0,
                    content = ccont.ToString()
                });
            }
            chapters.Add(new KeyValuePair<string, List<mapSection>>(clabel, genMap));
        }

        private void runListeners()
        {
            foreach (var listenerName in listeners.Keys)
            {
                if (!getters.ContainsKey(listenerName))
                {
                    continue;
                }
                var cstate = getters[listenerName]();
                var listener = listeners[listenerName];
                if (cstate == listener.pstate)
                {
                    continue;
                }
                else
                {
                    listener.pstate = cstate;
                    listeners[listenerName] = listener;
                    int targetChapter;
                    if (listener.conditionTarget.TryGetValue(cstate, out targetChapter))
                    {
                        currChapidx = targetChapter;
                        return;
                    }
                    else if (listener.wildcardTarget != -1)
                    {
                        currChapidx = listener.wildcardTarget;
                        return;
                    }
                }
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
                runListeners();
                var output = new StringBuilder();
                foreach (var section in currChapMap)
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

        /// <summary>
		/// Navigate to the next chapter of dynamic text
		/// </summary>
        public void nextChapter()
        {
            currChapidx++;
            currChapidx %= chapters.Count;
        }

        /// <summary>
		/// Navigate to the previous chapter of dynamic text
		/// </summary>
        public void prevChapter()
        {
            currChapidx--;
            if (currChapidx == -1)
            {
                currChapidx = chapters.Count - 1;
            }
        }

        /// <summary>
		/// Navigate to chapter 0 of dynamic text
		/// </summary>
        public void goCh0()
        {
            currChapidx = 0;
        }

        /// <summary>
        /// Get current chapter name/title
        /// </summary>
        /// <returns>Chapter name/title</returns>
        public string getChapterName()
        {
            return chapters[currChapidx].Key;
        }

        /// <summary>
        /// Attempts to locate a chapter by a given name, and change to it
        /// </summary>
        /// <param name="name">Targe</param>
        /// <returns>Bool indicating whether navigation was successful</returns>
        public bool getChapterByName(string name)
        {
            var count = chapters.Count;
            for (var i = 0; i < count; i++)
            {
                var chapter = chapters[i];
                if (chapter.Key == name)
                {
                    currChapidx = i;
                    return true;
                }
            }
            return false;
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

        private struct listenerData
        {
            public int wildcardTarget;
            public string pstate;
            public Dictionary<string, int> conditionTarget;
        }
    }
}
