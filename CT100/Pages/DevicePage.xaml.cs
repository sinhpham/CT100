using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;

namespace CT100
{
    public partial class DevicePage : ContentPage
    {
        public DevicePage(CT100Device currDev)
        {
            InitializeComponent();

            BindingContext = new DeviceVM(){ DeviceData = currDev };

            var ble = DependencyService.Get<IBLE>();

            _batt.Command = new Command(() =>
            {
                ble.ReadData(() => VM.DeviceData.BatteryLevel);

            });

            _count.Command = new Command(() =>
            {
                ble.ReadData(() => VM.DeviceData.RadCount);
            });
        }

        public DeviceVM VM { get { return (DeviceVM)BindingContext; } }
    }

    public class StyledTextCell : TextCell
    {
        public string Style{ get; set; }

        public string Accessory{ get; set; }
    }
}

