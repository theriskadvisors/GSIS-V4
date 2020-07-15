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

    public class AspNetEmployeePositionsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetEmployeePositions
        public ActionResult Index()
        {
            return View(db.AspNetEmployeePositions.ToList());
        }

        // GET: AspNetEmployeePositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetEmployeePosition aspNetEmployeePosition = db.AspNetEmployeePositions.Find(id);
            if (aspNetEmployeePosition == null)
            {
                return HttpNotFound();
            }
            return View(aspNetEmployeePosition);
        }

        // GET: AspNetEmployeePositions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetEmployeePositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PositionName")] AspNetEmployeePosition aspNetEmployeePosition)
        {
            if (ModelState.IsValid)
            {
                db.AspNetEmployeePositions.Add(aspNetEmployeePosition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetEmployeePosition);
        }

        // GET: AspNetEmployeePositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetEmployeePosition aspNetEmployeePosition = db.AspNetEmployeePositions.Find(id);
            if (aspNetEmployeePosition == null)
            {
                return HttpNotFound();
            }
            return View(aspNetEmployeePosition);
        }

        // POST: AspNetEmployeePositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PositionName")] AspNetEmployeePosition aspNetEmployeePosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetEmployeePosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetEmployeePosition);
        }

        // GET: AspNetEmployeePositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetEmployeePosition aspNetEmployeePosition = db.AspNetEmployeePositions.Find(id);
            if (aspNetEmployeePosition == null)
            {
                return HttpNotFound();
            }
            return View(aspNetEmployeePosition);
        }

        // POST: AspNetEmployeePositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetEmployeePosition aspNetEmployeePosition = db.AspNetEmployeePositions.Find(id);
            db.AspNetEmployeePositions.Remove(aspNetEmployeePosition);
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
