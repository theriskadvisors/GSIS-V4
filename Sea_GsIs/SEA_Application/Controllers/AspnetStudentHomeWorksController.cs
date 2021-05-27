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

namespace SEA_Application.Controllers
{
    public class AspnetStudentHomeWorksController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // GET: AspnetStudentHomeWorks

        public ActionResult Index()
        {
            var uid = User.Identity.GetUserId();
            var tid = db.AspNetEmployees.Where(x => x.UserId == uid).Select(x => x.Id).FirstOrDefault();
            var aspNetHomeWorks = db.AspNetHomeWorks.Include(a => a.AspNetClass);
            //ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
            //                                 join classcourse in db.AspNetClass_Courses on cls.Id equals classcourse.ClassId
            //                                 join enrolment in db.AspNetTeacher_Enrollments on classcourse.Id equals enrolment.CourseId
            //                                 where enrolment.TeacherId==tid
            //                                 select new {cls.Id, cls.Name }, "Id", "Name").Distinct().ToList();



            return View(aspNetHomeWorks.ToList());
        }

        //public ActionResult GetClassListbyTeacher()
        //{
        //  var uid = User.Identity.GetUserId();
        //  var tid = db.AspNetEmployees.Where(x => x.UserId == uid).Select(x => x.Id).FirstOrDefault();

        ////  string status;

        //  //List<GetClassbyTeacher_Result> list = new List<GetClassbyTeacher_Result>();
        //  //list = db.GetClassbyTeacher(tid.ToString()).ToList();
        //  //    status = Newtonsoft.Json.JsonConvert.SerializeObject(list);

        //  return Content(status);
        //   }

        public JsonResult DiaryByClass(int classId)
        {
            var Diary = (from diary in db.AspNetHomeWorks
                         where diary.ClassId == classId
                         select new { diary.Id, diary.AspNetClass.Name, diary.Date, diary.Principal_Approved_Status }).OrderByDescending(p => p.Date).Distinct().ToList();

            return Json(Diary, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Student_Diary(int HomeWorkId)
        {
            ViewBag.HomeWorkId = HomeWorkId;
            return View();
        }
        public ActionResult Student_DiaryIndex(int HomeWorkId)
        {
            ViewBag.HomeWorkId = db.AspNetHomeWorks.Where(x => x.Id == HomeWorkId).FirstOrDefault().Principal_Approved_Status;
            var aspNetStudent_HomeWork = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == HomeWorkId).Select(x => new { x.AspNetStudent.RollNo, x.TeacherComment, x.ParentComment, x.Status, x.AspNetStudent.Name, x.AspNetHomeWork.Date }).ToList();
            //   var data = Newtonsoft.Json.JsonConvert.SerializeObject(aspNetStudent_HomeWork.ToList());
            // return Content(data);
            return Json(aspNetStudent_HomeWork, JsonRequestBehavior.AllowGet);
            //    return View(aspNetStudent_HomeWork.ToList());
        }
        //public JsonResult SubjectByClass(int classId)
        //{
        //    var Subjects = (from classSubject in db.AspNetClass_Courses
        //                    join subject in db.AspNetCourses on classSubject.CourseId equals subject.Id
        //                    where classSubject.ClassId == classId
        //                    select new { subject.Id, subject.Name }).ToList();
        //    return Json(Subjects, JsonRequestBehavior.AllowGet);
        //}
        // GET: AspNetHomeWorks/Details/5

        public JsonResult SubjectByTeahcer(int classid)
        {
            if (classid != 0)
            {
                List<Class_Subject> ClassSubject = new List<Class_Subject>();
                var tid = User.Identity.GetUserId();
                // var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
                var student = db.AspNetStudents.Where(x => x.UserId == tid).FirstOrDefault();

                var enrolmentlist = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student.Id).ToList();
                foreach (var item in enrolmentlist)
                {
                    var classcourse = db.AspNetClass_Courses.Where(x => x.Id == item.CourseId && x.ClassId == classid).FirstOrDefault();
                    Class_Subject cs = new Class_Subject();
                    cs.Id = classcourse.CourseId;
                    cs.Class = classcourse.AspNetClass.Name;
                    cs.Subject = classcourse.AspNetCours.Name;
                    ClassSubject.Add(cs);
                }
                return Json(ClassSubject, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Student")]
        public ActionResult Diary()
        {


            return View();
        }
        public ActionResult GetStudentDiaryList()
        {
            var UserId = User.Identity.GetUserId();
            var GetUser = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
            var StudentId = db.AspNetStudents.Where(x => x.UserId == GetUser.Id).FirstOrDefault().Id;

            var StudentHomeworkList = (from homeWork in db.AspNetHomeWorks
                                       join studentHomeWork in db.AspNetStudentHomeWorks on homeWork.Id equals studentHomeWork.HomeWorkId
                                       where studentHomeWork.StudentId == StudentId
                                       select new { homeWork.Id, TeacherName = homeWork.AspNetEmployee.Name, homeWork.Date, studentHomeWork.TeacherComment }).ToList();


            return Json(StudentHomeworkList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DiaryDetails(int id)
        {
            var UserId = User.Identity.GetUserId();
            var GetUser = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
            var StudentId = db.AspNetStudents.Where(x => x.UserId == GetUser.Id).FirstOrDefault().Id;
            var TeacherComments = "";
                
            ViewBag.HomeWorkId = id;

           AspNetHomeWork HomeWork =  db.AspNetHomeWorks.Where(x => x.Id == id).FirstOrDefault();

          var StudentHomeWork =   db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == id && x.StudentId == StudentId).FirstOrDefault();

            if(StudentHomeWork != null)
            {

                TeacherComments = StudentHomeWork.TeacherComment;
            }

            ViewBag.TeacherComments = TeacherComments;



            return View(HomeWork);
        }
        public class Section_Subject
        {
            public int Id { get; set; }
            public string Section { get; set; }
            public string Subject { get; set; }

        }
        public ActionResult SubjectsOfTeacherByHomeWorkId(int HomeWorkId)
        {
            List<Section_Subject> SectionSubjects = new List<Section_Subject>();

            var HomeWork = db.AspNetHomeWorks.Where(x => x.Id == HomeWorkId).FirstOrDefault();
            var TeacherId = HomeWork.TeacherId;
            var SectionId = HomeWork.AspNetBranchClass_Sections.AspNetSection.Id;

            var ID = User.Identity.GetUserId();
            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId

                            where (branchclasssubject.SectionId == SectionId && enrollment.TeacherId == TeacherId)
                            select new
                            {
                                subject.Id,
                                subject.Name,
                            }).Distinct();


            foreach (var item in Subjects)
            {
                // var classcourse = db.AspNetClass_Courses.Where(x => x.Id == item.CourseId && x.ClassId == classid).FirstOrDefault();
                //  cs.Class = classcourse.AspNetClass.Name;
                Section_Subject cs = new Section_Subject();
                cs.Id = item.Id;
                cs.Subject = item.Name;
                SectionSubjects.Add(cs);
            }


            return Json(SectionSubjects, JsonRequestBehavior.AllowGet);

        }


    public class Subject_Homework
    {
        public int SubjectID { get; set; }
        public string HomeworkDetail { get; set; }
    }
    public class Homework
    {
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public string TeacherComment { get; set; }
        public string Reading { get; set; }
        public List<Subject_Homework> subject_Homework { get; set; }

    }
    public JsonResult AddDiary(Homework aspNetHomework)
    {
        AspNetHomeWork aspNetHomeworks = new AspNetHomeWork();
        aspNetHomeworks.ClassId = aspNetHomework.ClassId;
        aspNetHomeworks.Date = aspNetHomework.Date;
        aspNetHomeworks.Principal_Approved_Status = "Approved";
        db.AspNetHomeWorks.Add(aspNetHomeworks);
        db.SaveChanges();

        var HomeWorkID = db.AspNetHomeWorks.Max(x => x.Id);
        foreach (var subject in aspNetHomework.subject_Homework)
        {
            AspNetSubjectHomeWork aspNetSubject_Homework = new AspNetSubjectHomeWork();
            aspNetSubject_Homework.HomeWorkId = HomeWorkID;
            aspNetSubject_Homework.SubjectId = subject.SubjectID;
            aspNetSubject_Homework.HomeWorkDetail = subject.HomeworkDetail;
            db.AspNetSubjectHomeWorks.Add(aspNetSubject_Homework);
            db.SaveChanges();
        }
        List<string> ParentList = new List<string>();
        var StudentList = db.AspNetStudents.Where(x => x.ClassId == aspNetHomework.ClassId).Select(x => x.Id).ToList();
        foreach (var student in StudentList)
        {
            AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
            aspNetStudent_Homework.HomeWorkId = HomeWorkID;
            aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
            aspNetStudent_Homework.Status = "Not Seen by Parents";
            aspNetStudent_Homework.StudentId = student;
            aspNetStudent_Homework.TeacherComment = "";
            db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
            db.SaveChanges();
        }
        return Json("", JsonRequestBehavior.AllowGet);
    }

    public class Class_Subject
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }

    }
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AspNetHomeWork aspNetHomeWork = db.AspNetHomeWorks.Find(id);
        if (aspNetHomeWork == null)
        {
            return HttpNotFound();
        }
        return View(aspNetHomeWork);
    }

    // GET: AspNetHomeWorks/Create
    public ActionResult Create()
    {
        //ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
        var id = User.Identity.GetUserId();
        // var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();
        var student = db.AspNetStudents.Where(x => x.UserId == id).FirstOrDefault();

        ViewBag.ClassId = new SelectList(from cls in db.AspNetClasses
                                         join clascours in db.AspNetClass_Courses on cls.Id equals clascours.ClassId
                                         join te in db.AspNetStudent_Enrollments on clascours.Id equals te.CourseId
                                         where te.StudentId == student.Id
                                         select new { cls.Id, cls.Name }, "Id", "Name").ToList();
        return View();
    }

    public JsonResult Upload()
    {
        for (int i = 0; i < Request.Files.Count; i++)
        {
            HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                        //Use the following properties to get file's name, size and MIMEType
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string mimeType = file.ContentType;
            System.IO.Stream fileContent = file.InputStream;
            //To save file, use SaveAs method
            file.SaveAs(Server.MapPath("~/Content/Homework") + fileName); //File will be saved in application root

            var id = db.AspNetHomeWorks.OrderByDescending(u => u.Id).Select(x => x.Id).FirstOrDefault();
            var homework = db.AspNetHomeWorks.Where(x => x.Id == id).FirstOrDefault();
            homework.Attachment = fileName;
            db.SaveChanges();
        }
        return Json("Uploaded " + Request.Files.Count + " files");
    }

    // POST: AspNetHomeWorks/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ClassId,Date,Principal_Approved_Status")] AspNetHomeWork aspNetHomeWork)
    {
        if (ModelState.IsValid)
        {
            db.AspNetHomeWorks.Add(aspNetHomeWork);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetHomeWork.ClassId);
        return View(aspNetHomeWork);
    }

    // GET: AspNetHomeWorks/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AspNetHomeWork aspNetHomeWork = db.AspNetHomeWorks.Find(id);
        if (aspNetHomeWork == null)
        {
            return HttpNotFound();
        }
        ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetHomeWork.ClassId);
        return View(aspNetHomeWork);
    }
    public ActionResult Edit2(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AspNetSubjectHomeWork aspNetSubject_Homework = db.AspNetSubjectHomeWorks.Find(id);
        if (aspNetSubject_Homework == null)
        {
            return HttpNotFound();
        }
        ViewBag.HomeworkID = new SelectList(db.AspNetHomeWorks, "Id", "Id", aspNetSubject_Homework.HomeWorkId);
        ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName", aspNetSubject_Homework.AspNetCours.Name);
        return View(aspNetSubject_Homework);
    }

    [HttpPost]
    public ActionResult Edit2([Bind(Include = "Id,SubjectID,HomeworkDetail,HomeworkID")] AspNetSubjectHomeWork aspNetSubject_Homework)
    {
        if (ModelState.IsValid)
        {
            db.Entry(aspNetSubject_Homework).State = EntityState.Modified;
            db.SaveChanges();
            var obj = db.AspNetHomeWorks.Find(aspNetSubject_Homework.HomeWorkId);
            obj.Principal_Approved_Status = "Created";
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { HomeWorkId = aspNetSubject_Homework.HomeWorkId });
        }
        ViewBag.HomeworkID = new SelectList(db.AspNetHomeWorks, "Id", "Id", aspNetSubject_Homework.HomeWorkId);
        ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName", aspNetSubject_Homework.SubjectId);
        return View(aspNetSubject_Homework);
    }


    public ActionResult DairyDetail(int? id)
    {
        ViewBag.HomeworkID = db.AspNetHomeWorks.Where(x => x.Id == id).FirstOrDefault().Principal_Approved_Status;

        var aspNetSubject_Homework = db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == id).Include(a => a.AspNetCours);
        var aspNetSomework = db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == id).FirstOrDefault();
        return View(aspNetSubject_Homework.ToList());
    }


    // POST: AspNetHomeWorks/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ClassId,Date,Principal_Approved_Status")] AspNetHomeWork aspNetHomeWork)
    {
        if (ModelState.IsValid)
        {
            db.Entry(aspNetHomeWork).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetHomeWork.ClassId);
        return View(aspNetHomeWork);
    }

    // GET: AspNetHomeWorks/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        AspNetHomeWork aspNetHomeWork = db.AspNetHomeWorks.Find(id);
        if (aspNetHomeWork == null)
        {
            return HttpNotFound();
        }
        return View(aspNetHomeWork);
    }

    // POST: AspNetHomeWorks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        AspNetHomeWork aspNetHomeWork = db.AspNetHomeWorks.Find(id);
        db.AspNetHomeWorks.Remove(aspNetHomeWork);
        db.SaveChanges();
        return RedirectToAction("Index");
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