using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SEA_Application.Controllers
{
    public class AdmissionDashboardController : Controller
    {

        Sea_Entities db = new Sea_Entities();


        // GET: AdmissionDashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            var ID = User.Identity.GetUserId();
            var branchID = db.AspNetBranch_Admins.Where(x => x.AdminId == ID).Select(x => x.BranchId).FirstOrDefault();
            var Classes1 = db.AspNetBranch_Class.Where(x => x.BranchId == branchID).Select(x => x.AspNetClass.Name).Distinct().ToList();

            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
            classes.Add("In Process");


            ViewBag.AllClasses = classes;

            return View("BlankPage");
        }
        public ActionResult StudentRegistrationView()
        {
            return View();
        }


        [HttpGet]
        public ActionResult CreateRegistrationForm()
        {
            ViewBag.Error = TempData["ErrorMessage"] as string;
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name");
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");
            ViewBag.ParentId = new SelectList(db.AspNetParents, "Id", "UserId");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");
            ViewBag.SectionId = new SelectList(db.AspNetSections, "Id", "Name");
            ViewBag.PackageId = new SelectList(db.AspNetPackages, "Id", "Title");


            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRegistrationForm(StudentRegistration studentRegistration, HttpPostedFileBase[] attachment)
        {
            if (studentRegistration != null)
            {

                StudentRegistration obj = new StudentRegistration();
                obj.StudentName = studentRegistration.StudentName;
                obj.FatherName = studentRegistration.FatherName;
                obj.Email = studentRegistration.Email;
                obj.CellNo = studentRegistration.CellNo;
                obj.DateBirth = studentRegistration.DateBirth;
                obj.PlaceBirth = studentRegistration.PlaceBirth;
                obj.NationalityId = studentRegistration.NationalityId;
                obj.ReligionId = studentRegistration.ReligionId;
                obj.GenderId = studentRegistration.GenderId;
                obj.Address = studentRegistration.Address;
                obj.LastSchoolAttended = studentRegistration.LastSchoolAttended;
                obj.LastSchoolAttendedFromDate = studentRegistration.LastSchoolAttendedFromDate;
                obj.LastSchoolAttendedToDate = studentRegistration.LastSchoolAttendedToDate;
                obj.ReasonLeaving = studentRegistration.ReasonLeaving;
                db.StudentRegistrations.Add(obj);
                db.SaveChanges();

                var AllFiles = "";

                foreach (var file in attachment)
                {
                    if (file != null)
                    {
                        var FileName = file.FileName;
                        FileName = FileName + "_PRD" + obj.Id;
                        AllFiles += FileName + "/";
                        file.SaveAs(Path.Combine(Server.MapPath("~/Content/StudentRegistration"), FileName));
                    }

                }



                if (AllFiles != "")
                {
                    var StudentRegistration = db.StudentRegistrations.Where(x => x.Id == obj.Id).FirstOrDefault();
                    StudentRegistration.Documents = AllFiles;
                    db.SaveChanges();
                }
                return RedirectToAction("StudentRegistrationView");

            }

            return View(studentRegistration);
        }
        public ActionResult DownloadSubmittedDocument(string Name)
        {
            // AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentRegistration/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);

        }
        public ActionResult RegisteredStudentsList()
        {
            var StudentRegistrations = db.StudentRegistrations.Select(x => new { x.Id, x.StudentName, x.FatherName, x.Email, x.CellNo, x.Address, x.AspNetGender.Title, x.Documents }).ToList();

            return Json(StudentRegistrations, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EditStudentReg(int id)
        {

            var StudentRegistration = db.StudentRegistrations.Where(x => x.Id == id).FirstOrDefault();

            ViewBag.Error = TempData["ErrorMessage"] as string;
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name");
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", StudentRegistration.GenderId);
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", StudentRegistration.NationalityId);
            ViewBag.ParentId = new SelectList(db.AspNetParents, "Id", "UserId");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", StudentRegistration.ReligionId);
            ViewBag.SectionId = new SelectList(db.AspNetSections, "Id", "Name");
            ViewBag.PackageId = new SelectList(db.AspNetPackages, "Id", "Title");

            string DateBirthString = "";
            string LastSchoolAttendedFromDateString = "";
            string LastSchoolAttendedToDateString = "";

            if (StudentRegistration.DateBirth != null)
            {

                DateTime DateBirth = Convert.ToDateTime(StudentRegistration.DateBirth);

                DateBirthString = DateBirth.ToString("yyyy-MM-dd");
            }


            ViewBag.datebirth = DateBirthString;



            if (StudentRegistration.LastSchoolAttendedFromDate != null)
            {
                DateTime LastSchoolAttendedFromDate = Convert.ToDateTime(StudentRegistration.LastSchoolAttendedFromDate);

                LastSchoolAttendedFromDateString = LastSchoolAttendedFromDate.ToString("yyyy-MM-dd");

            }
            ViewBag.lastschoolfromdate = LastSchoolAttendedFromDateString;


            if (StudentRegistration.LastSchoolAttendedToDate != null)
            {
                DateTime LastSchoolAttendedToDate = Convert.ToDateTime(StudentRegistration.LastSchoolAttendedToDate);

                LastSchoolAttendedToDateString = LastSchoolAttendedToDate.ToString("yyyy-MM-dd");
            }
            ViewBag.lastschooltodate = LastSchoolAttendedToDateString;

            return View(StudentRegistration);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentReg(StudentRegistration studentRegistration, HttpPostedFileBase[] attachment)
        {

            if (studentRegistration != null)
            {
                var StudentRegistrationToUpdate = db.StudentRegistrations.Where(x => x.Id == studentRegistration.Id).FirstOrDefault();

                StudentRegistrationToUpdate.StudentName = studentRegistration.StudentName;
                StudentRegistrationToUpdate.FatherName = studentRegistration.FatherName;
                StudentRegistrationToUpdate.Email = studentRegistration.Email;
                StudentRegistrationToUpdate.CellNo = studentRegistration.CellNo;
                StudentRegistrationToUpdate.DateBirth = studentRegistration.DateBirth;
                StudentRegistrationToUpdate.PlaceBirth = studentRegistration.PlaceBirth;
                StudentRegistrationToUpdate.NationalityId = studentRegistration.NationalityId;
                StudentRegistrationToUpdate.ReligionId = studentRegistration.ReligionId;
                StudentRegistrationToUpdate.GenderId = studentRegistration.GenderId;
                StudentRegistrationToUpdate.Address = studentRegistration.Address;
                StudentRegistrationToUpdate.LastSchoolAttended = studentRegistration.LastSchoolAttended;
                StudentRegistrationToUpdate.LastSchoolAttendedFromDate = studentRegistration.LastSchoolAttendedFromDate;
                StudentRegistrationToUpdate.LastSchoolAttendedToDate = studentRegistration.LastSchoolAttendedToDate;
                StudentRegistrationToUpdate.ReasonLeaving = studentRegistration.ReasonLeaving;
                db.SaveChanges();

                var AllFiles = "";
                foreach (var file in attachment)
                {
                    if (file != null)
                    {

                        var FileName = file.FileName;
                        FileName = FileName + "_PRD" + StudentRegistrationToUpdate.Id;
                        AllFiles += FileName + "/";
                        file.SaveAs(Path.Combine(Server.MapPath("~/Content/StudentRegistration"), FileName));
                    }
                }

                if (AllFiles != "")
                {
                    var StudentRegistration = db.StudentRegistrations.Where(x => x.Id == StudentRegistrationToUpdate.Id).FirstOrDefault();
                    StudentRegistration.Documents = AllFiles;
                    db.SaveChanges();
                }
                return RedirectToAction("StudentRegistrationView");

            }


            return View();
        }

    }
}