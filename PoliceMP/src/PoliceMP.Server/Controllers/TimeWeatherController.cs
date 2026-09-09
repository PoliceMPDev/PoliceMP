using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Models.Weather;

namespace PoliceMP.Server.Controllers
{
    public class TimeWeatherController : Controller
    {
        #region Services

        private readonly ILogger<TimeWeatherController> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notification;
        private PlayerList _playerList;

        #endregion Services

        #region Variables

        private WeatherType _currentWeatherType = WeatherType.CLEAR;
        private DateTime _weatherOverrideTime = DateTime.MinValue;

        private readonly Timer _weatherCheckTimer = new Timer(60000)
        {
            AutoReset = true,
            Enabled = true
        };

        private Timer _timeTimer;

        private int _currentHour = 12;
        private int _currentMinute = 00;

        #endregion Variables

        public TimeWeatherController(ILogger<TimeWeatherController> logger, ILegacyServerCommunicationsManager comms, INotificationService notification, PlayerList playerList)
        {
            _logger = logger;
            _comms = comms;
            _notification = notification;
            _playerList = playerList;

            _weatherCheckTimer.Elapsed += WeatherCheckTimerOnElapsed;
        }

        private async void WeatherCheckTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_weatherOverrideTime == DateTime.MinValue) return;

                _logger.Trace($"Override Time: {_weatherOverrideTime}");

                if (DateTime.Compare(DateTime.Now, _weatherOverrideTime) < 0)
                {
                    _logger.Trace("Weather is being overriden!");
                    return;
                }

                _weatherOverrideTime = DateTime.MinValue;
            }
            catch (Exception ex)
            {
                _logger.Error("Weather", ex);
            }
        }

        public override async Task Started()
        {
            _comms.OnRequest(ServerEvents.FetchWeatherFromServer, OnReceiveWeatherRequest);

            var weatherTimer = new Timer(900000*4) //Every hour 
            {
                AutoReset = true,
                Enabled = true
            };

            weatherTimer.Elapsed += async (sender, args) =>
            {
                await WeatherTimerOnElapsed(sender, args);
            };

            _timeTimer = new Timer(30000)
            {
                AutoReset = true,
                Enabled = true
            };

            _timeTimer.Elapsed += TimeTimerOnElapsed;

            _comms.On<WeatherType>(ServerEvents.ForceSetWeatherToServer, OnForceSetWeatherFromClient);
            _comms.On<string, string, string>(ServerEvents.AdminManageServerTime, OnReceiveManageServerTime);
            _comms.OnRequest(ServerEvents.FetchServerTime, (player) =>
            {
                List<int> timeList = new List<int>
                {
                    _currentHour,
                    _currentMinute
                };
                return Task.FromResult(JsonConvert.SerializeObject(timeList));
            });
        }

        //Every hour change weather
        private async Task WeatherTimerOnElapsed(object sender, ElapsedEventArgs args)
        {
            Random r = new Random();
            switch (r.Next(3))
            {
                case 1:
                    _currentWeatherType = WeatherType.CLEAR;
                    break;
                case 2:
                    _currentWeatherType = WeatherType.RAINLIGHT;
                    break;
                case 3:
                    _currentWeatherType = WeatherType.EXTRASUNNY;
                    break;
                case 4:
                    _currentWeatherType = WeatherType.THUNDER;
                    break;
                case 5:
                    _currentWeatherType = WeatherType.CLEAR;
                    break;
                case 6:
                    _currentWeatherType = WeatherType.FOGGY;
                    break;
                case 7:
                    _currentWeatherType = WeatherType.EXTRASUNNY;   
                    break;
                case 8:
                    _currentWeatherType = WeatherType.CLEAR;
                    break;
                case 9:
                    _currentWeatherType = WeatherType.EXTRASUNNY;
                    break;
                default:
                    _currentWeatherType = WeatherType.CLEAR;
                    break;
                case 10:
                    _currentWeatherType = WeatherType.SMOG;
                    break;
                case 11:
                    _currentWeatherType = WeatherType.CLEAR;
                    break;
                
            }
            _weatherOverrideTime = DateTime.Now.AddHours(1);
            _logger.Debug($"Setting time to {_currentWeatherType}");
            _comms.ToClient(ClientEvents.SendWeatherToPlayers, _currentWeatherType);
        }

        private void OnReceiveManageServerTime(Player player, string itemData, string hourString, string minuteString)
        {
            var hour = int.Parse(hourString);
            var minute = int.Parse(minuteString);

            switch (itemData)
            {
                case "RESETTIME":
                    _timeTimer.Enabled = true;
                    TriggerEvent("txaLogger:CommandExecuted", $"[TIME/WEATHER] {player.Name} has unfrozen time!");
                    return;

                case "FREEZETIME":
                    _timeTimer.Enabled = false;
                    TriggerEvent("txaLogger:CommandExecuted", $"[TIME/WEATHER] {player.Name} has frozen time!");
                    return;

                case "SETTIME":
                    _currentHour = hour;
                    _currentMinute = minute;
                    _comms.ToClient(ServerEvents.ForcePlayerTime, _currentHour, _currentMinute);
                    TriggerEvent("txaLogger:CommandExecuted", $"[TIME/WEATHER] {player.Name} has set time to {hour}:{minute}!");
                    return;
            }
        }

        private void TimeTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _currentMinute++;
            if (_currentMinute >= 59)
            {
                _currentMinute = 0;
                _currentHour++;
                if (_currentHour >= 24)
                {
                    _currentHour = 0;
                }
            }
            //_logger.Debug($"Setting time to {_currentHour}:{_currentMinute}");

            _comms.ToClient(ServerEvents.ForcePlayerTime, _currentHour, _currentMinute);
        }

        private async Task OnForceSetWeatherFromClient(Player player, WeatherType weatherType)
        {
            _weatherOverrideTime = DateTime.Now.AddHours(1);
            _currentWeatherType = weatherType;
            _logger.Debug($"{player.Name} force set the weather to {_currentWeatherType}");
            TriggerEvent("txaLogger:CommandExecuted", $"[TIME/WEATHER] {player.Name} has set weather to {_currentWeatherType}!");
            _comms.ToClient(ClientEvents.SendWeatherToPlayers, _currentWeatherType);
        }

        private Task<WeatherType> OnReceiveWeatherRequest(Player player)
        {
            return Task.FromResult(_currentWeatherType);
        }
    }
}