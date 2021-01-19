using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Overlay
{
    class OverlayAnimationObject: overlayObject
    {
		/// <summary>
		/// Determines the position of the animation
		/// </summary>
        public GameOverlay.Drawing.Point position
        {
            get
            {
				return new GameOverlay.Drawing.Point(renderRect.X, renderRect.Y);
            }
            set
            {
				var r = renderRect;
				r.X = (int)value.X;
				r.Y = (int)value.Y;
				renderRect = r;
            }
        }
        public bool visible { get; set; }
        public string id { get; private set; }
        public bool linked { get; private set; }

		/// <summary>
		/// Determines whether overlay scaling is enabled
		/// </summary>
		public bool enableScaling { get; set; } 

		/// <summary>
		/// Determines the opacity of the rendered animation
		/// </summary>
		public float opacity { get; set; }
		
		private GTAOverlay host;
		private GameOverlay.Drawing.Graphics gfx;
		private List<GameOverlay.Drawing.Image> frames = new List<GameOverlay.Drawing.Image>();
		private int frameIndex = 0;
		private bool _fillOverlay = false;
		private System.Threading.SemaphoreSlim sem = new System.Threading.SemaphoreSlim(1, 1);
		private bool ready = false;
		//private double frameFactor = 1;
		private Rectangle _renderRect;
		private GameOverlay.Drawing.Image currentFrame
        {
            get
            {
				if (frames.Count > 0)
                {
					frameIndex++;
					frameIndex %= (int)(frames.Count / frameFactor);
					return frames[(int)(frameIndex * frameFactor)];
                }
				return null;
            }
        }

		/// <summary>
		/// Determines the dimensions/positon of the animation
		/// </summary>
		public Rectangle renderRect
		{
			get
			{
				if (_fillOverlay)
				{
					return new Rectangle(0, 0, host.width, host.height);
				}
				else if (enableScaling)
                {
					return _renderRect;
                }
				else
                {
					return new Rectangle();
                }
			}
			set
            {
				if (enableScaling && !fillOverlay)
                {
					_renderRect = value;
				}
            }
		}
		
		private GameOverlay.Drawing.Rectangle irenderRect { 
			get
            {
				return new GameOverlay.Drawing.Rectangle(renderRect.Left, renderRect.Top, renderRect.Right, renderRect.Bottom);
            } 
		}

		/// <summary>
		/// Determines whether or not the animation will fill the entire overlay
		/// </summary>
		public bool fillOverlay
        {
            get
            {
				return _fillOverlay;
            }
            set
            {
				_fillOverlay = value;
				if (_fillOverlay)
                {
					if (host != null)
                    {
						_renderRect = new Rectangle(0, 0, host.width, host.height);
						enableScaling = true;
                    }
                }
            }
        }

		/// <summary>
		/// Determines the FPS of the animation
		/// </summary>
		public double FPS { get; set; }

		private double frameFactor
		{
			get
			{
				return FPS / gfx.FPS;
			}
		}

		/// <summary>
		/// Animation object, used to render frame animations on the overlay
		/// </summary>
		/// <param name="id">Identifier of the object</param>
		public OverlayAnimationObject(string id)
        {
			this.id = id;
        }


		/// <summary>
		/// Loads frames from a GIF
		/// </summary>
		/// <param name="gifImg">GIF Image</param>
		public async Task loadGif(Image gifImg)
        {
			sem.Wait();
			ready = false;
			if (gifImg.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
				return;
            }
			var dimension = new System.Drawing.Imaging.FrameDimension(gifImg.FrameDimensionsList[0]);

			// Number of frames
			int frameCount = gifImg.GetFrameCount(dimension);
			for (var i = 0; i < frameCount; i++)
            {
				gifImg.SelectActiveFrame(dimension, i);
				using (var ms = new System.IO.MemoryStream())
                {
					var bmp = new Bitmap(gifImg);
					bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
					while (!host.Initialized && !disposing)
                    {
						await Task.Delay(100);
                    }
					if (disposing)
                    {
						sem.Release();
						return;
                    }
                    try
                    {
						frames.Add(new GameOverlay.Drawing.Image(gfx.GetRenderTarget(), ms.ToArray()));
					}
                    catch
                    {
						sem.Release();
						return;
                    }
				}
			}
			sem.Release();
			ready = true;
		}

		/// <summary>
		/// Deletes all animation frames
		/// </summary>
		public async void clearFrames()
        {
			ready = false;
			sem.Wait();
			frames.Clear();
			sem.Release();
        }

		/// <summary>
		/// Adds a animation frame
		/// </summary>
		/// <param name="img">Frame image</param>
		public async Task addFrame(Image img)
        {
			ready = false;
			sem.Wait();
			using (var ms = new System.IO.MemoryStream())
			{
				while (!host.Initialized && !disposing)
				{
					await Task.Delay(100);
				}
				img.Save(ms, img.RawFormat);
				try
				{
					addFrame(new GameOverlay.Drawing.Image(gfx, ms.ToArray()));
				}
				catch
				{
					sem.Release();
					return;
				}
			}
			sem.Release();
			ready = true;
		}

		/// <summary>
		/// Adds multiple animation frames
		/// </summary>
		/// <param name="imgs">Frame images</param>
		public async void addFrames(IList<Image> imgs)
		{
			foreach (var img in imgs)
            {
				await addFrame(img);
				if (disposing)
                {
					return;
                }
            }
		}

		/// <summary>
		/// Adds a animation frame
		/// </summary>
		/// <param name="img">Frame image</param>
		public void addFrame(GameOverlay.Drawing.Image img)
        {
			frames.Add(img);
			ready = true;
        }

		/// <summary>
		/// Adds multiple animation frames
		/// </summary>
		/// <param name="imgs">Frame images</param>
		public async void addFrames(IList<GameOverlay.Drawing.Image> imgs)
		{
			ready = false;
			sem.Wait();
			frames.AddRange(imgs);
			sem.Release();
			ready = true;
		}

		/// <summary>
		/// Gets an image from a resource identifier
		/// </summary>
		/// <param name="u">Resource Identifier</param>
		/// <returns>Image genned from URI</returns>
		public async Task<GameOverlay.Drawing.Image> imageFromURI(Uri u)
        {
			var res = System.Windows.Application.GetResourceStream(u);
			using (var ms = new System.IO.MemoryStream())
            {
				res.Stream.CopyTo(ms);
				sem.Wait();
				while (!host.Initialized)
				{
					await Task.Delay(100);
					if (disposing)
                    {
						sem.Release();
						return null;
                    }
				}
				var img = new GameOverlay.Drawing.Image(gfx, ms.ToArray());
				sem.Release();
				return img;
			}
        }

		public void render(GameOverlay.Drawing.Graphics gfx = null)
		{
			//gfx.DrawTextWithBackground(currentFont, tb,
			if (!ready)
            {
				return;
            }
			if (gfx == null ||frames.Count == 0)
			{
				return;
			}
			if (visible)
			{
				if (!enableScaling)
                {
					gfx.DrawImage(currentFrame, position, opacity);
				}
                else
                {
					gfx.DrawImage(currentFrame, irenderRect, opacity, true);
                }
			}
		}

		public void link(GTAOverlay host, GameOverlay.Drawing.Graphics gfx)
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

		#region IDisposable Support
		private bool disposedValue;
		private bool disposing = false;

		protected virtual void Dispose(bool disposing)
		{
			this.disposing = true;
			sem.Wait();
			if (!disposedValue)
			{
				foreach (var frame in frames)
                {
					frame.Dispose();
                }
				frames.Clear();
				
				if (host != null)
				{
					host.detach(id);
				}
				disposedValue = true;
			}
			sem.Release();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}
