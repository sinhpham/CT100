using System;
using CT100.iOS;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
using System.Diagnostics;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

[assembly: Xamarin.Forms.Dependency(typeof(BLE))]
namespace CT100.iOS
{
    public class BLE : IBLE
    {
        public void Init()
        {
            _cbcm = new CBCentralManager(new BTDelegate(), DispatchQueue.MainQueue);

            _cbcm.UpdatedState += (object sender, EventArgs e) =>
            {
                Debug.WriteLine("ble state updated: {0}", _cbcm.State);
                if (_cbcm.State == CBCentralManagerState.Unsupported || _cbcm.State == CBCentralManagerState.Unauthorized)
                {
                    RaiseErrorOccurred("Bluetooth not supported");
                }
                if (_cbcm.State == CBCentralManagerState.PoweredOn)
                {
                    Scan();
                }
            };

            _cbcm.DiscoveredPeripheral += (object s, CBDiscoveredPeripheralEventArgs e) =>
            {
                // Avoid duplication.
                if (_discoveredPer.IndexOf(e.Peripheral) != -1)
                {
                    return;
                }
                var d = new CT100Device() { Name = e.Peripheral.Name, UUID = e.Peripheral.Identifier.AsString() }; 
                _discoveredPer.Add(e.Peripheral);
                RaiseDeviceFound(d);
                Debug.WriteLine("Found: {0}", e.Peripheral.Name);
            };

            _cbcm.ConnectedPeripheral += (sender, e) =>
            {
                _connectedPer = e.Peripheral;
                InitPeripheral(_connectedPer);
                InitWriteHandle(_connectedDevice);
                _connectedPer.DiscoverServices();
            };

            _cbcm.DisconnectedPeripheral += (object sender, CBPeripheralErrorEventArgs e) =>
            {
                Debug.WriteLine("Disconnected");

                var notification = new UILocalNotification();
                notification.FireDate = DateTime.Now;
                notification.AlertAction = "View Alert";
                notification.AlertBody = "Current sensor disconnected!";
                notification.SoundName = UILocalNotification.DefaultSoundName;
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);

                // Trying to reconnect
                var perList = _cbcm.RetrievePeripheralsWithIdentifiers(_connectedPer.Identifier);
                var cPer = perList.FirstOrDefault(x => x.Identifier == _connectedPer.Identifier);
                if (cPer != null)
                {
                    _cbcm.ConnectPeripheral(cPer);
                }
            };
        }

        void InitPeripheral(CBPeripheral currPer)
        {
            currPer.DiscoveredService += (object sper, NSErrorEventArgs ev) =>
            {
                if (currPer.Services == null)
                {
                    currPer.DiscoverServices();
                    return;
                }

                _accService = currPer.Services.FirstOrDefault(ser => ser.UUID.ToString().Equals("FFA0", StringComparison.OrdinalIgnoreCase));
                if (_accService != null)
                {
                    Debug.WriteLine("Got acc service");
                    currPer.DiscoverCharacteristics(_accService);
                }

                _radService = currPer.Services.FirstOrDefault(ser => ser.UUID.ToString().Equals("AA70", StringComparison.OrdinalIgnoreCase));
                if (_radService != null)
                {
                    Debug.WriteLine("Got radiation service");
                    currPer.DiscoverCharacteristics(_radService);
                }

                _battService = currPer.Services.FirstOrDefault(ser => ser.UUID.ToString().Equals("180F", StringComparison.OrdinalIgnoreCase));
                if (_battService != null)
                {
                    Debug.WriteLine("Got battery serivce");
                    currPer.DiscoverCharacteristics(_battService);
                }
            };

            currPer.DiscoverCharacteristic += (object persender, CBServiceEventArgs pere) =>
            {
                Debug.WriteLine("characteristic dis");
                if (pere.Service == _accService)
                {
                    _accConfig = _accService.Characteristics.FirstOrDefault(ch => ch.UUID.ToString().Equals("FFA1", StringComparison.OrdinalIgnoreCase));
                    // Need to read to show in setting.
                    currPer.ReadValue(_accConfig);

                    _accData = _accService.Characteristics.FirstOrDefault(ch => ch.UUID.ToString().Equals("FFA5", StringComparison.OrdinalIgnoreCase));
                    if (_accData != null)
                    {
                        currPer.SetNotifyValue(true, _accData);
                    }
                }
                else if (pere.Service == _radService)
                {
                    _countData = _radService.Characteristics.FirstOrDefault(ch => ch.UUID.ToString().Equals("AA72", StringComparison.OrdinalIgnoreCase));
                    if (_countData != null)
                    {
                        Debug.WriteLine("Got key count characteristic");
                    }
                }
                else if (pere.Service == _battService)
                {
                    _battData = _battService.Characteristics.FirstOrDefault(ch => ch.UUID.ToString().Equals("2A19", StringComparison.OrdinalIgnoreCase));
                    if (_battData != null)
                    {
                        Debug.WriteLine("Got battery level charac");
                    }
                }
            };

            currPer.UpdatedCharacterteristicValue += (perSender, pere) =>
            {
                var valArr = pere.Characteristic.Value.ToArray();

                if (object.ReferenceEquals(pere.Characteristic, _accData))
                {
                    var bValue = (sbyte)(valArr[0]);
                    var zValue = calcAccel(bValue);
                    _connectedDevice.ZVal = zValue;
                }
                else if (object.ReferenceEquals(pere.Characteristic, _countData))
                {
                    _connectedDevice.RadCount = BitConverter.ToInt32(valArr, 0);
                }
                else if (object.ReferenceEquals(pere.Characteristic, _accConfig))
                {
                    _connectedDevice.AccelEnable = BitConverter.ToBoolean(valArr, 0);
                }
                else if (object.ReferenceEquals(pere.Characteristic, _battData))
                {
                    _connectedDevice.BatteryLevel = valArr[0];
                }
            };
        }

        public bool Scan()
        {
            if (_cbcm.State == CBCentralManagerState.PoweredOn)
            {
                _cbcm.ScanForPeripherals((CBUUID[])null, (PeripheralScanningOptions)null);
                return true;
            }
            return false;
        }

        public void Connect(CT100Device d)
        {
            _connectedDevice = d;

            var currPer = _discoveredPer.FirstOrDefault(p => string.CompareOrdinal(p.Identifier.AsString(), d.UUID) == 0);

            if (currPer != null)
            {
                _cbcm.StopScan();
                _cbcm.ConnectPeripheral(currPer);
            }
        }

        public void ReadData<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
            {
                throw new ArgumentNullException("selectorExpression");
            }
            var body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");


            var requestProp = (PropertyInfo)body.Member;

            var battProp = PropertyHelper<CT100Device>.GetProperty(x => x.BatteryLevel);
            var countProp = PropertyHelper<CT100Device>.GetProperty(x => x.RadCount);

            if (requestProp.Equals(battProp))
            {
                _connectedPer.ReadValue(_battData);
            }
            else if (requestProp.Equals(countProp))
            {
                _connectedPer.ReadValue(_countData);
            }
            else
            {
                Debug.WriteLine("Invalid read data command");
            }
        }

        void InitWriteHandle(CT100Device currDev)
        {
            currDev.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == PropertyHelper<CT100Device>.GetProperty(x => x.AccelEnable).Name)
                {
                    var dataBytes = BitConverter.GetBytes(_connectedDevice.AccelEnable);
                    var nsd = NSData.FromArray(dataBytes);

                    _connectedPer.WriteValue(nsd, _accConfig, CBCharacteristicWriteType.WithResponse);
                }
            };
        }

        CBCentralManager _cbcm;
        List<CBPeripheral> _discoveredPer = new List<CBPeripheral>();
        CBPeripheral _connectedPer;
        CT100Device _connectedDevice;

        CBService _accService;
        CBCharacteristic _accConfig;
        CBCharacteristic _accData;

        CBService _radService;
        CBCharacteristic _countData;

        CBService _battService;
        CBCharacteristic _battData;

        public event EventHandler<DeviceFoundEventArgs> DeviceFound;
        public event EventHandler<ErrorOccurredAventArgs> ErrorOccurred;

        public void RaiseDeviceFound(CT100Device d)
        {
            var handler = DeviceFound;
            if (handler != null)
            {
                handler(this, new DeviceFoundEventArgs(){ Device = d });
            }
        }

        public void RaiseErrorOccurred(string mess)
        {
            var handler = ErrorOccurred;
            if (handler != null)
            {
                handler(this, new ErrorOccurredAventArgs(){ Message = mess });
            }
        }

        float calcAccel(sbyte rawX)
        {
            float v;
            //-- calculate acceleration, unit g, range -2, +2
            v = (float)((rawX * 1.0) / (64));
            return v;
        }
    }

    public class BTDelegate : CBCentralManagerDelegate
    {
        public override void UpdatedState(CBCentralManager central)
        {
            Debug.WriteLine("state updated");
            if (central.State == CBCentralManagerState.PoweredOn)
            {
                Debug.WriteLine("scanning");
                central.ScanForPeripherals((CBUUID[])null, (PeripheralScanningOptions)null);
            }
        }
    }

    public static class PropertyHelper<T>
    {
        public static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

