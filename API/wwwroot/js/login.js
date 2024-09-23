async function authenticate(username, password) {
	const hashBuffer = await digestMessage_SHA384(`${username}:${password}`);
	const hashString = hashBuffer.toHexString();

	const authData = {
		username: "string",
		password: "string",
		algorithmName: "string",
	};

	let response = await fetch("/account/authenticate", {
		method: "POST",
		body: JSON.stringify(authData),
	});
	let authInfo = response.json();
	let isAuthenticated = authInfo["authenticated"] === "true";
	if (response.status === 200 && isAuthenticated) {
		localStorage["session.userId"] = authInfo["userId"];
		localStorage["session.username"] = authInfo["username"];
		localStorage["session.sessionId"] = authInfo["sessionId"];
	} else if (response.status === 401 || !isAuthenticated) {
		let msg = authInfo["message"] ?? "";
		alert("Login failed: " + msg);
	}
}
