let isRadioMoving = false;
let ShowSpeaker = false;
let RadioVolume = 30;
let RadioScale = 1

function SelectSettings(pSetting){
  $('.Settings-Items').css('opacity', '100%')
  $('.Settings-Items').css('cursor', 'auto')
  $('.Settings-Items').removeClass('pos-moving')

  if(pSetting === 'Radio Volume'){
    $('.Settings-Items').html(`
        <label for="Radio-Volume" class="form-label d-flex justify-content-center"><h1>Radio Volume (<span id="Radio-Volume-Value">${RadioVolume}%</span>)</h1></label>
        <input type="range" class="form-range" id="Radio-Volume" value="${RadioVolume}" onchange="ChangeRadioVolume(this.value)">
    `)
    $('#Settings').show();
  }
  if(pSetting === 'Show Speaker'){
    ShowSpeaker = !ShowSpeaker;
    Screens['Settings'].Build();
  }
  if(pSetting === 'Radio Scale'){
    $('.Settings-Items').html(`
        <label for="Radio-Scale" class="form-label d-flex justify-content-center"><h1>Radio Scale (<span id="Radio-Scale-Value">${RadioScale * 100}%</span>)</h1></label>
        <input type="range" class="form-range" id="Radio-Scale" value="${RadioScale * 100}" onchange="ChangeRadioScale(this.value)" min="10" max="200">
    `)
    $('#Settings').show();
  }
  if(pSetting === 'Radio Position'){
    isRadioMoving = true;
    $('.Settings-Items').addClass('pos-moving')
    $('.Settings-Items').css('opacity', '50%')
    $('.Settings-Items').css('cursor', 'move')
    $('.Settings-Items').css('user-select', 'none')
    $('.Settings-Items').html(`<h1>Drag the radio to your desired position.</h1>`)
    $('#Settings').show();
    dragElement(document.getElementById('Radio'))
  }
}

function ChangeRadioVolume(pVolume){
  RadioVolume = pVolume
  $('#Radio-Volume-Value').text(pVolume + '%')
  PlayAudio('SingleBleep')
}

function ChangeRadioScale(pScale){
  RadioScale = pScale / 100
  $('.Radio').css('transform', `scale(${RadioScale})`);
  $('#Radio-Scale-Value').text(pScale + '%')
}

function dragElement(elmnt) {
  let pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
  if (document.getElementById(elmnt.id)) {
    document.getElementById(elmnt.id).onmousedown = dragMouseDown;
  } else {
    elmnt.onmousedown = dragMouseDown;
  }

  function dragMouseDown(e) {
    if(!$('.pos-moving').is(':visible')) return closeDragElement();
    e = e || window.event;
    e.preventDefault();
    pos3 = e.clientX;
    pos4 = e.clientY;
    document.onmouseup = closeDragElement;
    document.onmousemove = elementDrag;
  }

  function elementDrag(e) {
    if(!$('.pos-moving').is(':visible')) return closeDragElement();
    e = e || window.event;
    e.preventDefault();
    pos1 = pos3 - e.clientX;
    pos2 = pos4 - e.clientY;
    pos3 = e.clientX;
    pos4 = e.clientY;
    // set the element's new position:
    elmnt.style.top = elmnt.offsetTop - pos2 + "px";
    elmnt.style.left = elmnt.offsetLeft - pos1 + "px";
  }

  function closeDragElement() {
    isRadioMoving = false;
    document.onmouseup = null;
    document.onmousemove = null;
  }

}