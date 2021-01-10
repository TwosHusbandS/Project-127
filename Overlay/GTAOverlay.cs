using GameOverlay.Drawing;
using GameOverlay.Windows;
using GSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Image = GameOverlay.Drawing.Image;

namespace Project_127.Overlay
{

	public class GTAOverlay : IDisposable
	{
		// If set to false, this starts and keeps KeyboardListenerEvent running 100% of the time.
		// Automatically set to true if we compile debug

		public static bool DebugMode = false;
		public const string targetWindowDebug = "TeamSpeak 3";
		public const string targetWindowBorderless = "Grand Theft Auto V";
		public const string targetWindowBorderlessEasterEgg = "Stealy Whealy Automobiley";
		public const string targetWindowMultiMonitor = "P127 - GameOverlay";

		public static bool indicateTheLessThanLegalProcurementOfMotorVehicles = false;

		public static string targetWindow
		{
			get
			{
				if (DebugMode)
				{
					return targetWindowDebug;
				}
				else
				{
					if (OverlayMode == OverlayModes.Borderless)
					{
						return targetWindowBorderless;
					}
					else if (OverlayMode == OverlayModes.MultiMonitor)
					{
						return targetWindowMultiMonitor;
					}
					else
					{
						if (indicateTheLessThanLegalProcurementOfMotorVehicles)
						{
							return targetWindowBorderlessEasterEgg;
						}
						else
						{
							return targetWindowBorderless;
						}
					}

				}
			}
		}




		private readonly GraphicsWindow _window;

		[DllImport("dwmapi.dll")]
		private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[Flags]
		private enum DwmWindowAttribute : uint
		{
			DWMWA_NCRENDERING_ENABLED = 1,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_CLOAK,
			DWMWA_CLOAKED,
			DWMWA_FREEZE_REPRESENTATION,
			DWMWA_LAST
		}

		private static RECT GetWindowRectangle(IntPtr hWnd)
		{
			RECT rect;

			int size = Marshal.SizeOf(typeof(RECT));
			DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

			return rect;
		}



		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Image> _images;
		private readonly List<overlayObject> overlayObjects;
		private float bgImageOpac = (float).7;
		private string bgImagePath = "";
		private int scrollInitial = 50;
		private overlayTextBox mainText, titleBox;

		public positionalText title { get; private set; }


		/// <summary>
		/// Internal. Determines the positioning of the overlay.
		/// </summary>
		private Positions _Position { get; set; } = Positions.TopLeft;

		/// <summary>
		/// Determines the positioning of the overlay.
		/// </summary>
		public Positions Position
		{
			get
			{
				return _Position;
			}
			set
			{
				if (OverlayMode == OverlayModes.Borderless)
				{
					_Position = value;
				}
			}
		}

		/// <summary>
		/// Determines the text content of the oberlay.
		/// </summary>
		public string text
		{
			get
			{
				return mainText.text;
			}
			set
			{
				mainText.text = value;
			}
		}

		/// <summary>
		/// Overrides the positioning of line wrap. (Disabled by vals <1)
		/// </summary>
		public int WrapCount
		{
			get
			{
				return mainText.wrapOnChar;
			}
			set
			{
				mainText.wrapOnChar = value;
			}
		}

		/// <summary>
		/// Determines the base offset of the overlay.
		/// </summary>
		public int PaddingSize { get; set; } = 0;

		/// <summary>
		/// Internal. Determines the X offset of the overlay (from padding position).
		/// </summary>
		private int _XMargin { get; set; } = 0;

		/// <summary>
		/// Determines the X offset of the overlay (from padding position).
		/// </summary>
		public int XMargin
		{
			get
			{
				return _XMargin;
			}
			set
			{
				if (OverlayMode == OverlayModes.Borderless)
				{
					_XMargin = value;
				}
			}
		}

		/// <summary>
		/// Internal. Determines the Y offset of the overlay (from padding position.
		/// </summary>
		private int _YMargin { get; set; } = 0;

		/// <summary>
		/// Determines the Y offset of the overlay (from padding position).
		/// </summary>
		public int YMargin
		{
			get
			{
				if (OverlayMode == OverlayModes.MultiMonitor)
				{
					if (MainWindow.OL_MM != null)
					{
						return (int)MainWindow.OL_MM.ActualHeight;
					}
					return 80;
				}
				else
				{
					return _YMargin;
				}
			}
			set
			{
				if (OverlayMode == OverlayModes.Borderless)
				{
					_YMargin = value;
				}
			}
		}

		/// <summary>
		/// Determines whether or not the background image is used.
		/// </summary>
		public bool UseBackground { get; set; } = false;

		/// <summary>
		/// Determines whether or not the background image should fill the whole overlay.
		/// </summary>
		public bool UseImageFill { get; set; } = false;

		private int _width = 0, _height = 0;

		/// <summary>
		/// Gets/Sets overlay height.
		/// </summary>
		public int height
		{
			get
			{
				return _window.Height;
			}
			set
			{
				_height = value;
			}
		}

		/// <summary>
		/// Gets/Sets overlay width.
		/// </summary>
		public int width
		{
			get
			{
				return _window.Width;
			}
			set
			{
				_width = value;
				if (OverlayMode == OverlayModes.MultiMonitor)
				{
					if (MainWindow.OL_MM != null)
					{
						double ration = MainWindow.OL_MM.ActualWidth / MainWindow.OL_MM.Width;
						MainWindow.OL_MM.Width = value / ration;
					}
				}
			}
		}

		/// <summary>
		/// Enum for the Parameter which determines if we are a fullscreen overlay or a multi monitor overlay
		/// </summary>
		public enum OverlayModes
		{
			Borderless,
			MultiMonitor
		}

		/// <summary>
		/// determines if we are a fullscreen overlay or a multi monitor overlay
		/// </summary>
		public static OverlayModes OverlayMode
		{
			get
			{
				if (MySettings.Settings.OverlayMultiMonitorMode)
				{
					return GTAOverlay.OverlayModes.MultiMonitor;
				}
				else
				{
					return GTAOverlay.OverlayModes.Borderless;
				}
			}
		}

		/// <summary>
		/// Generates the game overlay
		/// </summary>
		/// <param name="overlayMode">Enum if we are fullscreen or MultiMonitor Mode</param>
		/// <param name="position">The screen positioning (TopLeft, BottomRight, etc.)</param>
		/// <param name="width">The horizontal resolution</param>
		/// <param name="height">The vertical resolution</param>
		public GTAOverlay(Positions position = Positions.TopLeft, int width = 560, int height = 380)
		{
			HelperClasses.Logger.Log("Game Overlay Initiated");

			_brushes = new Dictionary<string, SolidBrush>();
			_images = new Dictionary<string, Image>();
			overlayObjects = new List<overlayObject>();
			//var wb = new WindowBounds();

			HelperClasses.Logger.Log("Searching for '" + targetWindow + "' window...");

			var windowHandle = WindowHelper.FindWindow(targetWindow);
			if (windowHandle == IntPtr.Zero)
			{
				HelperClasses.Logger.Log("Failed to find '" + targetWindow + "' window.");
			}
			else
			{
				HelperClasses.Logger.Log("'" + targetWindow + "' window found.");
			}
			//WindowHelper.GetWindowBounds(windowHandle, out wb);
			var gfx = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};
			var pos = coordFromPos(position, GetWindowRectangle(windowHandle), width, height);
			_window = new GraphicsWindow(pos[0], pos[1], width, height, gfx)
			{
				FPS = 30,
				IsTopmost = true,
				IsVisible = true
			};

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;

			this.Run();
			this.Visible = false;
			mainText = new overlayTextBox("GTAOVERLAY_MAIN");
			attach(mainText);
			mainText.visible = true;
			titleBox = new overlayTextBox("title");
			titleBox.text = "Project 1.27 GTA Overlay";
			titleBox.visible = true;
			this.attach(titleBox);
			title = titleBox;
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;
			if (e.RecreateResources)
			{
				foreach (var pair in _brushes) pair.Value.Dispose();
				foreach (var pair in _images) pair.Value.Dispose();
			}

			_brushes["textBack"] = gfx.CreateSolidBrush(0, 0, 0, (int)(.4 * 255));
			_brushes["textColor"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["background"] = gfx.CreateSolidBrush(0, 0, 0, 0);

			if (e.RecreateResources) return;

			//_fonts["arial"] = gfx.CreateFont("Arial", 12);
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			//var wb = new WindowBounds();
			//WindowHelper.GetWindowBounds(WindowHelper.FindWindow(targetWindow), out wb);
			var pos = coordFromPos(Position, GetWindowRectangle(WindowHelper.FindWindow(targetWindow)), _window.Width, _window.Height);
			_window.X = pos[0];
			_window.Y = pos[1];
			if (width != _width || height != _height)
			{
				if (_width == 0)
				{
					_width = _window.Width;
				}
				if (_height == 0)
				{
					_height = _window.Height;
				}
				_window.Resize(_width, _height);
			}
			var gfx = e.Graphics;
			gfx.ClearScene(_brushes["background"]);
			if (UseBackground && _images.ContainsKey("bgImage"))
			{
				if (UseImageFill)
				{
					gfx.DrawImage(_images["bgImage"], new Rectangle(0, 0, e.Graphics.Width, e.Graphics.Height), bgImageOpac, true);
				}
				else
				{
					gfx.DrawImage(_images["bgImage"], 0, 0, bgImageOpac);
				}
			}
			else if (bgImagePath != "")
			{
				try
				{
					_images["bgImage"] = new Image(gfx, bgImagePath);
				}
				catch
				{
					HelperClasses.Logger.Log("Image loading failed");
					bgImagePath = "";
				}
			}
			//gfx.DrawTextWithBackground(_fonts["textFont"], _brushes["textColor"], _brushes["textBack"], textOffsetX, textOffsetY, NoteText);

			foreach (var obj in overlayObjects)
			{
				obj.render(gfx);
			}

		}

		/// <summary>
		/// Enables the overlay
		/// </summary>
		public async void Run()
		{
			await Task.Run(() => _window.Create());
			//_window.Join();
		}

		/// <summary>
		/// Sets the text color & text background color
		/// </summary>
		/// <param name="textColor">Color of the text</param>
		/// <param name="textBG">Color of the text background</param>
		public async void setTextColors(System.Drawing.Color textColor, System.Drawing.Color textBG)
		{
			mainText.textColor = textColor;
			mainText.bgColor = textBG;
			title.textColor = textColor;
			title.bgColor = textBG;
		}

		/// <summary>
		/// Sets the path for the background image.
		/// </summary>
		/// <param name="path">Path for the background image file</param>
		public void setBgImage(string path)
		{
			if (System.IO.File.Exists(path))
			{
				//bgImage = new Image(_window.Graphics, path);
				bgImagePath = path;
			}
		}

		/// <summary>
		/// Sets the text content of the overlay.
		/// </summary>
		/// <param name="text">Text to display</param>
		public void setText(string text)
		{
			HelperClasses.Logger.Log("Overlay text updated");
			this.text = text;
			//<<<<<<< working
			//			this.text = text;
			//=======
			//			mainText.text = text;
			//>>>>>>> dev_ths_new
		}

		/// <summary>
		/// Sets the background color of the overlay.
		/// </summary>
		/// <param name="color">Background color</param>
		public async void setBackgroundColor(System.Drawing.Color color)
		{
			await graphicsReady();
			_brushes["background"].Color = Color.FromARGB(color.ToArgb());
		}

		/// <summary>
		/// Sets the overlay font
		/// </summary>
		/// <param name="fontFamily">Font family</param>
		/// <param name="fontSize">Font size in px</param>
		/// <param name="bold">Determines if bold</param>
		/// <param name="italic">Determines if italic</param>
		/// <param name="wordWrap">Enables auto line wrap</param>
		public async void setFont(string fontFamily, int fontSize, bool bold = false, bool italic = false, bool wordWrap = true)
		{
			mainText.setFont(fontFamily, fontSize, bold, italic, wordWrap);
			title.setFont(fontFamily, fontSize + 4, true, italic, wordWrap);
		}

		/// <summary>
		/// Sets the render offset for text.
		/// </summary>
		/// <param name="x">X Offset</param>
		/// <param name="y">Y Offset</param>
		public void setTextPosition(int x, int y)
		{
			mainText.position = new Point(x, y);
		}

		/// <summary>
		/// Represents the point at which text is rendered.
		/// </summary>
		public Point TextRenderPoint
		{
			get
			{
				return mainText.position;
			}
			set
			{
				mainText.position = value;
			}
		}

		private async Task<bool> graphicsReady() //Bool is just so it can be an awaitable task
		{
			while (!_window.IsInitialized || !_window.Graphics.IsInitialized)
			{
				await Task.Delay(250);
			}
			return true;
		}

		/// <summary>
		/// Determines the opacity of the background image (if it is enabled).
		/// </summary>
		public double BackgroundImageOpacity
		{
			get
			{
				return bgImageOpac;
			}
			set
			{
				bgImageOpac = (float)value;
			}
		}

		/// <summary>
		/// Sets the initial point for scrolling render.
		/// </summary>
		public void SetInitialScrollPosition()
		{
			scrollInitial = (int)mainText.position.Y;
		}

		/// <summary>
		/// Sets the initial point for scrolling render.
		/// </summary>
		/// <param name="y">Initial scroll y position</param>
		public void SetInitialScrollPosition(int y)
		{
			scrollInitial = y;
			scroll(9999);
		}

		/// <summary>
		/// Scrolls the text by a given number of pixels
		/// </summary>
		/// <param name="delta">Number of pixels to scroll</param>
		public async void scroll(int delta)
		{
			await graphicsReady();
			var aprx = mainText.approxBounds();
			int boundMin = (int)(scrollInitial - aprx.Height);
			//System.Windows.MessageBox.Show(aprx.Height.ToString());
			_window.Graphics.Height = (int)(aprx.Height > _window.Height ? aprx.Height * 2.1 : _window.Height);
			//System.Windows.MessageBox.Show(_window.Graphics.Height.ToString());
			var pos = mainText.position;
			pos.Y += delta;
			if (pos.Y > scrollInitial)
			{
				pos.Y = scrollInitial;
			}
			if (pos.Y < boundMin)
			{
				pos.Y = boundMin;
			}
			mainText.position = pos;
		}

		/// <summary>
		/// Determines whether or not the overlay is visible.
		/// </summary>
		public bool Visible
		{
			get
			{
				return _window.IsVisible;
			}
			set
			{
				_window.IsVisible = value;
				HelperClasses.Logger.Log("Set Overlay Visibility to " + this.Visible);
			}
		}
		private int[] coordFromPos(Positions p, RECT wb, int resx, int resy)
		{
			resx += PaddingSize + XMargin;
			resy += PaddingSize + YMargin;
			var coords = new int[2];
			int xborder = System.Windows.Forms.SystemInformation.FrameBorderSize.Width;
			int yborder = System.Windows.Forms.SystemInformation.FrameBorderSize.Height;
			switch (p)
			{
				case Positions.TopLeft:
					coords[1] = wb.Top + PaddingSize + YMargin;
					coords[0] = wb.Left + PaddingSize + XMargin;
					break;
				case Positions.TopRight:
					coords[1] = wb.Top + PaddingSize + YMargin;
					coords[0] = wb.Right - resx - xborder;
					break;
				case Positions.BottomLeft:
					coords[1] = wb.Bottom - resy - yborder;
					coords[0] = wb.Left + PaddingSize + XMargin + xborder;
					break;
				case Positions.BottomRight:
					coords[1] = wb.Bottom - resy - yborder;
					coords[0] = wb.Right - resx - xborder;
					break;
				default:
					goto case Positions.TopLeft;
			}
			return coords;
		}

		public enum Positions
		{
			TopLeft,
			TopRight,
			BottomRight,
			BottomLeft
		}

		public bool attach(overlayObject t)
		{
			if (t.linked)
			{
				return false;
			}
			t.link(this, _window.Graphics);
			overlayObjects.Add(t);
			return true;
		}

		public bool detach(string id)
		{
			var idx = overlayObjects.FindIndex(x => x.id == id);
			if (idx == -1)
			{
				return false;
			}
			overlayObjects[idx].unlink();
			overlayObjects.RemoveAt(idx);
			return true;
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_window.Dispose();
				mainText.Dispose();
				titleBox.Dispose();
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
	public interface overlayObject : IDisposable
	{
		//interface void render();

		/// <summary>
		/// Determines the position of the object.
		/// </summary>
		Point position { get; set; }

		/// <summary>
		/// Determines whether or not the object will render.
		/// </summary>
		bool visible { get; set; }

		/// <summary>
		/// Links the object to the Overlay.
		/// </summary>
		/// <param name="O">Overlay Object</param>
		/// <param name="G">Graphics Object</param>
		void link(GTAOverlay O, Graphics G);

		/// <summary>
		/// Unlinks the object from the Overlay.
		/// </summary>
		void unlink();

		/// <summary>
		/// Renders the object.
		/// </summary>
		/// <param name="g">Graphics object to render to</param>
		void render(Graphics g);

		/// <summary>
		/// Identifier of the object.
		/// </summary>
		string id { get; }

		/// <summary>
		/// Indicates whether the object is linked.
		/// </summary>
		bool linked { get; }

	}

	public interface positionalText
	{
		/// <summary>
		/// Determines the position of the object.
		/// </summary>
		Point position { get; set; }

		/// <summary>
		/// Determines whether or not the text will render.
		/// </summary>
		bool visible { get; set; }

		/// <summary>
		/// Determines the max width of a line in pixels
		/// </summary>
		int maxLineWidth { get; set; }

		/// <summary>
		/// Determines the maximum number of chars before character wrap (<1 disables by-char wrapping)
		/// </summary>
		int wrapOnChar { get; set; }

		/// <summary>
		/// Sets the textbox font
		/// </summary>
		/// <param name="fontFamily">Font family</param>
		/// <param name="fontSize">Font size in px</param>
		/// <param name="bold">Determines if bold</param>
		/// <param name="italic">Determines if italic</param>
		/// <param name="wordWrap">Enables auto line wrap</param>
		void setFont(string fontFamily, int fontSize, bool bold, bool italic, bool wordWrap);

		/// <summary>
		/// Determines the text content of the object.
		/// </summary>
		string text { get; set; }

		/// <summary>
		/// Determines the color of the text.
		/// </summary>
		System.Drawing.Color textColor { get; set; }

		/// <summary>
		/// Determines the color of the text background.
		/// </summary>
		System.Drawing.Color bgColor { get; set; }

	}

	public class overlayTextBox : overlayObject, positionalText
	{

		private int _maxLineWidth = 0;

		/// <summary>
		/// Determines the maximum number of chars before character wrap (<1 disables by-char wrapping)
		/// </summary>
		public int wrapOnChar { get; set; } = -1;

		private string charWrap(string wtext, int lmax)
		{
			var lines = new List<string>();
			wtext = wtext.Replace("\r\n", "\n");
			var ilines = wtext.Split('\n');
			foreach (string line in ilines)
			{
				if (line.Length > lmax)
				{
					var i = lmax - 1;
					while (line.ToCharArray()[i] != ' ' && (i > 0))
					{
						i--;
					}
					if (i == 0)
					{
						i = lmax - 1;
					}
					lines.Add(line.Substring(0, i + 1));
					lines.Add(charWrap(line.Substring(i + 1), lmax));
				}
				else
				{
					lines.Add(line);
				}
			}
			return String.Join("\r\n", lines);
		}

		/// <summary>
		/// Determines the max width of a line in pixels
		/// </summary>
		public int maxLineWidth
		{
			get
			{
				if ((gfx.Width - position.X) < _maxLineWidth)
				{
					return gfx.Width - (int)position.X;
				}
				return _maxLineWidth;
			}
			set
			{
				textUpdate = true;
				_maxLineWidth = value;
			}
		}

		private string _text = "";

		/// <summary>
		/// Determines the text content of the textbox.
		/// </summary>
		public string text
		{
			get
			{
				if (wrapOnChar > 0)
				{
					return charWrap(_text, wrapOnChar);
				}
				if (maxLineWidth > 0)
				{
					return autowrap;
				}
				return _text;
			}
			set
			{
				_text = value;
				textUpdate = true;
			}
		}

		private bool _textUpdate = true;

		private bool textUpdate
		{
			get
			{
				return _textUpdate;
			}
			set
			{
				_textUpdate = value;
				if (value)
				{
					Task.Run(async () => approxBounds(true));
				}
			}
		}

		private System.Drawing.Font SDFont
		{
			get
			{
				return new System.Drawing.Font(storedFont.fontFamily,
					storedFont.fontSize,
					(storedFont.bold ? System.Drawing.FontStyle.Bold : 0) |
					(storedFont.italic ? System.Drawing.FontStyle.Italic : 0),
					System.Drawing.GraphicsUnit.Pixel);
			}
		}

		private string _autowrap;

		private string autowrap
		{
			get
			{
				if (textUpdate)
				{
					textUpdate = false;
					var lines = _text.Split('\n');
					var outLines = new List<string>();
					foreach (var line in lines)
					{
						outLines.AddRange(autoWrapper(line));
					}
					_autowrap = String.Join("\n", outLines);
				}
				return _autowrap;
			}
		}



		private List<string> autoWrapper(string line)
		{
			var lines = new List<string>();
			if (line == "")
			{
				return lines;
			}
			int blength = line.Length, slength;
			while (System.Windows.Forms.TextRenderer.MeasureText(line.Substring(0, blength), SDFont).Width > maxLineWidth)
			{
				blength--;
			}
			if (blength == line.Length)
			{
				lines.Add(line);
				return lines;
			}
			slength = blength;
			while (!char.IsWhiteSpace(line[slength - 1]) && slength > 1)
			{
				slength--;
			}
			if (slength == 1)
			{
				slength = blength;
			}
			lines.Add(line.Substring(0, slength));
			lines.AddRange(autoWrapper(line.Substring(slength)));
			return lines;
		}

		public Point position { get; set; }

		public bool visible { get; set; }

		private Color _textColor = Color.Green, _bgColor = Color.Transparent;

		private SolidBrush _textBrush = null, _bgBrush = null;

		private SolidBrush textBrush
		{
			get
			{
				if (gfx == null)
				{
					return null;
				}
				if (_textBrush == null)
				{
					_textBrush = gfx.CreateSolidBrush(_textColor);
				}
				_textBrush.Color = _textColor;
				return _textBrush;
			}
		}

		private SolidBrush bgBrush
		{
			get
			{
				if (gfx == null)
				{
					return null;
				}
				if (_bgBrush == null)
				{
					_bgBrush = gfx.CreateSolidBrush(_bgColor);
				}
				_bgBrush.Color = _bgColor;
				return _bgBrush;
			}
		}

		private Font _textFont = null;

		private Font currentFont
		{
			get
			{
				if (gfx == null)
				{
					return null;
				}
				if (_textFont == null || storedFont.updated)
				{
					if (_textFont != null)
					{
						_textFont.Dispose();
					}
					_textFont = gfx.CreateFont(storedFont.fontFamily, storedFont.fontSize, storedFont.bold, storedFont.italic, storedFont.wordWrap);
				}
				return _textFont;
			}
		}

		public bool linked { get; private set; } = false;

		private Graphics gfx { get; set; } = null;

		private fontProperties storedFont = new fontProperties();

		private class fontProperties
		{
			private bool _updated = true;
			public bool updated
			{
				get
				{
					if (_updated)
					{
						_updated = false;
						return true;
					}
					return false;
				}
				set
				{
					_updated = value;
				}
			}
			public string fontFamily = "Consolas";
			public int fontSize = 24;
			public bool bold = false;
			public bool italic = false;
			public bool wordWrap = false;
		}

		/// <summary>
		/// Determines the color of the text.
		/// </summary>
		public System.Drawing.Color textColor
		{
			get
			{
				return System.Drawing.Color.FromArgb(_textColor.ToARGB());
			}
			set
			{
				_textColor = Color.FromARGB(value.ToArgb());
			}
		}

		/// <summary>
		/// Determines the color of the text background.
		/// </summary>
		public System.Drawing.Color bgColor
		{
			get
			{
				return System.Drawing.Color.FromArgb(_bgColor.ToARGB());
			}
			set
			{
				_bgColor = Color.FromARGB(value.ToArgb());
			}
		}


		public string id { get; private set; }

		private GTAOverlay host { set; get; } = null;

		/// <summary>
		/// Generates an overlayTextBox object.
		/// </summary>
		/// <param name="id">Textbox object id</param>
		public overlayTextBox(string id)
		{
			this.id = id;
		}

		/// <summary>
		/// Sets the textbox font
		/// </summary>
		/// <param name="fontFamily">Font family</param>
		/// <param name="fontSize">Font size in px</param>
		/// <param name="bold">Determines if bold</param>
		/// <param name="italic">Determines if italic</param>
		/// <param name="wordWrap">Enables auto line wrap</param>
		public void setFont(string fontFamily, int fontSize, bool bold = false, bool italic = false, bool wordWrap = true)
		{
			storedFont.fontFamily = fontFamily;
			storedFont.fontSize = fontSize;
			storedFont.bold = bold;
			storedFont.italic = italic;
			storedFont.wordWrap = wordWrap;
			storedFont.updated = true;
			textUpdate = true;
		}

		public void render(Graphics gfx = null)
		{
			//gfx.DrawTextWithBackground(currentFont, tb,
			if (gfx == null || text == null)
			{
				return;
			}
			if (visible)
			{
				gfx.DrawTextWithBackground(currentFont, textBrush, bgBrush, position, text);
			}
		}

		public void link(GTAOverlay host, Graphics gfx)
		{
			this.host = host;
			this.gfx = gfx;
			linked = true;
		}

		public void unlink()
		{
			linked = false;
			host = null;
			gfx = null;
		}

		private System.Drawing.Size _approxBounds = new System.Drawing.Size(-1, -1);

		/// <summary>
		/// Provides an approximation for the overlay size.
		/// </summary>
		/// <returns>Approximated size</returns>
		public System.Drawing.Size approxBounds()
		{
			return approxBounds(_approxBounds.Width == -1);
		}

		private System.Drawing.Size approxBounds(bool update)
		{
			if (update)
			{
				_approxBounds = System.Windows.Forms.TextRenderer.MeasureText(
					text,
					SDFont,
					new System.Drawing.Size(maxLineWidth, maxLineWidth),
					storedFont.wordWrap ? System.Windows.Forms.TextFormatFlags.WordBreak | System.Windows.Forms.TextFormatFlags.TextBoxControl : 0);
			}
			return _approxBounds;
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_textFont.Dispose();
				_textBrush.Dispose();
				_bgBrush.Dispose();
				if (host != null)
				{
					host.detach(id);
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}

}
