using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SliceOfPie;

namespace MvcWebApp.Controllers {
    public class FolderController : System.Web.Mvc.Controller {
        private SliceOfPie.Controller controller;

        public FolderController() {
            SliceOfPie.Controller.IsWebController = true;
            controller = SliceOfPie.Controller.Instance;
        }
        //
        // GET: /Folder/

        public ActionResult Index(int id) {
            return View(controller.GetFolderDirectly(id));
        }

        //
        // GET: /Folder/Create
        [HttpGet]
        public ActionResult CreateInFolder(Folder f) {
            return View(f);
        }

        //
        // POST: /Folder/Create

        [HttpPost]
        public ActionResult CreateInFolder(Folder newFolder, string text) {
            if (ModelState.IsValid) {
                Folder parent = controller.GetFolderDirectly((int)newFolder.FolderId);
                Folder result = controller.CreateFolder(newFolder.Title, User.Identity.Name, parent);
                return RedirectToAction("Index", result);
            }
            return View();
        }

        //
        // GET: /Folder/Create
        [HttpGet]
        public ActionResult CreateInProject(Folder f) {
            return View(f);
        }

        //
        // POST: /Folder/Create

        [HttpPost]
        public ActionResult CreateInProject(Folder newFolder, string text) {
            if (ModelState.IsValid) {
                Project parent = controller.GetProjectDirectly((int)newFolder.ProjectId);
                Folder result = controller.CreateFolder(newFolder.Title, User.Identity.Name, parent);
                return RedirectToAction("Index", "Folder", result);
            }
            return View();
        }

        //
        // GET: /Folder/Delete/5

        public ActionResult Delete(Folder f) {
            if (f == null) {
                return HttpNotFound();
            }
            return View(f);
        }

        //
        // POST: /Folder/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id) {
            controller.RemoveFolder(controller.GetFolderDirectly(id));
            return RedirectToAction("Overview","Project");
        }
    }
}
