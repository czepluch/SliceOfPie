using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public static class Helper {
        public static string GenerateName(int id, string title) {
            return id + "-" + title;
        }
    }
}
