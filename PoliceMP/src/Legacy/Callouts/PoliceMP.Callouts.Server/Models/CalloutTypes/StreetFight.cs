using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class StreetFight : Callout
    {
        private const int HAS_WEAPON_CHANCE = 10;

        public StreetFight(int id) : base(id)
        {
            Title = "Street Fight";
            Grade = 1;
            Description = "Some jakeys having a fight.";
            LocationSetting = LocationSetting.GetNextPositionOnSidewalk;
        }

        public override void Setup()
        {
            for (var i = 0; i < PoliceMpRandom.Next(2, 10); i++)
            {
                var ped = new CalloutPed()
                {
                    Key = $"Fighter{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                    BlipColor = BlipColor.Red,
                    HasBlip = true,
                    SpawnLocation = Location
                };

                if (PoliceMpRandom.Next(100) <= HAS_WEAPON_CHANCE)
                {
                    var index = PoliceMpRandom.Next(6);
                    if (index == 0) ped.WeaponHash = WeaponHash.Knife;
                    else if (index == 1) ped.WeaponHash = WeaponHash.Bat;
                    else if (index == 2) ped.WeaponHash = WeaponHash.Bottle;
                    else if (index == 3) ped.WeaponHash = WeaponHash.Crowbar;
                    else if (index == 4) ped.WeaponHash = WeaponHash.KnuckleDuster;
                    else if (index == 5) ped.WeaponHash = WeaponHash.Wrench;
                }

                AddEntity(ped);
            }
        }

        protected override void OnFirstPlayerArrived(Player player)
        {
            base.OnFirstPlayerArrived(player);

            var entities = _entities.Values.ToList();
            foreach (var entity in entities)
            {
                if (entity is CalloutPed ped)
                {
                    CalloutPed pedToAttack = null;
                    var attempts = 0;
                    while (pedToAttack == null || pedToAttack.Key.Equals(ped.Key))
                    {
                        if (attempts >= 5)
                            break;
                        attempts++;

                        pedToAttack = entities[PoliceMpRandom.Next(entities.Count() - 1)] as CalloutPed;
                    }
                    if (pedToAttack != null)
                        ClientEventAPI.SetPedAttackPed(player, ped.NetworkId, pedToAttack.NetworkId);
                }
            }
        }
    }
}
