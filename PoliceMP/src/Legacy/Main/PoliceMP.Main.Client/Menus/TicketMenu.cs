using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Main.Client.Actions.Cars.DriverInteractions;
using PoliceMP.Main.Client.Models;
using PoliceMP.Main.Client.Retrievers;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Main.Client.Menus
{
    public class TicketMenu : BaseScript
    {
        public static void AddToMenu(Menu parentMenu)
        {
            CreateTicketSubmenu(parentMenu);
        }

        private static void CreateTicketSubmenu(Menu parentMenu)
        {
            TicketPerson.CurrentTicket = new Ticket();

            var ticketMenu = new Menu("Person Interaction", "Fixed Penalty Notice");
            MenuController.AddSubmenu(parentMenu, ticketMenu);

            var ticketMenuBtn =
                new MenuItem("Issue Fixed Penalty Notice", "Issue a Fixed Penalty Notice to the person.");
            parentMenu.AddMenuItem(ticketMenuBtn);
            MenuController.BindMenuItem(parentMenu, ticketMenu, ticketMenuBtn);

            CreateOffencesSubmenu(ticketMenu);

            var fineAmount = new MenuItem("Fine", "The amount in pounds that the person will be fined.")
            { Label = $"£{TicketPerson.CurrentTicket.GetFine()}" };

            var pointsAmount = new MenuItem("Points", "The amount of points that will be issued to the person.")
            { Label = $"{TicketPerson.CurrentTicket.GetPoints()}" };

            var issue = new MenuItem("Issue", "Issue the Fixed Penalty Notice.");

            ticketMenu.AddMenuItem(fineAmount);
            ticketMenu.AddMenuItem(pointsAmount);
            ticketMenu.AddMenuItem(issue);

            ticketMenu.OnMenuOpen += menu =>
            {
                if (TicketPerson.CurrentTicket == null) TicketPerson.CurrentTicket = new Ticket();

                var fine = TicketPerson.CurrentTicket.GetFine();
                var points = TicketPerson.CurrentTicket.GetPoints();

                fineAmount.Label = $"£{fine}";
                pointsAmount.Label = $"{points}";

                issue.Enabled = fine > 0 || points > 0;
            };

            ticketMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == issue) API.ExecuteCommand("ticket");
            };
        }

        private static void CreateOffencesSubmenu(Menu ticketMenu)
        {
            var offencesMenu = new Menu("Fixed Penalty Notice", "Select Offences");
            MenuController.AddSubmenu(ticketMenu, offencesMenu);

            var offenceMenuBtn =
                new MenuItem("Select Offences", "Select the offences that are to be given to the person.")
                { Label = "→→→" };
            ticketMenu.AddMenuItem(offenceMenuBtn);
            MenuController.BindMenuItem(ticketMenu, offencesMenu, offenceMenuBtn);

            var categoriesList = new MenuListItem("Category",
                new List<string> { "Endorsable", "Non-Endorsable", "Anti-Social" },
                0, "The category of offence.");

            offencesMenu.AddMenuItem(categoriesList);

            AddOffenceCheckboxes(categoriesList.ListItems[0], offencesMenu);

            offencesMenu.OnMenuOpen += menu => AddOffenceCheckboxes(categoriesList.ListItems[0], offencesMenu);

            offencesMenu.OnListIndexChange += (menu, listItem, oldSelectionIndex, newSelectionIndex, itemIndex) =>
            {
                if (listItem == categoriesList)
                    AddOffenceCheckboxes(categoriesList.ListItems[newSelectionIndex], offencesMenu);
            };

            offencesMenu.OnCheckboxChange += (menu, menuItem, itemIndex, newCheckedState) =>
            {
                if (newCheckedState)
                {
                    if (TicketPerson.CurrentTicket.Offences.Count >= Ticket.MAX_OFFENCES)
                    {
                        ClientFunctions.ShowToast( "Tickets",
                            $"You can only give a maximum of {Ticket.MAX_OFFENCES} offences.", "error");
                        menuItem.Checked = false;
                        return;
                    }

                    TicketPerson.CurrentTicket.Offences.Add(menuItem.Text);
                }
                else
                {
                    TicketPerson.CurrentTicket.Offences.Remove(menuItem.Text);
                }
            };
        }

        private static void AddOffenceCheckboxes(string offenceCategory, Menu offencesMenu)
        {
            var oldOffenceBoxes = offencesMenu.GetMenuItems().OfType<MenuCheckboxItem>();
            foreach (var oldOffenceBox in oldOffenceBoxes)
                offencesMenu.RemoveMenuItem(oldOffenceBox);

            var offences = OffenceRetriever.GetAll()
                .Where(o => o.Category.Equals(offenceCategory));

            foreach (var offence in offences)
            {
                var offenceBox = new MenuCheckboxItem(offence.Name)
                {
                    Style = MenuCheckboxItem.CheckboxStyle.Tick,
                    Checked = TicketPerson.CurrentTicket.Offences.Contains(offence.Name)
                };

                offencesMenu.AddMenuItem(offenceBox);
            }
        }
    }
}