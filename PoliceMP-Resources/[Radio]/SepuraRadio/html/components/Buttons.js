function CreateDisplayButtons(pPage){
    let buttons = ``;
    if(!PageButtons[pPage]) return;
    PageButtons[pPage].forEach((button, index) => {
        buttons += `<button class="border-0 text-center ${
          PageButtons[pPage].length === 3 && index === 1 ? "middle" : ""
        } ${PageButtons[pPage].length === 3 ? "w-100" : "small"} ${
          PageButtons[pPage].length !== 3 && index == 1 ? "ms-auto" : ""
        }">${button.label}</button>`;
    });
    $('#Buttons').html(buttons);
}

function SelectMenuButton(pButton){
  let SelectedValue = SelectedScreen !== 'Menu' ? $('.Selected').text() : $('.MenuSelected').attr('id');
  let buttonIndex = MenuSelectButtonKey[pButton];
  let onSelect;
  if(PageButtons[SelectedScreen].length === 3 || buttonIndex === 0) onSelect = PageButtons[SelectedScreen][buttonIndex]?.onSelect;
  else if(buttonIndex === 1) return;
  else if(buttonIndex === 2) onSelect =  PageButtons[SelectedScreen][1]?.onSelect;
  if(onSelect) onSelect(SelectedValue);
}

function SelectArrowButton(pButton){
  if(Screens[SelectedScreen].Type === "List"){

    let SelectedElement =  $('.Selected').attr('id');
    let SelectedElementInt = parseInt(SelectedElement.substring(SelectedElement.lastIndexOf("-") + 1));
    let AmoutOfElements =  $('.List-Items').find('h1').length;
    
    if(pButton === 'up'){
      if(SelectedElementInt - 1 < 0) return;
      $('#' + SelectedElement).removeClass('Selected');
      $(`#list-items-${SelectedElementInt - 1}`).addClass('Selected');
    }

    if(pButton === 'down'){
      if(SelectedElementInt + 1 > AmoutOfElements - 1) return; 
      $('#' + SelectedElement).removeClass('Selected');
      $(`#list-items-${SelectedElementInt + 1}`).addClass('Selected');
    }

  }

  else if($('#Menu').is(":visible")){
    let AmoutOfElements = 9;
    let SelectedElement = $('.MenuSelected').attr('id');
    let SelectedElementInt = parseInt(SelectedElement.substring(SelectedElement.lastIndexOf("-") + 1));

    if(pButton === 'left'){
      if(SelectedElementInt - 1 <= 0) return;
      $('#' + SelectedElement).removeClass('MenuSelected');
      $(`#Menu-${SelectedElementInt - 1}`).addClass('MenuSelected');
    }

    if(pButton === 'right'){
      if(SelectedElementInt + 1 > AmoutOfElements) return; 
      $('#' + SelectedElement).removeClass('MenuSelected');
      $(`#Menu-${SelectedElementInt + 1}`).addClass('MenuSelected');
    }

    if(pButton === 'up'){
      if(SelectedElementInt - 3 <= 0) return;
      $('#' + SelectedElement).removeClass('MenuSelected');
      $(`#Menu-${SelectedElementInt - 3}`).addClass('MenuSelected');
    }

    if(pButton === 'down'){
      if(SelectedElementInt + 3 > AmoutOfElements) return; 
      $('#' + SelectedElement).removeClass('MenuSelected');
      $(`#Menu-${SelectedElementInt + 3}`).addClass('MenuSelected');
    }

    let NewSelectedElementInt = parseInt( $('.MenuSelected').attr('id').substring( $('.MenuSelected').attr('id').lastIndexOf("-") + 1));
    $('#Menu-Select-Text').text(MenuItems[NewSelectedElementInt - 1]);

  }
}

function SelectMenuItem(pValue){
  let value = parseInt(pValue.substring(pValue.lastIndexOf("-") + 1));
  if(!Screens[MenuItems[value - 1]]) return;
  Screens[MenuItems[value - 1]].Build();
}

function SelectPhoneAnswer(){ // Audio stop playing & create call radio channel
  if(!isOnCall || isRequestingCall) return;
  $.post('http://' + ResourceName + '/AnswerCall');
}

function SelectPhoneReject(pDontPost){
  if(!isOnCall) return;

  isOnCall = false;
  isRequestingCall = false;

  Screens['Home'].Build();
  if(!pDontPost) $.post('http://' + ResourceName + '/EndCall');
  $.post('http://' + ResourceName + '/SelectTalkGroup', JSON.stringify({ SelectedTalkGroup: SelectedTalkGroup, SelectedFolder: SelectedFolder }));
  PlayAudio('')
}

function Minimise(){
  $('body').hide();
  $.post('http://' + ResourceName + '/Minimise');
}

function TogglePower() {
  Power = !Power;
  if(!Power){
    $.post('http://' + ResourceName + '/SelectTalkGroup', JSON.stringify({ SelectedTalkGroup: 0 }));
    $.post('http://' + ResourceName + '/TogglePower', JSON.stringify({ Power: false }));
    return Screens['Off'].Build();
  }
  $('#Screen').show();
  Screens['Home'].Build();
  $.post('http://' + ResourceName + '/TogglePower', JSON.stringify({ Power: true }));
  
  PlayAudio('SingleBleep')

  if(SelectedTalkGroup === undefined){
    TempSelectedFolder = Config.InitalFolder;
    SelectListItem(Config.InitalTalkGroup, 'FinishTG')
  }else{
    SelectListItem(SelectedTalkGroup, 'FinishTG')
  }
}


let holdTimer
$('.red-phone').mousedown(function () {
  holdTimer = setTimeout(TogglePower, 500);
}).mouseup(function () {
  clearTimeout(holdTimer);
});