using System;

namespace CT100
{
    public class CT100Device : NPCBase
    {
        public string Name { get; set; }

        public string UUID { get; set; }

        public override string ToString()
        {
            return Name;
        }

        double _batteryLevel;

        public double BatteryLevel
        {
            get { return _batteryLevel; }
            set { SetProperty(ref _batteryLevel, value); }
        }

        double _zVal;

        public double ZVal
        {
            get { return _zVal; }
            set { SetProperty(ref _zVal, value); }
        }

        int _radCount;

        public int RadCount
        {
            get { return _radCount; }
            set { SetProperty(ref _radCount, value); }
        }

        bool _accelEnable;

        public bool AccelEnable
        {
            get { return _accelEnable; }
            set { SetProperty(ref _accelEnable, value); }
        }
    }
}

