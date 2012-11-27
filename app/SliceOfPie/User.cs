using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    partial class User {

        public User(string mail) {
            this.Mail = mail;
        }

        public string Mail{
            get;
            set;
        }

        private static User _local = new User("x@x.com");
        public static User Local {
            get {
                return _local;
            }
        }
    }
}
