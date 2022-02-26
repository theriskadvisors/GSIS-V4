using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SEA_Application.Controllers
{
    public class AccountHeadController : Controller
    {
        // GET: AccountHead

        Sea_Entities db = new Sea_Entities();


        public ActionResult Dashboard()
        {
            var ID = User.Identity.GetUserId();
            var branchID = db.AspNetBranch_Admins.Where(x => x.AdminId == ID).Select(x => x.BranchId).FirstOrDefault();
            var Classes1 = db.AspNetBranch_Class.Where(x => x.BranchId == branchID).Select(x => x.AspNetClass.Name).Distinct().ToList();

            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
            classes.Add("In Process");


            ViewBag.AllClasses = classes;

            return View("BlankPage");
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}