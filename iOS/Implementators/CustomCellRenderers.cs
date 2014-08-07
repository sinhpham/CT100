using System;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.UIKit;
using Xamarin.Forms;
using System.ComponentModel;
using CT100;
using CT100.iOS;

[assembly: ExportRenderer(typeof(StyledTextCell), typeof(StyledTextCellRenderer))]
namespace CT100.iOS
{
    public class StyledTextCellRenderer : TextCellRenderer
    {
        public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableView tv)
        {
            var sTextCell = (StyledTextCell)item;
            var style = UITableViewCellStyle.Default;
            Enum.TryParse(sTextCell.Style, out style);

            string text = "Xamarin.Forms.StyledTextCell";
            CellTableViewCell cellTableViewCell = tv.DequeueReusableCell(text) as CellTableViewCell;
            if (cellTableViewCell == null)
            {
                cellTableViewCell = new CellTableViewCell(style, text);
            }
            else
            {
                cellTableViewCell.Cell.PropertyChanged -= new PropertyChangedEventHandler(cellTableViewCell.HandlePropertyChanged);
            }
            cellTableViewCell.Cell = sTextCell;
            sTextCell.PropertyChanged += new PropertyChangedEventHandler(cellTableViewCell.HandlePropertyChanged);
            cellTableViewCell.PropertyChanged = new Action<object, PropertyChangedEventArgs>(this.HandlePropertyChanged);
            cellTableViewCell.TextLabel.Text = sTextCell.Text;
            cellTableViewCell.TextLabel.Font = sTextCell.TextFont.ToUIFont();

            if (cellTableViewCell.DetailTextLabel != null)
            {
                cellTableViewCell.DetailTextLabel.Text = sTextCell.Detail;
                cellTableViewCell.DetailTextLabel.Font = sTextCell.DetailTextFont.ToUIFont();
            }

            base.UpdateBackground(cellTableViewCell, item);

            var acc = UITableViewCellAccessory.None;
            Enum.TryParse(sTextCell.Accessory, out acc);

            cellTableViewCell.Accessory = acc;

            var selectionStyle = UITableViewCellSelectionStyle.Default;
            Enum.TryParse(sTextCell.SelectionStyle, out selectionStyle);
            cellTableViewCell.SelectionStyle = selectionStyle;

            return cellTableViewCell;
        }
    }
}

