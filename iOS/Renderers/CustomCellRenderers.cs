using System;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.UIKit;
using Xamarin.Forms;
using System.ComponentModel;
using CT100;
using CT100.iOS;

[assembly: ExportRenderer(typeof(TextCellWithDisclosure), typeof(TextCellWithDisclosureRenderer))]
namespace CT100.iOS
{
    public class TextCellWithDisclosureRenderer : TextCellRenderer
    {
        public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableView tv)
        {
            TextCell textCell = (TextCell)item;
            UITableViewCellStyle style = UITableViewCellStyle.Value1;
            string text = "Xamarin.Forms.TextCell";
            CellTableViewCell cellTableViewCell = tv.DequeueReusableCell(text) as CellTableViewCell;
            if (cellTableViewCell == null)
            {
                cellTableViewCell = new CellTableViewCell(style, text);
            }
            else
            {
                cellTableViewCell.Cell.PropertyChanged -= new PropertyChangedEventHandler(cellTableViewCell.HandlePropertyChanged);
            }
            cellTableViewCell.Cell = textCell;
            textCell.PropertyChanged += new PropertyChangedEventHandler(cellTableViewCell.HandlePropertyChanged);
            cellTableViewCell.PropertyChanged = new Action<object, PropertyChangedEventArgs>(this.HandlePropertyChanged);
            cellTableViewCell.TextLabel.Text = textCell.Text;
            cellTableViewCell.DetailTextLabel.Text = textCell.Detail;
            //cellTableViewCell.TextLabel.TextColor = textCell.TextColor.ToUIColor(TextCellRenderer.DefaultTextColor);
            //cellTableViewCell.DetailTextLabel.TextColor = textCell.DetailColor.ToUIColor(TextCellRenderer.DefaultDetailColor);
            base.UpdateBackground(cellTableViewCell, item);


            cellTableViewCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            return cellTableViewCell;
        }
    }
}

