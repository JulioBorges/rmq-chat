let urlSocket = $("#urlWebSocket").val();
let connection = new signalR.HubConnectionBuilder().withUrl(urlSocket).build();
let connected = false;

$("#sendMessage").prop('disabled', true);

connection.on("ReceiveMessage", function (sender, message) {
    let user = $("#sender").val();
    let messagesContainer = $("#chat-content");

    let divMessage = document.createElement("div");
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    divMessage.classList.add("media");
    divMessage.classList.add("media-chat");

    if (user === sender) {
        divMessage.innerHTML = `<div class='media-body'>` +
            `<p><img class='avatar' src='https://img.icons8.com/color/36/000000/administrator-male.png' alt='${user}'/>` +
            `<strong>${user} says:</strong> ${msg}</p></div>`;
    } else {
        divMessage.classList.add("media-chat-reverse");
        divMessage.innerHTML = `<div class='media-body'>` +
            `<p><strong>${sender} says:</strong> ${msg}</p></div>`;
    }
    messagesContainer.append(divMessage);
});

connection.on("UserConnected", function (user) {
    let actualUser = $("#sender").val();
    if (actualUser === user && connected) {
        return;
    }

    let messagesContainer = $("#chat-content");
    let divMessage = document.createElement("div");
    divMessage.innerHTML = `<span> User [${user}] connected.`;
    messagesContainer.append(divMessage);
    connected = user === actualUser;
});

connection.start().then(function () {
    let sender = $("#sender").val();
    connection.invoke("ConnectUser", sender).catch(function (err) {
        return console.error(err.toString());
    });
    $("#sendMessage").prop('disabled', false);
}).catch(function (err) {
    return console.error(err.toString());
});


$("#sendMessage").click(function () {

    let sender = $("#sender").val();
    let message = $("#message").val();
    connection.invoke("ProcessMessage", sender, message).catch(function (err) {
        return console.error(err.toString());
    });

    $("#message").val('');
    $("#message").focus();
    event.preventDefault();
});