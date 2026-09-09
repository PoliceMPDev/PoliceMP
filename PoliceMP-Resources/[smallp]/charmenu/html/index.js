$(function () {
    async function display(bool) {
        if (bool) {
            const isCiv = await $.get("https://PoliceMP/GetIsRunningAsCiv") // thank you Alon you are legend!
            if (isCiv) {
                $.post('https://charmenu/showCursor');
                $("#container").show();
            }
        } else {
            $("#container").hide();
        }
    }

    display(false)

    window.addEventListener('message', function(event) {
        var item = event.data;
        if (item.type === "ui") {
            if (item.status == true) {
                display(true)
            } else {
                display(false)
            }
        }
    })
    
    document.onkeyup = function (data) {
        if (data.which == 27) {
            display(false);
            $.post('https://charmenu/exit');
        }
    };
})

function refreshPed() {
    $.post('https://charmenu/refreshped', JSON.stringify({}));
    $.post('https://charmenu/exit', JSON.stringify({}));
}

document.addEventListener('DOMContentLoaded', function() {
    document.getElementById('refreshButton').addEventListener('click', refreshPed);
});
