const utf8Encoder = new TextEncoder();

ArrayBuffer.prototype.toHexString = function () {
    const byteArr = new Uint8Array(this);
    let stringArr = Array(byteArr.length);
    for (i = 0; i < byteArr.length; i++) {
        stringArr[i] = byteArr[i].toString(16).padStart(2, "0");
    }
    let result = stringArr.join("");
    return result;
};

async function digestMessage_SHA384(message) {
    const encoder = new TextEncoder();
    const data = encoder.encode(message);
    const hash = await window.crypto.subtle.digest("SHA-384", data);
    return hash;
}

function getSessionToken() {
    return localStorage.getItem("sessionToken");
}
function getSessionAccountId() {
    return localStorage.getItem("accountId");
}
function truncateString(str, len) {
    return str.substring(0, Math.min(len, str.length));
}

function formatCPR(cprString) {
    if (!cprString instanceof String) {
        return "";
    }
    let cpr = cprString.replace(/\D/g, "");
    // cpr = cpr.replaceAll("[^0-9]", "");
    if (cpr.length > 6) {
        cpr = cpr.substring(0, 6) + "-" + cpr.substring(6);
    }
    return truncateString(cpr, 11);
}

async function authenticate(username, password) {
    const hashBuffer = await digestMessage_SHA384(password);
    const hashString = hashBuffer.toHexString();

    const authData = {
        login: username,
        passwordHash: hashString,
    };

    let response = await fetch("/account/authenticate", {
        headers: {
            "Authentication": "Lagg " + getSessionToken(),
            "content-type": "application/json; charset=utf-8",
        },
        method: "POST",
        body: JSON.stringify(authData),
    });
    let authInfo = await response.json();
    console.log("authInfo: ", authInfo);
    if (response.status === 200 && authInfo["authenticated"]) {
        localStorage.setItem("accountId", authInfo["accountId"]);
        localStorage.setItem("sessionToken", authInfo["sessionToken"]);
        return true;
    } else {
        let msg = authInfo["message"] ?? "";
        alert("Login failed: " + msg);
        return false;
    }
}

function killSession() {
    console.log("logout()");
    localStorage.removeItem("accountId");
    localStorage.removeItem("sessionToken");
    window.location.replace("/index.html");
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
async function updateCpr(accountId, cpr) {
    let url = `/Account/${accountId}/cpr/${pr}`;
    console.log("updating cpr: ", url);
    let response = await fetch(url, {
        headers: {
            "Authentication": "Lagg " + getSessionToken(),
        },
        method: "PUT",
    });

    return response.status === 200;
}

async function validateCpr(accountId, cpr) {

}