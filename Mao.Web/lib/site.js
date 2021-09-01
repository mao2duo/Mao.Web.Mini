/** html as text */
function htmlEncode(html) {
    return $("<div></div>").text(html).html();
}
/** text as html */
function htmlDecode(text) {
    return $("<div></div>").html(text).text();
}
/** set name/value to cookie */
function setCookie(name, value, expireDays) {
    var expires = "";
    if (expireDays) {
        var now = new Date();
        now.setTime(now.getTime() + (expireDays * 24 * 60 * 60 * 1000));
        expires = ";expires=" + now.toUTCString();
    }
    document.cookie = name + "=" + escape(value) + expires + ";path=/";
}
/** get value by name from cookie */
function getCookie(name) {
    name = name + "=";
    var cookies = document.cookie.split(';');
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        while (cookie.charAt(0) == ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) == 0) {
            return unescape(cookie.substring(name.length, cookie.length));
        }
    }
    return "";
}
/** delete cookie by name */
function deleteCookie(name) {
    setCookie(name, "", -1);
}

/**
 * 存放大量文字的 cookie
 * @param {string} name
 * @param {any} value
 * @param {Number} expireDays
 */
function setBigCookie(name, value, expireDays) {
    deleteBigCookie(name);
    var str;
    if (typeof value != "undefined") {
        str = value.toString();
    }
    var index = 0;
    while (str.length > 0) {
        setCookie(name + "[" + index + "]", str.substring(0, 1024), expireDays);
        str = str.substring(1024);
        index++;
    }
}
/**
 * 取得大量文字的 cookie
 * @param {string} name
 */
function getBigCookie(name) {
    var index = 0;
    var str = "";
    var value = "";
    while (index == 0 || value.length) {
        value = getCookie(name + "[" + index + "]");
        str = str + value;
        index++;
    }
    return str;
}
/**
 * 刪除大量文字的 cookie
 * @param {string} name
 */
function deleteBigCookie(name) {
    var index = 0;
    var value = "";
    while (index == 0 || value.length) {
        value = getCookie(name + "[" + index + "]");
        if (value.length) {
            setCookie(name + "[" + index + "]", "", -1);
        }
        index++;
    }
}

/** get query string by name from url */
function getQueryString(name) {
    var regex = new RegExp("[?&]" + name.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)", "i");
    var results = regex.exec(location.href);
    if (results) {
        if (results[2]) {
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }
        return "";
    }
    return null;
}
/** 複製文字至剪貼簿 */
function copyToClipboard(text) {
    if (window.clipboardData && window.clipboardData.setData) {
        window.clipboardData.setData("Text", text);
        return Promise.resolve();
    }
    else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
        var textarea = document.createElement("textarea");
        textarea.textContent = text;
        textarea.style.position = "fixed";
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        document.body.removeChild(textarea);
        return Promise.resolve();
    }
    return Promise.reject();
}