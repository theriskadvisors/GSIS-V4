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
    public class AspnetQuestionsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        private TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

        // GET: AspnetQuestions
        public ActionResult Index()
        {
            var aspnetQuestions = db.AspnetQuestions.Include(a => a.AspnetLesson).Include(a => a.AspnetOption);
            return View(aspnetQuestions.ToList());
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult ViewQuestionAndQuiz()
        {
            var aspnetQuestions = db.AspnetQuestions.Include(a => a.AspnetLesson).Include(a => a.AspnetOption);
            return View(aspnetQuestions.ToList());
        }

        public ActionResult ViewQuestionAndQuizByAdmin()
        {
            var aspnetQuestions = db.AspnetQuestions.Include(a => a.AspnetLesson).Include(a => a.AspnetOption);
            return View(aspnetQuestions.ToList());
        }

        public ActionResult AllQuestionList()
        {
            var user = User.Identity.GetUserId();

            var AllQuestions = (from quiz in db.AspnetQuestions
                                join enrollment in db.AspNetTeacher_Enrollments on quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                                where quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetClass_Courses.ClassId
                                && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
                                && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == enrollment.AspNetClass_Courses.CourseId
                                && enrollment.AspNetEmployee.UserId == user
                                select new
                                {
                                    QuestionId = quiz.Id,
                                    QuestionName = quiz.Name,
                                    QuestionType = quiz.Type,
                                    LessonName = quiz.AspnetSubjectTopic.Name,
                                    Option = quiz.AspnetOption.Name,
                                    QuestionCreationDate = quiz.CreationDate,
                                    Class = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                    Section = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                    Subject = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                    Topic = quiz.AspnetSubjectTopic.Name,
                                    Status = quiz.Is_Active
                                }).Distinct().ToList();

            return Json(AllQuestions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllQuizList()
        {
            var user = User.Identity.GetUserId();

            var AllQuiz = (from quiz in db.Quiz_Topic_Questions
                           join enrollment in db.AspNetTeacher_Enrollments on quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                           where quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetClass_Courses.ClassId
                           && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
                           && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == enrollment.AspNetClass_Courses.CourseId
                           && enrollment.AspNetEmployee.UserId == user
                           select new
                           {
                               QuizId = quiz.AspnetQuiz.Id,
                               QuizName = quiz.AspnetQuiz.Name,
                               QuizDescription = quiz.AspnetQuiz.Description,
                               QuizStartDate = quiz.AspnetQuiz.Start_Date,
                               QuizDueDate = quiz.AspnetQuiz.Due_Date,
                               QuizCreatedBy = quiz.AspnetQuiz.Created_By,
                               QuizCreationDate = quiz.AspnetQuiz.CreationDate,
                               IsPublished = quiz.AspnetQuiz.IsPublished,
                               Class = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                               Subject = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                               Section = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                               Topic = quiz.AspnetSubjectTopic.Name
                           }).Distinct().ToList();

            //var AllQuiz = (from Quiz in db.AspnetQuizs
            //                  select new
            //                  {
            //                      QuizId = Quiz.Id,
            //                      QuizName = Quiz.Name,
            //                      QuizDescription = Quiz.Description,
            //                      QuizStartDate = Quiz.Start_Date,
            //                      QuizDueDate = Quiz.Due_Date,
            //                      QuizCreatedBy = Quiz.Created_By,
            //                      QuizCreationDate = Quiz.CreationDate,

            //                  }).ToList();

            return Json(AllQuiz, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllQuizListByAdmin()
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;
            if (User.IsInRole("Branch_Admin") || User.IsInRole("Branch_Principal"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }


            //  var user = User.Identity.GetUserId();

            var AllQuiz = (from quiz in db.Quiz_Topic_Questions
                           join enrollment in db.AspNetTeacher_Enrollments on quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                           where quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId == enrollment.AspNetClass_Courses.ClassId
                           && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId == enrollment.AspNetBranchClass_Sections.SectionId
                           && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == enrollment.AspNetClass_Courses.CourseId
                           && quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId == branchId
                           select new
                           {
                               QuizId = quiz.AspnetQuiz.Id,
                               QuizName = quiz.AspnetQuiz.Name,
                               QuizDescription = quiz.AspnetQuiz.Description,
                               QuizStartDate = quiz.AspnetQuiz.Start_Date,
                               QuizDueDate = quiz.AspnetQuiz.Due_Date,
                               Class = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                               Subject = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                               Section = quiz.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                               IsPublished = quiz.AspnetQuiz.IsPublished
                           }).Distinct().ToList();


            return Json(AllQuiz, JsonRequestBehavior.AllowGet);
        }


        // GET: AspnetQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetQuestion aspnetQuestion = db.AspnetQuestions.Find(id);
            if (aspnetQuestion == null)
            {
                return HttpNotFound();
            }
            return View(aspnetQuestion);
        }

        // GET: AspnetQuestions/Create
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(int id)
        {
            if (id == 0)
            {
                ViewBag.TopicExist = 0;
                ViewBag.LessonId = new SelectList(db.AspnetLessons, "Id", "Name");
                ViewBag.TeacherID = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Teacher")), "Id", "Name");

                ViewBag.TopicExist = 0;
                ViewBag.BranchId = null;
                ViewBag.ClassId = null;
                ViewBag.SectionId = null;
                ViewBag.SubId = null;
                ViewBag.TopicId = null;
            }
            else
            {

                AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);
                int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == aspnetSubjectTopic.Id).FirstOrDefault().GenericBranchClassSubjectId;
                var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();
                ViewBag.TopicExist = 1;
                ViewBag.BranchId = GenericObject.BranchId;
                ViewBag.ClassId = GenericObject.ClassId;
                ViewBag.SectionId = GenericObject.SectionId;
                ViewBag.SubId = GenericObject.SubjectId;
                ViewBag.TopicId = aspnetSubjectTopic.Id;

            }
            return View();
        }

        // POST: AspnetQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionAnswerViewModel QuestionAnswerViewModel, HttpPostedFileBase image)
        {
            foreach (var item in QuestionAnswerViewModel.TopicId)
            {

                var id = User.Identity.GetUserId();
                var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();

                AspnetQuestion Question = new AspnetQuestion();
                Question.Name = QuestionAnswerViewModel.QuestionName;

                if (Request.Form["QuestionIsActive"] == "on")
                {
                    Question.Is_Active = true;
                }
                else
                    Question.Is_Active = false;

                Question.TopicID = item;
                Question.Is_Quiz = false;
                Question.Type = QuestionAnswerViewModel.QuestionType;
                //   Question.LessonId = QuestionAnswerViewModel.LessonId;
                Question.AnswerId = null;
                Question.CreatedBy = username;
                DateTime PkTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);

                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var extension = Path.GetExtension(image.FileName);
                    image.SaveAs(Server.MapPath("~/Content/QuestionPhotos/") + image.FileName);
                    Question.Photo = image.FileName;
                }

                //Question.CreationDate = DateTime.Now;
                Question.CreationDate = PkTime;
                db.AspnetQuestions.Add(Question);
                db.SaveChanges();
                var QuestionType = QuestionAnswerViewModel.QuestionType;
                if (QuestionType == "MCQ" || QuestionType == "TF")
                {
                    AspnetOption Op1 = new AspnetOption();
                    Op1.Name = QuestionAnswerViewModel.OptionNameOne;
                    Op1.QuestionId = Question.Id;
                    //Op1.CreationDate = DateTime.Now;
                    Op1.CreationDate = PkTime;
                    db.AspnetOptions.Add(Op1);
                    db.SaveChanges();
                    AspnetOption Op2 = new AspnetOption();
                    Op2.Name = QuestionAnswerViewModel.QuestionNameTwo;
                    Op2.QuestionId = Question.Id;
                    //Op2.CreationDate = DateTime.Now;
                    Op2.CreationDate = PkTime;
                    db.AspnetOptions.Add(Op2);
                    db.SaveChanges();

                    AspnetOption Op3 = new AspnetOption();
                    Op3.Name = QuestionAnswerViewModel.QuestionNameThree;
                    Op3.QuestionId = Question.Id;
                    //Op3.CreationDate = DateTime.Now;
                    Op3.CreationDate = PkTime;
                    db.AspnetOptions.Add(Op3);
                    db.SaveChanges();

                    AspnetOption Op4 = new AspnetOption();
                    Op4.Name = QuestionAnswerViewModel.QuesitonNameFour;
                    Op4.QuestionId = Question.Id;
                    //Op4.CreationDate = DateTime.Now;
                    Op4.CreationDate = PkTime;
                    db.AspnetOptions.Add(Op4);
                    db.SaveChanges();

                    int AnswerId;

                    if (QuestionAnswerViewModel.Answer == "a")
                    {
                        AnswerId = Op1.Id;
                    }

                    else if (QuestionAnswerViewModel.Answer == "b")
                    {
                        AnswerId = Op2.Id;
                    }

                    else if (QuestionAnswerViewModel.Answer == "c")
                    {
                        AnswerId = Op3.Id;
                    }
                    else
                    {
                        AnswerId = Op4.Id;
                    }
                    Question.AnswerId = AnswerId;
                    db.SaveChanges();
                }
                else
                {
                    AspnetOption Op = new AspnetOption();
                    Op.Name = QuestionAnswerViewModel.FillAnswer;
                    Op.QuestionId = Question.Id;
                    //Op.CreationDate = DateTime.Now;
                    Op.CreationDate = PkTime;
                    db.AspnetOptions.Add(Op);
                    db.SaveChanges();
                    int AnswerId;
                    AnswerId = Op.Id;
                    Question.AnswerId = AnswerId;
                    db.SaveChanges();
                }
            }


            ViewBag.LessonId = new SelectList(db.AspnetLessons, "Id", "Name");
            return RedirectToAction("ViewQuestionAndQuiz");
        }
        public ActionResult TestQuestions()
        {
            return View();
        }

        public ActionResult TestQuestionsByTopics(int bdoIds)
        {

            var AllQuestion = (from topic in db.AspnetSubjectTopics
                               join lesson in db.AspnetLessons on topic.Id equals lesson.TopicId
                               join question in db.AspnetQuestions on lesson.Id equals question.LessonId

                               where topic.Id == bdoIds && question.Is_Quiz == false && question.Type == "MCQ"
                               select new
                               {
                                   question.Id,
                                   question.Name

                               }).ToList();

            return Json(AllQuestion, JsonRequestBehavior.AllowGet);
        }

        // GET: AspnetQuestions/Edit/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetQuestion aspnetQuestion = db.AspnetQuestions.Find(id);

            QuestionAnswerViewModel QuestionViewModel = new QuestionAnswerViewModel();

            if (aspnetQuestion != null)
            {
                QuestionViewModel.QuestionIsQuiz = Convert.ToBoolean(aspnetQuestion.Is_Quiz);
                QuestionViewModel.QuestionType = aspnetQuestion.Type;
                QuestionViewModel.QuestionName = aspnetQuestion.Name;
                QuestionViewModel.Id = aspnetQuestion.Id;
                QuestionViewModel.QuestionIsActive = Convert.ToBoolean(aspnetQuestion.Is_Active);
                string IsMandatory;



                string[] options = db.AspnetOptions.Where(x => x.QuestionId == aspnetQuestion.Id).Select(x => x.Name).ToArray();

                QuestionViewModel.OptionNameOne = options[0];
                QuestionViewModel.QuestionNameTwo = options[1];
                QuestionViewModel.QuestionNameThree = options[2];
                QuestionViewModel.QuesitonNameFour = options[3];

                List<int> AnwersListToChoones = db.AspnetOptions.Where(x => x.QuestionId == aspnetQuestion.Id).Select(x => x.Id).ToList();

                int count = 1;
                foreach (int FindAsnwer in AnwersListToChoones)
                {


                    if (FindAsnwer == aspnetQuestion.AnswerId)
                    {

                        break;

                    }
                    count++;

                }
                string Answer = "";
                if (count == 1)
                {
                    Answer = "a";
                }
                else if (count == 2)
                {
                    Answer = "b";

                }
                else if (count == 3)
                {
                    Answer = "c";

                }
                else if (count == 4)
                {


                    Answer = "d";

                }
                else
                {
                    Answer = "";

                }

                ViewBag.Answer = Answer;

                ViewBag.Photo = db.AspnetQuestions.Where(X => X.Id == id).FirstOrDefault().Photo;
                int? TopicId = db.AspnetQuestions.Where(X => X.Id == id).FirstOrDefault().TopicID;

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
                                // join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetEmployee.BranchId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId

                                where enrollment.AspNetEmployee.UserId == TeacherUserId
                                select new
                                {
                                    branch.Id,
                                    branch.Name,
                                }).Distinct();


                var Classes = (from classs in db.AspNetClasses
                               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                               join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                               where (branchclasssubject.BranchId == GenericObject.BranchId
                               && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id == GenericObject.BranchId

                               && enrollment.AspNetEmployee.UserId == TeacherUserId)
                               select new
                               {
                                   classs.Id,
                                   classs.Name,
                               }).Distinct();


                var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == TeacherUserId && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId && x.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == GenericObject.BranchId).Select(x => new
                {
                    Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                    Name = x.AspNetBranchClass_Sections.AspNetSection.Name
                }).Distinct();


                var Subjects = (from subject in db.AspNetCourses
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                                where (branchclasssubject.SectionId == GenericObject.SectionId && branchclasssubject.BranchId == GenericObject.BranchId && branchclasssubject.ClassId == GenericObject.ClassId
                                && enrollment.AspNetBranchClass_Sections.SectionId == GenericObject.SectionId && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == GenericObject.BranchId
                                && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId
                                && enrollment.AspNetEmployee.UserId == TeacherUserId)
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
                ViewBag.TopicId = new SelectList(Topics, "Id", "Name", TopicId);


            }

            return View(QuestionViewModel);
        }

        // POST: AspnetQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionAnswerViewModel QuestionAnswerViewModel, HttpPostedFileBase image)
        {

            AspnetQuestion Question = db.AspnetQuestions.Where(x => x.Id == QuestionAnswerViewModel.Id).FirstOrDefault();
            if (Request.Form["QuestionIsActive"] == "on")
            {
                Question.Is_Active = true;
            }
            else
                Question.Is_Active = false;

            Question.Name = QuestionAnswerViewModel.QuestionName;
            // Question.LessonId = QuestionAnswerViewModel.LessonId;
            Question.Is_Quiz = QuestionAnswerViewModel.QuestionIsQuiz;
            if (image != null)
            {
                var fileName = Path.GetFileName(image.FileName);
                var extension = Path.GetExtension(image.FileName);
                image.SaveAs(Server.MapPath("~/Content/QuestionPhotos/") + image.FileName);
            }

            if (image != null)
            {
                Question.Photo = image.FileName;

            }



            db.SaveChanges();

            var QuestionType = QuestionAnswerViewModel.QuestionType;
            if (QuestionType == "MCQ" || QuestionType == "TF")
            {
                AspnetOption[] options = db.AspnetOptions.Where(x => x.QuestionId == QuestionAnswerViewModel.Id).ToArray();

                options[0].Name = QuestionAnswerViewModel.OptionNameOne;
                options[1].Name = QuestionAnswerViewModel.QuestionNameTwo;
                options[2].Name = QuestionAnswerViewModel.QuestionNameThree;
                options[3].Name = QuestionAnswerViewModel.QuesitonNameFour;
                db.SaveChanges();


                int AnswerId;

                if (QuestionAnswerViewModel.Answer == "a")
                {

                    AnswerId = options[0].Id;
                }

                else if (QuestionAnswerViewModel.Answer == "b")
                {
                    AnswerId = options[1].Id;

                }

                else if (QuestionAnswerViewModel.Answer == "c")
                {
                    AnswerId = options[2].Id;

                }

                else
                {
                    AnswerId = options[3].Id;

                }

                Question.AnswerId = AnswerId;
                db.SaveChanges();

            }
            else
            {
                DateTime PkTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);
                AspnetOption Op = new AspnetOption();
                Op.Name = QuestionAnswerViewModel.FillAnswer;
                Op.QuestionId = Question.Id;
                //Op.CreationDate = DateTime.Now;
                Op.CreationDate = PkTime;
                db.AspnetOptions.Add(Op);
                db.SaveChanges();
                int AnswerId;
                AnswerId = Op.Id;
                Question.AnswerId = AnswerId;
                db.SaveChanges();
            }

            return RedirectToAction("ViewQuestionAndQuiz");
        }
        // GET: AspnetQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetQuestion aspnetQuestion = db.AspnetQuestions.Find(id);
            if (aspnetQuestion == null)
            {
                return HttpNotFound();
            }
            return View(aspnetQuestion);
        }

        // POST: AspnetQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspnetQuestion aspnetQuestion = db.AspnetQuestions.Find(id);
            db.AspnetQuestions.Remove(aspnetQuestion);
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
