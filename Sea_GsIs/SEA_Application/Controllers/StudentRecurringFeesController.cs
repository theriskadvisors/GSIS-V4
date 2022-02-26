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

        //GET: StudentRecurringFees
        public ActionResult Index()
        {
            var studentRecurringFees = db.ClassFees.Include(x => x.AspNetBranch_Class.AspNetClass).Include(x => x.AspNetBranch_Class.AspNetBranch);
            return View(studentRecurringFees.ToList());
        }

        //public ActionResult Index()
        //{
        //    var classFee = db.ClassFees.Include(s => s.AspNetSession).Include(x=>x.AspNetBranch_Class.AspNetBranch);
        //    return View(classFee.ToList());
        //}

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

            if (db.StudentRecurringFees.Where(x => x.ClassId == Id).Count() > 0)
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

        public ActionResult AllBranches()
        {
            var Branches = (from branch in db.AspNetBranches
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);
            return Content(status);
        }
        public ActionResult AllSession()
        {
            var Sessions = (from Session in db.AspNetSessions.Where(x=>x.AspNetStatu.Id == 1)

                            select new
                            {
                                Session.Id,
                                Session.Year,

                            });


            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sessions);
            return Content(status);
        }


        public ActionResult ClassesByBranch(int BranchId)
        {
            //var Classes = (from classs in db.AspNetClasses
            //               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
            //               where branchclasssubject.BranchId == BranchId
            //               select new
            //               {
            //                   classs.Id,
            //                   classs.Name,
            //               }).Distinct();


            var Classes = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.AspNetClass.Id, x.AspNetClass.Name });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
            return Content(status);
        }


        // POST: StudentRecurringFees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "Id,ClassId,ComputerFee,SecurityServices,LabCharges,Transportation,SportsShirt,ChineseClassFee,AdvanceTax,Other,TutionFee,TotalFee")]*/ ClassFee classFee)
        {

            try
            {
                var BranchId = Convert.ToInt32(Request.Form["BranchId"]);
                var ClassId = Convert.ToInt32(Request.Form["ClassId"]);

                int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;

                classFee.BranchClassID = BranchClassId;
                classFee.CreationDate = DateTime.Now;

                db.ClassFees.Add(classFee);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View(classFee);
            }


        }

        // GET: StudentRecurringFees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassFee classFee = db.ClassFees.Find(id);
            if (classFee == null)
            {
                return HttpNotFound();
            }
            //  ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", studentRecurringFee.ClassId);
            var Branches = (from branch in db.AspNetBranches
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();

            var Classes = db.AspNetBranch_Class.Where(x => x.BranchId == classFee.AspNetBranch_Class.BranchId).ToList().Select(x => new { x.AspNetClass.Id, x.AspNetClass.Name });

            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", classFee.AspNetBranch_Class.AspNetBranch.Id);

            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", classFee.AspNetBranch_Class.AspNetClass.Id);

            var Sessions = (from Session in db.AspNetSessions

                            select new
                            {
                                Session.Id,
                                Session.Year,

                            });

            ViewBag.SessionID = new SelectList(Sessions, "Id", "Year", classFee.SessionID);


            return View(classFee);
        }

        // POST: StudentRecurringFees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassFee classFee)
        {
            try
            {

                var BranchId = Convert.ToInt32(Request.Form["BranchId"]);
                var ClassId = Convert.ToInt32(Request.Form["ClassId"]);

                int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;

                classFee.BranchClassID = BranchClassId;


                ClassFee classFeeToUpdate = db.ClassFees.Where(x => x.Id == classFee.Id).FirstOrDefault();


                classFeeToUpdate.LabCharges = classFee.LabCharges;
                classFeeToUpdate.OtherServices = classFee.OtherServices;
                classFeeToUpdate.SessionID = classFee.SessionID;
                classFeeToUpdate.BranchClassID = BranchClassId;
                classFeeToUpdate.ComputerFee = classFee.ComputerFee;
                classFeeToUpdate.AdmissionFee = classFee.AdmissionFee;
                classFeeToUpdate.Total = classFee.Total;
                classFeeToUpdate.TutionFee = classFee.TutionFee;

                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View(classFee);
            }

            //  return View(classFee);
        }
        public ActionResult checkRecurringFeeExist(int BranchId, int ClassId)
        {
            var Msg = "";
            AspNetBranch_Class branchClass = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault();

            ClassFee classFee = db.ClassFees.Where(x => x.BranchClassID == branchClass.Id).FirstOrDefault();

            if (classFee != null)
            {

                Msg = "Class fee is already created of selected branch and class";

            }


            return Json(Msg, JsonRequestBehavior.AllowGet);
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
