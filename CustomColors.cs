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

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// All other colors:
        /// MW = MainWindow
        /// PU = Pop Up
        /// SE = Settings
        /// SFH = SafeFileHandler
        /// MO = Mouse Over
        /// DG = Data Grid
        /// </summary>
        

		// Colors and stuff which is referenced in all of the XAML. 
		// AFAIK, no Color is hardcoded.
		// Some of thickness, corner radius, margins are semi-hardcoded in XAML Styles.

        // Border of MainWindow
        public static Brush MW_Border { get; private set; } = MyColorWhite;

        // All the HamburgerButton Items, Backgrounds, etc.
        public static Brush MW_HamburgerMenuGridBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
        public static System.Windows.Thickness MW_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);
        public static Brush MW_ButtonBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
        public static Brush MW_ButtonForeground { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonBorderBrush { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonMOBackground { get; private set; } = SetOpacity(MyColorOrange, 70);
        public static Brush MW_ButtonMOForeground { get; private set; } = MyColorBlack;
        public static Brush MW_ButtonMOBorderBrush { get; private set; } = MyColorBlack;

        // Hamburger Button and "X"
        public static Brush MW_ButtonSmallBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
        public static Brush MW_ButtonSmallForeground { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonSmallBorderBrush { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonSmallMOBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
        public static Brush MW_ButtonSmallMOForeground { get; private set; } = MyColorOrange;
        public static Brush MW_ButtonSmallMOBorderBrush { get; private set; } = MyColorOrange;
        public static System.Windows.Thickness MW_ButtonSmallBorderThickness { get; private set; } = new System.Windows.Thickness(2);

        // GTA Launch Button
        // Border Color will depend on game running or not running, so we will not set this here. I guess. 
        public static Brush MW_ButtonGTABackground { get; private set; } = SetOpacity(MyColorBlack, 70);
        public static Brush MW_ButtonGTAForeground { get; private set; } = MyColorWhite;
        public static Brush MW_ButtonGTAMOBackground { get; private set; } = SetOpacity(MyColorBlack,50);
        public static Brush MW_ButtonGTAMOForeground { get; private set; } = MyColorOrange;
        public static System.Windows.Thickness MW_ButtonGTABorderThickness { get; private set; } = new System.Windows.Thickness(5);

        // POPUP Window
        public static Brush PU_Background { get; private set; } = MyColorBlack;
        public static Brush PU_BorderBrush { get; private set; } = MyColorWhite;
        public static Brush PU_BorderBrush_Inner { get; private set; } = MyColorOrange;
        public static Brush PU_ButtonBackground { get; private set; } = MyColorBlack;
        public static Brush PU_ButtonForeground { get; private set; } = MyColorWhite;
        public static Brush PU_ButtonBorderBrush { get; private set; } = MyColorWhite;
        public static Brush PU_ButtonMOBackground { get; private set; } = MyColorBlack;
        public static Brush PU_ButtonMOForeground { get; private set; } = MyColorOrange;
        public static Brush PU_ButtonMOBorderBrush { get; private set; } = MyColorOrange;
        public static System.Windows.Thickness PU_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);
        public static Brush PU_LabelForeground { get; private set; } = MyColorWhite;

        // SaveFilerHandler Window
        public static Brush SFH_Background { get; private set; } = MyColorBlack;
        public static Brush SFH_BorderBrush { get; private set; } = MyColorWhite;
        public static Brush SFH_BorderBrush_Inner { get; private set; } = MyColorOrange;
        public static Brush SFH_LabelForeground { get; private set; } = MyColorWhite;

        public static Brush SFH_ButtonBackground { get; private set; } = MyColorBlack;
        public static Brush SFH_ButtonForeground { get; private set; } = MyColorWhite;
        public static Brush SFH_ButtonBorderBrush { get; private set; } = MyColorWhite;
        public static Brush SFH_ButtonMOBackground { get; private set; } = MyColorBlack;
        public static Brush SFH_ButtonMOForeground { get; private set; } = MyColorOrange;
        public static Brush SFH_ButtonMOBorderBrush { get; private set; } = MyColorOrange;

        public static Brush SFH_SVBackground { get; private set; } = MyColorBlack;
        public static Brush SFH_SVForeground { get; private set; } = MyColorOrange;

        public static Brush SFH_DGBackground { get; private set; } = MyColorBlack;
        public static Brush SFH_DGForeground { get; private set; } = MyColorWhite;
        public static Brush SFH_DGCellBackground { get; private set; } = MyColorBlack;
        
        public static Brush SFH_DGCellSelected { get; private set; } =  MyColorOrange;

        public static System.Windows.Thickness SFH_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);


		// Settings Window
		public static Brush SE_Background { get; private set; } = MyColorBlack;
		public static Brush SE_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush SE_BorderBrush_Inner { get; private set; } = MyColorOrange;
		public static Brush SE_LabelForeground { get; private set; } = MyColorWhite;

		public static Brush SE_ButtonBackground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SE_ButtonMOBackground { get; private set; } = MyColorBlack;
		public static Brush SE_ButtonMOForeground { get; private set; } = MyColorOrange;
		public static Brush SE_ButtonMOBorderBrush { get; private set; } = MyColorOrange;
							 
		public static Brush SE_SVBackground { get; private set; } = MyColorBlack;
		public static Brush SE_SVForeground { get; private set; } = MyColorOrange;

		public static System.Windows.Thickness SE_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>
		/// Takes a Brush and a Opacity (0-100) and returns a Brush.
		/// </summary>
		/// <param name="pBrush"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
		private static Brush SetOpacity(Brush pBrush, int pOpacity)
        {
            double dOpacity = (((double)pOpacity) / 100);
            Brush NewBrush = pBrush.Clone();
            NewBrush.Opacity = dOpacity;
            return NewBrush;
        }


		/// <summary>
		/// Returns a Brush from a Hex String
		/// </summary>
		/// <param name="pString"></param>
		/// <returns></returns>
        private static Brush GetBrushHex(string pString)
        {
            return (GetBrushHex(pString, 100));
        }


		/// <summary>
		/// Returns a Brush from a Hex String and an Opacity (0-100)
		/// </summary>
		/// <param name="pString"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
		private static Brush GetBrushHex(string pString, int pOpacity)
        {
            Brush rtrn = (Brush)new BrushConverter().ConvertFromString("#" + pString.TrimStart('#'));
            return SetOpacity(rtrn,pOpacity);
        }


		/// <summary>
		/// Returns a Brush from RGB integers
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
        private static Brush GetBrushRGB(int r, int g, int b)
        {
            return GetBrushRGB(r,g,b, 100);
        }


		/// <summary>
		/// Returns a Brush from RGB integers, and an Opacity (0-100)
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <param name="pOpacity"></param>
		/// <returns></returns>
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


        /// <summary>
        /// DebugPopup Method. Just opens Messagebox with pMsg
        /// </summary>
        /// <param name="pMsg"></param>
        public static void DebugPopup(string pMsg)
        {
            System.Windows.Forms.MessageBox.Show(pMsg);
        }

    } // End of Class
} // End of Namespace
