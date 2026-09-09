/*
    * Author: Tim Plate
    * Project: Advanced Roleplay Environment
    * Copyright (c) 2022 Tim Plate Solutions
*/

var selectedBodyPart = "HEAD";
var lastActionCategory = "";
var bodyPartInformation = {};
var bodyPartsBleeding = [];
var tourniquets = [];
var infusions = [];
var lastLogs = [];
var bloodVolume = 0;
var localizationData = {};
var activePain = 0;
var lastActionClick = 0;
var soundsPlaying = [];

function closeMenu() {
    $(".nearbyPlayersContainer").hide();
    $(".menuContainer").fadeOut("fast");
    fetch(`https://${GetParentResourceName()}/NUIEventCloseMenu`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        }
    });
}

$(document).keydown(function (e) {
    switch (e.keyCode) {
        case 27: // ESC
            closeMenu();
            break;
    }
});

$(".closeButton").click(closeMenu);

function getDebugValue(debugId) {
    return $(`.debugValue[data-id='${debugId}'] .value`).text();
}

function updateDebugValue(debugId, debugValue) {
    if (typeof debugValue == 'number') debugValue = debugValue.toFixed(2);
    if (typeof debugValue == 'string') {
        if (debugValue == "OK") $(`.debugValue[data-id='${debugId}'] .value`).css("color", "green");
        else if (debugValue == "FATAL") $(`.debugValue[data-id='${debugId}'] .value`).css("color", "red");
        else if (debugValue == "HEMORRAGE I" || debugValue == "HEMORRAGE II" || debugValue == "HEMORRAGE III" || debugValue == "HEMORRAGE IV") $(`.debugValue[data-id='${debugId}'] .value`).css("color", "#006747");
        else $(`.debugValue[data-id='${debugId}'] .value`).css("color", "#006747");
    }

    $(`.debugValue[data-id='${debugId}'] .value`).text(debugValue);
}

const getTime = (time) => {
    const d = new Date(time * 1000);
    const dd = [d.getHours(), d.getMinutes(), d.getSeconds()].map((a) => (a < 10 ? '0' + a : a));
    return dd.join(':');
};

window.addEventListener('message', (event) => {
    switch (event.data.payload) {
        case "createDebugValue":
            if (typeof event.data.payloadData.value == 'number') event.data.payloadData.value = event.data.payloadData.value.toFixed(2);
            $(".debugContainer").append(`<div class="debugValue" data-id="${event.data.payloadData.id}">
            <div class="key">${event.data.payloadData.id}</div>
            <div class="value">${event.data.payloadData.value}</div>
            </div>`);
            break;

        case "deleteDebugValue":
            $(`.debugValue[data-id='${event.data.payloadData.id}']`).remove();
            break;

        case "updateDebugValue":
            updateDebugValue(event.data.payloadData.id, event.data.payloadData.value);
            break;

        case "createDebugSection":
            $(".debugContainer").append(`<div class="debugSection" data-id="${event.data.payloadData.name}">${event.data.payloadData.name}</div>`);
            break;

        case "deleteDebugSection":
            $(`.debugSection[data-id='${event.data.payloadData.id}']`).remove();
            break;

        case "toggleDispatchInfo":
            event.data.payloadData.value ? $(".contact_medic_box").fadeIn("fast") : $(".contact_medic_box").fadeOut("fast");

            if (event.data.payloadData.key) $(".contact_medic_key").text(event.data.payloadData.key);
            break;

        case "pushAvailableActions":
            _addActions(JSON.parse(event.data.payloadData.actions), event.data.payloadData.bodyPart, event.data.payloadData.category);
            break;

        case "updateInternalPainEffect":
            updateInternalPainEffect(event.data.payloadData.value);
            break;

        case "updateInternalBleedingEffect":
            updateInternalBleedingEffect(event.data.payloadData.value);
            break;

        case "updateHeartbeatSound":
            updateHeartbeatSound(event.data.payloadData.value);
            break;

        case "showManualRespawnText":
            if (event.data.payloadData.value)
                $(".manual_respawn_box").fadeIn("fast");
            else
                $(".manual_respawn_box").fadeOut("fast");

            if (event.data.payloadData.key) $(".manual_respawn_key").text(event.data.payloadData.key);

            break;

        case "stateMenu":
            $(".nearbyPlayersContainer").hide();

            if (event.data.payloadData.value) {
                $(".menuContainer").fadeIn("fast");
                $(".menuName").text(event.data.payloadData.title);
                onClickBodyPart("HEAD");
            } else {
                $(".menuContainer").fadeOut("fast");
            }

            break;

        case "updateLogs":
            $("#logList").html("");
            var logs = JSON.parse(event.data.payloadData.logs);
            if (!logs) return;

            logs.sort(function (a, b) {
                return b.timestamp - a.timestamp;
            });

            lastLogs = logs;

            logs.forEach(function (log) {
                $("#logList").append(`<li><div class="content">${log.message}</div><div class="time">${getTime(log.timestamp)}</div></li>`);
            });

            break;

        case "loadBodyPartInformation":
            stateLoadingIndicator(true);
            loadBodyPartInformation(event.data.payloadData.value, event.data.payloadData.tourniquets, event.data.payloadData.bleedings, event.data.payloadData.infusions, event.data.payloadData.pain, event.data.payloadData.bloodVolume, event.data.payloadData.triageSelection, event.data.payloadData.unconscious);
            break;

        case "updateECGValues":
            updateHeartRate(Math.round(event.data.payloadData.heartRate));
            updateBloodPressure(event.data.payloadData.highBP, event.data.payloadData.lowBP);
            break;

        case "toggleECGMenu":
            $(".ecg").css("display", event.data.payloadData.value ? "block" : "none");
            break;

        case "forcedUpdateBodyPartInformation":
            _showInjuries(selectedBodyPart, _getBodyPartInjuries(selectedBodyPart));
            _getAvailableActionsForCategory(lastActionCategory, selectedBodyPart);
            break;

        case "toggleTriageSelection":
            event.data.payloadData ? $(".triageSelection").show() : $(".triageSelection").hide();

        case "toggleUnconsciousScreen":
            if (event.data.payloadData.value) {
                $(".remaining_time").hide();
                $(".remaining_time").text(`00:00 ${_translateText("MINUTES_REMAINING")}`);
            }

            event.data.payloadData.value ? $(".unconsciousScreen").fadeIn("fast") : $(".unconsciousScreen").fadeOut("fast");
            break;

        case "toggleAnesthesiaScreen":
            event.data.payloadData.value ? $(".anesthesiaScreen").fadeIn("fast") : $(".anesthesiaScreen").fadeOut("fast");
            break;

        case "updateUnconsciousTime":
            //const minutes = Math.floor(event.data.payloadData.seconds / 60);
            //$(".remaining_time").text(`${minutes.toString().padStart(2, "0")}:${(event.data.payloadData.seconds % 60).toString().padStart(2, "0")} ${_translateText("MINUTES_REMAINING")}`);
            //$(".unconsciousScreen .progressbar").css("width", event.data.payloadData.seconds / event.data.payloadData.max_seconds * 100 + "%");

            //if ($(".remaining_time").css("display") == "none") $(".remaining_time").fadeIn("fast");
            break;

        case "play3dSound":
            play3dSound(event.data.payloadData.id, event.data.payloadData.coords, event.data.payloadData.distance, event.data.payloadData.name);
            break;

        case "stop3dSound":
            stop3dSound(event.data.payloadData.id);
            break;
    }
});

function stateLoadingIndicator(state) {
    state ? $(".loadingIndicator").fadeIn("fast") : $(".loadingIndicator").fadeOut("fast");
}

$(".bodyPart").click(function () {
    onClickBodyPart($(this).attr("data-bodyPart"));
});

$(".actionCategory i").click(function () {
    onClickAction(this);
});

function onClickBodyPart(bodyPartIdentifier) {
    if (Date.now() - lastActionClick < 250) return;

    selectedBodyPart = bodyPartIdentifier;
    lastActionClick = Date.now();

    $(".actionCategory i").removeClass("active");

    var localizedName = _translateText(bodyPartIdentifier);
    $(".bodyPartTitle").text(localizedName);

    var bodyPartCategory = _getBodyPartCategory(bodyPartIdentifier);
    _rethinkAvailableActions(bodyPartCategory, false);
    _showInjuries(bodyPartIdentifier, _getBodyPartInjuries(bodyPartIdentifier));
}

function onClickAction(htmlElement) {
    if ($(htmlElement).hasClass("disabled")) return;

    $(".actionCategory i").removeClass("active");

    var actionIdentifier = $(htmlElement).attr("data-actionIdentifier");
    $(htmlElement).addClass("active");

    lastActionCategory = actionIdentifier;
    _getAvailableActionsForCategory(actionIdentifier, selectedBodyPart);
}

function loadBodyPartInformation(bodyPartInjuries, tourniquetsData, bleedings, infusionsData, pain, bloodV, triageSelection, unconscious) {
    bodyPartInformation = JSON.parse(bodyPartInjuries);
    tourniquets = JSON.parse(tourniquetsData);
    bodyPartsBleeding = JSON.parse(bleedings);
    infusions = JSON.parse(infusionsData);
    bloodVolume = bloodV;
    activePain = pain;

    $('.bodyPart').each(function () {
        $(this).prop("src", $(this).attr("orig-src"));
    });

    for (const [key, value] of Object.entries(bodyPartInformation)) {
        var oldLevel = "none";

        value.forEach(injury => {
            var bodyPart = _getBodyPartByIdentifier(key);
            _updateBodyPartLevel(bodyPart, oldLevel, injury.level, injury.needSewing);
            oldLevel = injury.level;
        });
    }

    $(".bodyPart[data-tourniquet]").hide();
    tourniquets.forEach((location) => {
        const bodyPart = _getTourniquetBodyPartByIdentifier(location);
        $(bodyPart).show();
    });

    $("#triageSelect").val(triageSelection);
    $("#triageSelect").attr("data-value", triageSelection);
    unconscious ? $("#black_triage").show() : $("#black_triage").hide();

    stateLoadingIndicator(false);
}

var activePainInterval = undefined;
var _painEffectSwitch = false;

function updateInternalPainEffect(painLevel) {
    if (painLevel > 0 && !activePainInterval) {
        activePainInterval = setInterval(() => {
            if (_painEffectSwitch) {
                $(".painEffect").css("background", `radial-gradient(circle, rgba(0,0,0,0) 0%, rgba(255,255,255,0.75) ${Math.max(500 / painLevel, 200)}%)`)
                $(".painEffect").fadeOut("fast");
            } else {
                $(".painEffect").css("background", `radial-gradient(circle, rgba(0,0,0,0) 0%, rgba(255,255,255,0.75) ${Math.max(500 / painLevel, 200)}%)`)
                $(".painEffect").fadeIn("fast");
            }

            _painEffectSwitch = !_painEffectSwitch;
        }, Math.max(500 / painLevel, 250));
    } else {
        if (!activePainInterval) return;
        clearInterval(activePainInterval);
        activePainInterval = undefined;
    }

    if (painLevel <= 0 && !activePainInterval && $(".painEffect").css("display") == "block") $(".painEffect").fadeOut("fast");
}

function updateInternalBleedingEffect(bleeding) {
    $(`.bleedingEffect`).css("background", `radial-gradient(circle, rgba(0,0,0,0) 0%, rgba(255,0,0,0.75) 1000%`)
    bleeding ? $(".bleedingEffect").fadeIn("fast") : $(".bleedingEffect").fadeOut("fast");
}

var currentHeartbeatSound;
function updateHeartbeatSound(type) {
    if (type == "none") {
        if (!currentHeartbeatSound) return;
        currentHeartbeatSound.audio.pause();
        currentHeartbeatSound = undefined;
        return;
    }

    if (!currentHeartbeatSound) {
        currentHeartbeatSound = {
            type: type,
            audio: new Audio(`assets/sounds/heartrate_${type}.wav`)
        }
        currentHeartbeatSound.audio.loop = true;
        currentHeartbeatSound.audio.volume = 0.4;
        currentHeartbeatSound.audio.play();
        return;
    }

    if (currentHeartbeatSound.type == type) return;
    currentHeartbeatSound.audio.pause();

    currentHeartbeatSound.audio = new Audio(`assets/sounds/heartrate_${type}.wav`);
    currentHeartbeatSound.type = type;
    currentHeartbeatSound.audio.play();
}

function _getBodyPartInjuries(bodyPart) {
    return bodyPartInformation[bodyPart];
}

function _getBodyPartCategory(bodyPartIdentifier) {
    if (bodyPartIdentifier.includes("HEAD")) return "HEAD";
    if (bodyPartIdentifier.includes("ARM")) return "ARM";
    if (bodyPartIdentifier.includes("TORSO")) return "TORSO";
    if (bodyPartIdentifier.includes("LEG")) return "LEG";

    return "UNKNOWN";
}

function _forceSelectActionCategory(actionCategoryHtmlElement) {
    onClickAction(actionCategoryHtmlElement);
}

function _capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function _showInjuries(bodyPart, injuries) {
    const bloodLossLevel = returnBloodLossLevel(bloodVolume);
    const infusionsAtBodyPart = _getInfusionsAtBodyPart(bodyPart);
    const tourniquetAppliedText = tourniquets.includes(bodyPart) ? `<li style="color: #006747; font-weight: 600;">${_translateText("TOURNIQUET_APPLIED")}</li>` : "";
    const bleedingText = bodyPartsBleeding.includes(bodyPart) ? `<li style="color: #e62c2c; font-weight: 600;">${_translateText("ACTIVE_BLEEDING")}</li><br>` : "<br>";
    const painText = activePain > 0.0 ? `<li style="color: #FFFF00">${_translateText("ACTIVE_PAIN")}</li>` : "";
    const bloodVolumeText = bloodLossLevel ? `<li style="color: #e62c2c">${bloodLossLevel}</li>` : "";

    $(".bodyInjuries").html("");
    $(".bodyInjuries").append(tourniquetAppliedText);
    $(".bodyInjuries").append(painText);
    $(".bodyInjuries").append(bloodVolumeText);
    $(".bodyInjuries").append(bleedingText);

    for (i = 0; i < infusionsAtBodyPart.length; i++) {
        var infusion = infusionsAtBodyPart[i];
        $(".bodyInjuries").append(`<li style="color: #03b1fc; font-weight: 600;">${_translateText("ACTIVE_INFUSION")} ${_translateText("INFUSION_" + infusion.name.toUpperCase())} (~${Math.round(infusion.remainingVolume / 5) * 5}ml ${_translateText("VOLUME_LEFT")})</li> ${i == infusionsAtBodyPart.length - 1 ? "<br>" : ""}`);
    }

    if (!injuries || injuries.length == 0) {
        $(".bodyInjuries").append(`<li>${_translateText("NO_INJURIES_AT_THIS_BODY_PART")}</li>`);
        return;
    }

    injuries.forEach(injury => {
        const injuryNormal = `<li>1x ${_translateText(injury.level.toUpperCase())} ${_translateText(injury.key.toUpperCase())}</li>`;
        const injuryNoLevel = `<li>1x ${_translateText(injury.key.toUpperCase())}</li>`;
        const injuryNeedSewing = `<li style="color: #5a38e0; font-weight: 500;">1x ${_capitalizeFirstLetter(_translateText(injury.key.toUpperCase()))} (${_translateText("NEED_SEWING")})</li>`;
        
        if (injury.needSewing) {
            $(".bodyInjuries").append(injuryNeedSewing);
            return;
        }

        const hideLevelOnInjuries = [
            'puncturedlung',
            'crush',
            'burn_injury',
            'compartmentsyndrome',
            'blocked_airways',
            'partial_blockage',
            'broken_neck',
            'broken_toe',
            'broken_finger',
            'broken_ribs',
            'loss_of_circulation',
            'tbi',
            'heart_attack',
            'overdose',
            'anaphylaxis',
            'asthma_attack',
            'hypothermia',
            'electrocution',
            'hypoglycemia',
            'poisoned',
            'cardio_disturbance',
            'seizure',
            'barbs',
            'covid',
            'ebola',
            'anthrax',
            'tuberculosis',
            'smallpox',
            'yellowfever',
            'meningitis',
            'barbs',
            'interal_bleeding',
            'chest_infection',
            'mid_femur_fracture',
            'dislocation',
            'choking',
            'degloved_wound',
            'diabetic_ketoacidosis',
            'sucking_chest_wound',
            'abdominal_aneurysm',
            'hemothorax',
            'tension_pneumothorax',
            'blunt_chest_trauma',
            'cardiac_tamponade',
            'super_glued',


        ];
        if (hideLevelOnInjuries.includes(injury.key)) {
            $(".bodyInjuries").append(injuryNoLevel);
            return;    
        }

        $(".bodyInjuries").append(injuryNormal);
    });
}

function _rethinkAvailableActions(bodyPartCategory, isSelf) {
    var allowedActions = [];

    switch (bodyPartCategory) {
        case "HEAD":
            allowedActions = ["diagnoses", "bandages", "oxygen", "syringes", "doctor"];
            break;

        case "ARM":
            allowedActions = ["diagnoses", "bandages", "syringes", "doctor"];
            if (!isSelf) allowedActions.push("infusions");
            break;

        case "TORSO":
            allowedActions = ["bandages"];
            if (!isSelf) allowedActions.push("cpr", "doctor", "syringes");
            break;

        // case "TORSO":
        //     allowedActions = ["bandages", "oxygen"];
        //     if (!isSelf) allowedActions.push("cpr");
        //     break;
    

        case "LEG":
            allowedActions = ["diagnoses", "bandages", "syringes", "doctor"];
            break;
    }

    if (!isSelf) allowedActions.push("carry");

    $(".actionCategory i").addClass("disabled");
    allowedActions.forEach(action => {
        $(`.actionCategory i[data-actionIdentifier=${action}]`).removeClass("disabled");
    });

    _forceSelectActionCategory($(`.actionCategory i[data-actionIdentifier=${allowedActions[0]}]`));
}

function _addActions(actions, bodyPart, category) {
    var availableActions = [];
    actions.forEach((data) => {
        availableActions.push(_returnAction(data, bodyPart));
    });

    $(".buttonContainer").html("");
    $(".buttonContainer").hide();
    availableActions.forEach(action => {
        $(".buttonContainer").append(`<div class="actionButton" data-payload='${JSON.stringify(action.action_data)}'>${_translateText(action.translation_key)}</div>`);
    });
    $(".buttonContainer").show();

    $(".actionButton").click(function () {
        if (typeof $(this).attr("data-payload") === 'undefined') return;

        var actionData = JSON.parse($(this).attr("data-payload"));

        fetch(`https://${GetParentResourceName()}/NUIEventTriggerAction`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=UTF-8',
            },
            body: JSON.stringify({
                name: actionData.name,
                bodyPart: bodyPart
            })
        });
    });
}

function _getAvailableActionsForCategory(actionCategoryIdentifier, bodyPart) {
    fetch(`https://${GetParentResourceName()}/NUIEventGetAvailableActions`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: JSON.stringify({
            category: actionCategoryIdentifier,
            bodyPart: bodyPart
        })
    });
}

function _returnAction(translationKey, payload) {
    return {
        translation_key: translationKey,
        action_data: {
            name: translationKey,
            payload: payload
        }
    }
}

function _getNearbyPlayers() {
    fetch(`https://${GetParentResourceName()}/NUIEventGetNearbyPlayers`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: JSON.stringify({}),
    }).then(response => response.json()).then(data => {
        _updateNearbyPlayers(data);
    });
}

function _updateNearbyPlayers(players) {
    $(".nearbyPlayers").html("");
    players.forEach(player => {
        $(".nearbyPlayers").append(`<div class="nearbyPlayer" data-id="${player.id}">${player.name} (${player.distance.toFixed(2)}m)</div>`);
    });

    $(".nearbyPlayersContainer").fadeIn("fast");

    $(".nearbyPlayer").click(function () {
        var playerId = $(this).attr("data-id");
        _loadPlayerMenu(playerId);
    });
}

$(".playersButton").click(function () {
    _getNearbyPlayers();
});

$(".nearbyPlayersContainer>.containerClose").click(function () {
    $(".nearbyPlayersContainer").fadeOut("fast");
});

function _loadPlayerMenu(id) {
    fetch(`https://${GetParentResourceName()}/NUIEventGetPlayerMenu`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: JSON.stringify({
            id: id
        })
    });
}

function _updateBodyPartLevel(element, oldLevel, level, needSewing) {
    var currentImage = $(element).prop("src");
    var newLevel = "n";

    var newLevelRank = _getInjuryLevelRank(level);
    var oldLevelRank = _getInjuryLevelRank(oldLevel);

    switch (level) {
        case "minor":
            newLevel = "l"
            break;

        case "medium":
            newLevel = "m"
            break;

        case "large":
            newLevel = "h"
            break;

        case "fatal":
            newLevel = "s"
            break;
    }

    if (oldLevelRank > newLevelRank) return;
    if (needSewing) newLevel = "s";

    var newImageSrc = currentImage.split(";")[0] + ";" + newLevel + ".png";
    $(element).prop("src", newImageSrc);
}

$("img").attr("draggable", "false");

function _getInjuryLevelRank(level) {
    switch (level) {
        case "minor": return 1;
        case "medium": return 2;
        case "large": return 3;
        case "fatal": return 4;
        default: return 0;
    }
}

function _getInfusionsAtBodyPart(bodyPart) {
    return infusions.filter(x => x.bodyPart == bodyPart && x.remainingVolume > 0);
}

function _getBodyPartByIdentifier(identifier) {
    return $(`.bodyPart[data-bodyPart='${identifier}']`).not("[data-tourniquet]");
}

function _getTourniquetBodyPartByIdentifier(identifier) {
    return $(`.bodyPart[data-tourniquet][data-bodyPart='${identifier}']`);
}

function returnBloodLossLevel(bloodVolume) {
    if (bloodVolume < 3000) return _translateText("LOST_A_FATAL_AMOUNT_OF_BLOOD");
    else if (bloodVolume < 3600) return _translateText("LOST_A_LARGE_AMOUNT_OF_BLOOD");
    else if (bloodVolume < 4200) return _translateText("LOST_A_LOT_OF_BLOOD");
    else if (bloodVolume < 5400) return _translateText("LOST_SOME_BLOOD");
    return false;
}

// ToDo finish returnPainLevel
function returnPainLevel(painLevel) {
    if (painLevel > 5.0) return _translateText("HAS_A_VERY_HIGH_PAIN_LEVEL");
    else if (bloodVolume < 3600) return _translateText("LOST_A_LARGE_AMOUNT_OF_BLOOD");
    else if (bloodVolume < 4200) return _translateText("LOST_A_LOT_OF_BLOOD");
    else if (bloodVolume < 5400) return _translateText("LOST_SOME_BLOOD");
    return false;
}

function play3dSound(soundId, coords, distance, soundName) {
    var sound = new Howl({
        src: ['assets/sounds/' + soundName + '.wav'],
        volume: 1.0,
        id: soundId,
        onend: function () {
            delete soundsPlaying[soundId];
        },
    });

    sound.pos(coords.x, coords.y, coords.z);
    sound.orientation(coords.x, coords.y, coords.z);

    sound.pannerAttr({
        panningModel: 'equalpower',
        refDistance: 1,
        rolloffFactor: 5,
        distanceModel: 'linear'
    }, soundId);

    sound.play();
    soundsPlaying[soundId] = {
        sound: sound,
        coords: coords,
        distance: distance,
        id: soundId
    };
}

function stop3dSound(soundId) {
    if (typeof soundsPlaying[soundId] === 'undefined') return;

    var sound = soundsPlaying[soundId];
    sound.sound.stop();
    delete soundsPlaying[soundId];
}

setInterval(function () {
    if (Object.keys(soundsPlaying).length > 0) {
        fetch(`https://${GetParentResourceName()}/NUIEventUpdatePlayerPosition`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=UTF-8',
            },
            body: JSON.stringify({
            })
        }).then(res => res.json()).then(data => {
            data = JSON.parse(data);
            Howler.pos(data.coords.x, data.coords.y, data.coords.z);
            Howler.orientation(data.rotation.x, data.rotation.y, data.rotation.z, 0, 0, 0);

            for (var sound in soundsPlaying) {
                sound = soundsPlaying[sound];
                Howler.mute(calculateDistance(data.coords, sound.coords) > sound.distance, sound.id);
            }
        });
    }
}, 100);

String.format = function (format) {
    var args = Array.prototype.slice.call(arguments, 1);
    return format.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
            ? args[number]
            : match
            ;
    });
};

function _translateText(/**/) {
    var args = arguments;
    if (localizationData[args[0]] != undefined)
        return String.format(localizationData[args[0]], args[1])
    else
        return "Not translated: " + args[0]
}

function calculateDistance(p1, p2) {
    var a = p2.x - p1.x;
    var b = p2.y - p1.y;
    var c = p2.z - p1.z;

    return Math.sqrt(a * a + b * b + c * c);
}

$("#logCopy").click(function () {
    let text = "";
    lastLogs.forEach(function (log) {
        text += `[${getTime(log.timestamp)}] ${log.message}\n`;
    });

    /* Clipboard Workaround since navigator.[...] isn't allolwed */
    const el = document.createElement('textarea');
    el.value = text;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
});

var oldValue = $('#triageSelect').val();
$('#triageSelect').on('change', function () {
    const triageSelection = this.value;

    fetch(`https://${GetParentResourceName()}/NUIEventUpdateTriage`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        },
        body: JSON.stringify({
            triageSelection: triageSelection
        })
    }).then(res => res.json()).then(data => {
        if (!data) {
            $(this).val(oldValue);
            return;
        }

        oldValue = triageSelection;
        $(this).attr("data-value", triageSelection);
    });
});

$(document).ready(() => {
    fetch(`https://${GetParentResourceName()}/NUIEventLoadLocalizationData`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8',
        }
    }).then(response => response.json()).then(data => {
        fetch(`../script/languages/${data}.json`)
            .then(response => response.json())
            .then(data => {
                localizationData = data.messages;

                $("div").each(function () {
                    if ($(this).text().startsWith("NUI_"))
                        $(this).text(_translateText($(this).text().split("NUI_")[1]));
                });

                $("span").each(function () {
                    if ($(this).text().startsWith("NUI_"))
                        $(this).text(_translateText($(this).text().split("NUI_")[1]));
                });

                $("option").each(function () {
                    if ($(this).text().startsWith("NUI_"))
                        $(this).text(_translateText($(this).text().split("NUI_")[1]));
                });

                $(".draggable").draggable();
            });
    });
});
