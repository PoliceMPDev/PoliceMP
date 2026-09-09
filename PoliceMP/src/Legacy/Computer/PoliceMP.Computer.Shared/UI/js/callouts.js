/**
 * FiveM NUI Event Handlers
 */
$(function () {
    window.addEventListener("message", function (event) {
        if (!isComputerOpen) {
            return;
        }
        
        if (event.data.cmd == "updateCallouts") {
            if (event.data.calloutPlayerIsOn != null && event.data.calloutPlayerIsOn.Id > 0) {
                playerOnCalloutId = event.data.calloutPlayerIsOn.Id;
            }
            updateCallouts(event.data.callouts);            

        } else if (event.data.cmd == "calloutEnded") {
            // If they are viewing the callout that ended, show the all calls page.
            if (event.data.calloutId == viewingCalloutId || calloutWindowPage == "all") {
                calloutShowAll();
            }

        } else if (event.data.cmd == "joinedCallout") {
            playerOnCalloutId = event.data.calloutId;
        }
    });
});

/**
 * Callout Window
 */
var calloutWindow = null;

/**
 * What page are we on?
 * "all", "single", "units"
 */
var calloutWindowPage = "all";

/**
 * The callout that the player is on. -1 if none.
 */
var playerOnCalloutId = -1;

/**
 * The callout that the player is viewing. -1 if none.
 */
var viewingCalloutId = -1;

/**
 * Gets the callout window. Will create it if it's null.
 */
function getCalloutWindow() {
    if (calloutWindow == null) {
        createCalloutWindow();
    }

    return calloutWindow;
}

/**
 * Fill up the callout table with the most updated data
 * but only if that page is being displayed. Otherwise,
 * it would end up overwriting the current page HTML,
 * because they are both written in the same calloutWindow.
 * 
 * @param {array} calloutData 
 */
function updateCallouts(calloutData) {
    // Don't update the display if the callouts window isn't opened
    // to prevent it from opening itself when not needed to.
    if (calloutWindow == null || calloutWindowPage != "all") {
        return;
    }

    // Table head
    var dataString = `
    <div class="container">
        <div class="row">
            <div class="cell-10">
                <div class="panel">
                    <div data-role="panel" data-title-caption="Work info" data-title-icon="<span class='fa fa-users'>" data-collapsible="true" class="panel-content" data-role-panel="true">
                        <table class="table table-border cell-border striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Callout</th>
                                    <th>Grade</th>
                                    <th>Location</th>
                                    <th>Time</th>
                                    <th>On Call</th>
                                    <th>Status</th>
                                    <th>View</th>
                                </tr>
                            </thead>
                            <tbody>`;

            // Loop through callouts
            calloutData.forEach((callout, index) => {
                
                // Highlight if player is on callout
                if (callout.LocalPlayerIsOn) {
                    dataString += `<tr class="bg-cyan fg-white">`;
                } else {
                    dataString += `<tr>`;
                }

                dataString += `
                        <td>`+callout.Id+`</td>
                        <td>`+callout.Title+`</td>
                        <td>`+callout.Grade+`</td>
                        <td>On or near `+callout.Location.toUpperCase()+`</td>
                        <td>`+getFormattedTime(callout.Time)+`</td>
                        <td>`+callout.Players.length+`</td>`;
                // Status
                if (callout.FirstPlayerArrived) {
                    dataString += `<td><code class="bg-green fg-white">On Scene</code></td>`;
                } else if (callout.Players.length == 0) {
                    dataString += `<td><code class="bg-red fg-white">Awaiting</code></td>`
                } else {
                    dataString += `<td><code class="bg-yellow">On Route</code></td>`
                }

                var buttonColor;
                if (callout.LocalPlayerIsOn) {
                    buttonColor = "light";
                } else {
                    buttonColor = "bg-cyan fg-white";
                }
                dataString += `
                        <td class="text-center">
                            <button class="button cycle `+buttonColor+`" onclick="calloutShowSingle(`+callout.Id+`)">
                                <i class="fa fa-arrow-right" aria-hidden="true"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
            
            // Footer
            dataString += `
                    </tbody>
                </table>
                </div>
                <div class="panel-title"><span class="caption">Active Calls</span><span class="fa fa-phone icon"></span></div></div>
                </div>
            <div class="cell-2">
                <div class="tiles-grid tiles-group size-1">
                    <div onclick="calloutShowAll()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                        <span class="fa fa-phone icon"></span>
                        <span class="branding-bar">Active Calls</span>
                    </div>`;

        // Active call button
        if (playerOnCalloutId > 0) {
            dataString += `
                    <div onclick="calloutShowSingle(`+playerOnCalloutId+`)" data-role="tile" class="bg-green tile-medium" data-role-tile="true">
                        <span class="fa fa-car icon"></span>
                        <span class="branding-bar">Current Call</span>
                    </div>
            `;
        } else {
            dataString += `
                    <div data-role="tile" class="bg-gray tile-medium" data-role-tile="true">
                        <span class="fa fa-car icon"></span>
                        <span class="branding-bar">Current Call</span>
                    </div>
            `;
        }
        
        dataString += `
                    <div onclick="calloutShowUnits()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                        <span class="fa fa-user icon"></span>
                        <span class="branding-bar">Unit Info</span>
                    </div>
                    <div onclick="calloutShowAll()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                        <span class="fa fa-refresh icon"></span>
                        <span class="branding-bar">Refresh</span>
                    </div>
                </div>
            </div>
        </div>
    </div>`;

    var w = getCalloutWindow();
    w.elem.innerHTML = dataString;
}

/**
 * Update the single callout page with the specified callout
 * data.
 * 
 * @param {JSON callout data} callout 
 */
function updateSingleCallout(callout) {
    // Don't update the display if the callouts window isn't opened
    // to prevent it from opening itself when not needed to.
    if (calloutWindow == null || calloutWindowPage != "single") {
        return;
    }

    var w = getCalloutWindow();
    var dataString = `
    <div class="container">
        <div class="row">
            <div class="cell-5">
                <div class="panel">
                    <div data-role="panel" data-title-caption="General information" data-title-icon="<span class='mif-info'>" class="panel-content" data-role-panel="true" style="">
                    <table class="table table-border cell-border striped">
                    <tbody>
                        <tr>
                            <td><strong>ID</strong></td>
                            <td>`+callout.Id+`</td>
                        </tr>
                        <tr>
                            <td><strong>Callout</strong></td>
                            <td>`+callout.Title+`</td>
                        </tr>
                        <tr>
                            <td><strong>Grade</strong></td>
                            <td>`+callout.Grade+`</td>
                        </tr>
                        <tr>
                            <td><strong>Location</strong></td>
                            <td>On or near `+callout.Location.toUpperCase()+`</td>
                        </tr>
                        <tr>
                            <td><strong>Lat, Long</strong></td>
                            <td>`+callout.LatLong+`</td>
                        </tr>
                        <tr>
                            <td><strong>Time</strong></td>
                            <td>`+getFormattedTime(callout.Time)+`</td>
                        </tr>
                        <tr>
                            <td><strong>Status</strong></td>`;

            // Status
            if (callout.FirstPlayerArrived) {
                dataString += `<td><code class="bg-green fg-white">On Scene</code></td>`;
            } else if (callout.Players.length == 0) {
                dataString += `<td><code class="bg-red fg-white">Awaiting</code></td>`
            } else {
                dataString += `<td><code class="bg-yellow">On Route</code></td>`
            }

            // Continue
            dataString += `
                        </tr>
                        <tr>
                            <td><strong>Time of Arrival</strong></td>`;

            // Time of arrival
            if (callout.FirstPlayerArrived) {
                dataString += `<td>`+getFormattedTime(callout.TimeOfArrival)+`</td>`;
            } else {
                dataString += `<td>N/A</td>`;
            }
                    
            // Continue
            dataString += `
                        </tr>

                    </tbody>
                </table>
            </div>
            
            <div class="panel-title"><span class="caption">Callout Information</span><span class="mif-info icon"></span></div>
            </div>

            <div class="row">

        </div>
            </div>
            
            <div class="cell-5">
            
            <div class="panel">
            <div data-role="panel" data-title-caption="Work info" data-title-icon="<span class='fa fa-users'>" data-collapsible="true" class="panel-content" data-role-panel="true">
            <table class="table table-border cell-border striped">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Ranks</th>
                    <th>Status</th>
                    <th>Time of Arrival</th>
                </tr>
            </thead>
            <tbody>`;

    // Players
    callout.Players.forEach((player, index) => {
        dataString += `
            <tr>
                <td>`+player.Name+`</td>
                <td>`;

                // Ranks
                if (player.Ranks.length > 0) {
                    player.Ranks.forEach((rank, index) => {
                        dataString += `<code>`+rank+`</code>`;
                    });
                } else {
                    dataString += `<code>Cadet</code>`;
                }
                

                dataString += "</td>";

                // On scene or not
                if (player.HasArrived) {
                    dataString += `<td><code class="bg-green fg-white">On Scene</code></td>
                        <td>`+getFormattedTime(player.TimeOfArrival)+`</td>`;
                } else {
                    dataString += `<td><code class="bg-yellow">On Route</code></td>
                        <td>N/A</td>`;
                }

                dataString += `
            </tr>
        `;
    });
    
    dataString += `
            </tbody>
        </table>
        <div class="text-right">`;
        // Join or leave button
            if (callout.LocalPlayerIsOn) {
                dataString += `<button onclick="leaveCallout(`+callout.Id+`)" class="button alert">Leave</button>`;
            } else {
                dataString += `<button onclick="joinCallout(`+callout.Id+`)" class="button bg-cyan fg-white">Self-Dispatch</button>`;
            }

    dataString += `
        </div>
    </div>
    <div class="panel-title"><span class="caption">Assigned Units</span><span class="fa fa-users icon"></span></div></div>
    </div>
    <div class="cell-2">
    <div class="tiles-grid tiles-group size-1">
        <div onclick="calloutShowAll()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
            <span class="fa fa-phone icon"></span>
            <span class="branding-bar">Active Calls</span>
        </div>`;

    // Active call button
    if (playerOnCalloutId > 0) {
        dataString += `
                <div onclick="calloutShowSingle(`+playerOnCalloutId+`)" data-role="tile" class="bg-green tile-medium" data-role-tile="true">
                    <span class="fa fa-car icon"></span>
                    <span class="branding-bar">Current Call</span>
                </div>
        `;
    } else {
        dataString += `
                <div data-role="tile" class="bg-gray tile-medium" data-role-tile="true">
                    <span class="fa fa-car icon"></span>
                    <span class="branding-bar">Current Call</span>
                </div>
        `;
    }
     
    dataString += `
                <div onclick="calloutShowUnits()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                        <span class="fa fa-user icon"></span>
                        <span class="branding-bar">Unit Info</span>
                    </div>
                    <div onclick="calloutShowSingle(`+callout.Id+`)" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                        <span class="fa fa-refresh icon"></span>
                        <span class="branding-bar">Refresh</span>
                    </div>
                </div>
            </div>
        </div>   
        <br />
    </div>`;

    w.elem.innerHTML = dataString;
}

/**
 * Update the units page with the specified units data.
 * 
 * @param {JSON data units list} units 
 */
function updateUnits(units) {
    if (calloutWindow == null || calloutWindowPage != "units") {
        return;
    }

    var w = getCalloutWindow();
    var dataString = `
        <div class="container">
            <div class="row">
                <div class="cell-10">
                    <div class="panel">
                        <div data-role="panel" data-title-caption="Work info" data-title-icon="<span class='fa fa-users'>" data-collapsible="true" class="panel-content" data-role-panel="true">
                        <table class="table table-border cell-border striped">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Ranks</th>
                                <th>Status</th>
                                <th>Location</th>
                            </tr>
                        </thead>
                        <tbody>`;

    units.forEach((unit, index) => {
        dataString += `
                            <tr>
                                <td>`+unit.Id+`</td>
                                <td>`+unit.Name+`</td>
                                <td>`;
            
        // Ranks
        unit.Ranks.forEach((rank, index) => {
            dataString +=           `<code>`+rank+`</code> `;
        })

        dataString += `
                                </td>
                                <td>`+unit.Status+`</td>
                                <td>`+unit.Location.toUpperCase()+`</td>
                            </tr>`
    });

    dataString += `     
                        </tbody>
                    </table>
                </div>
                <div class="panel-title"><span class="caption">Units</span><span class="fa fa-users icon"></span></div></div>

                </div>
                <div class="cell-2">
                    <div class="tiles-grid tiles-group size-1">
                        <div onclick="calloutShowAll()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                            <span class="fa fa-phone icon"></span>
                            <span class="branding-bar">Active Calls</span>
                        </div>`;

            // Active call button
            if (playerOnCalloutId > 0) {
                dataString += `
                        <div onclick="calloutShowSingle(`+playerOnCalloutId+`)" data-role="tile" class="bg-green tile-medium" data-role-tile="true">
                            <span class="fa fa-car icon"></span>
                            <span class="branding-bar">Current Call</span>
                        </div>
                `;
            } else {
                dataString += `
                        <div data-role="tile" class="bg-gray tile-medium" data-role-tile="true">
                            <span class="fa fa-car icon"></span>
                            <span class="branding-bar">Current Call</span>
                        </div>
                `;
            }
            
            dataString += `
                        <div onclick="calloutShowUnits()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                            <span class="fa fa-user icon"></span>
                            <span class="branding-bar">Unit Info</span>
                        </div>
                        <div onclick="calloutShowUnits()" data-role="tile" class="bg-cyan tile-medium" data-role-tile="true">
                            <span class="fa fa-refresh icon"></span>
                            <span class="branding-bar">Refresh</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `;

    w.elem.innerHTML = dataString;
}

/**
 * Switches page to the "show all callouts" page.
 */
function calloutShowAll() {
    calloutWindowPage = "all";
    viewingCalloutId = -1;
    getCalloutWindow();
    post("requestCalloutData", JSON.stringify({ name: 'johnny', age: 99 })).then(resp => resp.json()).then(resp => {
        updateCallouts(JSON.parse(resp));
    });
}

/**
 * Shows the single callout page for the specified callout.
 * 
 * @param {int} id 
 */
function calloutShowSingle(id) {
    calloutWindowPage = "single";
    viewingCalloutId = id;
    getCalloutWindow();
    post("requestSingleCalloutData", JSON.stringify({ id: id })).then(resp => resp.json()).then(resp => {
        var callout = JSON.parse(resp);
        updateSingleCallout(callout);
    });
}

/**
 * Shows the units page.
 */
function calloutShowUnits() {
    calloutWindowPage = "units";
    viewingCalloutId = -1;
    getCalloutWindow();
    post("requestUnitsData", JSON.stringify({mate: 'mate'})).then(resp => resp.json()).then(resp => {
        var units = JSON.parse(resp);
        updateUnits(units);
    });
}

/**
 * Send a request to FiveM to join the callout.
 * 
 * @param {int} id 
 */
function joinCallout(id) {
    post("joinCallout", JSON.stringify({id: id})).then(resp => resp.json()).then(resp => {
        var result = JSON.parse(resp);
        if (result == "error") {
            swal({
                title: "Error",
                text: "Could not self-dispatch to call "+id+". Are you already on a call?",
                icon: "error",
              });
        } else if (result == "success") {
            swal({
                title: "Success",
                text: "You have successfully self-dispatched to call "+id+".",
                icon: "success",
              });
            playerOnCalloutId = id;
            calloutShowSingle(id);
        }
    });
}

/**
 * Send a request to FiveM to leave the callout.
 * 
 * @param {int} id 
 */
function leaveCallout(id) {
    post("leaveCallout", JSON.stringify({id: id}));
    swal({
        title: "Success",
        text: "You have successfully left call "+id+".",
        icon: "success",
      });
    playerOnCalloutId = -1;
    calloutShowAll();
}

/**
 * Creates the callout window and returns it.
 */
function createCalloutWindow() {
    calloutWindow = Desktop.createWindow({
        resizeable: true,
        draggable: true,
        width: 1000,
        height: 700,
        icon: "<span class='fa fa-phone'></span>",
        title: "Calls",
        onClose: function(win) {
            calloutWindow = null;
        }
    });
}

/**
 * Gets a formatted date.
 * Ex: 16:35 23-May-2020
 * 
 * @param {Date} date 
 */
function getFormattedDate(date) {
    return moment(date).format('HH:mm DD-MMM-YYYY');
}

/**
 * Gets a formatted time.
 * Ex: 16:35
 * 
 * @param {Date} date 
 */
function getFormattedTime(date) {
    return moment(date).format('HH:mm:ss');
}