using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CitizenFX.Core;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Overlays;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Client.Overlays.NewNotification
{
	public interface INewNotificationOverlay
	{
		//Enable and Disable
		void SendNotification(NewNotificationMessage notificationMessage);
	}
	public  class NewNotificationOverlay : Overlay, INewNotificationOverlay
	{
		public NewNotificationOverlay(string id, INuiManager nuiManager, ILogger<Overlay> logger) : base(id, nuiManager, logger)
		{
			Enable();
		}

		public void SendNotification(NewNotificationMessage notificationMessage)
        {
			Emit("SendNotification", notificationMessage);
            //throw new NotImplementedException();
        }
	}
}
