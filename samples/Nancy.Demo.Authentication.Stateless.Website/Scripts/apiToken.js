var apiKeyKey = "sample_apiKey";
var usernameKey = "sample_username";

var ApiToken = {

    Set: function (username, apiKey, rememberMe) {
        var days = rememberMe ? 10 : 0;
        createCookie(apiKeyKey, apiKey, days);
        createCookie(usernameKey, username, days);
    },

    Get: function () {
        var key = readCookie(apiKeyKey);
        var username = readCookie(usernameKey);
        var token = { Key: key, Username: username, IsValid: key != null };
        return token;
    },

    Delete: function () {
        eraseCookie(apiKeyKey);
        eraseCookie(usernameKey);
    }
};

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    } else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
    console.log(document.cookie);
}

function readCookie(name) {
    var cookieValue = null;
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) {
            var found = c.substring(nameEQ.length, c.length);
            cookieValue = found;
        }
    }
    return cookieValue;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}