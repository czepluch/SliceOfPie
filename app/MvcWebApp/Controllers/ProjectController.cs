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
        // GET: /Home(Projects)/

        public ActionResult Overview() {
            if (Request.IsAuthenticated)
                return View(controller.GetProjects(User.Identity.Name).ToList());
            else
                return RedirectToAction("LogOn", "Account");
            //return View(controller.Projects.ToList());
        }

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
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                controller.CreateProject(project.Title, User.Identity.Name);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        //
        // GET: /Home(Projects)/Delete/1

        public ActionResult Delete(Project p) {
            if (p == null) {
                return HttpNotFound();
            }
            return View(p);
        }

        //
        // POST: /Home(Projects)/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Project p) {
            controller.RemoveProject(p);
            if (p == null) {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }
    }
}
