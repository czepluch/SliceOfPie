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

        public ActionResult Create(int id, Type parentType) {
            return View();
        }

        //
        // POST: /Folder/Create

        [HttpPost]
        public ActionResult Create(string folderTitle, string parentType, int parentId) {
            if (ModelState.IsValid){
                var parent = new Folder();
                Folder result = controller.CreateFolder(folderTitle, User.Identity.Name, parent);
                return RedirectToAction("Index", result);
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
