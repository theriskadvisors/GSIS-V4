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
    public class StudentDiscountsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: StudentDiscounts
        //public ActionResult Index()
        //{
        //    var studentDiscounts = db.StudentDiscounts.Include(s => s.AspNetStudent).Include(s => s.FeeDiscount);
        //    return View(studentDiscounts.ToList());
        //}
        public ActionResult StudentDiscountIndex()
        {
            return View();

        }
        public ActionResult GetDiscount()
        {

            var durationlist = db.StudentDiscounts.ToList();
            List<_Discount> duratintype = new List<_Discount>();
            foreach (var item in durationlist)
            {
                var student = db.AspNetStudents.Where(x => x.Id == item.StudentId).FirstOrDefault();
                var feediscount = db.FeeDiscounts.Where(x => x.Id == item.FeeDiscountId).FirstOrDefault();
             
                _Discount dt = new  _Discount();
                dt.Id = item.Id;
                dt.Name = student.Name;
                dt.Discount = feediscount.Name;
                dt.Month = item.Month;
                duratintype.Add(dt);
            }
            return Json(duratintype, JsonRequestBehavior.AllowGet);
        }
        // GET: StudentDiscounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentDiscount studentDiscount = db.StudentDiscounts.Find(id);
            if (studentDiscount == null)
            {
                return HttpNotFound();
            }
            return View(studentDiscount);
        }

        // GET: StudentDiscounts/Create
        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name");
            ViewBag.FeeDiscountId = new SelectList(db.FeeDiscounts, "Id", "Name");
            return View();
        }

        // POST: StudentDiscounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FeeDiscountId,StudentId")] StudentDiscount studentDiscount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Month = Request.Form["Month"];
                    studentDiscount.Month = Month;
                    studentDiscount.Status = "Pending";
                    db.StudentDiscounts.Add(studentDiscount);
                    db.SaveChanges();

                    //Student_ChallanForm std_from = db.Student_ChallanForm.Where(x => x.StudentId == studentDiscount.StudentId).FirstOrDefault();
                    //var amount = db.FeeDiscounts.Where(x => x.Id == studentDiscount.FeeDiscountId).Select(x => x.Amount).FirstOrDefault();
                    //std_from.AmountPayable -= amount;
                    //db.SaveChanges();

                    var fee = db.StudentDiscounts.Where(x => x.FeeDiscountId == studentDiscount.FeeDiscountId).Select(x => x.FeeDiscount.Amount).FirstOrDefault();
                    
                    Ledger ledger = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                    ledger.StartingBalance -= fee;
                    ledger.CurrentBalance -= fee;
                    db.SaveChanges();

                    Ledger l = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();
                    l.StartingBalance -= fee;
                    l.CurrentBalance -= fee;
                    db.SaveChanges();
                 
                    return RedirectToAction("StudentDiscountIndex");
                }
            }
            catch
            {
                return RedirectToAction("Create");
            }

            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentDiscount.StudentId);
            ViewBag.FeeDiscountId = new SelectList(db.FeeDiscounts, "Id", "Name", studentDiscount.FeeDiscountId);
            return View(studentDiscount);
        }

        // GET: StudentDiscounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentDiscount studentDiscount = db.StudentDiscounts.Find(id);
            if (studentDiscount == null)
            {
                return RedirectToAction("StudentDiscountIndex");

            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentDiscount.StudentId);
            ViewBag.FeeDiscountId = new SelectList(db.FeeDiscounts, "Id", "Name", studentDiscount.FeeDiscountId);
            return View(studentDiscount);
        }

        // POST: StudentDiscounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FeeDiscountId,StudentId")] StudentDiscount studentDiscount)
        {
            if (ModelState.IsValid)
            {
                var Month = Request.Form["Month"];
                studentDiscount.Month = Month;
                db.Entry(studentDiscount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentDiscountIndex");
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentDiscount.StudentId);
            ViewBag.FeeDiscountId = new SelectList(db.FeeDiscounts, "Id", "Name", studentDiscount.FeeDiscountId);
            return View(studentDiscount);
        }

        // GET: StudentDiscounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentDiscount studentDiscount = db.StudentDiscounts.Find(id);
            if (studentDiscount == null)
            {
                return HttpNotFound();
            }
            return View(studentDiscount);
        }

        // POST: StudentDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentDiscount studentDiscount = db.StudentDiscounts.Find(id);
            db.StudentDiscounts.Remove(studentDiscount);
            db.SaveChanges();
            return RedirectToAction("StudentDiscountIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class _Discount
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Discount { get; set; }
            public string Month { get; set; }
        }
    }
}
