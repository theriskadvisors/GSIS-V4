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
    public class AspNetAssessmentsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetAssessments
        public ActionResult Index()
        {
            var aspNetAssessments = db.AspNetAssessments.Include(a => a.AspNetAssessmentType).Include(a => a.AspNetTerm);
            return View(aspNetAssessments.ToList());
        }

        // GET: AspNetAssessments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAssessment aspNetAssessment = db.AspNetAssessments.Find(id);
            if (aspNetAssessment == null)
            {
                return HttpNotFound();
            }
            return View(aspNetAssessment);
        }

        // GET: AspNetAssessments/Create
        public ActionResult Create()
        {
            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title");
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name");
            return View();
        }

        // POST: AspNetAssessments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AssessmentTypeId,Title,Description,Attachment,TermId,Weightage,Total,DueDate,PostingDate")] AspNetAssessment aspNetAssessment)
        {
            if (ModelState.IsValid)
            {
                db.AspNetAssessments.Add(aspNetAssessment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title", aspNetAssessment.AssessmentTypeId);
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name", aspNetAssessment.TermId);
            return View(aspNetAssessment);
        }

        // GET: AspNetAssessments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAssessment aspNetAssessment = db.AspNetAssessments.Find(id);
            if (aspNetAssessment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title", aspNetAssessment.AssessmentTypeId);
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name", aspNetAssessment.TermId);
            return View(aspNetAssessment);
        }

        // POST: AspNetAssessments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AssessmentTypeId,Title,Description,Attachment,TermId,Weightage,Total,DueDate,PostingDate")] AspNetAssessment aspNetAssessment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetAssessment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title", aspNetAssessment.AssessmentTypeId);
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name", aspNetAssessment.TermId);
            return View(aspNetAssessment);
        }

        // GET: AspNetAssessments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAssessment aspNetAssessment = db.AspNetAssessments.Find(id);
            if (aspNetAssessment == null)
            {
                return HttpNotFound();
            }
            return View(aspNetAssessment);
        }

        // POST: AspNetAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetAssessment aspNetAssessment = db.AspNetAssessments.Find(id);
            db.AspNetAssessments.Remove(aspNetAssessment);
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
