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
        public ROSIntegration()
        {
            InitializeComponent();
        }

        private const string jsf = @"
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

window.invokeNative('signin', s);
}

RGSC_JS_FINISH_SIGN_IN(JSON.stringify(data));
}

function RGSC_RAISE_UI_EVENT(a)
{
const d = JSON.parse(a);

if (d.EventId === 1) {
window.invokeNative('ui', d.Data.Action);
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

var css = '.rememberContainer, p.Header__signUp { display: none; } .SignInForm__descriptionText .Alert__text { display: none; } .Alert__content:after { content: \'A Rockstar Games Social Club account owning Grand Theft Auto V is required to use Project 1.27.\'; max-width: 600px; display: inline-block; }',
    head = document.head || document.getElementsByTagName('head')[0],
    style = document.createElement('style');

head.appendChild(style);

style.type = 'text/css';
style.appendChild(document.createTextNode(css));
";
        private void LoadingStateChange(object sender, LoadingStateChangedEventArgs args)
        {
            if (!args.IsLoading)
            {
                IFrame frame = browser.GetMainFrame();
                frame.ExecuteJavaScriptAsync(jsf, "https://rgl.rockstargames.com/temp.js", 0);
                System.Windows.Forms.MessageBox.Show(frame.Url);
            }
        }
    }
}

