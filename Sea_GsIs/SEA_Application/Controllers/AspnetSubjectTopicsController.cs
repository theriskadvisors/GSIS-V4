using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
//using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    //[Authorize(Roles = "Branch_Admin")]
    public class AspnetSubjectTopicsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();


        // GET: AspnetSubjectTopics
        public ActionResult Index()
        {
            //     var aspnetSubjectTopicsTesting = db.AspnetSubjectTopics.Include(a => a.GenericSubject));
            var UserId = User.Identity.GetUserId();
            int id = db.AspNetEmployees.Where(x => x.UserId == UserId).FirstOrDefault().Id;
            var aspnetSubjectTopics = db.AspnetSubjectTopics.Include(a => a.GenericSubject.Teacher_GenericSubjects.Where(x => x.TeacherId == id));
            // var aspnetSubjectTopics = db.AspnetSubjectTopics.Include(a => a.GenericSubject);
            return View(aspnetSubjectTopics.ToList());
            //  return View();
        }

        public ActionResult ViewTopicsAndLessons(string NavigateTo = "Topic")
        {
            //var aspnetSubjectTopics = db.AspnetSubjectTopics.Include(a => a.GenericSubject);
            //return View(aspnetSubjectTopics.ToList());
            //var aspnetSubjectTopics = db.AspnetSubjectTopics.Include(a => a.AspnetGenericBranchClassSubject.AspNetBranch).Include(a => a.AspnetGenericBranchClassSubject.AspNetBranch).Include(a => a.AspnetGenericBranchClassSubject.AspNetClass);

            //var UserId = User.Identity.GetUserId();
            //int id = db.AspNetEmployees.Where(x => x.UserId == UserId).FirstOrDefault().Id;
            //var aspnetSubjectTopics = db.AspnetSubjectTopics.Include(a => a.GenericSubject.Teacher_GenericSubjects.Where(x => x.TeacherId == id));
            
            ViewBag.NavigateTo = NavigateTo;
            return View(); //aspnetSubjectTopics.ToList()
        }

        public ActionResult AllLessonsList()
        {
            var ID = User.Identity.GetUserId();

            var AllLessons = (from lesson in db.AspnetLessons
                              join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                              where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                              && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                              && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                              select new
                              {
                                  LessonId = lesson.Id,
                                  LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                  LessonName = lesson.Name,
                                  LessonVidoeUrl = lesson.Video_Url,
                                  LessonDuration = lesson.DurationMinutes,
                                  LessonDescription = lesson.Description,
                                  LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                  LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                  LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                  LessonStatus = lesson.Status,
                                  LessonDate = lesson.CreationDate,
                                //  LessonStartDate = DbFunctions.TruncateTime(lesson.StartDate).ToString()
                                  LessonStartDate = lesson.StartDate


                           }).OrderByDescending(x=>x.LessonDate).Distinct().ToList();


            return Json(AllLessons, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllSubjectTopicList()
        {
            var ID = User.Identity.GetUserId();

            var AllSubjectTopicList = (from subjectTopic in db.AspnetSubjectTopics
                                       join enrollment in db.AspNetTeacher_Enrollments on subjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                                       where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == subjectTopic.AspnetGenericBranchClassSubject.ClassId
                                       && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == subjectTopic.AspnetGenericBranchClassSubject.SectionId 
                                       && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == subjectTopic.AspnetGenericBranchClassSubject.BranchId
                                       select new
                                       {
                                           Id = subjectTopic.Id,
                                           TopicName = subjectTopic.Name,
                                           StartDate = subjectTopic.StartDate,
                                           BranchName = subjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Name,
                                           ClassName = subjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                           SubjectName = subjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                           SectionName = subjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                           Description = subjectTopic.Description,
                                       }).Distinct().ToList();


            //var AllSubjectTopicList = (from subjectTopic in db.AspnetSubjectTopics
            //                               // join enrollment in db.AspNetTeacher_Enrollments on subjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
            //                               // where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == subjectTopic.AspnetGenericBranchClassSubject.ClassId
            //                           select new
            //                           {
            //                               Id = subjectTopic.Id,
            //                               TopicName = subjectTopic.Name,
            //                               StartDate = subjectTopic.StartDate,
            //                               BranchName = subjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Name,
            //                               ClassName = subjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
            //                               SubjectName = subjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
            //                               Description = subjectTopic.Description,
            //                           }).ToList();



            return Json(AllSubjectTopicList, JsonRequestBehavior.AllowGet);
        }


        // GET: AspnetSubjectTopics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);
            if (aspnetSubjectTopic == null)
            {
                return HttpNotFound();
            }
            return View(aspnetSubjectTopic);
        }

        // GET: AspnetSubjectTopics/Create
        public ActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(db.GenericSubjects, "Id", "SubjectName");

            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");



            return View();
        }



        // POST: AspnetSubjectTopics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,SubjectId,FAQ,OrderBy")] AspnetSubjectTopic aspnetSubjectTopic)
        {
            aspnetSubjectTopic.StartDate = null;
            aspnetSubjectTopic.EndDate = null;

            var BranchId = Convert.ToInt32(Request.Form["BranchId"]);
            var ClassId = Convert.ToInt32(Request.Form["ClassId"]);
            var SubjectId = Convert.ToInt32(Request.Form["SubjectId"]);

            var SectionId = Convert.ToInt32(Request.Form["SectionId"]);

            var obj = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SubjectId == SubjectId && x.SectionId == SectionId).FirstOrDefault();

            if (obj != null)
            {
                aspnetSubjectTopic.GenericBranchClassSubjectId = obj.Id;

            }

            if (ModelState.IsValid)
            {
                aspnetSubjectTopic.SubjectId = null;

                db.AspnetSubjectTopics.Add(aspnetSubjectTopic);
                db.SaveChanges();
                TempData["TopicCreated"] = "Created";
                return RedirectToAction("ViewTopicsAndLessons");
            }

            var UserId = User.Identity.GetUserId();
            //Teacher Subjects 
            var SubjectofCurrentSessionTeacher = from subject in db.GenericSubjects
                                                 join TeacherSubject in db.Teacher_GenericSubjects on subject.Id equals TeacherSubject.SubjectId
                                                 join employee in db.AspNetEmployees on TeacherSubject.TeacherId equals employee.Id
                                                 where employee.UserId == UserId
                                                 select new
                                                 {
                                                     subject.Id,
                                                     subject.SubjectName,
                                                 };

            ViewBag.SubjectId = new SelectList(SubjectofCurrentSessionTeacher, "Id", "SubjectName", aspnetSubjectTopic.SubjectId);
            return View(aspnetSubjectTopic);
        }


        public ActionResult CheckTopicOrderBy(string SubjectId, string OrderBy)
        {
            int SubId = Convert.ToInt32(SubjectId);
            int OrderByValue = Convert.ToInt32(OrderBy);

            var TopicExist = "";
            AspnetSubjectTopic subjectTopic = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubId && x.OrderBy == OrderByValue).FirstOrDefault();

            if (subjectTopic == null)
            {
                TopicExist = "No";
            }
            else
            {
                TopicExist = "Yes";
            }


            return Json(TopicExist, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult StudentByClass(int id)
        //{
        //    var result1 = db.AspNetSubjects.Where(x => x.ClassID == id).ToList();
        //    var obj = JsonConvert.SerializeObject(result1);
        //       return Json(obj, JsonRequestBehavior.AllowGet);

        //}


        // GET: AspnetSubjectTopics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);
            if (aspnetSubjectTopic == null)
            {
                return HttpNotFound();

            }
            
            int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == id).FirstOrDefault().GenericBranchClassSubjectId;

            //    ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName", Subject.ClassID);
            //     ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x=>x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            // ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x => x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();

            var ID = User.Identity.GetUserId();

            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetEmployee.BranchId
                            where enrollment.AspNetEmployee.UserId == ID
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();



            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                           where (branchclasssubject.BranchId == GenericObject.BranchId && enrollment.AspNetEmployee.UserId == ID)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();


            var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId).Select(x => new
            {
                Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                Name = x.AspNetBranchClass_Sections.AspNetSection.Name
            }).Distinct();
            
            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                            where (branchclasssubject.SectionId == GenericObject.SectionId && enrollment.AspNetEmployee.UserId == ID)
                            select new
                            {
                                subject.Id,
                                subject.Name,
                            }).Distinct();

            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
            ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
            ViewBag.SubjectId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);


            ViewBag.OrderBy = aspnetSubjectTopic.OrderBy;

            var count = db.AspnetLessons.Where(x => x.TopicId == id).Select(x => x.Id).ToList().Count;

            ViewBag.LessonCount = count;
            return View(aspnetSubjectTopic);
        }

        // POST: AspnetSubjectTopics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,SubjectId,FAQ,OrderBy")] AspnetSubjectTopic aspnetSubjectTopic)
        {

            var BranchId = Convert.ToInt32(Request.Form["BranchId"]);
            var ClassId = Convert.ToInt32(Request.Form["ClassId"]);
            var SubjectId = Convert.ToInt32(Request.Form["SubjectId"]);

            var SectionId = Convert.ToInt32(Request.Form["SectionId"]);

            var obj = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SubjectId == SubjectId && x.SectionId == SectionId).FirstOrDefault();

            if (obj != null)
            {
                aspnetSubjectTopic.GenericBranchClassSubjectId = obj.Id;

            }

            if (ModelState.IsValid)
            {

                aspnetSubjectTopic.SubjectId = null;
                db.Entry(aspnetSubjectTopic).State = EntityState.Modified;
                db.SaveChanges();

                TempData["TopicUpdated"] = "Updated";
                return RedirectToAction("ViewTopicsAndLessons");
            }

            // ViewBag.SubjectId = new SelectList(db.GenericSubjects, "Id", "SubjectName", aspnetSubjectTopic.SubjectId);
            return View(aspnetSubjectTopic);
        }

        // GET: AspnetSubjectTopics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);
            if (aspnetSubjectTopic == null)
            {
                return HttpNotFound();
            }
            return View(aspnetSubjectTopic);
        }

        // POST: AspnetSubjectTopics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);
            db.AspnetSubjectTopics.Remove(aspnetSubjectTopic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult DeleteTopic(int? id)
        {
            List<int> LessonIds = db.AspnetLessons.Where(x => x.TopicId == id).Select(x => x.Id).ToList();

            List<Quiz_Topic_Questions> QuizTopicQuestionnsToDelete = db.Quiz_Topic_Questions.Where(x => x.TopicId == id).ToList();
            db.Quiz_Topic_Questions.RemoveRange(QuizTopicQuestionnsToDelete);
            db.SaveChanges();


            foreach (var LessonId in LessonIds)
            {


                IEnumerable<AspnetComment> CommentsToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).SelectMany(x => x.AspnetComments);


                db.AspnetComments.RemoveRange(CommentsToDelete);
                db.SaveChanges();

                List<AspnetComment_Head> ListCommentHeadToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).ToList();

                db.AspnetComment_Head.RemoveRange(ListCommentHeadToDelete);
                db.SaveChanges();

                var AssignmentToDelete = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonId).FirstOrDefault();
                if (AssignmentToDelete != null)
                {

                    db.AspnetStudentAssignments.Remove(AssignmentToDelete);
                    db.SaveChanges();
                }

                List<AspnetStudentAttachment> StudentAttachmentListToDelete = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonId).ToList();
                db.AspnetStudentAttachments.RemoveRange(StudentAttachmentListToDelete);
                db.SaveChanges();


                List<AspnetStudentLink> StudentLinkListToDelete = db.AspnetStudentLinks.Where(x => x.LessonId == LessonId).ToList();

                db.AspnetStudentLinks.RemoveRange(StudentLinkListToDelete);
                db.SaveChanges();


                List<Event> AllEvents =  db.Events.Where(x => x.LessonID == LessonId).ToList();
                db.Events.RemoveRange(AllEvents);
                db.SaveChanges();



                List<AspnetStudentAssignmentSubmission> StudentAssignmentSubmissionListToDelete = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonId).ToList();

                db.AspnetStudentAssignmentSubmissions.RemoveRange(StudentAssignmentSubmissionListToDelete);
                db.SaveChanges();



                List<StudentLessonTracking> StudentLessonTrackingListToDelete = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId).ToList();

                db.StudentLessonTrackings.RemoveRange(StudentLessonTrackingListToDelete);

                db.SaveChanges();


                List<Student_Quiz_Scoring> StudentQuizScoringToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).SelectMany(x => x.Student_Quiz_Scoring).ToList();

                db.Student_Quiz_Scoring.RemoveRange(StudentQuizScoringToDelete);
                db.SaveChanges();


                List<Lesson_Session> LessonSessionToDelete = db.Lesson_Session.Where(x => x.LessonId == LessonId).ToList();
                db.Lesson_Session.RemoveRange(LessonSessionToDelete);
                db.SaveChanges();

                List<AspnetQuestion> QuestionListToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).ToList();

                db.AspnetQuestions.RemoveRange(QuestionListToDelete);
                db.SaveChanges();

                AspnetLesson LessonToDelete = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault();
                if (LessonToDelete != null)
                {

                    db.AspnetLessons.Remove(LessonToDelete);
                    db.SaveChanges();


                }

            }

            AspnetSubjectTopic SubjectTopicToDelete = db.AspnetSubjectTopics.Where(x => x.Id == id).FirstOrDefault();

            db.AspnetSubjectTopics.Remove(SubjectTopicToDelete);
            db.SaveChanges();

            TempData["TopicDeleted"] = "Deleted";

            return RedirectToAction("ViewTopicsAndLessons");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public JsonResult GetSubjectsByClass(int ClassID)
        {
            var SubjectsByClass = db.AspNetSubjects.Where(x => x.ClassID == ClassID).ToList();
            //string status = Newtonsoft.Json.JsonConvert.SerializeObject(SubjectsByClass);

            return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            //    return Content(status);

        }

    }
}
