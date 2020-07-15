using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class EmployeeViewAttendanceController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: EmployeeViewAttendance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAttendance()
        {
            var currentdate = DateTime.Now.Date;
            var present = db.EmployeeAutoPresents.Where(x => x.Date == currentdate).ToList();
            List<Attendance> Attendance = new List<Attendance>();
            foreach (var item in present)
            {
               // var name = db.AspNetEmployees.Where(x => x.Id == item.EmployeeId).Select(x => x.Name).FirstOrDefault();
                Attendance at = new Attendance();

                at.Name = item.AspNetEmployee.Name;
                at.EID = item.EmployeeId;
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
                var present = db.EmployeeAutoPresents.Where(x => x.Date == currentdate).ToList();
                foreach (var item in present)
                {
                  //  var name = db.AspNetEmployees.Where(x => x.Id == item.EmployeeId).Select(x => x.Name).FirstOrDefault();
                    Attendance at = new Attendance();

                    at.Name = item.AspNetEmployee.Name;
                    at.EID = item.EmployeeId;
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
                var pstd = db.EmployeeAutoPresents.Where(x => x.Date == currentdate).Select(x => x.EmployeeId.ToString());
                var tstd = db.AspNetEmployees.Where(x => x.VirtualRoleId == 2).Select(x => x.Id.ToString()).ToList();
                var astd = tstd.Except(pstd).ToList();
                foreach (var item in astd)
                {
                    Attendance a = new Attendance();
                    var name = db.AspNetEmployees.Where(x => x.Id.ToString() == item).FirstOrDefault();
                    a.Name = name.Name;
                    a.EID = name.Id;
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
                var present = db.EmployeeAutoPresents.Where(x => x.Date == date).ToList();
                foreach (var item in present)
                {
                  //  var name = db.AspNetEmployees.Where(x => x.Id == item.Id).Select(x => x.Name).FirstOrDefault();
                    Attendance at = new Attendance();

                    at.Name = item.AspNetEmployee.Name;
                    at.EID = item.EmployeeId;
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
                var absent = db.EmployeeAbsentTables.Where(x => x.Date == date).ToList();
                foreach (var item in absent)
                {
                    var name = db.AspNetEmployees.Where(x => x.Id == item.Id).FirstOrDefault();
                    Attendance at = new Attendance();

                    at.Name = name.Name;
                    at.EID = item.EmployeeId;
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
            public int? EID { get; set; }
            public string Day { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? TimeIn { get; set; }
            public TimeSpan? TimeOut { get; set; }
            public string IP_Address { get; set; }

        }
    }
}