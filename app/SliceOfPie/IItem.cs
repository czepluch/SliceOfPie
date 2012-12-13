using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public interface IItem {
        int Id { get; set; }
        string Title { get; set; }
        IItemContainer Parent { get; set; }
    }
}