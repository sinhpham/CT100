using System;
using System.Linq.Expressions;

namespace CT100
{
    public interface IBLE
    {
        void Init();
        bool Scan();
        void Connect(CT100Device d);
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
        void ReadData<T>(Expression<Func<T>> selectorExpression);
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public CT100Device Device{ get; set; }
    }
}

