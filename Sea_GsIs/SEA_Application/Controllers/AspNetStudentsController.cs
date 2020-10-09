using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using System.Net.Mail;
using System.Text;
using SEA_Application.CustomModel;
using Microsoft.Office.Interop.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SEA_Application.Controllers
{

    public class AspNetStudentsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private Sea_Entities db = new Sea_Entities();
        // private string StudentID;

        public AspNetStudentsController()
        {
            //   StudentID = Convert.ToString(System.Web.HttpContext.Current.Session["StudentID"]);
        }

        public AspNetStudentsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: AspNetStudents
        public ActionResult Index()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");

            return View();
        }
        public ActionResult StudentIndex()
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;
            if (User.IsInRole("Branch_Admin") || User.IsInRole("Branch_Principal"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            if (User.IsInRole("Accountant"))
            {
                ViewBag.UserRole = "Accountant";

                ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");
            }
            else
            {
                ViewBag.UserRole = "NotAccountant";
                ViewBag.ClassID = new SelectList(db.AspNetClasses.Where(x => x.AspNetBranch_Class.Any(y => y.BranchId == branchId)), "Id", "Name");
            }
            return View();
        }

        public ActionResult StudentsLoader()
        {

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveStudentsFromFile(RegisterViewModel model)
        {
            // if (ModelState.IsValid)

            var dbTransaction = db.Database.BeginTransaction();
            int? RowNumber = null;
            int? ColumnNumber = null;
            var StudentFeeMsg = "";
            try
            {
                HttpPostedFileBase file = Request.Files["StudentLoaderFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        RowNumber = rowIterator - 1;

                        var Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                        var UserName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        var UserNameExist = (from user in db.AspNetUsers //db.AspNetUsers.Where(x=>x.AspNetUser.UserName ==UserName).FirstOrDefault();
                                             join student in db.AspNetStudents on user.Id equals student.UserId
                                             where user.UserName == UserName
                                             select user).FirstOrDefault();

                        if (UserNameExist != null)
                        {
                            var AdmissionFee = workSheet.Cells[rowIterator, 3].Value;

                            if (AdmissionFee == null)
                            {
                                AdmissionFee = 0;
                            }

                            var DiscountAdmissionFee = workSheet.Cells[rowIterator, 4].Value;

                            if (DiscountAdmissionFee == null)
                            {
                                DiscountAdmissionFee = 0;
                            }
                            var DiscountedAdmissionFee = workSheet.Cells[rowIterator, 5].Value;

                            if (DiscountedAdmissionFee == null)
                            {
                                DiscountedAdmissionFee = 0;
                            }
                            var TutionFee = workSheet.Cells[rowIterator, 6].Value;

                            if (TutionFee == null)
                            {
                                TutionFee = 0;
                            }
                            var DiscountTutionFee = workSheet.Cells[rowIterator, 7].Value;

                            if (DiscountTutionFee == null)
                            {
                                DiscountTutionFee = 0;
                            }
                            var DiscountedTutionFee = workSheet.Cells[rowIterator, 8].Value;

                            if (DiscountedTutionFee == null)
                            {
                                DiscountedTutionFee = 0;
                            }
                            var LabFee = workSheet.Cells[rowIterator, 9].Value;

                            if (LabFee == null)
                            {
                                LabFee = 0;
                            }
                            var DiscountLabFee = workSheet.Cells[rowIterator, 10].Value;

                            if (DiscountLabFee == null)
                            {
                                DiscountLabFee = 0;
                            }

                            var DiscountedLabFee = workSheet.Cells[rowIterator, 11].Value;

                            if (DiscountedLabFee == null)
                            {
                                DiscountedLabFee = 0;
                            }
                            var ComputerFee = workSheet.Cells[rowIterator, 12].Value;

                            if (ComputerFee == null)
                            {
                                ComputerFee = 0;
                            }
                            var DiscountComputerFee = workSheet.Cells[rowIterator, 13].Value;

                            if (DiscountComputerFee == null)
                            {
                                DiscountComputerFee = 0;
                            }
                            var DiscountedComputerFee = workSheet.Cells[rowIterator, 14].Value;

                            if (DiscountedComputerFee == null)
                            {
                                DiscountedComputerFee = 0;
                            }
                            var OtherServices = workSheet.Cells[rowIterator, 15].Value;

                            if (OtherServices == null)
                            {
                                OtherServices = 0;
                            }
                            var DiscountOtherServices = workSheet.Cells[rowIterator, 16].Value;

                            if (DiscountOtherServices == null)
                            {
                                DiscountOtherServices = 0;
                            }
                            var DiscountedOtherServices = workSheet.Cells[rowIterator, 17].Value;

                            if (DiscountedOtherServices == null)
                            {
                                DiscountedOtherServices = 0;
                            }


                            var TotalDiscountPercentage = workSheet.Cells[rowIterator, 18].Value;

                            if (TotalDiscountPercentage == null)
                            {
                                TotalDiscountPercentage = 0;
                            }

                            var TotalFeeBeforeDiscount = workSheet.Cells[rowIterator, 19].Value;

                            if (TotalFeeBeforeDiscount == null)
                            {
                                TotalFeeBeforeDiscount = 0;
                            }
                            var TotalFeeAfterDiscount = workSheet.Cells[rowIterator, 20].Value;

                            if (TotalFeeAfterDiscount == null)
                            {
                                TotalFeeAfterDiscount = 0;
                            }
                            var TotalFeeAfterDiscountWithoutAdmission = workSheet.Cells[rowIterator, 21].Value;

                            if (TotalFeeAfterDiscountWithoutAdmission == null)
                            {
                                TotalFeeAfterDiscountWithoutAdmission = 0;
                            }

                            var JanurayMultiplier = workSheet.Cells[rowIterator, 22].Value;

                            if (JanurayMultiplier == null)
                            {
                                JanurayMultiplier = 0;
                            }
                            var FebruaryMultiplier = workSheet.Cells[rowIterator, 23].Value;

                            if (FebruaryMultiplier == null)
                            {
                                FebruaryMultiplier = 0;
                            }
                            var MarchMultiplier = workSheet.Cells[rowIterator, 24].Value;

                            if (MarchMultiplier == null)
                            {
                                MarchMultiplier = 0;
                            }
                            var AprilMultiplier = workSheet.Cells[rowIterator, 25].Value;

                            if (AprilMultiplier == null)
                            {
                                AprilMultiplier = 0;
                            }
                            var MayMultiplier = workSheet.Cells[rowIterator, 26].Value;

                            if (MayMultiplier == null)
                            {
                                MayMultiplier = 0;
                            }
                            var JuneMultiplier = workSheet.Cells[rowIterator, 27].Value;

                            if (JuneMultiplier == null)
                            {
                                JuneMultiplier = 0;
                            }
                            var JulyMultiplier = workSheet.Cells[rowIterator, 28].Value;

                            if (JulyMultiplier == null)
                            {
                                JulyMultiplier = 0;
                            }
                            var AugustMultiplier = workSheet.Cells[rowIterator, 29].Value;

                            if (AugustMultiplier == null)
                            {
                                AugustMultiplier = 0;
                            }
                            var SeptemberMultiplier = workSheet.Cells[rowIterator, 30].Value;

                            if (SeptemberMultiplier == null)
                            {
                                SeptemberMultiplier = 0;
                            }
                            var OctoberMultiplier = workSheet.Cells[rowIterator, 31].Value;

                            if (OctoberMultiplier == null)
                            {
                                OctoberMultiplier = 0;
                            }
                            var NovemberMultiplier = workSheet.Cells[rowIterator, 32].Value;

                            if (NovemberMultiplier == null)
                            {
                                NovemberMultiplier = 0;
                            }
                            var DecemberMultiplier = workSheet.Cells[rowIterator, 33].Value;

                            if (DecemberMultiplier == null)
                            {
                                DecemberMultiplier = 0;
                            }

                            var StudentExist = db.AspNetStudents.Where(x => x.UserId == UserNameExist.Id).FirstOrDefault();

                            if (StudentExist != null)
                            {
                                var StudentFeeExist = db.StudentFees.Where(x => x.StudentID == StudentExist.Id).FirstOrDefault();

                                if (StudentFeeExist == null)
                                {
                                    StudentFee studentFee = new StudentFee();

                                    studentFee.AdmissionFee = Convert.ToDouble(AdmissionFee);
                                    studentFee.TutionFee = Convert.ToDouble(TutionFee);
                                    studentFee.ComputerFee = Convert.ToDouble(ComputerFee);
                                    studentFee.LabCharges = Convert.ToDouble(LabFee);
                                    studentFee.OtherServices = Convert.ToDouble(OtherServices);
                                    studentFee.Total = Convert.ToDouble(TotalFeeBeforeDiscount);
                                    studentFee.DiscountAdmissionFee = Convert.ToDouble(DiscountAdmissionFee);
                                    studentFee.DiscountTutionFee = Convert.ToDouble(DiscountTutionFee);
                                    studentFee.DiscountLabCharges = Convert.ToDouble(DiscountLabFee);
                                    studentFee.DiscountComputerFee = Convert.ToDouble(DiscountComputerFee);
                                    studentFee.DiscountOtherServices = Convert.ToDouble(DiscountOtherServices);
                                    studentFee.DiscountTotal = Convert.ToDouble(TotalDiscountPercentage);
                                    studentFee.DiscountTutionFeeAmount = Convert.ToDouble(DiscountedTutionFee);
                                    studentFee.DiscountComputerFeeAmount = Convert.ToDouble(DiscountedComputerFee);
                                    studentFee.DiscountLabChargesAmount = Convert.ToDouble(DiscountedLabFee);
                                    studentFee.DiscountOtherServicesAmount = Convert.ToDouble(DiscountedOtherServices);
                                    studentFee.DiscountAdmissionFeeAmount = Convert.ToDouble(DiscountedAdmissionFee);
                                    studentFee.DiscountTotalAmount = Convert.ToDouble(TotalFeeAfterDiscount);
                                    studentFee.TotalWithoutAdmission = Convert.ToDouble(TotalFeeAfterDiscountWithoutAdmission);

                                    studentFee.StudentID = StudentExist.Id;
                                    studentFee.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                                    int ActiveSessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                                    studentFee.SessionID = ActiveSessionId;

                                    db.StudentFees.Add(studentFee);
                                    db.SaveChanges();


                                    StudentFeeMultiplier studentFeeMultiplier = new StudentFeeMultiplier();

                                    studentFeeMultiplier.Jan_Multiplier = Convert.ToDouble(JanurayMultiplier);
                                    studentFeeMultiplier.Feb_Multiplier = Convert.ToDouble(FebruaryMultiplier);
                                    studentFeeMultiplier.Mar_Multiplier = Convert.ToDouble(MarchMultiplier);
                                    studentFeeMultiplier.April__Multiplier = Convert.ToDouble(AprilMultiplier);
                                    studentFeeMultiplier.May_Multiplier = Convert.ToDouble(MayMultiplier);
                                    studentFeeMultiplier.June_Multiplier = Convert.ToDouble(JuneMultiplier);
                                    studentFeeMultiplier.July__Multiplier = Convert.ToDouble(JulyMultiplier);
                                    studentFeeMultiplier.Aug_Multiplier = Convert.ToDouble(AugustMultiplier);
                                    studentFeeMultiplier.Sep_Multiplier = Convert.ToDouble(SeptemberMultiplier);
                                    studentFeeMultiplier.Oct_Multiplier = Convert.ToDouble(OctoberMultiplier);
                                    studentFeeMultiplier.Nov_Multiplier = Convert.ToDouble(NovemberMultiplier);
                                    studentFeeMultiplier.Dec__Multiplier = Convert.ToDouble(DecemberMultiplier);

                                    studentFeeMultiplier.Jan_StatusPaid = false;
                                    studentFeeMultiplier.Feb_StatusPaid = false;
                                    studentFeeMultiplier.Mar_StatusPaid = false;
                                    studentFeeMultiplier.April_StatusPaid = false;
                                    studentFeeMultiplier.May_StatusPaid = false;
                                    studentFeeMultiplier.June_StatusPaid = false;
                                    studentFeeMultiplier.July_StatusPaid = false;
                                    studentFeeMultiplier.Aug_StatusPaid = false;
                                    studentFeeMultiplier.Sep_StatusPaid = false;
                                    studentFeeMultiplier.Oct_StatusPaid = false;
                                    studentFeeMultiplier.Nov_StatusPaid = false;
                                    studentFeeMultiplier.Dec_StatusPaid = false;
                                    studentFeeMultiplier.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                                    studentFeeMultiplier.StudentId = StudentExist.Id;
                                    studentFeeMultiplier.SesisonID = ActiveSessionId;


                                    db.StudentFeeMultipliers.Add(studentFeeMultiplier);
                                    db.SaveChanges();

                                }
                                else
                                {
                                    StudentFeeMsg = ":Fee is Already created of this student";

                                    throw new System.ArgumentException(":Fee is Already created of this student");
                                }

                            }

                        }
                        else
                        {
                            ViewBag.Error = "Error in Row " + RowNumber + " Please Enter Valid UserName";
                            dbTransaction.Dispose();

                            return View("StudentsLoader");
                        }

                    }
                    dbTransaction.Commit();
                    return RedirectToAction("StudentIndex");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();

                ViewBag.Error = "Error in Row " + RowNumber + " " + StudentFeeMsg;

                return View("StudentsLoader");
            }
        }
        public JsonResult GetStudents(DataTablesParam param)
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;

            if (User.IsInRole("Accountant"))
            {
                //var studentList = (from stdnt in db.AspNetStudents
                //                join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                //                join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                //                where stdnt.UserId == usr.Id && usr.StatusId != 2 // && stdnt.BranchId == branchId
                //                select new { stdnt.Name, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name }).Distinct().ToList();

                //List<Students> stdList = new List<Students>();
                //foreach (var item in studentList)
                //{
                //    Students s = new Students();
                //    s.Name = item.Name;
                //    s.RollNo = item.RollNo;
                //    s.PhoneNo = item.CellNo;
                //    s.JoiningDate = item.JoiningDate.ToString();
                //    s.ClassName = item.ClassName;
                //    stdList.Add(s);
                //}

                //  var studentList = db.AllStudentsList().ToList();


                int pageNo = 1;

                if (param.iDisplayStart >= param.iDisplayLength)
                {

                    pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;

                }

                int totalCount = 0;

                if (param.sSearch != null)
                {
                    totalCount = db.AllStudentsList().Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Count();

                    var studentList = db.AllStudentsList().Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                    return Json(new
                    {
                        aaData = studentList,
                        sEcho = param.sEcho,
                        iTotalDisplayRecords = totalCount,
                        iTotalRecords = totalCount

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    totalCount = db.AllStudentsList().Count();
                    var studentList = db.AllStudentsList().Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();
                    return Json(new
                    {
                        aaData = studentList,
                        sEcho = param.sEcho,
                        iTotalDisplayRecords = totalCount,
                        iTotalRecords = totalCount

                    }, JsonRequestBehavior.AllowGet);
                }




            }

            else if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                    .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                    .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            int pageNo1 = 1;

            if (param.iDisplayStart >= param.iDisplayLength)
            {

                pageNo1 = (param.iDisplayStart / param.iDisplayLength) + 1;

            }
            int totalCount1 = 0;

            if (param.sSearch != null)
            {


                totalCount1 = (from stdnt in db.AspNetStudents
                               join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                               join stufee in db.StudentFees
                               on stdnt.Id equals stufee.StudentID into egroup
                               from stufee in egroup.DefaultIfEmpty()
                               join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                               where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                               select new { stdnt.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Distinct().Count();

                var studentList = (from stdnt in db.AspNetStudents
                                   join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                   join stufee in db.StudentFees
                                   on stdnt.Id equals stufee.StudentID into egroup
                                   from stufee in egroup.DefaultIfEmpty()
                                   join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                   where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                                   select new { stdnt.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Distinct().OrderBy(x => x.Name).Skip((pageNo1 - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();

                return Json(new
                {
                    aaData = studentList,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = totalCount1,
                    iTotalRecords = totalCount1

                }, JsonRequestBehavior.AllowGet);

            }

            else
            {

                totalCount1 = (from stdnt in db.AspNetStudents
                               join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                               join stufee in db.StudentFees
                               on stdnt.Id equals stufee.StudentID into egroup
                               from stufee in egroup.DefaultIfEmpty()
                               join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                               where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                               select new { stdnt.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Distinct().Count();


                var studentList = (from stdnt in db.AspNetStudents
                                   join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                   join stufee in db.StudentFees
                                   on stdnt.Id equals stufee.StudentID into egroup
                                   from stufee in egroup.DefaultIfEmpty()
                                   join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                   where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                                   select new { stdnt.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Distinct().OrderBy(x => x.Name).Skip((pageNo1 - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


                return Json(new
                {
                    aaData = studentList,
                    sEcho = param.sEcho,
                    iTotalDisplayRecords = totalCount1,
                    iTotalRecords = totalCount1

                }, JsonRequestBehavior.AllowGet);
            }

            //  var students = db.AspNetStudents.ToList();

            //List<Students> std = new List<Students>();
            //foreach (var item in students)
            //{
            //    Students s = new Students();
            //    s.Name = item.Name;
            //    s.RollNo = item.RollNo;
            //    s.PhoneNo = item.CellNo;
            //    s.JoiningDate = item.JoiningDate.ToString();
            //    s.ClassName = item.ClassName;
            //    std.Add(s);
            //}

            //return Json(students, JsonRequestBehavior.AllowGet);




        }
        public ActionResult SiblingList()
        {

            var AllSiblingIds = db.AspNetStudents.OrderBy(x => x.SiblingId).Select(x => x.SiblingId).ToList();

            return View();
        }

        public ActionResult GetSiblingStudentsById(string SiblingId)
        {
            var AllSiblingStudentsList = db.AspNetStudents.Where(x => x.SiblingId == SiblingId).Select(x => new
            {
                x.AspNetUser.Name,
                x.AspNetUser.UserName,
                x.CellNo,
                BranchName = x.AspNetBranch.Name,
                ClassName = x.AspNetClass.Name,
                x.SiblingId,
            }).ToList();

            return Json(AllSiblingStudentsList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllSiblingIds()
        {
            //     var AllSiblingIds =      db.SiblingIds.tolist
            var AllSiblingIds = db.SiblingIds().ToList();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(AllSiblingIds);

            return Content(status);
        }




        [HttpGet]
        public ActionResult CreateSiblings()
        {



            return View();
        }

        [HttpPost]
        public ActionResult CreateSiblings(int? a)
        {

            var Students = Request.Form["StudentId"].Split(',').ToList();
            var SiblingId = Request.Form["SiblingId"];

            var SiblingRecord = db.AspNetStudents.Where(x => x.SiblingId == SiblingId).FirstOrDefault();

                ViewBag.Error = null;
         
                foreach (var item in Students)
                {
                    int StudentId = Convert.ToInt32(item);

                    var StudentToUpdate = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault();
                    StudentToUpdate.SiblingId = SiblingId;
                    db.SaveChanges();
                }
           
            
            return RedirectToAction("SiblingList");
        }

        public ActionResult SiblingExist(string SiblingId)
        {

            var SiblingExist = db.AspNetStudents.Where(x => x.SiblingId == SiblingId).FirstOrDefault();
            var ErrorMsg = "No";
            if (SiblingExist != null)
            {
                ErrorMsg = "Yes";
            }


            return Json(ErrorMsg, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAllStudents()
        {
            var loggedInUserId = User.Identity.GetUserId();

            var branchId = db.AspNetEmployees.Where(x => x.UserId == loggedInUserId).FirstOrDefault().BranchId;



            var students = (from stdnt in db.AspNetStudents
                            join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                            join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                            where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                            select new { stdnt.Name, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name }).Distinct().ToList();

            //  var students = db.AspNetStudents.ToList();

            List<Students> std = new List<Students>();
            foreach (var item in students)
            {
                Students s = new Students();
                s.Name = item.Name;
                s.RollNo = item.RollNo;
                s.PhoneNo = item.CellNo;
                s.JoiningDate = item.JoiningDate.ToString();
                s.ClassName = item.ClassName;
                std.Add(s);
            }

            return Json(std, JsonRequestBehavior.AllowGet);
        }
        public class Students
        {
            public string Name { get; set; }
            public string RollNo { get; set; }
            public string ClassName { get; set; }
            public string PhoneNo { get; set; }
            public string JoiningDate { get; set; }
        }
        [HttpGet]
        public ActionResult DisabledStudents()
        {
            return View();
        }
        public ActionResult GetDiabledStudents()
        {
            var students = (from stdnt in db.AspNetStudents
                            join usr in db.AspNetUsers
                            on stdnt.UserId equals usr.Id
                            where stdnt.UserId == usr.Id && usr.StatusId == 2
                            select new { stdnt.Name, stdnt.RollNo, stdnt.CellNo }).ToList();



            List<Students> std = new List<Students>();
            foreach (var item in students)
            {
                Students s = new Students();
                s.Name = item.Name;
                s.RollNo = item.RollNo;
                s.PhoneNo = item.CellNo;
                //s.Class = item.AspNetClass.Name;
                std.Add(s);
            }
            return Json(std, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DisableStudent(int id)
        {
            var uid = db.AspNetStudents.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            AspNetUser users = db.AspNetUsers.Where(x => x.Id == uid).FirstOrDefault();
            users.StatusId = 2;
            db.SaveChanges();
            return RedirectToAction("StudentIndex", "AspNetStudents");
        }
        public ActionResult EnableStudent(int id)
        {
            var uid = db.AspNetStudents.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            AspNetUser users = db.AspNetUsers.Where(x => x.Id == uid).FirstOrDefault();
            users.StatusId = 1;
            db.SaveChanges();
            return RedirectToAction("StudentIndex", "AspNetStudents");
        }

        public JsonResult StudentsList(int id)
        {

            var loggedInUserId = User.Identity.GetUserId();
            int branchId;

            if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            if (id != 0)
            {
                var students = (from stdnt in db.AspNetStudents
                                join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId && stdnt.ClassId == id
                                select new { stdnt.Name, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name }).Distinct().ToList();
                //  var student = db.AspNetStudents.Where(x => x.ClassId == id).ToList();
                List<Students> studentlist = new List<Students>();
                foreach (var item in students)
                {
                    Students s = new Students();
                    s.Name = item.Name;
                    s.RollNo = item.RollNo;
                    s.PhoneNo = item.CellNo;
                    s.JoiningDate = item.JoiningDate.ToString();
                    s.ClassName = item.ClassName;
                    studentlist.Add(s);
                }
                return Json(studentlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var students = (from stdnt in db.AspNetStudents
                                join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                                select new { stdnt.Name, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name }).Distinct().ToList();
                List<Students> std = new List<Students>();
                foreach (var item in students)
                {
                    Students s = new Students();
                    s.Name = item.Name;
                    s.RollNo = item.RollNo;
                    s.PhoneNo = item.CellNo;
                    s.JoiningDate = item.JoiningDate.ToString();
                    s.ClassName = item.ClassName;
                    std.Add(s);
                }
                return Json(std, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult StudentClass()
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;
            if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            var students = (from stdnt in db.AspNetStudents
                            join usr in db.AspNetUsers
                            on stdnt.UserId equals usr.Id
                            where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId
                            select new { usr.Id, Name = stdnt.Name + "(" + usr.UserName + ")", stdnt.RollNo, stdnt.CellNo, usr.Image }).OrderBy(x => x.Name).ToList();

            ViewBag.StudentList = new SelectList(students, "Id", "Name");

            List<AllBranchClass> AllBranchClasses = new List<AllBranchClass>();

            var AllBranches = db.AspNetBranches.ToList();
            var AllClasses = db.AspNetClasses.ToList();


            foreach (var branch in AllBranches)
            {
                var BranchClasses = db.AspNetBranch_Class.Where(x => x.BranchId == branch.Id).ToList();

                foreach (var branchClass in BranchClasses)
                {
                    AllBranchClass obj = new AllBranchClass();

                    obj.BranchClassId = branchClass.Id;
                    obj.BranchClassName = branchClass.AspNetBranch.Name + "-" + branchClass.AspNetClass.Name;

                    AllBranchClasses.Add(obj);

                }


            }


            //var AllBranchClasses = (from clas in db.AspNetClasses
            //                        join branchClass in db.AspNetBranch_Class on clas.Id equals branchClass.ClassId
            //                        where branchClass.BranchId == branchId && branchClass.IsActive == true
            //                        select new { classId = clas.Id, className = clas.Name, branchClassId = branchClass.Id }).OrderBy(x => x.className);

            ViewBag.ClassId = new SelectList(AllBranchClasses, "BranchClassId", "BranchClassName");

            return View();

        }
        public class AllBranchClass
        {
            public int BranchClassId { get; set; }
            public string BranchClassName { get; set; }
        }

        public ActionResult CheckStudentClassAndCourses(string StudentId)
        {
            AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == StudentId).FirstOrDefault();

            string IsStudentEntroll = "No";
            var ClassId = "";
            if (Student != null)
            {

                //if(Student.ClassId != null)
                //{

                //    IsStudentEntroll = "Yes";

                //}

                var AllStudents = db.AspNetStudent_Enrollments.Where(x => x.StudentId == Student.Id).FirstOrDefault();
                // var className = db.AspNetStudent_Enrollments.Where(x => x.StudentId == Student.Id).Select(x => x.AspNetClass_Courses.AspNetClass.Name).FirstOrDefault();

                var className = db.AspNetClasses.Where(x => x.Id == Student.ClassId).FirstOrDefault().Name;
                ClassId = Student.ClassId.ToString();

                var SectionName = db.AspNetStudent_Enrollments.Where(x => x.StudentId == Student.Id).Select(x => x.AspNetBranchClass_Sections.AspNetSection.Name).FirstOrDefault();
                if (AllStudents != null)
                {
                    IsStudentEntroll = className + "-" + SectionName;
                }
            }

            return Json(new { IsStudentEntroll = IsStudentEntroll, ClassId = ClassId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSections(int ClassId)
        {
            //var loggedInUserId = User.Identity.GetUserId();

            //var branchIds = db.AspNetBranch_Admins
            //.Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
            //.Select(branchAdmin => branchAdmin.BranchId).ToList();

            //List<int> AllBranchClassIds = db.AspNetBranch_Class.Where(x => x.ClassId == ClassId && x.IsActive == true && branchIds.Contains(x.BranchId)).Select(x => x.Id).ToList();
            //var AllBranchClassSections = (from branchClassSection in db.AspNetBranchClass_Sections
            //                              join section in db.AspNetSections on branchClassSection.SectionId equals section.Id
            //                              where AllBranchClassIds.Contains(branchClassSection.BranchClassId) && branchClassSection.IsActive == true
            //                              select new { sectionName = section.Name, sectionId = section.Id, branchClassSectionId = branchClassSection.Id }).OrderBy(x => x.sectionName).Distinct(); ;

            //string status = Newtonsoft.Json.JsonConvert.SerializeObject(AllBranchClassSections);
            //return Content(status);


            var Sections = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == ClassId).Select(x => new { x.Id, x.AspNetSection.Name });

            //var Sections = (from section in db.AspNetSections
            //                select new
            //                {
            //                    section.Id,
            //                    section.Name,

            //                }).Distinct();


            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);
            return Content(status);


        }
        public ActionResult GetCourses(int ClassId)
        {

            //var CoursesList = from course in db.AspNetCourses
            //                  join classCourses in db.AspNetClass_Courses on course.Id equals classCourses.CourseId
            //                  where classCourses.ClassId == ClassId && classCourses.IsMandatory == false && classCourses.IsActive == true
            //                  select new { classCourses.Id, course.Name };

            var BranchClass = db.AspNetBranch_Class.Where(x => x.Id == ClassId).FirstOrDefault().ClassId;
            var CoursesList = db.AspNetClass_Courses.Where(x => x.ClassId == BranchClass).Select(x => new { x.Id, x.AspNetCours.Name }).OrderBy(x => x.Name).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(CoursesList);


            return Content(status);

        }


        [HttpGet]
        public ActionResult StudentEntrollment(int a)
        {


            return View();
        }


        [HttpPost]
        public ActionResult StudentEntrollment()
        {
            var loggedInUserId = User.Identity.GetUserId();

            var branchId = db.AspNetBranch_Admins.Where(x => x.AdminId == loggedInUserId).Select(x => x.BranchId).FirstOrDefault();


            var ClassId = Request.Form["ClassId"];
            var StudentId = Request.Form["StudentList"];
            var SectionId = Request.Form["SectionId"];
            var Courses = Request.Form["Courses"];

            int BranchClassId = Convert.ToInt32(ClassId);
            int BranchClassSsectionID = Convert.ToInt32(SectionId);

            // this commented code get all mandatory sebjects and add in the coming subject list.
            //var MandatoryCoursesList = (from course in db.AspNetCourses
            //                            join classCourses in db.AspNetClass_Courses on course.Id equals classCourses.CourseId
            //                            where classCourses.ClassId == ClassIdInt && classCourses.IsMandatory == true && classCourses.IsActive == true
            //                            select new { classCourses.Id }).ToList();

            List<string> selectedsubjects = new List<string>();

            selectedsubjects.AddRange(Request.Form["Courses"].Split(',').ToList());

            //if (MandatoryCoursesList.Count != 0)
            //{

            //    foreach (var mandatorycourses in MandatoryCoursesList)
            //    {
            //        selectedsubjects.Add(mandatorycourses.Id.ToString());
            //    }
            //}

            AspNetStudent aspNetStudent = db.AspNetStudents.Where(x => x.UserId == StudentId).FirstOrDefault();
            var BranchClassObj = db.AspNetBranch_Class.Where(x => x.Id == BranchClassId).FirstOrDefault();

            aspNetStudent.ClassId = BranchClassObj.ClassId;
            aspNetStudent.BranchId = BranchClassObj.BranchId;
            db.SaveChanges();

            List<AspNetStudent_Enrollments> StudentEntrollmentList = db.AspNetStudent_Enrollments.Where(x => x.StudentId == aspNetStudent.Id).ToList();

            if (StudentEntrollmentList.Count != 0)
            {
                db.AspNetStudent_Enrollments.RemoveRange(StudentEntrollmentList);
                db.SaveChanges();

            }

            if (aspNetStudent != null)
            {
                //var courselist = db.AspNetClass_Courses.Where(x => x.ClassId == ClassIdInt).Select(x => x.Id).ToList();

                foreach (var item in selectedsubjects)
                {
                    AspNetStudent_Enrollments se = new AspNetStudent_Enrollments();
                    se.StudentId = aspNetStudent.Id;
                    se.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                    se.CourseId = Convert.ToInt32(item);

                    //var BCid = db.AspNetBranch_Class.Where(x => x.ClassId == ClassIdInt && x.BranchId == aspNetStudent.BranchId).Select(x => x.Id).FirstOrDefault();
                    se.SectionId = BranchClassSsectionID; //db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid && x.SectionId == SectionIdInt).Select(x => x.Id).FirstOrDefault();
                    db.AspNetStudent_Enrollments.Add(se);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("StudentClass", "AspNetStudents");
        }

        public JsonResult GetUserName(string userName)
        {
            check Check = new check();
            Check.count = 0;
            try
            {
                var user = db.AspNetUsers.Where(x => x.UserName == userName);
                if (user.Count() > 0)
                {
                    Check.count = 1;
                    Check.by = user.Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
                }
                else
                {
                    Check.count = 0;
                }
            }
            catch
            {
                Check.count = 0;
            }

            return Json(Check, JsonRequestBehavior.AllowGet);
        }

        public class check
        {
            public int count { get; set; }
            public string by { get; set; }
        }
        // GET: AspNetStudents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetStudent aspNetStudent = db.AspNetStudents.Find(id);
            if (aspNetStudent == null)
            {
                return HttpNotFound();
            }
            return View(aspNetStudent);
        }



        public ActionResult ClassDDL(int id)
        {
            var lst = (from b in db.AspNetBranch_Class.Where(x => x.BranchId == id) join c in db.AspNetClasses on b.ClassId equals c.Id select new { c.Name }).ToList();


            string list = Newtonsoft.Json.JsonConvert.SerializeObject(lst);

            return Content(list);
        }

        // GET: AspNetStudents/Create
        public ActionResult Create()
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



        // POST: AspNetStudents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // POST: AspNetStudents/Create

        [HttpPost]
        public async Task<ActionResult> Create(StudentRegistrationViewModel studentRegistrationViewModel /*, HttpPostedFileBase file*/ )
        {
            var IsStudentCreated = "No";
            var UserId = "";
            var dbTransaction = db.Database.BeginTransaction();

            try
            {

                int ActiveSessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;

                AspNetStudent aspNetStudent = new AspNetStudent();

                aspNetStudent.Name = studentRegistrationViewModel.Name;
                aspNetStudent.RollNo = studentRegistrationViewModel.RollNo;
                aspNetStudent.BranchId = studentRegistrationViewModel.BranchId;
                aspNetStudent.ClassId = studentRegistrationViewModel.ClassId;
                //aspNetStudent.Email = studentRegistrationViewModel.Email;
                aspNetStudent.NationalityId = studentRegistrationViewModel.NationalityId;
                aspNetStudent.ReligionId = studentRegistrationViewModel.ReligionId;
                aspNetStudent.GenderId = studentRegistrationViewModel.GenderId;
                aspNetStudent.Address = studentRegistrationViewModel.Address;
                aspNetStudent.Birthdate = studentRegistrationViewModel.Birthdate;
                aspNetStudent.CellNo = studentRegistrationViewModel.CellNo;
                aspNetStudent.File = studentRegistrationViewModel.File;

                //  var dbTransaction = db.Database.BeginTransaction();


                var Email = studentRegistrationViewModel.Email;
                var FName = studentRegistrationViewModel.FName;
                var MName = studentRegistrationViewModel.MName;
                var LName = studentRegistrationViewModel.LName;

                var FullName = FName + " " + MName + " " + LName;
                var Password = studentRegistrationViewModel.Password;

                if (ModelState.IsValid)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    var user = new ApplicationUser { UserName = studentRegistrationViewModel.RollNo, Email = Email, Name = FullName, PhoneNumber = aspNetStudent.CellNo };
                    var result = await UserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {


                        aspNetStudent.Name = FullName;
                        aspNetStudent.UserId = user.Id;

                        db.AspNetStudents.Add(aspNetStudent);


                        var roleStore = new RoleStore<IdentityRole>(context);
                        var roleManager = new RoleManager<IdentityRole>(roleStore);
                        var userStore = new UserStore<ApplicationUser>(context);
                        var userManager = new UserManager<ApplicationUser>(userStore);
                        userManager.AddToRole(user.Id, "Student");
                        db.SaveChanges();

                        char[] charArray = FName.ToCharArray();
                        var fletter = charArray[0].ToString();
                        AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetStudent.UserId).FirstOrDefault();

                        student.StatusId = 1;
                        db.SaveChanges();

                        ruffdata rd = new ruffdata();
                        rd.Name = FullName;
                        rd.UserName = studentRegistrationViewModel.RollNo;
                        rd.Password = Password;
                        rd.CreationDate = DateTime.Now;
                        db.ruffdatas.Add(rd);
                        db.SaveChanges();


                        StudentFee studentFee = new StudentFee();

                        studentFee.StudentID = aspNetStudent.Id;
                        studentFee.TutionFee = studentRegistrationViewModel.TutionFee;
                        studentFee.ComputerFee = studentRegistrationViewModel.ComputerFee;
                        studentFee.LabCharges = studentRegistrationViewModel.LabCharges;
                        studentFee.OtherServices = studentRegistrationViewModel.OtherServices;
                        studentFee.AdmissionFee = studentRegistrationViewModel.AdmissionFee;
                        studentFee.Total = studentRegistrationViewModel.TotalFee;
                        studentFee.DiscountTutionFee = studentRegistrationViewModel.DiscountTutionFee;
                        studentFee.DiscountLabCharges = studentRegistrationViewModel.DiscountLabCharges;
                        studentFee.DiscountOtherServices = studentRegistrationViewModel.DiscountOtherServices;
                        studentFee.DiscountAdmissionFee = studentRegistrationViewModel.DiscountAdmissionFee;
                        studentFee.DiscountComputerFee = studentRegistrationViewModel.DiscountComputerFee;
                        studentFee.DiscountTotal = studentRegistrationViewModel.DiscountTotal;
                        studentFee.SessionID = ActiveSessionId;
                        studentFee.DiscountTutionFeeAmount = studentRegistrationViewModel.DiscountTutionFeeAmount;
                        studentFee.DiscountComputerFeeAmount = studentRegistrationViewModel.DiscountComputerFeeAmount;
                        studentFee.DiscountLabChargesAmount = studentRegistrationViewModel.DiscountLabChargesAmount;
                        studentFee.DiscountOtherServicesAmount = studentRegistrationViewModel.DiscountOtherServicesAmount;
                        studentFee.DiscountAdmissionFeeAmount = studentRegistrationViewModel.DiscountAdmissionFeeAmount;
                        studentFee.DiscountTotalAmount = studentRegistrationViewModel.DiscountTotalAmount;
                        studentFee.TotalWithoutAdmission = studentFee.DiscountTotalAmount - studentFee.DiscountAdmissionFeeAmount;

                        studentFee.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();

                        db.StudentFees.Add(studentFee);
                        db.SaveChanges();



                        StudentFeeMultiplier studentFeeMultiplier = new StudentFeeMultiplier();
                        studentFeeMultiplier.Jan_Multiplier = studentRegistrationViewModel.Jan_Multiplier;
                        studentFeeMultiplier.Feb_Multiplier = studentRegistrationViewModel.Feb_Multiplier;
                        studentFeeMultiplier.Mar_Multiplier = studentRegistrationViewModel.Mar_Multiplier;
                        studentFeeMultiplier.April__Multiplier = studentRegistrationViewModel.April__Multiplier;
                        studentFeeMultiplier.May_Multiplier = studentRegistrationViewModel.May_Multiplier;
                        studentFeeMultiplier.June_Multiplier = studentRegistrationViewModel.June_Multiplier;
                        studentFeeMultiplier.July__Multiplier = studentRegistrationViewModel.July__Multiplier;
                        studentFeeMultiplier.Aug_Multiplier = studentRegistrationViewModel.Aug_Multiplier;
                        studentFeeMultiplier.Sep_Multiplier = studentRegistrationViewModel.Sep_Multiplier;
                        studentFeeMultiplier.Oct_Multiplier = studentRegistrationViewModel.Oct_Multiplier;
                        studentFeeMultiplier.Nov_Multiplier = studentRegistrationViewModel.Nov_Multiplier;
                        studentFeeMultiplier.Dec__Multiplier = studentRegistrationViewModel.Dec__Multiplier;
                        studentFeeMultiplier.SesisonID = ActiveSessionId;


                        studentFeeMultiplier.Jan_StatusPaid = false;
                        studentFeeMultiplier.Feb_StatusPaid = false;
                        studentFeeMultiplier.Mar_StatusPaid = false;
                        studentFeeMultiplier.April_StatusPaid = false;
                        studentFeeMultiplier.May_StatusPaid = false;
                        studentFeeMultiplier.June_StatusPaid = false;
                        studentFeeMultiplier.July_StatusPaid = false;
                        studentFeeMultiplier.Aug_StatusPaid = false;
                        studentFeeMultiplier.Sep_StatusPaid = false;
                        studentFeeMultiplier.Oct_StatusPaid = false;
                        studentFeeMultiplier.Nov_StatusPaid = false;
                        studentFeeMultiplier.Dec_StatusPaid = false;
                        studentFeeMultiplier.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                        studentFeeMultiplier.StudentId = aspNetStudent.Id;


                        db.StudentFeeMultipliers.Add(studentFeeMultiplier);
                        db.SaveChanges();

                        IsStudentCreated = "Yes";
                        UserId = user.Id;
                        string Error = "Student successfully saved.";

                        dbTransaction.Commit();

                        TempData["StudentCreated"] = "Created";
                        //return RedirectToAction("StudentIndex", "AspNetStudents", new { Error });
                        return Json(new { IsStudentCreated = IsStudentCreated, UserId = UserId }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    IsStudentCreated = "No";
                    UserId = "";
                    dbTransaction.Dispose();
                    TempData["ErrorMessage"] = "Student not created.";

                    return Json(new { IsStudentCreated = IsStudentCreated, UserId = UserId }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                IsStudentCreated = "No";
                UserId = "";
                var msg = ex.Message;
                dbTransaction.Dispose();
                // return RedirectToAction("Create", "AspNetStudents");
                return Json(new { IsStudentCreated = IsStudentCreated, UserId = UserId }, JsonRequestBehavior.AllowGet);

            }
            //ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetStudent.BranchId);
            //ViewBag.PackageId = new SelectList(db.AspNetPackages, "Id", "Name");
            //ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetStudent.ClassId);
            //ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetStudent.GenderId);
            //ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetStudent.NationalityId);
            //ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetStudent.ReligionId);

            return Json(new { IsStudentCreated = IsStudentCreated, UserId = UserId }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadImageOfStudent(string UserId)
        {

            var File = Request.Files["file"];
            var fileName = "";

            if (File != null)
            {

                if (File.ContentLength > 0)
                {
                    // fileName = Path.GetFileName(File.FileName);
                    // File.SaveAs(Server.MapPath("~/Content/TeacherSubmittedAssignment/") + fileName);


                    var name = Path.GetFileNameWithoutExtension(File.FileName);

                    var ext = Path.GetExtension(File.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    fileName = name + "" + ext;


                    File.SaveAs(Server.MapPath("~/Content/Images/StudentImages/") + fileName);
                }
            }

            AspNetUser User = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

            User.Image = fileName;
            db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFeeHeads(int BranchId, int ClassId)
        {

            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;
            var ClassFee = db.ClassFees.Where(x => x.BranchClassID == BranchClassId).Select(x => new { x.TutionFee, x.AdmissionFee, x.LabCharges, x.OtherServices, x.ComputerFee, x.Total }).FirstOrDefault();
            return Json(ClassFee, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Test1()
        {
            var users = db.AspNetUsers.Where(x => x.UserName.Contains("BB-")).ToList();

            //foreach(var item  in users)
            //{

            //    var user = new AspNetUser();
            //    user.Email = item.Email;
            //    user.Name = item.Name;
            //    user.UserName = item.UserName;
            //    string Password = item.PasswordHash;

            //    SendMail(user.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user.Name, user.UserName, Password));

            //}

            //var user = new AspNetUser();
            //user.Email = "fatamahahmad2096@gmail.com";
            //user.Name = "Dur-e-ain Fatima";
            //user.UserName = "BB-1003";
            //string Password = "Fatima@6770";
            //     SendMail(user.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user.Name, user.UserName, Password));


            //var user1 = new AspNetUser();
            //user1.Email = "shafqatfarah7@gmail.com";
            //user1.Name = "Farha Shafqat";
            //user1.UserName = "BB-1004";
            //string Password1 = "Shafqat@8288";
            //SendMail(user1.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user1.Name, user1.UserName, Password1));

            //var user2 = new AspNetUser();
            //user2.Email = "sabasabi5556@gmail.com";
            //user2.Name = "Saba Jamal";
            //user2.UserName = "BB-1005";
            //string Password2 = "Jamal@1923";
            //SendMail(user2.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user2.Name, user2.UserName, Password2));

            //var user3 = new AspNetUser();
            //user3.Email = "zubaidanaqvi786@gmail.com";
            //user3.Name = "Zubaida Hasan";
            //user3.UserName = "BB-1006";
            //string Password3 = "Hasan@9840";
            //SendMail(user3.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user3.Name, user3.UserName, Password3));


            //var user4 = new AspNetUser();
            //user4.Email = "nadiaqalb@yahoo.com";
            //user4.Name = "Nadia Abeer";
            //user4.UserName = "BB-1007";
            //string Password4 = "Abeer@6198";
            //SendMail(user4.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user4.Name, user4.UserName, Password4));

            //var user5 = new AspNetUser();
            //user5.Email = "munizajalil@gmail.com";
            //user5.Name = "Muniza Jalil";
            //user5.UserName = "BB-1008";
            //string Password5 = "Jalil@4700";
            //SendMail(user5.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user5.Name, user5.UserName, Password5));

            //var user4 = new AspNetUser();
            //user4.Email = "mariamaria186@gmail.com";
            //user4.Name = "Maria Irum";
            //user4.UserName = "BB-1009";
            //string Password4 = "Irum@5071";
            //SendMail(user4.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user4.Name, user4.UserName, Password4));

            //var user5 = new AspNetUser();
            //user5.Email = "humfsl83@gmail.com";
            //user5.Name = "Humaira Faisal";
            //user5.UserName = "BB-1010";
            //string Password5 = "Faisal@3653";
            //SendMail(user5.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user5.Name, user5.UserName, Password5));

            //var user4 = new AspNetUser();
            //user4.Email = "eisha29nov@gmail.com";
            //user4.Name = "Aisha Hassan";
            //user4.UserName = "BB-1011";
            //string Password4 = "Hassan@9671";
            //SendMail(user4.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user4.Name, user4.UserName, Password4));

            //var user5 = new AspNetUser();
            //user5.Email = "khadijanaqvi005@gmail.com";
            //user5.Name = "Syeda Khadija Afaq";
            //user5.UserName = "BB-1012";
            //string Password5 = "Afaq@4760";
            //SendMail(user5.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user5.Name, user5.UserName, Password5));


            //var user5 = new AspNetUser();
            //user5.Email = "shehparahaider@hotmail.com";
            //user5.Name = "Shehpara Haider";
            //user5.UserName = "BB-1013";
            //string Password5 = "Haider@9296";
            //SendMail(user5.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user5.Name, user5.UserName, Password5));

            //var user4 = new AspNetUser();
            //user4.Email = "farzanasafeersafeer@gmail.com";
            //user4.Name = "Farzana Safeer";
            //user4.UserName = "BB-1014";
            //string Password4 = "Safeer@8722";
            //SendMail(user4.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user4.Name, user4.UserName, Password4));
            //var user4 = new AspNetUser();
            //user4.Email = "5taholic@gmail.com";
            //user4.Name = "Zeeshan Elahi";
            //user4.UserName = "BB-1017";
            //string Password4 = "Elahi@3623";
            //SendMail(user4.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user4.Name, user4.UserName, Password4));



            //var user5 = new AspNetUser();
            //user5.Email = "talhaghaffar98@gmail.com";
            //user5.Name = "talha";
            //user5.UserName = "BB-1016";
            //string Password5 = "talha@123";
            //SendMail(user5.Email, "GSIS VLEP Login Created", "" + EmailDesign.SignupEmailTemplate(user5.Name, user5.UserName, Password5));



            return RedirectToAction("Index");
        }

        public bool SendMail(string toEmail, string subjeEnumerableDebugViewct, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                string[] EmailList = new string[] { toEmail, "seasupport@theriskadvisors.com" };
                foreach (var item in EmailList)
                {
                    SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                    client.EnableSsl = false;
                    client.Timeout = 100000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    MailMessage mailMessage = new MailMessage(senderEmail, item, "GSIS VLEP Login Created", emailBody);
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = UTF8Encoding.UTF8;

                    client.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                var logs = new AspNetLog();
                logs.Operation = ex.Message + " -----" + ex.InnerException.Message;
                logs.UserId = User.Identity.GetUserId();
                db.AspNetLogs.Add(logs);
                db.SaveChanges();
                return false;
            }

        }

        public JsonResult IsEmailExist(string Email)
        {
            int count;
            try
            {
                var user = db.AspNetUsers.Where(x => x.Email == Email);
                if (user.Count() > 0)
                {
                    count = 1;
                }
                else
                {
                    count = 0;
                }
            }
            catch
            {
                count = 0;
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsRollNoExist(string RollNo)
        {
            int count;
            try
            {
                var user = db.AspNetUsers.Where(x => x.UserName == RollNo);
                if (user.Count() > 0)
                {
                    count = 1;
                }
                else
                {
                    count = 0;
                }
            }
            catch
            {
                count = 0;
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Quiz_student_check()
        {
            TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime PKTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);
            //var fiveMins = System.Data.Entity.DbFunctions.AddMinutes()

            var StdID = User.Identity.GetUserId();
            var student_id = db.AspNetStudents.Where(x => x.UserId == StdID).Select(x => x.Id).FirstOrDefault();
            var student_performed_quizes = db.Student_Quiz_Scoring.Where(x => x.StudentId == student_id && x.AspnetQuiz.IsPublished == true).Select(x => x.AspnetQuiz.Id).Distinct().ToList();
            var all_quizes = db.AspnetQuizs.Where(x => x.IsPublished == true && (x.StartTime <= PKTime && PKTime <= System.Data.Entity.DbFunctions.AddMinutes(x.StartTime, 15))).Select(x => x.Id).ToList();
            var Unperformed_quizes = all_quizes.Except(student_performed_quizes).ToList();

            return Json(Unperformed_quizes, JsonRequestBehavior.AllowGet);
        }


        public class option
        {
            public int id;
            public string name;
        }

        public class question
        {
            public int id;
            public string name;
            public string Description;
            public string type;
            public List<option> options;
            public string selectedOption;
            public string CorrectOption;
            public string Photo;
        }

        public ViewResult Quizes()
        {
            return View();
        }

        public ActionResult QuizDetailsByAdmin(int Id)
        {

            ViewBag.QuizTime = db.AspnetQuizs.Where(x => x.Id == Id).Select(x => x.QuizTime).FirstOrDefault();
            var questionList_MCQS = new List<question>();
            var Quiz_Questions = db.Quiz_Topic_Questions.Where(x => x.QuizId == Id).ToList();

            foreach (var item in Quiz_Questions)
            {
                var q = new question();
                q.id = item.AspnetQuestion.Id;
                q.Photo = item.AspnetQuestion.Photo;
                q.name = item.AspnetQuestion.Name;
                q.type = item.AspnetQuestion.Type;
                q.Description = item.AspnetQuiz.Description;
                q.CorrectOption = item.AspnetQuestion.AspnetOption.Name;
                //q.selectedOption = db.AspnetOptions.Where(x => x.Id == item.SelectedOptionID.Value).FirstOrDefault().Name;
                //  q.CorrectOption = db.AspnetOptions.Where(x => x.Id == item.AspnetQuestion.AnswerId).FirstOrDefault().Name;

                q.options = new List<option>();
                Random r1 = new Random();
                var op = db.AspnetOptions.Where(x => x.QuestionId == q.id).ToList();
                foreach (var item1 in op)
                {
                    var op1 = new option();
                    op1.id = item1.Id;
                    op1.name = item1.Name;

                    q.options.Add(op1);
                }

                questionList_MCQS.Add(q);
            }

            Random r = new Random();
            var myArray = questionList_MCQS.OrderBy(x => r.Next()).ToArray();
            ViewBag.questionList_MCQS = myArray;
            ViewBag.QuizId = Id;
            ViewBag.QuizName = db.AspnetQuizs.Where(x => x.Id == Id).FirstOrDefault().Name;
            ViewBag.QuizDescription = db.AspnetQuizs.Where(x => x.Id == Id).FirstOrDefault().Description;

            return View();
        }

        public ActionResult GetStudentsQuiz()
        {
            var StdID = User.Identity.GetUserId();
            TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);
            //var today = DateTime.Today.Date;
            var subjects_list = db.Student_GenericSubjects.Where(x => x.StudentId == StdID).Select(x => x.GenericSubjectId).ToList();
            //var quizes = db.Quiz_Topic_Questions.Where(x => x.AspnetQuiz.Start_Date == today && today <= x.AspnetQuiz.Due_Date && subjects_list.Contains(x.AspnetQuestion.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId)).ToList();
            var quizes = (from QTQ in db.Quiz_Topic_Questions.Where(x => x.AspnetQuiz.Start_Date <= today && x.AspnetQuiz.IsPublished == true)
                          join Q in db.AspnetQuestions on QTQ.QuestionId equals Q.Id
                          join enrollment in db.AspNetStudent_Enrollments on QTQ.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.CourseId
                          where QTQ.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
                          && QTQ.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId == enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                          && QTQ.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId
                          && enrollment.AspNetStudent.UserId == StdID
                          select new
                          {
                              QuizName = QTQ.AspnetQuiz.Name,
                              QuizDescription = QTQ.AspnetQuiz.Description,
                              StartDate = QTQ.AspnetQuiz.StartTime,
                              DueDate = QTQ.AspnetQuiz.Due_Date.ToString(),
                              Duration = QTQ.AspnetQuiz.QuizTime,
                              Meeting = QTQ.AspnetQuiz.MeetingLink,
                              QuizId = QTQ.AspnetQuiz.Id,
                              Subject = Q.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                          }).Distinct().ToList();


            return Json(quizes, JsonRequestBehavior.AllowGet);

        }
        public ActionResult QuizDetails(int Id, int StudentId)
        {
            ViewBag.QuizTime = db.AspnetQuizs.Where(x => x.Id == Id).Select(x => x.QuizTime).FirstOrDefault();
            var questionList_MCQS = new List<question>();
            var Quiz_Questions = db.Student_Quiz_Scoring.Where(x => x.QuizId == Id && x.StudentId == StudentId).ToList();

            foreach (var item in Quiz_Questions)
            {
                var q = new question();
                q.id = item.AspnetQuestion.Id;
                q.name = item.AspnetQuestion.Name;
                q.type = item.AspnetQuestion.Type;
                q.Photo = item.AspnetQuestion.Photo;

                q.selectedOption = db.AspnetOptions.Where(x => x.Id == item.SelectedOptionID.Value).Select(x => x.Name).FirstOrDefault();
                q.CorrectOption = db.AspnetOptions.Where(x => x.Id == item.AspnetQuestion.AnswerId).Select(x => x.Name).FirstOrDefault();

                q.options = new List<option>();
                Random r1 = new Random();
                var op = db.AspnetOptions.Where(x => x.QuestionId == q.id).ToList();
                foreach (var item1 in op)
                {
                    var op1 = new option();
                    op1.id = item1.Id;
                    op1.name = item1.Name;

                    q.options.Add(op1);
                }

                questionList_MCQS.Add(q);
            }

            Random r = new Random();
            var myArray = questionList_MCQS.OrderBy(x => r.Next()).ToArray();
            ViewBag.questionList_MCQS = myArray;
            ViewBag.QuizId = Id;
            ViewBag.QuizName = db.AspnetQuizs.Where(x => x.Id == Id).FirstOrDefault().Name;
            ViewBag.QuizDescription = db.AspnetQuizs.Where(x => x.Id == Id).FirstOrDefault().Description;
            return View();
        }

        //Question of a Quiz;
        public ActionResult GetQuestions(int Id)
        {
            ViewBag.QuizTime = db.AspnetQuizs.Where(x => x.Id == Id).FirstOrDefault().QuizTime;
            var questionList_MCQS = new List<question>();
            var questionList_Fill = new List<question>();
            var Quiz_Questions = db.Quiz_Topic_Questions.Where(x => x.QuizId == Id && x.AspnetQuestion.Type == "MCQ").ToList();

            foreach (var item in Quiz_Questions)
            {
                var q = new question();
                q.id = item.AspnetQuestion.Id;
                q.name = item.AspnetQuestion.Name;
                q.type = item.AspnetQuestion.Type;
                q.Photo = item.AspnetQuestion.Photo;
                q.options = new List<option>();
                Random r1 = new Random();
                var op = db.AspnetOptions.Where(x => x.QuestionId == q.id).ToList();
                foreach (var item1 in op)
                {
                    var op1 = new option();
                    op1.id = item1.Id;
                    op1.name = item1.Name;

                    q.options.Add(op1);
                }

                questionList_MCQS.Add(q);
            }

            //var Quiz_Questions_Fill = db.Quiz_Topic_Questions.Where(x => x.QuizId == Id && x.AspnetQuestion.Type == "Fill").ToList();

            //foreach (var item in Quiz_Questions_Fill)
            //{
            //    var q = new question();
            //    q.id = item.AspnetQuestion.Id;
            //    q.name = item.AspnetQuestion.Name;
            //    q.type = item.AspnetQuestion.Type;

            //    questionList_Fill.Add(q);
            //}

            Random r = new Random();
            var myArray = questionList_MCQS.OrderBy(x => r.Next()).ToArray();
            ViewBag.questionList_MCQS = myArray;
            ViewBag.QuizId = Id;
            return View();
        }

        public ActionResult StartQuiz_Student(int QuizId)
        {
            try
            {
                string status = "error";
                var StdID = User.Identity.GetUserId();
                var questions = db.Quiz_Topic_Questions.Where(x => x.QuizId == QuizId).Select(x => x.QuestionId).ToList();
                var student_id = db.AspNetStudents.Where(x => x.UserId == StdID).Select(x => x.Id).FirstOrDefault();

                if (db.Student_Quiz_Scoring.Where(x => x.QuizId == QuizId && x.StudentId == student_id).Count() == 0)
                {
                    foreach (var item in questions)
                    {
                        var quiz_student = new Student_Quiz_Scoring();
                        quiz_student.QuizId = QuizId;
                        quiz_student.QuestionId = item;
                        quiz_student.StudentId = student_id;
                        db.Student_Quiz_Scoring.Add(quiz_student);
                    }
                    db.SaveChanges();
                    status = "Success";
                }
                else
                {
                    status = "You've already started the Quiz";
                }

                //db.SaveChanges();
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var logs = new AspNetLog();
                //logs.UserID = StudentID;
                logs.Operation = "Error while starting the Quiz ->" + ex;
                //logs.Time = DateTime.Now;
                db.AspNetLogs.Add(logs);
                db.SaveChanges();
                return Json("Something went Wrong", JsonRequestBehavior.AllowGet);
            }

        }
        public class Quiz
        {
            public List<int> OptionId;
            public int QuizId;
            public List<int> QuestionId;
        }
        public ActionResult submit_question(string Question, string Answer, int QuizID)
        {

            var StdID = User.Identity.GetUserId();

            var student_id = db.AspNetStudents.Where(x => x.UserId == StdID).Select(x => x.Id).FirstOrDefault();
            int score = 0;
            string[] selectedQuestions = Question.Split(',');
            string[] selectedAnswers = Answer.Split(',');

            int i = 0;
            foreach (var item in selectedQuestions)
            {
                var record = db.Student_Quiz_Scoring.Where(x => x.QuizId == QuizID && x.QuestionId.ToString() == item && x.StudentId == student_id).FirstOrDefault();
                var ans = db.AspnetQuestions.Where(x => x.Id.ToString() == item).Select(x => x.AnswerId).FirstOrDefault();
                if (selectedAnswers[i] == ans.ToString())
                {
                    record.SelectedOptionID = Int32.Parse(selectedAnswers[i]);
                    record.Score = "true";
                    score++;
                }
                else
                {
                    if (selectedAnswers[i] == null || selectedAnswers[i] == "")
                    {
                        record.SelectedOptionID = null;
                        record.Score = "false";
                    }
                    else
                    {
                        record.SelectedOptionID = Int32.Parse(selectedAnswers[i]);
                        record.Score = "false";
                    }
                }
                i++;
                db.SaveChanges();
            }

            return Content(score.ToString());
        }

        // GET: AspNetStudents/Edit/5
        public ActionResult Edit(string userName)
        {
            if (userName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // AspNetStudent aspNetStudent = db.AspNetStudents.Find(userName);
            AspNetStudent aspNetStudent = db.AspNetStudents.Where(x => x.RollNo == userName).FirstOrDefault();

            var studentFee = db.StudentFees.Where(x => x.StudentID == aspNetStudent.Id).FirstOrDefault();
            var studentFeeMultiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == aspNetStudent.Id).FirstOrDefault();
            StudentRegistrationViewModel studentRegistrationViewModel = new StudentRegistrationViewModel();

            if (studentFee != null && studentFeeMultiplier != null)
            {
                studentRegistrationViewModel.TutionFee = studentFee.TutionFee.Value;
                studentRegistrationViewModel.ComputerFee = studentFee.ComputerFee.Value;
                studentRegistrationViewModel.LabCharges = studentFee.LabCharges.Value;
                studentRegistrationViewModel.OtherServices = studentFee.OtherServices.Value;
                studentRegistrationViewModel.AdmissionFee = studentFee.AdmissionFee.Value;
                studentRegistrationViewModel.TotalFee = studentFee.Total.Value;
                studentRegistrationViewModel.DiscountTutionFee = studentFee.DiscountTutionFee.Value;
                studentRegistrationViewModel.DiscountLabCharges = studentFee.DiscountLabCharges.Value;
                studentRegistrationViewModel.DiscountOtherServices = studentFee.DiscountOtherServices.Value;
                studentRegistrationViewModel.DiscountAdmissionFee = studentFee.DiscountAdmissionFee.Value;
                studentRegistrationViewModel.DiscountComputerFee = studentFee.DiscountComputerFee.Value;
                studentRegistrationViewModel.DiscountTotal = studentFee.DiscountTotal.Value;
                studentRegistrationViewModel.DiscountTutionFeeAmount = studentFee.DiscountTutionFeeAmount.Value;
                studentRegistrationViewModel.DiscountComputerFeeAmount = studentFee.DiscountComputerFeeAmount.Value;
                studentRegistrationViewModel.DiscountLabChargesAmount = studentFee.DiscountLabChargesAmount.Value;
                studentRegistrationViewModel.DiscountOtherServicesAmount = studentFee.DiscountOtherServicesAmount.Value;
                studentRegistrationViewModel.DiscountAdmissionFeeAmount = studentFee.DiscountAdmissionFeeAmount.Value;
                studentRegistrationViewModel.DiscountTotalAmount = studentFee.DiscountTotalAmount.Value;
                studentRegistrationViewModel.StudentID = aspNetStudent.Id;

                studentRegistrationViewModel.Jan_Multiplier = studentFeeMultiplier.Jan_Multiplier.Value;
                studentRegistrationViewModel.Feb_Multiplier = studentFeeMultiplier.Feb_Multiplier.Value;
                studentRegistrationViewModel.Mar_Multiplier = studentFeeMultiplier.Mar_Multiplier.Value;
                studentRegistrationViewModel.April__Multiplier = studentFeeMultiplier.April__Multiplier.Value;
                studentRegistrationViewModel.May_Multiplier = studentFeeMultiplier.May_Multiplier.Value;
                studentRegistrationViewModel.June_Multiplier = studentFeeMultiplier.June_Multiplier.Value;
                studentRegistrationViewModel.July__Multiplier = studentFeeMultiplier.July__Multiplier.Value;
                studentRegistrationViewModel.Aug_Multiplier = studentFeeMultiplier.Aug_Multiplier.Value;
                studentRegistrationViewModel.Sep_Multiplier = studentFeeMultiplier.Sep_Multiplier.Value;
                studentRegistrationViewModel.Oct_Multiplier = studentFeeMultiplier.Oct_Multiplier.Value;
                studentRegistrationViewModel.Nov_Multiplier = studentFeeMultiplier.Nov_Multiplier.Value;
                studentRegistrationViewModel.Dec__Multiplier = studentFeeMultiplier.Dec__Multiplier.Value;

                //if (aspNetStudent.Birthdate != null)
                //{

                //    DateTime Date = Convert.ToDateTime(aspNetStudent.Birthdate);

                //    string date = Date.ToString("yyyy-MM-dd");

                //    ViewBag.BirthDate = date;
                //}
                //else
                //{
                //    ViewBag.BirthDate = null;
                //}
                ViewBag.BirthDate = null;
            }
            else
            {

                studentRegistrationViewModel.TutionFee = 0;
                studentRegistrationViewModel.ComputerFee = 0;
                studentRegistrationViewModel.LabCharges = 0;
                studentRegistrationViewModel.OtherServices = 0;
                studentRegistrationViewModel.AdmissionFee = 0;
                studentRegistrationViewModel.TotalFee = 0;
                studentRegistrationViewModel.DiscountTutionFee = 0;
                studentRegistrationViewModel.DiscountLabCharges = 0;
                studentRegistrationViewModel.DiscountOtherServices = 0;
                studentRegistrationViewModel.DiscountAdmissionFee = 0;
                studentRegistrationViewModel.DiscountComputerFee = 0;
                studentRegistrationViewModel.DiscountTotal = 0;
                studentRegistrationViewModel.DiscountTutionFeeAmount = 0;
                studentRegistrationViewModel.DiscountComputerFeeAmount = 0;
                studentRegistrationViewModel.DiscountLabChargesAmount = 0;
                studentRegistrationViewModel.DiscountOtherServicesAmount = 0;
                studentRegistrationViewModel.DiscountAdmissionFeeAmount = 0;
                studentRegistrationViewModel.DiscountTotalAmount = 0;
                studentRegistrationViewModel.StudentID = aspNetStudent.Id;

                studentRegistrationViewModel.Jan_Multiplier = 0;
                studentRegistrationViewModel.Feb_Multiplier = 0;
                studentRegistrationViewModel.Mar_Multiplier = 0;
                studentRegistrationViewModel.April__Multiplier = 0;
                studentRegistrationViewModel.May_Multiplier = 0;
                studentRegistrationViewModel.June_Multiplier = 0;
                studentRegistrationViewModel.July__Multiplier = 0;
                studentRegistrationViewModel.Aug_Multiplier = 0;
                studentRegistrationViewModel.Sep_Multiplier = 0;
                studentRegistrationViewModel.Oct_Multiplier = 0;
                studentRegistrationViewModel.Nov_Multiplier = 0;
                studentRegistrationViewModel.Dec__Multiplier = 0;
                ViewBag.BirthDate = null;

            }

            studentRegistrationViewModel.FName = aspNetStudent.Name;
            studentRegistrationViewModel.Email = aspNetStudent.AspNetUser.Email;
            studentRegistrationViewModel.RollNo = aspNetStudent.RollNo;
            studentRegistrationViewModel.CellNo = aspNetStudent.CellNo;

            studentRegistrationViewModel.Birthdate = aspNetStudent.Birthdate;


            if (aspNetStudent == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentImage = aspNetStudent.AspNetUser.Image;
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetStudent.BranchId);

            //var Classes = (from classs in db.AspNetClasses
            //               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
            //               where branchclasssubject.BranchId == aspNetStudent.BranchId
            //               select new
            //               {
            //                   classs.Id,
            //                   classs.Name,
            //               }).Distinct();


            var Classes = db.AspNetBranch_Class.Where(x => x.BranchId == aspNetStudent.BranchId).ToList().Select(x => new { x.AspNetClass.Id, x.AspNetClass.Name });


            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", aspNetStudent.ClassId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetStudent.GenderId);
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetStudent.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetStudent.ReligionId);
            ViewBag.Status = db.AspNetUsers.Where(x => x.UserName == userName).Select(x => x.AspNetStatu.Name).FirstOrDefault();
            ViewBag.BranchName = db.AspNetStudents.Where(x => x.RollNo == userName).Select(x => x.AspNetBranch.Name).FirstOrDefault();
            ViewBag.ClassName = db.AspNetStudents.Where(x => x.RollNo == userName).Select(x => x.AspNetClass.Name).FirstOrDefault();
            ViewBag.Image = db.AspNetUsers.Where(x => x.UserName == userName).Select(x => x.Image).FirstOrDefault();


            return View(studentRegistrationViewModel);
        }


        // POST: AspNetStudents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(StudentRegistrationViewModel studentRegistrationViewModel)
        {
            var IsStudentUpdated = "No";
            var UserId = "";

            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                int ActiveSessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;

                AspNetStudent aspNetStudent = db.AspNetStudents.Where(x => x.Id == studentRegistrationViewModel.StudentID).FirstOrDefault();

                aspNetStudent.Name = studentRegistrationViewModel.Name;
                aspNetStudent.RollNo = studentRegistrationViewModel.RollNo;
                //  aspNetStudent.BranchId = studentRegistrationViewModel.BranchId;
                // aspNetStudent.ClassId = studentRegistrationViewModel.ClassId;
                //aspNetStudent.Email = studentRegistrationViewModel.Email;
                aspNetStudent.NationalityId = studentRegistrationViewModel.NationalityId;
                aspNetStudent.ReligionId = studentRegistrationViewModel.ReligionId;
                aspNetStudent.GenderId = studentRegistrationViewModel.GenderId;
                aspNetStudent.Address = studentRegistrationViewModel.Address;
                aspNetStudent.Birthdate = studentRegistrationViewModel.Birthdate;
                aspNetStudent.CellNo = studentRegistrationViewModel.CellNo;
                aspNetStudent.File = studentRegistrationViewModel.File;


                var Email = studentRegistrationViewModel.Email;
                var FName = studentRegistrationViewModel.FName;
                var MName = studentRegistrationViewModel.MName;
                var LName = studentRegistrationViewModel.LName;

                var FullName = FName + " " + MName + " " + LName;
                aspNetStudent.Name = FullName;
                db.SaveChanges();

                AspNetUser user = db.AspNetUsers.Where(x => x.Id == aspNetStudent.UserId).FirstOrDefault();

                user.Name = FName;
                user.PhoneNumber = studentRegistrationViewModel.CellNo;
                db.SaveChanges();

                StudentFee studentFee = db.StudentFees.Where(x => x.StudentID == studentRegistrationViewModel.StudentID).FirstOrDefault();

                if (studentFee != null)

                {


                    studentFee.TutionFee = studentRegistrationViewModel.TutionFee;
                    studentFee.ComputerFee = studentRegistrationViewModel.ComputerFee;
                    studentFee.LabCharges = studentRegistrationViewModel.LabCharges;
                    studentFee.OtherServices = studentRegistrationViewModel.OtherServices;
                    studentFee.AdmissionFee = studentRegistrationViewModel.AdmissionFee;
                    studentFee.Total = studentRegistrationViewModel.TotalFee;
                    studentFee.DiscountTutionFee = studentRegistrationViewModel.DiscountTutionFee;
                    studentFee.DiscountLabCharges = studentRegistrationViewModel.DiscountLabCharges;
                    studentFee.DiscountOtherServices = studentRegistrationViewModel.DiscountOtherServices;
                    studentFee.DiscountAdmissionFee = studentRegistrationViewModel.DiscountAdmissionFee;
                    studentFee.DiscountComputerFee = studentRegistrationViewModel.DiscountComputerFee;
                    studentFee.DiscountTotal = studentRegistrationViewModel.DiscountTotal;
                    studentFee.DiscountTutionFeeAmount = studentRegistrationViewModel.DiscountTutionFeeAmount;
                    studentFee.DiscountComputerFeeAmount = studentRegistrationViewModel.DiscountComputerFeeAmount;
                    studentFee.DiscountLabChargesAmount = studentRegistrationViewModel.DiscountLabChargesAmount;
                    studentFee.DiscountOtherServicesAmount = studentRegistrationViewModel.DiscountOtherServicesAmount;
                    studentFee.DiscountAdmissionFeeAmount = studentRegistrationViewModel.DiscountAdmissionFeeAmount;
                    studentFee.DiscountTotalAmount = studentRegistrationViewModel.DiscountTotalAmount;
                    studentFee.TotalWithoutAdmission = studentFee.DiscountTotalAmount - studentFee.DiscountAdmissionFeeAmount;

                    db.SaveChanges();

                }


                StudentFeeMultiplier studentFeeMultiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == studentRegistrationViewModel.StudentID).FirstOrDefault();

                if (studentFeeMultiplier != null)
                {

                    studentFeeMultiplier.Jan_Multiplier = studentRegistrationViewModel.Jan_Multiplier;
                    studentFeeMultiplier.Feb_Multiplier = studentRegistrationViewModel.Feb_Multiplier;
                    studentFeeMultiplier.Mar_Multiplier = studentRegistrationViewModel.Mar_Multiplier;
                    studentFeeMultiplier.April__Multiplier = studentRegistrationViewModel.April__Multiplier;
                    studentFeeMultiplier.May_Multiplier = studentRegistrationViewModel.May_Multiplier;
                    studentFeeMultiplier.June_Multiplier = studentRegistrationViewModel.June_Multiplier;
                    studentFeeMultiplier.July__Multiplier = studentRegistrationViewModel.July__Multiplier;
                    studentFeeMultiplier.Aug_Multiplier = studentRegistrationViewModel.Aug_Multiplier;
                    studentFeeMultiplier.Sep_Multiplier = studentRegistrationViewModel.Sep_Multiplier;
                    studentFeeMultiplier.Oct_Multiplier = studentRegistrationViewModel.Oct_Multiplier;
                    studentFeeMultiplier.Nov_Multiplier = studentRegistrationViewModel.Nov_Multiplier;
                    studentFeeMultiplier.Dec__Multiplier = studentRegistrationViewModel.Dec__Multiplier;
                    db.SaveChanges();


                }

                if (studentFee == null && studentFeeMultiplier == null)
                {


                    StudentFee studentFee1 = new StudentFee();

                    studentFee1.StudentID = aspNetStudent.Id;
                    studentFee1.TutionFee = studentRegistrationViewModel.TutionFee;
                    studentFee1.ComputerFee = studentRegistrationViewModel.ComputerFee;
                    studentFee1.LabCharges = studentRegistrationViewModel.LabCharges;
                    studentFee1.OtherServices = studentRegistrationViewModel.OtherServices;
                    studentFee1.AdmissionFee = studentRegistrationViewModel.AdmissionFee;
                    studentFee1.Total = studentRegistrationViewModel.TotalFee;
                    studentFee1.DiscountTutionFee = studentRegistrationViewModel.DiscountTutionFee;
                    studentFee1.DiscountLabCharges = studentRegistrationViewModel.DiscountLabCharges;
                    studentFee1.DiscountOtherServices = studentRegistrationViewModel.DiscountOtherServices;
                    studentFee1.DiscountAdmissionFee = studentRegistrationViewModel.DiscountAdmissionFee;
                    studentFee1.DiscountComputerFee = studentRegistrationViewModel.DiscountComputerFee;
                    studentFee1.DiscountTotal = studentRegistrationViewModel.DiscountTotal;
                    studentFee1.SessionID = ActiveSessionId;
                    studentFee1.DiscountTutionFeeAmount = studentRegistrationViewModel.DiscountTutionFeeAmount;
                    studentFee1.DiscountComputerFeeAmount = studentRegistrationViewModel.DiscountComputerFeeAmount;
                    studentFee1.DiscountLabChargesAmount = studentRegistrationViewModel.DiscountLabChargesAmount;
                    studentFee1.DiscountOtherServicesAmount = studentRegistrationViewModel.DiscountOtherServicesAmount;
                    studentFee1.DiscountAdmissionFeeAmount = studentRegistrationViewModel.DiscountAdmissionFeeAmount;
                    studentFee1.DiscountTotalAmount = studentRegistrationViewModel.DiscountTotalAmount;
                    studentFee1.TotalWithoutAdmission = studentFee1.DiscountTotalAmount - studentFee1.DiscountAdmissionFeeAmount;

                    studentFee1.CreationDate = DateTime.Now;

                    db.StudentFees.Add(studentFee1);
                    db.SaveChanges();

                    StudentFeeMultiplier studentFeeMultiplier1 = new StudentFeeMultiplier();
                    studentFeeMultiplier1.Jan_Multiplier = studentRegistrationViewModel.Jan_Multiplier;
                    studentFeeMultiplier1.Feb_Multiplier = studentRegistrationViewModel.Feb_Multiplier;
                    studentFeeMultiplier1.Mar_Multiplier = studentRegistrationViewModel.Mar_Multiplier;
                    studentFeeMultiplier1.April__Multiplier = studentRegistrationViewModel.April__Multiplier;
                    studentFeeMultiplier1.May_Multiplier = studentRegistrationViewModel.May_Multiplier;
                    studentFeeMultiplier1.June_Multiplier = studentRegistrationViewModel.June_Multiplier;
                    studentFeeMultiplier1.July__Multiplier = studentRegistrationViewModel.July__Multiplier;
                    studentFeeMultiplier1.Aug_Multiplier = studentRegistrationViewModel.Aug_Multiplier;
                    studentFeeMultiplier1.Sep_Multiplier = studentRegistrationViewModel.Sep_Multiplier;
                    studentFeeMultiplier1.Oct_Multiplier = studentRegistrationViewModel.Oct_Multiplier;
                    studentFeeMultiplier1.Nov_Multiplier = studentRegistrationViewModel.Nov_Multiplier;
                    studentFeeMultiplier1.Dec__Multiplier = studentRegistrationViewModel.Dec__Multiplier;
                    studentFeeMultiplier1.SesisonID = ActiveSessionId;


                    studentFeeMultiplier1.Jan_StatusPaid = false;
                    studentFeeMultiplier1.Feb_StatusPaid = false;
                    studentFeeMultiplier1.Mar_StatusPaid = false;
                    studentFeeMultiplier1.April_StatusPaid = false;
                    studentFeeMultiplier1.May_StatusPaid = false;
                    studentFeeMultiplier1.June_StatusPaid = false;
                    studentFeeMultiplier1.July_StatusPaid = false;
                    studentFeeMultiplier1.Aug_StatusPaid = false;
                    studentFeeMultiplier1.Sep_StatusPaid = false;
                    studentFeeMultiplier1.Oct_StatusPaid = false;
                    studentFeeMultiplier1.Nov_StatusPaid = false;
                    studentFeeMultiplier1.Dec_StatusPaid = false;
                    studentFeeMultiplier1.CreationDate = DateTime.Now;
                    studentFeeMultiplier1.StudentId = aspNetStudent.Id;

                    db.StudentFeeMultipliers.Add(studentFeeMultiplier1);
                    db.SaveChanges();


                }

                IsStudentUpdated = "Yes";
                UserId = user.Id;
                TempData["StudentUpdated"] = "Updated";

                dbTransaction.Commit();
                return Json(new { IsStudentUpdated = IsStudentUpdated, UserId = UserId }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                IsStudentUpdated = "No";
                UserId = "";
                dbTransaction.Dispose();

                return Json(new { IsStudentCreated = IsStudentUpdated, UserId = UserId }, JsonRequestBehavior.AllowGet);

            }

            //if (ModelState.IsValid)
            //{
            //    db.Entry(aspNetStudent).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("StudentIndex");
            //}
            //ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetStudent.BranchId);
            //ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetStudent.ClassId);
            //ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetStudent.GenderId);
            //ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetStudent.NationalityId);
            //ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetStudent.ReligionId);
            //return View(aspNetStudent);

        }

        // GET: AspNetStudents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetStudent aspNetStudent = db.AspNetStudents.Find(id);
            if (aspNetStudent == null)
            {
                return HttpNotFound();
            }
            return View(aspNetStudent);
        }

        // POST: AspNetStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetStudent aspNetStudent = db.AspNetStudents.Find(id);
            db.AspNetStudents.Remove(aspNetStudent);
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
