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
const antiForgeryInput = document.querySelector("#antiForgeryForm input[name='__RequestVerificationToken']");

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

function createDeleteForm(messageId, channelName) {
    const form = document.createElement("form");
    form.method = "post";
    form.action = `/Chat/${encodeURIComponent(channelName)}?handler=Delete&name=${encodeURIComponent(channelName)}`;

    const messageIdInput = document.createElement("input");
    messageIdInput.type = "hidden";
    messageIdInput.name = "messageId";
    messageIdInput.value = messageId;

    form.appendChild(messageIdInput);

    if (antiForgeryInput && antiForgeryInput.value) {
        const tokenInput = document.createElement("input");
        tokenInput.type = "hidden";
        tokenInput.name = "__RequestVerificationToken";
        tokenInput.value = antiForgeryInput.value;
        form.appendChild(tokenInput);
    }

    const button = document.createElement("button");
    button.type = "submit";
    button.className = "btn btn-sm btn-outline-danger";
    button.textContent = "Delete";

    form.appendChild(button);

    return form;
}

function appendMessage(messageId, userName, messageText, isoUtcString) {
    if (!messagesList || !channelNameInput) return;

    const emptyState = messagesList.querySelector("[data-empty-state='true']");
    if (emptyState) {
        emptyState.remove();
    }

    const item = document.createElement("li");
    item.className = "list-group-item";
    item.setAttribute("data-message-id", messageId);

    const timeText = formatTimestamp(isoUtcString);
    const currentUserName = currentUserInput?.value || "";

    const wrapper = document.createElement("div");
    wrapper.className = "d-flex justify-content-between align-items-start gap-3";

    const content = document.createElement("div");
    content.className = "flex-grow-1";
    content.innerHTML = `
        <strong>${userName}</strong><br />
        <small class="text-muted">${timeText}</small><br />
        <span>${messageText}</span>
    `;

    wrapper.appendChild(content);

    if (currentUserName && userName === currentUserName) {
        const deleteForm = createDeleteForm(messageId, channelNameInput.value);
        wrapper.appendChild(deleteForm);
    }

    item.appendChild(wrapper);
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
