using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class ListableItemEventArgs : EventArgs {
        public ListableItemEventArgs(ListableItem item) {
            this.Item = item;
        }
        public ListableItem Item { get; protected set; }
    }
}
