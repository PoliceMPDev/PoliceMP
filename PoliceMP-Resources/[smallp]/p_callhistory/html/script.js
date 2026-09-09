// script.js

window.addEventListener('message', function (event) {
  const data = event.data;

  if (data.action === 'show') {
    document.getElementById('main-container').style.display = 'flex';
    updateLeft(data.left);
    updateRight(data.right);
    setActiveTab(data.tab || 'CCTV');
  }

  if (data.action === 'hide') {
    document.getElementById('main-container').style.display = 'none';
  }

  if (data.action === 'updateLeft') {
    updateLeft(data.message);
  }

  if (data.action === 'updateRight') {
    updateRight(data.message);
  }
});

function updateLeft(msg) {
  document.getElementById('leftMessage').textContent = msg;
}

function updateRight(msg) {
  document.getElementById('rightMessage').textContent = msg;
}

function navigateLeft(dir) {
  fetch(`https://${GetParentResourceName()}/navigateLeft`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ direction: dir })
  });
}

function navigateRight(dir) {
  fetch(`https://${GetParentResourceName()}/navigateRight`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ direction: dir })
  });
}

function switchLeftTab(tab) {
  setActiveTab(tab);
  fetch(`https://${GetParentResourceName()}/tabLeft`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ tab })
  });
}

function closeUI() {
  fetch(`https://${GetParentResourceName()}/close`, {
    method: 'POST' });
  document.getElementById('main-container').style.display = 'none';
}

function setActiveTab(tab) {
  document.getElementById('tabCCTV').classList.remove('active');
  document.getElementById('tabANPR').classList.remove('active');
  document.getElementById(`tab${tab}`).classList.add('active');
}
