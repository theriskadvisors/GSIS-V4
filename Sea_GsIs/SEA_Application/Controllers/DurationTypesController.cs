using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    public class DurationTypesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: DurationTypes
        //public ActionResult Index()
        //{
        //    return View(db.DurationTypes.ToList());
        //}
        public ActionResult DurationIndex()
        {
            return View();
        }
        public ActionResult GetDuration()
        {
           var durationlist= db.DurationTypes.ToList();
            List<DurationType> duratintype = new List<DurationType>();
            foreach (var item in durationlist)
            {
                DurationType dt = new DurationType();
                dt.Id = item.Id;
                dt.Type_Name = item.Type_Name;
                dt.Month_Duration = item.Month_Duration;
                duratintype.Add(dt);
            }
            return Json(duratintype, JsonRequestBehavior.AllowGet);
        }
        // GET: DurationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DurationType durationType = db.DurationTypes.Find(id);
            if (durationType == null)
            {
                return HttpNotFound();
            }
            return View(durationType);
        }

        // GET: DurationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DurationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type_Name,Month_Duration")] DurationType durationType)
        {
            if (ModelState.IsValid)
            {
                db.DurationTypes.Add(durationType);
                db.SaveChanges();
                return RedirectToAction("DurationIndex");
            }

            return View(durationType);
        }

        // GET: DurationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DurationType durationType = db.DurationTypes.Find(id);
            if (durationType == null)
            {
                return HttpNotFound();
            }
            return View(durationType);
        }

        // POST: DurationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type_Name,Month_Duration")] DurationType durationType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(durationType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DurationIndex");
            }
            return View(durationType);
        }

        // GET: DurationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DurationType durationType = db.DurationTypes.Find(id);
            if (durationType == null)
            {
                return HttpNotFound();
            }
            return View(durationType);
        }

        // POST: DurationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DurationType durationType = db.DurationTypes.Find(id);
            db.DurationTypes.Remove(durationType);
            db.SaveChanges();
            return RedirectToAction("DurationIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
