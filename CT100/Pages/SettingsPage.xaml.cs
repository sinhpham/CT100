﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CT100
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = new SettingsVM();
        }
    }
}

