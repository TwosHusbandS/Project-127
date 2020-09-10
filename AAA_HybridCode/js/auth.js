/*
 * This file is based on SCUIStub from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in javascript by @dr490n/@jaredtb  
 */
const ROS_PLATFORM_KEY_LAUNCHER = "C6fU6TQTPgUVmy3KIB5g8ElA7DrenVpGogSZjsh+lqJaQHqv4Azoctd/v4trfX6cBjbEitUKBG/hRINF4AhQqcg="

btoa = (b) => b.toString('base64');
atob = (a) => Buffer.from(a, 'base64');

crypto = require("crypto");
class LoginHandler {

    m_future = null;

    EncryptROSData(input, sessionKey = "") {
        // initialize state
        let state = new ROSCryptoState();
        let output = [];
    
        // decode session key, if needed
        let hasSecurity = (!sessionKey);
    
        let sessKey;
    
        if (hasSecurity) {
            let keyData = atob(sessionKey);
            sessKey = keyData.slice(0, 16);
        }
    
        // get a random RC4 key
        let rc4Key = crypto.randomBytes(16);
    
        // XOR the key with the global XOR key and write it to the output
        for (let i = 0; i < rc4Key.length; i++) {
            let thisChar = rc4Key[i] ^ state.GetXorKey[i];
    
            output.push(thisChar);
    
            if (hasSecurity) {
                rc4Key[i] ^= sessKey[i];
            }
        }
    
        // create a RC4 cipher for the data
        let rc4 = crypto.createCipheriv("RC4", rc4Key, null);
    
        // encrypt the passed user data using the key
        let inData = Buffer.from(input);
    
        //rc4->encipher(inData);
        inData = rc4.update(inData);
    
        // write the inData to the output stream
        output = Buffer.from(output);
        output = Buffer.concat([output, inData]);
    
        // get a hash for the stream's content so far
        let tempContent = output.slice();
    
        let sha1;
    
        if (!hasSecurity) {
            sha1 = crypto.createHash('sha1');
        } else {
            let hmac = crypto.createHmac("sha1", rc4Key);
    
            sha1 = hmac;
        }
    
        sha1.update(tempContent);
        sha1.update(state.GetHashKey);
    
        let hashData = sha1.digest();
    
        // and return the appended output
        return Buffer.concat([tempContent, hashData]);
    }
    
    DecryptROSData(data, size, sessionKey = "") {
        // initialize state
        let state = new ROSCryptoState();
    
        // read the packet RC4 key from the packet
        rc4Key = Buffer.alloc(16);
    
        let hasSecurity = (!sessionKey); //HaS sEcUrItY, LOL
    
        let sessKey;
    
        if (hasSecurity) {
            let keyData = atob(sessionKey) //ascii b/c raw bytes
            sessKey = keyData.slice(0, 16);
        }
    
        for (let i = 0; i < rc4Key.length; i++) {
            rc4Key[i] = data[i] ^ state.GetXorKey[i];
    
            if (hasSecurity) {
                rc4Key[i] ^= sessKey[i];
            }
        }
    
        // initialize RC4 with the packet key
        let rc4 = crypto.createDecipheriv("RC4", rc4Key, null);;
    
        // read the block size from the data
        let blockSizeData = Buffer.alloc(4);
        //let blockSizeDataLE = Buffer.alloc(4); <== This is a bad way to do it
        blockSizeData = rc4.update(data.slice(16, 20))
    
        // swap endianness
        //blockSizeDataLE[3] = blockSizeData[0];
        //blockSizeDataLE[2] = blockSizeData[1]; <== Use Standard librarys for 
        //blockSizeDataLE[1] = blockSizeData[2];     this, goddamn...
        //blockSizeDataLE[0] = blockSizeData[3];
    
        //uint32_t blockSize = (*(uint32_t*)&blockSizeDataLE) + 20;
    
        let blockSize = blockSizeData.readUint32BE(0) + 20;
    
        // create a buffer for the block
        let blockData;
    
        // a result stringstream as well
        let result = "";
    
        // loop through packet blocks
        let start = 20;
    
        while (start < size) {
            // calculate the end of this block
            let end = Math.min(size, start + blockSize);
    
            // remove the size of the SHA1 hash from the end
            end -= 20;
    
            let thisLen = end - start;
    
            // decrypt the block
            blockData = rc4.update(data.slice(start, start + thisLen));
    
            // TODO: compare the resulting hash
            // In response to this wonderful comment by fleovium of the fivem team...
            // Kindly fuck off, do it yourself ~@dr490n
    
            // append to the result buffer
            result += blockData.toString('utf-8')
    
            // increment the counter
            start += blockSize;
        }
    
        return result;
    }

    ProcessLogin(username, password) //CB ==> Callback, 2 string args
    {
        let callback_l0 = (r, res, rej) => {
            if (r.error) {
                console.error(`ROS error: ${r.error.message}\n`);

                rej("Error contacting Rockstar Online Services.");
            } else {
                let returnedXml = this.DecryptROSData(r.text, r.text.length);
                let tree = new treeBrowse(returnedXml);


                if (!parseInt(tree.get("Response.Status"))) {
                    let code = tree.get("Response.Error.<xmlattr>.Code");
                    let codeEx = tree.get("Response.Error.<xmlattr>.CodeEx");

                    if (code == "AuthenticationFailed" && codeEx == "LoginAttempts") {
                        rej("Login attempts exceeded. Please log in on https://socialclub.rockstargames.com/ and fill out the CAPTCHA, then try again.");
                    } else {
                        rej(`Could not sign on to the Social Club. Error code: ${code}/${codeEx}`);
                    }

                    return;
                } else {
                    res(returnedXml);
                }
            }
        }
        let url = "http://prod.ros.rockstargames.com/launcher/11/launcherservices/auth.asmx/CreateTicketSc3";
        let Body = this.EncryptROSData(BuildPOSTString([
            ["ticket", ""],
            ["email", (username.search('@') != -1) ? username : ""],
            ["nickname", (username.search('@') == -1) ? username : ""],
            ["password", password],
            ["platformName", "pcros"]
        ]));
        let Header = {
            UserAgent: GetROSVersionString(),
            Host: "prod.ros.rockstargames.com"
        };
        return new Promise((res, rej) => {
            xhttp = new XMLHttpRequest();
            xhttponreadystatechange = () => {
                let r = {};
                if (this.readyState == 4 && this.status == 200) {
                    r.text = this.responseText;
                    callback_l0(r, res, rej);
                } else if (this.readyState == 4) {
                    r.error = true;
                    r.error.code = this.status;
                    r.error.message = this.responseText;
                    callback_l0(r, res, rej);
                }
            };

        })
    }

    /*bool HandleRequest(fwRefContainer<net::HttpRequest> request, fwRefContainer<net::HttpResponse> response) override
    {
        request->SetDataHandler([=](const std::vector<uint8_t>& data)
        {
            // get the string
            std::string str(data.begin(), data.end());

            // parse the data
            rapidjson::Document document;
            document.Parse(str.c_str());

            if (document.HasParseError())
            {
                response->SetStatusCode(200);
                response->End(va("{ \"error\": \"pe %d\" }", document.GetParseError()));

                return;
            }

            if (request->GetPath() == "/ros/validate")
            {
                HandleValidateRequest(document, response);

                return;
            }

            auto handleResponse = [=](const std::string& error, const std::string& loginData)
            {
                if (!error.empty())
                {
                    // and write HTTP response
                    response->SetStatusCode(403);
                    response->SetHeader("Content-Type", "text/plain; charset=utf-8");
                    response->End(error);

                    return;
                }

                std::istringstream stream(loginData);

                boost::property_tree::ptree tree;
                boost::property_tree::read_xml(stream, tree);

                // generate initial XML to be contained by JSON
                tinyxml2::XMLDocument document;

                auto rootElement = document.NewElement("Response");
                document.InsertFirstChild(rootElement);

                // set root attributes
                rootElement->SetAttribute("ms", 30.0);
                rootElement->SetAttribute("xmlns", "CreateTicketResponse");

                // elements
                auto appendChildElement = [&](tinyxml2::XMLNode* node, const char* key, auto value)
                {
                    auto element = document.NewElement(key);
                    element->SetText(value);

                    node->InsertEndChild(element);

                    return element;
                };

                auto appendElement = [&](const char* key, auto value)
                {
                    return appendChildElement(rootElement, key, value);
                };

                // create the document
                appendElement("Status", 1);
                appendElement("Ticket", tree.get<std::string>("Response.Ticket").c_str()); // 'a' repeated
                appendElement("PosixTime", static_cast<unsigned int>(time(nullptr)));
                appendElement("SecsUntilExpiration", 86399);
                appendElement("PlayerAccountId", tree.get<int>("Response.PlayerAccountId"));
                appendElement("PublicIp", "127.0.0.1");
                appendElement("SessionId", tree.get<std::string>("Response.SessionId").c_str());
                appendElement("SessionKey", tree.get<std::string>("Response.SessionKey").c_str()); // '0123456789abcdef'
                appendElement("SessionTicket", tree.get<std::string>("Response.SessionTicket").c_str());
                appendElement("CloudKey", "8G8S9JuEPa3kp74FNQWxnJ5BXJXZN1NFCiaRRNWaAUR=");

                // services
                auto servicesElement = appendElement("Services", "");
                servicesElement->SetAttribute("Count", 0);

                // Rockstar account
                tinyxml2::XMLNode* rockstarElement = appendElement("RockstarAccount", "");
                appendChildElement(rockstarElement, "RockstarId", tree.get<std::string>("Response.RockstarAccount.RockstarId").c_str());
                appendChildElement(rockstarElement, "Age", 18);
                appendChildElement(rockstarElement, "AvatarUrl", "Bully/b20.png");
                appendChildElement(rockstarElement, "CountryCode", "CA");
                appendChildElement(rockstarElement, "Email", tree.get<std::string>("Response.RockstarAccount.Email").c_str());
                appendChildElement(rockstarElement, "LanguageCode", "en");
                appendChildElement(rockstarElement, "Nickname", fmt::sprintf("R%08x", ROS_DUMMY_ACCOUNT_ID).c_str());

                appendElement("Privileges", "1,2,3,4,5,6,8,9,10,11,14,15,16,17,18,19,21,22,27");

				auto privsElement = appendElement("Privs", "");
				auto privElement = appendChildElement(privsElement, "p", "");
				privElement->SetAttribute("id", "27");
				privElement->SetAttribute("g", "True");

                // format as string
                tinyxml2::XMLPrinter printer;
                document.Print(&printer);

                // JSON document
                rapidjson::Document json;

                // this is an object
                json.SetObject();

                // append data
                auto appendJson = [&](const char* key, auto value)
                {
                    rapidjson::Value jsonKey(key, json.GetAllocator());

                    rapidjson::Value jsonValue;
                    GetJsonValue(value, json, jsonValue);

                    json.AddMember(jsonKey, jsonValue, json.GetAllocator());
                };

                appendJson("SessionKey", tree.get<std::string>("Response.SessionKey").c_str());
                appendJson("SessionTicket", tree.get<std::string>("Response.SessionTicket").c_str());
                appendJson("Ticket", tree.get<std::string>("Response.Ticket").c_str());
                appendJson("Email", tree.get<std::string>("Response.RockstarAccount.Email").c_str());
                appendJson("SaveEmail", true);
                appendJson("SavePassword", true);
                appendJson("Password", "DetCon1");
                appendJson("Nickname", fmt::sprintf("R%08x", ROS_DUMMY_ACCOUNT_ID).c_str());
                appendJson("RockstarId", tree.get<std::string>("Response.RockstarAccount.RockstarId").c_str());
                appendJson("CallbackData", 2);
                appendJson("Local", false);
                appendJson("SignedIn", true);
                appendJson("SignedOnline", true);
                appendJson("AutoSignIn", false);
                appendJson("Expiration", 86399);
                appendJson("AccountId", tree.get<std::string>("Response.PlayerAccountId").c_str());
                appendJson("Age", 18);
                appendJson("AvatarUrl", "Bully/b20.png");
                appendJson("XMLResponse", printer.CStr());
				appendJson("OrigNickname", tree.get<std::string>("Response.RockstarAccount.Nickname").c_str());

                // serialize json
                rapidjson::StringBuffer buffer;

                rapidjson::Writer<rapidjson::StringBuffer> writer(buffer);
                json.Accept(writer);

                // and write HTTP response
                response->SetStatusCode(200);
                response->SetHeader("Content-Type", "application/json; charset=utf-8");
                response->End(std::string(buffer.GetString(), buffer.GetSize()));
            };

            bool local = (document.HasMember("local") && document["local"].GetBool());

            if (!local)
            {
                std::string username = document["username"].GetString();
                std::string password = document["password"].GetString();

                ProcessLogin(username, password, [=](const std::string& error, const std::string& loginData)
                {
                    if (error.empty())
                    {
                        SaveAccountData(loginData);
                    }

                    handleResponse(error, loginData);
                });
            }
            else
            {
                std::string str;
                bool hasData = LoadAccountData(str);

                if (!hasData)
                {
                    handleResponse("No login data.", "");
                }
                else
                {
                    handleResponse("", str);
                }
            }
        });

        return true;
    }*/
};


class ROSCryptoState {
    /*
    uint8_t m_rc4Key[32];
    uint8_t m_xorKey[16];
    uint8_t m_hashKey[16];
    */

    constructor() {
        let platformStr = atob(ROS_PLATFORM_KEY_LAUNCHER);

        this.m_rc4Key = platformStr.slice(1, 33)
        this.m_xorKey = platformStr.slice(33, 49);
        this.m_hashKey = platformStr.slice(49, 65);

        let m_rc4;

        m_rc4 = crypto.createDecipheriv("RC4", this.m_rc4Key, null);
        this.m_xorKey = m_rc4.update(this.m_xorKey, "binary", "utf8");
        this.m_xorKey + m_rc4.final();

        m_rc4 = crypto.createDecipheriv("RC4", this.m_rc4Key, null);
        this.m_hashKey = m_rc4.update(this.m_hashKey, "binary", "utf8");
        this.m_hashKey + m_rc4.final();
    }

    get GetXorKey() {
        return this.m_xorKey;
    }

    get GetHashKey() {
        return this.m_hashKey;
    }
}

function GetROSVersionString() {
    let xorBuff = Buffer.from([0xC5, 0xC5, 0xC5, 0xC5]);
    let baseStr = "e=1,t=launcher,p=pcros,v=11";
    baseStr = Buffer.from(baseStr);
    xorBuff = Buffer.concat([xorBuff, Buffer.alloc(baseStr.length)]);

    for (let i = 0; i < baseStr.length; i++) {
        xorBuff.writeUInt8(baseStr[i] ^ 0xC5, i + 4);
    }
    let base64str = xorBuff.toString("base64");
    return `ros ${base64str}`;
}

function BuildPOSTString(fields) {
    let retVal = "";
    for (let field of fields) {
        retVal += field[0] + "=" + encodeURIComponent(field[2]) + "&";
    }
    return retVal.substring(0, retVal.length - 1);
}

function HeadersHmac(challenge, method, path, sessionKey, sessionTicket) {

    let cryptoState = new ROSCryptoState();
    let rc4Xor = atob(sessionKey);
    let hmacKey = []

    for (let i = 0; i < 16; i++) {
        hmacKey.push(rc4Xor[i] ^ cryptoState.GetXorKey[i]);
    }


    let hmac = crypto.createHmac("sha1", hmacKey);

    //method
    hmac.update(method);
    hmac.update([0])

    // path
    hmac.update(path);
    hmac.update([0]);

    // ros-SecurityFlags
    hmac.update("239");
    hmac.update([0]);

    // ros-SessionTicket
    hmac.update(sessionTicket);
    hmac.update([0]);

    // ros-Challenge
    hmac.update(btoa(challenge));
    hmac.update([0]);

    // platform hash key
    hmac.update(cryptoState.GetHashKey);

    return Uint8Array(hmac.digest());
}


module.exports = {
    LoginHandler
}
