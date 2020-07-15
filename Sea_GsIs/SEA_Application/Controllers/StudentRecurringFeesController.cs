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
    public class StudentRecurringFeesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: StudentRecurringFees
        public ActionResult Index()
        {
            var studentRecurringFees = db.StudentRecurringFees.Include(s => s.AspNetClass);
            return View(studentRecurringFees.ToList());
        }

        // GET: StudentRecurringFees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentRecurringFee studentRecurringFee = db.StudentRecurringFees.Find(id);
            if (studentRecurringFee == null)
            {
                return HttpNotFound();
            }
            return View(studentRecurringFee);
        }

        // GET: StudentRecurringFees/Create

        public ActionResult CheckClassDeplication(int Id)
        {
            string status = "error";
           
           if(db.StudentRecurringFees.Where(x=>x.ClassId == Id).Count() > 0)
            {
                status = "error";
            }
            else
            {
                status = "success";
            }

            return Content(status);
        }

        public ActionResult Create()
        {
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            return View();
        }

        // POST: StudentRecurringFees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClassId,ComputerFee,SecurityServices,LabCharges,Transportation,SportsShirt,ChineseClassFee,AdvanceTax,Other,TutionFee,TotalFee")] StudentRecurringFee studentRecurringFee)
        {
            try
            {


             
                if (ModelState.IsValid)
                {
                    db.StudentRecurringFees.Add(studentRecurringFee);
                    db.SaveChanges();
                    var stdList = db.AspNetStudents.Where(x => x.ClassId == studentRecurringFee.ClassId).ToList().Count();
                    for (int i = 0; i < stdList; i++)
                    {
                        Ledger ledger = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                        ledger.StartingBalance += Convert.ToDecimal(studentRecurringFee.TotalFee);
                        ledger.CurrentBalance += Convert.ToDecimal(studentRecurringFee.TotalFee);
                        db.SaveChanges();

                    }

                    for (int i = 0; i < stdList; i++)
                    {
                        Ledger ledger = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();
                        ledger.StartingBalance += Convert.ToDecimal(studentRecurringFee.TotalFee);
                        ledger.CurrentBalance += Convert.ToDecimal(studentRecurringFee.TotalFee);
                        db.SaveChanges();

                    }

                    //multiplier
                    List<AspNetStudent> list = db.AspNetStudents.Where(x => x.ClassId == studentRecurringFee.ClassId).ToList();
                    foreach (var std in list)
                    {
                        var classid = db.AspNetStudents.Where(x => x.Id == std.Id).Select(x => x.ClassId).FirstOrDefault();
                        var totalfee = db.StudentRecurringFees.Where(x => x.ClassId == std.ClassId).FirstOrDefault();
                        var installment = totalfee.TotalFee / 12;
                        string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                        for (int i = 0; i < Months.Count(); i++)
                        {
                            StudentFeeMonth stdfeemonth = new StudentFeeMonth();
                            stdfeemonth.StudentId = std.Id;
                            stdfeemonth.Status = "Pending";
                            stdfeemonth.InstalmentAmount = installment;
                            stdfeemonth.FeePayable = installment;
                            var dddd = DateTime.Now;
                            var d = dddd.ToString("yyyy-MM-dd");
                            stdfeemonth.IssueDate = dddd;
                            stdfeemonth.Months = Months[i];
                            db.StudentFeeMonths.Add(stdfeemonth);
                            db.SaveChanges();

                        }

                    }


                    return RedirectToAction("Index");
                }
            }
            catch(Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
            }

            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", studentRecurringFee.ClassId);
            return View(studentRecurringFee);
        }

        // GET: StudentRecurringFees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentRecurringFee studentRecurringFee = db.StudentRecurringFees.Find(id);
            if (studentRecurringFee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", studentRecurringFee.ClassId);
            return View(studentRecurringFee);
        }

        // POST: StudentRecurringFees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClassId,ComputerFee,SecurityServices,LabCharges,Transportation,SportsShirt,ChineseClassFee,AdvanceTax,Other,TutionFee,TotalFee")] StudentRecurringFee studentRecurringFee)
        {

            if (ModelState.IsValid)
            {
                db.Entry(studentRecurringFee).State = EntityState.Modified;
                if(db.SaveChanges()>0)
                {



                    List<AspNetStudent> list = db.AspNetStudents.Where(x => x.ClassId == studentRecurringFee.ClassId).ToList();
                    foreach (var std in list)
                    {

                        var tdfee = db.StudentFeeMonths.Where(x => x.StudentId == std.Id ).ToList();
                        foreach (var item in tdfee)
                        {
                            StudentFeeMonth student = db.StudentFeeMonths.Where(x => x.Id == item.Id).FirstOrDefault();
                            db.StudentFeeMonths.Remove(student);
                            db.SaveChanges();
                        }

                        var classid = db.AspNetStudents.Where(x => x.Id == std.Id).Select(x => x.ClassId).FirstOrDefault();
                        var totalfee = db.StudentRecurringFees.Where(x => x.ClassId == std.ClassId).FirstOrDefault();
                        var installment = totalfee.TotalFee / 12;
                        string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                        for (int i = 0; i < Months.Count(); i++)
                        {
                            StudentFeeMonth stdfeemonth = new StudentFeeMonth();
                            stdfeemonth.StudentId = std.Id;
                            stdfeemonth.Status = "Pending";
                            stdfeemonth.InstalmentAmount = installment;
                            stdfeemonth.FeePayable = installment;
                            var dddd = DateTime.Now;
                            var d = dddd.ToString("yyyy-MM-dd");
                            stdfeemonth.IssueDate = dddd;
                            stdfeemonth.Months = Months[i];
                            db.StudentFeeMonths.Add(stdfeemonth);
                            db.SaveChanges();

                        }


                    }

                }



                return RedirectToAction("Index");
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", studentRecurringFee.ClassId);
            return View(studentRecurringFee);
        }

        // GET: StudentRecurringFees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentRecurringFee studentRecurringFee = db.StudentRecurringFees.Find(id);
            if (studentRecurringFee == null)
            {
                return HttpNotFound();
            }
            return View(studentRecurringFee);
        }

        // POST: StudentRecurringFees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentRecurringFee studentRecurringFee = db.StudentRecurringFees.Find(id);
            db.StudentRecurringFees.Remove(studentRecurringFee);
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
