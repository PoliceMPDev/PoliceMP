using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.Anticheat
{
    internal class MenuCheck : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;

        public MenuCheck(ILegacyClientCommunicationsManager comms, ITickManager ticks)
        {
            _comms = comms;
            _ticks = ticks;

            _ticks.On(CheckForModMenus);
        }

        private async Task CheckForModMenus()
        {
            List<TextureData> detectableTextures = new List<TextureData>
            {
                new TextureData("HydroMenu", "HydroMenuHeader", "HydroMenu"),
                new TextureData("John", "John2", "SugarMenu"),
                new TextureData("darkside", "logo", "Darkside"),
                new TextureData("ISMMENU", "ISMMENUHeader", "ISMMENU"),
                new TextureData("dopatest", "duiTex", "Copypaste Menu"),
                new TextureData("fm", "menu_bg", "Fallout"),
                new TextureData("wave", "logo", "Wave"),
                new TextureData("wave1", "logo1", "Wave (alt.)"),
                new TextureData("meow2", "woof2", "Alokas66", 1000, 1000),
                new TextureData("adb831a7fdd83d_Guest_d1e2a309ce7591dff86", "adb831a7fdd83d_Guest_d1e2a309ce7591dff8Header6", "Guest Menu"),
                new TextureData("hugev_gif_DSGUHSDGISDG", "duiTex_DSIOGJSDG", "HugeV Menu"),
                new TextureData("MM", "menu_bg", "MetrixFallout"),
                new TextureData("wm", "wm2", "WM Menu"),
                new TextureData("absoluteeulen", "Absolut", "Absolut Menu"),
                new TextureData("Dopamine", "Dopameme", "Dopamine Menu"),
                new TextureData("SkidMenu", "skidmenu", "Skid Menu"),
                new TextureData("tiago", "Tiago", "Tiago Menu"),
                new TextureData("lynxmenu", "lynxmenu", "Lynx Menu"),
                new TextureData("Reaper", "reaper", "Reaper Menu")
            };

            foreach (var data in detectableTextures)
            {
                var resolution = API.GetTextureResolution(data.Txd, data.Txt);


                if (data.X.HasValue && data.Y.HasValue)
                {
                    if (resolution.X == data.X && resolution.Y == data.Y)
                    {
                        SendMessageToMods($"Possible mod menu detected for {Game.Player.Name} ({data.Txt}) ({data.Name})");
                    }
                }
                else
                {
                    if (resolution.X != 4.0f)
                    {
                        SendMessageToMods($"Possible mod menu detected for {Game.Player.Name} ({data.Txt}) ({data.Name})");
                    }
                }
            }
        }

        private class TextureData
        {
            public string Txd { get; }
            public string Txt { get; }
            public string Name { get; }
            public float? X { get; }
            public float? Y { get; }

            public TextureData(string txd, string txt, string name, float? x = null, float? y = null)
            {
                Txd = txd;
                Txt = txt;
                Name = name;
                X = x;
                Y = y;
            }
        }

        private void SendMessageToMods(string message)
        {
            _comms.ToServer(ServerEvents.SendMessageToMods, message);
        }
    }
}
