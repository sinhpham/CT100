using System;
using Xamarin.Forms;

namespace CT100
{
    public class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            var menuPage = new MenuPage();
            menuPage.MenuItemChanged += OnMenuItemChanged;

            Master = new NavigationPage(menuPage) { Title = "Menu" };
            Detail = menuPage.DefaultPage;
        }

        public void OnMenuItemChanged(object sender, MenuItemChangedEventArg e)
        {
            Detail = e.SelectedMenuItem.NaviPage ?? Detail;
            IsPresented = false;
        }
    }
}

