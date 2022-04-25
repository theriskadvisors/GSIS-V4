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
                obj.BranchId = studentRegistration.BranchId;
                obj.ClassId = studentRegistration.ClassId;
                obj.SectionId = studentRegistration.SectionId;

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
        } //Create Registration Form

        public ActionResult ClassFeeView()
        {
            return View();

        }
        public ActionResult ClassFeeList()
        {
            var PreRegistrationFeeList = db.PreRegistrationClassFees.Select(x => new { x.Id, ClassName = x.AspNetClass.Name, SessionName = x.AspNetSession.Year, x.AdmissionFee, x.SecurityFee, x.MonthlyTutionFee, x.AnnualFund, x.Stationary, x.LabChargesPhysics, x.LabChargesBiology, x.LabChargesChemistry, x.LabChargesComputer, x.LeaderInMe, x.ExamCharges, x.Deferred, x.LessReceived, x.Total, x.StudentRegistrationFee }).ToList();

            return Json(PreRegistrationFeeList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CreateClassFee()
        {


            return View();
        }
        [HttpPost]

        public ActionResult CreateClassFee(PreRegistrationClassFee ClassFee)
        {
            ClassFee.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();

            db.PreRegistrationClassFees.Add(ClassFee);
            db.SaveChanges();

            return RedirectToAction("ClassFeeView");
        }
        public ActionResult ClassFeeExist(int ClassId, int SessionId)
        {
            var Msg = "";
            var ClassFee = db.PreRegistrationClassFees.Where(x => x.ClassId == ClassId && x.SessionId == SessionId).FirstOrDefault();

            if (ClassFee != null)
            {
                Msg = "Selected Classfee is already created for the selected session";

            }

            return Json(Msg, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditClassFee(int Id)
        {
            var PreRegistrationClassFee = db.PreRegistrationClassFees.Where(x => x.Id == Id).FirstOrDefault();

            return View(PreRegistrationClassFee);
        }
        [HttpPost]
        public ActionResult EditClassFee(PreRegistrationClassFee classFee)
        {
            var ClassFeeToUpdate = db.PreRegistrationClassFees.Where(x => x.Id == classFee.Id).FirstOrDefault();
            ClassFeeToUpdate.ClassId = classFee.ClassId;
            ClassFeeToUpdate.SessionId = classFee.SessionId;
            ClassFeeToUpdate.StudentRegistrationFee = classFee.StudentRegistrationFee;
            ClassFeeToUpdate.AdmissionFee = classFee.AdmissionFee;
            ClassFeeToUpdate.SecurityFee = classFee.SecurityFee;
            ClassFeeToUpdate.MonthlyTutionFee = classFee.MonthlyTutionFee;
            ClassFeeToUpdate.AnnualFund = classFee.AnnualFund;
            ClassFeeToUpdate.Stationary = classFee.Stationary;
            ClassFeeToUpdate.LabChargesPhysics = classFee.LabChargesPhysics;
            ClassFeeToUpdate.LabChargesBiology = classFee.LabChargesBiology;
            ClassFeeToUpdate.LabChargesChemistry = classFee.LabChargesChemistry;
            ClassFeeToUpdate.LabChargesComputer = classFee.LabChargesComputer;
            ClassFeeToUpdate.LeaderInMe = classFee.LeaderInMe;
            ClassFeeToUpdate.ExamCharges = classFee.ExamCharges;
            ClassFeeToUpdate.Deferred = classFee.Deferred;
            ClassFeeToUpdate.LessReceived = classFee.LessReceived;
            ClassFeeToUpdate.Total = classFee.Total;

            db.SaveChanges();

            return RedirectToAction("ClassFeeView");
        }


        public ActionResult DownloadSubmittedDocument(string Name)
        {
            // AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentRegistration/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);

        }
        public ActionResult RegisteredStudentsList()
        {
            var StudentRegistrations = db.StudentRegistrations.Select(x => new { x.Id, x.StudentName, x.FatherName, x.Email, x.CellNo, x.Address, x.AspNetGender.Title, x.Documents, FeeStatus = x.RegistrationFees.FirstOrDefault().Status }).ToList();

            //  var StudentRegistrations = db.RegistrationFees.Select(x => new { x.StudentRegistration.Id, x.StudentRegistration.StudentName, x.StudentRegistration.FatherName, x.StudentRegistration.Email, x.StudentRegistration.CellNo, x.StudentRegistration.Address, x.StudentRegistration.AspNetGender.Title, x.StudentRegistration.Documents ,x.Status  }).GroupBy(x=>x.Id).ToList();


            return Json(StudentRegistrations, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EditStudentReg(int id)
        {

            var StudentRegistration = db.StudentRegistrations.Where(x => x.Id == id).FirstOrDefault();



            ViewBag.Error = TempData["ErrorMessage"] as string;
            ViewBag.BranchIdDisabled = new SelectList(db.AspNetBranches, "Id", "Name", StudentRegistration.BranchId);

            var Classes = db.AspNetBranch_Class.Where(x => x.AspNetBranch.Id == StudentRegistration.BranchId).Select(x => new { x.AspNetClass.Id, x.AspNetClass.Name }).ToList();
            var Sections = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.AspNetBranch.Id == StudentRegistration.BranchId && x.AspNetBranch_Class.AspNetClass.Id == StudentRegistration.ClassId).Select(x => new { x.AspNetSection.Id, x.AspNetSection.Name }).ToList();


            ViewBag.ClassIdDisabled = new SelectList(Classes, "Id", "Name", StudentRegistration.ClassId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", StudentRegistration.GenderId);
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", StudentRegistration.NationalityId);
            ViewBag.ParentId = new SelectList(db.AspNetParents, "Id", "UserId");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", StudentRegistration.ReligionId);
            ViewBag.SectionIdDisabled = new SelectList(Sections, "Id", "Name", StudentRegistration.SectionId);
            ViewBag.PackageId = new SelectList(db.AspNetPackages, "Id", "Title");

            var RegistrationFee = db.RegistrationFees.Where(x => x.StudentRegistrationId == StudentRegistration.Id).FirstOrDefault();

            if (RegistrationFee != null)
            {
                ViewBag.FeeExist = true;
                ViewBag.FeeStatus = RegistrationFee.Status;

            }
            else
            {
                ViewBag.FeeExist = false;
            }



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
                StudentRegistrationToUpdate.BranchId = studentRegistration.BranchId;
                StudentRegistrationToUpdate.ClassId = studentRegistration.ClassId;
                StudentRegistrationToUpdate.SectionId = studentRegistration.SectionId;
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

        public ActionResult AllBranches()
        {
            var Branches = (from branch in db.AspNetBranches.Where(X => X.IsAdministrativeBranch == true)

                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);
            return Content(status);
        }


        public ActionResult ClassesByBranch(int BranchId)
        {
            var BranchClasses = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.AspNetClass.Id, x.AspNetClass.Name });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(BranchClasses);
            return Content(status);
        }

        public ActionResult SectionByClasses(int ClassId, int BranchId)
        {
            var ID = User.Identity.GetUserId();

            var Sections = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.AspNetBranch.Id == BranchId && x.AspNetBranch_Class.AspNetClass.Id == ClassId).Select(x => new { x.AspNetSection.Id, x.AspNetSection.Name });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateRegistrationFee(int StudentRegistrationId)
        {
            var StudentRegistration = db.StudentRegistrations.Where(x => x.Id == StudentRegistrationId).FirstOrDefault();

            if (StudentRegistration != null)
            {
                ViewBag.StudentName = StudentRegistration.StudentName;
                ViewBag.FatherName = StudentRegistration.FatherName;
                ViewBag.Email = StudentRegistration.Email;
                ViewBag.ClassName = StudentRegistration.AspNetClass.Name;
                ViewBag.ClassId = StudentRegistration.AspNetClass.Id;
                ViewBag.StudentRegistrionId = StudentRegistrationId;
            }
            else
            {
                return RedirectToAction("StudentRegistrationView");
            }



            return View();
        }
        public ActionResult GetRegistrationClassFee(int ClassId)
        {
            var PreRegistrationClassFee = db.PreRegistrationClassFees.Where(x => x.ClassId == ClassId && x.AspNetSession.IsActive == true).Select(x => new { x.AdmissionFee, x.SecurityFee, x.MonthlyTutionFee, x.AnnualFund, x.Stationary, x.LabChargesPhysics, x.LabChargesChemistry, x.LabChargesComputer, x.LabChargesBiology, x.LeaderInMe, x.ExamCharges, x.Deferred, x.LessReceived, x.Total, x.StudentRegistrationFee }).FirstOrDefault();

            return Json(new { PreRegistrationClassFee =  PreRegistrationClassFee}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateRegistrationFee(RegistrationFee registrationFee)
        {
            try
            {
                RegistrationFee registration = new RegistrationFee();

                registration.AdmissionFee = registrationFee.AdmissionFee;
                registration.StudentRegistrationFee = registrationFee.StudentRegistrationFee;
                registration.SecurityFee = registrationFee.SecurityFee;
                registration.MonthlyTutionFee = registrationFee.MonthlyTutionFee;
                registration.AnnualFund = registrationFee.AnnualFund;
                registration.Stationary = registrationFee.Stationary;
                registration.LabChargesPhysics = registrationFee.LabChargesPhysics;
                registration.LabChargesBiology = registrationFee.LabChargesBiology;
                registration.LabChargesChemistry = registrationFee.LabChargesChemistry;
                registration.LabChargesComputer = registrationFee.LabChargesComputer;
                registration.Notes = registrationFee.Notes;
                registration.Total = registrationFee.Total;
                registration.StudentRegistrationId = registrationFee.StudentRegistrationId;
                registration.ExamCharges = registrationFee.ExamCharges;
                registration.LeaderInMe = registrationFee.LeaderInMe;
                registration.Deferred = registrationFee.Deferred;
                registration.LessReceived = registrationFee.LessReceived;
                registration.DueDate = registrationFee.DueDate;
                registration.IssueDate = registrationFee.IssueDate;
                registration.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                registration.FineFirstFromDate = registrationFee.FineFirstFromDate;
                registration.FineFirstToDate = registrationFee.FineFirstToDate;
                registration.FineFirstAmount = registrationFee.FineFirstAmount;

                registration.FineSecondFromDate = registrationFee.FineSecondFromDate;
                registration.FineSecondToDate = registrationFee.FineSecondToDate;
                registration.FineSecondAmount = registrationFee.FineSecondAmount;

                registration.FineThirdFromDate = registrationFee.FineThirdFromDate;
                registration.FineThirdToDate = registrationFee.FineThirdToDate;
                registration.FineThirdAmount = registrationFee.FineThirdAmount;

                registration.Status = "Pending";

                db.RegistrationFees.Add(registration);
                db.SaveChanges();

                return RedirectToAction("EditRegistrationFee", new { RegistrationFeeId = registration.Id });
            }
            catch (Exception ex)
            {
                return View();

            }

        }
        [HttpGet]
        public ActionResult EditRegistrationFee(int RegistrationFeeId)
        {
            var RegistrationFee = db.RegistrationFees.Where(x => x.Id == RegistrationFeeId).FirstOrDefault();

            ViewBag.StudentName = RegistrationFee.StudentRegistration.StudentName;
            ViewBag.FatherName = RegistrationFee.StudentRegistration.FatherName;
            ViewBag.Email = RegistrationFee.StudentRegistration.Email;
            //ViewBag.StudentRegistrionId = RegistrationFee.StudentRegistrationId;

            return View(RegistrationFee);
        }
        [HttpPost]
        public ActionResult EditRegistrationFee(RegistrationFee registrationFee)
        {

            var registration = db.RegistrationFees.Where(x => x.Id == registrationFee.Id).FirstOrDefault();

            registration.AdmissionFee = registrationFee.AdmissionFee;
            registration.StudentRegistrationFee = registrationFee.StudentRegistrationFee;
            registration.SecurityFee = registrationFee.SecurityFee;
            registration.MonthlyTutionFee = registrationFee.MonthlyTutionFee;
            registration.AnnualFund = registrationFee.AnnualFund;
            registration.Stationary = registrationFee.Stationary;
            registration.LabChargesPhysics = registrationFee.LabChargesPhysics;
            registration.LabChargesBiology = registrationFee.LabChargesBiology;
            registration.LabChargesChemistry = registrationFee.LabChargesChemistry;
            registration.LabChargesComputer = registrationFee.LabChargesComputer;
            registration.Notes = registrationFee.Notes;
            registration.Total = registrationFee.Total;

            registration.ExamCharges = registrationFee.ExamCharges;
            registration.LeaderInMe = registrationFee.LeaderInMe;
            registration.Deferred = registrationFee.Deferred;
            registration.LessReceived = registrationFee.LessReceived;
            registration.DueDate = registrationFee.DueDate;
            registration.IssueDate = registrationFee.IssueDate;
            registration.FineFirstFromDate = registrationFee.FineFirstFromDate;
            registration.FineFirstToDate = registrationFee.FineFirstToDate;
            registration.FineFirstAmount = registrationFee.FineFirstAmount;

            registration.FineSecondFromDate = registrationFee.FineSecondFromDate;
            registration.FineSecondToDate = registrationFee.FineSecondToDate;
            registration.FineSecondAmount = registrationFee.FineSecondAmount;

            registration.FineThirdFromDate = registrationFee.FineThirdFromDate;
            registration.FineThirdToDate = registrationFee.FineThirdToDate;
            registration.FineThirdAmount = registrationFee.FineThirdAmount;

            db.SaveChanges();

            return RedirectToAction("StudentRegistrationView");
        }
        public ActionResult RegistrationChallan(int RegistrationFeeId)
        {
            var RegistrationFee = db.RegistrationFees.Where(x => x.Id == RegistrationFeeId).FirstOrDefault();

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(RegistrationFee.DueDate.Value.Month);
            int year = Convert.ToInt32(RegistrationFee.DueDate.Value.Year.ToString().Substring(2, 2));

            ViewBag.FeePeriod = monthName + "-" + year;

            string FirstFromDate = "";
            string FirstToDate = "";

            string SecondFromDate = "";
            string SecondToDate = "";

            string ThirdFromDate = "";
            string ThirdToDate = "";

            if (RegistrationFee.FineFirstFromDate != null && RegistrationFee.FineFirstToDate != null)
            {
                FirstFromDate = FormattedDay(RegistrationFee.FineFirstFromDate.Value.Day);
                FirstToDate = FormattedDay(RegistrationFee.FineFirstToDate.Value.Day);
            }



            if (RegistrationFee.FineSecondFromDate != null && RegistrationFee.FineSecondToDate != null)
            {

                SecondFromDate = FormattedDay(RegistrationFee.FineSecondFromDate.Value.Day);
                SecondToDate = FormattedDay(RegistrationFee.FineSecondToDate.Value.Day);
            }

            if (RegistrationFee.FineThirdFromDate != null && RegistrationFee.FineThirdToDate != null)
            {
                ThirdFromDate = FormattedDay(RegistrationFee.FineThirdFromDate.Value.Day);
                ThirdToDate = FormattedDay(RegistrationFee.FineThirdToDate.Value.Day);
            }



            if (FirstFromDate != "" && FirstToDate != "")
            {
                ViewBag.FirstDate = FirstFromDate + " to " + FirstToDate;
            }
            else
            {
                ViewBag.FirstDate = "";
            }

            if (SecondFromDate != "" && SecondToDate != "")
            {
                ViewBag.SecondDate = SecondFromDate + " to " + SecondToDate;

            }
            else
            {
                ViewBag.SecondDate = "";
            }


            if (ThirdFromDate != "" && ThirdToDate != "")
            {
                ViewBag.ThirdDate = ThirdFromDate + " to " + ThirdToDate;

            }
            else
            {
                ViewBag.ThirdDate = "";


            }



            return View(RegistrationFee);
        }
        public ActionResult AllClasses()
        {
            var Classes = db.AspNetClasses.Select(x => new { x.Id, x.Name }).ToList();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
            return Content(status);

        }
        public ActionResult AllSession()
        {
            var Sessions = (from Session in db.AspNetSessions.Where(x => x.AspNetStatu.Id == 1)

                            select new
                            {
                                Session.Id,
                                Session.Year,

                            });


            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sessions);
            return Content(status);
        }

        public string FormattedDay(int Day)
        {
            string FormattedDay = "";

            if (Day == 1)
            {
                FormattedDay = Day.ToString() + "st";
            }
            else if (Day == 2)
            {
                FormattedDay = Day.ToString() + "nd";
            }
            else if (Day == 3)
            {
                FormattedDay = Day.ToString() + "rd";
            }
            else
            {
                FormattedDay = Day.ToString() + "th";
            }


            return FormattedDay.ToString();
        }

        public ActionResult DeleteStudentRegistration(int StudentRegistrationId)
        {
            var RegistrationFeeToDelete = db.RegistrationFees.Where(x => x.StudentRegistrationId == StudentRegistrationId).FirstOrDefault();

            if (RegistrationFeeToDelete != null)
            {
                db.RegistrationFees.Remove(RegistrationFeeToDelete);
                db.SaveChanges();
            }


            var StudentRegistrationToDelete = db.StudentRegistrations.Where(x => x.Id == StudentRegistrationId).FirstOrDefault();

            if (StudentRegistrationToDelete != null)
            { 
                db.StudentRegistrations.Remove(StudentRegistrationToDelete);
                db.SaveChanges();
            }

            return RedirectToAction("StudentRegistrationView");
        }




    }
}