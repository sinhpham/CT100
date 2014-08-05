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

            var scanTI = new ToolbarItem(){ Name = "scan" };
            scanTI.Command = new Command(obj =>
            {
                scanTI.Name = scanTI.Name == "scan" ? "stop" : "scan";
                // Force refresh toolbar items UI.
                ToolbarItems.Remove(scanTI);
                ToolbarItems.Add(scanTI);
            });

            ToolbarItems.Add(scanTI);
        }
    }

    public class TextCellWithDisclosure : TextCell
    {

    }
}

