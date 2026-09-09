$(function () {
    function display(bool, forWho, cannabis, cocaine) {
        if (bool) {
            if(forWho === "subject"){
                $("#container").show();
            }
            if(forWho === "source"){
                if(cannabis === "CannabisYes" && cocaine === "CocaineNo"){
                    $("#container2").show();
                }
                if(cannabis === "CannabisNo" && cocaine === "CocaineYes"){
                    $("#container3").show();
                }
                if(cannabis === "CannabisYes" && cocaine === "CocaineYes"){
                    $("#container4").show();
                }
                if(cannabis === "CannabisNo" && cocaine === "CocaineNo"){
                    $("#container5").show();
                }
            }
        } else {
            $("#container").hide();
            $("#container2").hide();
            $("#container3").hide();
            $("#container4").hide();
            $("#container5").hide();
        }
    }

    display(false, "1")

    window.addEventListener('message', function(event) {
        var item = event.data;
        if (item.type === "ui") {
            if (item.status == true) {
                display(true, item.forWho, item.Cannabis, item.Cocaine)
            } else {
                display(false, 1, 1, 1)
            }
        }
    })
    //when the user clicks on the submit button, it will run
    $("#submit").click(function () {
        var checkCannabisRadio = document.querySelector('input[name="cannabisYes_No"]:checked');
        var checkCocaineRadio = document.querySelector('input[name="cocaineYes_No"]:checked');
        $.post('http://Drugalyser/subjectSubmit', JSON.stringify({
            cannabis: checkCannabisRadio.id,
            cocaine: checkCocaineRadio.id,
        }));
        return;
    })
})
