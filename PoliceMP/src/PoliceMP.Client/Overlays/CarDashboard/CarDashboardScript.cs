using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Overlays;
using PoliceMP.Core.Client.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Overlays.CarDashboardHud
{

	public interface ICarDashboardHud
	{
		//Enable and Disable
	}
	public class CarDashboardHud : ICarDashboardHud
	{

        public CarDashboardHud(INuiManager nuiManager)
        {
			nuiManager.On<string>("GetCarWindowState", GetCarShit); 
        }


		private string GetCarShit()
		{
			return "Shit Sucks";
		}

    }
}
