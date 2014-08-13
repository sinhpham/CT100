using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CT100
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = App.Container.GetInstance<HomeVM>();
            var ble = DependencyService.Get<IBLE>();

            ble.DeviceFound += (sender, e) =>
            {
                VM.Devices.Add(e.Device);
            };

            ble.ErrorOccurred += (sender, e) =>
            {
                DisplayAlert("Alert", e.Message, "OK");
            };

            _listView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var d = (CT100Device)e.SelectedItem;
                ble.Connect(d);
                _listView.SelectedItem = null;

                var dp = new DevicePage(d);

                Navigation.PushAsync(dp);
            };

            ble.Init();
        }

        public HomeVM VM { get { return (HomeVM)BindingContext; } }
    }
}

