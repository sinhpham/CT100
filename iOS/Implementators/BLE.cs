using System;
using CT100.iOS;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
using System.Diagnostics;
using MonoTouch.UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(BLE))]
namespace CT100.iOS
{
    public class BLE : IBLE
    {


        public void Scan()
        {
            if (_cbcm != null)
            {
                _cbcm.StopScan();
                _cbcm.ScanForPeripherals((CBUUID[])null, (PeripheralScanningOptions)null);
                return;
            }

            _cbcm = new CBCentralManager(new BTDelegate(), DispatchQueue.MainQueue);

            _cbcm.UpdatedState += (object sender, EventArgs e) =>
            {
                Debug.WriteLine("state updated event");
                if (_cbcm.State == CBCentralManagerState.PoweredOn)
                {
                    Debug.WriteLine("scanning");
                    _cbcm.ScanForPeripherals((CBUUID[])null, (PeripheralScanningOptions)null);
                }
            };

            _cbcm.DiscoveredPeripheral += (object s, CBDiscoveredPeripheralEventArgs e) =>
            {
                RaiseDeviceFound(new Device() { Name = e.Peripheral.Name });
                Console.WriteLine("Found: {0}", e.Peripheral.Name);
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

        CBCentralManager _cbcm;

        public event EventHandler<DeviceFoundEventArgs> DeviceFound;

        public void RaiseDeviceFound(Device d)
        {
            var handler = DeviceFound;
            if (handler != null)
            {
                handler(this, new DeviceFoundEventArgs(){ Device = d });
            }
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

