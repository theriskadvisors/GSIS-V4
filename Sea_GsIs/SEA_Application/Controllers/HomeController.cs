//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.Owin;
//using Owin;
//using SEA_Application.Models;        
//using System.Web.Optimization;
//using System;

namespace SEA_Application.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Calander(string callback ,int TaskID, string OwnerID, string Title, string Description, string StartTimezone, string Start, string End, string EndTimezone, string RecurrenceRule, string RecurrenceID, string RecurrenceException, string IsAllDay)
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