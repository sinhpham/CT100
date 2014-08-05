using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;

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

            var scanTI = new ToolbarItem(){ Name = "scan" };
            scanTI.Command = new Command(obj =>
            {
                scanTI.Name = scanTI.Name == "scan" ? "stop" : "scan";
                // Force refresh toolbar items UI.
                ToolbarItems.Remove(scanTI);
                ToolbarItems.Add(scanTI);


                ble.Scan();
            });

            ToolbarItems.Add(scanTI);
        }

        public HomeVM VM { get { return (HomeVM)BindingContext; } }
    }

    public class TextCellWithDisclosure : TextCell
    {

    }
}

