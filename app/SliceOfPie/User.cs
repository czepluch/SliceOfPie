using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    partial class User {
        private static User _local = new User();

        public User Local {
            get {
                return _local;
            }
        }
    }
}
