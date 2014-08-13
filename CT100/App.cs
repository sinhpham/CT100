using System;
using Xamarin.Forms;
using SimpleInjector;

namespace CT100
{
    public class App
    {
        static App()
        {
            Container = new Container();
            Container.RegisterSingle(() => new SettingsVM());
            Container.RegisterSingle(() => new HomeVM());
        }

        public static Container Container { get; private set; }

        public static Page GetMainPage()
        {	
            var rootPage = new RootPage();

            return rootPage;
        }
    }
}

