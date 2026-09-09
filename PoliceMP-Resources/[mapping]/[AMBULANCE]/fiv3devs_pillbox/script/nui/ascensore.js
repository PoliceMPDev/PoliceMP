$(function() {
    var menuOptions = $(".menuoption");
    var mainMenu = $("#mainmenu");
    var actionContainer = $(".actionmenu"); 

    init();
	
    window.addEventListener("message", function(event) {
        var item = event.data;
        if (item.showmenu) {
            ResetMenu();
            $('body').css('background-color', 'rgba(0, 0, 75, 0.15)');
            actionContainer.fadeIn();
        } else if (item.hidemenu) {
            $('body').css('background-color', 'transparent');
            actionContainer.fadeOut();
        }
    });

    document.onkeyup = function(data) {
        if (data.which == 27 && actionContainer.is(":visible")) {
            sendData("ButtonClick", "fechar"); 
        }
    };

    function ResetMenu() {
        $("div").each(function(i, obj) {
            var element = $(this);
            element.attr("data-parent") ? element.hide() : mainMenu.show();
        });
    }

    function init() {
        menuOptions.each(function(i, obj) {
            if ($(this).attr("data-action")) {
                $(this).click(function() {
                    var data = $(this).data("action");
                    $.post("https://fiv3devs_pillbox/GetFloorData", JSON.stringify(data), function(floorData) {
                        if (floorData.enabled) {
                            $.post("https://fiv3devs_pillbox/IsAuthorized", JSON.stringify(data), function(response) {
                                if (response.authorized) {
                                    sendData("ButtonClick", data);
                                    playSong(); 
                                } else {
                                    sendData("ButtonClick", data);
                                }
                            });
                        } else {
                            sendData("ButtonClick", data); 
                        }
                    });
                });
            } else if ($(this).attr("data-sub")) {
                var menu = $(this).data("sub");
                var element = $("#" + menu);
                $(this).click(function() {
                    element.show();
                    mainMenu.hide();
                });
                $(".subtop button, .back").click(function() {
                    element.hide();
                    mainMenu.show();
                });
            }
        });
    }

    function sendData(name, data) {
        $.post("https://fiv3devs_pillbox/" + name, JSON.stringify(data), function(datab) {
            if (datab != "ok") {
                console.log(datab); 
            }
        });
    }

    $('.category_item').click(function() {
		let pegArma = $(this).attr('category');
		$('.item-item').css('transform', 'scale(0)').hide(); 
		setTimeout(function() {
			$('.item-item[category="' + pegArma + '"]').show().css('transform', 'scale(1)');
		}, 100);
	});

    $('.category_item[category="all"]').click(function() {
        setTimeout(function() {
            $('.item-item').show().css('transform', 'scale(1)'); 
        }, 100);
    });

    function playSong() {
		(new Audio('ascensore.mp3')).play().volume = 0.2; 
	}
});
