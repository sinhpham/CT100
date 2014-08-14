using System;

namespace CT100
{
    public class DeviceVM : NPCBase
    {
        public DeviceVM()
        {
        }

        CT100Device _deviceData;

        public CT100Device DeviceData
        {
            get { return _deviceData; }
            set { SetProperty(ref _deviceData, value); }
        }

        bool _displayingAlert;

        public bool DisplayingAlert
        {
            get { return _displayingAlert; }
            set { SetProperty(ref _displayingAlert, value); }
        }

        bool _alertDismissed;

        public bool AlertDismissed
        {
            get { return _alertDismissed; }
            set { SetProperty(ref _alertDismissed, value); }
        }
    }
}

