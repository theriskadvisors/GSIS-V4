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
    public class StudentPenaltiesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: StudentPenalties
        //public ActionResult Index()
        //{
        //    var studentPenalties = db.StudentPenalties.Include(s => s.AspNetStudent).Include(s => s.PenaltyFee);
        //    return View(studentPenalties.ToList());
        //}
        public ActionResult StudentPaneltiesIndex()
        {
            return View();
        }
        public ActionResult GetPenalty()
        {
            var durationlist = db.StudentPenalties.ToList();
            List<Student_Penalty> duratintype = new List<Student_Penalty>();
            foreach (var item in durationlist)
            {
                var name = db.AspNetStudents.Where(x => x.Id == item.StudentId).FirstOrDefault();
                var penaltyname = db.PenaltyFees.Where(x => x.Id == item.PenaltyId).FirstOrDefault();
                Student_Penalty dt = new  Student_Penalty();
                dt.StudentName = name.Name;
                dt.PenaltyName = penaltyname.Name;
                duratintype.Add(dt);
            }
            return Json(duratintype, JsonRequestBehavior.AllowGet);
        }
        // GET: StudentPenalties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentPenalty studentPenalty = db.StudentPenalties.Find(id);
            if (studentPenalty == null)
            {
                return HttpNotFound();
            }
            return View(studentPenalty);
        }

        // GET: StudentPenalties/Create
        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name");
            ViewBag.PenaltyId = new SelectList(db.PenaltyFees, "Id", "Name");
            return View();
        }

        // POST: StudentPenalties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StudentId,PenaltyId")] StudentPenalty studentPenalty)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    studentPenalty.Status = "Pending";
                    db.StudentPenalties.Add(studentPenalty);
                    db.SaveChanges();

                    //Student_ChallanForm std_from = db.Student_ChallanForm.Where(x => x.StudentId == studentPenalty.StudentId).FirstOrDefault();
                    //var amount = db.PenaltyFees.Where(x => x.Id == studentPenalty.PenaltyId).Select(x => x.Amount).FirstOrDefault();
                    //std_from.AmountPayable += amount;
                    //db.SaveChanges();

                    return RedirectToAction("StudentPaneltiesIndex");
                }
            }
            catch
            {
                return RedirectToAction("Create");
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentPenalty.StudentId);
            ViewBag.PenaltyId = new SelectList(db.PenaltyFees, "Id", "Name", studentPenalty.PenaltyId);
            return View(studentPenalty);
        }

        // GET: StudentPenalties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentPenalty studentPenalty = db.StudentPenalties.Find(id);
            if (studentPenalty == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentPenalty.StudentId);
            ViewBag.PenaltyId = new SelectList(db.PenaltyFees, "Id", "Name", studentPenalty.PenaltyId);
            return View(studentPenalty);
        }

        // POST: StudentPenalties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StudentId,PenaltyId")] StudentPenalty studentPenalty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentPenalty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentPaneltiesIndex");
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentPenalty.StudentId);
            ViewBag.PenaltyId = new SelectList(db.PenaltyFees, "Id", "Name", studentPenalty.PenaltyId);
            return View(studentPenalty);
        }

        // GET: StudentPenalties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentPenalty studentPenalty = db.StudentPenalties.Find(id);
            if (studentPenalty == null)
            {
                return HttpNotFound();
            }
            return View(studentPenalty);
        }

        // POST: StudentPenalties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentPenalty studentPenalty = db.StudentPenalties.Find(id);
            db.StudentPenalties.Remove(studentPenalty);
            db.SaveChanges();
            return RedirectToAction("StudentPaneltiesIndex");
        }
      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class Student_Penalty
        {
            public string StudentName { get; set; }
            public string PenaltyName { get; set; }
         

        }
    }
}
