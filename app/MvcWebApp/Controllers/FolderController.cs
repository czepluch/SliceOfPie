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

        public ActionResult Create() {
            return View();
        }

        //
        // POST: /Folder/Create

        [HttpPost]
        public ActionResult Create(Folder folder, Project p) {
            if (ModelState.IsValid){
                controller.CreateFolder(folder.Title, User.Identity.Name, folder.Parent);
                return RedirectToAction("Index");
            }
                return View(p);
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
        public ActionResult DeleteConfirmed(Folder f) {
            controller.RemoveFolder(f);
            if (f == null) {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }
    }
}
