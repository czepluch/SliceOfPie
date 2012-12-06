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
        public abstract Project AddProject(string title, int id = 0, bool db = false);

        /// <summary>
        /// Removes a project from the model.
        /// </summary>
        /// <param name="p">Project to be removed</param>
        public abstract void RemoveProject(Project p);

        /// <summary>
        /// Adds a folder to the model.
        /// </summary>
        /// <param name="parent">The parent container to place the folder in. All folders must be placed in either
        /// projects or folders.</param>
        /// <param name="title">Name of the folder</param>
        /// <param name="db">Used to indicate whether the folder is retrieved from database or created
        /// from scratch. If set to true, the object is added to the model from the database, if set to
        /// false, it is created and then added to the model. This should only ever be set when loading
        /// files at launch.</param>
        /// <returns>The newly created folder.</returns>
        public abstract Folder AddFolder(IItemContainer parent, string title, int id = 0, bool db = false);

        /// <summary>
        /// Remove the folder from the model
        /// </summary>
        /// <param name="f">Folder to remove</param>
        public abstract void RemoveFolder(Folder f);

        /// <summary>
        /// Add a new document to the model.
        /// </summary>
        /// <param name="parent">Container for the document. All documents must be placed in either folders
        /// or projects.</param>
        /// <param name="title">Name of the document</param>
        /// <param name="db">Used to indicate whether the document is retrieved from database or created
        /// from scratch. If set to true, the object is added to the model from the database, if set to
        /// false, it is created and then added to the model. This should only ever be set when loading
        /// files at launch.</param>
        /// <returns>The newly created document</returns>
        public abstract Document AddDocument(IItemContainer parent, string title, string revision = "", int id = 0, bool db = false);

        /// <summary>
        /// Save the document.
        /// </summary>
        /// <param name="d">Document to save</param>
        public abstract void SaveDocument(Document d);

        /// <summary>
        /// Remove the document from the model
        /// </summary>
        /// <param name="d">Document to remove</param>
        public abstract void RemoveDocument(Document d);

        /// <summary>
        /// Synchronizes all files for the user with the given email so the local and
        /// remote states are the same.
        /// Uploads first, then downloads.
        /// </summary>
        /// <param name="userMail">Email of user</param>
        public abstract void SyncFiles(string userMail);
    }
}