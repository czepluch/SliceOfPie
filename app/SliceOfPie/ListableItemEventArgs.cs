using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    /// <summary>
    /// Event arguments which can hold a ListableItem
    /// </summary>
    public class ListableItemEventArgs : EventArgs {
        
        /// <summary>
        /// Constructs the event arguments based on a ListableItem
        /// </summary>
        /// <param name="item">The Listable item to associate with the event</param>
        public ListableItemEventArgs(IListableItem item) {
            this.Item = item;
        }
        /// <summary>
        /// This is the ListableItem associated with the event
        /// </summary>
        public IListableItem Item { get; protected set; }
    }
}
