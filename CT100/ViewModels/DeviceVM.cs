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
    }
}

