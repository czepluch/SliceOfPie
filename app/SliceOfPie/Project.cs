using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    partial class Project:ItemContainer {
        public IEnumerable<Item> ListItems() {
            List<Item> items = new List<Item>();

            items.Add(new Folder());
            items.Add(new Document());

            foreach (Item item in items) {
                yield return item;
            }
        }
    }
}
