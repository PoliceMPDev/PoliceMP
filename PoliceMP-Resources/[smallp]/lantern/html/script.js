document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("nui").style.display = "none"; // Hide NUI by default
});

// Listen for messages from the Lua client
window.addEventListener("message", function (event) {
    if (event.data.action === "open") {
        document.getElementById("nui").style.display = "flex"; // Show the NUI
    }
});

// Handle form submission
document.getElementById("submitBtn").addEventListener("click", function () {
    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const dob = document.getElementById("dob").value.trim();

    if (firstName && lastName && dob) {
        // Send data to the Lua client
        fetch(`https://${GetParentResourceName()}/submitDetails`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ firstName, lastName, dob })
        }).then(() => {
            document.getElementById("nui").style.display = "none"; // Hide the NUI
        }).catch((err) => {
            console.error("Error sending data:", err);
        });
    } else {
        // Send "NOT ON PNC" response to Lua
        fetch(`https://${GetParentResourceName()}/submitDetails`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ firstName: "NOT ON PNC", lastName: "", dob: "" })
        }).then(() => {
            document.getElementById("nui").style.display = "none"; // Hide the NUI
        }).catch((err) => {
            console.error("Error sending 'NOT ON PNC' response:", err);
        });
    }
});

// Handle ESC key to close the NUI
window.addEventListener("keydown", function (event) {
    if (event.key === "Escape") {
        document.getElementById("nui").style.display = "none"; // Hide the NUI
        fetch(`https://${GetParentResourceName()}/close`, { method: "POST" }).catch((err) => {
            console.error("Error closing NUI:", err);
        });
    }
});
