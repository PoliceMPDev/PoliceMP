function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

$(function () {
    function display(bool) {
        if (bool) {
            $("#container").show();
            updateDateTime(); // Call to update date and time when the UI is shown
            startDynamicTime(); // Start dynamic time updates
        } else {
            $("#container").hide();
            clearInterval(timeInterval); // Stop updating time when hidden
        }
    }

    // Function to update the current date and time
    function updateDateTime() {
        const now = new Date();
        const options = {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: false
        };
        const formattedDateTime = now.toLocaleString('en-US', options);
        document.getElementById('datetime').innerText = formattedDateTime;
    }

    // Start dynamic time updates every second
    let timeInterval;
    function startDynamicTime() {
        timeInterval = setInterval(updateDateTime, 1000);
    }

    display(false);
    window.addEventListener('message', (event) => {
        var item = event.data;
        if (item.type === "ui") {
            if (item.enable === true) {
                display(true);
                document.body.style.display = event.data.enable ? "block" : "none";
                var str = new String("");
                for (i in item.data) {
                    str = str + `<li>${escapeHtml(item.data[i])}</li>`;
                }
                document.getElementById("notes").innerHTML = str;
                updateDateTime(); // Update the date and time when the UI is enabled
            } else {
                display(false);
                document.body.style.display = event.data.enable ? "none" : "block";
            }
        }
    });

    document.onkeyup = function (data) {
        if (data.which == 27) { // Escape key
            $.post('https://notepad/exit', JSON.stringify({}));
        }
    };

    $("#submit").click(function () {
        let input = $("#form").val();
        if (input.length >= 2048) {
            $.post('https://notepad/error', JSON.stringify({
                error: "Too many characters!",
            }));
            return;
        } else if (!input) {
            $.post('https://notepad/exit', JSON.stringify({}));
            return;
        }
        $.post('https://notepad/save', JSON.stringify({
            main: input,
        }));
        return;
    });

    $("#clear").click(function () {
        $.post('https://notepad/clear', JSON.stringify({}));
    });
});
