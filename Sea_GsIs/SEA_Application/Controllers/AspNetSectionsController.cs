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
    public class AspNetSectionsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetSections
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetSection()
        {
            var sections = db.AspNetSections.ToList();
            List<AspNetSection> AspNetsection = new List<AspNetSection>();
            foreach (var item in sections)
            {
                AspNetSection sec = new AspNetSection();
                sec.Id = item.Id;
                sec.Name = item.Name;
                AspNetsection.Add(sec);
            }
            return Json(AspNetsection,JsonRequestBehavior.AllowGet);
        }
        // GET: AspNetSections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetSection aspNetSection = db.AspNetSections.Find(id);
            if (aspNetSection == null)
            {
                return HttpNotFound();
            }
            return View(aspNetSection);
        }

        // GET: AspNetSections/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetSections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AspNetSection aspNetSection)
        {
            if (ModelState.IsValid)
            {
                db.AspNetSections.Add(aspNetSection);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetSection);
        }

        // GET: AspNetSections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetSection aspNetSection = db.AspNetSections.Find(id);
            if (aspNetSection == null)
            {
                return HttpNotFound();
            }
            return View(aspNetSection);
        }

        // POST: AspNetSections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AspNetSection aspNetSection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetSection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetSection);
        }

        // GET: AspNetSections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetSection aspNetSection = db.AspNetSections.Find(id);
            if (aspNetSection == null)
            {
                return HttpNotFound();
            }
            return View(aspNetSection);
        }

        // POST: AspNetSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetSection aspNetSection = db.AspNetSections.Find(id);
            db.AspNetSections.Remove(aspNetSection);
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
