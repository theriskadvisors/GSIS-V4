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
    public class AspNetClassesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetClasses
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetClasses()
        {
            List<AspNetClass> classes = new List<AspNetClass>();
            var result = db.AspNetClasses.ToList();
            foreach (var item in result)
            {
                AspNetClass c = new AspNetClass();
                c.Id = item.Id;
                c.Name = item.Name;
                classes.Add(c);
            }
            return Json(classes,JsonRequestBehavior.AllowGet);
        }
        // GET: AspNetClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetClass aspNetClass = db.AspNetClasses.Find(id);
            if (aspNetClass == null)
            {
                return HttpNotFound();
            }
            return View(aspNetClass);
        }

        // GET: AspNetClasses/Create
        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name");
            ViewBag.NextClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.SectionId = new SelectList(db.AspNetSections, "Id", "Name");
            return View();
        }

        // POST: AspNetClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsActive,NextClassId,SectionId,BranchId,ClassName")] AspNetClass aspNetClass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AspNetClasses.Add(aspNetClass);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
           catch(Exception e)
            {
                return RedirectToAction("Create");
            }

            ViewBag.NextClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetClass.NextClassId);
            return View(aspNetClass);
        }
        public JsonResult GetSectionName(int section)
        {
            var result = db.AspNetSections.Where(x => x.Id == section).Select(x => x.Name).FirstOrDefault();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        // GET: AspNetClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetClass aspNetClass = db.AspNetClasses.Find(id);
            if (aspNetClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.NextClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetClass.NextClassId);
            return View(aspNetClass);
        }

        // POST: AspNetClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsActive,NextClassId,SectionId,BranchId,ClassName")] AspNetClass aspNetClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NextClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetClass.NextClassId);
            return View(aspNetClass);
        }

        // GET: AspNetClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetClass aspNetClass = db.AspNetClasses.Find(id);
            if (aspNetClass == null)
            {
                return HttpNotFound();
            }
            return View(aspNetClass);
        }

        // POST: AspNetClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetClass aspNetClass = db.AspNetClasses.Find(id);
            db.AspNetClasses.Remove(aspNetClass);
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
