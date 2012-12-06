﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcWebApp.Models;

namespace MvcWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the Slice of Pie online file sharer and editor";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Editor()
        {
            return View(new Revision());
        }

    }
}
