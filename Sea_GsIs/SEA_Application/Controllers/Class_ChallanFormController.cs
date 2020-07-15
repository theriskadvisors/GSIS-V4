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
    public class Class_ChallanFormController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: Class_ChallanForm
        //public ActionResult Index()
        //{
        //    var class_ChallanForm = db.Class_ChallanForm.Include(c => c.AspNetClass).Include(c => c.DurationType);
        //    return View(class_ChallanForm.ToList());
        //}
        public ActionResult ClassChallanIndex()
        {
            return View();
        }
        public ActionResult GetChallan()
        {
            var challan = db.Class_ChallanForm.ToList();
            List<Class_Challan> classchallan = new List<Class_Challan>();
            foreach (var item in challan)
            {
                var classname = db.AspNetClasses.Where(x => x.Id == item.ClassId).Select(x => x.Name).FirstOrDefault();
                var durationtype = db.DurationTypes.Where(x => x.Id == item.DurationId).Select(x => x.Type_Name).FirstOrDefault();
                Class_Challan ch = new Class_Challan();
                ch.ClassName = classname;
                ch.Duration = durationtype;
                ch.StartDate = item.StartDate;
                ch.EndDate = item.EndDate;
                ch.Total = item.TotalAmount;
                classchallan.Add(ch);

            }
            return Json(classchallan, JsonRequestBehavior.AllowGet);
        }
        // GET: Class_ChallanForm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ChallanForm class_ChallanForm = db.Class_ChallanForm.Find(id);
            if (class_ChallanForm == null)
            {
                return HttpNotFound();
            }
            return View(class_ChallanForm);
        }

        // GET: Class_ChallanForm/Create
        public ActionResult Create()
        {
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.DurationId = new SelectList(db.DurationTypes, "Id", "Type_Name");
            return View();
        }

        // POST: Class_ChallanForm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ClassId,DurationId,StartDate,EndDate,TotalAmount")] Class_ChallanForm class_ChallanForm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Class_ChallanForm.Add(class_ChallanForm);
                    db.SaveChanges();
                    var studetnlist = db.AspNetStudents.Where(x => x.ClassId == class_ChallanForm.ClassId).ToList();
                    foreach (var item in studetnlist)
                    {
                        Student_ChallanForm std_form = new Student_ChallanForm();
                        std_form.ClassChallanFormId = class_ChallanForm.Id;
                        std_form.AmountPayable = class_ChallanForm.TotalAmount;
                        std_form.TutionFee = class_ChallanForm.TotalAmount;
                        std_form.StudentId = item.Id;
                        db.Student_ChallanForm.Add(std_form);
                        db.SaveChanges();
                    }
                    return RedirectToAction("ClassChallanIndex");
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Create");
            }
            

            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", class_ChallanForm.ClassId);
            ViewBag.DurationId = new SelectList(db.DurationTypes, "Id", "Type_Name", class_ChallanForm.DurationId);
            return View(class_ChallanForm);
        }

        // GET: Class_ChallanForm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ChallanForm class_ChallanForm = db.Class_ChallanForm.Find(id);
            if (class_ChallanForm == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", class_ChallanForm.ClassId);
            ViewBag.DurationId = new SelectList(db.DurationTypes, "Id", "Type_Name", class_ChallanForm.DurationId);
            return View(class_ChallanForm);
        }

        // POST: Class_ChallanForm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ClassId,DurationId,StartDate,EndDate,TotalAmount")] Class_ChallanForm class_ChallanForm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(class_ChallanForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClassChallanIndex");
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", class_ChallanForm.ClassId);
            ViewBag.DurationId = new SelectList(db.DurationTypes, "Id", "Type_Name", class_ChallanForm.DurationId);
            return View(class_ChallanForm);
        }

        // GET: Class_ChallanForm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ChallanForm class_ChallanForm = db.Class_ChallanForm.Find(id);
            if (class_ChallanForm == null)
            {
                return HttpNotFound();
            }
            return View(class_ChallanForm);
        }

        // POST: Class_ChallanForm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class_ChallanForm class_ChallanForm = db.Class_ChallanForm.Find(id);
            db.Class_ChallanForm.Remove(class_ChallanForm);
            db.SaveChanges();
            return RedirectToAction("ClassChallanIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class Class_Challan
        {
            public string ClassName { get; set; }
            public string Duration { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal? Total { get; set; }

        }
    }
}
