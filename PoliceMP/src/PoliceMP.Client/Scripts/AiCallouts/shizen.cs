using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Shared.Extensions;

namespace PoliceMP.Client.Scripts.AiCallouts
{
    public class shizen
    {
        public async Task OldNotificationSystem(string title, string subtitle, string body)
        {
            IEnumerable<PedHash> pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>();
            PedHash pedHash = pedHashes.GetRandom();

            Ped ped = await World.CreatePed(pedHash, new Vector3((float)-2.94, (float)-0.61, (float)0.03));

            API.RequestModel(ped.Model);
            while (!API.HasModelLoaded(ped.Model))
            {
                Debug.WriteLine(("Waiting for model to load"));
            }

            int handle = API.RegisterPedheadshot(ped.Handle);
            while (!API.IsPedheadshotReady(handle) || !API.IsPedheadshotValid(handle))
            {
                await BaseScript.Delay(100);
            }

            var txd = API.GetPedheadshotTxdString(handle);

            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(body);

            const int iconType = 1;
            const bool flash = false;
            API.EndTextCommandThefeedPostMessagetext(txd, txd, flash, iconType, title, subtitle);
            const bool showInBrief = true;
            const bool blink = false;
            API.EndTextCommandThefeedPostTicker(blink, showInBrief);
            API.UnregisterPedheadshot(handle);
            ped.Delete();
        }

        private void OnNewCallout(string title, string subtitle, string body)
        {
            // Your logic to handle the new callout event
            Debug.WriteLine("New callout received!");
            Debug.WriteLine("Title: " + title);
            Debug.WriteLine("Subtitle: " + subtitle);
        }

        private void TestCommands()
        {
            //API.RegisterCommand("bliptester", new Action(OnAttachedToCallout), false);
        }
    }
}