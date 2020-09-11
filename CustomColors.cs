using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project_127
{
    /// <summary>
    /// Class for Custom Colors and GUI Stuff
    /// </summary>
    class CustomColors
    {

        /// <summary>
        /// Actual Main colors we use:
        /// </summary>
        public static Brush MyColorWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#ffffff");
        public static Brush MyColorBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#000000");
        public static Brush MyColorOrange { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#E35627");

        public static Brush MyColorWhite70 { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#B3ffffff");
        public static Brush MyColorBlack70 { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#B3000000");
        public static Brush MyColorOrange70 { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#B3E35627");

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// All other colors:
        /// MW = MainWindow
        /// PU = Pop Up
        /// SE = Settings
        /// SFH = SafeFileHandler
        /// MO = Mouse Over
        /// </summary>
        public static Brush MW_Border { get; private set; } = MyColorWhite;

        public static Brush MW_ButtonBackground { get; private set; } = SetOpacity(MyColorBlack,20);
        public static Brush MW_ButtonForeground { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonBorderBrush { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonMOBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
        public static Brush MW_ButtonMOForeground { get; private set; } = GetBrushRGB(227, 86, 39);
        public static Brush MW_ButtonMOBorderBrush { get; private set; } = GetBrushRGB(227, 86, 39);

        public static Brush MW_ButtonSmallBackground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonSmallForeground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonSmallBorderBrush { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonSmallMOBackground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonSmallMOForeground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonSmallMOBorderBrush { get; private set; } = MyColorWhite70;

        public static Brush MW_ButtonGTABackground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonGTAForeground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonGTABorderBrush { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonGTAMOBackground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonGTAMOForeground { get; private set; } = MyColorWhite70;
        public static Brush MW_ButtonGTAMOBorderBrush { get; private set; } = MyColorWhite70;


        public static Brush PU_Background { get; private set; } = MyColorWhite70;
        public static Brush PU_BorderBrush { get; private set; } = MyColorWhite70;
        public static Brush PU_BorderBrush_Inner { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonBackground { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonForeground { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonBorderBrush { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonMOBackground { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonMOForeground { get; private set; } = MyColorWhite70;
        public static Brush PU_ButtonMOBorderBrush { get; private set; } = MyColorWhite70;
        public static Brush PU_LabelForeground { get; private set; } = MyColorWhite70;

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static Brush SetOpacity(Brush pBrush, int pOpacity)
        {
            double dOpacity = (((double)pOpacity) / 100);
            pBrush.Opacity = dOpacity;
            return pBrush;
        }

        private static Brush GetBrushHex(string pString)
        {
            return (GetBrushHex(pString, 100));
        }

        private static Brush GetBrushHex(string pString, int pOpacity)
        {
            Brush rtrn = (Brush)new BrushConverter().ConvertFromString("#" + pString.TrimStart('#'));
            return SetOpacity(rtrn,pOpacity);
        }


        private static Brush GetBrushRGB(int r, int g, int b)
        {
            return GetBrushRGB(r,g,b, 100);
        }


        // yeye this ugly like yo mama but its just for internal testing. Wont be called in production
        private static Brush GetBrushRGB(int r, int g, int b, int pOpacity)
        {
            try
            {
                string hex = string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);

                return GetBrushHex(hex, pOpacity);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("this shouldnt have happened. Error in RGB / Hex conversion");
                Environment.Exit(1);
                return null;
            }
        }

    }
}
