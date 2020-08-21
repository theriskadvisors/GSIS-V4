using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    public class Class_SessionController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: Class_Session
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AllClassSession()
        {

            var classSessionList = (from classession in db.Class_Session
                                    join clas in db.AspNetClasses on classession.ClassId equals clas.Id
                                    select new
                                    {
                                        classession.Id,
                                        classession.ClassSessionName,
                                        classession.Start_Date,
                                        classession.End_Date,
                                        clas.Name,

                                    }).ToList();

            return Json(classSessionList, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckClassSessionExist(int ClassId)
        {
            var ClassSesison = db.Class_Session.Where(x => x.ClassId == ClassId).FirstOrDefault();
            var Error = "";

            if (ClassSesison != null)
            {
                Error = "Session is already created of Selected Class";

            }

            return Json(Error, JsonRequestBehavior.AllowGet);

        }
        // GET: Class_Session/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Class_Session/Create
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");

            return View();
        }
        public ActionResult AllClasses()
        {
            //   var ID = User.Identity.GetUserId();
            var Classes = db.AspNetClasses.Select(x => new { x.Id, x.Name }).ToList();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
            return Content(status);
        }

        // POST: Class_Session/Create
        [HttpPost]
        public ActionResult Create(Class_Session classSession)
        {
            try
            {
                db.Class_Session.Add(classSession);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class_Session/Edit/5
        public ActionResult Edit(int id)
        {

            Class_Session ClassSession = db.Class_Session.Where(x => x.Id == id).FirstOrDefault();

            ViewBag.StartDate = ClassSession.Start_Date;
            ViewBag.EndDate = ClassSession.Start_Date;



            var StartDate = Convert.ToDateTime(ClassSession.Start_Date);

            var StartDateInString = StartDate.ToString("yyyy-MM-dd");

            ViewBag.StartDate = StartDateInString;

            //End Date

            var EndDate = Convert.ToDateTime(ClassSession.End_Date);

            var EndDateInString = EndDate.ToString("yyyy-MM-dd");

            ViewBag.EndDate = EndDateInString;

            //  ViewBag.ClassId = new SelectList(db.AspNetClasses,"Id","")

            var Classes = db.AspNetClasses.Select(x => new { x.Id, x.Name });
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", ClassSession.ClassId);



            return View(ClassSession);
        }

        // POST: Class_Session/Edit/5
        [HttpPost]
        public ActionResult Edit(Class_Session ClassSession)
        {
            try
            {

                Class_Session ClassSessionToUpdate = db.Class_Session.Where(x => x.Id == ClassSession.Id).FirstOrDefault();

                ClassSessionToUpdate.ClassSessionName = ClassSession.ClassSessionName;
                ClassSessionToUpdate.Start_Date = ClassSession.Start_Date;
                ClassSessionToUpdate.End_Date = ClassSession.End_Date;

                db.SaveChanges();
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class_Session/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Class_Session/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
