using System.Threading.Tasks;
namespace PoliceMP.Client.Scripts.ELS
{
    public interface IELS
    {
        public Task InitElsVehicle(int vehicle, int sirentype);
    }
}