﻿using System;
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

        private IFileModel fileModel;

        private Controller(IFileModel model = null) {
            if (model == null) model = new LocalFileModel();
            fileModel = model;
        }

        #region Project

        public Project CreateProject(string name, string userMail) {
            return fileModel.AddProject(name);
        }

        public void RemoveProject(Project p) {
            fileModel.RemoveProject(p);
        }

        public void ShareProject(Project p, IEnumerable<string> emails) {
            //nope
        }

        public void SyncProject(Project p) {
            //story cool, yes
        }

        #endregion

        #region Document

        public Document CreateDocument(string name, string userMail, IItemContainer parent) {
            return fileModel.AddDocument(parent, name);
        }

        public void SaveDocument(Document document) {
            fileModel.SaveDocument(document);
        }

        public void RemoveDocument(Document d) {
            fileModel.RemoveDocument(d);
        }

        #endregion

        #region Folder

        public Folder CreateFolder(String name, string userMail, IItemContainer parent) {
            return fileModel.AddFolder(parent, name);
        }

        public void SaveFolder(Folder f) {
            //LOLNOPE
        }

        public void RemoveFolder(Folder f) {
            fileModel.RemoveFolder(f);
        }

        #endregion


        /// <summary>
        /// Get An IEnumerable of the project associated with the user denoted by userId
        /// (defaults to User.Local as a convenience for the local client)
        /// </summary>
        /// <returns>returns project related to user.</returns>
        public IEnumerable<Project> GetProjects(string userMail) {
            foreach (Project p in fileModel.GetProjects(userMail)) {
                yield return p;
            }
        }
    }
}