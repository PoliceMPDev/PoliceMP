using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Services
{
    public class InputService  : Script, IInputService
    {
        public InputService()
        {
            
        }

        public async Task<string> ShowKeyboardInput(string textEntry, string exampleText, int maxStringLength)
        {
            API.AddTextEntry("FMMC_KEY_TIP1", textEntry);
            API.DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", exampleText, "", "", "", maxStringLength);

            while (API.UpdateOnscreenKeyboard() != 1 && API.UpdateOnscreenKeyboard() != 2)
            {
                await Delay(0);
            }

            if (API.UpdateOnscreenKeyboard() == 2) return null;
            
            var result = API.GetOnscreenKeyboardResult();
            await Delay(500);
            return result;

        }
    }
}