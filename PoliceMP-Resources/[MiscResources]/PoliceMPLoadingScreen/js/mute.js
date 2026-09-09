document.addEventListener('DOMContentLoaded', function () {
    var tag = document.createElement('script');
    tag.src = "https://www.youtube.com/iframe_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

    var player;
    window.onYouTubeIframeAPIReady = function() {
        console.log('YouTube IFrame API ready');
        player = new YT.Player('videoIframe', {
            events: {
                'onReady': onPlayerReady,
            }
        });
    };

    function onPlayerReady(event) {
        event.target.setVolume(5);
        var muteButton = document.getElementById('muteButton');
        var icon = muteButton.querySelector('i');

        muteButton.addEventListener('click', function() {
            if (player.isMuted()) {
                player.unMute();
                icon.classList.remove('fa-volume-mute');
                icon.classList.add('fa-volume-up');
            } else {
                player.mute();
                icon.classList.remove('fa-volume-up');
                icon.classList.add('fa-volume-mute');
            }
        });
    }
});
