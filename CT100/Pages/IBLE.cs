using System;

namespace CT100
{
    public interface IBLE
    {
        void Scan();
        void Connect(Device d);
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public Device Device{ get; set; }
    }
}

