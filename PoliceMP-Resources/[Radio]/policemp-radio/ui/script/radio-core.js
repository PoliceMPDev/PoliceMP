var currentState = 'hidden';
var currentChannel = false;
var talking = false;
var powerOn = false;

var radioChannelConfig = {
    1: 'Ops 1',
    2: 'Int Ops',
    3: 'Event 1',
    4: 'Event 2',
    5: 'Event 3',
    6: 'LAS',
    7: 'LFB',
    8: 'Highways',
    9: 'Training 1',
    10: 'Training 2',
};

function setState(state) {
    var position;

    switch (state) {
        case 'hidden':
            position = -70;
        break;

        case 'active':
            position = -20;
        break;

        case 'modifying':
            position = 2;
        break;
    };

    $('#radio-core').css('bottom', `${position}vh`);

    currentState = state;
};

function playSound(sound, volume) {
    var audio = document.getElementById(sound);
    audio.volume = volume;
    audio.play();
};

function toggleModifying(status) {
    if (status == false) {
        if ($('#active-talkers-container > p > i > i').length < 1) {
            setState('hidden');
        } else {
            setState('active');
        }
    } else {
        setState('modifying');
    };
};

function togglePower(status) {
    if (status == true) {
        $('#radio-screen').css('background', 'none')
        $('#menu-options').css('display', 'flex');
        $('#active-talkers-container').css('display', 'block');

        powerOn = true;
    } else {
        $('#radio-screen').css('background', 'black')
        $('#menu-options').css('display', 'none');
        $('#active-talkers-container').css('display', 'none');

        powerOn = false;
    };
};

function clearSpeakers() {
    $('#active-talkers-container > p > i').html('');

    if (currentState !== 'modifying') {
        setState('hidden')
    };
};

function addSpeaker(channelID, playerID, name) {
    if (channelID !== currentChannel) {
        return;
    };

    $('#active-talkers-container').css('display', 'block');


    playSound('talk-start', 0.4)

    var newLine = `<i id='speaking-unit-${playerID}'><br>${name} (${playerID})</i>`;

    $('#active-talkers-container > p > i').append(newLine);

    if (currentState == 'hidden') {
        setState('active')
    };
};

function removeSpeaker(channelID, playerID) {
    if (channelID !== currentChannel) {
        return;
    };

    var elementID = `#speaking-unit-${playerID}`;

    playSound('talk-end', 0.4)

    $(elementID).remove();

    var speakerLeft = $('#active-talkers-container > p > i > i').length;

    if (speakerLeft < 1) {
        clearSpeakers();
    };
};

function changeChannel(channelID, speakers) {
    if (radioChannelConfig[channelID] == null) {
        return
    };

    currentChannel = channelID;

    clearSpeakers();

    $('#talk-group-container > h1').html(radioChannelConfig[channelID]);

    for (var playerID in speakers) {
        addSpeaker(channelID, playerID, speakers[playerID]);
    };
};

function coreChangeChannel(channel) {
    if (powerOn === true) {
        playSound('button-click', 0.3)

        fetch('http://policemp-radio/change-channel', {
            method: 'POST',
            body: JSON.stringify({
                channel: channel
            })
        });
    };
};

function channelIncrease() {
    var newID = currentChannel + 1

    if (newID > Object.keys(radioChannelConfig).length) {
        newID = 1;
    };

    changeChannel(newID, {});

    coreChangeChannel(newID)
}

function channelDecrease() {
    var newID = currentChannel - 1

    if (newID < 1) {
        newID = Object.keys(radioChannelConfig).length;
    }

    changeChannel(newID, {});

    coreChangeChannel(newID)
}

function setChannel(channel){
    changeChannel(channel);
    coreChangeChannel(channel);
}

$(document).keydown(function(event) {
    if (currentState !== 'modifying') {
        return;
    };

    if (event.keyCode === 113 && event.shiftKey) {
        toggleModifying(false)

        fetch('http://policemp-radio/close-modification', {
            method: 'POST',
            body: JSON.stringify({})
        });
    }
})

$(document).keyup(function(event) {
    if (currentState !== 'modifying') {
        return;
    };

    if (event.keyCode == 27) {
        toggleModifying(false)

        fetch('http://policemp-radio/close-modification', {
            method: 'POST',
            body: JSON.stringify({})
        });
    };
});

$(document).ready(function() {
    window.addEventListener('message', (event) => {
        var item = event.data;

        if (item == undefined) {
            return
        }

        switch (item.type) {
            case 'radio-voice-toggle':
                if (item.talking == true) {
                    addSpeaker(item.channel, item.user, item.name)
                } else {
                    removeSpeaker(item.channel, item.user)
                }
            break;

            case 'toggle-modification':
                toggleModifying(item.status)
            break;

            case 'update-radiochannel':
              console.log('Updated Radio Channel to: ' + item.channel);
              if(item.channel == 0) { break;}
              setChannel(item.channel);
              break;
        }
    })
});

$('#radio-on').click(function() {
    if (powerOn == false) {
        togglePower(true)

        changeChannel(1, {})

        fetch('http://policemp-radio/power-on', {
            method: 'POST',
            body: JSON.stringify({})
        });
    };
});

$('#radio-off').click(function() {
    if (powerOn == true) {
        togglePower(false)

        fetch('http://policemp-radio/power-off', {
            method: 'POST',
            body: JSON.stringify({})
        });
    };
});

$('#radio-channel-up').click(function() {
    if (powerOn == true) {
        channelIncrease();
    };
});

$('#radio-channel-down').click(function() {
    if (powerOn == true) {
        channelDecrease();
    };
});


$('#channel-1').click(function() {
    if (powerOn == true) {
        setChannel(1);
    };
});

$('#channel-2').click(function() {
    if (powerOn == true) {
        setChannel(2);
    };
});
$('#channel-3').click(function() {
    if (powerOn == true) {
        setChannel(3);
    };
});

$('#channel-4').click(function() {
    if (powerOn == true) {
        setChannel(4);
    };
});
$('#channel-5').click(function() {
    if (powerOn == true) {
        setChannel(5);
    };
});
$('#channel-6').click(function() {
    if (powerOn == true) {
        setChannel(6);
    };
});
$('#channel-7').click(function() {
    if (powerOn == true) {
        setChannel(7);
    };
});
$('#channel-8').click(function() {
    if (powerOn == true) {
        setChannel(8);
    };
});
$('#channel-9').click(function() {
    if (powerOn == true) {
        setChannel(9);
    };
});
$('#channel-10').click(function() {
    if (powerOn == true) {
        setChannel(10);
    };
});
