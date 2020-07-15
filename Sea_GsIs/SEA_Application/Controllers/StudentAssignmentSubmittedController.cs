using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Text;
using System.Net;
using SEA_Application.CustomModel;

namespace SEA_Application.Controllers
{
    public class StudentAssignmentSubmittedController : Controller
    {

        private Sea_Entities db = new Sea_Entities();
        public ActionResult Index()
        {

            return View();


        }

        [HttpGet]
        public ActionResult TeacherComments(int id)
        {
            ViewBag.Id = id;

            AspnetStudentAssignmentSubmission StudentAssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id).FirstOrDefault();


            if (StudentAssignmentSubmission != null)
            {

                ViewBag.TotalMarks = StudentAssignmentSubmission.AspnetLesson.AspnetStudentAssignments.FirstOrDefault().TotalMarks;
                ViewBag.ObtainedMarks = StudentAssignmentSubmission.ObtainedMarks;
                ViewBag.TeacherComments = StudentAssignmentSubmission.TeacherComments;
                ViewBag.TeacherFile = StudentAssignmentSubmission.TeacherAssignment + "" + StudentAssignmentSubmission.TeacherResubmittedFile;
                //  ViewBag.IsReSubmissionRequired = StudentAssignmentSubmission.ResubmitRequired;	

                if (StudentAssignmentSubmission.TeacherResubmittedFile != null)
                {
                    ViewBag.TeacherResubmittedFileExist = "Yes";
                }
                else
                {
                    ViewBag.TeacherResubmittedFileExist = "No";
                }
                if (StudentAssignmentSubmission.ResubmitRequired == true)
                {
                    ViewBag.IsReSubmissionRequired = 1;
                }
                else if (StudentAssignmentSubmission.ResubmitRequired == false)
                {
                    ViewBag.IsReSubmissionRequired = 0;
                }
                else
                {
                    ViewBag.IsReSubmissionRequired = null;
                }

            }
            else
            {
                ViewBag.ObtainedMarks = null;
                ViewBag.TotalMarks = null;
                ViewBag.TeacherComments = null;
                ViewBag.TeacherFile = null;
                ViewBag.IsReSubmissionRequired = null;
            }



            return View();

        }
        [HttpPost]
        public ActionResult TeacherCommentsMethod(int id, string TeacherComments)

        {
            AspnetStudentAssignmentSubmission assignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id).FirstOrDefault();
            assignmentSubmission.TeacherComments = TeacherComments;
            db.SaveChanges();


            //int? LessonId = assignmentSubmission.LessonId;
            //int? StudentId = assignmentSubmission.StudentId;

            //var StudentIdInString = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().UserId;

            //StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId && x.StudentId == StudentIdInString).FirstOrDefault();

            //if (LessonTracking != null)
            //{
            //    LessonTracking.Assignment_Status = "Approved";
            //    db.SaveChanges();

            //}

            return Json("", JsonRequestBehavior.AllowGet);

        }

        public JsonResult TeacherAssignmentSubmission(int id, string TeacherComments, float ObtainedMarks, IEnumerable<HttpPostedFileBase> file, bool IsResubmissionRequired)
        {
            var CheckResubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id && x.ResubmitRequired == true).FirstOrDefault();
            if (CheckResubmission == null)
            {
                var fileName = "";
                var AllFiles = "";
                if (file != null)
                {
                    foreach (var file1 in file)
                    {
                        if (file1 != null) // && file1.ContentLength > 0)	
                        {
                            var name = Path.GetFileNameWithoutExtension(file1.FileName);
                            var ext = Path.GetExtension(file1.FileName);
                            fileName = name + "_LR_" + id + ext;
                            AllFiles += fileName + "/";
                            file1.SaveAs(Server.MapPath("~/Content/TeacherSubmittedAssignment/") + fileName);
                        }
                    }
                }
                AspnetStudentAssignmentSubmission assignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id).FirstOrDefault();
                assignmentSubmission.TeacherComments = TeacherComments.TrimStart().TrimEnd();
                assignmentSubmission.ObtainedMarks = ObtainedMarks;
                assignmentSubmission.ResubmitRequired = IsResubmissionRequired;
                if (fileName != "")
                {
                    assignmentSubmission.TeacherAssignment = AllFiles;
                }
                db.SaveChanges();
                var UserId = User.Identity.GetUserId();
                var UserName = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Name;
                // SendMail(assignmentSubmission.AspNetStudent.AspNetUser.SecondaryEmail, "GSIS VLEP Assignment Reviewed", EmailDesign.TeacherAssignmentCheckedTemplate(UserName, assignmentSubmission.AspNetStudent.Name, assignmentSubmission.AspnetLesson.Name, assignmentSubmission.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name));	
            }
            else
            {
                //resubmissionBlock	
                var fileName = "";
                var AllFiles = "";
                if (file != null)
                {
                    foreach (var file1 in file)
                    {
                        if (file1 != null) // && file1.ContentLength > 0)	
                        {
                            var name = Path.GetFileNameWithoutExtension(file1.FileName);
                            var ext = Path.GetExtension(file1.FileName);
                            fileName = name + "_LR_" + id + ext;
                            AllFiles += fileName + "/";
                            file1.SaveAs(Server.MapPath("~/Content/TeacherSubmittedAssignment/") + fileName);
                        }
                    }
                }
                AspnetStudentAssignmentSubmission assignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id).FirstOrDefault();
                assignmentSubmission.TeacherComments = TeacherComments.TrimStart().TrimEnd();
                assignmentSubmission.ObtainedMarks = ObtainedMarks;

                //TimeZone time2 = TimeZone.CurrentTimeZone;
                //DateTime test = time2.ToUniversalTime(DateTime.Now);
                //var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);
                //assignmentSubmission.ResubmissionDate = pakistantime;

                if (fileName != "")
                {
                    assignmentSubmission.TeacherResubmittedFile = AllFiles;
                }
                db.SaveChanges();
                //var UserId = User.Identity.GetUserId();
                //var UserName = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Name;
                // SendMail(assignmentSubmission.AspNetStudent.AspNetUser.SecondaryEmail, "GSIS VLEP Assignment Reviewed", EmailDesign.TeacherAssignmentCheckedTemplate(UserName, assignmentSubmission.AspNetStudent.Name, assignmentSubmission.AspnetLesson.Name, assignmentSubmission.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name));	
            }
            //TempData["AssignmentCreated"] = "Created";
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentLessonTracking()
        {

            return View(db.StudentLessonTrackings.ToList());
        }
        public ActionResult GetStudentAssignements(int Id)
        {
            string status = "error";
            try
            {

                int StudentID = Int32.Parse(db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == Id).FirstOrDefault().StudentId.ToString());
                if (StudentID != null && StudentID != 0)
                {
                    var list = (from listdata in db.AspnetStudentAssignmentSubmissions.Where(x => x.StudentId == StudentID && x.Id == Id) select new { Id = listdata.Id, StudentID = listdata.StudentId, AssigmentName = listdata.AssignmentFileName }).ToList();

                    status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                }

            }
            catch (Exception ex)
            {
                // logAppException(ex.ToString(), "StateLookUp");
            }
            return Content(status);
        }

        public ActionResult GetStudentReSubmittedAssignements(int Id)
        {
            string status = "error";
            try
            {
                int StudentID = Int32.Parse(db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == Id).FirstOrDefault().StudentId.ToString());
                if (StudentID != null && StudentID != 0)
                {
                    var list = (from listdata in db.AspnetStudentAssignmentSubmissions.Where(x => x.StudentId == StudentID && x.Id == Id) select new { Id = listdata.Id, StudentID = listdata.StudentId, AssigmentName = listdata.StudentResubmittedFile }).ToList();
                    status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                }
            }
            catch (Exception ex)
            {
                // logAppException(ex.ToString(), "StateLookUp");	
            }
            return Content(status);
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

                    MailMessage mailMessage = new MailMessage(senderEmail, item, subjeEnumerableDebugViewct , emailBody);
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
        public JsonResult GetStudentAssignments()
        {
            var ID = User.Identity.GetUserId();

            var list = (from lesson in db.AspnetLessons
                        join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                        join SAS in db.AspnetStudentAssignmentSubmissions on lesson.Id equals SAS.LessonId
                        where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                        && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                        && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                        select new
                        {
                            NameOfStudent = SAS.AspNetStudent.Name,
                            AssignmnetDueDate = SAS.AssignmentDueDate.ToString(),
                            AssignmentSubmittedDate = SAS.AssignmentSubmittedDate.ToString(),
                            SAS.AssignmentFileName,
                            SAS.LessonId,
                            SAS.TopicId,
                            SAS.StudentId,
                            TeacherComments = SAS.TeacherComments,
                            AssignmentId = SAS.Id,
                            BranchName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Name,
                            ClassName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                            SectionName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                            Lesson = SAS.AspnetLesson.Name
                        }).Distinct().ToList();

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public ActionResult StudentAssignments()
        {
            //var UserId = User.Identity.GetUserId();

            ////var enrollment = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == UserId);

            //var ID = User.Identity.GetUserId();

            //var list = (from lesson in db.AspnetLessons
            //            join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
            //            join SAS in db.AspnetStudentAssignmentSubmissions on lesson.Id equals SAS.LessonId
            //            where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
            //            && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
            //            select new
            //            {
            //                NameOfStudent = SAS.AspNetStudent.Name,
            //                AssignmnetDueDate = SAS.AssignmentDueDate,
            //                AssignmentSubmittedDate = SAS.AssignmentSubmittedDate,
            //                SAS.AssignmentFileName,
            //                SAS.LessonId,
            //                SAS.TopicId,
            //                SAS.StudentId,
            //                TeacherComments = SAS.TeacherComments,
            //                AssignmentId = SAS.Id,
            //                BranchName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Name,
            //                ClassName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
            //                SectionName = SAS.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
            //                Lesson = SAS.AspnetLesson.Name
            //            }).Distinct().ToList();


            //List<AssignmentViewModel> listAssignmentViewModel = new List<AssignmentViewModel>();

            //foreach (var submittedAssignment in list)
            //{

            //    AssignmentViewModel assignmentViewModel = new AssignmentViewModel();

            //    //    int? ClassId = submittedAssignment.ClassId;
            //    //   string CourseType = submittedAssignment.CourseType;
            //    DateTime? DueDate = submittedAssignment.AssignmnetDueDate;
            //    DateTime? SubmittedDate = submittedAssignment.AssignmentSubmittedDate;
            //    string FileName = submittedAssignment.AssignmentFileName;
            //    int? LessonId = submittedAssignment.LessonId;
            //    //  int? SubjectId = submittedAssignment.SubjectId;
            //    int? TopicId = submittedAssignment.TopicId;
            //    int? StudentId = submittedAssignment.StudentId;
            //    int AssignemntId = submittedAssignment.AssignmentId;



            //    // var ClassName = db.AspNetClasses.Where(x => x.Id == ClassId).FirstOrDefault().ClassName;
            //    // var SubjectName = db.GenericSubjects.Where(x => x.Id == SubjectId).FirstOrDefault().SubjectName;
            //    var TopicName = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().Name;
            //    var LessonName = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().Name;
            //    var StudentID = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().UserId;
            //    var StudentName = db.AspNetUsers.Where(x => x.Id == StudentID).FirstOrDefault().Name;

            //    int BranchClassSectionId = db.AspNetStudent_Enrollments.Where(x => x.StudentId == StudentId).FirstOrDefault().SectionId;


            //    var BranchClassSectionObj = db.AspNetBranchClass_Sections.Where(x => x.Id == BranchClassSectionId && x.IsActive == true);

            //    int BranchClassId = BranchClassSectionObj.FirstOrDefault().BranchClassId;

            //    assignmentViewModel.SectionName = BranchClassSectionObj.FirstOrDefault().AspNetSection.Name;


            //    var BranchClassObj = db.AspNetBranch_Class.Where(x => x.Id == BranchClassId && x.IsActive == true);


            //    assignmentViewModel.BranchName = BranchClassObj.FirstOrDefault().AspNetBranch.Name;
            //    assignmentViewModel.ClassName = BranchClassObj.FirstOrDefault().AspNetClass.Name;

            //    //   var AssignmentId =  db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonId).FirstOrDefault().Id;

            //    assignmentViewModel.AssignmentId = AssignemntId;
            //    assignmentViewModel.AssignmentName = FileName;

            //    DateTime ConvertedDueDate = Convert.ToDateTime(DueDate);

            //    assignmentViewModel.AssignmnetDueDate = ConvertedDueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //    DateTime ConvertedSubmittedDate = Convert.ToDateTime(SubmittedDate);

            //    assignmentViewModel.AssignmentSubmittedDate = ConvertedSubmittedDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);


            //    //    assignmentViewModel.Section = ClassName;
            //    //   assignmentViewModel.SubjectName = SubjectName;
            //    assignmentViewModel.Topic = TopicName;
            //    assignmentViewModel.Lesson = LessonName;
            //    assignmentViewModel.NameOfStudent = StudentName;
            //    // assignmentViewModel.CourseType = CourseType;

            //    if (submittedAssignment.TeacherComments != null)
            //    {

            //        var TrimStringStart = submittedAssignment.TeacherComments.TrimStart();
            //        var TrimStringEnd = TrimStringStart.TrimEnd();

            //        if (TrimStringEnd.Count() > 10)
            //        {

            //            string sub = TrimStringEnd.Substring(0, 10);

            //            assignmentViewModel.TeacherComments = sub + "....";
            //        }
            //        else
            //        {


            //            assignmentViewModel.TeacherComments = TrimStringEnd;
            //        }

            //    }
            //    else

            //    {
            //        assignmentViewModel.TeacherComments = submittedAssignment.TeacherComments;

            //    }


            //    listAssignmentViewModel.Add(assignmentViewModel);


            //}

            //return View(listAssignmentViewModel);
            return View();
        }


        public ActionResult DownloadFileOfAssignment(int id, string Name)
        {
            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }

        public ActionResult DownloadResubmittedFileOfAssignment(int id, string Name)
        {
            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);
            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), Name);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }
    }

    //public class AssignmentViewModel
    //{

    //    [Display(Name = "Student Name")]
    //    public string NameOfStudent { get; set; }
    //    public string Section { get; set; }
    //    [Display(Name = "Course Type")]
    //    public string CourseType { get; set; }

    //    public string Topic { get; set; }
    //    public string Lesson { get; set; }
    //    [Display(Name = "Assignment")]

    //    public string AssignmentName { get; set; }

    //    [Display(Name = "Subject Name")]

    //    public string SubjectName { get; set; }

    //    [Display(Name = "Assignment Due Date")]
    //    public DateTime? AssignmnetDueDate { get; set; }

    //    [Display(Name = "Assignment Submitted Date")]

    //    public DateTime? AssignmentSubmittedDate { get; set; }

    //}


}