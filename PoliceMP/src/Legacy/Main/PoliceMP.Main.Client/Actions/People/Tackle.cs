using CitizenFX.Core;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Tackle : BaseScript
    {
        /*[Tick]
        private async Task OnTick()
        {
            if (Game.PlayerPed.IsRunning || Game.PlayerPed.IsSprinting)
            {
                var pedId = Functions.GetPedInFrontOfPlayer();
                if (pedId == -1) return;
                if (!Functions.IsPedInteractable(pedId)) return;

                Screen.DisplayHelpTextThisFrame("Press ~y~Q ~w~to ground tackle.");

                if (API.IsControlPressed(0, 44))
                {
                    var ped = (Ped) Entity.FromHandle(pedId);
                    ped.IsPersistent = true;
                    ped.BlockPermanentEvents = true;
                    ped.CanRagdoll = false;

                    Game.PlayerPed.Task.PlayAnimation("missmic2ig_11", "mic_2_ig_11_intro_goon");

                    await Delay(500);

                    var count = 0;
                    while (count < 100)
                    {
                        count++;

                        if (Game.PlayerPed.IsTouching(ped))
                        {
                            await Delay(100);
                            if (Game.PlayerPed.IsTouching(ped))
                            {
                                ped.Ragdoll();
                                ped.ApplyForce(new Vector3(0.1f, 0.1f, 0.1f), Game.PlayerPed.Rotation);
                                break;
                            }
                        }

                        await Delay(10);
                    }

                    await Delay(1000);

                    Game.PlayerPed.Task.ClearAll();
                    ped.Task.ClearAll();

                    ped.IsCollisionEnabled = true;
                    Game.PlayerPed.IsPositionFrozen = false;
                    ped.IsPositionFrozen = false;

                    Game.PlayerPed.Task.PlayAnimation("rcmcollect_paperleadinout@", "meditate_getup");
                    ped.MarkAsNoLongerNeeded();
                }
            }
        }*/
    }
}