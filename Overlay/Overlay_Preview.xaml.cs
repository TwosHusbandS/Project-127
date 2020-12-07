using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Project_127.MySettings;
using Color = System.Drawing.Color;

namespace Project_127.Overlay
{
	/// <summary>
	/// Interaction logic for Overlay_Preview.xaml
	/// </summary>
	public partial class Overlay_Preview : Page
	{
		public static Overlay_Preview OP;

		private static DispatcherTimer MyDispatcherTimer;

		private static bool IsDispatcherTimerRunning = true;

		// Starting at 1 here btw...dont judge me if you read this :D

		private static int PreviewIndex = 1;
		private static int PreviewIndexMin = 1;
		private static int PreviewIndexMax = 10;

		

		private int PreviewMarginX;
		private int PreviewMarginY;

		public Overlay_Preview()
		{
			InitializeComponent();

			Overlay_Preview.OP = this;

			SetButtonMouseOverMagic(btn_LeftArrow);
			SetButtonMouseOverMagic(btn_RightArrow);
			SetButtonMouseOverMagic(btn_PlayPause);

			SetBackground();
			SetForeground();
			SetLocation();
			SetMarginX();
			SetMarginY();
			SetWidth();
			SetHeight();
			SetFont();
			SetTextSize();

			Preview.Content = "What is Lorem Ipsum? Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. Why do we use it? It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like). Where does it come from? Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32. The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. Sections 1.10.32 and 1.10.33 from \"de Finibus Bonorum et Malorum\" by Cicero are also reproduced in their exact original form, accompanied by English versions from the 1914 translation by H. Rackham. Where can I get some? There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.";

			SetPreviewBackground();


			MyDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			MyDispatcherTimer.Tick += new EventHandler(MyDispatcherTimerTick);
			MyDispatcherTimer.Interval = TimeSpan.FromMilliseconds(2500);
			MyDispatcherTimer.Start();
		}

		private void MyDispatcherTimerTick(object sender, EventArgs e)
		{
			SetPreviewBackgroundNext();
		}

		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender);
		}

		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			SetButtonMouseOverMagic((Button)sender);
		}


		public static void StopDispatcherTimer()
		{
			if (MyDispatcherTimer != null)
			{
				MyDispatcherTimer.Stop();
			}
		}

		public static void StartDispatcherTimer()
		{
			if (MyDispatcherTimer != null)
			{
				MyDispatcherTimer.Start();
			}
			PreviewIndex = 1;
			OP.SetPreviewBackground();
		}

		public void SetButtonMouseOverMagic(Button myBtn)
		{
			switch (myBtn.Name)
			{
				case "btn_RightArrow":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork\arrowright_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork\arrowright.png");
					}
					break;
				case "btn_LeftArrow":
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork\arrowleft_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, @"Artwork\arrowleft.png");
					}
					break;
				case "btn_PlayPause":
					string Path = @"Artwork\P";
					if (IsDispatcherTimerRunning)
					{
						Path += "ause";
					}
					else
					{
						Path += "lay";
					}
					if (myBtn.IsMouseOver)
					{
						MainWindow.MW.SetControlBackground(myBtn, Path + "_mo.png");
					}
					else
					{
						MainWindow.MW.SetControlBackground(myBtn, Path + ".png");
					}
					break;
			}
		}

		private void btn_PlayPause_Click(object sender, RoutedEventArgs e)
		{
			if (IsDispatcherTimerRunning)
			{
				IsDispatcherTimerRunning = false;
				MyDispatcherTimer.Stop();
				btn_PlayPause.ToolTip = "Start Slideshow";
			}
			else
			{
				IsDispatcherTimerRunning = true;
				MyDispatcherTimer.Start();
				btn_PlayPause.ToolTip = "Pause Slideshow";
			}
			SetButtonMouseOverMagic(btn_PlayPause);
		}

		private void btn_RightArrow_Click(object sender, RoutedEventArgs e)
		{
			SetPreviewBackgroundNext();
			if (IsDispatcherTimerRunning)
			{
				MyDispatcherTimer.Stop();
				MyDispatcherTimer.Start();
			}
		}

		private void btn_LeftArrow_Click(object sender, RoutedEventArgs e)
		{
			SetPreviewBackgroundPrev();
			if (IsDispatcherTimerRunning)
			{
				MyDispatcherTimer.Stop();
				MyDispatcherTimer.Start();
			}
		}


		public void SetFont()
		{
			Preview.FontFamily = new FontFamily(Settings.OverlayTextFont);
		}

		public void SetLocation()
		{
			switch (Settings.OverlayLocation)
			{
				case GTAOverlay.Positions.TopLeft:
					Preview.HorizontalAlignment = HorizontalAlignment.Left;
					Preview.VerticalAlignment = VerticalAlignment.Top;
					break;
				case GTAOverlay.Positions.TopRight:
					Preview.HorizontalAlignment = HorizontalAlignment.Right;
					Preview.VerticalAlignment = VerticalAlignment.Top;
					break;
				case GTAOverlay.Positions.BottomLeft:
					Preview.HorizontalAlignment = HorizontalAlignment.Left;
					Preview.VerticalAlignment = VerticalAlignment.Bottom;
					break;
				case GTAOverlay.Positions.BottomRight:
					Preview.HorizontalAlignment = HorizontalAlignment.Right;
					Preview.VerticalAlignment = VerticalAlignment.Bottom;
					break;
			}

			int MarginX = PreviewMarginX;
			int MarginY = PreviewMarginY;
			Thickness newMargin = new Thickness(0);

			if (Settings.OverlayLocation == GTAOverlay.Positions.TopLeft)
			{
				newMargin = new Thickness(MarginX, MarginY, 0, 0);
			}
			else if (Settings.OverlayLocation == GTAOverlay.Positions.TopRight)
			{
				newMargin = new Thickness(0, MarginY, MarginX, 0);
			}
			else if (Settings.OverlayLocation == GTAOverlay.Positions.BottomLeft)
			{
				newMargin = new Thickness(MarginX, 0, 0, MarginY);
			}
			else if (Settings.OverlayLocation == GTAOverlay.Positions.BottomRight)
			{
				newMargin = new Thickness(0, 0, MarginX, MarginY);
			}

			Preview.Margin = newMargin;
		}

		public void SetTextSize(int FontSize = -1)
		{
			if (FontSize == -1)
			{
				FontSize = Settings.OverlayTextSize;
			}
			FontSize = FontSize / 3;
			Preview.FontSize = FontSize;
		}

		public void SetHeight(int Height = -1)
		{
			if (Height == -1)
			{
				Height = Settings.OverlayHeight;
			}
			Height = Height / 3;
			Preview.Height = Height;
		}

		public void SetWidth (int Width = -1)
		{
			if (Width == -1)
			{
				Width = Settings.OverlayWidth;
			}
			Width = Width / 3;
			Preview.Width = Width;
		}

		public void SetMarginX (int MarginX = -1)
		{
			if (MarginX == -1)
			{
				MarginX = Settings.OverlayMarginX;
			}
			MarginX = MarginX / 3;
			this.PreviewMarginX = MarginX;
			SetLocation();
		}


		public void SetMarginY(int MarginY = -1)
		{
			if (MarginY == -1)
			{
				MarginY = Settings.OverlayMarginY;
			}
			MarginY = MarginY / 3;
			this.PreviewMarginY = MarginY;
			SetLocation();
		}


		public void SetPreviewBackgroundNext()
		{
			if (PreviewIndex == PreviewIndexMax)
			{
				PreviewIndex = PreviewIndexMin;
			}
			else
			{
				PreviewIndex += 1;
			}
			SetPreviewBackground();
		}

		public void SetPreviewBackgroundPrev()
		{
			if (PreviewIndex == PreviewIndexMin)
			{
				PreviewIndex = PreviewIndexMax;
			}
			else
			{
				PreviewIndex -= 1;
			}
			SetPreviewBackground();
		}

		public void SetPreviewBackground()
		{
			MainWindow.MW.SetControlBackground(Preview_BG, @"Artwork\Preview_" + PreviewIndex.ToString() + ".png");
			lbl_Header.Content = "Overview - Preview(" + PreviewIndex  + " / " + PreviewIndexMax + ")";
		}

		public void SetBackground()
		{
			SetBackground(Settings.OverlayBackground);
		}

		public void SetBackground(Color pColor)
		{
			Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, pColor.R, pColor.G, pColor.B));
			brush.Opacity = (double)((byte)pColor.A) / 255;
			Preview.Background = brush;
		}

		public void SetForeground()
		{
			SetForeground(Settings.OverlayForeground);
		}

		public void SetForeground(Color pColor)
		{
			Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, pColor.R, pColor.G, pColor.B));
			brush.Opacity = (double)((byte)pColor.A) / 255;
			Preview.Foreground = brush;
		}


	}
}
