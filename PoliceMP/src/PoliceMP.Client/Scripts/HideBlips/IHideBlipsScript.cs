using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Client.Scripts.HideBlips
{
    public interface IHideBlipsScript
    {
        bool AreBlipsHiddenForPed(Player player);
    }
}
