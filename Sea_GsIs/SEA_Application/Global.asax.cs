using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication;
namespace SEA_Application
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Sea_Entities db = new Sea_Entities();
        protected void Application_Start()
        {
            //var timeofday = DateTime.Now;
            //var date = timeofday.Date;
            //var day = timeofday.DayOfWeek.ToString();

            //var holiday = db.CalendarNotifications.Where(x => x.EndDate >= date).ToList();
            //if (holiday.Count != 0)
            //{
            //    foreach (var item in holiday)
            //    {
            //        if (date <= item.StartDate && date <= item.EndDate)
            //        {
            //            if (day != "Sunday")
            //            {
            //                SetUpTimer(new TimeSpan(16, 00, 00));
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (day != "Sunday")
            //    {
            //        SetUpTimer(new TimeSpan(16, 00, 00));
            //    }
            //}
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        private System.Threading.Timer timer;
        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.SomeMethodRunsAt1600();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }
        private void SomeMethodRunsAt1600()
        {
            var datetime = DateTime.Now;
            var currentdate = datetime.Date;
            var timeout = new TimeSpan(16, 00, 00);
            //////////////////////////////////////////////USER TIME_OUT ///////////////////////////////////////////////////
            var Uout = db.UserAutoPresents.Where(x => x.Date == currentdate && x.TimeOut == null).ToList();
            foreach (var item in Uout)
            {
                UserAutoPresent at = db.UserAutoPresents.Where(x => x.AspNetUser.Id == item.UserId && x.Date == currentdate).FirstOrDefault();
                at.TimeOut = timeout;
                db.SaveChanges();
            }
            var pstd = db.UserAutoPresents.Where(x => x.Date == currentdate).Select(x => x.AspNetUser.UserName).ToList();
            var tstd = db.AspNetUsers.Where(x => x.AspNetStatu.Name != "Active").Select(x => x.UserName).ToList();
            var astd = tstd.Except(pstd).ToList();
            foreach (var item in astd)
            {
                UserAutoAbsent a = new UserAutoAbsent();
                var UserId = db.AspNetUsers.Where(x => x.UserName == item).Select(x=>x.Id).FirstOrDefault();
                a.Date = currentdate;
                a.UserId = UserId;
                db.SaveChanges();
            }
            ///////////////////////////////////////////EMPLOYEE TIME_OUT///////////////////////////////////////////////////////
            var Eout = db.EmployeeAutoPresents.Where(x => x.Date == currentdate && x.TimeOut == null).ToList();
            foreach (var item in Eout)
            {
                EmployeeAutoPresent at = db.EmployeeAutoPresents.Where(x => x.AspNetEmployee.Id == item.EmployeeId && x.Date == currentdate).FirstOrDefault();
                at.TimeOut = timeout;
                db.SaveChanges();
            }
            var Pstd = db.EmployeeAutoPresents.Where(x => x.Date == currentdate).Select(x => x.AspNetEmployee.Id.ToString()).ToList();
            var Tstd = db.AspNetEmployees.Where(x => x.VirtualRoleId==2).Select(x => x.Id.ToString()).ToList();
            var Astd = Tstd.Except(Pstd).ToList();
            foreach (var item in Astd)
            {
                EmployeeAbsentTable a = new EmployeeAbsentTable();
                var id = Int32.Parse(item);
                var EmpId = db.AspNetEmployees.Where(x => x.Id == id).Select(x => x.Id).FirstOrDefault();
                a.Date = currentdate;
                a.EmployeeId = EmpId;
                db.SaveChanges();
            }
        }
    }
}
