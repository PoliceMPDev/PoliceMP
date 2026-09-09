// ** Toggle Pump ** //

function TogglePump() {
  $('.Panel-Body-Button_L_Circle_PumpToggle').css(
    'border',
    '2px solid #00ff2a',
  );
  setTimeout(
    () =>
      $('.Panel-Body-Button_L_Circle_PumpToggle').css(
        'border',
        '2px solid #FF4D00',
      ),
    450,
  );
  $.post(`https://zPump/Pump-Toggle`);
}

// ** Switch RPM ** //

function SwitchRPM(string) {
  if (string == 'Increase') {
    $('.Panel-Body-Button_R_Circle_RPMUP').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_R_Circle_RPMUP').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/Pump-RPM-Increase`);
  } else if (string == 'Decrease') {
    $('.Panel-Body-Button_R_Circle_RPMDOWN').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_R_Circle_RPMDOWN').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/Pump-RPM-Decrease`);
  } else if (string == 'Idle') {
    $('.Panel-Body-Button_R_Circle_RPMIDLE').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_R_Circle_RPMIDLE').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/Pump-RPM-Idle`);
  }
}

// ** Switch Pressure ** //

function SwitchPressure(string) {
  if (string == '30') {
    $('.Panel-Body-Button_L_Circle_30BAR').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_L_Circle_30BAR').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/30BAR-HP`);
  } else if (string == '7.0') {
    $('.Panel-Body-Button_L_Circle_7BAR').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_L_Circle_7BAR').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/7BAR-LP`);
  } else if (string == '5.5') {
    $('.Panel-Body-Button_L_Circle_55BAR').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_L_Circle_55BAR').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/5.5BAR-LP`);
  } else if (string == '3.0') {
    $('.Panel-Body-Button_L_Circle_3BAR').css('border', '2px solid #00ff2a');
    setTimeout(
      () =>
        $('.Panel-Body-Button_L_Circle_3BAR').css(
          'border',
          '2px solid #FF4D00',
        ),
      450,
    );
    $.post(`https://zPump/3BAR-LP`);
  }
}

// ** NUI Listener ** //

window.addEventListener('message', function (event) {
  if (event.data.type == 'Display-NUI') {
    if (event.data.Arg == 'true') {
      $('#Panel-Container').css('display', 'block');
    } else {
      $('#Panel-Container').css('display', 'none');
      $('#Screen-Ingition-On').css('display', 'none');
    }
  } else if (event.data.type == 'Display-NUI-Contents') {
    if (event.data.Arg == 'Ing-On') {
      $('#Screen-Ingition-Off').css('display', 'none');
      $('#Screen-Ingition-On').css('display', 'block');
    } else {
      $('#Screen-Ingition-On').css('display', 'none');
      $('#Screen-Ingition-Off').css('display', 'block');
    }
  } else if (event.data.type == 'Display-Pump-Data') {
    if (event.data.Arg == 'Pump-On') {
      $('.Inner_Button_L_1').css('border', '3px solid #15ff00');
      $('#Pump-On-Body').css('display', 'block');
      $('#PumpText').css('display', 'none');
    } else if (event.data.Arg == 'Pump-Off') {
      $('.Inner_Button_L_1').css('border', '3px solid #0099ff');
      $('#Pump-On-Body').css('display', 'none');
      $('#PumpText').css('display', 'block');
      ResetInnerButtons();
      document.getElementById('REQSTPRESS').innerHTML =
        'Requesting 0.0 Bar LOW Pressure';
      $('#LPGuage').attr('data-value', parseFloat(0));
      $('#HPGuage').attr('data-value', parseFloat(0));
      document.getElementById('LPValue').innerHTML = '0.0';
      document.getElementById('HPValue').innerHTML = '0';
    }
  } else if (event.data.type == 'Set-Pressure') {
    if (event.data.Press == 0) {
      ResetInnerButtons();
      $('#LPGuage').attr('data-value', parseFloat(0));
      $('#HPGuage').attr('data-value', parseFloat(event.data.Press));
      document.getElementById('REQSTPRESS').innerHTML = event.data.Text;
    } else if (event.data.Press == 3) {
      ResetInnerButtons();
      $('.Inner_Button_L_2').css('background-color', '#0051ff');
      $('.Inner_Button_L_2').css('border', '3px solid #0051ff');
      $('#HPGuage').attr('data-value', 0);
      $('#LPGuage').attr('data-value', parseFloat(event.data.Press));
      document.getElementById('REQSTPRESS').innerHTML = event.data.Text;
      document.getElementById('LPValue').innerHTML = '3.0';
      document.getElementById('HPValue').innerHTML = '0';
    } else if (event.data.Press == 5.5) {
      ResetInnerButtons();
      $('.Inner_Button_L_3').css('background-color', '#0051ff');
      $('.Inner_Button_L_3').css('border', '3px solid #0051ff');
      $('#HPGuage').attr('data-value', parseFloat(0));
      $('#LPGuage').attr('data-value', parseFloat(event.data.Press));
      document.getElementById('REQSTPRESS').innerHTML = event.data.Text;
      document.getElementById('LPValue').innerHTML = '5.5';
      document.getElementById('HPValue').innerHTML = '0';
    } else if (event.data.Press == 7) {
      ResetInnerButtons();
      $('.Inner_Button_L_4').css('background-color', '#0051ff');
      $('.Inner_Button_L_4').css('border', '3px solid #0051ff');
      $('#HPGuage').attr('data-value', parseFloat(0));
      $('#LPGuage').attr('data-value', parseFloat(event.data.Press));
      document.getElementById('REQSTPRESS').innerHTML = event.data.Text;
      document.getElementById('LPValue').innerHTML = '7.0';
      document.getElementById('HPValue').innerHTML = '0';
    } else if (event.data.Press == 30) {
      ResetInnerButtons();
      $('.Inner_Button_L_5').css('background-color', '#0051ff');
      $('.Inner_Button_L_5').css('border', '3px solid #0051ff');
      $('#LPGuage').attr('data-value', parseFloat(0));
      $('#HPGuage').attr('data-value', parseFloat(event.data.Press));
      document.getElementById('REQSTPRESS').innerHTML = event.data.Text;

      document.getElementById('LPValue').innerHTML = '0.0';
      document.getElementById('HPValue').innerHTML = '30';
    }
  }
});

// ** Close NUI ( ESC ) ** //

document.onkeyup = function (event) {
  event = event || window.event;
  var EnteredKey = event.keyCode || event.which;
  if (EnteredKey == 27) {
    $.post(`https://zPump/Cancel-NUI`);
  }
};

// ** Reset Interior Buttons ** //

function ResetInnerButtons() {
  $('.Inner_Button_L_2').css('background-color', 'transparent');
  $('.Inner_Button_L_2').css('border', '3px solid #0099ff');
  $('.Inner_Button_L_3').css('background-color', 'transparent');
  $('.Inner_Button_L_3').css('border', '3px solid #0099ff');
  $('.Inner_Button_L_4').css('background-color', 'transparent');
  $('.Inner_Button_L_4').css('border', '3px solid #0099ff');
  $('.Inner_Button_L_5').css('background-color', 'transparent');
  $('.Inner_Button_L_5').css('border', '3px solid #0099ff');
}
