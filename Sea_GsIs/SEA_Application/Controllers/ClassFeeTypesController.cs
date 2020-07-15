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
    public class ClassFeeTypesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: ClassFeeTypes
        //public ActionResult Index()
        //{
        //    var classFeeTypes = db.ClassFeeTypes.Include(c => c.AspNetClass).Include(c => c.ClassFee);
        //    return View(classFeeTypes.ToList());
        //}
        public ActionResult FeeTypeIndex()
        {
            return View();
        }
        public ActionResult GetClassFee()
        {
            var durationlist = db.ClassFeeTypes.ToList();
            List<Fee_Type> duratintype = new List<Fee_Type>();
            foreach (var item in durationlist)
            {
                var classname = db.AspNetClasses.Where(x => x.Id == item.ClassId).Select(x => x.Name).FirstOrDefault();
                var classfee = db.ClassFees.Where(x => x.Id == item.ClassFeeId).Select(x => x.Name).FirstOrDefault();
                Fee_Type dt = new Fee_Type();
                dt.ClassName = classname;
                dt.FeeType = classfee;
                duratintype.Add(dt);
            }
            return Json(duratintype, JsonRequestBehavior.AllowGet);
        }
        // GET: ClassFeeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFeeType classFeeType = db.ClassFeeTypes.Find(id);
            if (classFeeType == null)
            {
                return HttpNotFound();
            }
            return View(classFeeType);
        }

        // GET: ClassFeeTypes/Create
        public ActionResult Create()
        {
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.ClassFeeId = new SelectList(db.ClassFees, "Id", "Name");
            return View();
        }

        // POST: ClassFeeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClassFeeId,ClassId")] ClassFeeType classFeeType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.ClassFeeTypes.Add(classFeeType);
                    db.SaveChanges();
                    var studentlist = db.AspNetStudents.Where(x => x.ClassId == classFeeType.ClassId).ToList();
                    foreach (var item in studentlist)
                    {
                        Student_ChallanForm std_form = db.Student_ChallanForm.Where(x => x.StudentId == item.Id).FirstOrDefault();
                        var amountpayable = db.ClassFees.Where(x => x.Id == classFeeType.ClassFeeId).Select(x => x.Amount).FirstOrDefault();
                        std_form.AmountPayable += amountpayable;
                        db.SaveChanges();
                    }
                    return RedirectToAction("FeeTypeIndex");
                }
            }
            catch
            {
                return RedirectToAction("Create");
            }


            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", classFeeType.ClassId);
            ViewBag.ClassFeeId = new SelectList(db.ClassFees, "Id", "Name", classFeeType.ClassFeeId);
            return View(classFeeType);
        }

        // GET: ClassFeeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFeeType classFeeType = db.ClassFeeTypes.Find(id);
            if (classFeeType == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", classFeeType.ClassId);
            ViewBag.ClassFeeId = new SelectList(db.ClassFees, "Id", "Name", classFeeType.ClassFeeId);
            return View(classFeeType);
        }

        // POST: ClassFeeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClassFeeId,ClassId")] ClassFeeType classFeeType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classFeeType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("FeeTypeIndex");
            }
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", classFeeType.ClassId);
            ViewBag.ClassFeeId = new SelectList(db.ClassFees, "Id", "Name", classFeeType.ClassFeeId);
            return View(classFeeType);
        }

        // GET: ClassFeeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFeeType classFeeType = db.ClassFeeTypes.Find(id);
            if (classFeeType == null)
            {
                return HttpNotFound();
            }
            return View(classFeeType);
        }

        // POST: ClassFeeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassFeeType classFeeType = db.ClassFeeTypes.Find(id);
            db.ClassFeeTypes.Remove(classFeeType);
            db.SaveChanges();
            return RedirectToAction("FeeTypeIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class Fee_Type
        {
            public string ClassName { get; set; }
            public string FeeType { get; set; }
        }
    }
}
