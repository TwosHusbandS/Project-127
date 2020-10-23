using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Image = GameOverlay.Drawing.Image;

namespace Project_127
{
    public class GTAOverlay : IDisposable
    {
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

		private const string targetWindow = "Command Prompt";
		//private const string targetWindow = "Project - 1.27";
		//private const string targetWindow = "Grand Theft Auto V";
		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;
        private float bgImageOpac = (float).7;
        private string bgImagePath = "";
        private string NoteText = "";
		private int textOffsetX = 20;
		private int textOffsetY = 20;
		private int scrollInitial = 20;

		//// <summary>
		/// Determines the positioning of the overlay.
		/// </summary>
		public Positions Position { get; set; }

		//// <summary>
		/// Overrides the positioning of line wrap.
		/// </summary>
		public int WrapCount { get; set; } = int.MaxValue;

		//// <summary>
		/// Determines the base offset of the overlay.
		/// </summary>
		public int PaddingSize { get; set; } = 0;

		/// <summary>
		/// Determines the X offset of the overlay (from padding position).
		/// </summary>
		public int XMargin { get; set; } = 0;

		/// <summary>
		/// Determines the Y offset of the overlay (from padding position.
		/// </summary>
		public int YMargin { get; set; } = 0;

		/// <summary>
		/// Determines whether or not the background image is used.
		/// </summary>
		public bool UseBackground { get; set; } = false;

		/// <summary>
		/// Determines whether or not the background image should fill the whole overlay.
		/// </summary>
		public bool UseImageFill { get; set; } = false;

		/// <summary>
		/// Generates the game overlay
		/// </summary>
		/// <param name="position">The screen positioning (TopLeft, BottomRight, etc.)</param>
		/// <param name="width">The horizontal resolution</param>
		/// <param name="height">The vertical resolution</param>
		/// <param name="wrapNum">The number of characters before a line wrap</param>
		public GTAOverlay(Positions position = Positions.TopLeft, int width = 560, int height = 380)
		{
			HelperClasses.Logger.Log("Game Overlay Initiated");
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();
            //var wb = new WindowBounds();
			HelperClasses.Logger.Log("Searching for GTAV window...");
			var windowHandle = WindowHelper.FindWindow(targetWindow);
			if (windowHandle == IntPtr.Zero)
            {
				HelperClasses.Logger.Log("Failed to find GTAV window.");
			} 
			else
            {
				HelperClasses.Logger.Log("GTAV window found.");
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
				FPS = 10,
				IsTopmost = true,
				IsVisible = true
			};

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;

			this.Run();
			this.Visible = false;
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;
			if (e.RecreateResources)
			{
				foreach (var pair in _brushes) pair.Value.Dispose();
				foreach (var pair in _images) pair.Value.Dispose();
			}

			_brushes["textBack"] = gfx.CreateSolidBrush(0, 0, 0, (int)(.4*255));
			_brushes["textColor"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["background"] = gfx.CreateSolidBrush(0, 0, 0, 0);

			if (e.RecreateResources) return;

			//_fonts["arial"] = gfx.CreateFont("Arial", 12);
			_fonts["textFont"] = gfx.CreateFont("Consolas", 24, false, false, true);
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
            //var wb = new WindowBounds();
            //WindowHelper.GetWindowBounds(WindowHelper.FindWindow(targetWindow), out wb);
			var pos = coordFromPos(Position, GetWindowRectangle(WindowHelper.FindWindow(targetWindow)), _window.Width, _window.Height);
            _window.X = pos[0];
            _window.Y = pos[1];
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
			gfx.DrawTextWithBackground(_fonts["textFont"], _brushes["textColor"], _brushes["textBack"], textOffsetX, textOffsetY, NoteText);

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
			await graphicsReady();
			_brushes["textColor"].Color = Color.FromARGB(textColor.ToArgb());
			_brushes["textBack"].Color = Color.FromARGB(textBG.ToArgb());
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
			NoteText = charWrap(text, WrapCount);
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
			await graphicsReady();
			_fonts["textFont"] = _window.Graphics.CreateFont(fontFamily, fontSize, bold, italic, wordWrap);
        }

		/// <summary>
		/// Sets the render offset for text.
		/// </summary>
		/// <param name="x">X Offset</param>
		/// <param name="y">Y Offset</param>
		public void setTextPosition(int x, int y)
        {
			textOffsetX = x;
			textOffsetY = y;
        }

		/// <summary>
		/// Represents the point at which text is rendered.
		/// </summary>
		public Point TextRenderPoint
		{
			get
			{
				return new Point(textOffsetX, textOffsetY);
			}
			set
			{
				setTextPosition((int)value.X, (int)value.Y);
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
		public void SetInitialScropPosition()
		{
			scrollInitial = textOffsetY;
		}

		/// <summary>
		/// Sets the initial point for scrolling render.
		/// </summary>
		/// <param name="y">Initial scroll y position</param>
		public void SetInitialScropPosition(int y)
        {
			scrollInitial = y;
        }

		/// <summary>
		/// Scrolls the text by a given number of pixels
		/// </summary>
		/// <param name="delta">Number of pixels to scroll</param>
		public async void scroll(int delta)
        {
			await graphicsReady();
			var f = _fonts["textFont"];
			var wtext = charWrap(NoteText, WrapCount);
			var lines = wtext.Replace("\r\n", "\n").Split('\n');
			int lineH = 0;
			int lineCount = 0;
			foreach (var line in lines)
            {
				var SF = new System.Drawing.Font(f.FontFamilyName,
					f.FontSize,
					(f.Bold ? System.Drawing.FontStyle.Bold : 0) |
					(f.Italic ? System.Drawing.FontStyle.Italic : 0),
					System.Drawing.GraphicsUnit.Pixel);
				var lineS = System.Windows.Forms.TextRenderer.MeasureText(line, SF);
				double n = ((double)lineS.Width) / (_window.Graphics.Width - 2 * textOffsetX);
				var lh = (int)Math.Ceiling((double)lineS.Height);
				lineH = lh > lineH ? lh : lineH;
				if (!f.WordWeapping)
                {
					n = 1;
                }
				n = (n > 0) ? n : 1;
				lineCount += (int)Math.Ceiling(n);
			}
			int boundMin = (int)(scrollInitial - lineH * (lineCount-1));
			_window.Graphics.Height = (int)(lineH * lineCount * 2);
			textOffsetY += delta;
			if (textOffsetY > scrollInitial)
            {
				textOffsetY = scrollInitial;
            }
			if (textOffsetY < boundMin)
            {
				textOffsetY = boundMin;
            }
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
		private int[] coordFromPos(Positions p, RECT wb, int resx, int resy)
        {
			resx += PaddingSize + XMargin;
			resy += PaddingSize + YMargin;
			var coords = new int[2];
			switch (p)
            {
				case Positions.TopLeft:
					coords[1] = wb.Top + PaddingSize + YMargin;
					coords[0] = wb.Left + PaddingSize + XMargin;
					break;
				case Positions.TopRight:
					coords[1] = wb.Top + PaddingSize + YMargin;
					coords[0] = wb.Right - resx;
					break;
				case Positions.BottomLeft:
					coords[1] = wb.Bottom - resy;
					coords[0] = wb.Left + PaddingSize + XMargin;
					break;
				case Positions.BottomRight:
					coords[1] = wb.Bottom - resy;
					coords[0] = wb.Right - resx;
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

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_window.Dispose();

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
