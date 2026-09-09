using MenuAPI;

namespace PoliceMP.Client.Extensions
{
    static class MenuExtensions
    {
        public static MenuItem AddItem(this Menu menu, MenuItem menuItem)
        {
            menu.AddMenuItem(menuItem);
            return menuItem;
        }
    }
}