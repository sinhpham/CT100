using System;
using Xamarin.Forms;

namespace CT100
{
    public class MenuItem
    {
        public MenuItem(string title, Func<Page> initRootPage)
        {
            MenuTitle = title;

            _initRootPage = initRootPage;
        }

        Func<Page> _initRootPage;

        public string MenuTitle { get; set; }

        NavigationPage _naviPage;

        public NavigationPage NaviPage
        {
            get
            {
                // Delay initiation of a page until requested.
                if (_naviPage == null)
                {
                    _naviPage = new NavigationPage(RootPage);
                }
                return _naviPage;
            }
        }

        Page _rootPage;

        public Page RootPage
        {
            get
            {
                if (_rootPage == null)
                {
                    _rootPage = _initRootPage();
                    _rootPage.Title = MenuTitle;
                    _initRootPage = null;
                }
                return _rootPage;
            }
        }

        public override string ToString()
        {
            return MenuTitle;
        }
    }
}

