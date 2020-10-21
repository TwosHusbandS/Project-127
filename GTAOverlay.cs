using GameOverlay.Drawing;
using GameOverlay.Windows;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = GameOverlay.Drawing.Image;

namespace Project_127
{
    class GTAOverlay : IDisposable
    {
		private readonly GraphicsWindow _window;

		//private const string targetWindow = "Project - 1.27";
		private const string targetWindow = "Grand Theft Auto V";
		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;
		private int wrapConstant;
		private float bgImageOpac = (float).7;
		private Positions renderPosition;
		//private Image bgImage = null;
		private string bgImagePath = "";
		private bool enableBackground = false;
        private string NoteText = "";
		private int renderMargin;

		/// <summary>
		/// Generates the game overlay
		/// </summary>
		/// <param name="position">The screen positioning (TopLeft, BottomRight, etc.)</param>
		/// <param name="width">The horizontal resolution</param>
		/// <param name="height">The vertical resolution</param>
		/// <param name="wrapNum">The number of characters before a line wrap</param>
		public GTAOverlay(Positions position = Positions.TopLeft, int width = 560, int height = 380, int wrapNum = 40, int margin = 0)
		{
			HelperClasses.Logger.Log("Game Overlay Initiated");
			wrapConstant = wrapNum;
			renderMargin = margin;
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();
            var wb = new WindowBounds();
			HelperClasses.Logger.Log("Searching for GTAV window...");
			var windowHandle = WindowHelper.FindWindow(targetWindow);
			if (windowHandle == IntPtr.Zero)
            {
				HelperClasses.Logger.Log("Failed to find GTAV window.");
			} else
            {
				HelperClasses.Logger.Log("GTAV window found.");
			}
			WindowHelper.GetWindowBounds(windowHandle, out wb);
            var gfx = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};
			var pos = coordFromPos(position, wb, width, height);
			_window = new GraphicsWindow(pos[0], pos[1], width, height, gfx)
			{
				FPS = 10,
				IsTopmost = true,
				IsVisible = true
			};

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;
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
			_fonts["consolas"] = gfx.CreateFont("Consolas", 24);
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
            var wb = new WindowBounds();
            WindowHelper.GetWindowBounds(WindowHelper.FindWindow(targetWindow), out wb);
			var pos= coordFromPos(Position, wb, e.Graphics.Width, e.Graphics.Height);
            _window.X = pos[0];
            _window.Y = pos[1];
			var gfx = e.Graphics;
			gfx.ClearScene(_brushes["background"]);
			if (enableBackground && _images.ContainsKey("bgImage"))
            {
				gfx.DrawImage(_images["bgImage"], 0, 0, bgImageOpac);

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

			gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["textColor"], _brushes["textBack"], 20, 20, NoteText);

		}

		/// <summary>
		/// Enables the overlay
		/// </summary>
		public void Run()
		{
			_window.Create();
			//_window.Join();
		}
		
		/// <summary>
		/// Sets the text color & text background color
		/// </summary>
		/// <param name="textColor">Color of the text</param>
		/// <param name="textBG">Color of the text background</param>
		public async void setTextColors(System.Drawing.Color textColor, System.Drawing.Color textBG)
        {
            while (!_window.IsInitialized || !_window.Graphics.IsInitialized)
            {
				await Task.Delay(250);
            }
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
			NoteText = charWrap(text, wrapConstant);
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
		/// Determines whether or not the background image is used.
		/// </summary>
		public bool UseBackground
        {
            get
            {
				return enableBackground;
            }
            set
            {
				enableBackground = value;
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
		private int[] coordFromPos(Positions p, WindowBounds wb, int resx, int resy)
        {
			resx += renderMargin;
			resy += renderMargin;
			var coords = new int[2];
			switch (p)
            {
				case Positions.TopLeft:
					coords[1] = wb.Top + renderMargin;
					coords[0] = wb.Left + renderMargin;
					break;
				case Positions.TopRight:
					coords[1] = wb.Top + renderMargin;
					coords[0] = wb.Right - resx;
					break;
				case Positions.BottomLeft:
					coords[1] = wb.Bottom - resy;
					coords[0] = wb.Left + renderMargin;
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



		//// <summary>
		/// Determines the positioning of the overlay.
		/// </summary>
		public Positions Position
        {
            get
            {
				return renderPosition;
            }
            set
            {
				renderPosition = value;
            }
        }

		//// <summary>
		/// Determines the positioning of line wrap.
		/// </summary>
		public int WrapCharacter
        {
            get
            {
				return wrapConstant;
            }
            set
            {
				wrapConstant = value;
            }
        }

		//// <summary>
		/// Determines the positioning of line wrap.
		/// </summary>
		public int MarginSize
		{
			get
			{
				return renderMargin;
			}
			set
			{
				renderMargin = value;
			}
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
