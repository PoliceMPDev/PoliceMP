/*
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared.Models;
using NativeUI;
using NativeUI.PauseMenu;
using System.Collections.Generic;
using System;
using CitizenFX.Core.Native;

namespace PoliceMP.Client.Scripts
{
	public class TestMenu : Script
	{
		private readonly IClientCommunicationsManager _comms;
		private readonly ILogger<TestMenu> _logger;
		private readonly ICommandManager _commands;
		private readonly INotificationService _notifications;

		MenuPool mp = new MenuPool();
		TabView MenuContainer = new TabView("PoliceMP Computer");

		public TestMenu(ILogger<TestMenu> logger, ICommandManager commands, IClientCommunicationsManager comms, INotificationService notifications)
		{
			_logger = logger;
			_commands = commands;
			_comms = comms;
			_notifications = notifications;
		}

		protected override Task OnStartAsync()
		{
			MenuContainer.SideStringTop = Game.Player.Name;
			MenuContainer.SideStringMiddle = "Player ID: " + Game.Player.Handle.ToString();

			
			MenuContainer.SideStringBottom = "Doing lines, shagging nines";
			MenuContainer.DisplayHeader = true;

			mp.AddPauseMenu(MenuContainer);

			TabItem Item1 = new TabItem("simple TabItem");

			TabTextItem Item2 = new TabTextItem("TabTextItem", "This is the Title inside", "With a cool text to be added where you can write whatever you want");

			TabItemSimpleList Item3 = new TabItemSimpleList("TabItemSimpleList", new Dictionary<string, string>()
			{
				["Item 1"] = "subItem 1",
				["Item 2"] = "subItem 2",
				["Item 3"] = "subItem 3",
				["Item 4"] = "subItem 4",
				["Item 5"] = "subItem 5",
				["Item 6"] = "subItem 6"
			});


			List<UIMenuItem> items = new List<UIMenuItem>()
		{
			new UIMenuItem("Item 1"),
			new UIMenuCheckboxItem("Item 2", true),
			new UIMenuListItem("Item 3", new List<dynamic>(){"Item1", 2, 3.0999 }, 0),
			new UIMenuSliderItem("Item 4", "", true),
			new UIMenuSliderProgressItem("Item 5", 20, 0),
		};

			TabInteractiveListItem Item4 = new TabInteractiveListItem("TabInteractiveListItem", items);
			List<MissionInformation> info = new List<MissionInformation>()
		{
			new MissionInformation("Mission 1", new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("item 1", "description 1"),
				new Tuple<string, string>("item 2", "description 2"),
				new Tuple<string, string>("item 3", "description 3"),
				new Tuple<string, string>("item 4", "description 4"),
				new Tuple<string, string>("item 5", "description 5"),
			}),
			new MissionInformation("Mission 2", new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("item 1", "description 1"),
				new Tuple<string, string>("item 2", "description 2"),
				new Tuple<string, string>("item 3", "description 3"),
				new Tuple<string, string>("item 4", "description 4"),
				new Tuple<string, string>("item 5", "description 5"),
			}),
		};
			TabSubmenuItem Item5 = new TabSubmenuItem("TabSubmenuItem", new List<TabItem>()
		{
			new TabItem("simple TabItem"),
			new TabTextItem("TabTextItem", "This is the Title inside", "With a cool text to be added where you can write whatever you want"),
			new TabItemSimpleList("TabItemSimpleList", new Dictionary<string, string>()
			{
				["Item 1"] = "subItem 1",
				["Item 2"] = "subItem 2",
				["Item 3"] = "subItem 3",
				["Item 4"] = "subItem 4",
				["Item 5"] = "subItem 5",
				["Item 6"] = "subItem 6"
			}),
			new TabMissionSelectItem("Mission tab", info),
			new TabInteractiveListItem("TabInteractiveListItem", items)
		});
			TabMissionSelectItem Item6 = new TabMissionSelectItem("Mission tab", info);

			MenuContainer.AddTab(Item1);
			MenuContainer.AddTab(Item2);
			MenuContainer.AddTab(Item3);
			MenuContainer.AddTab(Item4);
			MenuContainer.AddTab(Item5);
			MenuContainer.AddTab(Item6);
			// this way we can choose which tab is the defualt one
			Item1.Active = true;
			Item1.Focused = true;
			Item1.Visible = true;
			MenuContainer.Visible = true;

			return Task.FromResult(0);
		}
		protected override async Task OnTickAsync()
		{
			mp.ProcessMenus();
			if (API.IsInputDisabled(2) && Game.IsControlJustPressed(0, (Control)166))
			{				
				MenuContainer.Visible = !MenuContainer.Visible;
			}
		}
    }
}
*/
