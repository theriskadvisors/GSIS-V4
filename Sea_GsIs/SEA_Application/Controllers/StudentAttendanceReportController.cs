using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class StudentAttendanceReportController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: StudentAttendanceReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StudentReport(string UserName)
        {
            ViewBag.User = UserName;
            return View();
        }
        public JsonResult GetAttendanceDetail(string UserName)
        {
            var users=db.UserAutoPresents.Where(x => x.AspNetUser.UserName == UserName).ToList();
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
            return Json(attendance,JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentReportStatus(string username, string status)
        {
            if (status == "Absent")
            {
                var absentdetail = db.UserAutoAbsents.Where(x => x.AspNetUser.UserName == username).ToList();
                List<Attendance> report = new List<Attendance>();
                foreach (var item in absentdetail)
                {
                    Attendance at = new Attendance();
                    at.Date = item.Date;
                    at.Name = item.AspNetUser.Name;   
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.UserName = username;
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
                var presentdetails = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == username).ToList();
                List<Attendance> report = new List<Attendance>();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = username;
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

        public JsonResult RadioResult(string radioValue, string username)
        {
            List<Attendance> report = new List<Attendance>();

            if (radioValue == "Day")
            {
                var date = DateTime.Now;
                var presentdetails = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == username && x.Date == date.Date).ToList();
                var absentdetail = db.UserAutoAbsents.Where(x => x.AspNetUser.UserName == username && x.Date == DateTime.Now).ToList();

                if (presentdetails.Count != 0)
                {
                    foreach (var item in presentdetails)
                    {
                        var length = item.TimeOut - item.TimeIn;
                        Attendance at = new Attendance();
                        at.Name = item.AspNetUser.Name;
                        at.UserName = username;
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
                        at.UserName = username;
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
                var presentdetails = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == username && x.Date > date).ToList(); 
                    foreach (var item in presentdetails)
                    {
                        var length = item.TimeOut - item.TimeIn;
                        Attendance at = new Attendance();
                        at.Name = item.AspNetUser.Name;
                        at.UserName = username;
                        at.Date = item.Date;
                        at.Day = item.Date.Value.DayOfWeek.ToString();
                        at.TimeIn = item.TimeIn;
                        at.TimeOut = item.TimeOut;
                        at.Status = "Present";
                        at.ShiftLength = length;
                        at.IP_Address = item.IP_Address;
                        report.Add(at);
                    }

                var absentdetail = db.UserAutoAbsents.Where(x => x.AspNetUser.UserName == username && x.Date > date).ToList();
                foreach (var item in absentdetail)
                    {
                        Attendance at = new Attendance();
                        at.Date = item.Date;
                        at.Name = item.AspNetUser.Name;
                        at.Day = item.Date.Value.DayOfWeek.ToString();
                        at.UserName = username;
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
                var presentdetails = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == username && x.Date.Value.Month == month).ToList();
                foreach (var item in presentdetails)
                {
                    var length = item.TimeOut - item.TimeIn;
                    Attendance at = new Attendance();
                    at.Name = item.AspNetUser.Name;
                    at.UserName = username;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.Status = "Present";
                    at.ShiftLength = length;
                    at.IP_Address = item.IP_Address;
                    report.Add(at);

                }
                var absent = db.UserAutoAbsents.Where(x => x.AspNetUser.UserName == username && x.Date.Value.Month == month).ToList();
                foreach (var item in absent)
                {
                    Attendance at = new Attendance();
                    at.Date = item.Date;
                    at.Name = item.AspNetUser.Name;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.UserName = username;
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

        public JsonResult RangeDateFilter(string username, DateTime start, DateTime end)
        {
            List<Attendance> report = new List<Attendance>();

            var present = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == username && x.Date >= start && x.Date <= end).ToList();
            foreach (var item in present)
            {
                var length = item.TimeOut - item.TimeIn;
                Attendance at = new Attendance();
                at.Name = item.AspNetUser.Name;
                at.UserName = username;
                at.Date = item.Date;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.TimeIn = item.TimeIn;
                at.TimeOut = item.TimeOut;
                at.Status = "Present";
                at.ShiftLength = length;
                at.IP_Address = item.IP_Address;
                report.Add(at);
            }
            var absent = db.UserAutoAbsents.Where(x => x.AspNetUser.UserName == username && x.Date >= start && x.Date <= end).ToList();
            foreach (var item in absent)
            {
                Attendance at = new Attendance();
                at.Date = item.Date;
                at.Name = item.AspNetUser.Name;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.UserName = username;
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