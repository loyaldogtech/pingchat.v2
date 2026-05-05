"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .build();

const sendButton = document.getElementById("sendButton");
const messageInput = document.getElementById("messageInput");
const messagesList = document.getElementById("messagesList");
const channelNameInput = document.getElementById("channelName");
const currentUserInput = document.getElementById("currentUser");

if (sendButton) {
    sendButton.disabled = true;
}

connection.on("ReceiveMessage", function (userName, message) {
    const item = document.createElement("li");
    item.className = "list-group-item";

    const now = new Date();
    const timeText = now.toLocaleTimeString([], {
        hour: "numeric",
        minute: "2-digit"
    });

    item.innerHTML = `
        <strong>${userName}</strong><br />
        <small class="text-muted">${timeText}</small><br />
        <span>${message}</span>
    `;

    messagesList.appendChild(item);
});

connection.start()
    .then(function () {
        if (channelNameInput && channelNameInput.value) {
            return connection.invoke("JoinChannel", channelNameInput.value);
        }
    })
    .then(function () {
        if (sendButton) {
            sendButton.disabled = false;
        }
    })
    .catch(function (err) {
        console.error(err.toString());
    });

if (sendButton) {
    sendButton.addEventListener("click", function (event) {
        const message = messageInput.value;
        const channelName = channelNameInput.value;
        const userName = currentUserInput.value || "Anonymous";

        if (!message || !channelName) {
            return;
        }

        connection.invoke("SendMessage", channelName, userName, message)
            .then(function () {
                messageInput.value = "";
                messageInput.focus();
            })
            .catch(function (err) {
                console.error(err.toString());
            });

        event.preventDefault();
    });
}
