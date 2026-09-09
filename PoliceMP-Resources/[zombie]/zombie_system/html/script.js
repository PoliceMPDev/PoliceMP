let growlAudio = null;
let growlLoopActive = false;

function playGrowlLoop() {
    if (!growlLoopActive) return;

    const growls = [
        'zombie_growl_1.mp3',
        'zombie_growl_2.mp3'
    ];
    const sound = growls[Math.floor(Math.random() * growls.length)];
    growlAudio = new Audio(sound);
    growlAudio.volume = 0.05;
    growlAudio.play();

    growlAudio.onended = () => {
        if (!growlLoopActive) return;
        const delay = Math.floor(Math.random() * 5000) + 3000; // 3–5 sec
        setTimeout(() => {
            playGrowlLoop();
        }, delay);
    };
}

window.addEventListener('message', function (event) {
    if (event.data.type === 'zombie_near_start') {
        if (!growlLoopActive) {
            growlLoopActive = true;
            playGrowlLoop();
        }
    }

    if (event.data.type === 'zombie_near_stop') {
        growlLoopActive = false;
        if (growlAudio) {
            growlAudio.pause();
            growlAudio.currentTime = 0;
            growlAudio = null;
        }
    }

    if (event.data.type === 'zombie_attack') {
        const attacks = [
            'zombie_aggressive_1.mp3',
            'zombie_aggressive_2.mp3',
            'zombie_aggressive_3.mp3',
            'zombie_aggressive_4.mp3',
            'zombie_aggressive_5.mp3'
        ];
        const sound = attacks[Math.floor(Math.random() * attacks.length)];
        const audio = new Audio(sound);
        audio.volume = 0.3;
        audio.play();
    }

    if (event.data.type === 'zombie_alert') {
        const audio = new Audio('zombie_alert.mp3');
        audio.volume = 0.5;
        audio.play();
    }
});
