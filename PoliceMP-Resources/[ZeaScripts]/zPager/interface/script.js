const soundStore = {};
const getElementById = (id) => document.getElementById(id);
const getComputedStyleProperty = (element, property) => getComputedStyle(element)[property];

window.addEventListener('message', async ({ data }) => {
   const { type, load } = data;
   switch (type) {
      case 'ui': 
         showUI()
      break;

      case 'alarm':
         if (!soundStore[load[0]]) {
            loadSound([load[0]], 'alarm.mp3', 0.0, false)
            soundStore[load[0]].addEventListener('ended', () => {
               $.post(`https://${GetParentResourceName()}/zeaDevelopment:deleteSound`, JSON.stringify(load[0]));
               removeUI()
            });
         } else {
            break;
         }
      break;

      case 'setVolume':
         if (soundStore[load[0]]) {
            soundStore[load[0]].volume = load[1];
         } else {
            break;
         }
      break;
   
      case 'deleteSound':
         if (soundStore[load[0]]) {
            soundStore[load[0]].pause();
            soundStore[load[0]] = null;
         } else {
            break;
         }
      break;
   }
});

const postStatus = (status) => {
  $.post(`http://${GetParentResourceName()}/zeaDevelopment:postStatus`, JSON.stringify(status));
};

const showUI = () => {
   const element = getElementById('uiContainer');
   const styleProperty = getComputedStyleProperty(element, 'bottom');
   if (styleProperty === '-250px') {
      element.style.bottom = '0px';
      postStatus('showing');
   }
};

const removeUI = () => {
   const element = getElementById('uiContainer');
   const styleProperty = getComputedStyleProperty(element, 'bottom');
   if (styleProperty === '0px') {
      element.style.bottom = '-250px';
      postStatus('hidden');
   } 
};

const loadSound = (soundName, filename, volume = 0.0, loop = false) => {
   if (!soundStore[soundName]) {
     soundStore[soundName] = new Audio(`sounds/${filename}`);
     soundStore[soundName].volume = volume;
     soundStore[soundName].loop = loop;
     soundStore[soundName].play();
   }
};