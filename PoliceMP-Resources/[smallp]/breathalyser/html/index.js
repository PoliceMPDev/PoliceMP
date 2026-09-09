$(function () {
    function display(bool) {
        if (bool) {
            $("#container").show();
            var audio = new Audio('sounds/BeepShort.ogg');
            audio.play();
        } else {
            $("#container").hide();
        }
    }

    display(false);

    window.addEventListener('message', function(event) {
        var item = event.data;
        if (item.type === "ui") {
            display(item.status);
        } else if (item.type === 'data') {
            $('#bacLevel').text(item.bac);
            $('#bacLevel').css("color", `var(${item.textColor})`);
        }
    });

    document.onkeyup = function (data) {
        if (data.which == 27) { // ESC key
            $.post(`https://${GetParentResourceName()}/exit`, JSON.stringify({}));
            return;
        }
    };

    $("#power").click(function () {
        $.post(`https://${GetParentResourceName()}/exit`, JSON.stringify({}));
        return;
    });

    $("#start").click(function () {
        $.post(`https://${GetParentResourceName()}/startBac`, JSON.stringify({}));

        // Ensure the audio file path is correct. Assuming 'BeepShort.ogg' is in 'html/sounds/' folder
        var audio = new Audio('sounds/BeepShort.ogg');
        audio.play();

        return;
    });
});
