using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.CharCustom
{
    public interface ICharCustom
    {
        public Component GetCombination(int id);
        
        public Component GetPropCombination(int id);
        public void LoadOutfit(PedOutfit outfit);
    }
}