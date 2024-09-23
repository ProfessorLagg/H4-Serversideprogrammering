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

function getSessionToken(){
    return localStorage.getItem("sessionToken")
}
