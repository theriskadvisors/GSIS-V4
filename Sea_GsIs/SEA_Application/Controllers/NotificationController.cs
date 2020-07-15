using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        private Sea_Entities db = new Sea_Entities();
        public ActionResult Index()
        {
            return View();
        }
        //public JsonResult GetNotifications()
        //{
        //    try
        //    {
        //        var UserNameLog = User.Identity.Name;
        //        AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
        //        if (this.User.IsInRole("Teacher"))
        //        {

        //            var NotificationsList = (from notification in db.AspNetNotification_User
        //                                     where notification.UserID == currentUser.Id && notification.Seen == false
        //                                     select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();
        //        }
        //        if (this.User.IsInRole("Parent"))
        //        {
        //            var NotificationsList = (from notification in db.AspNetNotification_User
        //                                     where notification.UserID == currentUser.Id && notification.Seen == false
        //                                     select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();
        //            return Json(NotificationsList, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var NotificationsList = (from notification in db.AspNetNotification_User
        //                                     where notification.UserID == currentUser.Id && notification.Seen == false
        //                                     select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();
        //            return Json(NotificationsList, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //}


        public JsonResult GetNotifications()
        {
            //using (SEA_DatabaseEntities db = new SEA_DatabaseEntities()) {
            try
            {
                var UserNameLog = User.Identity.Name;
                AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
                if (this.User.IsInRole("Teacher"))
                {

                    var NotificationsList = (from notification in db.AspNetNotification_User
                                             where notification.UserID == currentUser.Id && notification.Seen == false
                                             select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();

                    //  List<notifications> NotificationsList = new List<notifications>();
                    //  var teacher = db.AspNetEmployees.Where(x => x.UserName == currentUser.UserName).Select(x => x).ToList();
                    //  foreach (var item in teacher)
                    // {
                    //   var TeacherNotification = (from notification in db.AspNetPushNotifications
                    //                            where int.Parse(notification.UserID) == item.Id // && notification.IsOpenTeacher==false
                    //                            select new { notification.Id, notification.Message, notification.Time, notification.UserID }).ToList();
                    //    foreach (var notificationOfList in TeacherNotification)
                    //    {
                    //      notifications notification = new notifications();
                    //      notification.Id = notificationOfList.Id;
                    //      notification.Message = notificationOfList.Message;
                    //      notification.Time = notificationOfList.Time;
                    //      notification.UserID = notificationOfList.UserID;
                    //      NotificationsList.Add(notification);
                    //   }
                    //}
                }
                if (this.User.IsInRole("Parent"))
                {

                    //List<notifications> NotificationsList = new List<notifications>();

                    var NotificationsList = (from notification in db.AspNetNotification_User
                                             where notification.UserID == currentUser.Id && notification.Seen == false
                                             select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();


                    //var NotificationsList = db.AspNetPushNotifications.Where(x => x.UserID == currentUser.Id && x.IsOpen == false).ToList();
                    return Json(NotificationsList, JsonRequestBehavior.AllowGet);

                }
                if (this.User.IsInRole("Student"))
                {

                    //List<notifications> NotificationsList = new List<notifications>();

                    var NotificationsList = (from notification in db.AspNetNotification_User
                                             where notification.UserID == currentUser.Id && notification.Seen == false
                                             select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();


                    //var NotificationsList = db.AspNetPushNotifications.Where(x => x.UserID == currentUser.Id && x.IsOpen == false).ToList();
                    return Json(NotificationsList, JsonRequestBehavior.AllowGet);

                }


                else
                {
                    //List<notifications> NotificationsList = new List<notifications>();

                    //var NotificationsList = db.AspNetPushNotifications.Where(x => x.UserID == currentUser.Id && x.IsOpen == false).ToList();
                    var NotificationsList = (from notification in db.AspNetNotification_User
                                             where notification.UserID == currentUser.Id && notification.Seen == false
                                             select new { notification.Id, notification.AspNetNotification.Subject, notification.AspNetNotification.Time, notification.AspNetNotification.Description, notification.AspNetNotification.SenderID }).ToList();

                    return Json(NotificationsList, JsonRequestBehavior.AllowGet);
                }
                //}
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult Details(int? id)
        //{
        //    var UserNameLog = User.Identity.Name;
        //    AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AspNetNotification_User aspNetNotification = db.AspNetNotification_User.Where(x => x.Id == id).FirstOrDefault();
        //    if (aspNetNotification.UserID == currentUser.Id)
        //    {
        //        aspNetNotification.Seen = true;
        //        db.SaveChanges();
        //    }

        //    if (aspNetNotification == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aspNetNotification.AspNetNotification);
        //}


        public ActionResult Details(int? id)
        {
            var UserNameLog = User.Identity.Name;
            AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //if (this.User.IsInRole("Teacher"))
            //{
            //    ViewBag.AchorTagText = "Reply to Student Comment";
            //}
            //else if (this.User.IsInRole("Student"))
            //{
            //    ViewBag.AchorTagText = "See Teacher Reply";
            //}
            //else
            //{
            //    ViewBag.AchorTagText = "";
            //}

            AspNetNotification_User aspNetNotification = db.AspNetNotification_User.Where(x => x.Id == id).FirstOrDefault();

            if (aspNetNotification.UserID == currentUser.Id)
            {
                aspNetNotification.Seen = true;
                db.SaveChanges();
            }

            ViewBag.AchorTagText = aspNetNotification.AspNetNotification.NavigateText;

            if (aspNetNotification == null)
            {
                return HttpNotFound();
            }
            return View(aspNetNotification.AspNetNotification);
        }
    }
}