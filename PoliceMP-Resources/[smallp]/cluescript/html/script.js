document.addEventListener('DOMContentLoaded', function() {
    let currentMarkerID = null;

    window.addEventListener('message', function(event) {
        let data = event.data;

        if (data.action === "open") {
            document.getElementById("container").style.display = "block";
            document.getElementById("message-box").style.display = "none";
            document.getElementById("message").value = ''; // Clear input field

            // Focus on the input field
            setTimeout(() => {
                document.getElementById("message").focus(); 
            }, 100);
        } else if (data.action === "showMessage") {
            document.getElementById("message-display").innerText = data.text;
            document.getElementById("message-box").style.display = "block";
            document.getElementById("container").style.display = "none";

            currentMarkerID = data.clueID;
            document.getElementById("delete").style.display = data.canDelete ? "inline-block" : "none";
        } else if (data.action === "hideMessage") {
            document.getElementById("message-box").style.display = "none";
        } else if (data.action === "hide") {
            document.getElementById("container").style.display = "none";
            document.getElementById("message-box").style.display = "none";
        } else if (data.action === "clearInput") {
            document.getElementById("message").value = ''; // Clear input field
        }
    });

    document.getElementById("submit").addEventListener("click", function() {
        const message = document.getElementById("message").value;
        if (message.trim() !== "") {
            fetch(`https://${GetParentResourceName()}/submitMessage`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ text: message })
            }).then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok ' + response.statusText);
                }
                document.getElementById("message").value = ''; 
                document.getElementById("container").style.display = "none"; // Close dialog
            }).catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
        } else {
            console.log("Input field is empty, not submitting");
        }
    });

    document.getElementById("delete").addEventListener("click", function() {
        if (currentMarkerID !== null) {
            fetch(`https://${GetParentResourceName()}/deleteClue`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ clueID: currentMarkerID })
            }).then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok ' + response.statusText);
                }

                document.getElementById("message-box").style.display = "none"; // Close message box
                document.getElementById("container").style.display = "none"; // Hide NUI after deletion

                fetch(`https://${GetParentResourceName()}/closeNUI`, { method: "POST" });
            }).catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
        }
    });

    document.getElementById("close").addEventListener("click", function() {
        document.getElementById("container").style.display = "none"; // Hide NUI
        document.getElementById("message-box").style.display = "none"; // Ensure message box is hidden
        fetch(`https://${GetParentResourceName()}/closeNUI`, { method: "POST" });
    });

    document.getElementById("close-container").addEventListener("click", function() {
        document.getElementById("container").style.display = "none"; // Hide NUI
        fetch(`https://${GetParentResourceName()}/closeNUI`, { method: "POST" });
    });
});
