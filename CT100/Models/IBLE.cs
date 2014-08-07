﻿using System;
using System.Linq.Expressions;

namespace CT100
{
    public interface IBLE
    {
        void Init();

        bool Scan();

        void Connect(CT100Device d);

        void ReadData<T>(Expression<Func<T>> selectorExpression);

        event EventHandler<DeviceFoundEventArgs> DeviceFound;
        event EventHandler<ErrorOccurredAventArgs> ErrorOccurred;
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public CT100Device Device{ get; set; }
    }

    public class ErrorOccurredAventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}

