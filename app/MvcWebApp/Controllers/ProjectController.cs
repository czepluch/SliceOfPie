using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SliceOfPie;
using System.Data;
using System.Data.Entity;

namespace MvcWebApp.Controllers
{
    public class ProjectController : System.Web.Mvc.Controller
    {
        private SliceOfPie.Controller controller;

        public ProjectController()
        {
            SliceOfPie.Controller.IsWebController = true;
            controller = SliceOfPie.Controller.Instance;
        }

        //
        // GET: /Project/

        public ActionResult Overview() {
            if (Request.IsAuthenticated)
                return View(controller.GetProjects(User.Identity.Name).ToList());
            else
                return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Project/Index
        public ActionResult Index(int id)
        {
            if (Request.IsAuthenticated)
                return View(controller.GetProjectDirectly(id));
            else
                return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Project/Create

        [HttpPost]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                Project result = controller.CreateProject(project.Title, User.Identity.Name);
                return RedirectToAction("Index", result);
            }

            return View(project);
        }

        //
        // GET: /Project/Delete/1

        public ActionResult Delete(Project p) {
            if (p == null) {
                return HttpNotFound();
            }
            return View(p);
        }

        //
        // POST: /Projects/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Project p) {
            controller.RemoveProject(p);
            if (p == null) {
                return HttpNotFound();
            }
            return RedirectToAction("Overview");
        }

        //
        // GET: /Project/Share/5

        public ActionResult Share(Project p) {
            return View(p);
        }

        //
        // POST: /Project/ShareComplete

        [HttpPost, ActionName("Share")]
        public ActionResult ShareComplete(Project p, string emailsAsString) {
            controller.ShareProject(p, emailsAsString);
            return RedirectToAction("Overview");
        }
    }
}
