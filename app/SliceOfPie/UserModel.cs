using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class UserModel {

        /// <summary>
        /// Check whether the provided login details are a valid user. If the user does no exist,
        /// it is created and true is returned. If it does exist and the information is correct,
        /// true is returned. If it does exist and the information is INCORRECT, false is returned.
        /// </summary>
        /// <param name="userMail">User email to validate</param>
        /// <param name="password">Password corresponding to email</param>
        /// <returns>Whether the user is valid</returns>
        public bool ValidateLogin(string userMail, string password) {
            bool userValid;
            using (var dbContext = new sliceofpieEntities2()) {
                if (dbContext.Users.Count(dbUser => dbUser.Email.Equals(userMail)) > 0) { //user exists
                    User u = dbContext.Users.First(dbUser => dbUser.Email.Equals(userMail));
                    if (u.Password.Equals(password)) {
                        userValid = true;
                    }
                    else {
                        userValid = false;
                    }
                }
                else { //create user
                    dbContext.Users.AddObject(new User() {
                        Email = userMail,
                        Password = password
                    });
                    dbContext.SaveChanges();
                    userValid = true;
                }
            }
            return userValid;
        }

    }
}
