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

            BindingContext = new HomeVM();
            var ble = DependencyService.Get<IBLE>();

            ble.DeviceFound += (sender, e) =>
            {
                VM.Devices.Add(e.Device);
            };

            ble.ErrorOccurred += (sender, e) =>
            {
                DisplayAlert("Alert", e.Message, "OK");
            };

            const string scanStr = "scan";
            var scanTI = new ToolbarItem(){ Name = scanStr };
            scanTI.Command = new Command(obj =>
            {
                if (string.CompareOrdinal(scanTI.Name, scanStr) == 0)
                {
                    // Need to scan.
                    if (ble.Scan())
                    {
                        scanTI.Name = "stop";
                        // Force refresh UI.
                        ToolbarItems.Remove(scanTI);
                        ToolbarItems.Add(scanTI);
                    }
                }
                else
                {
                    // TODO: Need to stop scanning.
                }
            });

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

            ToolbarItems.Add(scanTI);

            ble.Init();
        }

        public HomeVM VM { get { return (HomeVM)BindingContext; } }
    }
}

