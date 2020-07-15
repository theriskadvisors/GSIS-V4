using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using System.IO;
namespace SEA_Application.Controllers
{
    public class AspNetBranchAnnouncementsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: AspNetBranchAnnouncements
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StudentsPasswords()
        {
            return View();
        }

        public ActionResult GetStudentsPasswords()
        {
            var passwords = db.ruffdatas.ToList();
            return Json(passwords, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentTestSubmission(int id)
        {
            var IsSubmitted = "";
            var UserId1 = User.Identity.GetUserId();

            AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();


          
              
                var File = Request.Files["file"];

                var fileName = "";
                if (File.ContentLength > 0)
                {
                    fileName = Path.GetFileName(File.FileName);
                    File.SaveAs(Server.MapPath("~/Content/StudentSubmittedTest/") + fileName);

                }

            

            return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnnouncementIndex()
        {
            var id = User.Identity.GetUserId();
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
            ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                             join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                             join te in db.AspNetTeacher_Enrollments on clascours.Id equals te.CourseId
                                             where te.TeacherId == teacherid
                                             select new { cls.Id, cls.Name }, "Id", "Name").ToList();
            return View();
        }
        public ActionResult GetAccouncement()
        {
            var id = User.Identity.GetUserId();
            //var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
            var branchIDs = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == id).Select(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id).Distinct().ToList();
            var AnnouncementList = (from announcement in db.AspNetAnnouncements.Where(x=> branchIDs.Contains(x.BranchId.Value))
                                    select new { announcement.Id, announcement.Title, announcement.Description, announcement.FileName, date = announcement.Date.ToString() }).ToList();

            return Json(AnnouncementList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult SubjectsByClass(string[] id)
        {
            var TeacherID = User.Identity.GetUserId();
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == TeacherID).Select(x => x.Id).FirstOrDefault();
            db.Configuration.ProxyCreationEnabled = false;
            List<int> ids = new List<int>();

            foreach (var item in id)
            {
                int a = Convert.ToInt32(item);
                ids.Add(a);
            }

            var Subjects = (from clascours in db.AspNetClass_Courses
                            join employees in db.AspNetTeacher_Enrollments on clascours.Id equals employees.CourseId
                            where ids.Contains(clascours.ClassId) && employees.TeacherId == teacherid
                            select new { clascours.AspNetCours.Name, clascours.CourseId }).ToList();

            //  var Subjects = db.AspNetClass_Courses.Where(x => ids.Contains(x.AspNetClass.Id) && x.tea == TeacherID).Select(x => new { x.AspNetCours.Name, x.Id }).ToList();
            ViewBag.Subjects = Subjects;
            return Json(Subjects, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public JsonResult SubjectsByClass(int id)
        //{
        //    var TeacherID = User.Identity.GetUserId();
        //    var teacherid = db.AspNetEmployees.Where(x => x.UserId == TeacherID).Select(x => x.Id).FirstOrDefault();

        //    if (User.IsInRole("Teacher"))
        //    {
        //        db.Configuration.ProxyCreationEnabled = false;

        //        // var classTeacherID = db.AspNetClasses.Where(x => x.Id == id).Select(x => x.TeacherID).FirstOrDefault();
        //        var classTeacherID = (from cls in db.AspNetClasses
        //                              join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
        //                              join te in db.AspNetTeacher_Enrollments on clascours.Id equals te.CourseId
        //                              where te.TeacherId == teacherid && cls.Id==id
        //                              select new { te.AspNetEmployee.UserId }).FirstOrDefault();


        //        if (TeacherID == TeacherID)
        //        {
        //            db.Configuration.ProxyCreationEnabled = false;
        //            var subject = (from subjects in db.AspNetSubjects
        //                           orderby subjects.Id descending
        //                           where subjects.ClassID == id
        //                           select new { subjects.Id, subjects.SubjectName }).ToList();

        //            return Json(subject, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            db.Configuration.ProxyCreationEnabled = false;
        //            List<AspNetSubject> sub = db.AspNetSubjects.Where(r => r.ClassID == id && r.TeacherID == TeacherID).OrderByDescending(r => r.Id).ToList();

        //            return Json(sub, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    else
        //    {
        //        var sub = db.AspNetSubjects.Where(x => x.ClassID == id).Select(x => new { x.Id, x.SubjectName }).ToList();
        //        return Json(sub, JsonRequestBehavior.AllowGet);
        //    }


        //}

        // GET: AspNetAnnouncements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAnnouncement aspNetAnnouncement = db.AspNetAnnouncements.Find(id);
            if (aspNetAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(aspNetAnnouncement);
        }

        // GET: AspNetAnnouncements/Create
        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
            ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                             join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                             join te in db.AspNetTeacher_Enrollments on clascours.Id equals te.CourseId
                                             where te.TeacherId == teacherid
                                             select new { cls.Id, cls.Name }, "Id", "Name").ToList();
            return View();
        }

        // POST: AspNetAnnouncements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Date")] AspNetAnnouncement aspNetAnnouncement)
        {


            HttpPostedFileBase Assignment = Request.Files["FileName"];
            var fileName = Path.GetFileName(Assignment.FileName);
        

            if (Assignment.ContentLength > 0)
            {
                Assignment.SaveAs(Server.MapPath("~/Content/BranchAnnouncement/") + fileName);

            }


            if (ModelState.IsValid)
            {
                var dbContextTransaction = db.Database.BeginTransaction();
                try
                {
                    aspNetAnnouncement.FileName = fileName;
                    aspNetAnnouncement.Date = DateTime.Now;
                    db.AspNetAnnouncements.Add(aspNetAnnouncement);
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    //int announcementID = db.AspNetAnnouncements.Max(item => item.Id);
                    //List<int> SubjectIDs = new List<int>();
                  
                    
                    //var student = (from std in db.AspNetStudents
                                   
                    //               select new { std.UserId }).ToList();

                    //foreach (var item in student)
                    //{

                    //    AspNetStudent_Announcement stu_ann = new AspNetStudent_Announcement();
                    //    stu_ann.UserID = item.UserId;
                    //    stu_ann.AnnouncementID = announcementID;
                    //    stu_ann.Seen = false;
                    //    db.AspNetStudent_Announcement.Add(stu_ann);
                    //    db.SaveChanges();
                    //}
                    //////////////////////////////Notification///////////////////////////////////

                    var NotificationObj = new AspNetNotification();
                    NotificationObj.Description = aspNetAnnouncement.Description;
                    NotificationObj.Subject = aspNetAnnouncement.Title;
                    NotificationObj.SenderID = User.Identity.GetUserId();
                    NotificationObj.Time = DateTime.Now;
                    NotificationObj.Url = "/AspNetAnnouncements/Details/" + aspNetAnnouncement.Id;
                    db.AspNetNotifications.Add(NotificationObj);
                    db.SaveChanges();

                 var NotificationID = db.AspNetNotifications.Max(x => x.Id);
                   // var students = db.AspNetStudent_Announcement.Where(sp => sp.AnnouncementID == aspNetAnnouncement.Id).Select(x => x.UserID).ToList();

                      //ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                      //                       join classcourse in db.AspNetClass_Courses on cls.Id equals classcourse.ClassId
                      //                       join enrolment in db.AspNetTeacher_Enrollments on classcourse.Id equals enrolment.CourseId
                      //                       where enrolment.TeacherId==tid
                      //                       select new {cls.Id, cls.Name }, "Id", "Name").Distinct().ToList();

                    
               //  var parents = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Parent")  || x.AspNetRoles.Select(y=>y.Name).Contains("Teacher")  && x.StatusId == null).ToList();




                 //foreach (var receiver in parents)
                 //   {
                 //       var notificationRecieve = new AspNetNotification_User();
                 //       notificationRecieve.NotificationID = NotificationID;
                 //       notificationRecieve.UserID = receiver.Id;
                 //       notificationRecieve.Seen = false;
                 //       db.AspNetNotification_User.Add(notificationRecieve);
                 //       db.SaveChanges();
                 //   }


                }
                catch (Exception e)
                {
                    var dsds = e.Message;
                    dbContextTransaction.Dispose();
                }

                return RedirectToAction("AnnouncementIndex", "AspNetBranchAnnouncements");
            }

            return View(aspNetAnnouncement);
        }


        // GET: AspNetAnnouncements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAnnouncement aspNetAnnouncement = db.AspNetAnnouncements.Find(id);
            if (aspNetAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(aspNetAnnouncement);
        }

        // POST: AspNetAnnouncements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Date")] AspNetAnnouncement aspNetAnnouncement)
        {
            var id = User.Identity.GetUserId();
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
            if (ModelState.IsValid)
            {
                aspNetAnnouncement.BranchId = teacherid.BranchId;
                db.Entry(aspNetAnnouncement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AnnouncementIndex");
            }
            return View(aspNetAnnouncement);
        }

        // GET: AspNetAnnouncements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetAnnouncement aspNetAnnouncement = db.AspNetAnnouncements.Find(id);
            if (aspNetAnnouncement == null)
            {
                return HttpNotFound();
            }
            return View(aspNetAnnouncement);
        }

        // POST: AspNetAnnouncements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetAnnouncement aspNetAnnouncement = db.AspNetAnnouncements.Find(id);
            db.AspNetAnnouncements.Remove(aspNetAnnouncement);
            db.SaveChanges();
            return RedirectToAction("AnnouncementIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}