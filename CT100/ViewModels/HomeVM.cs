using System;
using System.Collections.ObjectModel;

namespace CT100
{
    public class HomeVM : NPCBase
    {
        public HomeVM()
        {
            Devices = new ObservableCollection<CT100Device>();

            Devices.Add(new CT100Device(){ Name = "aaa" });
        }

        public ObservableCollection<CT100Device> Devices { get; private set; }
    }
}

