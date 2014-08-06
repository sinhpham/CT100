using System;

namespace CT100
{
    public interface IBLE
    {
        void Init();
        bool Scan();
        void Connect(CT100Device d);
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
        byte[] ReadData();
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public CT100Device Device{ get; set; }
    }
}

