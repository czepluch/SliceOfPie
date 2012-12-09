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

        public HomeController()
        {
            SliceOfPie.Controller.IsWebController = true;
            controller = SliceOfPie.Controller.Instance;
        }

        //
        // GET: /Home(Projects)/

        public ViewResult Index()
        {
            return View(controller.GetProjects("me@michaelstorgaard.com").ToList());
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
        public ActionResult Create(SliceOfPie.Project project)
        {
            if (ModelState.IsValid)
            {
                controller.CreateProject("Lolpopz", "me@michaelstorgaard.com"); //Might need correction.
                //controller.SaveChanges(); //No such method. Should there be?
                return RedirectToAction("Index");
            }

            return View(project);
        }

        ////
        //// GET: /Home(Projects)/Delete/1

        //public ActionResult Delete(int id = 0)
        //{
        //    Project project = controller.Projects.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        ////
        //// POST: /Home(Projects)/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id = 0)
        //{
        //    Project project = controller.Projects.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    controller.Projects.Remove(project);
        //    controller.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        ////
        //// GET: /Home(Projects)/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
        //    Project project = controller.Projects.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        ////
        //// POST: /Home(Projects)/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Project project)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        controller.Entry(project).State = EntityState.Modified;
        //        controller.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(project);
        //}
    }
}
