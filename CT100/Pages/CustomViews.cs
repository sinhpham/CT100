using System;
using Xamarin.Forms;

namespace CT100
{
    public class StyledTextCell : TextCell
    {
        public string Style { get; set; }

        public string Accessory { get; set; }

        public string SelectionStyle { get; set; }

        public Font TextFont { get; set; }

        public Font DetailTextFont { get; set; }
    }

    public class NonSelectableTableView : TableView
    {

    }
}

