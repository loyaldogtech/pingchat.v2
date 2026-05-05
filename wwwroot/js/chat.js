"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .build();

const sendButton = document.getElementById("sendButton");
const messageInput = document.getElementById("messageInput");
const messagesList = document.getElementById("messagesList");
const channelNameInput = document.getElementById("channelName");
const currentUserInput = document.getElementById("currentUser");
const messagesContainer = document.getElementById("messagesContainer");

function scrollMessagesToBottom() {
    if (!messagesContainer) return;
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function formatTimestamp(isoUtcString) {
    const date = new Date(isoUtcString);
    return date.toLocaleTimeString([], {
        hour: "numeric",
        minute: "2-digit"
    });
}

function appendMessage(messageId, userName, messageText, isoUtcString) {
    if (!messagesList) return;

    const emptyState = messagesList.querySelector("[data-empty-state='true']");
    if (emptyState) {
        emptyState.remove();
    }

    const item = document.createElement("li");
    item.className = "list-group-item";
    item.setAttribute("data-message-id", messageId);

    const timeText = formatTimestamp(isoUtcString);

    item.innerHTML = `
        <div class="d-flex justify-content-between align-items-start gap-3">
            <div class="flex-grow-1">
                <strong>${userName}</strong><br />
                <small class="text-muted">${timeText}</small><br />
                <span>${messageText}</span>
            </div>
        </div>
    `;

    messagesList.appendChild(item);
    scrollMessagesToBottom();
}

if (sendButton) {
    sendButton.disabled = true;
}

connection.on("ReceiveMessage", function (messageId, userName, messageText, createdAtUtc) {
    appendMessage(messageId, userName, messageText, createdAtUtc);
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

        if (messageInput) {
            messageInput.focus();
        }

        scrollMessagesToBottom();
    })
    .catch(function (err) {
        console.error(err.toString());
    });

function sendCurrentMessage() {
    if (!messageInput || !channelNameInput) {
        return;
    }

    const message = messageInput.value.trim();
    const channelName = channelNameInput.value;
    const userName = currentUserInput?.value || "Anonymous";

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
}

if (sendButton) {
    sendButton.addEventListener("click", function (event) {
        event.preventDefault();
        sendCurrentMessage();
    });
}

if (messageInput) {
    messageInput.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            sendCurrentMessage();
        }
    });
}
