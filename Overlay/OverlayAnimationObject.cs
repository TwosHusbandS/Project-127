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
		public bool enableScaling { get; set; } 
		public float opacity { get; set; }
		
		private Point point2 { get; set; }
		private GTAOverlay host;
		private GameOverlay.Drawing.Graphics gfx;
		private List<GameOverlay.Drawing.Image> frames = new List<GameOverlay.Drawing.Image>();
		private int frameIndex = 0;
		private bool _fillOverlay = false;
		private System.Threading.Mutex mut = new System.Threading.Mutex();
		private bool ready = false;
		private double frameFactor = 1;
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

		public Rectangle renderRect
		{
			get
			{
				if (enableScaling)
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

		public double FPS
        {
            get
            {
				return gfx.FPS * frameFactor;
            }
            set
            {
				frameFactor = value / gfx.FPS;
            }
        }

		public OverlayAnimationObject(string id)
        {
			this.id = id;
        }

		public async Task loadGif(Image gifImg)
        {
			mut.WaitOne();
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
					while (!gfx.IsInitialized && !disposing)
                    {
						await Task.Delay(100);
                    }
					if (disposing)
                    {
						mut.ReleaseMutex();
						return;
                    }
                    try
                    {
						frames.Add(new GameOverlay.Drawing.Image(gfx.GetRenderTarget(), ms.ToArray()));
					}
                    catch
                    {
						return;
                    }
				}
			}
			mut.ReleaseMutex();
			ready = true;
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
			mut.WaitOne();
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
			mut.ReleaseMutex();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}
