using System;
using System.Collections.ObjectModel;

namespace CT100
{
    public class HomeVM : NPCBase
    {
        public HomeVM()
        {
            Devices = new ObservableCollection<Device>();
        }

        public ObservableCollection<Device> Devices { get; private set; }
    }

    public class Device
    {
        public string Name{ get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

