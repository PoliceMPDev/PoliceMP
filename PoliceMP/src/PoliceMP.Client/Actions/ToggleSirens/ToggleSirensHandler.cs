using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Scripts.Garage;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.Decors;

namespace PoliceMP.Client.Actions.ToggleSirens
{
    public class ToggleSirensHandler : ActionHandler<ToggleSirens>
    {
        private IGarageInterface _garageInterface;
        private IPermissionService _permissionService;
        
        public ToggleSirensHandler(IGarageInterface garageInterface, IPermissionService permissionService)
        {
            _garageInterface = garageInterface;
            _permissionService = permissionService;
        }
        
        protected override async Task<bool> Handle(ToggleSirens action)
        {
            if (action.Vehicle.HasDriver()) return false;

            if (!action.Vehicle.IsEngineRunning)
            {
                action.Vehicle.SetEngineState(true);
            }

            var userAces = await _permissionService.GetUserAces();

            if (userAces.IsAdmin || userAces.IsDeveloper || userAces.IsModerator)
            {
                API.DecorSetBool(action.Vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
                API.DecorSetInt(action.Vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, 0);
                return true;
            }

            var personalVehicle = (Vehicle) Entity.FromNetworkId(_garageInterface.PersonalVehicleNetId);

            if (!Entity.Exists(personalVehicle)) return false;
            
            switch (action.Vehicle.IsSirenActive)
            {
                case false when personalVehicle == action.Vehicle:
                    API.DecorSetBool(action.Vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
                    API.DecorSetInt(action.Vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, 0);
                    return true;
                case true:
                    API.DecorSetBool(action.Vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
                    API.DecorSetInt(action.Vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, 0);
                    return true;
                default:
                    return false;
            }
        }
        
    }
}