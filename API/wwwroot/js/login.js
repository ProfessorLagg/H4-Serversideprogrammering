async function authenticate(username, password) {
	const hashBuffer = await digestMessage_SHA384(password);
	const hashString = hashBuffer.toHexString();

	const authData = {
		login: username,
		passwordHash: hashString,
	};

	let response = await fetch("/account/authenticate", {
		headers: {
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
