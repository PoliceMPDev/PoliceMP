$('body').hide();

let PlayingAudio = [];

window.addEventListener("message", (pEvent) => {
    const { data } = pEvent;

    if(data.action == 'PlaySound'){
        if(data.soundId == undefined) return;
        var audio = new Audio(`./audio/${data.sound}`);
        audio.play();
        audio.volume = data.volume;
        audio.loop = data.isLooped;
        PlayingAudio[data.soundId] = audio;
        return;
    }

    if(data.action == 'StopSound'){
        if(data.soundId == undefined) return;
        PlayingAudio[data.soundId].pause();
        return;
    }
    
    if(!data.display){
        $('body').hide();
    }else{
        let html = ''
        data.keys.forEach(key => {
            html += `<div class="flex gap-1">
                <div class="bg-gray-800 rounded p-[5px] flex justify-center items-center rounded">
                    <div class="bg-indigo-800/50 border border-indigo-600 flex justify-center items-center px-2 text-xl text-white rounded shadow-[0_0px_5px_5px_rgba(0,0,0,0.15)]">${key.key}</div>
                </div>
                <div class="bg-gray-800 flex justify-center items-center text-lg text-white px-6 rounded shadow-[0_0px_5px_5px_rgba(0,0,0,0.15)]">
                    ${key.label}
                </div>
            </div>`
        }) 
        $('#keys').html(html);
        $('body').show();
    }
});