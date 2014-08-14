using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CT100
{
    public partial class DevicePage : ContentPage
    {
        public DevicePage(CT100Device currDev)
        {
            InitializeComponent();

            BindingContext = new DeviceVM(){ DeviceData = currDev };

            _settings = App.Container.GetInstance<SettingsVM>();

            var ble = DependencyService.Get<IBLE>();

            _batt.Command = new Command(() =>
            {
                ble.ReadData(() => VM.DeviceData.BatteryLevel);
            });

            _radCountArr.Command = new Command(() =>
            {
                ble.ReadCountArr();
            });

            _count.Command = new Command(() =>
            {
                ble.ReadData(() => VM.DeviceData.RadCount);
            });

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await _actInd.FadeTo(0);
                    _grid.Children.Remove(_actInd);
                });
            });

            VM.DeviceData.PropertyChanged += (sender, e) =>
            {
                // Alarm logic.
//                if (e.PropertyName == "RadCountTotal")
//                {
//                    var currTotal = VM.DeviceData.RadCountTotal;
//                    if (currTotal >= _settings.TotalAlarm && !VM.DisplayingAlert && !VM.AlertDismissed)
//                    {
//                        Navigation.PushModalAsync(new AlertPage(VM));
//                    }
//                    else if (currTotal < _settings.TotalAlarm)
//                    {
//                        VM.AlertDismissed = false;
//                    }
//                }
                // Testing with 6 secs.
                if (e.PropertyName == "Avg6Secs")
                {
                    var currAvg = VM.DeviceData.Avg6Secs;
                    if (currAvg >= _settings.Avg2MinAlarm && !VM.DisplayingAlert && !VM.AlertDismissed)
                    {
                        Navigation.PushModalAsync(new AlertPage(VM));
                    }
                    else if (currAvg < _settings.TotalAlarm)
                    {
                        VM.AlertDismissed = false;
                    }
                }
            };
        }

        public DeviceVM VM { get { return (DeviceVM)BindingContext; } }

        SettingsVM _settings;
    }
}

