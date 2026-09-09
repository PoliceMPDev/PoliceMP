const ui = {
   activeSound: null,
   panelType : '',

   sound: function(filename, volume = 0.5) {
      if (this.activeSound) {
         this.activeSound.pause();
      }
      const audio = new Audio(`sounds/${filename}`);
      audio.volume = volume;
      audio.play();

      this.activeSound = audio;
   },

   post: function(type, data) {
      const allowedTypes = ['command', 'unfocus'];
      if (!allowedTypes.includes(type)) {
         console.log('Invalid POST type:', type);
         return;
      }
      
      fetch(`https://${GetParentResourceName()}/${type}`, {
         method: 'POST',
         headers: {
            'Content-Type': 'application/json; charset=UTF-8',
         },
         body: JSON.stringify({ data }),
      })
         .then((resp) => {
            if (!resp.ok) {
               throw new Error(`HTTP error! Status: ${resp.status}`);
            }
            return resp.json();
         })
         .catch((error) => {
            console.error('Error in POST request:', error);
         });
   },
};

$(document).ready(function () {
   $('button').on('click', function () {
      const command = $(this).data('command');
      ui.post('command', command);
   });

   window.addEventListener('message', ({ data }) => {
      const { type, load } = data;

      switch (type) {
         case 'play-sound':
            ui.sound(load.fileName, load.volume || 0.5);
            break;

         case 'show-controller':
            $(`#${load.type}`).fadeIn();
            ui.panelType = load.type
            break;

         case 'hide-controller':
            $(`#${ui.panelType}`).fadeOut();
            break;

         case 'toggle-controller':
            $(`#${load.type}`).each(function () {
               $(this).css('display') === 'none' ? $(this).fadeIn() : $(this).fadeOut();
            });
            break;

         case 'update-controller':
            const { types } = data;
            for (const key in types) {
               if (Object.hasOwnProperty.call(types, key)) {
                  $(`button[data-command="${key}"]`).css('background-color', types[key]);
               }
            }
            break;
      }
   });

   $(document).keydown(function (event) {
      if (event.keyCode === 27) {
         event.preventDefault();
         ui.post('unfocus');
      }
   });
});

function dragMoveListener(event) {
   const target = event.target;
   const x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
   const y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

   window.requestAnimationFrame(() => {
      target.style.position = 'relative';
      target.style.transform = `translate(${x}px, ${y}px)`;
      target.setAttribute('data-x', x);
      target.setAttribute('data-y', y);
   });
}

document.addEventListener(
   'DOMContentLoaded',
   () => {
      interact('.resize-drag').draggable({
         listeners: { move: dragMoveListener },
         inertia: true,
         modifiers: [
            interact.modifiers.restrictRect({
               restriction: 'parent',
               endOnly: true,
            }),
         ],
         ignoreFrom: '#menu-close',
      });
   },
   false
);
