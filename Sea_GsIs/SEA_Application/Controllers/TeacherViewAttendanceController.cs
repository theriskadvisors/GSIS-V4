using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class TeacherViewAttendanceController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: TeacherViewAttendance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAttendance()
        {
            var currentdate = DateTime.Now.Date;
            var present = db.UserAutoPresents.Where(x => x.Date == currentdate && x.UserType=="Teacher").ToList();
            List<Attendance> Attendance = new List<Attendance>();
            foreach (var item in present)
            {
                Attendance at = new Attendance();

                at.Name = item.AspNetUser.Name;
                at.UserName = item.AspNetUser.UserName;
                at.Date = item.Date;
                at.Day = item.Date.Value.DayOfWeek.ToString();
                at.TimeIn = item.TimeIn;
                at.TimeOut = item.TimeOut;
                at.IP_Address = item.IP_Address;
                Attendance.Add(at);
            }
            return Json(Attendance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Status(string type)
        {
            List<Attendance> attendance = new List<Attendance>();
            var currentdate = DateTime.Now.Date;
            if (type == "Present")
            {
                var present = db.UserAutoPresents.Where(x => x.Date == currentdate && x.UserType == "Teacher").ToList();
                foreach (var item in present)
                {
                    Attendance at = new Attendance();

                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.IP_Address = item.IP_Address;
                    attendance.Add(at);
                }
            }
            else if (type == "Absent")
            {
                var pstd = db.UserAutoPresents.Where(x => x.Date == currentdate && x.UserType == "Teacher").Select(x => x.UserId).ToList();
                var tstd = db.AspNetEmployees.Where(x=>x.VirtualRoleId==4).Select(x => x.UserId).ToList();
                var astd = tstd.Except(pstd).ToList();
                foreach (var item in astd)
                {
                    Attendance a = new Attendance();
                    var un = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                    a.Name = un.Name;
                    a.UserName = un.UserName;
                    a.Date = currentdate;
                    a.Day = currentdate.DayOfWeek.ToString();
                    a.TimeIn = null;
                    a.TimeOut = null;
                    a.IP_Address = null;
                    attendance.Add(a);
                }
            }
            return Json(attendance, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DateFilter(string type, DateTime date)
        {
            List<Attendance> attendance = new List<Attendance>();
            if (type == "Present")
            {
                var present = db.UserAutoPresents.Where(x => x.Date == date && x.UserType == "Teacher").ToList();
                foreach (var item in present)
                {
                    Attendance at = new Attendance();

                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = item.TimeIn;
                    at.TimeOut = item.TimeOut;
                    at.IP_Address = item.IP_Address;
                    attendance.Add(at);
                }
                return Json(attendance, JsonRequestBehavior.AllowGet);
            }
            else if (type == "Absent")
            {
                var absent = db.UserAutoAbsents.Where(x => x.Date == date && x.UserType == "Teacher").ToList();
                foreach (var item in absent)
                {
                    Attendance at = new Attendance();

                    at.Name = item.AspNetUser.Name;
                    at.UserName = item.AspNetUser.UserName;
                    at.Date = item.Date;
                    at.Day = item.Date.Value.DayOfWeek.ToString();
                    at.TimeIn = null;
                    at.TimeOut = null;
                    at.IP_Address = null;
                    attendance.Add(at);
                }
                return Json(attendance, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public class Attendance
        {
            public string Name { get; set; }
            public string UserName { get; set; }
            public string Day { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? TimeIn { get; set; }
            public TimeSpan? TimeOut { get; set; }
            public string IP_Address { get; set; }

        }
    }
}