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
    public class
        AspNetHomeWorksController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetHomeWorks
        public ActionResult Index()
        {
            var uid = User.Identity.GetUserId();
            var tid = db.AspNetEmployees.Where(x => x.UserId == uid).Select(x => x.Id).FirstOrDefault();
            var aspNetHomeWorks = db.AspNetHomeWorks.Include(a => a.AspNetClass);

            var result = (from cls in db.AspNetClasses
                          join classcourse in db.AspNetClass_Courses on cls.Id equals classcourse.ClassId
                          join enrolment in db.AspNetTeacher_Enrollments on classcourse.Id equals enrolment.CourseId
                          where enrolment.TeacherId == tid
                          select new { cls.Id, cls.Name }).Distinct().ToList();


            ViewBag.ClassId = new SelectList(result, "Id", "Name").ToList();



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

        public JsonResult DiaryList()
        {
            var id = User.Identity.GetUserId();
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();

            var Diary = (from diary in db.AspNetHomeWorks
                         join enrollment in db.AspNetTeacher_Enrollments on diary.AspNetBranchClass_Sections.AspNetSection.Id equals enrollment.AspNetBranchClass_Sections.AspNetSection.Id
                         where enrollment.TeacherId == teacherid && diary.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId
                         && diary.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
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

        public JsonResult SubjectByTeahcer(int SectionId)
        {
            if (SectionId != 0)
            {
                List<Section_Subject> SectionSubjects = new List<Section_Subject>();
                var tid = User.Identity.GetUserId();
                var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
                var enrolmentlist = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).GroupBy(x => x.CourseId).Select(x => new { CourseId = x.Key }).ToList();


                var ID = User.Identity.GetUserId();
                var Subjects = (from subject in db.AspNetCourses
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                                where (branchclasssubject.SectionId == SectionId && enrollment.AspNetEmployee.UserId == ID)
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
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadDiaryAttachment(String Name)
        {
            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/Homework/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }

        public class Subject_Homework
        {
            public int SubjectID { get; set; }
            public string HomeworkDetail { get; set; }
        }
        public class Homework
        {
            public int HomeworkId { get; set; }
            public int BranchId { get; set; }
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public DateTime Date { get; set; }
            public string TeacherComment { get; set; }
            public string Reading { get; set; }

            public string Attachment { get; set; }

            public List<int> StudentsList { get; set; }
            public List<Subject_Homework> subject_Homework { get; set; } = new List<Subject_Homework>();

        }
        public JsonResult AddDiary(Homework aspNetHomework)
        {
            var uid = User.Identity.GetUserId();
            var tid = db.AspNetEmployees.Where(x => x.UserId == uid).Select(x => x.Id).FirstOrDefault();

            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == aspNetHomework.BranchId && x.ClassId == aspNetHomework.ClassId).FirstOrDefault().Id;
            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == aspNetHomework.SectionId).FirstOrDefault().Id;
            
            AspNetHomeWork aspNetHomeworks = new AspNetHomeWork();
            aspNetHomeworks.ClassId = aspNetHomework.ClassId;
            aspNetHomeworks.SectionId = BranchClassSectionId;
            aspNetHomeworks.Date = aspNetHomework.Date;
            aspNetHomeworks.TeacherId = tid;
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
            // var StudentList = db.AspNetStudents.Where(x => x.ClassId == aspNetHomework.ClassId).Select(x => x.Id).ToList();


            var AllStudents = (from enrollment in db.AspNetStudent_Enrollments.Where(x => x.SectionId == BranchClassSectionId)
                               join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                               select new
                               {
                                   student.Id,
                               }).Distinct().ToList();


            List<int> StudentIds = AllStudents.Select(x => x.Id).ToList();

            List<int> IdsToRemove = new List<int>();


            if (aspNetHomework.StudentsList != null)
            {

                foreach (var item in aspNetHomework.StudentsList)
                {

                    foreach (var item1 in StudentIds)
                    {
                        if (item == item1)
                        {
                            IdsToRemove.Add(item);
                        }
                    }

                }
            }


            StudentIds.RemoveAll(x => IdsToRemove.Contains(x));

            if (aspNetHomework.StudentsList != null)
            {
                //  aspNetHomework.StudentsList.Remove(0);

                if (aspNetHomework.StudentsList.Count() != 0)

                {

                    foreach (var student in StudentIds)
                    {
                        AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                        aspNetStudent_Homework.HomeWorkId = HomeWorkID;
                        aspNetStudent_Homework.Status = "Not Seen by Parents";
                        aspNetStudent_Homework.StudentId = student;
                        aspNetStudent_Homework.TeacherComment = null;
                        db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                        db.SaveChanges();
                    }

                    foreach (var student in aspNetHomework.StudentsList)
                    {
                        AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                        aspNetStudent_Homework.HomeWorkId = HomeWorkID;
                        aspNetStudent_Homework.Status = "Not Seen by Parents";
                        aspNetStudent_Homework.StudentId = student;
                        aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
                        db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                        db.SaveChanges();
                    }
                }
            }
            else

            {
                foreach (var student in StudentIds)
                {
                    AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                    aspNetStudent_Homework.HomeWorkId = HomeWorkID;
                    aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
                    aspNetStudent_Homework.Status = "Not Seen by Parents";
                    aspNetStudent_Homework.StudentId = student;
                    aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
                    db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                    db.SaveChanges();
                }

            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

         public ActionResult UpdateDiary(Homework aspNetHomework)
         {
            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == aspNetHomework.BranchId && x.ClassId == aspNetHomework.ClassId).FirstOrDefault().Id;

            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == aspNetHomework.SectionId).FirstOrDefault().Id;


            AspNetHomeWork HomeWorkToUpdate =    db.AspNetHomeWorks.Where(x => x.Id == aspNetHomework.HomeworkId).FirstOrDefault();
            HomeWorkToUpdate.Id = aspNetHomework.HomeworkId;
            HomeWorkToUpdate.Date = aspNetHomework.Date;
            HomeWorkToUpdate.SectionId = BranchClassSectionId;
            HomeWorkToUpdate.ClassId = aspNetHomework.ClassId;
            db.SaveChanges();

            List<AspNetSubjectHomeWork> SubjectHomeworkToRemove= db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == aspNetHomework.HomeworkId).ToList();
            db.AspNetSubjectHomeWorks.RemoveRange(SubjectHomeworkToRemove);
            db.SaveChanges();

            List<AspNetStudentHomeWork> StudentHomeWorkToRemove = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == aspNetHomework.HomeworkId).ToList();
            db.AspNetStudentHomeWorks.RemoveRange(StudentHomeWorkToRemove);
            db.SaveChanges();


            foreach (var subject in aspNetHomework.subject_Homework)
            {
                AspNetSubjectHomeWork aspNetSubject_Homework = new AspNetSubjectHomeWork();
                aspNetSubject_Homework.HomeWorkId = aspNetHomework.HomeworkId;
                aspNetSubject_Homework.SubjectId = subject.SubjectID;
                aspNetSubject_Homework.HomeWorkDetail = subject.HomeworkDetail;
                db.AspNetSubjectHomeWorks.Add(aspNetSubject_Homework);
                db.SaveChanges();
            }

            var AllStudents = (from enrollment in db.AspNetStudent_Enrollments.Where(x => x.SectionId == aspNetHomework.SectionId)
                               join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                               select new
                               {
                                   student.Id,
                               }).Distinct().ToList();

            List<int> StudentIds = AllStudents.Select(x => x.Id).ToList();

            List<int> IdsToRemove = new List<int>();

            if (aspNetHomework.StudentsList != null)
            {

                foreach (var item in aspNetHomework.StudentsList)
                {

                    foreach (var item1 in StudentIds)
                    {
                        if (item == item1)
                        {
                            IdsToRemove.Add(item);
                        }
                    }

                }
            }


            StudentIds.RemoveAll(x => IdsToRemove.Contains(x));

            if (aspNetHomework.StudentsList != null)
            {
                //  aspNetHomework.StudentsList.Remove(0);

                if (aspNetHomework.StudentsList.Count() != 0)

                {

                    foreach (var student in StudentIds)
                    {
                        AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                        aspNetStudent_Homework.HomeWorkId = aspNetHomework.HomeworkId;
                        aspNetStudent_Homework.Status = "Not Seen by Parents";
                        aspNetStudent_Homework.StudentId = student;
                        aspNetStudent_Homework.TeacherComment = null;
                        db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                        db.SaveChanges();
                    }

                    foreach (var student in aspNetHomework.StudentsList)
                    {
                        AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                        aspNetStudent_Homework.HomeWorkId = aspNetHomework.HomeworkId;
                        aspNetStudent_Homework.Status = "Not Seen by Parents";
                        aspNetStudent_Homework.StudentId = student;
                        aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
                        db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                        db.SaveChanges();
                    }
                }
            }
            else

            {
                foreach (var student in StudentIds)
                {
                    AspNetStudentHomeWork aspNetStudent_Homework = new AspNetStudentHomeWork();
                    aspNetStudent_Homework.HomeWorkId = aspNetHomework.HomeworkId;
                    aspNetStudent_Homework.TeacherComment = aspNetHomework.TeacherComment;
                    aspNetStudent_Homework.Status = "Not Seen by Parents";
                    aspNetStudent_Homework.StudentId = student;
                    aspNetStudent_Homework.TeacherComment = "";
                    db.AspNetStudentHomeWorks.Add(aspNetStudent_Homework);
                    db.SaveChanges();
                }

            }



            return Json("", JsonRequestBehavior.AllowGet);
            }

        public class Section_Subject
        {
            public int Id { get; set; }
            public string Section { get; set; }
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
            var teacherid = db.AspNetEmployees.Where(x => x.UserId == id).Select(x => x.Id).FirstOrDefault();

            var result = (from cls in db.AspNetClasses
                          join classcourse in db.AspNetClass_Courses on cls.Id equals classcourse.ClassId
                          join enrolment in db.AspNetTeacher_Enrollments on classcourse.Id equals enrolment.CourseId
                          where enrolment.TeacherId == teacherid
                          select new { cls.Id, cls.Name }).Distinct().ToList();


            ViewBag.ClassId = new SelectList(result, "Id", "Name").ToList();



            return View();
        }
      
        public JsonResult Upload()
        {
            var id = db.AspNetHomeWorks.OrderByDescending(u => u.Id).Select(x => x.Id).FirstOrDefault();

            var AllFiles = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;

                var name = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);

                fileName = name + "_Diary_" + id + ext;
                AllFiles += fileName + "/";

                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                // file.SaveAs(Server.MapPath("~/Content/Homework") + fileName); //File will be saved in application root
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Homework"), fileName));
            }

            if (AllFiles != "")
            {
                var homework = db.AspNetHomeWorks.Where(x => x.Id == id).FirstOrDefault();
                homework.Attachment = AllFiles;
                db.SaveChanges();
            }

            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public JsonResult EditUploadDiary(int HomeworkId)
        {
           // var id = db.AspNetHomeWork..Select(x => x.Id).FirstOrDefault();

            var AllFiles = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;

                var name = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);

                fileName = name + "_Diary_" + HomeworkId + ext;
                AllFiles += fileName + "/";

                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                // file.SaveAs(Server.MapPath("~/Content/Homework") + fileName); //File will be saved in application root
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Homework"), fileName));
            }

            if (AllFiles != "")
            {
                var homework = db.AspNetHomeWorks.Where(x => x.Id == HomeworkId).FirstOrDefault();
                homework.Attachment = AllFiles;
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
            else
            {
                Homework HomeWorkObj = new Homework();

                HomeWorkObj.Date = aspNetHomeWork.Date.Value;


                var BranchClassSection = db.AspNetBranchClass_Sections.Where(x => x.Id == aspNetHomeWork.SectionId).FirstOrDefault();
                var BranchClass = db.AspNetBranch_Class.Where(x => x.Id == BranchClassSection.BranchClassId).FirstOrDefault();
                HomeWorkObj.HomeworkId = aspNetHomeWork.Id;
                HomeWorkObj.BranchId = BranchClass.BranchId;
                HomeWorkObj.ClassId = BranchClass.ClassId;
                HomeWorkObj.SectionId = BranchClassSection.SectionId;

                var TeacherComments = "";
                var StudentHomeWork = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == aspNetHomeWork.Id && x.TeacherComment != null).FirstOrDefault();
                var SubjectHomeWorkList = db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == aspNetHomeWork.Id).ToList();

                if (StudentHomeWork != null)
                {
                    TeacherComments = StudentHomeWork.TeacherComment;
                }

                HomeWorkObj.TeacherComment = TeacherComments;

                List<Subject_Homework> SubjectHomeWorkListItems = new List<Subject_Homework>();

                foreach (var SubjectHomework in SubjectHomeWorkList)
                {
                    Subject_Homework SH = new Subject_Homework();

                    SH.SubjectID = SubjectHomework.SubjectId.Value;
                    SH.HomeworkDetail = SubjectHomework.HomeWorkDetail;

                    SubjectHomeWorkListItems.Add(SH);
                }

                HomeWorkObj.subject_Homework.AddRange(SubjectHomeWorkListItems);
                HomeWorkObj.Attachment = aspNetHomeWork.Attachment;

                return View(HomeWorkObj);
            }


            // ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name", aspNetHomeWork.ClassId);


            return View();
        }
        public ActionResult GetHomeworkDetails(int HomeWorkId)
        {
            var HomeWorkList = db.AspNetSubjectHomeWorks.Where(x => x.HomeWorkId == HomeWorkId).ToList();

            List<Subject_Homework> SubjectHomeWorksList = new List<Subject_Homework>();

            foreach (var Subject in HomeWorkList)
            {
                Subject_Homework SubjectHomeWork = new Subject_Homework();

                SubjectHomeWork.SubjectID = Subject.SubjectId.Value;
                SubjectHomeWork.HomeworkDetail = Subject.HomeWorkDetail;

                SubjectHomeWorksList.Add(SubjectHomeWork);
            }

            return Json(SubjectHomeWorksList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSelectedStudentList(int HomeWorkId)
        {

            var SelectStudentsIds = db.AspNetStudentHomeWorks.Where(x => x.HomeWorkId == HomeWorkId && x.TeacherComment != null && x.TeacherComment != "").Select(x => new { x.StudentId }).ToList();

            return Json(SelectStudentsIds, JsonRequestBehavior.AllowGet);
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
