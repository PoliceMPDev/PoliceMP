using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Scripts.AiCallouts;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.PrisonBlips
{
    public class PrisonBlips : Script
    {
        private readonly IPlayerController _playerController;
        
        /// <summary>
        /// Represents the locations of prison cells.
        /// </summary>
        public static readonly List<Vector3> PrisonCells = new List<Vector3>()
        {
            new((float)-1098.02, (float)-820.87, (float)5.47), //343 - Vespooky Cells
            new((float)-1095.44, (float)-824.14, (float)5.47), //343 - Vespooky Cells
            new((float)-1093.27, (float)-827.36, (float)5.47), //343 - Vespooky Cells
            new((float)-1090.61, (float)-830.16, (float)5.47), //343 - Vespooky Cells
            new((float)-1085.01, (float)-826.01, (float)5.47), //343 - Vespooky Cells
            new((float)-1086.97, (float)-822.85, (float)5.47), //343 - Vespooky Cells
            new((float)-1086.97, (float)-822.85, (float)5.47), //343 - Vespooky Cells
            new((float)-1089.55, (float)-819.67, (float)5.47), //343 - Vespooky Cells


            new((float)525.43, (float)-2.91, (float)70.62), //592 - Vinewood (Holding Cell 1)
            new((float)522.78, (float)1.07, (float)70.62), //592 - Vinewood (Holding Cell 2)
            new((float)515.05, (float)-3.54, (float)70.62), //592 - Vinewood (Holding Cell 3)
            new((float)536.30, (float)20.21, (float)70.62), //592 - Vinewood Cell 1
            new((float)539.35, (float)22.06, (float)70.62), //592 - Vinewood Cell 2
            new((float)544.98, (float)25.36, (float)70.62), //592 - Vinewood Cell 3
            new((float)547.64, (float)27.57, (float)70.62), //592 - Vinewood Cell 4
            new((float)552.26, (float)19.61, (float)70.62), //592 - Vinewood Cell 5
            new((float)548.95, (float)17.65, (float)70.62), //592 - Vinewood Cell 6
            new((float)543.40, (float)13.93, (float)70.62), //592 - Vinewood Cell 7
            new((float)540.37, (float)12.60, (float)70.62), //592 - Vinewood Cell 8


            new((float)372.51, (float)-1588.31, (float)23.80), //143 - Davis Cells
            new((float)376.56, (float)-1591.67, (float)23.80), //143 - Davis Cells
            new((float)376.56, (float)-1591.67, (float)23.80), //143 - Davis Cells
            new((float)380.55, (float)-1595.20, (float)23.80), //143 - Davis Cells
            new((float)380.55, (float)-1595.20, (float)23.80), //143 - Davis Cells
            new((float)384.29, (float)-1598.31, (float)23.80), //143 - Davis Cells
            new((float)384.29, (float)-1598.31, (float)23.80), //143 - Davis Cells
            new((float)387.57, (float)-1602.39, (float)23.80), //143 - Davis Cells
            new((float)386.05, (float)-1606.20, (float)23.80), //143 - Davis Cells
            new((float)381.95, (float)-1607.84, (float)23.80), //143 - Davis Cells
            new((float)378.39, (float)-1606.23, (float)23.80), //143 - Davis Cells
            new((float)375.73, (float)-1604.62, (float)23.80), //143 - Davis Cells


            new((float)1849.25, (float)3682.29, (float)34.27), //1029 - Sandy Shores Cells
            new((float)1845.92, (float)3680.13, (float)34.27), //1029 - Sandy Shores Cells
            new((float)1842.33, (float)3677.89, (float)34.27), //1029 - Sandy Shores Cells


            new((float)-431.00, (float)5992.83, (float)31.71), //3005 - Paleto Cells
            new((float)-428.16, (float)5994.99, (float)31.71), //3005 - Paleto Cells
            new((float)-425.74, (float)5997.84, (float)31.71), //3005 - Paleto Cells


            new((float)472.37, (float)-993.80, (float)24.91), //217 - Mission Row (Cell 1)
            new((float)476.53, (float)-994.57, (float)24.91), //217 - Mission Row (Cell 2)
            new((float)480.81, (float)-994.93, (float)24.91), //217 - Mission Row (Cell 3)
            new((float)480.42, (float)-1005.65, (float)24.91), //217 - Mission Row (Cell 4)
            new((float)476.22, (float)-1005.79, (float)24.91), //217 - Mission Row (Cell 5)
            new((float)471.95, (float)-1005.99, (float)24.91), //217 - Mission Row (Cell 6)


            new((float)-869.79, (float)-2412.76, (float)14.06), //97 - Heathrow Cells
            new((float)-875.30, (float)-2409.07, (float)14.06), //97 - Heathrow Cells
            new((float)-873.06, (float)-2406.24, (float)14.06), //97 - Heathrow Cells
            new((float)-867.90, (float)-2409.76, (float)14.06), //97 - Heathrow Cells
            new((float)-866.34, (float)-2406.98, (float)14.06), //97 - Heathrow Cells
            new((float)-871.56, (float)-2403.37, (float)14.06), //97 - Heathrow Cells
            new((float)-864.55, (float)-2403.87, (float)14.06), //97 - Heathrow Cells
            new((float)-865.17, (float)-2397.74, (float)14.06), //97 - Heathrow Holding Cell
            new((float)-863.58, (float)-2395.17, (float)14.06), //97 - Heathrow Holding Cell
            new((float)-861.83, (float)-2392.33, (float)14.06), //97 - Heathrow Holding Cell
        };
        
        public PrisonBlips(ITickManager tickManager, IPlayerController playerController)
        {
            _playerController = playerController;

            tickManager.On(CreatePrisonBlips);
        }
        
        private async Task CreatePrisonBlips()
        {
            if (null == _playerController.GrabbedPed) return;
            
            float distance;
            foreach (Vector3 location in PrisonCells)
            {
                // Create a blip at each prison cell location.
                distance = API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y,
                    Game.PlayerPed.Position.Z, location.X, location.Y, location.Z, true);
                if (distance > 5.0f)
                    continue;

                float scale = 0.1F * API.GetGameplayCamFov();

                API.DrawMarker(0, location.X, location.Y, location.Z - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 20, 20, 200,
                    50,
                    false, true, 2, false, null, null, false);

                API.SetTextScale(0.1F * scale, 0.1F * scale);
                API.SetTextFont(4);
                API.SetTextProportional(true);
                API.SetTextColour(250, 250, 250, 255);
                API.SetTextDropshadow(1, 1, 1, 1, 255);
                API.SetTextEdge(2, 0, 0, 0, 255);
                API.SetTextDropShadow();
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.SetTextCentre(true);
                API.AddTextComponentString("Place Suspect into cell.");
                API.SetDrawOrigin(location.X, location.Y, location.Z + 1F, 0);
                API.DrawText(0, 0);
                API.ClearDrawOrigin();

                if (distance <= 1.0f)
                {
                    // Delete AI peds when they reach the blip
                    var attachedPed = _playerController.GrabbedPed;
                    if (null != attachedPed)
                    {
                        try
                        {
                            await _playerController.UngrabPed();
                        }
                        catch (Exception ex)
                        {
                            // Ignore just continue anyway to delete it
                        }
                        
                        var handle = attachedPed.Handle;
                        API.DeleteEntity(ref handle);
                    }
                }
            }
        }
    }
}