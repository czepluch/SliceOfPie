﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SliceOfPie;

namespace MvcWebApp.Controllers {
    public class DocumentController : System.Web.Mvc.Controller {
        private SliceOfPie.Controller controller;

        public DocumentController() {
            SliceOfPie.Controller.IsWebController = true;
            controller = SliceOfPie.Controller.Instance;
        }

        [ValidateInput(false)] // Allow html rendering
        public ActionResult Edit(Document d) {
            return View(controller.GetDocumentDirectly(d.Id));
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditSave(Document d) {
            controller.SaveDocument(d);
            return View(d.Id);
        }

        public ActionResult Create(Document d) {
            return View(d);
        }

        //
        // POST: /Folder/Create

        [HttpPost, ActionName("Create")]
        public ActionResult CreateCompleted(Document newDocument) {
                //Get parent
                IItemContainer parent;
                if (newDocument.ProjectId != null) {
                    parent = controller.GetProjectDirectly((int)newDocument.ProjectId);
                }
                else {
                    parent = controller.GetFolderDirectly((int)newDocument.FolderId);
                }

                Document result = controller.CreateDocument(newDocument.Title, User.Identity.Name, parent);
                return RedirectToAction("Edit", result);
        }


        public ActionResult Delete(int id) {
            return View();
        }

        //
        // POST: /Folder/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection) {
            try {
                controller.RemoveDocument(controller.GetDocumentDirectly(id));
                return RedirectToAction("Overview","Project");
            }
            catch {
                return View();
            }
        }
    }
}
