using Project_127.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Auth
{
    public class DynamicMTLOffsets
    {
        static readonly byte[] match_pattern_default = { 0x84, 0xC0, 0x75, 0x19, 0xFF, 0xC7, 0x83, 0xFF,
        0x01, 0x7C, 0xD8 };
        static readonly Int32 pattern_search_offset_default = 35;
        static readonly Int32 blob_offset_default = 4;
        static readonly Int32 blob_count_default = 16384;
        static readonly Int32 rockstarId_offset_default = 0xEF0;
        static readonly Int32 rockstarId_take_default = 8;
        static readonly Int32 sessKey_offset_default = 0x1110;
        static readonly Int32 sessKey_take_default = 16;
        static readonly Int32 ticket_offset_default = 0xAF0;
        static readonly Int32 sessTicket_offset_default = 0xCF0;
        static readonly Int32 rockstarNick_offset_default = 0xEA7;
        static readonly Int32 countryCode_offset_default = 0xE0C;
        static readonly UInt64 IDSegment_bitwiseXOR_default = 0xDEADCAFEBABEFEED;

        public byte[] match_pattern { get; private set; }
        public Int32 pattern_search_offset { get; private set; }
        public Int32 blob_offset { get; private set; }
        public Int32 blob_count { get; private set; }
        public Int32 rockstarId_offset { get; private set; }
        public Int32 rockstarId_take { get; private set; }
        public Int32 sessKey_offset { get; private set; }
        public Int32 sessKey_take { get; private set; }
        public Int32 ticket_offset { get; private set; }
        public Int32 sessTicket_offset { get; private set; }
        public Int32 rockstarNick_offset { get; private set; }
        public Int32 countryCode_offset { get; private set; }
        public UInt64 IDSegment_bitwiseXOR { get; private set; }


        /// <summary>
        /// Returns Default DMO Object
        /// </summary>
        private DynamicMTLOffsets()
        {
            this.match_pattern = match_pattern_default;
            this.pattern_search_offset = pattern_search_offset_default;
            this.blob_offset = blob_offset_default;
            this.blob_count = blob_count_default;
            this.rockstarId_offset = rockstarId_offset_default;
            this.rockstarId_take = rockstarId_take_default;
            this.sessKey_offset = sessKey_offset_default;
            this.sessKey_take = sessKey_take_default;
            this.ticket_offset = ticket_offset_default;
            this.sessTicket_offset = sessTicket_offset_default;
            this.rockstarNick_offset = rockstarNick_offset_default;
            this.countryCode_offset = countryCode_offset_default;
            this.IDSegment_bitwiseXOR = IDSegment_bitwiseXOR_default;
        }

        private DynamicMTLOffsets(string XML_Content)
        {
            // No try/catch since we specifically try/catch where this is called to return null

            string match_pattern_string = HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "match_pattern");
            List<byte> tmp = new List<byte>();
            foreach (string asdf in match_pattern_string.Replace(" ", "").Split(','))
            {
                tmp.Add(Convert.ToByte(asdf, 16));
            }

            this.match_pattern = tmp.ToArray();
            this.pattern_search_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "pattern_search_offset"));
            this.blob_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "blob_offset"));
            this.blob_count = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "blob_count"));
            this.rockstarId_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "rockstarId_offset"), 16);
            this.rockstarId_take = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "rockstarId_take"));
            this.sessKey_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "sessKey_offset"), 16);
            this.sessKey_take = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "sessKey_take"));
            this.ticket_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "ticket_offset"), 16);
            this.sessTicket_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "sessTicket_offset"), 16);
            this.rockstarNick_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "rockstarNick_offset"), 16);
            this.countryCode_offset = Convert.ToInt32(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "countryCode_offset"), 16);
            this.IDSegment_bitwiseXOR = Convert.ToUInt64(HelperClasses.FileHandling.GetXMLTagContent(XML_Content, "IDSegment_bitwiseXOR"), 16);
        }

        private void WriteToFile()
        {
            try
            {
                List<string> content = new List<string>();
                content.Add("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                content.Add("<item>");

                List<string> tmp = new List<string>();
                for (int i = 0; i <= match_pattern.Length - 1; i++)
                {
                    tmp.Add(String.Format("0x{0:X2}", match_pattern[i]));
                }

                content.Add("\t<match_pattern>" + String.Join(", ", tmp) + "</match_pattern>");
                content.Add("\t<pattern_search_offset>" + pattern_search_offset + "</pattern_search_offset>");
                content.Add("\t<blob_offset>" + blob_offset + "</blob_offset>");
                content.Add("\t<blob_count>" + blob_count + "</blob_count>");
                content.Add("\t<rockstarId_offset>" + String.Format("0x{0:X}", rockstarId_offset) + "</rockstarId_offset>");
                content.Add("\t<rockstarId_take>" + rockstarId_take + "</rockstarId_take>");
                content.Add("\t<sessKey_offset>" + String.Format("0x{0:X}", sessKey_offset) + "</sessKey_offset>");
                content.Add("\t<sessKey_take>" + sessKey_take + "</sessKey_take>");
                content.Add("\t<ticket_offset>" + String.Format("0x{0:X}", ticket_offset) + "</ticket_offset>");
                content.Add("\t<sessTicket_offset>" + String.Format("0x{0:X}", sessTicket_offset) + "</sessTicket_offset>");
                content.Add("\t<rockstarNick_offset>" + String.Format("0x{0:X}", rockstarNick_offset) + "</rockstarNick_offset>");
                content.Add("\t<countryCode_offset>" + String.Format("0x{0:X}", countryCode_offset) + "</countryCode_offset>");
                content.Add("\t<IDSegment_bitwiseXOR>" + String.Format("0x{0:X}", IDSegment_bitwiseXOR) + "</IDSegment_bitwiseXOR>");
                content.Add("</item>");

                HelperClasses.FileHandling.WriteStringToFileOverwrite(Globals.MTLOffsetsLocalFilepath, content.ToArray());
            }
            catch (Exception ex)
            {
                HelperClasses.Logger.Log("Error writing Dynamic MTL Offsets to file. " + ex.ToString());
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || this == null)
            {
                return false;
            }

            var other = obj as DynamicMTLOffsets;

            for (int i = 0; i <= this.match_pattern.Length - 1; i++)
            {
                if (this.match_pattern[i] != other.match_pattern[i])
                {
                    return false;
                }
            }

            if (this.pattern_search_offset != other.pattern_search_offset) { return false; }
            if (this.blob_offset != other.blob_offset) { return false; }
            if (this.blob_count != other.blob_count) { return false; }
            if (this.rockstarId_offset != other.rockstarId_offset) { return false; }
            if (this.rockstarId_take != other.rockstarId_take) { return false; }
            if (this.sessKey_offset != other.sessKey_offset) { return false; }
            if (this.sessKey_take != other.sessKey_take) { return false; }
            if (this.ticket_offset != other.ticket_offset) { return false; }
            if (this.sessTicket_offset != other.sessTicket_offset) { return false; }
            if (this.rockstarNick_offset != other.rockstarNick_offset) { return false; }
            if (this.countryCode_offset != other.countryCode_offset) { return false; }
            if (this.IDSegment_bitwiseXOR != other.IDSegment_bitwiseXOR) { return false; }


            return true;
        }


        public static async Task SetUpMTLOffsets()
        {
            MainWindow.DMO = await GetMTLOffsets().ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<DynamicMTLOffsets> GetMTLOffsets()
        {
            // DMO = DynamicMtlOffsets

            DynamicMTLOffsets DMO_File = GetFromFile();
            DynamicMTLOffsets DMO_Github = await GetFromGithub().ConfigureAwait(continueOnCapturedContext: false);
            DynamicMTLOffsets DMO_Default = GetDefaults();


            // if file is null
            if (DMO_File == null)
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: Local file is null");

                // if file and github are both null
                if (DMO_Github == null)
                {
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Github is null");
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Writing defaults to file and returning defaults");
                    DMO_Default.WriteToFile();
                    return DMO_Default;
                }
                // if file is null, github is good
                else
                {
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Github is working");
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Writing Github to file and returning github");

                    DMO_Github.WriteToFile();
                    return DMO_Github;
                }
            }
            // if file is good
            else
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: Local file is working");

                // if file is good and github is null
                if (DMO_Github == null)
                {
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Github is null");
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: returning local file");
                    return DMO_File;
                }
                // if file is good and github is good
                else
                {
                    HelperClasses.Logger.Log("Dynamic MTL Offsets: Github is working");

                    // if they are the same
                    if (DMO_File.Equals(DMO_Github))
                    {
                        HelperClasses.Logger.Log("Dynamic MTL Offsets: local file and github are the same, returning local file");

                        // doesnt matter if we return DMO_File or DMO_Github
                        return DMO_File;
                    }
                    else
                    {
                        HelperClasses.Logger.Log("Dynamic MTL Offsets: local file and github are different");

                        // Local file is read only. Taking that.
                        if (HelperClasses.FileHandling.IsFileReadOnly(Globals.MTLOffsetsLocalFilepath))
                        {
                            HelperClasses.Logger.Log("Dynamic MTL Offsets: local file is read only, returning local file");

                            return DMO_File;
                        }
                        // Local file is NOT read only
                        else
                        {
                            HelperClasses.Logger.Log("Dynamic MTL Offsets: local file is NOT read only, asking user");

                            bool yesno = PopupWrapper.PopupYesNo("There are new MTL Offsets on github.\nDo you want to replace your local ones?\n\nIf you do not know what you are doing,\nor have not been instructed otherwise,\nclick Yes.");
                            if (yesno == true)
                            {
                                HelperClasses.Logger.Log("Dynamic MTL Offsets: User wants github. Writing github to file, returning github");
                                DMO_Github.WriteToFile();
                                return DMO_Github;
                            }
                            else
                            {
                                HelperClasses.Logger.Log("Dynamic MTL Offsets: User does NOT want github. Returning local file");

                                return DMO_File;
                            }
                        }
                    }
                }
            }
        }

        private static DynamicMTLOffsets GetDefaults()
        {
            return new DynamicMTLOffsets();
        }

        private static async Task<DynamicMTLOffsets> GetFromGithub()
        {
            string input = "";
            input = await Globals.XML_MTLOffsetsGithub().ConfigureAwait(continueOnCapturedContext: false);
            if (String.IsNullOrWhiteSpace(input))
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: Github input empty");
                return null;
            }
            try
            {
                return new DynamicMTLOffsets(input);
            }
            catch (Exception e)
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: Github constructor failed: " + e.ToString());
                return null;
            }
        }

        private static DynamicMTLOffsets GetFromFile()
        {
            string input = "";
            input = HelperClasses.FileHandling.ReadContentOfFile(Globals.MTLOffsetsLocalFilepath);
            if (String.IsNullOrWhiteSpace(input))
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: File input empty");
                return null;
            }
            try
            {
                return new DynamicMTLOffsets(input);
            }
            catch (Exception e)
            {
                HelperClasses.Logger.Log("Dynamic MTL Offsets: File constructor failed: " + e.ToString());
                return null;
            }
        }


    }
}
