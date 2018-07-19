using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NLog.Web.AspNet461.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Hello World");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}