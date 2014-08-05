using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;

namespace CT100
{
    public partial class DevicePage : ContentPage
    {
        public DevicePage(Device currDev)
        {
            InitializeComponent();

            var ble = DependencyService.Get<IBLE>();

            _batt.Command = new Command(() =>
            {
                Debug.WriteLine("batt tapped");
                var ret = ble.ReadData();

            });
        }
    }
}

