using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Utilities;
using Microsoft.Ajax.Utilities;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Text;
using OfficeOpenXml;
using System.Globalization;

namespace SEA_Application.Controllers
{
    public class ExamsController : Controller
    {
        // GET: Exams
        Sea_Entities db = new Sea_Entities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExamTypeIndex()
        {
            return View();
        }
        public ActionResult ExamDetailList()
        {
            var loggedInUserId = User.Identity.GetUserId();

            var UserRole = db.GetUserRoleById(loggedInUserId).FirstOrDefault().ToString();

            if (UserRole == "Teacher")
            {

                var TeacherCourses = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == loggedInUserId).Select(x => new { BranchId = x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id, ClassId = x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id, SectionId = x.AspNetBranchClass_Sections.AspNetSection.Id }).ToList();

                List<int> TeacherBranchIds = TeacherCourses.Select(x => x.BranchId).Distinct().ToList();
                List<int> TeacherClassIds = TeacherCourses.Select(x => x.ClassId).Distinct().ToList();
                List<int> TeacherSectionIds = TeacherCourses.Select(x => x.SectionId).Distinct().ToList();

                var Exams = db.ExamDetails.Where(x => TeacherBranchIds.Contains(x.BranchId.Value) && TeacherClassIds.Contains(x.ClassId.Value) && TeacherSectionIds.Contains(x.SectionId.Value)).Select(x => new { x.Id, StudentName = x.AspNetStudent.Name, ExamName = x.ExamType.Name, Total = x.ExamMarksDetails.Sum(y => y.Total), Obtained = x.ExamMarksDetails.Sum(y => y.Obtained), x.Grade, x.ParentsRemarks, x.Status }).ToList();

                return Json(Exams, JsonRequestBehavior.AllowGet);

            }
            else if (UserRole == "Student")
            {

                var StudentCourses = db.AspNetStudent_Enrollments.Where(x => x.AspNetStudent.AspNetUser.Id == loggedInUserId).Select(x => new { BranchId = x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id, ClassId = x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id, SectionId = x.AspNetBranchClass_Sections.AspNetSection.Id }).Distinct().ToList();
                List<int> StudentBranchIds = StudentCourses.Select(x => x.BranchId).Distinct().ToList();
                List<int> StudentClassIds = StudentCourses.Select(x => x.ClassId).Distinct().ToList();
                List<int> StudentSectionIds = StudentCourses.Select(x => x.SectionId).Distinct().ToList();

                var Student = db.AspNetStudents.Where(x => x.AspNetUser.Id == loggedInUserId).FirstOrDefault();
                var Exams = db.ExamDetails.Where(x => x.Status == "Published" && StudentBranchIds.Contains(x.BranchId.Value) && StudentClassIds.Contains(x.ClassId.Value) && StudentSectionIds.Contains(x.SectionId.Value) && x.StudentId == Student.Id).Select(x => new { x.Id, StudentName = x.AspNetStudent.Name, ExamName = x.ExamType.Name, Total = x.ExamMarksDetails.Sum(y => y.Total), Obtained = x.ExamMarksDetails.Sum(y => y.Obtained), x.Grade, x.ParentsRemarks }).ToList();

                return Json(Exams, JsonRequestBehavior.AllowGet);

            }
            else if (UserRole == "Branch_Admin")
            {
                var branchId = db.AspNetBranch_Admins
               .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
               .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();


                var Exams = db.ExamDetails.Where(x => x.BranchId == branchId).Select(x => new { x.Id, StudentName = x.AspNetStudent.Name, ExamName = x.ExamType.Name, Total = x.ExamMarksDetails.Sum(y => y.Total), Obtained = x.ExamMarksDetails.Sum(y => y.Obtained), x.Grade, x.ParentsRemarks, x.Status }).ToList();

                return Json(Exams, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ExamDetailsAdminView()
        {


            return View();
        }


        [HttpGet]
        public ActionResult CreateExamType()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateExamType(ExamType examType)
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
                branchId = 0;
                return View();
            }

            ExamType Exam = new ExamType();
            Exam.Name = examType.Name;
            Exam.FromDate = examType.FromDate;
            Exam.ToDate = examType.ToDate;
            Exam.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
            Exam.CreatedBy = loggedInUserId;
            Exam.BranchId = branchId;

            db.ExamTypes.Add(Exam);
            db.SaveChanges();

            return RedirectToAction("ExamTypeIndex");

        }

        [HttpGet]
        public ActionResult EditExamType(int id)
        {

            var ExamTypeToEdit = db.ExamTypes.Where(x => x.Id == id).FirstOrDefault();

            var FromDateString = "";
            var ToDateString = "";

            if (ExamTypeToEdit.FromDate != null)
            {

                DateTime FromDate = Convert.ToDateTime(ExamTypeToEdit.FromDate);

                FromDateString = FromDate.ToString("yyyy-MM-dd");
            }


            ViewBag.FromDate = FromDateString;

            if (ExamTypeToEdit.ToDate != null)
            {

                DateTime ToDate = Convert.ToDateTime(ExamTypeToEdit.ToDate);

                ToDateString = ToDate.ToString("yyyy-MM-dd");
            }


            ViewBag.ToDate = ToDateString;

            return View(ExamTypeToEdit);
        }
        [HttpPost]
        public ActionResult EditExamType(ExamType examType)
        {


            var ExamTypeToUpdate = db.ExamTypes.Where(x => x.Id == examType.Id).FirstOrDefault();

            if (ExamTypeToUpdate != null)
            {
                ExamTypeToUpdate.Name = examType.Name;
                ExamTypeToUpdate.FromDate = examType.FromDate;
                ExamTypeToUpdate.ToDate = examType.ToDate;
                db.SaveChanges();

            }

            return RedirectToAction("ExamTypeIndex");
        }

        public ActionResult ExamTypeList()
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId = 0;
            if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }

            var ExamTypesList = db.ExamTypes.Where(x => x.BranchId == branchId).Select(x => new { x.Id, x.Name, x.FromDate, x.ToDate }).ToList();



            return Json(ExamTypesList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StudentExamDetailView()
        {

            return View();
        }

        public ActionResult CreateExamDetail()
        {

            ViewBag.Error = "";


            return View();
        }
        public ActionResult SaveStudentExamDetail(ExamDetailVM examDetail, List<ExamMarksDetailVM> examMarksDetail)
        {

            var ID = User.Identity.GetUserId();

            var Teacher = db.AspNetEmployees.Where(x => x.UserId == ID).FirstOrDefault();

            var IsExamCreated = "No";
            var UserId = "";
            var dbTransaction = db.Database.BeginTransaction();

            var ExamDetailId = 0;
            try
            {
                var ExamDetailExist = db.ExamDetails.Where(x => x.BranchId == examDetail.BranchId && x.ClassId == examDetail.ClassId && x.SectionId == examDetail.SectionId && x.StudentId == examDetail.StudentId && x.ExamTypeId == examDetail.ExamTypeId).FirstOrDefault();

                if (ExamDetailExist == null)
                {


                    ExamDetail examDetailToAdd = new ExamDetail();

                    examDetailToAdd.BranchId = examDetail.BranchId;
                    examDetailToAdd.ClassId = examDetail.ClassId;
                    examDetailToAdd.SectionId = examDetail.SectionId;
                    examDetailToAdd.StudentId = examDetail.StudentId;
                    examDetailToAdd.Grade = null;//examDetail.Grade;
                    examDetailToAdd.ParentsRemarks = null;
                    examDetailToAdd.ExamTypeId = examDetail.ExamTypeId;
                    examDetailToAdd.Total = examDetail.Total;
                    examDetailToAdd.Obtained = examDetail.Obtained;
                    examDetailToAdd.Status = "Created";
                    //examDetailToAdd.Grade = examDetail.Grade;
                    // examDetailToAdd.
                    examDetailToAdd.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();

                    db.ExamDetails.Add(examDetailToAdd);
                    db.SaveChanges();

                    ExamDetailId = examDetailToAdd.Id;
                }
                else
                {
                    ExamDetailId = ExamDetailExist.Id;
                }


                List<ExamMarksDetail> ExamMarksDetailList = new List<ExamMarksDetail>();

                foreach (var item in examMarksDetail)
                {
                    // var ExamMarkDetailExist = db.ExamMarksDetails.Where(x => x.CourseId == item.CourseId && x.ExamDetail.BranchId == examDetail.BranchId && x.ExamDetail.ClassId == examDetail.ClassId && x.ExamDetail.SectionId == examDetail.SectionId && x.ExamDetail.ExamTypeId == examDetail.ExamTypeId && x.ExamDetail.StudentId == examDetail.StudentId).FirstOrDefault();

                    //if (ExamMarkDetailExist == null)
                    //{
                    ExamMarksDetail obj = new ExamMarksDetail();

                    obj.CourseId = item.CourseId;
                    obj.Total = item.TotalMarks;
                    obj.Obtained = item.ObtainedMarks;
                    obj.Grade = item.SubjectGrade;

                    if (item.ExamDate == null)
                    {
                        obj.ExamDate = null;
                    }
                    else
                    {
                        obj.ExamDate = Convert.ToDateTime(item.ExamDate);

                    }
                    obj.IsAttended = item.IsAttended;
                    obj.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                    obj.TeacherId = Teacher.Id;
                    obj.Comments = item.Comments;
                    obj.ExamDetail_Id = ExamDetailId;

                    ExamMarksDetailList.Add(obj);
                    //}

                }

                db.ExamMarksDetails.AddRange(ExamMarksDetailList);
                db.SaveChanges();

                IsExamCreated = "Yes";

                //  string Error = "Student successfully saved.";

                dbTransaction.Commit();



            }
            catch (Exception ex)
            {

                IsExamCreated = "No";

                dbTransaction.Dispose();
            }


            return Json(new { IsExamCreated = IsExamCreated }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExamsDetailFromFile()
        {

            var dbTransaction = db.Database.BeginTransaction();
            int? RowNumber = null;
            int? ColumnNumber = null;
            var ErrorMsg = "";
            string StudentFeeId = null;
            string StatusMsg = null;
            List<string> ErrorList = new List<string>();

            try
            {

                HttpPostedFileBase file = Request.Files["exams"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                using (var package = new ExcelPackage(file.InputStream))
                {

                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();

                    int BranchId = 0;
                    int ExamId = 0;
                    int ClassId = 0;
                    int SectionId = 0;


                    var BranchName = workSheet.Cells[1, 2].Value;

                    if (BranchName == null)
                    {
                        RowNumber = 1;

                        ErrorList.Add("Error in " + RowNumber + ": Please enter branch name");

                    }
                    else
                    {
                        var BranchExist = db.AspNetBranches.Where(x => x.Name == BranchName.ToString()).FirstOrDefault();

                        if (BranchExist == null)
                        {
                            RowNumber = 1;

                            ErrorList.Add("Error in " + RowNumber + ": Please enter valid branch name");
                            // throw new System.ArgumentException(":Please enter valid branch name");
                        }
                        else
                        {
                            BranchId = BranchExist.Id;
                        }

                    }


                    var ExamName = workSheet.Cells[1, 5].Value;
                    if (ExamName == null)
                    {
                        RowNumber = 1;

                        ErrorList.Add("Error in " + RowNumber + ": Please enter exam name");

                    }
                    else
                    {
                        var ExamExist = db.ExamTypes.Where(x => x.Name == ExamName.ToString()).FirstOrDefault();

                        if (ExamExist == null)
                        {
                            RowNumber = 1;
                            ErrorList.Add("Error in " + RowNumber + ": Please enter valid exam name");

                        }
                        else
                        {
                            ExamId = ExamExist.Id;
                        }


                    }

                    var ClassName = workSheet.Cells[2, 2].Value;

                    if (ClassName == null)
                    {
                        RowNumber = 2;

                        ErrorList.Add("Error in " + RowNumber + ": Please enter class name");

                    }
                    else
                    {
                        var ClassExist = db.AspNetClasses.Where(x => x.Name == ClassName.ToString()).FirstOrDefault();

                        if (ClassExist == null)
                        {
                            RowNumber = 2;

                            ErrorList.Add("Error in " + RowNumber + ": Please enter valid class name");
                        }
                        else
                        {
                            ClassId = ClassExist.Id;

                        }
                    }

                    var SectionName = workSheet.Cells[2, 5].Value;
                    if (SectionName == null)
                    {
                        RowNumber = 2;

                        ErrorList.Add("Error in " + RowNumber + ": Please enter exam name");
                    }
                    else
                    {
                        var SectionExist = db.AspNetSections.Where(x => x.Name == SectionName.ToString()).FirstOrDefault();

                        if (SectionExist == null)
                        {
                            RowNumber = 2;

                            ErrorList.Add("Error in " + RowNumber + ": Please enter valid section name");
                        }
                        else
                        {


                            SectionId = SectionExist.Id;
                        }

                    }

                    List<StudentExamExcel> studentExamExcel = new List<StudentExamExcel>();

                    for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
                    {
                        RowNumber = rowIterator;

                        StudentExamExcel obj = new StudentExamExcel();

                        var StudentId = workSheet.Cells[rowIterator, 1].Value;

                        if (StudentId == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter student Id");
                        }
                        else
                        {
                            var StudentIdExist = db.AspNetStudents.Where(x => x.AspNetUser.UserName == StudentId.ToString()).FirstOrDefault();

                            if (StudentIdExist == null)
                            {
                                ErrorList.Add("Error in " + RowNumber + ": Please enter valid student Id");

                            }
                            else
                            {
                                obj.StudentId = StudentIdExist.Id;

                            }
                        }

                        var SubjectName = workSheet.Cells[rowIterator, 2].Value;

                        if (SubjectName == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter subject name ");

                        }
                        else
                        {
                            var SubjectExist = db.AspNetCourses.Where(x => x.Name == SubjectName.ToString()).FirstOrDefault();

                            if (SubjectExist == null)
                            {
                                ErrorList.Add("Error in " + RowNumber + ": Please enter valid subject name ");

                            }
                            else
                            {
                                obj.SubjectId = SubjectExist.Id;

                            }

                        }

                        var TotalMarks = workSheet.Cells[rowIterator, 3].Value;

                        if (TotalMarks == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter total marks ");

                        }
                        else
                        {
                            double val;

                            var TotalParsed = double.TryParse(TotalMarks.ToString(),
                                            NumberStyles.Float,
                                            CultureInfo.CurrentCulture.NumberFormat,
                                            out val);
                            if (TotalParsed == false)
                            {
                                ErrorList.Add("Error in " + RowNumber + ": Please enter total marks in numeric form ");

                            }
                            else
                            {
                                //TotalMarks
                                obj.Total = val;
                            }



                        }

                        var ObtainedMarks = workSheet.Cells[rowIterator, 4].Value;

                        if (ObtainedMarks == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter obtained marks ");


                        }
                        else
                        {

                            double val;

                            var ObtainedParse = double.TryParse(ObtainedMarks.ToString(),
                                            NumberStyles.Float,
                                            CultureInfo.CurrentCulture.NumberFormat,
                                            out val);
                            if (ObtainedParse == false)
                            {
                                ErrorList.Add("Error in " + RowNumber + ": Please enter obtained marks in numeric form ");

                            }
                            else
                            {
                                //Obtained Marks
                                obj.Obtained = val;
                            }

                        }

                        var ExamDate = workSheet.Cells[rowIterator, 5].Value;

                        if (ExamDate == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter exam date ");

                        }
                        else
                        {

                            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                                               "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                                               "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                                               "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                                               "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm"};

                            DateTime DateValue;
                            bool DateParsed = DateTime.TryParseExact(ExamDate.ToString(), formats,
                                                new CultureInfo("en-US"),
                                                DateTimeStyles.None,
                                                out DateValue);

                            if (DateParsed == false)
                            {

                                ErrorList.Add("Error in " + RowNumber + ": Please enter exam date in mm/dd/yyyy form ");


                            }
                            else
                            {
                                obj.ExamDate = DateValue;
                                //DateValue
                            }


                        }


                        //var IsAttended = workSheet.Cells[rowIterator, 6].Value;

                        //if (IsAttended == null)
                        //{
                        //    ErrorList.Add("Error in " + RowNumber + ": Please enter is attended  ");

                        //}
                        //else
                        //{
                        //    if (IsAttended.ToString() == "Yes" || IsAttended.ToString() == "No")
                        //    {
                        //        obj.IsAttended = false;

                        //        if (IsAttended.ToString() == "Yes")
                        //        {
                        //            obj.IsAttended = true;

                        //        }


                        //    }
                        //    else
                        //    {
                        //        ErrorList.Add("Error in " + RowNumber + ": Is Attended column value could be Yes or No");
                        //    }

                        //}

                        obj.IsAttended = true;


                        var TeacherId = workSheet.Cells[rowIterator, 6].Value;

                        if (TeacherId == null)
                        {
                            ErrorList.Add("Error in " + RowNumber + ": Please enter is teacher id   ");

                        }
                        else
                        {
                            var UserTeacher = db.AspNetUsers.Where(x => x.UserName == TeacherId.ToString()).FirstOrDefault();

                            if (UserTeacher != null)
                            {
                                var Role = db.GetUserRoleById(UserTeacher.Id).FirstOrDefault().ToString();

                                if (Role == "Teacher")
                                {
                                    var Employee = db.AspNetEmployees.Where(x => x.UserId == UserTeacher.Id).FirstOrDefault();
                                    obj.TeacherId = Employee.Id;
                                }
                                else
                                {
                                    ErrorList.Add("Error in " + RowNumber + ": Please enter valid  teacher id   ");

                                }

                            }
                            else
                            {
                                ErrorList.Add("Error in " + RowNumber + ": Please enter valid  teacher id   ");

                            }

                        }

                        var Grade = workSheet.Cells[rowIterator, 7].Value;
                        if (Grade != null)
                        {
                            obj.Grade = Grade.ToString();

                        }

                        var Comments = workSheet.Cells[rowIterator, 8].Value;
                        if (Comments != null)
                        {

                            obj.Comments = Comments.ToString();
                        }



                        studentExamExcel.Add(obj);


                    }//end of for loop


                    if (ErrorList.Count() > 0)
                    {
                        ViewBag.Error = ErrorList;
                        return View("CreateExamDetail");

                    }
                    else
                    {
                        var AllStudents = studentExamExcel.Select(x => x.StudentId).Distinct().ToList();

                        //List<StudentMarks> studentMarks = new List<StudentMarks>();

                        //foreach (var item in AllStudents)
                        //{
                        //    StudentMarks mark = new StudentMarks();

                        //    double Total = 0;
                        //    double Obtained = 0;

                        //    foreach (var item1 in studentExamExcel)
                        //    {

                        //        if (item == item1.StudentId)
                        //        {

                        //            Total = Total + item1.Total;
                        //            Obtained = Obtained + item1.Obtained;
                        //        }

                        //    }

                        //    mark.StudentId = item;
                        //    mark.Total = Total;
                        //    mark.Obtained = Obtained;
                        //    studentMarks.Add(mark);

                        //}


                        List<ExamDetail> examDetailList = new List<ExamDetail>();

                        foreach (var id in AllStudents)
                        {
                            var IsExamDetailExist = db.ExamDetails.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SectionId == SectionId && x.ExamTypeId == ExamId && x.StudentId == id).FirstOrDefault();

                            if (IsExamDetailExist == null)
                            {
                                ExamDetail examDetail = new ExamDetail();

                                examDetail.BranchId = BranchId;
                                examDetail.ClassId = ClassId;
                                examDetail.ExamTypeId = ExamId;
                                examDetail.SectionId = SectionId;
                                examDetail.StudentId = id;
                                examDetail.Grade = null;
                                examDetail.Total = null;//mark.Total;//Total will be counted during result
                                examDetail.Obtained = null;// mark.Obtained;//Total will be counted during result
                                examDetail.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();

                                examDetailList.Add(examDetail);
                            }

                        }

                        db.ExamDetails.AddRange(examDetailList);
                        db.SaveChanges();

                        List<ExamMarksDetail> marksDetailList = new List<ExamMarksDetail>();

                        foreach (var item in studentExamExcel)
                        {
                            ExamMarksDetail marksDetail = new ExamMarksDetail();


                            //  var ExamMarkDetailExist = db.ExamMarksDetails.Where(x => x.CourseId == item.SubjectId && x.ExamDetail.BranchId == BranchId && x.ExamDetail.ClassId == ClassId && x.ExamDetail.SectionId == SectionId && x.ExamDetail.ExamTypeId == ExamId && x.ExamDetail.StudentId == item.StudentId).FirstOrDefault();

                            //if (ExamMarkDetailExist == null)
                            // {
                            var ExamDetail = db.ExamDetails.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SectionId == SectionId && x.ExamTypeId == ExamId && x.StudentId == item.StudentId).FirstOrDefault();

                            marksDetail.CourseId = item.SubjectId;
                            marksDetail.Total = item.Total;
                            marksDetail.Obtained = item.Obtained;
                            marksDetail.ExamDate = item.ExamDate;
                            marksDetail.IsAttended = item.IsAttended;
                            marksDetail.TeacherId = item.TeacherId;
                            marksDetail.Grade = item.Grade;
                            marksDetail.Comments = item.Comments;
                            marksDetail.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                            marksDetail.ExamDetail_Id = ExamDetail.Id;

                            marksDetailList.Add(marksDetail);

                            // }

                        }

                        db.ExamMarksDetails.AddRange(marksDetailList);
                        db.SaveChanges();
                        dbTransaction.Commit();

                    }//end of else 



                }//end of using 
            }//
            catch (Exception ex)
            {
                var msg = ex.Message;
                dbTransaction.Dispose();
                if (ErrorList.Count() == 0)
                {
                    ErrorList.Add("Something went wrong");
                }

                ViewBag.Error = ErrorList;

                return View("CreateExamDetail");

            }

            return RedirectToAction("StudentExamDetailView");
        }

        public ActionResult GetExamTypes()
        {
            var ID = User.Identity.GetUserId();

            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            var BranchId = db.AspNetEmployees.Where(x => x.UserId == ID).FirstOrDefault().BranchId;
            var ExamTypes = db.ExamTypes.Where(x => x.BranchId == BranchId).Select(x => new { x.Id, x.Name }).ToList();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(ExamTypes);
            return Content(status);
        }

        public ActionResult StudentExamsList()
        {


            return View();
        }

        public ActionResult StudentExamMarksDetails(int ExamDetailId)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var UserRole = db.GetUserRoleById(loggedInUserId).FirstOrDefault().ToString();


            if (UserRole == "Student")
            {
                var ExamDetailOfStudent = db.ExamDetails.Where(x => x.Id == ExamDetailId && x.Status == "Published" && x.AspNetStudent.AspNetUser.Id == loggedInUserId).FirstOrDefault();

                if(ExamDetailOfStudent == null)
                {
                    return RedirectToAction("StudentExamsList");

                }

            }

            ViewBag.ExamDetailId = ExamDetailId;

            var ExamDetails = db.ExamDetails.Where(x => x.Id == ExamDetailId).FirstOrDefault();
            ViewBag.Status = ExamDetails.Status;

            return View();
        }
        public ActionResult ExamDetails(int ExamDetailId)
        {
            var ExamDetails = db.ExamDetails.Where(x => x.Id == ExamDetailId).Select(x => new { x.Id, StudentName = x.AspNetStudent.Name, ExamTypeName = x.ExamType.Name, TotalMarks = x.ExamMarksDetails.Sum(y => y.Total), ObtainedMarks = x.ExamMarksDetails.Sum(y => y.Obtained), x.ParentsRemarks }).FirstOrDefault();
            var ExamMarksDetails = db.ExamMarksDetails.Where(x => x.ExamDetail_Id == ExamDetails.Id).Select(x => new { x.Id, x.AspNetCours.Name, x.Total, x.Obtained, x.IsAttended, x.ExamDate, x.Grade }).ToList();

            var ExamTypeName = ExamDetails.ExamTypeName;
            var StudentName = ExamDetails.StudentName;
            var TotalMarks = ExamDetails.TotalMarks;
            var ObtainedMarks = ExamDetails.ObtainedMarks;

            return Json(new { ExamTypeName = ExamTypeName, ParentsRemarks = ExamDetails.ParentsRemarks, StudentName = StudentName, TotalMarks = TotalMarks, ObtainedMarks = ObtainedMarks, ExamMarksDetails = ExamMarksDetails }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateParentsRemarks(int ExamDetailId, string ParentsRemarks)
        {
            var ExamDetailsToUpdate = db.ExamDetails.Where(x => x.Id == ExamDetailId).FirstOrDefault();
            ExamDetailsToUpdate.ParentsRemarks = ParentsRemarks;

            db.SaveChanges();


            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintReport(int ExamDetailId)
        {
            var ExamDetail = db.ExamDetails.Where(x => x.Id == ExamDetailId).FirstOrDefault();

            ExamPrintForm examPrintForm = new ExamPrintForm();

            var GradeExist = "No";

            if (ExamDetail != null)
            {
                var ExamMarksDetails = db.ExamMarksDetails.Where(x => x.ExamDetail_Id == ExamDetail.Id).ToList();


                var StudentName = ExamDetail.AspNetStudent.Name;
                var RollNo = ExamDetail.AspNetStudent.AspNetUser.UserName;
                var Class = ExamDetail.AspNetClass.Name + "-" + ExamDetail.AspNetSection.Name;
                var Section = ExamDetail.AspNetSection.Name;
                var ExamName = ExamDetail.ExamType.Name;

                examPrintForm.StudentName = StudentName;
                examPrintForm.SectionName = Section;
                examPrintForm.ClassName = Class;
                examPrintForm.ExamName = ExamName;
                examPrintForm.RollNo = RollNo;


                List<SubjectMarksPercentage> studentMarksPercentage = new List<SubjectMarksPercentage>();
                int i = 1;
                foreach (var item in ExamMarksDetails)
                {
                    SubjectMarksPercentage markPercentage = new SubjectMarksPercentage();
                    markPercentage.No = i;
                    markPercentage.SubjectName = item.AspNetCours.Name;
                    markPercentage.TeacherName = item.AspNetEmployee.Name;
                    markPercentage.Grade = item.Grade;
                    markPercentage.Comments = item.Comments;


                    if (item.Grade != null)
                    {
                        GradeExist = "Yes";

                    }


                    double Percentage = 0;

                    if (item.Total > 0)
                    {
                        Percentage = Math.Round(item.Obtained.Value * 100 / item.Total.Value, 2);
                    }
                    markPercentage.MarksPercentage = Percentage;

                    studentMarksPercentage.Add(markPercentage);
                    i++;
                }
                examPrintForm.subjectMarksPercentage.AddRange(studentMarksPercentage);

            }
            ViewBag.GradeExist = GradeExist;

            return View(examPrintForm);
        }
        public ActionResult PublishExam(int ExamDetailId)
        {
            var ID = User.Identity.GetUserId();
            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            if (UserRole == "Branch_Admin")
            {
                var ExamDetailToPublish = db.ExamDetails.Where(x => x.Id == ExamDetailId).FirstOrDefault();

                ExamDetailToPublish.Status = "Published";
                db.SaveChanges();
            }

            return RedirectToAction("ExamDetailsAdminView");
        }

        public class ExamPrintForm
        {
            public string StudentName { get; set; }
            public string RollNo { get; set; }
            public string ClassName { get; set; }
            public string SectionName { get; set; }
            public string ExamName { get; set; }

            public List<SubjectMarksPercentage> subjectMarksPercentage = new List<SubjectMarksPercentage>();

        }
        public class SubjectMarksPercentage
        {
            public int No { get; set; }
            public string SubjectName { get; set; }
            public double MarksPercentage { get; set; }
            public string Grade { get; set; }
            public string TeacherName { get; set; }

            public string Comments { get; set; }
        }


        public class ExamDetailVM
        {
            public int Id { get; set; }
            public int BranchId { get; set; }
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int StudentId { get; set; }
            public string Grade { get; set; }
            public string ParentsRemarks { get; set; }
            public int ExamTypeId { get; set; }
            public double Total { get; set; }
            public double Obtained { get; set; }

            public List<ExamMarksDetail> ExamMarksDetail = new List<ExamMarksDetail>();

        }

        public class ExamMarksDetailVM
        {
            public int CourseId { get; set; }
            public double TotalMarks { get; set; }
            public double ObtainedMarks { get; set; }
            public int ExamDetail_Id { get; set; }
            public int TeacherId { get; set; }
            public string ExamDate { get; set; }
            public string Comments { get; set; }
            public bool IsAttended { get; set; }
            public string SubjectGrade { get; set; }
        }
        public class StudentMarks
        {
            public int StudentId { get; set; }
            public double Total { get; set; }
            public double Obtained { get; set; }
        }

        public class StudentExamExcel
        {
            public int StudentId { get; set; }
            public int SubjectId { get; set; }
            public double Total { get; set; }
            public double Obtained { get; set; }
            public DateTime ExamDate { get; set; }
            public bool IsAttended { get; set; }
            public string Grade { get; set; }
            public string Comments { get; set; }
            public int TeacherId { get; set; }

        }
    }
}