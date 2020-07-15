using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    public class AspnetQuizsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspnetQuizs
        public ActionResult Index()
        {
            return View(db.AspnetQuizs.ToList());
        }

        // GET: AspnetQuizs/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetQuiz aspnetQuiz = db.AspnetQuizs.Find(id);
            if (aspnetQuiz == null)
            {
                return HttpNotFound();
            }

            var AllTopicIDs = db.Quiz_Topic_Questions.Where(x => x.QuizId == id).Select(x => x.TopicId).ToList();


            var AllTopic = from topic in db.AspnetSubjectTopics
                           where AllTopicIDs.Contains(topic.Id)
                           select topic;
            ViewBag.TopicId = new SelectList(AllTopic, "Id", "Name");


            //All Questions To Display

            var AllQuestionIDS = db.Quiz_Topic_Questions.Where(x => x.QuizId == id).Select(x => x.QuestionId).ToList();

            var AllQuestion = from Question in db.AspnetQuestions
                              where AllQuestionIDS.Contains(Question.Id)
                              select Question;
            ViewBag.QuestionID = new SelectList(AllQuestion, "Id", "Name");

            DateTime Date = Convert.ToDateTime(aspnetQuiz.Start_Date);
            string StartDate = Date.ToString("yyyy-MM-dd");
            ViewBag.StartDate = StartDate;

            DateTime Date1 = Convert.ToDateTime(aspnetQuiz.Due_Date);
            string DueDate = Date1.ToString("yyyy-MM-dd");
            ViewBag.DueDate = DueDate;


            return View(aspnetQuiz);
        }

        // GET: AspnetQuizs/Create
        public ActionResult Create()
        {
            // ViewBag.TopicId = new SelectList(db.AspnetSubjectTopics, "Id", "Name");



            return View();
        }

        public ActionResult AllTestList()
        {


            return View();
        }
        public ActionResult GetAllTestList()
        {
            var ID = User.Identity.GetUserId();

            var TestSubjects = db.TestSubjects.Where(x=> x.CreatedBy == ID).Select(x => new { x.Id, x.Title, x.Description, x.StartDate, x.EndTime, x.FileName, x.AspNetCours.Name, x.TotalMarks });

            return Json(TestSubjects, JsonRequestBehavior.AllowGet);

            //  return View();
        }

        public ActionResult PublishChecker(int QuizID)
        {
            string status = "notpublised";
            AspnetQuiz quiz = db.AspnetQuizs.Where(x => x.Id == QuizID).FirstOrDefault();

            if (quiz.IsPublished == true)
            {
                status = "published";
            }

            return Content(status);
        }

        public ActionResult PublishQuiz(int QuizID)
        {
            string status = "error";
            AspnetQuiz quiz = db.AspnetQuizs.Where(x => x.Id == QuizID).FirstOrDefault();
            quiz.IsPublished = true;
            if (db.SaveChanges() > 0)
            {
                status = "success";
            }
            return Content(status);
        }


        public ActionResult GetQuizList()
        {
            var user = User.Identity.GetUserId();

            //var AllQuiz = (from quiz in db.Quiz_Topic_Questions
            //               join enrollment in db.AspNetTeacher_Enrollments on quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
            //               join std in db.Student_Quiz_Scoring on quiz.QuizId equals std.QuizId
            //               where quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetClass_Courses.ClassId
            //               && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
            //               && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == enrollment.AspNetClass_Courses.CourseId
            //               && enrollment.AspNetEmployee.UserId == user
            //               select new
            //               {
            //                   QuizId = quiz.AspnetQuiz.Id,
            //                   QuizName = quiz.AspnetQuiz.Name,
            //                   QuizDescription = quiz.AspnetQuiz.Description,
            //                   QuizStartDate = quiz.AspnetQuiz.Start_Date,
            //                   QuizDueDate = quiz.AspnetQuiz.Due_Date,
            //                   QuizCreatedBy = quiz.AspnetQuiz.Created_By,
            //                   QuizCreationDate = quiz.AspnetQuiz.CreationDate,
            //                   IsPublished = quiz.AspnetQuiz.IsPublished,
            //                   Class = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
            //                   Subject = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
            //                   Section = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
            //                   Topic = quiz.AspnetSubjectTopic.Name,
            //                   StudentName = std.AspNetStudent.Name,
            //               }).Distinct().ToList();

            var quizlst = (from std in db.Student_Quiz_Scoring
                          join quiz in db.Quiz_Topic_Questions on std.QuizId equals quiz.QuizId
                          join enrollment in db.AspNetTeacher_Enrollments on quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                          where quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetClass_Courses.ClassId
                          && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
                          && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == enrollment.AspNetClass_Courses.CourseId
                          && enrollment.AspNetEmployee.UserId == user
                          select new
                          {
                              QuizID = quiz.AspnetQuiz.Id,
                              QuizName = quiz.AspnetQuiz.Name,
                              QuizDesription = quiz.AspnetQuiz.Description,
                              StartDate = quiz.AspnetQuiz.Start_Date.ToString(),
                              DueDate = quiz.AspnetQuiz.Due_Date,

                              QuizCreatedBy = quiz.AspnetQuiz.Created_By,
                              QuizCreationDate = quiz.AspnetQuiz.CreationDate,
                              IsPublished = quiz.AspnetQuiz.IsPublished,
                              Class = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                              Subject = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                              Section = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                              Topic = quiz.AspnetSubjectTopic.Name,
                              StudentName = std.AspNetStudent.Name,
                              StudentID = std.AspNetStudent.Id
                          }).Distinct().ToList();
            return Json(quizlst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadTest(int id)
        {

            TestSubject SubjectTest = db.TestSubjects.Where(x => x.Id == id).FirstOrDefault();

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentTests/"), SubjectTest.FileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), SubjectTest.FileName);

        }

        [HttpGet]
        public ActionResult StudentTests()
        {



            return View();
        }
        [HttpPost]
        public ActionResult StudentTests(TestSubject TestSubject)
        {

            var id = User.Identity.GetUserId();

            var BranchId = Convert.ToInt32(Request.Form["BranchId"]);
            var ClassId = Convert.ToInt32(Request.Form["ClassId"]);
            var SectionId = Convert.ToInt32(Request.Form["SectionId"]);
            var SubjectId = Convert.ToInt32(Request.Form["SubId"]);
            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;
            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == SectionId).FirstOrDefault().Id;

            TestSubject.SubjectId = SubjectId;
            TestSubject.BranchClassSectionId = BranchClassSectionId;
            TestSubject.CreatedBy = id;
            TestSubject.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();


            db.TestSubjects.Add(TestSubject);
            db.SaveChanges();


            HttpPostedFileBase AttachmentFile = Request.Files["AttachmentFile"];

            if (AttachmentFile != null)
            {

                if (AttachmentFile.ContentLength > 0)
                {
                    var name = Path.GetFileNameWithoutExtension(AttachmentFile.FileName);

                    var ext = Path.GetExtension(AttachmentFile.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    var FileName = name + "_TT_" + TestSubject.Id + ext;

                    AttachmentFile.SaveAs(Server.MapPath("~/Content/StudentTests/") + FileName);

                    TestSubject.FileName = FileName;
                    db.SaveChanges();
                }
            }

            TempData["TestCreated"] = "Created";
            return RedirectToAction("AllTestList", "AspnetQuizs");
        }


        public ActionResult StudentSubmittedTestList()
        {

            return View();
        }


        public ActionResult GetStudentSubmittedTestList()
        {
            var ID = User.Identity.GetUserId();

            var AllStudentSubmittedTestList = (from TestSubject in db.TestSubjects
                                               join StudentTestSubjects in db.StudentTestSubjects on TestSubject.Id equals StudentTestSubjects.TestSubjectId
                                               where TestSubject.CreatedBy == ID
                                               select new
                                               {
                                                   StudentName = StudentTestSubjects.AspNetStudent.AspNetUser.Name,
                                                   StudentTestSubjects.Id,
                                                   StudentTestSubjects.StudentSubmittedTestName,
                                                   TestSubject.Title,
                                                   TestSubject.Description,
                                                   StudentTestSubjects.StudentSubDate,
                                                   TestSubject.StartDate,
                                                   ClassName = TestSubject.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name,
                                                   SectionName = TestSubject.AspNetBranchClass_Sections.AspNetSection.Name,
                                                   TestSubject.AspNetCours.Name,
                                                   TestSubject.EndTime,
                                                   StudentTestSubjects.StudentId,
                                                   TestSubject.TotalMarks,
                                                   StudentTestSubjects.ObtainedMarks,

                                                   StudentTestSubjects.Status,


                                               }).ToList();

            return Json(AllStudentSubmittedTestList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadStudentSubmittedTest(int id)
        {

            StudentTestSubject StudentTestSubject = db.StudentTestSubjects.Where(x => x.Id == id).FirstOrDefault();

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentSubmittedTest/"), StudentTestSubject.StudentSubmittedTestName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), StudentTestSubject.StudentSubmittedTestName);

        }

        public ActionResult TeacherSubmitTest(int id, int StudentId)
        {


            StudentTestSubject StudentTestSubject = db.StudentTestSubjects.Where(x => x.Id == id && x.StudentId == StudentId).FirstOrDefault();

            ViewBag.Id = StudentTestSubject.Id;

            if (StudentTestSubject != null)
            {

                ViewBag.TeacherComment = StudentTestSubject.TeacherComments;
                ViewBag.ObtainedMarks = StudentTestSubject.ObtainedMarks;
            }
            else
            {
                ViewBag.TeacherComment = null;
                ViewBag.ObtainedMarks = null;


            }

            ViewBag.TotalMarks = StudentTestSubject.TestSubject.TotalMarks;


            return View();

        }

        public ActionResult TeacherTestSubmission(int id, string TeacherComments, float ObtainedMarks)
        {
            var File = Request.Files["file"];
            var fileName = "";

            if (File != null)
            {

                if (File.ContentLength > 0)
                {
                    //fileName = Path.GetFileName(File.FileName);

                    //File.SaveAs(Server.MapPath("~/Content/TeacherSubmittedTest/") + fileName);



                    var name = Path.GetFileNameWithoutExtension(File.FileName);

                    var ext = Path.GetExtension(File.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    fileName = name + "_LR_" + id + ext;


                    File.SaveAs(Server.MapPath("~/Content/TeacherSubmittedTest/") + fileName);


                }
            }

            //AspnetStudentAssignmentSubmission assignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.Id == id).FirstOrDefault();
            //assignmentSubmission.TeacherComments = TeacherComments.TrimStart().TrimEnd();

            var StudentTestSubject = db.StudentTestSubjects.Where(x => x.Id == id).FirstOrDefault();

            StudentTestSubject.TeacherComments = TeacherComments.TrimStart().TrimEnd();
            StudentTestSubject.TeacherSubDate = GetLocalDateTime.GetLocalDateTimeFunction();
            StudentTestSubject.ObtainedMarks = ObtainedMarks;

            if (fileName != "")
            {
                StudentTestSubject.TeacherSubmittedTestName = fileName;
            }


            db.SaveChanges();

            TempData["TestSubmit"] = "Submitted";
            return Json("", JsonRequestBehavior.AllowGet);

        }

        // POST: AspnetQuizs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AspnetQuiz aspnetQuiz)
        {


            var id = User.Identity.GetUserId();
            var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();

            DateTime startDate = DateTime.Parse(aspnetQuiz.Start_Date.ToString());
            DateTime start = DateTime.Parse(aspnetQuiz.StartTime.ToString());
            aspnetQuiz.StartTime = startDate.Date.Add(start.TimeOfDay);

            aspnetQuiz.CreationDate = DateTime.Now;
            aspnetQuiz.Created_By = username;
            db.AspnetQuizs.Add(aspnetQuiz);
            db.SaveChanges();

            string[] QuestionIDs = Request.Form["QuestionID"].Split(',');
            foreach (var a in QuestionIDs)
            {
                int Questionid = Convert.ToInt32(a);
                int SubjectTopicId = db.AspnetQuestions.Where(x => x.Id == Questionid).Select(x => x.TopicID.Value).FirstOrDefault();
                Quiz_Topic_Questions QuizTopicQuestions = new Quiz_Topic_Questions();
                QuizTopicQuestions.QuestionId = Questionid;
                QuizTopicQuestions.QuizId = aspnetQuiz.Id;
                QuizTopicQuestions.TopicId = SubjectTopicId;
                db.Quiz_Topic_Questions.Add(QuizTopicQuestions);
                db.SaveChanges();
            }

            return RedirectToAction("ViewQuestionAndQuiz", "AspnetQuestions");
        }

        public ActionResult QuizList()
        {
            return View();
        }
        public ActionResult QuizAllQuestions(int QuizID)
        {

            var QuestionNames = from Quiz in db.AspnetQuizs
                                join QuizTopicQuestion in db.Quiz_Topic_Questions on Quiz.Id equals QuizTopicQuestion.QuizId
                                join Question in db.AspnetQuestions on QuizTopicQuestion.QuestionId equals Question.Id
                                where QuizTopicQuestion.QuizId == QuizID
                                select new
                                {
                                    Question.Name
                                };



            return Json(QuestionNames, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuestionsByTopics(int[] bdoIds)
        {

            List<AspnetSubjectTopic> AllTopics = (from topic in db.AspnetSubjectTopics
                                                  where bdoIds.Contains(topic.Id)
                                                  select topic).ToList();

            //var AllQuestions =   AllTopics.Select(x => x.AspnetLessons.Select(y => y.AspnetQuestions).Select(y => y)).ToList();


            var AllQuestion = (from topic in db.AspnetSubjectTopics
                               join question in db.AspnetQuestions.Where(x=> x.Is_Active == true) on topic.Id equals question.TopicID
                               where bdoIds.Contains(topic.Id) /*&& question.Is_Quiz == true*/
                               select new
                               {
                                   question.Id,
                                   question.Name

                               }).ToList();

            return Json(AllQuestion, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSubjectTopics(int SubjectId)
        {

            var subjectTopics = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubjectId).Select(x => new { x.Id, x.Name }).ToList();


            return Json(subjectTopics, JsonRequestBehavior.AllowGet);
        }





        // GET: AspnetQuizs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspnetQuiz aspnetQuiz = db.AspnetQuizs.Find(id);


            if (aspnetQuiz == null)
            {
                return HttpNotFound();
            }

            DateTime Date = Convert.ToDateTime(aspnetQuiz.Start_Date);
            string StartDate = Date.ToString("yyyy-MM-dd");
            ViewBag.StartDate = StartDate;

            //ViewBag.StartTime = aspnetQuiz.StartTime;
            if (aspnetQuiz.Start_Date != null && aspnetQuiz.StartTime != null)
            {


                DateTime LessonStartDate = Convert.ToDateTime(aspnetQuiz.Start_Date);
                string StartDateOfLEsson = LessonStartDate.ToString("yyyy-MM-dd");
                ViewBag.LessonStartDate = StartDateOfLEsson;
                string StartTimeOfLEsson = aspnetQuiz.StartTime.Value.TimeOfDay.ToString();
                ViewBag.StartTime = StartTimeOfLEsson;

            }




            DateTime Date1 = Convert.ToDateTime(aspnetQuiz.Due_Date);
            string DueDate = Date1.ToString("yyyy-MM-dd");
            ViewBag.DueDate = DueDate;

            var AllTopicIDs = db.Quiz_Topic_Questions.Where(x => x.QuizId == id).Select(x => x.TopicId).ToList();
            var AllQuestionIDS = db.Quiz_Topic_Questions.Where(x => x.QuizId == id).Select(x => x.QuestionId).ToList();
            var AllQuestion = from Question in db.AspnetQuestions
                              where AllQuestionIDS.Contains(Question.Id)
                              select Question;
            ViewBag.QuestionID = new SelectList(AllQuestion, "Id", "Name");

            var TopicId = db.Quiz_Topic_Questions.Where(x => x.QuizId == id).Select(x => x.TopicId).FirstOrDefault();
            int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;

            var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();

            int? GenericBranchId = GenericObject.BranchId;
            int? GenericClassId = GenericObject.ClassId;
            int? GenericSectionId = GenericObject.SectionId;
            int? GenericSubjectId = GenericObject.SubjectId;

            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == GenericBranchId && x.ClassId == GenericClassId).FirstOrDefault().Id;
            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == GenericSectionId).FirstOrDefault().Id;
            int ClassCoursesID = db.AspNetClass_Courses.Where(x => x.ClassId == GenericObject.ClassId && x.CourseId == GenericSubjectId).Select(x => x.Id).FirstOrDefault();
            int TeacherId = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BranchClassSectionId && x.CourseId == ClassCoursesID).Select(x => x.TeacherId).FirstOrDefault();

            var TeacherUserId = db.AspNetEmployees.Where(x => x.Id == TeacherId).FirstOrDefault().UserId;
            var ID = User.Identity.GetUserId();

            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetEmployee.BranchId
                            where enrollment.AspNetEmployee.UserId == TeacherUserId
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();

            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                           where (branchclasssubject.BranchId == GenericObject.BranchId && enrollment.AspNetEmployee.UserId == TeacherUserId)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();

            var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == TeacherUserId && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId).Select(x => new
            {
                Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                Name = x.AspNetBranchClass_Sections.AspNetSection.Name
            }).Distinct();


            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                            where (branchclasssubject.SectionId == GenericObject.SectionId && enrollment.AspNetEmployee.UserId == TeacherUserId)
                            select new
                            {
                                subject.Id,
                                subject.Name,
                            }).Distinct();

            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == GenericObject.BranchId && x.ClassId == GenericObject.ClassId && x.SubjectId == GenericObject.SubjectId && x.SectionId == GenericObject.SectionId).FirstOrDefault();
            var genericTableIid = Generic.Id;
            var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == genericTableIid).ToList().Select(x => new { x.Id, x.Name });

            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
            ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
            ViewBag.SubId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);
            ViewBag.TopicId = new SelectList(Topics, "Id", "Name");

            ViewBag.QuizID = id;
            return View(aspnetQuiz);
        }

        // POST: AspnetQuizs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AspnetQuiz aspnetQuiz)
        {
            if (ModelState.IsValid)
            {
                AspnetQuiz quiz = db.AspnetQuizs.Where(x => x.Id == aspnetQuiz.Id).FirstOrDefault();
              
                DateTime startDate = DateTime.Parse(aspnetQuiz.Start_Date.ToString());
                DateTime start = DateTime.Parse(aspnetQuiz.StartTime.ToString());
                quiz.StartTime = startDate.Date.Add(start.TimeOfDay);


                quiz.Name = aspnetQuiz.Name;
                quiz.Description = aspnetQuiz.Description;
                quiz.Start_Date = aspnetQuiz.Start_Date;
                quiz.Due_Date = aspnetQuiz.Due_Date;
                quiz.MeetingLink = aspnetQuiz.MeetingLink;
                quiz.QuizTime = aspnetQuiz.QuizTime;
                db.SaveChanges();

                if (Request.Form["QuestionID"] != null)
                {
                    string[] QuestionIDs = Request.Form["QuestionID"].Split(',');
                    List<Quiz_Topic_Questions> QuizTopicQuestionsToRemove = db.Quiz_Topic_Questions.Where(x => x.QuizId == aspnetQuiz.Id).ToList();

                    db.Quiz_Topic_Questions.RemoveRange(QuizTopicQuestionsToRemove);
                    db.SaveChanges();

                    foreach (var a in QuestionIDs)
                    {
                        int Questionid = Convert.ToInt32(a);
                        int SubjectTopicId = db.AspnetQuestions.Where(x => x.Id == Questionid).Select(x => x.TopicID.Value).FirstOrDefault();
                        Quiz_Topic_Questions QuizTopicQuestions = new Quiz_Topic_Questions();
                        QuizTopicQuestions.QuestionId = Questionid;
                        QuizTopicQuestions.QuizId = aspnetQuiz.Id;
                        QuizTopicQuestions.TopicId = SubjectTopicId;
                        db.Quiz_Topic_Questions.Add(QuizTopicQuestions);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ViewQuestionAndQuiz", "AspnetQuestions");
        }

        public ActionResult DeleteQuiz(int QuizID)
        {
            string Status = "error";

            List<Quiz_Topic_Questions> Qt = db.Quiz_Topic_Questions.Where(x=>x.QuizId == QuizID).ToList();

            db.Quiz_Topic_Questions.RemoveRange(Qt);
           if(db.SaveChanges()>0)
           {
               AspnetQuiz Quiz = db.AspnetQuizs.Find(QuizID);
               db.AspnetQuizs.Remove(Quiz);
               if (db.SaveChanges() > 0)
               {

                   Status = "Success";
               }
           }

            return Content(Status);
        }
        // GET: AspnetQuizs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetQuiz aspnetQuiz = db.AspnetQuizs.Find(id);
            if (aspnetQuiz == null)
            {
                return HttpNotFound();
            }
            return View(aspnetQuiz);
        }

        // POST: AspnetQuizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspnetQuiz aspnetQuiz = db.AspnetQuizs.Find(id);
            db.AspnetQuizs.Remove(aspnetQuiz);
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
