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
    public class AspNetPackagesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetPackages
        public ActionResult Index()
        {
            return View(db.AspNetPackages.ToList());
        }

        // GET: AspNetPackages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPackage aspNetPackage = db.AspNetPackages.Find(id);
            if (aspNetPackage == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPackage);
        }

        // GET: AspNetPackages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title")] AspNetPackage aspNetPackage)
        {
            if (ModelState.IsValid)
            {
                db.AspNetPackages.Add(aspNetPackage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetPackage);
        }

        // GET: AspNetPackages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPackage aspNetPackage = db.AspNetPackages.Find(id);
            if (aspNetPackage == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPackage);
        }

        // POST: AspNetPackages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title")] AspNetPackage aspNetPackage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetPackage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetPackage);
        }

        // GET: AspNetPackages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPackage aspNetPackage = db.AspNetPackages.Find(id);
            if (aspNetPackage == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPackage);
        }

        // POST: AspNetPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetPackage aspNetPackage = db.AspNetPackages.Find(id);
            db.AspNetPackages.Remove(aspNetPackage);
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
