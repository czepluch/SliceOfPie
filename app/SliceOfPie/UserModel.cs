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
            if (!(userMail.Length > 0 && password.Length > 0)) {
                throw new ArgumentException("Email or password cannot be blank");
            }
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

        public void ShareProject(int projectId, string userMail) {
            if (projectId == 0) {
                throw new ArgumentException("Project has to be synced, before it can be shared");
            }
            if (userMail.Length < 1) {
                throw new ArgumentException("User email cannot be blank");
            }
            userMail = userMail.Trim();
            bool userExists = false;
            using (var dbContext = new sliceofpieEntities2()) {
                if (dbContext.Users.Count(dbUser => dbUser.Email.Equals(userMail)) > 0) {
                    userExists = true;
                }
            }
            if (!userExists) {
                throw new ArgumentException("User does not exist");
            }
            bool projectUserExists = true;
            using (var dbContext = new sliceofpieEntities2()) {
                var projectUsers = from projectUser in dbContext.ProjectUsers
                                   where projectUser.ProjectId == projectId && projectUser.UserEmail == userMail
                                   select projectUser;
                if (projectUsers.Count() == 0) {
                    projectUserExists = false;
                }
            }
            if (projectUserExists) {
                throw new ArgumentException("User is already sharing this project");
            }
            using (var dbContext = new sliceofpieEntities2()) {
                ProjectUser projectUser = new ProjectUser();
                projectUser.ProjectId = projectId;
                projectUser.UserEmail = userMail;
                dbContext.ProjectUsers.AddObject(projectUser);
                dbContext.SaveChanges();
            }
        }

    }
}
