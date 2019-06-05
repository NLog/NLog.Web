using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Web.AspNetCore2.Example.Models;

namespace NLog.Web.AspNetCore2.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //NLog: inject logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Hello, this is the index!");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact(bool posted = false)
        {
            ViewData["Message"] = "Your contact page.";
            ViewData["Posted"] = posted;

            return View(new MessageModel() { From = "me", Message = "My Message"});
        }

        [HttpPost]
        public IActionResult PostMessage([FromForm] MessageModel messageModel)
        {
            _logger.LogInformation("Posted an message");
            return RedirectToAction("Contact", new { posted = true });
        }

        public IActionResult Error()
        {
            _logger.LogError("Ow noos! an error");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
