using System;

namespace CT100
{
    public interface IBLE
    {
        void Init();
        void Scan();
        void Connect(Device d);
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
        byte[] ReadData();
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public Device Device{ get; set; }
    }
}

