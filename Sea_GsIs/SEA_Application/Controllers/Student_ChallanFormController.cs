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
    public class Student_ChallanFormController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: Student_ChallanForm
        //public ActionResult Index()
        //{
        //    var student_ChallanForm = db.Student_ChallanForm.Include(s => s.AspNetStudent).Include(s => s.Class_ChallanForm);
        //    return View(student_ChallanForm.ToList());
        //}
      
        public ActionResult studentChallanform()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");
            return View();
        }
        [HttpGet]
        public JsonResult StudentsByClass(int id)
        {
            var stdnd = db.AspNetStudents.Where(x => x.ClassId == id).Select(x => new { x.Name, x.Id, x.RollNo }).ToList();
            List<Students_ByClass> stdCls = new List<Students_ByClass>();
            foreach (var item in stdnd)
            {
                Students_ByClass std = new Students_ByClass();
                std.id = item.Id;
                std.Name = item.Name;
                std.RollNumber = item.RollNo;
                stdCls.Add(std);
            }

            return Json(stdCls,JsonRequestBehavior.AllowGet);
        }

         public JsonResult GetChallanForm(int studentid)
        {
            challanform challan = new challanform();
            challan.SchoolName = "Global System of Integreted Studies";
            challan.BranchName = "Islamabad";
            challan.ChallanCopy = new List<string>();
            challan.ChallanCopy.Add("Parent Copy");
            challan.ChallanCopy.Add("Bank Copy");
            challan.ChallanCopy.Add("School Copy");
            var classid = db.AspNetStudents.Where(x => x.Id == studentid).Select(x => x.ClassId).FirstOrDefault();
            var feetypes = (from classtype in db.ClassFeeTypes
                            join classfee in db.ClassFees on classtype.ClassFeeId equals classfee.Id
                            where classid == classtype.ClassId
                            select new { classfee.Name, classfee.Amount }).ToList();
            challan.FeeType = new List<FeeTypes>();
            foreach (var item in feetypes)
            {
                FeeTypes stu_pay = new FeeTypes();
                stu_pay.Amount = item.Amount;
                stu_pay.Name = item.Name;
                challan.FeeType.Add(stu_pay);
            }

            var discounttype = (from feedisc in db.FeeDiscounts
                            join std_disc in db.StudentDiscounts on feedisc.Id equals std_disc.FeeDiscountId
                            where studentid == std_disc.StudentId
                            select new { feedisc.Name, feedisc.Amount }).ToList();
            challan.DiscountType = new List<DiscountType>();
            foreach (var item in discounttype)
            {
                DiscountType stu_pay = new DiscountType();
                stu_pay.Amount = item.Amount;
                stu_pay.Name = item.Name;
                challan.DiscountType.Add(stu_pay);
            }

            var penaltytype = (from feepenalty in db.PenaltyFees
                            join stdPenalty in db.StudentPenalties on feepenalty.Id equals stdPenalty.PenaltyId
                            where studentid == stdPenalty.StudentId
                            select new { feepenalty.Name, feepenalty.Amount }).ToList();
            challan.PenaltyType = new List<PenaltyTypes>();
            foreach (var item in penaltytype)
            {
                PenaltyTypes stu_pay = new  PenaltyTypes();
                stu_pay.Amount = item.Amount;
                stu_pay.Name = item.Name;
                challan.PenaltyType.Add(stu_pay);
            }
            var classchallan = db.Class_ChallanForm.Where(x => x.ClassId == classid).FirstOrDefault();
            challan.DueDate = classchallan.StartDate;
            challan.ValidDate = classchallan.EndDate;
            var student = db.AspNetStudents.Where(x => x.Id == studentid).FirstOrDefault();
            challan.StudentName = student.Name;
            challan.StudentUserName = student.RollNo;
            challan.StudentClass = student.AspNetClass.Name;
            challan.UserID = student.UserId;
            var Session = db.AspNetSessions.Where(x => x.StatusId == 1).FirstOrDefault();
            challan.AcademicSessionStart = Session.StartDate;
            challan.AcademicSessionEnd = Session.EndDate;
            var studentchallan = db.Student_ChallanForm.Where(x => x.StudentId == studentid).FirstOrDefault();
            challan.TotalAmount = studentchallan.AmountPayable;
            challan.TutionFee = studentchallan.TutionFee;
            return Json(challan,JsonRequestBehavior.AllowGet);
        }
        public class challanform
        {
            public string SchoolName { get; set; }
            public string BranchName { get; set; }
            public List<string> ChallanCopy { get; set; }
            public DateTime? AcademicSessionStart { get; set; }
            public DateTime? AcademicSessionEnd { get; set; }
           // public int? ChallanID { get; set; }
            public string UserID { get; set; }
            public string StudentName { get; set; }
            public string StudentUserName { get; set; }
            public string StudentClass { get; set; }
            public List<FeeTypes> FeeType { get; set; }
            public List<DiscountType> DiscountType { get; set; }
            public List<PenaltyTypes> PenaltyType { get; set; }
            //public List<String> DiscountNotes { get; set; }
            public DateTime? DueDate { get; set; }
           // public List<String> Notes { get; set; }
           // public DateTime PrintedDate { get; set; }
            public decimal? TotalAmount { get; set; }
            public decimal? TutionFee { get; set; }
            // public int? Penalty { get; set; }
            public DateTime? ValidDate { get; set; }
          //  public int? Previous { get; set; }
        }
        public class FeeTypes
        {
            public decimal? Amount { get; set; }
            public string Name { get; set; }
        }
        public class DiscountType
        {
            public decimal? Amount { get; set; }
            public string Name { get; set; }
        }
        public class PenaltyTypes
        {
            public decimal? Amount { get; set; }
            public string Name { get; set; }
        }
        // GET: Student_ChallanForm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_ChallanForm student_ChallanForm = db.Student_ChallanForm.Find(id);
            if (student_ChallanForm == null)
            {
                return HttpNotFound();
            }
            return View(student_ChallanForm);
        }

        // GET: Student_ChallanForm/Create
        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name");
            ViewBag.ClassChallanFormId = new SelectList(db.Class_ChallanForm, "Id", "Title");
            return View();
        }

        // POST: Student_ChallanForm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClassChallanFormId,StudentId,AmountPayable")] Student_ChallanForm student_ChallanForm)
        {
            if (ModelState.IsValid)
            {
                db.Student_ChallanForm.Add(student_ChallanForm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", student_ChallanForm.StudentId);
            ViewBag.ClassChallanFormId = new SelectList(db.Class_ChallanForm, "Id", "Title", student_ChallanForm.ClassChallanFormId);
            return View(student_ChallanForm);
        }
        
        // GET: Student_ChallanForm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_ChallanForm student_ChallanForm = db.Student_ChallanForm.Find(id);
            if (student_ChallanForm == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", student_ChallanForm.StudentId);
            ViewBag.ClassChallanFormId = new SelectList(db.Class_ChallanForm, "Id", "Title", student_ChallanForm.ClassChallanFormId);
            return View(student_ChallanForm);
        }

        // POST: Student_ChallanForm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClassChallanFormId,StudentId,AmountPayable")] Student_ChallanForm student_ChallanForm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student_ChallanForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", student_ChallanForm.StudentId);
            ViewBag.ClassChallanFormId = new SelectList(db.Class_ChallanForm, "Id", "Title", student_ChallanForm.ClassChallanFormId);
            return View(student_ChallanForm);
        }

        // GET: Student_ChallanForm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_ChallanForm student_ChallanForm = db.Student_ChallanForm.Find(id);
            if (student_ChallanForm == null)
            {
                return HttpNotFound();
            }
            return View(student_ChallanForm);
        }

        // POST: Student_ChallanForm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student_ChallanForm student_ChallanForm = db.Student_ChallanForm.Find(id);
            db.Student_ChallanForm.Remove(student_ChallanForm);
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

     
        public class Students_ByClass
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string RollNumber { get; set; }
        }
    }
}
