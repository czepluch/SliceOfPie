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

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return View(controller.GetProjects(User.Identity.Name).ToList());
            else
                return RedirectToAction("LogOn", "Account");
            //return View(controller.Projects.ToList());
        }

        public ActionResult About()
        {
            return View();
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

        ////
        //// GET: /Home(Projects)/Edit/5

        //public ActionResult Edit(Project p) {
        //    controller.RenameProject(p);
        //    if (project == null) {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        ////
        //// POST: /Home(Projects)/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Project project) {
        //    if (ModelState.IsValid) {
        //        controller.Entry(project).State = EntityState.Modified;
        //        controller.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(project);
        //}
    }
}
