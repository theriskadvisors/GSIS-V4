using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace SEA_Application.Controllers
{
    public class ParentController : Controller
    {
        private string ParentID;
        private static string StudentID;
        private Sea_Entities db = new Sea_Entities();
        public ParentController()
        {
            ParentID = Convert.ToString(System.Web.HttpContext.Current.Session["ParentID"]);
        }
        [HttpGet]
        public void SetChildID(string studentID)
        {
            StudentID = studentID;
            Session["ChildID"] = studentID;
        }

        [HttpGet]
        public JsonResult GetChildID(string studentID)
        {
            if (StudentID == null)
            {
                StudentID = studentID;
                Session["ChildID"] = studentID;
            }
            string v1 = db.AspNetParent_Child.Where(x => x.ChildID == studentID).Select(x => x.ParentID).FirstOrDefault();
            string v2 = db.AspNetParent_Child.Where(x => x.ChildID == StudentID).Select(x => x.ParentID).FirstOrDefault();
            if (v1 != v2)
            {
                return Json(studentID, JsonRequestBehavior.AllowGet);

            }

            return Json(StudentID, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetProjectList()
        {
            List<int> subjectIDs = (from student_subject in db.AspNetStudent_Subject
                                    where student_subject.StudentID == StudentID
                                    select student_subject.SubjectID).ToList();
            ViewBag.StudentID = StudentID;
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects.Where(x => subjectIDs.Contains(x.Id)), "Id", "SubjectName");

            //ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");
            return View();
        }
        public JsonResult GetChildren()
        {
            var children = (from child in db.AspNetUsers
                            join parent_children in db.AspNetParent_Child on child.Id equals parent_children.ChildID
                            where parent_children.ParentID == ParentID
                            select new { child.Id, child.Name }).ToList();

            return Json(children, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult ProjectDetails(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AspNetProject aspNetProject = db.AspNetProjects.Find(id);
        //    if (aspNetProject == null)
        //    {
        //        return HttpNotFound();
        //    }
        //   // ViewBag.ClassID = new SelectList(db.AspNetSubjects.Where(x => x.TeacherID == TeacherID).Select(x => x.AspNetClass).Distinct(), "Id", "ClassName");
        //    ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");
        //    return View(aspNetProject);
        //}

        
        // GET: Parent
        public ActionResult Index()
        {

            return View();
        }
        public JsonResult GetEvents()
        {
            using (Sea_Entities dc = new Sea_Entities())
            {
                var id = User.Identity.GetUserId();
                var events = dc.Events.Where(x => x.UserId == id || x.IsPublic == true).Select(x => new { x.Description, x.End, x.EventID, x.IsFullDay, x.Subject, x.ThemeColor, x.Start, x.IsPublic }).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
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
        [HttpPost]
        public JsonResult ParentComment(string status, string comment, int homeworkId)
        {
            var dbTransection = db.Database.BeginTransaction();
            var stdid = db.AspNetStudents.Where(x => x.UserId == StudentID).Select(x => x.Id).FirstOrDefault();
            
            db.AspNetStudentHomeWorks.Where(x => x.StudentId == stdid && x.HomeWorkId == homeworkId).FirstOrDefault().ParentComment = comment;
            db.AspNetStudentHomeWorks.Where(x => x.StudentId == stdid && x.HomeWorkId == homeworkId).FirstOrDefault().Status = status;
            db.SaveChanges();
            dbTransection.Commit();

            return Json("Saved", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Parent_HomeWork()
        {
            var pid = User.Identity.GetUserId();
            ViewBag.ParentId = db.AspNetParent_Child.Where(x => x.ParentID == pid).Select(x => x.ChildID).FirstOrDefault();
           
            return View();
        }
        public ActionResult DiaryDetails(int? id)
        {
            var stdid = db.AspNetStudents.Where(x => x.UserId == StudentID).Select(x => x.Id).FirstOrDefault();
            var courses = db.AspNetStudent_Enrollments.Where(x => x.StudentId == stdid).Select(x => x.CourseId).ToList();
            var aspNetSubject_Homework = db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == id ).Include(a => a.AspNetCours);

            ViewBag.teachercomment = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == id && x.StudentId== stdid).Select(x => x.TeacherComment).FirstOrDefault();
            ViewBag.parentcomment = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == id && x.StudentId == stdid).Select(x => x.ParentComment).FirstOrDefault();
            ViewBag.Status = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == id && x.StudentId.ToString() == StudentID).Select(x => x.Status).FirstOrDefault();
            ViewBag.Attachment = db.AspNetHomeWorks.Where(x => x.Id == id).Select(x => x.Attachment).FirstOrDefault();
            return View(aspNetSubject_Homework.ToList());
        }
        public JsonResult StudentHomeWork(string id)
        {
            var sid = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();

            var homework = (from student_homework in db.AspNetStudentHomeWorks
                            join hw in db.AspNetHomeWorks on student_homework.HomeWorkId equals hw.Id
                            where student_homework.StudentId == sid.Id && hw.Principal_Approved_Status == "Approved"
                            select new { student_homework.TeacherComment, student_homework.AspNetHomeWork.Id, student_homework.AspNetHomeWork.Date }).OrderByDescending(x => x.Date).ToList();
            return Json(homework, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdminAnnouncement()
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
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => new { x.Id, x.BranchId }).FirstOrDefault();
            var AnnouncementList = (from announcement in db.AspNetAnnouncements

                                    select new { announcement.Id, announcement.Title, announcement.Description, date = announcement.Date.ToString() }).ToList();

            return Json(AnnouncementList, JsonRequestBehavior.AllowGet);
        }

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
                return RedirectToAction("AdminAnnouncement");
            }
            return View(aspNetAnnouncement);
        }



        public ActionResult Get_ParentDiary()
        {
            var stdid = db.AspNetStudents.Where(x => x.UserId == StudentID).Select(x => x.Id).FirstOrDefault();
            var homework = (from student_homework in db.AspNetStudentHomeWorks
                            join hw in db.AspNetHomeWorks on student_homework.HomeWorkId equals hw.Id
                            where student_homework.StudentId == stdid
                            select new { student_homework.TeacherComment, student_homework.AspNetHomeWork.Id, student_homework.AspNetHomeWork.Date }).OrderByDescending(x => x.Date).ToList();

            return Json(homework,JsonRequestBehavior.AllowGet);
        }
        public ActionResult CalendarNotification()
        {
            var id = User.Identity.GetUserId();
            var fullname = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var namelist = fullname.Split(' ');
            var name = namelist[0];
            var result = new { name };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /////////////////////////Announcement////////////////////
        public ActionResult Announcement()
        {
            var std = db.AspNetStudents.Where(x => x.UserId == StudentID).Select(x => x).FirstOrDefault();

            //ViewBag.SubjectID = new SelectList(db.AspNetBranch_Class.Where(x => x.BranchId == std.BranchId && x.ClassId == std.ClassId).Select(x => new { Id = x.ClassId, Name = x.AspNetClass.Name }), "Id", "Name").ToList();
            //ViewBag.SubjectID = new SelectList(db.AspNetStudents.Where(x=> x.ClassId == std.ClassId && x.BranchId == std.BranchId), "Id", "Name");
            return View();
        }
        public ActionResult GetAnnouncement()
        {
            var branchid = db.AspNetStudents.Where(x => x.UserId == StudentID).Select(x => x.BranchId).FirstOrDefault();
            var announcement = (from std_ann in db.AspNetStudent_Announcement
                                join sub_ann in db.AspNetAnnouncement_Subject on std_ann.AnnouncementID equals sub_ann.AnnouncementID
                                where std_ann.UserID == StudentID && std_ann.AspNetAnnouncement.BranchId == branchid
                                select new { std_ann.AnnouncementID, sub_ann.AspNetCours.Name, sub_ann.AspNetAnnouncement.Title, Date = std_ann.AspNetAnnouncement.Date.ToString() }).ToList();

            return Json(announcement,JsonRequestBehavior.AllowGet);
        }
        public ActionResult AnnouncementDetails(int? id)
        {
            ViewBag.AnnouncementID = id;
            return View();
        }
        public JsonResult GetAnnouncementDetails(int? id)
        
{
            var Announcement = (from ann in db.AspNetAnnouncements
                         join ann_sub in db.AspNetAnnouncement_Subject on ann.Id equals ann_sub.AnnouncementID
                         where ann.Id == id
                         select new { ann.Title, ann.Description, ann_sub.AspNetCours.Name }).FirstOrDefault();
            //var Announcement = db.AspNetAnnouncements.Where(x => x.Id == id).Select(x => new { x.Title,x.Description,x.AspNetAnnouncement_Subject}).FirstOrDefault();

            return Json(Announcement,JsonRequestBehavior.AllowGet);
        }
    }
}