using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class Controller {
        
        private static Controller _instance = new Controller();
        public static Controller Instance {
            get {
                return _instance;
            }
        }

        private FileModel fileModel;

        private Controller() {
            fileModel = new LocalFileModel();
        }

        public CreateProject(String name, String userMail) {
            return new Project();
        }

        public boolean SaveProject(Project p) {
            return true;
        }

        
        /// <summary>
        /// Get An IEnumerable of the project associated with the user denoted by userId
        /// (defaults to User.Local as a convenience for the local client)
        /// </summary>
        /// <returns>returns project related to user.</returns>
        public IEnumerable<Project> GetProjects(int userId = 42) {
            foreach (Project p in fileModel.GetProjects(userId)) {
                yield return p;
            }
        }
    }
}