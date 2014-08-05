using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CT100
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            _chosenItemCmd = new Command(obj =>
            {
                var mic = MenuItemChanged;
                if (mic != null)
                {
                    mic.Invoke(this, new MenuItemChangedEventArg()
                    {
                        SelectedMenuItem = obj as MenuItem,
                    });
                }
            });

            _menuItems = new List<MenuItem>()
            {
                new MenuItem("Home", () => new HomePage()),
                new MenuItem("Settings", () => new ContentPage()),
                new MenuItem("About", () => new ContentPage()),
            };

            _listView.ItemsSource = _menuItems;

            _listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem != null)
                {
                    var mi = (MenuItem)e.SelectedItem;
                    _listView.SelectedItem = null;
                    _chosenItemCmd.Execute(mi);
                }
            };
        }

        List<MenuItem> _menuItems;

        public Page DefaultPage
        {
            get
            {
                return _menuItems[0].NaviPage;
            }
        }

        public event EventHandler<MenuItemChangedEventArg> MenuItemChanged;

        Command _chosenItemCmd;
    }

    public class MenuItemChangedEventArg : EventArgs
    {
        public MenuItem SelectedMenuItem { get; set; }
    }
}

