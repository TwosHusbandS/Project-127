class treeBrowse { //Quick and dirty simulation of boost parseTrees to save time
    constructor (xml) {
        this.dom = (new DOMParser()).parseFromString(xml, "text/xml");
    }
    get = (path) => {
        path = path.split(".");
        return this.#pathParse(path, this.dom);
    }
    #pathParse(path, sdom) {
        if (path.length == 2 && path[0] == "<xmlattr>"){
            return sdom.getAttribute(path[1]);
        } else if (path[0] == "<xmlattr>"){
            throw "XMLATTR Requires 1 argument";
        }
        if (path.length){
            this.#pathParse(path.slice(1), sdom.getElementsByTagName(path[0])[0]);
        } else {
            return sdom.childNodes[0].nodeValue;
        }
    }
}