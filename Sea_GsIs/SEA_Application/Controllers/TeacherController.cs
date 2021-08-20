using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.IO;

namespace SEA_Application.Controllers
{
    //[Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: Teacher



        [Authorize(Roles = "Teacher")]
        public ActionResult Index()
        {
            //int teacherId = GetLoggedInTeacherId();
            //return View(db.AspNetTeacher_Enrollments
            //    .Where(enrollment => enrollment.TeacherId == teacherId)
            //    .ToList());

            var ID = User.Identity.GetUserId();
            var Classes1 = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID).Select(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name).Distinct().ToList();

            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
            classes.Add("Not Published");

            ViewBag.AllClasses = classes;

            return View();
        }

        //public ActionResult LessonPlanList()
        //{
        //    List<LessonPlan> lp = db.LessonPlans.ToList();

        //    return View(lp);
        //}
        [Authorize(Roles = "Teacher")]
        public ActionResult LessonPlan()
        {

            return View();
        }
        [Authorize(Roles = "Teacher,Branch_Admin,Branch_Principal")]
        public ActionResult EditLessonPlan(int ID)
        {
            LessonPlan LP = db.LessonPlans.Where(x => x.Id == ID).FirstOrDefault();

            int ClassId = LP.ClassID.Value;

            //Wrong SectionId  LP.SectionID;
            int ActualSectionId = db.AspnetSubjectTopics.Where(x => x.Id == LP.TopicID).Select(x => x.AspnetGenericBranchClassSubject.SectionId.Value).FirstOrDefault();
            int BranchId = db.AspnetSubjectTopics.Where(x => x.Id == LP.TopicID).Select(x => x.AspnetGenericBranchClassSubject.BranchId.Value).FirstOrDefault();

            ViewBag.UserRole = "";

            ViewBag.ID = ID;
            // ViewBag.SectionID = LP.SectionID;
            ViewBag.SectionID = ActualSectionId;
            ViewBag.ClassID = LP.ClassID;
            ViewBag.TopicID = LP.TopicID;
            ViewBag.SubjectID = LP.SubjectID;
            ViewBag.BranchID = BranchId;


            if (User.IsInRole("Branch_Admin"))
            {
                ViewBag.UserRole = "Admin";
                ViewBag.BranchName = db.AspNetBranches.Where(x => x.Id == BranchId).FirstOrDefault().Name;
                ViewBag.ClassName = db.AspNetClasses.Where(x => x.Id == ClassId).FirstOrDefault().Name;
                ViewBag.SubjectName = db.AspNetCourses.Where(x => x.Id == LP.SubjectID).FirstOrDefault().Name;
                //ViewBag.SectionName = db.AspNetSections.Where(x => x.Id == LP.SectionID).FirstOrDefault().Name;
                ViewBag.SectionName = db.AspNetSections.Where(x => x.Id == ActualSectionId).FirstOrDefault().Name;
                ViewBag.TopicName = db.AspnetSubjectTopics.Where(x => x.Id == LP.TopicID).FirstOrDefault().Name;

            }

            return View(LP);
        }

        [HttpPost]
        public ActionResult EditLessonPlans(LessonPlan LP, IEnumerable<HttpPostedFileBase> files)
        {

            try
            {

                var ID = User.Identity.GetUserId();

                //var BranchID = (from branch in db.AspNetBranches
                //                join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                //                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                //                where enrollment.AspNetEmployee.UserId == ID
                //                select new
                //                {
                //                    branch.Id,

                //                }).Distinct().FirstOrDefault().Id;

                var BranchID1 = Request.Form["BranchID"];

                var BranchID = Convert.ToInt32(Request.Form["BranchID"]);

                int ClassId = LP.ClassID.Value;
                int SectionId = LP.SectionID.Value;


                int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.BranchId == BranchID && x.AspNetBranch_Class.ClassId == ClassId).Select(x => x.Id).FirstOrDefault();


                LP.SectionID = BranchClassSectionId;


                var fileName = "";
                var AllFiles = "";

                //  bool? status = db.LessonPlans.Where(x => x.Id == LP.Id).FirstOrDefault().Status;
                //  LP.Status = status;
                LessonPlan LessonPlanToUpdate = db.LessonPlans.Where(x => x.Id == LP.Id).FirstOrDefault();
                LP.Status = LessonPlanToUpdate.Status;
                LP.LessonPlanAttachments = LessonPlanToUpdate.LessonPlanAttachments;

                if (files != null)
                {
                    foreach (var file1 in files)
                    {
                        if (file1 != null && file1.ContentLength > 0)
                        {
                            var name = Path.GetFileNameWithoutExtension(file1.FileName);
                            var ext = Path.GetExtension(file1.FileName);
                            // fileName = name + "_LS_" + LessonID + Student.Id + ext;	
                            fileName = name + "_LP_" + LP.Id + ext;
                            AllFiles += fileName + "/";
                            file1.SaveAs(Path.Combine(Server.MapPath("~/Content/LessonPlanAttachments/"), fileName));
                        }
                    }
                }
                if (AllFiles != "")
                {
                    // LessonPlan LessonPlanToUpdate = db.LessonPlans.Where(x => x.Id == LP.Id).FirstOrDefault();
                    LessonPlanToUpdate.LessonPlanAttachments = AllFiles;
                    LP.LessonPlanAttachments = AllFiles;
                    // db.SaveChanges();
                }


                Sea_Entities db1 = new Sea_Entities();
                db1.Entry(LP).State = EntityState.Modified;
                db1.SaveChanges();

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LessonPlan(LessonPlan LP, IEnumerable<HttpPostedFileBase> files)
        {
            var ID = User.Identity.GetUserId();

            var BranchID = Convert.ToInt32(Request.Form["BranchID"]);

            //var BranchID = (from branch in db.AspNetBranches
            //                join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
            //                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
            //                where enrollment.AspNetEmployee.UserId == ID
            //                select new
            //                {
            //                    branch.Id,

            //                }).Distinct().FirstOrDefault().Id;

            int ClassId = LP.ClassID.Value;
            int SectionId = LP.SectionID.Value;


            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.BranchId == BranchID && x.AspNetBranch_Class.ClassId == ClassId).Select(x => x.Id).FirstOrDefault();


            LP.SectionID = BranchClassSectionId;


            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                if (LP != null)
                {
                    var fileName = "";
                    var AllFiles = "";

                    LP.CreationDate = DateTime.Now;
                    LP.Status = false;
                    LP.CreatedBy = User.Identity.GetUserId();
                    db.LessonPlans.Add(LP);
                    db.SaveChanges();

                    if (files != null)
                    {
                        foreach (var file1 in files)
                        {
                            if (file1 != null && file1.ContentLength > 0)
                            {
                                var name = Path.GetFileNameWithoutExtension(file1.FileName);
                                var ext = Path.GetExtension(file1.FileName);
                                // fileName = name + "_LS_" + LessonID + Student.Id + ext;	
                                fileName = name + "_LP_" + LP.Id + ext;
                                AllFiles += fileName + "/";
                                file1.SaveAs(Path.Combine(Server.MapPath("~/Content/LessonPlanAttachments/"), fileName));
                            }
                        }
                    }
                    if (AllFiles != "")
                    {
                        LessonPlan LessonPlanToUpdate = db.LessonPlans.Where(x => x.Id == LP.Id).FirstOrDefault();
                        LessonPlanToUpdate.LessonPlanAttachments = AllFiles;
                        db.SaveChanges();
                    }

                }

                dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                dbTransaction.Dispose();
            }


            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        [Authorize(Roles = "Teacher,Branch_Admin,Branch_Principal")]

        public ActionResult LessonPlanList()
        {

            ViewBag.UserRole = "";
            if (User.IsInRole("Branch_Admin"))
            {
                ViewBag.UserRole = "Admin";
            }

            return View();
        }
        [HttpGet]
        public ActionResult TeacherEnrollment()
        {

            return View();
        }
        [HttpPost]
        public ActionResult TeacherEnrollment(int? b)
        {

            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                var ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                var TeacherId = Request.Form["TeacherId"];
                var SectionId = Convert.ToInt32(Request.Form["SectionId"]);
                var Courses = Convert.ToInt32(Request.Form["Courses"]);


                var Teacher = db.AspNetEmployees.Where(x => x.UserId == TeacherId).Select(x => x.Id).FirstOrDefault();
                var SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();

                AspNetTeacher_Enrollments TeacherEnrollment = new AspNetTeacher_Enrollments();

                TeacherEnrollment.SectionId = SectionId;
                TeacherEnrollment.SessionId = SessionId;
                TeacherEnrollment.CourseId = Courses;
                TeacherEnrollment.TeacherId = Teacher;

                db.AspNetTeacher_Enrollments.Add(TeacherEnrollment);
                db.SaveChanges();

                var BranchId = db.AspNetBranch_Class.Where(x => x.Id == ClassId).Select(x => x.BranchId).FirstOrDefault();
                var ClassIdOfClass = db.AspNetBranch_Class.Where(x => x.Id == ClassId).Select(x => x.ClassId).FirstOrDefault();
                var SectionIdOfSection = db.AspNetBranchClass_Sections.Where(x => x.Id == SectionId).Select(x => x.SectionId).FirstOrDefault();
                var SubjectIdofSubject = db.AspNetClass_Courses.Where(x => x.Id == Courses).Select(x => x.CourseId).FirstOrDefault();

                var generictest = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassIdOfClass &&
                                                x.SectionId == SectionIdOfSection && x.SubjectId == SubjectIdofSubject).FirstOrDefault();

                if (generictest == null)
                {
                    var generic = new AspnetGenericBranchClassSubject();

                    generic.BranchId = BranchId;
                    generic.ClassId = ClassIdOfClass;
                    generic.SectionId = SectionIdOfSection;
                    generic.SubjectId = SubjectIdofSubject;

                    db.AspnetGenericBranchClassSubjects.Add(generic);
                    db.SaveChanges();
                }

                dbTransaction.Commit();
                return RedirectToAction("TeacherEnrollment", "Teacher");

              //  return RedirectToAction("TeacherClassSubject", "BranchAdmin");
            }
            catch (Exception e)
            {  
                dbTransaction.Dispose();
                return RedirectToAction("TeacherEnrollment", "Teacher");

               // return RedirectToAction("TeacherEnrollment", "BranchAdmin", model);
            }

           // return RedirectToAction("TeacherEnrollment", "Teacher");

        }
        //CheckTeacherClassAndCourses


        public ActionResult CheckTeacherClassAndCourses(string TeacherId, int ClassId, int SectionId, int CourseId)
        {
            var Teacher = db.AspNetEmployees.Where(x => x.UserId == TeacherId).Select(x => x.Id).FirstOrDefault();

            string IsTeacherEntroll = "No";

            var TeacherEnrollment = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == Teacher && x.SectionId == SectionId && x.CourseId == CourseId).FirstOrDefault();

            //AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == StudentId).FirstOrDefault();

            if (TeacherEnrollment != null)
            {
                IsTeacherEntroll = "Yes";
            }


            return Json(new { IsTeacherEntroll = IsTeacherEntroll }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLessonPlanList()
        {

            var ID = User.Identity.GetUserId();

            int branchId;
            if (User.IsInRole("Branch_Admin"))
            {

                branchId = db.AspNetBranch_Admins
              .Where(branchAdmin => branchAdmin.AdminId.Equals(ID, StringComparison.OrdinalIgnoreCase))
              .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();

                var AllLessonPlan = (from lessonPlan in db.LessonPlans.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         Id = lessonPlan.Id,
                                         ClassName = lessonPlan.AspNetClass.Name,
                                         SubjectName = lessonPlan.AspNetCours.Name,
                                         TopicName = lessonPlan.AspnetSubjectTopic.Name,
                                         Week = lessonPlan.Week,
                                         Chapter = lessonPlan.Chapter,
                                         Status = lessonPlan.Status,
                                         Date = lessonPlan.LessonPlanDate.Value,

                                     }).Distinct().ToList();
                return Json(AllLessonPlan, JsonRequestBehavior.AllowGet);
            }
            else
            {


                var AllLessonPlan = (from lessonPlan in db.LessonPlans
                                     join enrollment in db.AspNetTeacher_Enrollments on lessonPlan.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                                     where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lessonPlan.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                                     && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lessonPlan.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                                     && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lessonPlan.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                                     select new
                                     {
                                         Id = lessonPlan.Id,
                                         ClassName = lessonPlan.AspNetClass.Name,
                                         SubjectName = lessonPlan.AspNetCours.Name,
                                         TopicName = lessonPlan.AspnetSubjectTopic.Name,
                                         Week = lessonPlan.Week,
                                         Chapter = lessonPlan.Chapter,
                                         Status = lessonPlan.Status,
                                         Date = lessonPlan.LessonPlanDate.Value,

                                     }).Distinct().ToList();
                return Json(AllLessonPlan, JsonRequestBehavior.AllowGet);

            }


        }




        public ActionResult PublishLesson(int id)
        {
            LessonPlan LessonPlanToUpdate = db.LessonPlans.Where(x => x.Id == id).FirstOrDefault();
            LessonPlanToUpdate.Status = true;

            db.SaveChanges();

            return RedirectToAction("LessonPlanList");

        }
        public ActionResult DownloadLessonPlanAttachment(string Name)
        {

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/LessonPlanAttachments/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }


        public ActionResult TeacherStudents()
        {
            return View();
        }
        public JsonResult getClasses()
        {
            var ID = User.Identity.GetUserId();
            var Classes = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID).Select(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name).Distinct().ToList();
            return new JsonResult { Data = Classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            e.UserId = User.Identity.GetUserId();
            var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.Events.Add(e);
                }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        #region Utils

        private int GetLoggedInTeacherId()
        {
            var loggedInUserId = User.Identity.GetUserId();
            return db.AspNetTeachers
                .Where(teacher => teacher.UserId.Equals(loggedInUserId))
                .First()
                .Id;
        }

        #endregion
        public ActionResult CalendarNotification()
        {
            var id = User.Identity.GetUserId();
            var checkdate = DateTime.Now;
            var date = TimeZoneInfo.ConvertTime(DateTime.UtcNow.ToUniversalTime(), TimeZoneInfo.Local);
            var fullname = "";

            fullname = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var namelist = fullname.Split(' ');
            var name = namelist[0];
            var day = date.DayOfWeek;
            var dd = date.Day;
            var mm = date.Month;
            var yy = date.Year;
            string[] array = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            var Date = day + ", " + dd + " " + array[mm - 1] + " " + yy;
            var result = new { checkdate, Date, name };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Teacher")]
        public ActionResult TeacherSubject()
        {
            return View();
        }
        public JsonResult GetTeacherSubjects()
        {
            List<Class_Subject> ClassSubject = new List<Class_Subject>();
            var tid = User.Identity.GetUserId();


            var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var enrolmentlist = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).ToList();


            foreach (var item in enrolmentlist)
            {
                //var classcourse = db.AspNetClass_Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                Class_Subject cs = new Class_Subject();
                cs.Class = item.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name;
                cs.Section = item.AspNetBranchClass_Sections.AspNetSection.Name;
                cs.Subject = item.AspNetClass_Courses.AspNetCours.Name;
                cs.Id = item.Id;
                //cs.Class = classcourse.AspNetClass.Name;
                //cs.Subject = classcourse.AspNetCours.Name;
                //int BC_ID = db.AspNetBranch_Class.Where(x => x.ClassId == classcourse.ClassId).FirstOrDefault().Id;
                //int SectionID =  db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BC_ID).FirstOrDefault().SectionId;
                //cs.Section = db.AspNetSections.Where(x => x.Id == SectionID).FirstOrDefault().Name;



                ClassSubject.Add(cs);
            }
            return Json(ClassSubject, JsonRequestBehavior.AllowGet);
        }

        public class Class_Subject
        {
            public int Id { get; set; }
            public string Class { get; set; }
            public string Subject { get; set; }
            public string Section { get; set; }

        }
        public ActionResult ViewAttendance()
        {
            return View();
        }

        public ActionResult ViewStudentAttendence()
        {
            var id = User.Identity.GetUserId();
            ViewBag.teacherid = db.AspNetEmployees.Where(x => x.UserId == id).FirstOrDefault().Id;
            return View();
        }

        public ActionResult StudentDetails(string RollNo)
        {
            List<GetStudentDetails_Result> list = new List<GetStudentDetails_Result>();
            ViewBag.StudentDetails = db.GetStudentDetails(RollNo).ToList();

            return View();
        }

        public ActionResult LoadStudentList(int id)
        {

            List<ShowStudentAttendence_Result> list = new List<ShowStudentAttendence_Result>();
            list = db.ShowStudentAttendence(id).ToList();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            return Content(status);

        }
        public JsonResult GetAttendanceDetail()
        {
            string TeacherID = User.Identity.GetUserId();
            var users = db.UserAutoPresents.Where(x => x.UserId == TeacherID).ToList();

            List<Attendance> attendance = new List<Attendance>();
            foreach (var item in users)
            {
                var length = item.TimeOut - item.TimeIn;

                Attendance at = new Attendance();
                at.Name = item.AspNetUser.Name;
                at.UserName = item.AspNetUser.UserName;
                at.Status = "Present";
                at.Date = item.Date;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.TimeIn = item.TimeIn;
                at.TimeOut = item.TimeOut;
                at.ShiftLength = length;
                at.IP_Address = item.IP_Address;
                attendance.Add(at);
            }
            return Json(attendance, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Teacher_Att_ReportStatus(string status)
        {
            string TeacherID = User.Identity.GetUserId();

            if (status == "Absent")
            {
                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == TeacherID).ToList();
                List<Attendance> report = new List<Attendance>();
                foreach (var item in absentdetail)
                {
                    Attendance at = new Attendance();
                    at.Date = item.Date;
                    at.Name = item.AspNetUser.Name;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.UserName = item.AspNetUser.UserName;
                    at.TimeIn = null;
                    at.TimeOut = null;
                    at.Status = "Absent";
                    at.ShiftLength = null;
                    at.IP_Address = null;
                    report.Add(at);
                }
                return Json(report, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == TeacherID).ToList();
                List<Attendance> report = new List<Attendance>();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.Status = "Present";
                    at.ShiftLength = length;
                    at.IP_Address = item.IP_Address;
                    report.Add(at);
                }
                return Json(report, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult RadioResult(string radioValue)
        {
            string TeacherID = User.Identity.GetUserId();

            List<Attendance> report = new List<Attendance>();

            if (radioValue == "Day")
            {
                var date = DateTime.Now;
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == TeacherID && x.Date == date.Date).ToList();
                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == TeacherID && x.Date == DateTime.Now).ToList();

                if (presentdetails.Count != 0)
                {
                    foreach (var item in presentdetails)
                    {
                        var length = item.TimeOut - item.TimeIn;
                        Attendance at = new Attendance();
                        at.Name = item.AspNetUser.Name;
                        at.UserName = item.AspNetUser.UserName;
                        at.Date = item.Date;
                        at.Day = item.Date.Value.DayOfWeek.ToString();
                        at.TimeIn = item.TimeIn;
                        at.TimeOut = item.TimeOut;
                        at.Status = "Present";
                        at.ShiftLength = length;
                        at.IP_Address = item.IP_Address;
                        report.Add(at);
                    }
                    return Json(report, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in absentdetail)
                    {
                        Attendance at = new Attendance();
                        at.Date = item.Date;
                        at.Name = item.AspNetUser.Name;
                        at.Day = item.Date.Value.DayOfWeek.ToString();
                        at.UserName = item.AspNetUser.UserName;
                        at.TimeIn = null;
                        at.TimeOut = null;
                        at.Status = "Absent";
                        at.ShiftLength = null;
                        at.IP_Address = null;
                        report.Add(at);
                    }
                    return Json(report, JsonRequestBehavior.AllowGet);
                }
            }
            else if (radioValue == "Week")
            {
                var date = DateTime.Now;
                date = date.AddDays(-7);
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == TeacherID && x.Date > date).ToList();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.Status = "Present";
                    at.ShiftLength = length;
                    at.IP_Address = item.IP_Address;
                    report.Add(at);
                }

                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == TeacherID && x.Date > date).ToList();
                foreach (var item in absentdetail)
                {
                    Attendance at = new Attendance();
                    at.Date = item.Date;
                    at.Name = item.AspNetUser.Name;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.UserName = item.AspNetUser.UserName;
                    at.TimeIn = null;
                    at.TimeOut = null;
                    at.Status = "Absent";
                    at.ShiftLength = null;
                    at.IP_Address = null;
                    report.Add(at);
                }
                return Json(report, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var month = DateTime.Now.Month;
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == TeacherID && x.Date.Value.Month == month).ToList();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.Status = "Present";
                    at.ShiftLength = length;
                    at.IP_Address = item.IP_Address;
                    report.Add(at);

                }
                var absent = db.UserAutoAbsents.Where(x => x.UserId == TeacherID && x.Date.Value.Month == month).ToList();
                foreach (var item in absent)
                {
                    Attendance at = new Attendance();
                    at.Date = item.Date;
                    at.Name = item.AspNetUser.Name;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.UserName = item.AspNetUser.UserName;
                    at.TimeIn = null;
                    at.TimeOut = null;
                    at.Status = "Absent";
                    at.ShiftLength = null;
                    at.IP_Address = null;
                    report.Add(at);
                }
                return Json(report, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RangeDateFilter(DateTime start, DateTime end)
        {
            string TeacherID = User.Identity.GetUserId();
            List<Attendance> report = new List<Attendance>();

            var present = db.UserAutoPresents.Where(x => x.UserId == TeacherID && x.Date >= start && x.Date <= end).ToList();
            foreach (var item in present)
            {
                var length = item.TimeOut - item.TimeIn;
                Attendance at = new Attendance();
                at.Name = item.AspNetUser.Name;
                at.UserName = item.AspNetUser.UserName;
                at.Date = item.Date;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.TimeIn = item.TimeIn;
                at.TimeOut = item.TimeOut;
                at.Status = "Present";
                at.ShiftLength = length;
                at.IP_Address = item.IP_Address;
                report.Add(at);
            }
            var absent = db.UserAutoAbsents.Where(x => x.UserId == TeacherID && x.Date >= start && x.Date <= end).ToList();
            foreach (var item in absent)
            {
                Attendance at = new Attendance();
                at.Date = item.Date;
                at.Name = item.AspNetUser.Name;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.UserName = item.AspNetUser.UserName;
                at.TimeIn = null;
                at.TimeOut = null;
                at.Status = "Absent";
                at.ShiftLength = null;
                at.IP_Address = null;
                report.Add(at);
            }

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public class Attendance
        {
            public string Name { get; set; }
            public string UserName { get; set; }
            public string Day { get; set; }
            public string Status { get; set; }
            public TimeSpan? ShiftLength { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? TimeIn { get; set; }
            public TimeSpan? TimeOut { get; set; }
            public string IP_Address { get; set; }

        }
    }

}