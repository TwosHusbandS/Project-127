function invokeNative(name, json) {
    CefSharp.PostMessage(name + ':' + json);
}

function RGSC_GET_PROFILE_LIST() {
    return JSON.stringify({
        Profiles: [], //profileList.profiles,
        NumProfiles: 0 //profileList.profiles.length
    });
}

function RGSC_UI_STATE_RESPONSE(arg) {
    var data = JSON.parse(arg);

    if (!data.Visible) {
        $("#scuiPanelInstruction").hide();
        $('#headerWrapper').hide();
        0
    }
}

function RGSC_GET_TITLE_ID() {
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

/*function RGSC_GET_VERSION_INFO() {
    return JSON.stringify({
        Version: 'ROS.. in browser!'
    });
}*/
function RGSC_GET_VERSION_INFO()
{
	return JSON.stringify({
		Version: '2.0.3.7',
		TitleVersion: ''
	});
}

function RGSC_GET_COMMAND_LINE_ARGUMENTS()
{
	return JSON.stringify({
		Arguments: []
	});
}

var rosCredentials = {};

function RGSC_SIGN_IN(s) {
    var data = JSON.parse(s);

    if (data.XMLResponse) {
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

function RGSC_RAISE_UI_EVENT(a) {
    const d = JSON.parse(a);

    if (d.EventId === 1) {
        invokeNative('ui', d.Data.Action);
    }
}

function RGSC_MODIFY_PROFILE(a) {
    RGSC_JS_FINISH_MODIFY_PROFILE(a);
}

function RGSC_SIGN_OUT() {}

function RGSC_DELETE_PROFILE(a) {}

function RGSC_REQUEST_PLAYER_LIST_COUNTS(st) {}

function RGSC_REQUEST_PLAYER_LIST(st) {}

// yes, this is mistyped as RSGC
function RSGC_LAUNCH_EXTERNAL_WEB_BROWSER(a) {
    var d = JSON.parse(a);
}

function RGSC_GET_ROS_CREDENTIALS() {
    return JSON.stringify(rosCredentials);
}

function RGSC_REQUEST_UI_STATE(a) {
    var data = JSON.parse(a);

    RGSC_UI_STATE_RESPONSE(a);

    data.Online = true;

    RGSC_JS_UI_STATE_RESPONSE(JSON.stringify(data));
}

function RGSC_READY_TO_ACCEPT_COMMANDS()
{
	RGSC_JS_REQUEST_UI_STATE(JSON.stringify({ Visible: true, Online: true, State: "SIGNIN" }));
	return true;
}

RGSC_JS_READY_TO_ACCEPT_COMMANDS();
/*RGSC_JS_REQUEST_UI_STATE(JSON.stringify({
    Visible: true,
    Online: true,
    State: "SIGNIN"
}));*/

if (!localStorage.getItem('loadedOnce')) {
    localStorage.setItem('loadedOnce', true);
    setTimeout(() => {
        location.reload();
    }, 500);
}

var css = '.SignInForm__forgotPassword, .autoSignIn, .SaveCredentials__tooltip, p.Header__signUp { display: none; } .SignInForm__descriptionText .Alert__text { display: none; } .Alert__info>.Alert__content:after { content: \'A Social Club account owning Grand Theft Auto V is required to use Project 1.27.\'; max-width: 600px; display: inline-block; color: black; } body {background: rgba(0, 0, 0, 0) !important;} #root {background: rgba(0, 0, 0, 0) !important;} .Alert__danger {background-color: rgba(189,08,08,0.7)} .Alert__info {background-color: rgba(193,206,209,1); margin-bottom: 50px;} .TextInput-mtl__field {background: rgba(0,0,0,.7)} .Button-mtl__primary {background: #c1ced1;} .SignInForm__wrapper {margin-top: 20px;}',
    head = document.head || document.getElementsByTagName('head')[0],
    style = document.createElement('style');

head.appendChild(style);

style.type = 'text/css';
style.appendChild(document.createTextNode(css));

function setEmail(email) {
    email = decodeURIComponent(email);
    var e = document.querySelector('[data-ui-name="emailInput"]');
    if (!e) {
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
    var e = document.querySelector('[data-ui-name="passwordInput"]');
    if (!e) {
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
    try {
        if (document.querySelector('#rememberMeProfile').checked != active) {
            document.querySelector('#rememberMeProfile').click();
        }
    } catch {
        location.reload();
    }
    
}

function rememberMeHandler() {
    var creds;
    if (document.querySelector('#rememberMeProfile').checked) {
        creds = {
            email: document.querySelector('[data-ui-name="emailInput"]').value,
            pass: document.querySelector('[data-ui-name="passwordInput"]').value,
            remember: 'true'
        }
    } else {
        creds = {
            remember: 'false'
        }
    }
    invokeNative('remember', JSON.stringify(creds));
}

document.addEventListener('input', rememberMeHandler);

var mhRTCounter = 0;
var mhSRTCounter = 0;




function modHtml() {
    if (!document.querySelector('.AppControls__appControls')) {
        setTimeout(modHtml, 100);
        mhRTCounter++;
        return;

    } else if (!document.querySelector('.SignInForm__signInButton').innerText) {
        if ((mhSRTCounter - mhRTCounter) > 10) {
            invokeNative('HardReload', '');
        }
        mhSRTCounter++;
        setTimeout(modHtml, 100);
        return;
    }
    document.querySelector('div[style=\'display: initial;\']').outerHTML = '';
    document.querySelector('.AppControls__appControls').outerHTML = '';
    document.querySelector('.Footer__container').outerHTML = '';
    document.querySelector('.HeaderLayout__header').outerHTML = '';
    setTimeout(invokeNative, 200, 'ready', '');
}

setTimeout(modHtml, 1000);