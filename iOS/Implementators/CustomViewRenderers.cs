using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CT100;
using CT100.iOS;

[assembly: ExportRenderer(typeof(NonSelectableTableView), typeof(NonSelectableTableViewRenderer))]
namespace CT100.iOS
{
    public class NonSelectableTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TableView> e)
        {
            base.OnElementChanged(e);

            Control.AllowsSelection = false;
        }
    }
}

