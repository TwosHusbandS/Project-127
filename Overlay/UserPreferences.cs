using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.Overlay
{
	public class UserPreferences

	{
		#region Member Variables

		private double _windowTop;
		private double _windowLeft;
		private double _windowHeight;
		private double _windowWidth;
		private System.Windows.WindowState _windowState;

		#endregion //Member Variables

		#region Public Properties

		public double WindowTop
		{
			get { return _windowTop; }
			set { _windowTop = value; }
		}

		public double WindowLeft
		{
			get { return _windowLeft; }
			set { _windowLeft = value; }
		}

		public double WindowHeight
		{
			get { return _windowHeight; }
			set { _windowHeight = value; }
		}

		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		public System.Windows.WindowState WindowState
		{
			get { return _windowState; }
			set { _windowState = value; }
		}

		#endregion //Public Properties

		#region Constructor

		public UserPreferences()
		{
			//Load the settings
			Load();

			//Size it to fit the current screen
			SizeToFit();

			//Move the window at least partially into view
			MoveIntoView();
		}

		#endregion //Constructor

		#region Functions

		/// <summary>
		/// If the saved window dimensions are larger than the current screen shrink the
		/// window to fit.
		/// </summary>
		public void SizeToFit()
		{
			if (_windowHeight > System.Windows.SystemParameters.VirtualScreenHeight)
			{
				_windowHeight = System.Windows.SystemParameters.VirtualScreenHeight;
			}

			if (_windowWidth > System.Windows.SystemParameters.VirtualScreenWidth)
			{
				_windowWidth = System.Windows.SystemParameters.VirtualScreenWidth;
			}
		}

		/// <summary>
		/// If the window is more than half off of the screen move it up and to the left 
		/// so half the height and half the width are visible.
		/// </summary>
		public void MoveIntoView()
		{
			if (_windowTop + _windowHeight / 2 > System.Windows.SystemParameters.VirtualScreenHeight)
			{
				_windowTop = System.Windows.SystemParameters.VirtualScreenHeight - _windowHeight;
			}

			if (_windowLeft + _windowWidth / 2 > System.Windows.SystemParameters.VirtualScreenWidth)
			{
				_windowLeft = System.Windows.SystemParameters.VirtualScreenWidth - _windowWidth;
			}

			if (_windowTop < 0)
			{
				_windowTop = 0;
			}

			if (_windowLeft < 0)
			{
				_windowLeft = 0;
			}
		}

		private void Load()
		{
			_windowTop = MyWindowTop;
			_windowLeft = MyWindowLeft;
			_windowHeight = MyWindowHeight;
			_windowWidth = MyWindowWidth;
			_windowState = MyWindowState;
		}


		public static double MyWindowTop = 50;
		public static double MyWindowLeft = 50;
		public static double MyWindowHeight = 40;
		public static double MyWindowWidth = 800;
		public static System.Windows.WindowState MyWindowState = System.Windows.WindowState.Normal;

		public void Save()
		{
			if (_windowState != System.Windows.WindowState.Minimized)
			{
				MyWindowTop = _windowTop;
				MyWindowLeft = _windowLeft;
				MyWindowHeight = _windowHeight;
				MyWindowWidth = _windowWidth;
				MyWindowState = _windowState;
			}
		}

		#endregion //Functions

	}
}
