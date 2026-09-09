let AudioPlaying = null;

function PlayAudio(pAudio, pLoop){
    if(AudioPlaying != null){
        AudioPlaying.pause();
        AudioPlaying = null;
    }
    if(pAudio === '') return;
    audioPlayer = new Howl({src: ["./media/audio/" + pAudio + ".ogg"]});
    audioPlayer.volume(RadioVolume / 100);
    audioPlayer.loop(pLoop);
    audioPlayer.play();
    AudioPlaying = audioPlayer; 
}