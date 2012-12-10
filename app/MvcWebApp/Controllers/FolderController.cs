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
        public ActionResult Create(FormCollection collection) {
            try {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch {
                return View();
            }
        }

        //
        // GET: /Folder/Delete/5

        public ActionResult Delete(int id) {
            return View();
        }

        //
        // POST: /Folder/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch {
                return View();
            }
        }
    }
}
