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
using System.Xml.XPath;
using System.IO;
using System.Text.Json;
using System.Web.Script.Serialization;
using System.Drawing;
using CefSharp.Wpf.Example.Handlers;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.Net;
using CredentialManagement;
using System.Security;
/*
* This file is based on LegitimacyNUI.cpp from the CitizenFX Project - http://citizen.re/
* 
* See the included licenses for licensing information on this code
* 
* Rewritten for Project 1.27 by @dr490n/@jaredtb  
*/

namespace Project_127
{
    /// <summary>
    /// Interaction logic for ROSIntegration.xaml
    /// </summary>
    public partial class ROSIntegration : Window
    {
        private Region region;

        private void OnBrowserMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(browser);

            if (region.IsVisible((float)point.X, (float)point.Y))
            {
                var window = Window.GetWindow(this);
                window.DragMove();

                e.Handled = true;
            }
        }

        private void OnDragHandlerRegionsChanged(Region region)
        {
            if (region != null)
            {
                //Only wire up event handler once
                if (this.region == null)
                {
                    browser.PreviewMouseLeftButtonDown += OnBrowserMouseLeftButtonDown;
                }

                this.region = region;
            }
        }

        public ROSIntegration()
        {
            //CefSettings s = new CefSettings();
            //s.CachePath = "B:\\test";
            //Cef.Initialize(s);
            InitializeComponent();
            var dragHandler = new DragHandler();
            dragHandler.RegionsChanged += OnDragHandlerRegionsChanged;
            browser.DragHandler = dragHandler;
            browser.BrowserSettings.BackgroundColor = 0x13 << 16 | 0x15 << 8 | 0x18;
            if (Settings.EnableRememberMe)
            {
                //fetchStoredCredentials
            }
            //browser.BrowserSettings.ApplicationCache = CefState.Enabled;
        }
        private string passField;
        private string emField;

        private static JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        private const string jsf = @"
function invokeNative(name, json){
    CefSharp.PostMessage(name+':'+json);
}

function RGSC_GET_PROFILE_LIST()
{
return JSON.stringify({
Profiles: [],//profileList.profiles,
NumProfiles: 0//profileList.profiles.length
});
}

function RGSC_UI_STATE_RESPONSE(arg)
{
var data = JSON.parse(arg);

if (!data.Visible)
{
$(""#scuiPanelInstruction"").hide();
$('#headerWrapper').hide();
}
}

function RGSC_GET_TITLE_ID()
{
return JSON.stringify({
RosTitleName: 'gta5',
RosEnvironment: 'prod',
RosTitleVersion: 11,
RosPlatform: 'pcros',
Platform: 'viveport',
IsLauncher: true,
Language: 'en-US'
});
}

function RGSC_GET_VERSION_INFO()
{
return JSON.stringify({
Version: 'ROS.. in browser!'
});
}

var rosCredentials = {};

function RGSC_SIGN_IN(s)
{
var data = JSON.parse(s);

if (data.XMLResponse)
{
// TODO: store other credentials in native code
rosCredentials.Ticket = data.ticket;

var profileData = {
Local: false,
RockstarId: data.RockstarId,
LastSignInTime: new Date().getTime(),
AvatarUrl: data.AvatarUrl,
Nickname: data.Nickname,
SaveEmail: data.SaveEmail || data.SavePassword || data.AutoSignIn,
SavePassword: data.SavePassword || data.AutoSignIn,
AutoSignIn: data.AutoSignIn,
Password: (data.SavePassword || data.AutoSignIn) ? data.Password : '',
Email: (data.SaveEmail || data.SavePassword || data.AutoSignIn) ? data.Email : ''
};

invokeNative('signin', s);
}

RGSC_JS_FINISH_SIGN_IN(JSON.stringify(data));
}

function RGSC_RAISE_UI_EVENT(a)
{
const d = JSON.parse(a);

if (d.EventId === 1) {
invokeNative('ui', d.Data.Action);
}
}

function RGSC_MODIFY_PROFILE(a)
{
RGSC_JS_FINISH_MODIFY_PROFILE(a);
}

function RGSC_SIGN_OUT()
{
}

function RGSC_DELETE_PROFILE(a)
{
}

function RGSC_REQUEST_PLAYER_LIST_COUNTS(st)
{
}

function RGSC_REQUEST_PLAYER_LIST(st)
{
}

// yes, this is mistyped as RSGC
function RSGC_LAUNCH_EXTERNAL_WEB_BROWSER(a)
{
var d = JSON.parse(a);
}

function RGSC_GET_ROS_CREDENTIALS()
{
return JSON.stringify(rosCredentials);
}

function RGSC_REQUEST_UI_STATE(a)
{
var data = JSON.parse(a);

RGSC_UI_STATE_RESPONSE(a);

data.Online = true;

RGSC_JS_UI_STATE_RESPONSE(JSON.stringify(data));
}

function RGSC_READY_TO_ACCEPT_COMMANDS()
{
return true;
}

RGSC_JS_READY_TO_ACCEPT_COMMANDS();
RGSC_JS_REQUEST_UI_STATE(JSON.stringify({ Visible: true, Online: true, State: ""SIGNIN"" }));

if (!localStorage.getItem('loadedOnce')) {
localStorage.setItem('loadedOnce', true);
setTimeout(() => {
location.reload();
}, 500);
}

var css = '.autoSignIn, .SaveCredentials__tooltip, p.Header__signUp { display: none; } .SignInForm__descriptionText .Alert__text { display: none; } .Alert__content:after { content: \'A Rockstar Games Social Club account owning Grand Theft Auto V is required to use Project 1.27.\'; max-width: 600px; display: inline-block; }',
    head = document.head || document.getElementsByTagName('head')[0],
    style = document.createElement('style');

head.appendChild(style);

style.type = 'text/css';
style.appendChild(document.createTextNode(css));

function setEmail(email) {
    email = decodeURIComponent(email);
    var e = document.querySelector('[data-ui-name=""emailInput""]');
    if (!e){
        setTimeout(setEmail, 500, email);
        return;
    }
    var ef = Object.getOwnPropertyNames(e);
    var rgx = /__reactEventHandlers\$.+$/g;
    var evfs = []
    for (const field of ef) {
        if (field.match(rgx)) {
            evfs.push(field);
        }
    }
    e[evfs[0]].onChange({
        target: {
            value: email
        }
    });
}

function setPass(pass) {
    pass = decodeURIComponent(pass);
    var e = document.querySelector('[data-ui-name=""passwordInput""]');
    if (!e){
        setTimeout(setPass, 500, pass);
        return;
    }
    var ef = Object.getOwnPropertyNames(e);
    var rgx = /__reactEventHandlers\$.+$/g;
    var evfs = []
    for (const field of ef) {
        if (field.match(rgx)) {
            evfs.push(field);
        }
    }
    e[evfs[0]].onChange({
        target: {
            value: pass
        }
    });
}

function rememberMeState(active) {
    if (document.querySelector('#rememberMeProfile').checked != active) {
        document.querySelector('#rememberMeProfile').click();
    }
}

function rememberMeHandler() {
    var creds;
    if (document.querySelector('#rememberMeProfile').checked) {
        creds = {
        email: document.querySelector('[data-ui-name=""emailInput""]').value,
        pass: document.querySelector('[data-ui-name=""passwordInput""]').value,
        remember: 'true'
        }
    } else {
        creds = {
            remember: 'false'
        }
    }
    invokeNative('remember', JSON.stringify(creds));
}

function initDragClick(){
    var e = document.querySelector('#drag');
    if (!e){
        setTimeout(initDragClick, 500);
        return;
    }
    e.setAttribute('style','-webkit-app-region: drag;');
    for (const elem of document.querySelectorAll('.AppControls__windowControls')) {
        elem.setAttribute('style','-webkit-app-region: no-drag;')
    }
}

initDragClick();

document.addEventListener('input', rememberMeHandler);
";
        private const string credSenderJS = "setTimeout(rememberMeState, 500, true); setTimeout(setEmail, 500, {0}); setTimeout(setPass, 750, {1})";
        private void LoadingStateChange(object sender, LoadingStateChangedEventArgs args)
        {
            if (!args.IsLoading)
            {
                IFrame frame = browser.GetMainFrame();

                frame.ExecuteJavaScriptAsync(jsf, "https://rgl.rockstargames.com/temp.js", 0);
                if (Settings.EnableRememberMe)
                {
                    var pass = System.Net.WebUtility.UrlEncode(passField);
                    var email = System.Net.WebUtility.UrlEncode(emField);
                    frame.ExecuteJavaScriptAsync(string.Format(credSenderJS, email, pass), "https://rgl.rockstargames.com/temp2.js", 0);
                }
                else
                {
                    frame.ExecuteJavaScriptAsync("setTimeout(rememberMeState, 500, false);", "https://rgl.rockstargames.com/temp2.js", 0);
                }

                
            }
        }

        private async void browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            char[] sep = new char[1];
            sep[0] = ':';
            string[] message = e.Message.ToString().Split(sep, 2);
            //MessageBox.Show(e.Message.ToString());
            if (message[0] == "ui")
            {
                if (message[1] == "Close")
                {
                    this.Dispatcher.Invoke(() => 
                    {
                        this.Close(); 
                    });
                } 
                else if (message[1] == "Maximize")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (this.WindowState != WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Maximized;
                        }
                        else
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    });
 
                }
                else if (message[1] == "Minimize")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.WindowState = WindowState.Minimized;
                    });
                }
            }
            else if (message[0] == "signin") //if this is called, we have a valid login
            {
                //login(message[1]);
                var jsond = json.Deserialize<Dictionary<String, String>>(message[1]);
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
                // Do somethin with valsuccess (true if ownership is valid)

                if (valsucess)
                {
                    //MessageBox.Show("Login Success");
                } 
                else
                {
                    System.Windows.Forms.MessageBox.Show("Login Failure");
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            }
            else if (message[0] == "remember")
            {
                var jsond = json.Deserialize<Dictionary<String, String>>(message[1]);
                if (jsond["remember"] == "true")
                {
                    passField = jsond["pass"];
                    emField = jsond["email"];
                    Settings.EnableRememberMe = true;
                }
                else
                {
                    Settings.EnableRememberMe = false;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(e.Message.ToString());
            }
        }
        
        private void fetchStoredCredentials()
        {
            using (var creds = new Credential())
            {
                creds.Target = "Project127Login";
                creds.Load();
                passField = creds.Password;
                emField = creds.Username;
            }
        }

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

    }
}

