var my_random

$(function () {
    // Existing code...
    window.addEventListener('message', function (event) {
        if (event.data.module == "pmp_boom") {
            if (event.data.event_call == "defuse:toggle_ui:on") {
                document.getElementById('explosives').style.display = "initial";
                my_random = random_number(0, 4);
            } else {
                document.getElementById('explosives').style.display = "none";
            }
        }
    });

    // Close button event listener
    document.getElementById('closeButton').addEventListener('click', function () {
        document.getElementById('explosives').style.display = "none";
        // Optionally, send a message to the server to handle any necessary cleanup
        $.post('https://pmp_boom/closeUI', JSON.stringify({}));
		
    });
});


function explosive_btn_press(key) {
	var display = document.getElementById('display')
	if (key === '#') {
		display.innerHTML = "&nbsp;"
	} else {
		if (display.innerHTML == "&nbsp;") { display.innerHTML = "" }
		display.innerHTML += key
	}
}

function attempt_disarm() {
	var display = document.getElementById('display')
	var end_string = display.innerHTML
	if (end_string == "&nbsp;") { end_string = "" }

    $.post('https://pmp_boom/attempt', JSON.stringify({
        value: end_string
    }));
}

function random_number(min, max) {
	return Math.floor(Math.random() * (max - min + 1) + min)
}

function attempt_cut() {
    // Generate a 50/50 chance
    var isSuccess = Math.random() < 0.5; // 50% chance to be true

    // Post the result as a string since your server-side expects "true" or "false"
    $.post('https://pmp_boom/attempt', JSON.stringify({
        value: isSuccess.toString() // Converts boolean to "true" or "false"
    }));
}
