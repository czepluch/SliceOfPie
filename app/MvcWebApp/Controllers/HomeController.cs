using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using MvcWebApp.Models;
using SliceOfPie;
using System.Data;
using System.Data.Entity;

namespace MvcWebApp.Controllers
{
    public class HomeController : System.Web.Mvc.Controller
    {
        private SliceOfPie.Controller controller;
        public String mail = "me@michaelstorgaard.com";

        public HomeController()
        {
            SliceOfPie.Controller.IsWebController = true;
            controller = SliceOfPie.Controller.Instance;
        }

        //
        // GET: /Home(Projects)/

        public ViewResult Index()
        {
            return View(controller.GetProjects(mail).ToList());
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
                controller.CreateProject(project.Title, "me@michaelstorgaard.com"); //Might need correction.
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
