using System;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Services.Geolocation;
using System.Threading.Tasks;

namespace CT100
{
    public class SettingsVM : NPCBase
    {
        public SettingsVM()
        {
            _geo = DependencyService.Get<IGeolocator>();
            _geo.StartListening(5000, 0);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    var p = await _geo.GetPositionAsync(1000);

                    Timestamp = DateTimeOffset.Now;
                    Lat = p.Latitude;
                    Long = p.Longitude;
                }
            }, TaskCreationOptions.LongRunning);
        }

        IGeolocator _geo;

        DateTimeOffset _timestamp;

        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }

        double _lat;

        public double Lat
        {
            get { return _lat; }
            set { SetProperty(ref _lat, value); }
        }

        double _long;

        public double Long
        {
            get { return _long; }
            set { SetProperty(ref _long, value); }
        }
    }
}

