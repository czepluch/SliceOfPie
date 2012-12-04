using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public abstract class IFileModel {

        /// <summary>
        /// Gets all projects for a user.
        /// </summary>
        /// <param name="userMail">Email of user.</param>
        /// <returns>An enumerable of all the projects belonging to the user.</returns>
        public abstract IEnumerable<Project> GetProjects(string userMail);

        /// <summary>
        /// Add project to system.
        /// </summary>
        /// <param name="title">Name of the project</param>
        /// <param name="db">Used to indicate whether the project is retrieved from database or created
        /// from scratch. If set to true, the object is added to the model from the database, if set to
        /// false, it is created and then added to the model. This should only ever be set when loading
        /// files at launch.</param>
        /// <returns>The newly created project</returns>
        public abstract Project AddProject(string title, bool db = false);

        /// <summary>
        /// Removes a project from the model.
        /// </summary>
        /// <param name="p">Project to be removed</param>
        public abstract void RemoveProject(Project p);

        /// <summary>
        /// Adds a folder to the model.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public abstract Folder AddFolder(IItemContainer parent, string title, bool db = false);
        public abstract void RemoveFolder(Folder f);
        public abstract Document AddDocument(IItemContainer parent, string title, bool db = false);
        public abstract void SaveDocument(Document d);
        public abstract void RemoveDocument(Document d);
    }
}