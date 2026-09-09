using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.Weather
{
    public class WeatherScript : Script
    {
        #region Services

        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
        private readonly ILogger<WeatherScript> _logger;

        #endregion

        #region Variables

        private WeatherType _currentWeatherType = WeatherType.CLEAR;

        #endregion

        public WeatherScript(ILegacyClientCommunicationsManager comms, ILogger<WeatherScript> logger, ITickManager ticks)
        {
            _comms = comms;
            _logger = logger;
            _ticks = ticks;
            _comms.On<int, int>(ServerEvents.ForcePlayerTime, (hour, minute) =>
            {
                API.NetworkOverrideClockTime(hour, minute, 0);
            });

            _comms.On<bool>(ClientEvents.PlayerSpawned, async (firstSpawn) =>
            {
                var currentWeatherType = await _comms.Request<WeatherType>(ServerEvents.FetchWeatherFromServer);
                OnReceiveWeatherFromServer(currentWeatherType);
            });

            _comms.On<WeatherType>(ClientEvents.SendWeatherToPlayers, OnReceiveWeatherFromServer);
        }

        protected override async Task OnStartAsync()
        {
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;

            if (month == 12 && day >= 15)
            {
                OnReceiveWeatherFromServer(_currentWeatherType);
                return;
            }
            
            var currentWeather = await _comms.Request<WeatherType>(ServerEvents.FetchWeatherFromServer);
            OnReceiveWeatherFromServer(currentWeather);
            
            SetTime();
        }

        private async Task SetTime()
        {
            var timeJson = await _comms.Request<string>(ServerEvents.FetchServerTime);
            List<int> timeList = JsonConvert.DeserializeObject<List<int>>(timeJson);
            var hour = timeList[0];
            var minute = timeList[1];
            API.NetworkOverrideClockTime(hour, minute, 0);
        }

        private void OnReceiveWeatherFromServer(WeatherType weatherType)
        {
            _currentWeatherType = weatherType;
            
            _logger.Debug($"Setting the weather to {_currentWeatherType}");
            switch (_currentWeatherType)
            {
                // timecycle_keyframe_data BLIZZARD = 0x27EA2814 CLEAR = 0x36A83D84 CLEARING = 0x6DB1A50D CLOUDS = 0x30FDAF5C EXTRASUNNY = 0x97AA0A79 FOGGY = 0xAE737644 HALLOWEEN = 0xC91A3202 NEUTRAL = 0xA4CA1326 OVERCAST = 0xBB898D2D RAIN = 0x54A69840 SMOG = 0x10DCF4B5 SNOW = 0xEFB6EFF6 SNOWLIGHT = 0x23FB812B THUNDER = 0xB677829F XMAS = 0xAAC9C895
                case WeatherType.CLEAR:
                    API.SetWeatherTypeNow("CLEAR");
                    API.SetOverrideWeather("CLEAR");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0f);
                    API.WaterOverrideSetStrength(0f);
                    API.WaterOverrideSetRipplebumpiness(0f);
                    API.WaterOverrideSetRippledisturb(0f);
                    API.WaterOverrideSetShorewavemaxamplitude(0.1f);
                    API.WaterOverrideSetShorewaveminamplitude(0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.EXTRASUNNY:
                    API.SetWeatherTypeNow("EXTRASUNNY");
                    API.SetOverrideWeather("EXTRASUNNY");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0f);
                    API.WaterOverrideSetStrength(0f);
                    API.WaterOverrideSetRipplebumpiness(0f);
                    API.WaterOverrideSetRippledisturb(0f);
                    API.WaterOverrideSetShorewavemaxamplitude(0.1f);
                    API.WaterOverrideSetShorewaveminamplitude(0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.RAINLIGHT:
                    API.SetWeatherTypeNow("RAIN");
                    API.SetOverrideWeather("RAIN");
                    API.SetRainLevel(0.4f);
                    API.SetRainFxIntensity(0.4f);
                    API.SetSnowLevel(0f);
                    API.WaterOverrideSetStrength(1.0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.RAINHEAVY:
                    API.SetWeatherTypeNow("RAIN");
                    API.SetOverrideWeather("RAIN");
                    API.SetRainLevel(1f);
                    API.SetRainFxIntensity(1f);
                    API.SetSnowLevel(0f);
                    API.WaterOverrideSetStrength(1.5f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.THUNDER:
                    API.SetWeatherTypeNow("THUNDER");
                    API.SetOverrideWeather("THUNDER");
                    API.SetRainLevel(1f);
                    API.SetRainFxIntensity(1f);
                    API.SetSnowLevel(0f);
                    API.WaterOverrideSetStrength(2.0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.BLIZZARD:
                    API.SetWeatherTypeNow("BLIZZARD");
                    API.SetOverrideWeather("BLIZZARD");
                    API.SetRainLevel(1f);
                    API.SetRainFxIntensity(1f);
                    API.SetSnowLevel(10f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.HALLOWEEN:
                    API.SetWeatherTypeNow("HALLOWEEN");
                    API.SetOverrideWeather("HALLOWEEN");
                    API.SetSnowLevel(0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.SNOW:
                    API.SetWeatherTypeNow("SNOW");
                    API.SetOverrideWeather("SNOW");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(5f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.SNOWLIGHT:
                    API.SetWeatherTypeNow("SNOWLIGHT");
                    API.SetOverrideWeather("SNOWLIGHT");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(1f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.FOGGY:
                    API.SetWeatherTypeNow("FOGGY");
                    API.SetOverrideWeather("FOGGY");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.XMAS:
                    API.SetWeatherTypeNow("XMAS");
                    API.SetOverrideWeather("XMAS");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0.5f);
                    API.SetForcePedFootstepsTracks(true);
                    API.SetForceVehicleTrails(true);
                    API.SetBlackout(false);
                    break;
                case WeatherType.SNOW_BLACK_ICE:
                    API.SetWeatherTypeNow("XMAS");
                    API.SetOverrideWeather("XMAS");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(1.8f);
                    API.SetForcePedFootstepsTracks(true);
                    API.SetForceVehicleTrails(true);
                    API.SetBlackout(false);
                    break;
                case WeatherType.SMOG:
                    API.SetWeatherTypeNow("SMOG");
                    API.SetOverrideWeather("SMOG");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0f);
                    API.SetBlackout(false);
                    break;
                case WeatherType.BLACKOUTHALLOWEEN:
                    API.SetWeatherTypeNow("HALLOWEEN");
                    API.SetOverrideWeather("HALLOWEEN");
                    API.ClearCloudHat();
                    // API.SetRainLevel(-1f);
                    // API.SetRainFxIntensity(-1f);
                    API.SetRainLevel(0f); //test woody
                    API.SetRainFxIntensity(0f); //test woody
                    API.SetSnowLevel(0f);
                    API.SetBlackout(true);
                    API.SetArtificialLightsStateAffectsVehicles(false);
                    break;
                case WeatherType.BLACKOUTRAIN:
                    API.SetWeatherTypeNow("RAIN");
                    API.SetOverrideWeather("RAIN");
                    API.ClearCloudHat();
                    API.SetRainLevel(0.5f);
                    API.SetRainFxIntensity(0.5f);
                    API.SetSnowLevel(0f);
                    API.SetBlackout(true);
                    API.SetArtificialLightsStateAffectsVehicles(false);
                    break;
                case WeatherType.BLACKOUTTHUNDER:
                    API.SetWeatherTypeNow("THUNDER");
                    API.SetOverrideWeather("THUNDER");
                    API.ClearCloudHat();
                    API.SetRainLevel(1f);
                    API.SetRainFxIntensity(1f);
                    API.SetSnowLevel(0f);
                    API.SetBlackout(true);
                    API.SetArtificialLightsStateAffectsVehicles(false);
                    break;
                case WeatherType.BLACKOUTCLEAR:
                    API.SetWeatherTypeNow("CLEAR");
                    API.SetOverrideWeather("CLEAR");
                    API.ClearCloudHat();
                    API.SetRainLevel(0f);
                    API.SetRainFxIntensity(0f);
                    API.SetSnowLevel(0f);
                    API.SetBlackout(true);
                    API.SetArtificialLightsStateAffectsVehicles(false);
                    break;
                    /*
                case WeatherType.Test:
                    API.SetWeatherTypeNow("CLEAR");
                    API.SetOverrideWeather("CLEAR");
                    API.ClearCloudHat();
                    API.SetRainLevel(-1f);
                    API.SetRainFxIntensity(-1f);
                    API.SetSnowLevel(0f);
                    _ticks.On(RaisedWaterLevel);
                    API.SetBlackout(false);
                    break;
                    */
                default:
                    API.ClearCloudHat();
                    API.SetRainLevel(0f);
                    API.SetRainFxIntensity(0f);
                    API.SetSnowLevel(0f);
                    break;
            }

            SetTime();
        }

        private Task RaisedWaterLevel()
        {
            //API.ModifyWater(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, 100f, 1000f);
            //API.GetWat
            //var waterquad = API.GetWaterQuadAtCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y);
            API.SetWaterQuadBounds(0, -5000, -5000, 5000, 5000);
            API.SetWaterQuadLevel(0, 10f);
            return Task.FromResult(0);
        }
    }
}