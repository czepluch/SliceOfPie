using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public partial class User {
        public string Mail{
            get;
            set;
        }

        private static User _local = new User();
        public static User Local {
            get {
                return _local;
            }
        }
    }
}
