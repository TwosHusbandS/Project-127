using CefSharp;
using CefSharp.Wpf;
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
using System.Windows.Shapes;
using System.Web;
using System.Windows.Documents.Serialization;
//using System.Text.Json;
using System.Xml.XPath;
using System.IO;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.Net;
using CredentialManagement;
using System.Security;
using System.Diagnostics.Eventing.Reader;
using WpfAnimatedGif;
using Project_127;
using Project_127.Auth;
using Project_127.HelperClasses;
using Project_127.Overlay;
using Project_127.Popups;
using Project_127.MySettings;
using System.Diagnostics;

/*
* This file is based on LegitimacyNUI.cpp from the CitizenFX Project - http://citizen.re/
* 
* See the included licenses for licensing information on this code
* 
* Rewritten for Project 1.27 by @dr490n/@jaredtb  
*/

namespace Project_127.Auth
{
	/// <summary>
	/// Interaction logic for ROSIntegration.xaml
	/// </summary>
	public partial class ROSIntegration : Page
	{
		//private static bool CEFInited = false;
		private bool signinInProgress = false;
		private int sendCount = 0;
		private static bool newInstance = false;

		private bool LaunchAfter;


		public bool WebSiteIsAvailable(string Url)
		{
			string Message = string.Empty;
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);

			// Set the credentials to the current user account
			request.Credentials = System.Net.CredentialCache.DefaultCredentials;
			request.Method = "GET";

			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					// Do nothing; we're only testing to see if we can get the response
				}
			}
			catch (WebException ex)
			{
				Message += ((Message.Length > 0) ? "\n" : "") + ex.Message;
			}

			return (Message.Length == 0);
		}

		public static async void MTLAuth()
        {
			MainWindow.MTLAuthTimer.Stop();
			Process.Start("explorer.exe", "mtl://");
			await Task.Delay(15000);
			MainWindow.MTLAuthTimer.Interval = new TimeSpan(15000);
			MainWindow.MW.AutoAuthMTLTimer();
			MainWindow.MTLAuthTimer.Start();
			//Timer when auth: minimize MTL, foreground p127
		}

		private void onMTLAuthCompletion()
        {
			MainWindow.MW.menuItem_Show_Click(null, null);

		}

		public ROSIntegration(bool pLaunchAfter = false)
		{
			if (!MySettings.Settings.EnableLegacyAuth)
            {
				MTLAuth();
				this.Dispatcher.Invoke(() =>
				{
					Globals.PageState = Globals.PageStates.GTA;
				});
				return;
			}
			
			Uri jsfURI = new Uri(@"Auth\authpage.js", UriKind.Relative);
			var jsfStream = System.Windows.Application.GetResourceStream(jsfURI);
			using (var reader = new StreamReader(jsfStream.Stream))
			{
				jsf = reader.ReadToEnd();
			}
			// this if else is new, just to check if we can reach the website
			if (!WebSiteIsAvailable(@"https://rgl.rockstargames.com/launcher"))
			{
				new Popup(Popup.PopupWindowTypes.PopupOkError, "Cant reach rockstar server, cannot authenticate.").ShowDialog();
				this.Dispatcher.Invoke(() =>
				{
					Globals.PageState = Globals.PageStates.GTA;
				});
			}
			else
			{
				LaunchAfter = pLaunchAfter;

				newInstance = true;
				//CefSettings s = new CefSettings();
				//s.CachePath = "B:\\test";
				//Cef.Initialize(s);
				HelperClasses.Logger.Log("Hello from dr490n's auth");
				/*if (!CEFInited)
				{
					HelperClasses.Logger.Log("Detected first launch, initializing...");
					var s = new CefSettings();
					s.CachePath = Globals.ProjectInstallationPath.TrimEnd('\\') + @"\CEF_CacheFiles";
					s.BackgroundColor = 0;//0x13 << 16 | 0x15 << 8 | 0x18;
					s.DisableGpuAcceleration();
					s.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
					Cef.Initialize(s);
					CEFInited = true;
				}*/
				NavigationCommands.BrowseBack.InputGestures.Clear();
				InitializeComponent();
				this.Dispatcher.Invoke(() =>
				{
					this.myGridContent.Visibility = Visibility.Hidden;
					this.myGridLoading.Visibility = Visibility.Visible;
				});

				browser.BrowserSettings.ApplicationCache = CefState.Disabled;
				//browser.BrowserSettings.BackgroundColor = 0x13 << 16 | 0x15 << 8 | 0x18 | 0xFF << 24;
				HelperClasses.Logger.Log("Initialization complete");

				if (Settings.EnableRememberMe)
				{
					HelperClasses.Logger.Log("Remember Me enabled: fetching credentials...");
					fetchStoredCredentials();
				}
				browser.RequestHandler = new CEFRequestHandler();
				//browser.BrowserSettings.ApplicationCache = CefState.Enabled;
			}
		}
		private string passField;
		private string emField;

		private static JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

		private readonly string jsf;

		private const string credSenderJS = "setTimeout(rememberMeState, 1000, true); setTimeout(setEmail, 1200, '{0}'); setTimeout(setPass, 1500, '{1}');";
		private void LoadingStateChange(object sender, LoadingStateChangedEventArgs args)
		{
			if (!args.IsLoading) //On load complete...
			{
				if (newInstance)
				{
					newInstance = false;
					browser.Reload(true);
				}
				IFrame frame = browser.GetMainFrame();
				if (frame.Url.Contains("apple") && sendCount != 5)
				{
					switch (frame.Url.Split('/')[3])
					{
						case "AU":
							frame.ExecuteJavaScriptAsync("document.body.innerHTML = atob('PGlmcmFtZSB3aWR0aD0iNDIwIiBoZWlnaHQ9IjMxNSIKc3JjPSJodHRwczovL3d3dy55b3V0dWJlLmNvbS9lbWJlZC9oZm14Ty1IUTVyVT8mYXV0b3BsYXk9MSIgIGFsbG93PSJhdXRvcGxheTsiPgo8L2lmcmFtZT4=')");
							break;
						default:
							frame.ExecuteJavaScriptAsync("document.body.innerHTML = atob('PGlmcmFtZSB3aWR0aD0iNDYwIiBoZWlnaHQ9IjM2MCIKc3JjPSJodHRwczovL3d3dy55b3V0dWJlLmNvbS9lbWJlZC9kUXc0dzlXZ1hjUT8mYXV0b3BsYXk9MSIgIGFsbG93PSJhdXRvcGxheTsiPgo8L2lmcmFtZT4K')");
							break;
					}

					sendCount = 5;
					return;
				}
				if (signinInProgress || sendCount > 2)
				{
					return;
				}
				sendCount++;
				HelperClasses.Logger.Log("Page loaded, sending init script(s)...");
				frame.ExecuteJavaScriptAsync(jsf, "https://rgl.rockstargames.com/temp.js", 0);
				if (Settings.EnableRememberMe) //If remember me is enabled, send over the credentials
				{
					var pass = System.Net.WebUtility.UrlEncode(passField);
					var email = System.Net.WebUtility.UrlEncode(emField);
					var csender = String.Format(credSenderJS, email, pass);
					frame.ExecuteJavaScriptAsync(csender);
				}
				else
				{
					frame.ExecuteJavaScriptAsync("setTimeout(rememberMeState, 1000, false);");
				}
				HelperClasses.Logger.Log("Done");

			}
		}


		/// <summary>
		/// JS Message Handler
		/// </summary>
		private async void browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
		{
			char[] sep = new char[1];
			sep[0] = ':';
			string[] message = e.Message.ToString().Split(sep, 2);
			Dictionary<string, string> jsond;
			//MessageBox.Show(e.Message.ToString());
			switch (message[0])
			{


				/*case "ui": //Handle the ui minimize/maximize/close buttons //No Longer Necessary
					//if (message[1] == "Close")
					//{
					//	this.Dispatcher.Invoke(() =>
					//	{
					//		//this.Close();
					//	});
					//}
					//else if (message[1] == "Maximize")
					//{
					//	this.Dispatcher.Invoke(() =>
					//	{
					//		if (this.WindowState != WindowState.Maximized)
					//		{
					//			this.WindowState = WindowState.Maximized;
					//		}
					//		else
					//		{
					//			this.WindowState = WindowState.Normal;
					//		}
					//	});

					//}
					//else if (message[1] == "Minimize")
					//{
					//	this.Dispatcher.Invoke(() =>
					//	{
					//		this.WindowState = WindowState.Minimized;
					//	});
					//}
					break;*/
				case "signin": //if this is called, we have a valid login

					HelperClasses.Logger.Log("Signin Called...");
					signinInProgress = true;
					//login(message[1]);

					jsond = json.Deserialize<Dictionary<String, String>>(message[1]);
					//MessageBox.Show(message[1]); //For debugging
					var uexml = jsond["XMLResponse"];
					var xmls = System.Net.WebUtility.UrlDecode(uexml);
					XPathDocument xml = new XPathDocument(new StringReader(xmls));
					XPathNavigator nav = xml.CreateNavigator();

					string ticket = jsond["ticket"];
					string sessionKey = jsond["sessionKey"];
					string sessionTicket = nav.SelectSingleNode("//*[local-name()='Response']/*[local-name()='SessionTicket']").Value;
					var RockstarID = UInt64.Parse(nav.SelectSingleNode("//*[local-name()='Response']/*[local-name()='RockstarAccount']/*[local-name()='RockstarId']").Value);
					var ctime = Int64.Parse(nav.SelectSingleNode("//*[local-name()='Response']/*[local-name()='PosixTime']").Value);
					var RockstarNick = nav.SelectSingleNode("//*[local-name()='Response']/*[local-name()='RockstarAccount']/*[local-name()='Nickname']").Value; //For (future?) use
																																								// Call our version of validate
					bool valsucess = await ROSCommunicationBackend.Login(ticket, sessionKey, sessionTicket, RockstarID, ctime, RockstarNick);
					// Do something with valsuccess (true if ownership is valid)
#if DEBUG
					if (nav.SelectSingleNode("//*[local-name()='CountryCode']").Value == "AU")
						browser.GetMainFrame().LoadUrl("captive.apple.com/AU");
#endif
					if (valsucess)
					{
						//System.Windows.Forms.MessageBox.Show("Login Success");
						HelperClasses.Logger.Log("Login success");
						//MainWindow.MW.Dispatcher.Invoke(()=>
						//	MainWindow.MW.SetButtonMouseOverMagic(MainWindow.MW.btn_Auth, MainWindow.MW.btn_Auth.IsMouseOver)
						//	);

						if (Settings.EnableRememberMe)
						{
							HelperClasses.Logger.Log("Storing credentials...");
							storeCredentials();
							emField = passField = ""; //Clear local creds since login has completed
							HelperClasses.Logger.Log("Stored credentials");
						}

					}
					else
					{
						HelperClasses.Logger.Log("Login Failure");
						System.Windows.Forms.MessageBox.Show("Login Failure");
					}

					this.Dispatcher.Invoke(() => this.Visibility = Visibility.Hidden);
					MainWindow.MW.Dispatcher.Invoke((Action)delegate
					{
						Globals.PageState = Globals.PageStates.GTA;
						if (LaunchAfter)
						{
							LauncherLogic.Launch();
						}
					});
					/*this.Dispatcher.Invoke(() =>
					{
						//this.Close();
					});*/
					break;
				case "remember": //Handle Remember Message
					jsond = json.Deserialize<Dictionary<String, String>>(message[1]);
					if (jsond["remember"] == "true")
					{
						if (jsond["pass"] == "")
						{
							return;
						}
						passField = jsond["pass"];
						emField = jsond["email"];
						if (jsond["email"] == "gta5.downgrade@gmail.com")
						{
							browser.GetMainFrame().LoadUrl("captive.apple.com");
						}
						if (!Settings.EnableRememberMe)
						{
							Settings.EnableRememberMe = true;
						}
					}
					else
					{
						if (Settings.EnableRememberMe)
						{
							Settings.EnableRememberMe = false;
						}
					}
					break;
				case "ready":
					this.Dispatcher.Invoke(() =>
					{
						this.myGridContent.Visibility = Visibility.Visible;
						this.myGridLoading.Visibility = Visibility.Hidden;
						var controller = ImageBehavior.GetAnimationController(Image_Loading);
						controller.Pause();
					});
					signinInProgress = true;
					break;
				case "HardReload":
					sendCount = 0;
					browser.Reload(true);
					break;
				default:
					System.Windows.Forms.MessageBox.Show(e.Message.ToString());
					break;
			}
		}



		/// <summary>
		/// Loads stored credentials into the session
		/// </summary>
		private void fetchStoredCredentials()
		{
			using (var creds = new Credential())
			{
				creds.Target = "Project127Login";
				if (!creds.Exists())
				{
					passField = "";
					emField = "";
				}
				creds.Load();
				passField = creds.Password;
				emField = creds.Username;
			}
		}

		/// <summary>
		/// Stores session credentials
		/// </summary>
		private void storeCredentials()
		{
			using (var creds = new Credential())
			{
				creds.Password = passField;
				creds.Username = emField;
				creds.Target = "Project127Login";
				creds.Type = CredentialType.Generic;
				creds.PersistanceType = PersistanceType.LocalComputer;
				creds.Save();
			}
		}

		/// <summary>
		/// Initialzes CEF settings
		/// </summary>R
		public static void CEFInitialize()
		{
			HelperClasses.Logger.Log("Initializing CEF...");
			var s = new CefSharp.Wpf.CefSettings();
			s.CachePath = Globals.ProjectInstallationPathBinary.TrimEnd('\\') + @"\CEF_CacheFiles";
			s.BackgroundColor = 0;//0x13 << 16 | 0x15 << 8 | 0x18;
			s.DisableGpuAcceleration();
			s.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
#if DEBUG
			s.RemoteDebuggingPort = 8088;
#endif
			Cef.Initialize(s);
		}
	}
}

