﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using System.Net;

namespace SEA_Application.Controllers
{
    public class StudentAnnouncementsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();


        // GET: StudentAnnouncements

    

        // GET: AspNetAnnouncements

        public ActionResult AnnouncementIndex()
        {
            var id = User.Identity.GetUserId();
            int StudentId=   db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault().Id;

            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
            ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                             join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                             join te in db.AspNetStudent_Enrollments on clascours.Id equals te.CourseId
                                             where te.StudentId == StudentId
                                             select new { cls.Id, cls.Name }, "Id", "Name").ToList();
            return View();
        }
        public ActionResult GetAccouncement()
        {
            var id = User.Identity.GetUserId();
          //  var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
            var student =  db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();
            var AnnouncementList = (from announcement in db.AspNetAnnouncements
                                    join anouncmnt_sub in db.AspNetAnnouncement_Subject on announcement.Id equals anouncmnt_sub.AnnouncementID
                                    join clascours in db.AspNetClass_Courses on anouncmnt_sub.SubjectID equals clascours.CourseId
                                    join te in db.AspNetStudent_Enrollments on clascours.Id equals te.CourseId
                                    where te.StudentId == student.Id && announcement.BranchId == student.BranchId
                                    select new { announcement.Id, announcement.Title, announcement.Description, anouncmnt_sub.AspNetCours.Name }).ToList();
            return Json(AnnouncementList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminAnnouncement()
        {
            var id = User.Identity.GetUserId();
            //   var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
            var student = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();


            ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                             join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                             join te in db.AspNetStudent_Enrollments on clascours.Id equals te.CourseId
                                             where te.StudentId == student.Id
                                             select new { cls.Id, cls.Name }, "Id", "Name").ToList();
            return View();
        }
        public ActionResult GetAdminAccouncement()
        {
            var id = User.Identity.GetUserId();
           // var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
           
            var student = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();
            var AnnouncementList = (from announcement in db.AspNetAnnouncements

                select new { announcement.Id, announcement.Title, announcement.Description, date = announcement.Date.ToString() }).ToList();

            return Json(AnnouncementList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit1(int? id)
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
        public ActionResult Edit1([Bind(Include = "Id,Title,Description,Date")] AspNetAnnouncement aspNetAnnouncement)
        {
            var id = User.Identity.GetUserId();
            //var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();

            var student = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                aspNetAnnouncement.BranchId = student.BranchId;
                db.Entry(aspNetAnnouncement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminAnnouncement");
            }
            return View(aspNetAnnouncement);
        }



        [HttpGet]
        public JsonResult SubjectsByClass(string[] id)
        {
            if (id[0] != "" || id == null)
            {
                var TeacherID = User.Identity.GetUserId();
                var teacherid = db.AspNetEmployees.Where(x => x.UserId == TeacherID).Select(x => x.Id).FirstOrDefault();
                var student = db.AspNetStudents.Where(x => x.UserId == TeacherID).FirstOrDefault();
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
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
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
          //  var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
            var student = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();

            ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                             join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                             join te in db.AspNetStudent_Enrollments on clascours.Id equals te.CourseId
                                             where te.StudentId == student.Id
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
            var id = User.Identity.GetUserId();
          //  var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
               var studentid = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();
            string subjects = Request.Form["subjects"];
            string ClsID = Request.Form["ClassId"];
            var ClassID = Convert.ToInt32(ClsID);

            IEnumerable<string> selectedsubjects = subjects.Split(',');
            if (ModelState.IsValid)
            {
                var dbContextTransaction = db.Database.BeginTransaction();
                try
                {

                    aspNetAnnouncement.Date = DateTime.Now;
                    aspNetAnnouncement.BranchId = studentid.BranchId;
                    db.AspNetAnnouncements.Add(aspNetAnnouncement);
                    db.SaveChanges();

                    int announcementID = db.AspNetAnnouncements.Max(item => item.Id);
                    List<int> SubjectIDs = new List<int>();
                    foreach (var item in selectedsubjects)
                    {
                        int subjectID = Convert.ToInt32(item);
                        SubjectIDs.Add(subjectID);
                    }
                    foreach (var item in SubjectIDs)
                    {
                        AspNetAnnouncement_Subject ann_sub = new AspNetAnnouncement_Subject();
                        ann_sub.SubjectID = item;
                        ann_sub.AnnouncementID = announcementID;
                        db.AspNetAnnouncement_Subject.Add(ann_sub);
                        db.SaveChanges();
                    }
                    var student = (from cls in db.AspNetClass_Courses
                                   join std_enrolment in db.AspNetStudent_Enrollments on cls.Id equals std_enrolment.CourseId
                                   join emp_enrolment in db.AspNetStudent_Enrollments on cls.Id equals emp_enrolment.CourseId
                                   where SubjectIDs.Contains(cls.CourseId) && emp_enrolment.StudentId == studentid.Id && cls.ClassId == ClassID
                                   select new { std_enrolment.AspNetStudent.UserId }).ToList();

                    foreach (var item in student)
                    {

                        AspNetStudent_Announcement stu_ann = new AspNetStudent_Announcement();
                        stu_ann.UserID = item.UserId;
                        stu_ann.AnnouncementID = announcementID;
                        stu_ann.Seen = false;
                        db.AspNetStudent_Announcement.Add(stu_ann);
                        db.SaveChanges();
                    }
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
                    var students = db.AspNetStudent_Announcement.Where(sp => sp.AnnouncementID == aspNetAnnouncement.Id).Select(x => x.UserID).ToList();

                    var users = new List<String>();

                    foreach (var item in students)
                    {
                        var parentID = db.AspNetParent_Child.Where(x => x.ChildID == item).Select(x => x.ParentID).FirstOrDefault();
                        users.Add(parentID);
                    }

                    var allusers = users.Union(students);

                    foreach (var receiver in allusers)
                    {
                        var notificationRecieve = new AspNetNotification_User();
                        notificationRecieve.NotificationID = NotificationID;
                        notificationRecieve.UserID = receiver;
                        notificationRecieve.Seen = false;
                        db.AspNetNotification_User.Add(notificationRecieve);
                        db.SaveChanges();
                    }

                    dbContextTransaction.Commit();

                }
                catch (Exception e)
                {
                    var dsds = e.Message;
                    dbContextTransaction.Dispose();
                }

                return RedirectToAction("AnnouncementIndex", "AspNetAnnouncements");
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
            var studentid = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();

            //var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
            if (ModelState.IsValid)
            {
                aspNetAnnouncement.BranchId = studentid.BranchId;
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