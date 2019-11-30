using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfLab3.UIObjects;
using static WpfLab3.Events;

namespace WpfLab3
{
    public class MatrixUIObjects: ObservableCollection<MatrixUI> 
    {
        protected override void InsertItem(int index, MatrixUI item)
        {
            base.InsertItem(index, item);
            item.ItemEndEdit += new ItemEndEditEventHandler(ItemEndEditHandler);
        }
        
        void ItemEndEditHandler(IEditableObject sender)
        {
            if (ItemEndEdit != null)
            {
                ItemEndEdit(sender);
            }
        }
        public event ItemEndEditEventHandler ItemEndEdit;


    }
}
