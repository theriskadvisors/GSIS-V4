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
    public class ClassFeesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: ClassFees
        //public ActionResult Index()
        //{
        //    return View(db.ClassFees.ToList());
        //}
        public ActionResult ClassFeeIndex()
        {
            return View();
        }
        public ActionResult GetClassFee()
        {
            var durationlist = db.ClassFees.ToList();
            List<ClassFee> duratintype = new List<ClassFee>();
            foreach (var item in durationlist)
            {
                ClassFee dt = new ClassFee();
                //dt.Name = item.Name;
                //dt.Amount = item.Amount;
                duratintype.Add(dt);
            }
            return Json(duratintype, JsonRequestBehavior.AllowGet);
        }
        // GET: ClassFees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFee classFee = db.ClassFees.Find(id);
            if (classFee == null)
            {
                return HttpNotFound();
            }
            return View(classFee);
        }

        // GET: ClassFees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClassFees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Amount")] ClassFee classFee)
        {
            if (ModelState.IsValid)
            {
                db.ClassFees.Add(classFee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(classFee);
        }

        // GET: ClassFees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFee classFee = db.ClassFees.Find(id);
            if (classFee == null)
            {
                return HttpNotFound();
            }
            return View(classFee);
        }

        // POST: ClassFees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Amount")] ClassFee classFee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classFee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classFee);
        }

        // GET: ClassFees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFee classFee = db.ClassFees.Find(id);
            if (classFee == null)
            {
                return HttpNotFound();
            }
            return View(classFee);
        }

        // POST: ClassFees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassFee classFee = db.ClassFees.Find(id);
            db.ClassFees.Remove(classFee);
            db.SaveChanges();
            return RedirectToAction("Index");
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
