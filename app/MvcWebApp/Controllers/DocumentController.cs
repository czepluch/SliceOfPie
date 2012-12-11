using System;
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

        //
        // GET: /Document/Show

        [ValidateInput(false)] // Allow html rendering
        public ActionResult Show(Document d) {
            return View(controller.GetDocumentDirectly(d.Id));
        }

        [ValidateInput(false)] // Allow html rendering
        public ActionResult Edit(Document d) {
            return View(controller.GetDocumentDirectly(d.Id));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)] // Allow html rendering
        public ActionResult EditSave(Document d, string NewContent) {
            d.CurrentRevision = NewContent;
            controller.SaveDocument(d);
            return RedirectToAction("Show", d);
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


        public ActionResult Delete(Document d) {
            return View(d);
        }

        //
        // POST: /Folder/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Document d) {
            controller.RemoveDocument(d);
            return RedirectToAction("Overview","Project");
        }
    }
}
