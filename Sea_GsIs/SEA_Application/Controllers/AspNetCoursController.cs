using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class AspNetCoursController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetCours
        public ActionResult Index()
        {
            //var aspNetCourses = db.AspNetCourses.Include(a => a.AspNetDepartment);
            return View(db.AspNetCourses.ToList());
        }

        // GET: AspNetCours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCours aspNetCours = db.AspNetCourses.Find(id);
            if (aspNetCours == null)
            {
                return HttpNotFound();
            }
            return View(aspNetCours);
        }

        // GET: AspNetCours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetCours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AspNetCours aspNetCours)
        {
            if (ModelState.IsValid)
            {
                db.AspNetCourses.Add(aspNetCours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.AspNetDepartments, "Id", "Name");
            return View(aspNetCours);
        }

        // GET: AspNetCours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCours aspNetCours = db.AspNetCourses.Find(id);
            if (aspNetCours == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.AspNetDepartments, "Id", "Name");
            return View(aspNetCours);
        }

        // POST: AspNetCours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AspNetCours aspNetCours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetCours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.AspNetDepartments, "Id", "Name");
            return View(aspNetCours);
        }

        // GET: AspNetCours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCours aspNetCours = db.AspNetCourses.Find(id);
            if (aspNetCours == null)
            {
                return HttpNotFound();
            }
            return View(aspNetCours);
        }

        // POST: AspNetCours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetCours aspNetCours = db.AspNetCourses.Find(id);
            db.AspNetCourses.Remove(aspNetCours);
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
