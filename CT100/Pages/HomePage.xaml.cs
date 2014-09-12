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
            _ble = DependencyService.Get<IBLE>();

            _ble.DeviceFound += (sender, e) =>
            {
                VM.Devices.Add(e.Device);
            };

            _ble.ErrorOccurred += (sender, e) =>
            {
                DisplayAlert("Alert", e.Message, "OK");
            };

            _listView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var d = (CT100Device)e.SelectedItem;
                _ble.Connect(d);
                _listView.SelectedItem = null;

                var dp = new DevicePage(d);

                Navigation.PushAsync(dp);
            };

            //_ble.Init();
        }

        IBLE _ble;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ble.Disconnect();
            _ble.Init();
            VM.Devices.Clear();
        }

        public HomeVM VM { get { return (HomeVM)BindingContext; } }
    }
}

