using System;
using CT100.iOS;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
using System.Diagnostics;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;

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
                    throw new InvalidOperationException("Bluetooth not supported");
                }
            };

            _cbcm.DiscoveredPeripheral += (object s, CBDiscoveredPeripheralEventArgs e) =>
            {
                var d = new CT100Device() { Name = e.Peripheral.Name, UUID = e.Peripheral.Identifier.AsString() }; 
                _perList.Add(e.Peripheral);
                RaiseDeviceFound(d);
                Console.WriteLine("Found: {0}", e.Peripheral.Name);
            };

            _cbcm.ConnectedPeripheral += (sender, e) =>
            {
                _connectedPer = e.Peripheral;
                _connectedPer.DiscoveredService += (object sper, NSErrorEventArgs ev) =>
                {
                    if (_connectedPer.Services == null)
                    {
                        _connectedPer.DiscoverServices();
                        return;
                    }

                    foreach (var ser in  _connectedPer.Services)
                    {
                        if (ser.UUID.ToString().Equals("FFA0", StringComparison.OrdinalIgnoreCase))
                        {
                            _accService = ser;
                            Console.WriteLine("Got acc service");
                            _connectedPer.DiscoverCharacteristics(_accService);
                        }
                    }
                };

                _connectedPer.DiscoverCharacteristic += (object persender, CBServiceEventArgs pere) =>
                {
                    Console.WriteLine("characteristic dis");
                    if (pere.Service == _accService)
                    {
                        foreach (var charac in _accService.Characteristics)
                        {
                            if (charac.UUID.ToString().Equals("FFA1", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("got acc config charac");

                                var dataBytes = new byte[1] { 0x01 };
                                var nsd = NSData.FromArray(dataBytes);

                                _connectedPer.WriteValue(nsd, charac, CBCharacteristicWriteType.WithResponse);
                            }
                            else if (charac.UUID.ToString().Equals("FFA5", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("got acc data z charac");
                                _connectedPer.SetNotifyValue(true, charac);
                                _accData = charac;
                            }
                        }
                    }
                };

                _connectedPer.UpdatedCharacterteristicValue += (perSender, pere) =>
                {
                    var valArr = pere.Characteristic.Value.ToArray();

                    if (pere.Characteristic.UUID == CBUUID.FromString("FFA5"))
                    {
                        var bValue = (sbyte)(valArr[0]);
                        var zValue = calcAccel(bValue);
                        _connectedDevice.ZVal = zValue;
                        //Debug.WriteLine("z value: {0}", zValue);
                    }
                };

                _connectedPer.DiscoverServices();
            };

            _cbcm.DisconnectedPeripheral += (object sender, CBPeripheralErrorEventArgs e) =>
            {
                Console.WriteLine("Disconnected");
                // create the notification
                var notification = new UILocalNotification();

                // set the fire date (the date time in which it will fire)
                notification.FireDate = DateTime.Now;

                // configure the alert stuff
                notification.AlertAction = "View Alert";
                notification.AlertBody = "Current sensor disconnected!";

                // modify the badge
                //notification.ApplicationIconBadgeNumber = 1;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);

                // Trying to reconnect
                //                var perList = _cbcm.RetrievePeripheralsWithIdentifiers(_connectedUUID);
                //                var cPer = perList.FirstOrDefault(x => x.Identifier == _connectedUUID);
                //                if (cPer != null)
                //                {
                //                    _cbcm.ConnectPeripheral(cPer);
                //                }

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

            var currPer = _perList.FirstOrDefault(p => string.CompareOrdinal(p.Identifier.AsString(), d.UUID) == 0);

            if (currPer != null)
            {
                _cbcm.StopScan();
                _cbcm.ConnectPeripheral(currPer);
            }
        }

        public byte[] ReadData()
        {
            _connectedPer.ReadValue(_accData);

            return null;
        }

        CBCentralManager _cbcm;
        List<CBPeripheral> _perList = new List<CBPeripheral>();
        CBPeripheral _connectedPer;
        CT100Device _connectedDevice;
        CBService _accService;
        CBCharacteristic _accData;

        public event EventHandler<DeviceFoundEventArgs> DeviceFound;

        public void RaiseDeviceFound(CT100Device d)
        {
            var handler = DeviceFound;
            if (handler != null)
            {
                handler(this, new DeviceFoundEventArgs(){ Device = d });
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
            Console.WriteLine("state updated");
            if (central.State == CBCentralManagerState.PoweredOn)
            {
                Console.WriteLine("scanning");
                central.ScanForPeripherals((CBUUID[])null, (PeripheralScanningOptions)null);
            }
        }
    }
}

