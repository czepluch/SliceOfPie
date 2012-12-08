using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcWebApp.Models;
using SliceOfPie;

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

        public ActionResult Index()
        {
            Project p = controller.GetProjects("me@michaelstorgaard.com").First(); 
            ViewBag.Message = p.Title;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
