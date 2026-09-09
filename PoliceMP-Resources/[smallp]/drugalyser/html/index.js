$(function () {
    function display(bool) {
        if (bool) {
            $("#container").show();
        } else {
            $("#container").hide();
        }
    }

    display(false);

    window.addEventListener('message', function (event) {
        var item = event.data;
        if (item.type === "ui") {
            display(item.status);
        } else if (item.type === 'data') {
            let bacDisplay = '';
            let bacColor = '--color-red'; // Default color
    
            // Define patterns based on bac data
            switch (item.bac.toLowerCase()) {
                case "clear":
                    bacDisplay = "0 0 I\n0 0 I\n0 0 I";
                    break;
                case "cocaine":
                    bacDisplay = "0 0 I\n0 0 I\n0 I I";
                    break;
                case "cannabis":
                    bacDisplay = "I 0 I\n0 0 I\n0 0 I";
                    break;
                case "opiates":
                    bacDisplay = "0 0 I\n0 0 I\nI 0 I";
                    break;
                case "meth":
                    bacDisplay = "0 0 I\n0 I I\n0 0 I";
                    break;
                case "benzo":
                    bacDisplay = "0 I I\n0 0 I\n0 0 I";
                    break;
                default:
                    bacDisplay = "";
                    bacColor = '--color-black'; // Use black for unknown results
                    break;
            }
    
            // Convert text into styled HTML
            let formattedDisplay = bacDisplay.split("\n").map(line => {
                return line
                    .split("")
                    .map(char => char === "0" ? `<span class="invisible">0</span>` : char)
                    .join("");
            }).join("<br>");
    
            $('#bacLevel').html(formattedDisplay); // Use .html() to apply styled HTML
            $('#bacLevel').css("color", `var(${bacColor})`);
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
        return;
    });
});
