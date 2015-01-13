using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CT100
{
    public class CT100Device : NPCBase
    {
        public CT100Device()
        {
            CpmR = GS * 3600;
            CpnSv = CpmR / 10000;
            nSvpC = 1 / CpnSv;
            uSvpC = nSvpC / 1000;
            DCF = uSvpC * ECF;
        }

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
                RadCountTotal2Mins += _radCount;
                _radCountTotal6Secs += _radCount;

                RadCount2MinsData.Enqueue(_radCount);
                if (RadCount2MinsData.Count > 120)
                {
                    RadCountTotal2Mins -= RadCount2MinsData.Dequeue();
                }

                _radCount6SecsData.Enqueue(_radCount);
                if (_radCount6SecsData.Count > 6)
                {
                    _radCountTotal6Secs -= _radCount6SecsData.Dequeue();
                }

                RaisePropertyChanged(() => Avg2Mins);
                RaisePropertyChanged(() => Avg6Secs);
                RaisePropertyChanged(() => DoseRate2Mins);
                RaisePropertyChanged(() => DoseRate6Secs);
            }
        }

        int _radCountTotal2Mins;

        public int RadCountTotal2Mins
        {
            get { return _radCountTotal2Mins; }
            set { SetProperty(ref _radCountTotal2Mins, value); }
        }

        Int64 _radCountTotal;

        public Int64 RadCountTotal
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
            get { return (double)RadCountTotal2Mins / RadCount2MinsData.Count; }
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

        List<int> _deviceRadCountArr;

        public List<int> DeviceRadCountArr
        {
            get { return _deviceRadCountArr; }
            set { SetProperty(ref _deviceRadCountArr, value); }
        }

        int? _deviceRadCountEndIdx;

        public int? DeviceRadCountEndIdx
        {
            get { return _deviceRadCountEndIdx; }
            set { SetProperty(ref _deviceRadCountEndIdx, value); }
        }

        int _devRadTotalCountAlertLevel;

        public int DevRadTotalCountAlertLevel
        {
            get { return _devRadTotalCountAlertLevel; }
            set { SetProperty(ref _devRadTotalCountAlertLevel, value); }
        }

        public void AssignDeviceCountArr()
        {
            if (DeviceRadCountArr == null || DeviceRadCountEndIdx == null)
            {
                Debug.WriteLine("not enough");
            }
            else
            {

                var firstList = DeviceRadCountArr.Where((x, i) =>
                {
                    return i >= DeviceRadCountEndIdx;
                });
                var lastList = DeviceRadCountArr.Where((x, i) =>
                {
                    return i < DeviceRadCountEndIdx;
                });
                var orderedList = firstList.Concat(lastList).ToList();

                // 2 min array.
                RadCount2MinsData.Clear();
                RadCountTotal2Mins = 0;
                foreach (var val in orderedList)
                {
                    RadCount2MinsData.Enqueue(val);
                    RadCountTotal2Mins += val;
                }

                RadCountTotal = RadCountTotal2Mins;

                // 6 seconds array
                _radCount6SecsData.Clear();
                _radCountTotal6Secs = 0;
                foreach (var val in orderedList.Skip(orderedList.Count - 6))
                {
                    _radCount6SecsData.Enqueue(val);
                    _radCountTotal6Secs += val;
                }
            }
        }

        const double ECF = 0.6;
        const double GS = 7.5;

        readonly double CpmR;
        readonly double CpnSv;
        readonly double nSvpC;
        readonly double uSvpC;
        readonly double DCF;

        public double DoseRate2Mins
        {
            get
            {
                return Avg2Mins * DCF * 3600;
            }
        }

        public double DoseRate6Secs
        {
            get
            {
                return Avg6Secs * DCF * 3600;
            }
        }

    }
}

