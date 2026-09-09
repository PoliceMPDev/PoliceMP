/**
 * FiveM NUI Event Handlers
 */
$(function () {
   // $("body").hide();

    window.addEventListener("message", function (event) {
        // Hide and show handlers
        if (event.data.cmd == "show") {
            isComputerOpen = true;
            $("body").show();
        } else if (event.data.cmd == "hide") {
            isComputerOpen = false;
            $("body").hide();
        }
    });
});

var isComputerOpen = false;

/* 
* For sending data back to C#
* Ex: post("eventName", JSON.stringify({ name: 'johnny', age: 99 }));
* Then in C# it would be something like this:
* 
* private async void OnSelectGang(IDictionary<string, object> data, CallbackDelegate callback)
* {
*    var name = data["name"].ToString();
*    if (!int.TryParse(data["age"].ToString(), out int age))
*         return;
*     // then whatever here
* }
*/
function post(eventName, data) {
    return fetch(`https://${GetParentResourceName()}/${eventName}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: data
    });
}

/**
 * Close the computer.
 */
function logout() {
    post("hide", JSON.stringify({dummy: 'hi'}));
    $("body").hide();
}

function focusWindow(wID) {
    for (var key in Desktop.wins) {
        $("#"+key).css("z-index", 1);
    }

    $("#"+wID).css("z-index", 3);
}

/**
 * Desktop
 */

var Desktop = {
    options: {
        windowArea: ".window-area",
        windowAreaClass: "",
        taskBar: ".task-bar > .tasks",
        taskBarClass: ""
    },

    wins: {},

    setup: function(options){
        this.options = $.extend( {}, this.options, options );
        return this;
    },

    addToTaskBar: function(wnd){
        var icon = wnd.getIcon();
        var wID = wnd.win.attr("id");
        var item = $(`<span onclick="focusWindow('`+wnd.win.attr("id")+`')">`).addClass("task-bar-item started").html(icon);
        
        item.data("wID", wID);
        item.appendTo($(this.options.taskBar));
    },

    removeFromTaskBar: function(wnd){
        var wID = wnd.attr("id");
        var items = $(".task-bar-item");
        var that = this;
        $.each(items, function(){
            var item = $(this);
            if (item.data("wID") === wID) {
                delete that.wins[wID];
                item.remove();
            }
        })
    },

    createWindow: function(o){
        o.onDragStart = function(){
            win = $(this);
            $(".window").css("z-index", 1);

            if (!win.hasClass("modal")) {
                win.css("z-index", 3);
            }
        };
        o.onDragStop = function(){
            win = $(this);
            if (!win.hasClass("modal"))
                win.css("z-index", 2);
        };
        o.onWindowDestroy = function(win){
            Desktop.removeFromTaskBar($(win));
        };

        var w = $("<div>").appendTo($(this.options.windowArea));
        var wnd = w.window(o).data("window");

        var win = wnd.win;
        var shift = Metro.utils.objectLength(this.wins) * 16;

        if (wnd.options.place === "auto" && wnd.options.top === "auto" && wnd.options.left === "auto") {
            win.css({
                top: shift,
                left: shift
            });
        }
        this.wins[win.attr("id")] = wnd;
        this.addToTaskBar(wnd);

        return wnd;
    }
};

Desktop.setup();