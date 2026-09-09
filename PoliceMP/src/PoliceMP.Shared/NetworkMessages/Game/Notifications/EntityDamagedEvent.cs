using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class EntityDamagedEvent : INotification
    {
        public int VictimNetworkId { get; set; }
        public int AttackerNetworkId { get; set; }
        public float Damage { get; set; }
        public int HitBone { get; set; }
        public bool VictimDied { get; set; }
        public uint WeaponHash { get; set; }
        public bool IsMeleeDamage { get; set; }
        public bool WasHitWithVehicle { get; set; }
        public int VehicleDamageFlags { get; set; }

    }
}