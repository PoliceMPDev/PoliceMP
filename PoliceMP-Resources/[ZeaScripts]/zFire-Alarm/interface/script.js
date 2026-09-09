const soundStore = {};
const getElementById = (id) => document.getElementById(id);
const getComputedStyleProperty = (element, property) => getComputedStyle(element)[property];
var panelAddress
var panelId

window.addEventListener('message', async ({ data }) => {
  const { type, load } = data;
  const sound = soundStore[load[0]];

  switch (type) {
    case 'showUi':
      panelId = load[0];
      panelAddress = load[3];
      showUi(load[1], load[2]);
    break;

    case 'play':
      if (!sound) {
        loadSound(load[0], load[1][0], 0.0, false);
        playAudioLoop(load[0]);
      }
    break;

    case 'volume':
      if (sound) {
        sound.volume = load[1];
      }
    break;

    case 'delete':
      if (sound) {
        soundStore[load[0]] = null;
      }
    break;
  }
});

const postStatus = (status) => {
  $.post(`http://${GetParentResourceName()}/zeaDevelopment:postStatus`, JSON.stringify(status));
};
 
const showUi = (status, reason) => {
  const element = document.getElementById('panel-container');
  const styleProperty = window.getComputedStyle(element).getPropertyValue('bottom');
  if (styleProperty === '-250px') {
    updatePanelUI(status, reason);
    element.style.bottom = '450px';
    postStatus('showing');
  }
};

const hideUi = () => {
  const element = document.getElementById('panel-container');
  const styleProperty = window.getComputedStyle(element).getPropertyValue('bottom');
  if (styleProperty === '450px') {
    element.style.bottom = '-250px';
    postStatus('hidden');
  }
};

const loadSound = (soundName, filename, volume = 0.0, loop = false) => {
  if (!soundStore[soundName]) {
    const audio = new Audio(`sounds/${filename}.mp3`);
    audio.volume = volume;
    audio.loop = loop;
    audio.play();
    soundStore[soundName] = audio;
  }
};

const playAudioLoop = (soundName) => {
  const audio = soundStore[soundName];
  if (audio) {
    audio.currentTime = 0.0;
    setTimeout(() => playAudioLoop(soundName), 500);
  }
};

document.onkeyup = (event) => {
  event = event || window.event;
  if (event.key === 'Escape') {
    hideUi();
  }
};

const months = [
  "January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
];
const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const formatDate = () => {
  const today = new Date();
  const dayName = days[today.getDay()];
  const day = today.getDate();
  const monthName = months[today.getMonth()];
  const year = today.getFullYear();
  return `${panelAddress} : ${dayName} ${day} ${monthName} ${year}`;
};

const statusIndicator = document.getElementById('status-indicator');
const statusText = document.getElementById('status-text');
const messageText = document.getElementById('message-text');
const activateButton = document.getElementById('activate');
const resetButton = document.getElementById('reset');

const updatePanelUI = (isAlarmSounding, reason) => {
  statusIndicator.style.backgroundColor = isAlarmSounding ? 'red' : 'green';
  statusIndicator.style.boxShadow = isAlarmSounding ? '0 0 10px rgba(255, 0, 0, 0.6)' : '0 0 10px rgba(0, 255, 0, 0.6)';
  statusText.textContent = isAlarmSounding ? 'Alarm Sounding' : 'Normal';
  messageText.textContent = isAlarmSounding ? reason : formatDate();
  activateButton.disabled = isAlarmSounding;
  resetButton.disabled = !isAlarmSounding;
};

const activateButtonClickHandler = () => {
  updatePanelUI(true, 'ALARM ACTIVATION: Alarm Panel');
  $.post(`http://${GetParentResourceName()}/zeaDevelopment:activateButton`, JSON.stringify(panelId));
};

const resetButtonClickHandler = () => {
  updatePanelUI(false, '');
  $.post(`http://${GetParentResourceName()}/zeaDevelopment:resetButton`, JSON.stringify(panelId));
};

activateButton.addEventListener('click', activateButtonClickHandler);
resetButton.addEventListener('click', resetButtonClickHandler);