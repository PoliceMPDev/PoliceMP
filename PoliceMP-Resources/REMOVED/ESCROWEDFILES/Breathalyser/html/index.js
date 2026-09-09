$(function () {
    function display(bool, forWho, toDo) {
        if (bool) {
            if(forWho === "subject"){
                $("#container").show();
            }
            if(forWho === "source"){
                if(toDo === "Wait"){
                    $("#container2").show();
                }
                if(toDo === "Pass"){
                    $("#container3").show();
                }
                if(toDo === "Fail"){
                    $("#container4").show();
                }
            }
        } else {
            $("#container").hide();
            $("#container2").hide();
            $("#container3").hide();
            $("#container4").hide();
        }
    }

    display(false, "1")

    window.addEventListener('message', function(event) {
        var item = event.data;
        if (item.type === "ui") {
            if (item.status == true) {
                display(true, item.forWho, item.toDo)
            } else {
                display(false, 1)
            }
        }
    })
    //when the user clicks on the submit button, it will run
    $("#submit").click(function () {
        let amount = $("#amount").val()
        $.post('http://Breathalyser/subjectSubmit', JSON.stringify({
            amountOfBreath: amount,
        }));
        return;
    })
    $("#closeSource").click(function () {
        $.post('http://Breathalyser/closeSourceNUI', JSON.stringify({
            close: true,
        }));
        return;
    })
    $("#closeSource2").click(function () {
        $.post('http://Breathalyser/closeSourceNUI2', JSON.stringify({
            close: true,
        }));
        return;
    })
})
