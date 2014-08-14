using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CT100
{
    public partial class AlertPage : ContentPage
    {
        public AlertPage(DeviceVM currVM)
        {
            InitializeComponent();

            BindingContext = currVM;

            VM.DisplayingAlert = true;

            _okBtn.Command = new Command(() =>
            {
                Navigation.PopModalAsync();
                VM.DisplayingAlert = false;
                VM.AlertDismissed = true;
            });
        }

        public DeviceVM VM { get { return (DeviceVM)BindingContext; } }
    }
}

