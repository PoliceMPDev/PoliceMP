const soundStore = {};
const getElementById = (id) => document.getElementById(id);
const getComputedStyleProperty = (element, property) => getComputedStyle(element)[property];

window.addEventListener('message', async ({ data }) => {
  const { type, load } = data;

  switch (type) {
    case 'play':
      if (!soundStore[load[0]]) {
        loadSound(load[0], load[1][0], 0.0, false);
        soundStore[load[0]].addEventListener('ended', () => {
          $.post(`https://zTurnout-System/Turnout-System:postSound`, JSON.stringify([load[0], load[1][0]]));
        });
      }
    break;

    case 'incidentForm':
      toggleUi('form', true);
    break;

    case 'tipSheet':
      tipSheet(load[0], load[1], load[2])
    break;

    case 'postForm':
      postForm(load)
    break;

    case 'volume':
      if (soundStore[load[0]]) {
        soundStore[load[0]].volume = load[1];
      }
    break;

    case 'delete':
      if (soundStore[load[0]]) {
        soundStore[load[0]] = null;
      }
    break;
  }
});

const toggleUi = (type, show) => {
  if (type === 'form') {
    this.element = document.getElementById('form-container');
    this.styleProperty = window.getComputedStyle(element).getPropertyValue('bottom');
    this.shouldShow = show ? styleProperty === '-1050px' : styleProperty === '150px';

    if (shouldShow) {
      element.style.bottom = show ? '150px' : '-1050px';
      element.style.visibility = show ? 'visible' : 'hidden';
    }
    createPost('zTurnout-System', 'Turnout-System:postStatus', show ? 'showing' : 'hidden');
  } else if (type === 'tipsheet') {
    this.element = document.getElementById('tipsheet-container');
    this.styleProperty = window.getComputedStyle(element).getPropertyValue('bottom');
    this.shouldShow = show ? styleProperty === '-1050px' : styleProperty === '-20px';

    if (shouldShow) {
      element.style.bottom = show ? '-20px' : '-1050px';
      element.style.visibility = show ? 'visible' : 'hidden';
    }
    createPost('zTurnout-System', 'Turnout-System:postStatus', show ? 'showing' : 'hidden');
  }
};

document.onkeyup = (event) => {
  event = event || window.event;
  if (event.key === 'Escape') {
    toggleUi('form', false);
    toggleUi('tipsheet', false);
  }
};

document.addEventListener("DOMContentLoaded", function () {
  const incidentForm = document.getElementById("incident-form");
  const cancelButton = document.getElementById("cancel-button");

  incidentForm.addEventListener("submit", function (event) {
    const formData = {
      array: getFieldValue("mobilise").split(/,\s*/),
      type: getFieldValue("incident-type"),
      address: getFieldValue("incident-address"),
      details: getFieldValue("incident-details"),
      talkgroup: getFieldValue("talkgroup"),
      timedate: formatDate(),
      mapref: getFieldValue("map-ref"),
      inum: `#${generateNumber(1, 9999)}`,
      tom: fullDateTime(),
      stype: 'form'
    };

    toggleUi('form', false)
    createPost('zTurnout-System', 'Turnout-System:postForm', formData)
    incidentForm.reset();
  });
  cancelButton.addEventListener("click", function () {
    toggleUi('form', false)
    incidentForm.reset();
  });
});

const tipSheet = (form, mtype) => {
  $('#tipsheet-container').empty();
  const { tom, mobilise, type, address, details, inum, timedate, talkgroup, mapref } = form

  let element = $(`
    <b>**********************************************</b><br>
    <b>MOBILISATION MESSAGE at ${tom}</b><br>
    <b>Callsigns: ${mobilise}</b><br>
    <b>Incident Type: ${type}</b><br>
    <b>*****************************************************************</b><br>
    <b>MOBILISE MOBILISE MOBILISE</b><br>
    <br>
    <b>Incident Address: ${address}</b><br>
    <br>
    <b>Incident Details: ${details}</b><br>
    <br>
    <br>
    <b>Incident Number: ${inum}</b><br>
    <b>Call: Time Of Call: ${timedate}</b><br>
    <br>
    <b>Talkgroup: ${talkgroup}</b><br>
    <br>
    <b>Mapref: ${mapref}</b><br>
    <br>
    <b>END OF MESSAGE</b><br>
    <b>*****************************************************************</b><br>
  `);
  $('#tipsheet-container').append(element);
  toggleUi('tipsheet', true);

  if ( mtype === 'auto' ) {
    setTimeout(() => {
      toggleUi('tipsheet', false);
    }, "5000");
  }
}

const postForm = (form) => {
  const formData = {
    inum: form.inum,
    array: form.mobilise.split(/,\s*/),
    type: form.type,
    address: form.address,
    details: form.details,
    talkgroup: form.talkgroup,
    timedate: form.timedate,
    tom: formatDate(),
    mapref: form.mapref,
    tom: fullDateTime(),
    stype: form.stype
  };

  createPost('zTurnout-System', 'Turnout-System:postForm', formData)
};

const formatDate = () => {
  const today = new Date();
  const day = today.getDate();
  const month = today.getMonth() + 1;
  const year = today.getFullYear();
  return `${formatTime(today)} ${day}/${month}/${year}`;
};

const formatTime = (date) => {
  const h = addZero(date.getHours());
  const m = addZero(date.getMinutes());
  return `${h}:${m}`;
};

const addZero = (i) => {
  return i < 10 ? `0${i}` : `${i}`;
};

const getFieldValue = (elementId) => {
  return document.getElementById(elementId).value;
};

const generateNumber = (min, max) => {
  return Math.floor(Math.random() * (max - min)) + min;
}

const createPost = (resource, to, data) => {
  $.post(`http://${resource}/${to}`, JSON.stringify(data));
};

const fullDateTime = () => {
  const months = [
    'Jan', 'Feb', 'Mar', 'Apr',
    'May', 'Jun', 'Jul', 'Aug',
    'Sep', 'Oct', 'Nov', 'Dec'
  ];

  const now = new Date();
  const day = now.getDate().toString().padStart(2, '0');
  const month = months[now.getMonth()];
  const year = now.getFullYear().toString().slice(-2);
  const hours = now.getHours().toString().padStart(2, '0');
  const minutes = now.getMinutes().toString().padStart(2, '0');
  const seconds = now.getSeconds().toString().padStart(2, '0');

  return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
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