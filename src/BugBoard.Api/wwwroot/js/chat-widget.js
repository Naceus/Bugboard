
let hasOpened = false;
const sessionId = crypto.randomUUID();
const chatInput = document.getElementById("chat-input-field");
const messages = document.querySelector(".chat-messages");
document.getElementById("chat-toggle").addEventListener("click", () => {
    document.getElementById("chat-window").classList.toggle("open");

    if (!hasOpened) {
        document.querySelector(".chat-messages").innerHTML += `<div>
        <strong>
        Agent
        </strong>
        <br>
        Hi there! 👋
        <br>
        My name is Nathan.
        <br>
        How can I assist you today?</div>`
        hasOpened = true;
    }
})

document.getElementById("chat-send-btn").addEventListener("click", () => {
    document.querySelector(".chat-messages").innerHTML += `
    <br>
    <strong>
    You
    </strong>
    <br>
    <div>${chatInput.value}</div>`
    messages.scrollTop = messages.scrollHeight;
    fetch(`/Api/Agent`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ chatInput: chatInput.value, sessionId })
    }).then(response => response.json()).then(data => {
        document.querySelector(".chat-messages").innerHTML += `<div>
        <br>
        <strong>
        Agent
        </strong>
        <br>
        ${data.output}
        <br>
        </div>`
        messages.scrollTop = messages.scrollHeight;
    })
    chatInput.value = "";
})
