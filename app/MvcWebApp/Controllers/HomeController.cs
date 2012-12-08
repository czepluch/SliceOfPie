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

        //public ActionResult Index()
        //{
        //    Project p = controller.GetProjects("me@michaelstorgaard.com").Last();
        //    ViewBag.Id = p.Id;
        //    ViewBag.Message = p.Title;
            

        //    return View();
        //}

        public ViewResult Index()
        { 
            return View(controller.GetProjects("me@michaelstorgaard.com").ToList());
        }

        public ActionResult About()
        {
            Project p = controller.GetProjects("me@michaelstorgaard.com").Last();
            ViewBag.Id = p.Id;
            ViewBag.Message = p.Title;

            return View();
        }

        public ActionResult Project()
        {
            Project p = controller.GetProjects("me@michaelstorgaard.com").First();
            ViewBag.Project = p.Title;


            return View();
        }
    }
}
