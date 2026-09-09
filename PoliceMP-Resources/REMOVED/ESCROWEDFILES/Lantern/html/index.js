$(function () {
    function display(bool, forWho) {
        if (bool) {
            if(forWho === "subject"){
                $("#container").show();
            }
            if(forWho === "source"){
                $("#container2").show();
            }
        } else {
            $("#container").hide();
            $("#container2").hide();
        }
    }

    display(false, "1")

    window.addEventListener('message', function(event) {
        var item = event.data;
        if (item.type === "ui") {
            if (item.status == true) {
                display(true, item.forWho)
            } else {
                display(false, 1)
            }
        }
    })
    //when the user clicks on the submit button, it will run
    $("#submit").click(function () {
        let firstName = $("#firstname").val()
        let secondName = $("#secondname").val()
        let dob = $("#dob").val()
        $.post('http://Lantern/names', JSON.stringify({
            firstname: firstName,
            secondname: secondName,
            Dob: dob,
        }));
        return;
    })
    $("#abort").click(function () {
        $.post('http://Lantern/abort', JSON.stringify({
            text: "Not on PNC"
        }));
        return;
    })

    $("#skip").click(function () {
        $.post('http://Lantern/skip', JSON.stringify({
            text: "Skip"
        }));
        return;
    })

    $("#retry").click(function () {
        $.post('http://Lantern/retry', JSON.stringify({
            text: "Retry"
        }));
        return;
    })

    $("#next").click(function () {
        $.post('http://Lantern/next', JSON.stringify({
            text: "Next"
        }));
        return;
    })
})
