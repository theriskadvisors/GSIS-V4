using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class AutoAttendanceController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: AutoAttendance
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UserImage(string user_name)
        {
            //var image = db.AspNetUsers.Where(x => x.UserName == user_name).Select(x => x.Image).FirstOrDefault();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        public ActionResult Auto_Attendance(string AttendanceId)
        {
            var currentdate = DateTime.Now.Date;     
            var time = DateTime.Now.TimeOfDay;
            var paktime = time + new TimeSpan(05, 00, 00);        
            var AttId = AttendanceId.Split('_');
            var Id = AttId[0];
            var user = AttId[1];
            if (Id == "Std")
            {

                var result = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                var userid = db.AspNetUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();
                if (result==null)
                {
                    UserAutoPresent present = new UserAutoPresent();
                    present.Date = currentdate;
                    present.TimeIn = paktime;
                    present.TimeOut = null;
                    present.UserId = userid;
                    present.IP_Address = GetMACAddress();
                    present.UserType = "Student";
                    db.UserAutoPresents.Add(present);
                    db.SaveChanges();
                }
                else
                {
                    UserAutoPresent present = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                    var timecheck = present.TimeIn + new TimeSpan(00, 05, 00);
                    if (paktime > timecheck)
                    {
                        present.TimeOut = paktime;
                        db.SaveChanges();
                    }
                }
            }
            else if(Id=="Tec"){
                var result = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                var userid = db.AspNetUsers.Where(x => x.UserName == user).Select(x => x.Id).FirstOrDefault();
                if (result == null)
                {
                    UserAutoPresent present = new UserAutoPresent();
                    present.Date = currentdate;
                    present.TimeIn = paktime;
                    present.TimeOut = null;
                    present.UserId = userid;
                    present.IP_Address = GetMACAddress();
                    present.UserType = "Teacher";
                    db.UserAutoPresents.Add(present);
                    db.SaveChanges();
                }
                else
                {
                    UserAutoPresent present = db.UserAutoPresents.Where(x => x.AspNetUser.UserName == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                    var timecheck = present.TimeIn + new TimeSpan(00, 05, 00);
                    if (paktime > timecheck)
                    {
                        present.TimeOut = paktime;
                        db.SaveChanges();
                    }
                }
            }
            else if (Id == "GSISEmp")
            {
                var result = db.EmployeeAutoPresents.Where(x => x.Id.ToString() == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                var userid = db.AspNetEmployees.Where(x => x.Id.ToString() == user).Select(x => x.Id).FirstOrDefault();
                if (result == null)
                {
                    EmployeeAutoPresent present = new EmployeeAutoPresent();
                    present.Date = currentdate;
                    present.TimeIn = paktime;
                    present.TimeOut = null;
                    present.EmployeeId = userid;
                    present.IP_Address = GetMACAddress();
                    db.EmployeeAutoPresents.Add(present);
                    db.SaveChanges();
                }
                else
                {
                    EmployeeAutoPresent present = db.EmployeeAutoPresents.Where(x => x.Id.ToString() == user && x.Date == currentdate).Select(x => x).FirstOrDefault();
                    var timecheck = present.TimeIn + new TimeSpan(00, 05, 00);
                    if (paktime > timecheck)
                    {
                        present.TimeOut = paktime;
                        db.SaveChanges();
                    }
                }
            }
            return View();
        }

   
    }
}