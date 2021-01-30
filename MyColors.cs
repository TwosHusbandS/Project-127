using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Project_127
{
	class MyColors
	{
		/// COLOR STUFF

		/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Actual Main colors we use:
		/// </summary>
		public static Brush MyColorWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
		public static Brush MyColorOffWhite { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#c1ced1");
		public static Brush MyColorOffWhite85 { get; private set; } = SetOpacity((Brush)new BrushConverter().ConvertFromString("#c1ced1"), 85);
		public static Brush MyColorBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#000000");
		public static Brush MyColorOffBlack { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#1a1a1a");
		public static Brush MyColorOffBlack70 { get; private set; } = SetOpacity((Brush)new BrushConverter().ConvertFromString("#1a1a1a"), 70);
		public static Brush MyColorOffBlack50 { get; private set; } = SetOpacity((Brush)new BrushConverter().ConvertFromString("#1a1a1a"), 50);
		public static Brush MyColorOrange { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#E35627");
		public static Brush MyColorGreen { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#4cd213");
		public static Brush MyColorEmu { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#E35627");
		public static Brush MyColorSCL { get; private set; } = (Brush)new BrushConverter().ConvertFromString("#4cd213");


		/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// All other colors:
		/// App = App_Wide styles
		/// MW = MainWindow
		/// PU = Pop Up
		/// SE = Settings
		/// SFH = SafeFileHandler
		/// MO = Mouse Over
		/// DG = Data Grid
		/// </summary>
		/// 


		// App - Wide stuff
		public static Brush App_LabelForeground { get; private set; } = MyColorWhite;

		public static Thickness App_ButtonBorderThickness { get; private set; } = new Thickness(0);

		public static Brush App_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush App_ButtonBackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush App_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush App_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush App_ButtonMOForeground { get; private set; } = MyColorBlack;
		public static Brush App_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		public static Brush App_Submenu_Background { get; private set; } = SetOpacity(MyColorBlack, 65);

		public static Brush SM_ButtonBackground { get; private set; } = MyColorOffBlack;
		public static Brush SM_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush SM_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush SM_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush SM_ButtonMOForeground { get; private set; } = MyColorOffBlack;
		public static Brush SM_ButtonMOBorderBrush { get; private set; } = Brushes.Transparent;
		public static System.Windows.Thickness SM_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);


		public static Brush App_ScrollViewerForeground { get; private set; } = MyColorOffWhite;



		public static Thickness App_ButtonSmallBorderThickness { get; private set; } = new Thickness(0);
		public static Brush App_ButtonSmallBorderBrush { get; private set; } = MyColorWhite;


		// MainWindow
		public static Thickness MW_BorderThickness { get; private set; } = new System.Windows.Thickness(2);
		public static Brush MW_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_HamburgerMenuGridBackground { get; private set; } = SetOpacity(MyColorBlack, 65);
		public static Brush MW_HamburgerMenuSeperatorBrush { get; private set; } = MyColorWhite;

		public static Thickness MW_ButtonHamburgerMenuBorderThickness { get; private set; } = new Thickness(0);
		public static Brush MW_ButtonHamburgerMenuBorderBrush { get; private set; } = App_ButtonBorderBrush;
		public static Brush MW_ButtonHamburgerMenuBackground { get; private set; } = App_ButtonBackground;
		public static Brush MW_ButtonHamburgerMenuForeground { get; private set; } = App_ButtonForeground;
		public static Brush MW_ButtonHamburgerMenuMOBackground { get; private set; } = App_ButtonMOBackground;
		public static Brush MW_ButtonHamburgerMenuMOForeground { get; private set; } = App_ButtonMOForeground;
		public static Brush MW_ButtonHamburgerMenuMOBorderBrush { get; private set; } = App_ButtonMOBorderBrush;


		// GTA Launch Button
		// Border Color will depend on game running or not running, so we will not set this here. I guess. 
		public static System.Windows.Thickness MW_ButtonGTABorderThickness { get; private set; } = new System.Windows.Thickness(5);
		public static Brush MW_ButtonGTAGameNotRunningBorderBrush { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAGameRunningBorderBrush { get; private set; } = MyColorGreen;
		public static Brush MW_ButtonGTABackground { get; private set; } = SetOpacity(MyColorBlack, 70);
		public static Brush MW_ButtonGTAForeground { get; private set; } = MyColorWhite;
		public static Brush MW_ButtonGTAMOBackground { get; private set; } = SetOpacity(MyColorOffWhite, 100);
		public static Brush MW_ButtonGTAMOForeground { get; private set; } = MyColorBlack;

		// GTA Label (Upgraded, Downgrad, Unsure etc.
		public static Brush MW_GTALabelDowngradedForeground { get; private set; } = MyColorGreen;
		public static Brush MW_GTALabelUpgradedForeground { get; private set; } = Brushes.White;
		public static Brush MW_GTALabelUnsureForeground { get; private set; } = Brushes.Red;





		// POPUP Window
		public static Brush PU_Background { get; private set; } = MyColorOffBlack;
		public static Brush PU_BorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_LabelForeground { get; private set; } = MyColorWhite;

		public static Brush PU_ButtonBackground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonForeground { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonBorderBrush { get; private set; } = MyColorWhite;
		public static Brush PU_ButtonMOBackground { get; private set; } = MyColorOffWhite;
		public static Brush PU_ButtonMOForeground { get; private set; } = MyColorOffBlack;
		public static Brush PU_ButtonMOBorderBrush { get; private set; } = MyColorOffBlack;
		public static System.Windows.Thickness PU_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);

		public static Brush ProgressBarBackground { get; private set; } = MyColorOffBlack;
		public static Brush ProgressBarForeground { get; private set; } = MyColorOffWhite;
		public static Brush ProgressBarBorderBrush { get; private set; } = MyColorOffWhite;

		public static Brush DropDownBackground { get; private set; } = MyColorBlack;
		public static Brush DropDownForeground { get; private set; } = MyColorOffWhite;
		public static Brush DropDownPopDownBackground { get; private set; } = MyColorBlack;
		public static Brush DropDownBorderBrush { get; private set; } = MyColorWhite;


		// SaveFilerHandler Window


		public static Brush SFH_DGBorderBrush { get; private set; } = MyColorWhite;
		public static Thickness SFH_DGBorderThickness { get; private set; } = new Thickness(2);
		public static Brush SFH_DGHeaderBackground { get; private set; } = MyColorOffWhite;
		public static Brush SFH_DGHeaderForeground { get; private set; } = MyColorBlack;
		public static Brush SFH_DGBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SFH_DGRowBackground { get; private set; } = Brushes.Transparent;
		public static Brush SFH_DGAlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite, 20);
		public static Brush SFH_DGForeground { get; private set; } = MyColorWhite;
		public static Brush SFH_DGCellBorderBrush { get; private set; } = Brushes.Transparent;
		public static Thickness SFH_DGCellBorderThickness { get; private set; } = new Thickness(0);
		//public static Brush SFH_DGSelectedBackground { get; private set; } = Brushes.Transparent;
		//public static Brush SFH_DGSelectedForeground { get; private set; } = GetBrushHex("#76e412");
		//public static Brush SFH_DGSelectedBorderBrush { get; private set; } = GetBrushHex("#76e412");
		public static Brush SFH_DGSelectedBackground { get; private set; } = SetOpacity(MyColorWhite, 80);
		public static Brush SFH_DGSelectedForeground { get; private set; } = MyColorOffBlack;
		public static Brush SFH_DGSelectedBorderBrush { get; private set; } = MyColorOffWhite;
		public static Thickness SFH_DGSelectedBorderThickness { get; private set; } = new Thickness(2);

		//GetBrushRGB(226, 0, 116);

		// Settings Window

		public static Brush SE_RowBackground { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush SE_AlternateRowBackground { get; private set; } = SetOpacity(MyColorOffWhite, 20);
		public static Brush SE_BorderBrush_Inner { get; private set; } = MyColorWhite;

		public static Brush SE_Lbl_Header_Background { get; private set; } = MyColorOffWhite;
		public static Brush SE_Lbl_Header_Foreground { get; private set; } = MyColorOffBlack;


		// ReadMe Window

		public static Brush ReadME_Inner_Background { get; private set; } = SetOpacity(MyColorBlack, 50);
		public static Brush ReadME_Inner_BorderBrush { get; private set; } = MyColorWhite;
		public static Thickness ReadME_Inner_BorderThickness { get; private set; } = new Thickness(2);
		public static CornerRadius ReadME_Inner_CornerRadius { get; private set; } = new CornerRadius(10);

		// Using a lot of settings stuff (grid-background, grid second / alternative row color, button styles inside grid) on the noteoverlay...whatevs XD

		public static Brush NO_Slider_Track_Brush { get; private set; } = MyColorOffWhite;
		public static Brush NO_Slider_Thumb_Brush { get; private set; } = MyColorOffWhite;
		public static Brush NO_Slider_Thumb_MO_Brush { get; private set; } = MyColorOffBlack;


		//public static Brush SE_LabelForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_LabelSetForeground { get; private set; } = MyColorWhite;

		//public static Brush SE_ButtonBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonBorderBrush { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonMOBackground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonMOForeground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonMOBorderBrush { get; private set; } = MyColorWhite;

		//public static Brush SE_ButtonSetBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonSetForeground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetBorderBrush { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetMOBackground { get; private set; } = MyColorWhite;
		//public static Brush SE_ButtonSetMOForeground { get; private set; } = MyColorBlack;
		//public static Brush SE_ButtonSetMOBorderBrush { get; private set; } = MyColorWhite;

		//public static Brush SE_SVBackground { get; private set; } = MyColorBlack;
		//public static Brush SE_SVForeground { get; private set; } = MyColorWhite;

		//public static System.Windows.Thickness SE_ButtonBorderThickness { get; private set; } = new System.Windows.Thickness(2);





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
			return SetOpacity(rtrn, pOpacity);
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
			return GetBrushRGB(r, g, b, 100);
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
	}
}
