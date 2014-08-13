using System;
using System.Collections.Generic;

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

        int _radCount;

        public int RadCount
        {
            get { return _radCount; }
            set
            {
                SetProperty(ref _radCount, value);

                RadCountTotal += _radCount;
                _radCountTotal6Secs += _radCount;

                RadCount2MinsData.Enqueue(_radCount);
                if (RadCount2MinsData.Count > 120)
                {
                    RadCountTotal -= RadCount2MinsData.Dequeue();
                }

                _radCount6SecsData.Enqueue(_radCount);
                if (_radCount6SecsData.Count > 6)
                {
                    _radCountTotal6Secs -= _radCount6SecsData.Dequeue();
                }

                RaisePropertyChanged(() => Avg2Mins);
                RaisePropertyChanged(() => Avg6Secs);

                // Alert logic

            }
        }

        int _radCountTotal;

        public int RadCountTotal
        {
            get { return _radCountTotal; }
            set { SetProperty(ref _radCountTotal, value); }
        }


        Queue<int> _radCount2MinsData = new Queue<int>();

        public Queue<int> RadCount2MinsData { get { return _radCount2MinsData; } }

        Queue<int> _radCount6SecsData = new Queue<int>();
        int _radCountTotal6Secs;


        public double Avg2Mins
        {
            get { return (double)RadCountTotal / RadCount2MinsData.Count; }
        }

        public double Avg6Secs
        {
            get { return (double)_radCountTotal6Secs / _radCount6SecsData.Count; }
        }

        bool _enableBuzzer;

        public bool EnableBuzzer
        {
            get { return _enableBuzzer; }
            set { SetProperty(ref _enableBuzzer, value); }
        }
    }
}

