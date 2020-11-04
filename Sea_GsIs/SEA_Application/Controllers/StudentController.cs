using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SEA_Application.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: Student
        public ActionResult Index()
        {
            var ID = User.Identity.GetUserId();
            var Classes1 = db.AspNetStudent_Enrollments.Where(x => x.AspNetStudent.UserId == ID).Select(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name).Distinct().ToList();
            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
          //  classes.Add("Not Published");
            //  classes.Add("ClassEight");
            //classes.Add("ClassNine");
            //classes.Add("ClassTen");

            ViewBag.AllClasses = classes;

            return View();
        }

        public ActionResult GetSubjectTopicsAndLessons(int Id)
        {

            try
            {
                var UserId = User.Identity.GetUserId();
                
                var SubjectsTopics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == Id).ToList();

                List<Topic> TopicListObj = new List<Topic>();

                int Count = 0;
                foreach (var a in SubjectsTopics)
                {
                    int count1 = 0;
                    decimal DurationCount = 0;

                    Topic TopicObj = new Topic();

                    //  var LessonList = db.AspnetLessons.Where(x => x.TopicId == a.Id).ToList();




                    //var today = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), TimeZoneInfo.Local);
                    TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow , PK_ZONE);

                  //var DatetimeG = GetLocalDateTime.GetLocalDateTimeFunction();

                    //today = today.AddHours(23);
                    var LessonList = (from Lesson in db.AspnetLessons.Where(x => x.Status == true)
                                      where Lesson.TopicId == a.Id && Lesson.StartDate <= today
                                      select Lesson).ToList();

                    TopicObj.TopicId = a.Id;
                    TopicObj.TopicName = a.Name;
                    TopicObj.Orderby = Convert.ToInt32(a.OrderBy);

                    List<Lesson> LessonsList = new List<Lesson>();


                    foreach (var lesson in LessonList)
                    {
                        var LessonExist = "";
                        StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == lesson.Id & x.StudentId == UserId).FirstOrDefault();

                        if (LessonTracking == null)
                        {
                            LessonExist = "No";
                        }
                        else
                        {
                            LessonExist = "Yes";
                        }
                        Lesson lessonobj = new Lesson();
                        lessonobj.LessonId = lesson.Id;
                        lessonobj.LessonName = lesson.Name;
                        lessonobj.OrderBy = Convert.ToInt32(lesson.OrderBy);
                        lessonobj.LessonDuration = Int32.Parse(lesson.DurationMinutes.ToString());
                        lessonobj.LessonExistInTrackingTable = LessonExist;
                        lessonobj.EncryptedID = lesson.EncryptedID;
                        lessonobj.startDate = lesson.StartDate.ToString().Split(' ')[0];

                        DurationCount = DurationCount + lesson.DurationMinutes ?? 0;

                        LessonsList.Add(lessonobj);
                        Count++;
                        count1++;
                    }

                    List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.OrderBy).ToList();

                    TopicObj.LessonList = OrderByLessons;

                    TopicObj.TotalLessons = Count;
                    TopicObj.TotalLessons1 = count1;
                    TopicObj.TopicDuration = Int32.Parse(DurationCount.ToString());

                    TopicListObj.Add(TopicObj);
                }

                TopicListObj = TopicListObj.Where(x => x.LessonList.Count != 0).ToList();

                return Json(TopicListObj.OrderBy(x => x.Orderby).ToList(), JsonRequestBehavior.AllowGet);
                //return Json(Id, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                var a = ex.Message;
                return Json(a, JsonRequestBehavior.AllowGet);
            }


            // return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentReport()
        {
            return View();
        }
        public JsonResult GetAttendanceDetail()
        {
            var StudentID = User.Identity.GetUserId();
            var users = db.UserAutoPresents.Where(x => x.UserId == StudentID).ToList();
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
        public JsonResult StudentReportStatus(string status)
        {
            var StudentID = User.Identity.GetUserId();
            if (status == "Absent")
            {
                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == StudentID).ToList();
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
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == StudentID).ToList();
                List<Attendance> report = new List<Attendance>();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.Name;
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
            List<Attendance> report = new List<Attendance>();
            var StudentID = User.Identity.GetUserId();

            if (radioValue == "Day")
            {
                var date = DateTime.Now;
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == StudentID && x.Date == date.Date).ToList();
                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == StudentID && x.Date == DateTime.Now).ToList();

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
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId== StudentID && x.Date > date).ToList();
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

                var absentdetail = db.UserAutoAbsents.Where(x => x.UserId == StudentID && x.Date > date).ToList();
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
                var presentdetails = db.UserAutoPresents.Where(x => x.UserId == StudentID && x.Date.Value.Month == month).ToList();
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
                var absent = db.UserAutoAbsents.Where(x => x.UserId == StudentID && x.Date.Value.Month == month).ToList();
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

        public JsonResult RangeDateFilter( DateTime start, DateTime end)
        {
            var StudentID = User.Identity.GetUserId();
            List<Attendance> report = new List<Attendance>();

            var present = db.UserAutoPresents.Where(x => x.UserId == StudentID && x.Date >= start && x.Date <= end).ToList();
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
            var absent = db.UserAutoAbsents.Where(x => x.UserId == StudentID && x.Date >= start && x.Date <= end).ToList();
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

        public class Lesson
        {
            public int LessonId { get; set; }
            public string LessonName { get; set; }
            public int LessonDuration { get; set; }

            public string LessonExistInTrackingTable { get; set; }
            public string EncryptedID { get; set; }
            public string startDate { get; set; }
            public int OrderBy { get; set; }
            public int LessonCount { get; set; }
        }

        public class Topic
        {
            public int TopicId { get; set; }
            public string TopicName { get; set; }
            public int TopicDuration { get; set; }
            public int Orderby { get; set; }
            public int TotalLessons { get; set; }
            public int TotalLessons1 { get; set; }
            public List<Lesson> LessonList { get; set; }

        }

    }
}