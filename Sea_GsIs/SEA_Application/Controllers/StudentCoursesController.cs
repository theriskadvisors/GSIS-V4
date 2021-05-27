using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SEA_Application.CustomModel;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
//using static SEA_Application.Controllers.AjaxAuthorizationController;

namespace SEA_Application.Controllers
{
    //[Authorize(Roles = "Student")]
    public class StudentCoursesController : Controller
    {
        public static List<question1> QuestionsStaticList = new List<question1>();
        public static string TotalScore { get; set; }
        public static string ReviseLessons { get; set; }

        private Sea_Entities db = new Sea_Entities();
        //   public static int SessionID = Convert.ToInt32(SessionIDStaticController.GlobalSessionID);


        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {

            return View();
        }
   
        //[Authorize(Roles = "Admin,Principal,Accountant,Student,Teacher,Staff,PhotoCopier,Receptionist")]
        // [Authorize(Roles = "Student")]
        [AjaxAuthorize]
        public ActionResult MeetingInfo(int LessonID)
        {
            //var Status = "Fail";
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Json(Status);
            //}
             var StudentID = User.Identity.GetUserId();

            TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime PKTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);

            if (db.StudentLessonTrackings.Where(x => x.StudentId == StudentID && x.LessonId == LessonID).Count() > 0)
            {
                //update the record
                StudentLessonTracking slt = db.StudentLessonTrackings.Where(x => x.StudentId == StudentID && x.LessonId == LessonID).FirstOrDefault();

                if (slt.MeetingJoinTime == null)
                {
                    DateTime MeetingStartTime = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().StartTime.Value;
                    if (PKTime.Date == MeetingStartTime.Date)
                    {
                        var difference = PKTime.TimeOfDay - MeetingStartTime.TimeOfDay;
                        var totalmin = difference.Hours * 60 + difference.Minutes;
                        if (totalmin <= 15)
                        {
                            slt.MeetingJoinStatus = "Present";
                        }
                        else if (totalmin > 15 && totalmin <= 60)
                        {
                            slt.MeetingJoinStatus = "Late";
                        }
                        else
                        {
                            slt.MeetingJoinStatus = "Absent";
                        }
                        slt.MeetingJoinTime = PKTime;
                        db.SaveChanges();
                    }
                    else
                    {
                        slt.MeetingJoinStatus = "Absent";
                        slt.MeetingJoinTime = PKTime;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                //create new record
                StudentLessonTracking slt = new StudentLessonTracking();
                slt.LessonId = LessonID;
                slt.StudentId = StudentID;
                //   slt.StartDate = DateTime.Now;
                slt.MeetingJoinTime = PKTime;

                DateTime MeetingStartTime = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().StartTime.Value;
                if (PKTime.Date == MeetingStartTime.Date)
                {
                    var difference = PKTime.TimeOfDay - MeetingStartTime.TimeOfDay;
                    var totalmin = difference.Hours * 60 + difference.Minutes;
                    if (totalmin <= 15)
                    {
                        slt.MeetingJoinStatus = "Present";
                    }
                    else if (totalmin > 15 && totalmin <= 60)
                    {
                        slt.MeetingJoinStatus = "Late";
                    }
                    else
                    {
                        slt.MeetingJoinStatus = "Absent";
                    }
                    slt.MeetingJoinTime = PKTime;
                    db.StudentLessonTrackings.Add(slt);
                    db.SaveChanges();
                }
                else
                {
                    slt.MeetingJoinStatus = "Absent";
                    slt.MeetingJoinTime = PKTime;
                    db.StudentLessonTrackings.Add(slt);
                    db.SaveChanges();
                }

            }
           // Status = "Success";
            return Json("");
        }

        public ActionResult AttendanceChecker(int LessonId)
        {
            string status = "";
            string ContentType = "";
            var UserId = User.Identity.GetUserId();

            ContentType = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().ContentType;
            StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId && x.StudentId == UserId).FirstOrDefault();

            if (LessonTracking == null)
            {
                status = "Absent";

            }
            else if (ContentType == "Link")
            {

                status = LessonTracking.MeetingJoinStatus;
            }
            else
            {

                status = LessonTracking.LessonStatus;
            }

            if (status == "" || status == null)
            {
                status = "Absent";
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllSubjectsOfStudent()
        {

            var userID = User.Identity.GetUserId();
            var UserRole = db.GetUserRoleById(userID).FirstOrDefault();
            // int ClassID = db.AspNetClasses.Where(x => x.SessionID == SessionID).FirstOrDefault().Id;


            //var AllSubjectsOfStudent = from Subject in db.AspNetSubjects
            //                           join StudentSubject in db.AspNetStudent_Subject on Subject.Id equals StudentSubject.SubjectID
            //                           where StudentSubject.StudentID == userID
            //                           select new
            //                           {
            //                               Subject.Id,
            //                               Subject.SubjectName,
            //                               Subject.CourseType,
            //                               Subject.Points,
            //                            };

            //var AllSubjectsOfStudent = from Subject in db.GenericSubjects
            //                           join StudentSubject in db.Student_GenericSubjects on Subject.Id equals StudentSubject.GenericSubjectId
            //                           where StudentSubject.StudentId == "91897279-8bd3-4faa-b6d3-695df41d2177"
            //                           select new
            //                           {
            //                               Subject.Id,
            //                               Subject.SubjectName,

            //                           };
            //var session = db.AspNetSessions.Where(x => x.IsActive == true).Select(x => x.Id).FirstOrDefault();
            //var student_id = db.AspNetStudents.Where(x => x.UserId == userID).Select(x => x.Id).FirstOrDefault();
            //var section = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student_id && x.SessionId == session).Select(x => x.AspNetBranchClass_Sections.AspNetSection.Id).FirstOrDefault();
            //var Studentclass = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student_id && x.SessionId == session).Select(x => x.AspNetClass_Courses.AspNetClass.Id).FirstOrDefault();
            //var branchId = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student_id && x.SessionId == session).Select(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id).FirstOrDefault();



            //var AllSubjectsOfStudent = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == branchId && x.ClassId == Studentclass && x.SectionId == section).Select(x => new { x.AspNetCours.Name, x.Id }).Distinct().ToList();

            var subjects = (from enrollment in db.AspNetStudent_Enrollments
                            join generic in db.AspnetGenericBranchClassSubjects on enrollment.AspNetClass_Courses.CourseId equals generic.SubjectId
                            where enrollment.AspNetClass_Courses.ClassId == generic.ClassId && enrollment.AspNetBranchClass_Sections.SectionId == generic.SectionId
                            && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == generic.BranchId && enrollment.AspNetStudent.UserId == userID
                            select new
                            {
                                Id = generic.Id,
                                Name = generic.AspNetCours.Name
                            }).Distinct();

            //var AllSubjectsOfStudent = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student_id ).Select(x => new { x.AspNetClass_Courses.AspNetCours.Name, x.AspNetClass_Courses.AspNetCours.Id }).Distinct().ToList();
            //var AllSubjectsOfStudent = (from user in db.AspNetUsers;
            //                           join student in db.AspNetStudents on user.Id equals student.UserId
            //                           join studentenroll in db.AspNetStudent_Enrollments on student.Id equals studentenroll.StudentId
            //                           join classcourses in db.AspNetClass_Courses on studentenroll.CourseId equals classcourses.Id
            //                           join courses in db.AspNetCourses on classcourses.CourseId equals courses.Id
            //                           //where user.Id == userID
            //                           select new {


            //                               courses.Id,
            //                               courses.Name,

            //                           }).Distinct();


            return Json(subjects, JsonRequestBehavior.AllowGet);


        }
        [Authorize(Roles = "Student")]
        public ActionResult SubjectTopics(int id)
        {
            
            ViewBag.SubjectId = id;


            return View();
        }

        [Authorize(Roles = "Student")]
        public ActionResult StudentLessons(string id)
        {
            var Lesson = db.AspnetLessons.Where(x => x.EncryptedID == id).FirstOrDefault();
            if (Lesson == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? TopicId = Lesson.TopicId;
            string Name = Lesson.Name;

            // tab 6 Data Start
            var questionList_MCQS = new List<question>();
            //    List<AspnetSubjectTopic> SubjectTopics =   db.AspnetSubjectTopics.Where(x => x.Id == 35).ToList();

            List<int> AllLessonofTopics = db.AspnetLessons.Where(x => x.AspnetSubjectTopic.Id == TopicId).Select(x => x.Id).ToList();

            var items = AllLessonofTopics.Select(num => (int?)num).ToList();

            var Questions = from question in db.AspnetQuestions
                            where items.Contains(question.LessonId) && question.Type == "MCQ" && question.Is_Quiz == false
                            select question;

            foreach (var item in Questions)
            {
                var q = new question();
                q.id = item.Id;
                q.name = item.Name;
                q.type = item.Type;

                q.options = new List<option>();
                var op = db.AspnetOptions.Where(x => x.QuestionId == q.id).ToList();
                foreach (var item1 in op)
                {
                    var op1 = new option();
                    op1.id = item1.Id;
                    op1.name = item1.Name;
                    q.options.Add(op1);
                }
                questionList_MCQS.Add(q);
            }

            ViewBag.questionList_MCQS = questionList_MCQS;
            //Tab 6 data end

            ViewBag.TopicId = TopicId;
            ViewBag.LessonID = Lesson.Id;
            //   ViewBag.SubjectId = Lesson.AspnetSubjectTopic.GenericSubject.Id;
            ViewBag.SubjectId = Lesson.AspnetSubjectTopic.GenericBranchClassSubjectId;

            return View();
        }
        public ActionResult IsLastLesson(int LessonID)
        {
            var Lesson = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault();

            int? TopicId = Lesson.TopicId;
            string Name = Lesson.Name;

            string LessonLastName = db.AspnetLessons.Where(x => x.TopicId == TopicId).OrderByDescending(x => x.Name).Select(x => x.Name).FirstOrDefault();

            var IsLastLesson = "";

            if (Name == LessonLastName)
            {
                IsLastLesson = "Yes";
            }
            else
            {
                IsLastLesson = "No";

            }

            //ViewBag.TopicId = TopicId;
            // ViewBag.IsLastLesson = IsLastLesson;



            return Json(new { TopicId = TopicId, IsLastLesson = IsLastLesson }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLessonViewContent(int LessonID)
        {
            var Lesson = db.AspnetLessons.Where(x => x.Id == LessonID).Select(x => new { LessonId = x.Id, LessonName = x.Name, LessonVideo = x.Video_Url, LessonDescription = x.Description + " (" + x.StartTime.ToString() + ")", ContentType = x.ContentType, MeetingLink = x.MeetingLink, LessonImg = x.LessonIMG }).FirstOrDefault();

            return Json(Lesson, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Student")]
        public ActionResult Test(int id)
        {

            return View();

        } // End of Test Action Methods


        //public ActionResult submit_question(string Question, string Answer)
        //{
        //    int score = 0;
        //    string[] selectedQuestions = Question.Split(',');
        //    string[] selectedAnswers = Answer.Split(',');

        //    int i = 0;
        //    foreach (var item in selectedQuestions)
        //    {
        //        var ans = db.AspnetQuestions.Where(x => x.Id.ToString() == item).Select(x => x.AnswerId).FirstOrDefault();
        //        string val = selectedAnswers[i];
        //        if (selectedAnswers[i] == ans.ToString())
        //        {
        //            score++;
        //        }
        //        else { }
        //        i++;

        //        // db.SaveChanges();
        //    }
        //    var questionList_MCQS = new List<question>();

        //    i = 0;
        //    foreach (var item in selectedQuestions)
        //    {
        //        var q = new question();

        //        AspnetQuestion Queston = db.AspnetQuestions.Where(x => x.Id.ToString() == item).FirstOrDefault();

        //        q.id = Convert.ToInt32(item);
        //        q.name = Queston.Name;
        //        q.type = Queston.Type;
        //        q.CorrentAnswer = Queston.AnswerId;
        //        q.StudentAnswer = Convert.ToInt32(selectedAnswers[i]);

        //        q.options = new List<option>();
        //        var op = db.AspnetOptions.Where(x => x.QuestionId == q.id).ToList();
        //        foreach (var item1 in op)
        //        {
        //            var op1 = new option();
        //            op1.id = item1.Id;
        //            op1.name = item1.Name;
        //            q.options.Add(op1);
        //        }
        //        questionList_MCQS.Add(q);

        //        i++;
        //    }


        //    // return Content(score.ToString());

        //    //return RedirectToAction("Index");
        //    QuestionsStaticList = questionList_MCQS;
        //    return Json(questionList_MCQS, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult submit_question(string Question, string Answer)
        {
            int score = 0;
            string[] selectedQuestions = Question.Split(',');
            string[] selectedAnswers = Answer.Split(',');

            int i = 0;

            var questionList_MCQS = new List<question1>();

            foreach (var item in selectedQuestions)
            {
                question1 QuestionObj = new question1();

                var ans = db.AspnetQuestions.Where(x => x.Id.ToString() == item).Select(x => x.AnswerId).FirstOrDefault();
                var QuestionFromDB = db.AspnetQuestions.Where(x => x.Id.ToString() == item).FirstOrDefault();

                QuestionObj.name = QuestionFromDB.Name;
                QuestionObj.id = QuestionFromDB.Id;
                QuestionObj.type = QuestionFromDB.Type;


                string val = selectedAnswers[i];
                if (selectedAnswers[i] == ans.ToString())
                {


                    score++;

                    var RightAnswer = "Selected Answer is Correct";

                    QuestionObj.Message = RightAnswer;
                    QuestionObj.IsCorrect = "Yes";

                }
                else
                {


                    var AnswerName = db.AspnetOptions.Where(x => x.Id == QuestionFromDB.AnswerId).FirstOrDefault().Name;
                    var LessonName = db.AspnetLessons.Where(x => x.Id == QuestionFromDB.LessonId).FirstOrDefault().Name;

                    if (selectedAnswers[i] == "")
                    {

                        QuestionObj.IsCorrect = "No";
                        var WrongAnswer = "Correct Answer  is " + AnswerName + " you need to revise " + LessonName + " Lesson";

                        QuestionObj.Message = WrongAnswer;

                    }
                    else
                    {

                        QuestionObj.IsCorrect = "No";
                        var WrongAnswer = "Your Answer is Wrong .Correct Answer  is " + AnswerName + " you need to revise " + LessonName + " Lesson";

                        QuestionObj.Message = WrongAnswer;


                    }

                }

                i++;

                questionList_MCQS.Add(QuestionObj);
            }

            return Json(questionList_MCQS, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Student")]
        public ActionResult TestResult()
        {


            ViewBag.TotalScore = TotalScore;
            ViewBag.ReviseLessons = ReviseLessons;
            ViewBag.QuestionsList = QuestionsStaticList;


            return View();
        }



        public class question1
        {
            public int id;
            public string name;
            public string type;
            public string Message;
            public string IsCorrect;

        }


        public class option
        {
            public int id;
            public string name;
        }

        public class question
        {
            public int id;
            public string name;
            public string type;
            public int? CorrentAnswer;
            public int? StudentAnswer;
            public List<option> options;
        }

        public ActionResult GetSubjectTopicsAndLessons(int SubjectId)
        {

            try
            {

                var UserId = User.Identity.GetUserId();

                //  var userSessionId = db.AspNetUsers_Session.Where(x => x == UserId).FirstO rDefault().SessionID;



                //  var SubjectTopics = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubjectId).ToList();


                //var AllSubjectTopicsLessons = from SubjectTopic in db.AspnetSubjectTopics
                //                              join Lesson in db.AspnetLessons on SubjectTopic.Id equals Lesson.TopicId
                //                              where SubjectTopic.SubjectId == SubjectId
                //                              select new
                //                              {
                //                                  TopicId = SubjectTopic.Id,
                //                                  TopicName =   SubjectTopic.Name,
                //                                  LessonId = Lesson.Id,
                //                                  LessonName = Lesson.Name,
                //                                  Lesson.Duration,
                //                                  Lesson.Description

                //                              };

                var SubjectsTopics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == SubjectId).ToList();


                List<Topic> TopicListObj = new List<Topic>();

                int Count = 0;
                foreach (var a in SubjectsTopics)
                {
                    int count1 = 0;
                    decimal DurationCount = 0;

                    Topic TopicObj = new Topic();

                    //  var LessonList = db.AspnetLessons.Where(x => x.TopicId == a.Id).ToList();

                    var today = DateTime.Today;
                    var LessonList = (from Lesson in db.AspnetLessons.Where(x => x.Status == true)
                                          // join LessonSession in db.Lesson_Session on Lesson.Id equals LessonSession.LessonId
                                      where Lesson.TopicId == a.Id/* && Lesson.IsActive == true*/  && Lesson.StartDate <= today //&& LessonSession.StartDate <= today && today <= LessonSession.DueDate
                                      select Lesson).ToList();



                    TopicObj.TopicId = a.Id;
                    TopicObj.TopicName = a.Name;
                    TopicObj.Orderby = Convert.ToInt32(a.OrderBy);

                    List<Lesson> LessonsList = new List<Lesson>();


                    foreach (var lesson in LessonList)
                    {
                        var LessonExist = "";
                        StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == lesson.Id & x.StudentId == UserId).FirstOrDefault();

                        if (LessonTracking == null)
                        {
                            LessonExist = "No";
                        }
                        else
                        {
                            LessonExist = "Yes";
                        }
                        Lesson lessonobj = new Lesson();
                        lessonobj.LessonId = lesson.Id;
                        lessonobj.LessonName = lesson.Name;
                        lessonobj.OrderBy = Convert.ToInt32(lesson.OrderBy);
                        lessonobj.LessonDuration = Int32.Parse(lesson.DurationMinutes.ToString());
                        lessonobj.LessonExistInTrackingTable = LessonExist;
                        lessonobj.EncryptedID = lesson.EncryptedID;


                        DurationCount = DurationCount + lesson.DurationMinutes ?? 0;

                        LessonsList.Add(lessonobj);
                        Count++;
                        count1++;
                    }

                    List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.OrderBy).ToList();

                    TopicObj.LessonList = OrderByLessons;

                    TopicObj.TotalLessons = Count;
                    TopicObj.TotalLessons1 = count1;
                    TopicObj.TopicDuration = Int32.Parse(DurationCount.ToString());

                    TopicListObj.Add(TopicObj);
                }

                TopicListObj = TopicListObj.Where(x => x.LessonList.Count != 0).ToList();

                return Json(TopicListObj.OrderBy(x => x.Orderby).ToList(), JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                var a = ex.Message;
            }


            return Json("", JsonRequestBehavior.AllowGet);


        }

        public ActionResult MarkChecker(int LessonId)
        {
            string status = "";
            var UserId = User.Identity.GetUserId();
            StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId && x.StudentId == UserId).FirstOrDefault();
            status = "False";
            if (LessonTracking == null || LessonTracking.IsCompleted == null)
            {
                status = "True";

            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Student")]
         [AjaxAuthorize]
        public ActionResult UpdateStudentLessonTracking(int LessonId)
        {
            string status = "";
            var UserId = User.Identity.GetUserId();
            TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime PKTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);

            if (db.StudentLessonTrackings.Where(x => x.LessonId == LessonId && x.StudentId == UserId).Count() > 0)
            {
                StudentLessonTracking slt = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId && x.StudentId == UserId).FirstOrDefault();
                if (slt.IsCompleted == null)
                {
                    DateTime MeetingStartTime = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().StartTime.Value;
                    if (PKTime.Date == MeetingStartTime.Date)
                    {
                        slt.LessonStatus = "Present";
                    }
                    else
                    {
                        slt.LessonStatus = "Late";
                    }
                    slt.IsCompleted = true;
                    slt.StartDate = PKTime;
                    slt.Assignment_Status = "Pending";
                    db.SaveChanges();
                    status = "True";
                }
                else
                {
                    status = "False";
                }
            }
            else
            {
                StudentLessonTracking lessonTracking = new StudentLessonTracking();
                DateTime MeetingStartTime = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().StartTime.Value;
                if (PKTime.Date == MeetingStartTime.Date)
                {
                    lessonTracking.LessonStatus = "Present";
                }
                else
                {
                    lessonTracking.LessonStatus = "Late";
                }

                lessonTracking.LessonId = LessonId;
                lessonTracking.IsCompleted = true;
                //TimeZone time2 = TimeZone.CurrentTimeZone;
                //DateTime test = time2.ToUniversalTime(PKTime);
                //var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                //DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);
                lessonTracking.StartDate = PKTime;
                lessonTracking.StudentId = User.Identity.GetUserId();
                lessonTracking.Assignment_Status = "Pending";
                db.StudentLessonTrackings.Add(lessonTracking);
                if (db.SaveChanges() > 0)
                {
                    status = "True";
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCourseContent(int LessonID)
        {


            var UserId = User.Identity.GetUserId();

            var TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
            var SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

            var SubjectsTopics = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubjectId).ToList();

            List<Topic> TopicListObj = new List<Topic>();

            int Count = 0;
            foreach (var a in SubjectsTopics)
            {
                int count1 = 0;
                Topic TopicObj = new Topic();

                //var list = db.AspnetLessons.Where(x => x.TopicId == a.Id).ToList();

                var UserId1 = User.Identity.GetUserId();

                // var userSessionId = db.AspNetUsers_Session.Where(x => x.UserID == UserId1).FirstOrDefault().SessionID;


                TopicObj.TopicId = a.Id;
                TopicObj.TopicName = a.Name;
                TopicObj.Orderby = Convert.ToInt32(a.OrderBy);


                var today = DateTime.Today;

                var LessonList = (from Lesson in db.AspnetLessons.Where(x => x.Status == true)
                                  join LessonSession in db.Lesson_Session on Lesson.Id equals LessonSession.LessonId
                                  where Lesson.TopicId == a.Id /*&& Lesson.IsActive == true*/ && /*LessonSession.SessionId == userSessionId && */LessonSession.StartDate <= today && today <= LessonSession.DueDate
                                  select Lesson).ToList();

                List<Lesson> LessonsList = new List<Lesson>();

                foreach (var lesson in LessonList)
                {
                    var LessonExist = "";
                    StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == lesson.Id && x.StudentId == UserId).FirstOrDefault();

                    if (LessonTracking == null)
                    {
                        LessonExist = "No";
                    }
                    else
                    {
                        LessonExist = "Yes";
                    }

                    Lesson lessonobj = new Lesson();
                    lessonobj.LessonId = lesson.Id;
                    lessonobj.LessonName = lesson.Name;
                    lessonobj.LessonDuration = Int32.Parse(lesson.DurationMinutes.ToString());
                    lessonobj.LessonExistInTrackingTable = LessonExist;
                    lessonobj.OrderBy = Convert.ToInt32(lesson.OrderBy);

                    LessonsList.Add(lessonobj);
                    Count++;
                    count1++;
                }

                // List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.LessonName).ToList();

                List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.OrderBy).ToList();

                TopicObj.LessonList = OrderByLessons;
                TopicObj.TotalLessons = Count;
                TopicObj.TotalLessons1 = count1;

                TopicListObj.Add(TopicObj);
            }

            // return Json(TopicListObj, JsonRequestBehavior.AllowGet);


            return Json(TopicListObj.OrderBy(x => x.Orderby).ToList(), JsonRequestBehavior.AllowGet);


        }


        public ActionResult StudentAssignment(int LessonID)
        {

            //AspnetStudentAssignment SA  =    db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).FirstOrDefault();
            //    var AssignmentName = "";
            //    var AssignmentDueDate = "";
            //    int AssignmentId =;
            //    if(SA !=null)
            //    {

            //        AssignmentName = SA.FileName;
            //        AssignmentDueDate =  SA.DueDate.ToString();
            //        AssignmentId = SA.Id;
            //    }

            //  new { StudentAssigmentName = AssignmentName, StudentAssignmentDueDate = AssignmentDueDate, StudentAssignmentId = AssignmentId }
            var UserId = User.Identity.GetUserId();

            int StudentId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().Id;

            var a = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).Select(x => new { x.Id, x.FileName, x.DueDate, x.Name, x.TotalMarks, x.Description }).FirstOrDefault();


            var AssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == StudentId).FirstOrDefault();

            var FileName = "";
            var DueDate = "";
            var AssignmentId = "";
            var AssignName = "";
            var TotalMarks = "";
            var AssignDesc = "";
            var AssignmentExist = "No";
            if (a != null)
            {
                FileName = a.FileName;
                AssignName = a.Name;
                AssignmentExist = "Yes";
                TotalMarks = Convert.ToString(a.TotalMarks);
                AssignDesc = a.Description;


                if (a.DueDate != null)
                {

                    DueDate = Convert.ToString(a.DueDate.Value.Date);
                }

                AssignmentId = Convert.ToString(a.Id);

            }

            var TeacherComments = "";
            var SubmittedAssignmentFileName = "Empty";

            var TeacherAssignmentFileName = "Empty";
            var AssignmentSubmissionId = "";

            var ObtainedMarks = "";
            var Resubmission = "No";
            var StudentResubmittedFile = "Empty";
            var TeacherResubmittedFile = "Empty";

            if (AssignmentSubmission != null)
            {
                TeacherComments = AssignmentSubmission.TeacherComments;
                SubmittedAssignmentFileName = AssignmentSubmission.AssignmentFileName;
                AssignmentSubmissionId = Convert.ToString(AssignmentSubmission.Id);
                ObtainedMarks = Convert.ToString(AssignmentSubmission.ObtainedMarks);

                if (AssignmentSubmission.TeacherAssignment != "" || AssignmentSubmission.TeacherAssignment != null)
                {
                    if (AssignmentSubmission.TeacherAssignment != null)
                    {

                        TeacherAssignmentFileName = AssignmentSubmission.TeacherAssignment;
                    }

                }

                if (AssignmentSubmission.ResubmitRequired != null && AssignmentSubmission.ResubmitRequired == true)
                {
                    Resubmission = "Yes";
                }
                if (AssignmentSubmission.StudentResubmittedFile != null)
                {
                    StudentResubmittedFile = AssignmentSubmission.StudentResubmittedFile;
                }
                if (AssignmentSubmission.TeacherResubmittedFile != null)
                {
                    TeacherResubmittedFile = AssignmentSubmission.TeacherResubmittedFile;
                }

            }

            if (FileName == null)
            {
                FileName = "---";
            }

            if (AssignName == null)
            {
                AssignName = "---";
            }

            if (AssignDesc == null)
            {
                AssignDesc = "---";
            }

            return Json(new { TeacherResubmittedFile = TeacherResubmittedFile, StudentResubmittedFile = StudentResubmittedFile, Resubmission = Resubmission, AssignDesc = AssignDesc, TotalMarks = TotalMarks, ObtainedMarks = ObtainedMarks, AssignmentSubmissionId = AssignmentSubmissionId, TeacherAssignmentFileName = TeacherAssignmentFileName, SubmittedAssignmentFileName = SubmittedAssignmentFileName, AssignmentExist = AssignmentExist, StudentAssigmentName = AssignName, FileName = FileName, StudentAssignmentDueDate = DueDate, StudentAssignmentId = AssignmentId, TeacherComments = TeacherComments }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult DownloadFileOfTeacherAssignment(int id, string Name)
        {
          
            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/TeacherSubmittedAssignment/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);

        }
        [Authorize]
        public ActionResult DownloadStudentResubmittedAssignment(int id, string Name)
        {
            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);
            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), Name);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }
        [Authorize]
        public ActionResult DownloadTeacherResubmittedAssignment(int id, string Name)
        {
            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);
            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/TeacherSubmittedAssignment/"), Name);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);
        }



        [Authorize]
        public ActionResult DownloadFileOfStudentAssignment(int id, string Name)
        {

            AspnetStudentAssignmentSubmission studentAssignment = db.AspnetStudentAssignmentSubmissions.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), Name);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), Name);

        }




        public ActionResult ReadingMaterials(int LessonID)
        {

            var StudentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonID).Select(x => new { AttachmentId = x.Id, AttachmentName = x.Name, AttachmentPath = x.Path }).ToList();

            var StudentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == LessonID).Select(x => new { LinkUrl = x.URL }).ToList();


            return Json(new { StuAttachment = StudentAttachments, StuLinks = StudentLinks }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult DownloadFile(int id)
        {

            AspnetStudentAttachment studentAttachment = db.AspnetStudentAttachments.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAttachments/"), studentAttachment.Path);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), studentAttachment.Path);


        }
        [Authorize]
        public ActionResult DownloadFileOfAssignment(string id)
        {
            int idd = Convert.ToInt32(id);

            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Find(idd);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), studentAssignment.FileName);

            return File(filepath, MimeMapping.GetMimeMapping(filepath), studentAssignment.FileName);


        }


        //public ActionResult StudentAssignmentSubmission(int LessonID)
        //{
        //    var IsSubmitted = "";
        //    var UserId1 = User.Identity.GetUserId();

        //    AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();

        //    AspnetStudentAssignmentSubmission StudentAssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == Student.Id).FirstOrDefault();

        //    if (StudentAssignmentSubmission == null)
        //    {
        //        IsSubmitted = "Submit Assignment Successfully";

        //        var File = Request.Files["file"];

        //        var fileName = "";
        //        if (File.ContentLength > 0)
        //        {
        //            fileName = Path.GetFileName(File.FileName);
        //            File.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);

        //        }

        //        AspnetStudentAssignmentSubmission AssignmentSubmission = new AspnetStudentAssignmentSubmission();

        //        int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
        //        int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

        //        //  db.AspnetSubjectTopics.Where(x => x.Id == LessonID);

        //        var Subject = db.GenericSubjects.Where(x => x.Id == SubjectId).FirstOrDefault();

        //        var id = User.Identity.GetUserId();
        //        var UserId = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault().Id;

        //        int StudentId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().Id;
        //        int? ClassId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().ClassId;

        //        var AssignmentDueDate = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).FirstOrDefault().DueDate;


        //        TimeZone time2 = TimeZone.CurrentTimeZone;
        //        DateTime test = time2.ToUniversalTime(DateTime.Now);
        //        var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");


        //        DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);
        //        AssignmentSubmission.LessonId = LessonID;
        //        AssignmentSubmission.TopicId = TopicId;
        //        AssignmentSubmission.SubjectId = SubjectId;
        //        AssignmentSubmission.ClassId = ClassId;
        //        AssignmentSubmission.CourseType = Subject.SubjectType;
        //        AssignmentSubmission.StudentId = StudentId;
        //        AssignmentSubmission.AssignmentSubmittedDate = pakistantime;
        //        AssignmentSubmission.AssignmentDueDate = AssignmentDueDate;
        //        AssignmentSubmission.AssignmentFileName = fileName;

        //        db.AspnetStudentAssignmentSubmissions.Add(AssignmentSubmission);
        //        db.SaveChanges();

        //    }
        //    else
        //    {
        //        IsSubmitted = "Submit Assignment failed, you have already Submited Assignment";
        //    }


        //    StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == LessonID && x.StudentId == UserId1).FirstOrDefault();

        //    if (LessonTracking != null)
        //    {

        //        LessonTracking.Assignment_Status = "Submitted";
        //        db.SaveChanges();

        //    }


        //    return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
        //}// Student Assignment Submission
        [AjaxAuthorize]
        public ActionResult StudentAssignmentSubmission(int LessonID, IEnumerable<HttpPostedFileBase> file)
        {
            var UserId1 = User.Identity.GetUserId();
            int StudentID = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault().Id;
            var StudentAssignmentSubmissionObj = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == StudentID).FirstOrDefault();
            if (StudentAssignmentSubmissionObj == null)
            {
                var IsSubmitted = "";
                AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();
                AspnetStudentAssignmentSubmission StudentAssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == Student.Id).FirstOrDefault();
                if (StudentAssignmentSubmission == null)
                {
                    IsSubmitted = "Submit Assignment Successfully";
                    // var File = Request.Files["file"];	
                    var AllFiles = "";
                    var fileName = "";
                    AspnetStudentAssignmentSubmission AssignmentSubmission = new AspnetStudentAssignmentSubmission();
                    int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
                    var id = User.Identity.GetUserId();
                    var UserId = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault().Id;
                    int StudentId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().Id;
                    var AssignmentDueDate = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).FirstOrDefault().DueDate;
                    TimeZone time2 = TimeZone.CurrentTimeZone;
                    DateTime test = time2.ToUniversalTime(DateTime.Now);
                    var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);
                    AssignmentSubmission.LessonId = LessonID;
                    AssignmentSubmission.TopicId = TopicId;
                    AssignmentSubmission.SubjectId = null;
                    AssignmentSubmission.CourseType = null;
                    AssignmentSubmission.StudentId = StudentId;
                    AssignmentSubmission.AssignmentSubmittedDate = pakistantime;
                    AssignmentSubmission.AssignmentDueDate = AssignmentDueDate;
                    AssignmentSubmission.ResubmitRequired = false;
                    //AssignmentSubmission.AssignmentFileName = AllFiles;	
                    db.AspnetStudentAssignmentSubmissions.Add(AssignmentSubmission);
                    db.SaveChanges();
                    if (file != null)
                    {
                        foreach (var file1 in file)
                        {
                            if (file1 != null && file1.ContentLength > 0)
                            {
                                var name = Path.GetFileNameWithoutExtension(file1.FileName);
                                var ext = Path.GetExtension(file1.FileName);
                                // fileName = name + "_LS_" + LessonID + Student.Id + ext;	
                                fileName = name + "_LS_" + AssignmentSubmission.Id + ext;
                                AllFiles += fileName + "/";
                                file1.SaveAs(Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), fileName));
                            }
                        }
                    }
                    if (AllFiles != "")
                    {
                        AssignmentSubmission.AssignmentFileName = AllFiles;
                        db.SaveChanges();
                    }
                }
                return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var IsSubmitted = "";
                AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();
                AspnetStudentAssignmentSubmission StudentAssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == Student.Id && x.StudentResubmittedFile != null).FirstOrDefault();
                if (StudentAssignmentSubmission == null)
                {
                    IsSubmitted = "Submit Assignment Successfully";
                    // var File = Request.Files["file"];	
                    var AllFiles = "";
                    var fileName = "";
                    // AspnetStudentAssignmentSubmission AssignmentSubmission = new AspnetStudentAssignmentSubmission();	
                    AspnetStudentAssignmentSubmission StudentAssignmentSubmissionToUpdate = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == Student.Id).FirstOrDefault();


                    TimeZone time2 = TimeZone.CurrentTimeZone;
                    DateTime test = time2.ToUniversalTime(DateTime.Now);
                    var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);
                    StudentAssignmentSubmissionToUpdate.ResubmissionDate = pakistantime;
                    //AssignmentSubmission.AssignmentFileName = AllFiles;	
                    //db.AspnetStudentAssignmentSubmissions.Add(AssignmentSubmission);	
                    db.SaveChanges();
                    if (file != null)
                    {
                        foreach (var file1 in file)
                        {
                            if (file1 != null && file1.ContentLength > 0)
                            {
                                var name = Path.GetFileNameWithoutExtension(file1.FileName);
                                var ext = Path.GetExtension(file1.FileName);
                                // fileName = name + "_LS_" + LessonID + Student.Id + ext;	
                                fileName = name + "_LS_" + StudentAssignmentSubmissionToUpdate.Id + ext;
                                AllFiles += fileName + "/";
                                file1.SaveAs(Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), fileName));
                            }
                        }
                    }
                    if (AllFiles != "")
                    {
                        StudentAssignmentSubmissionToUpdate.StudentResubmittedFile = AllFiles;
                        db.SaveChanges();
                    }
                    
                }
                return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
            }
        }// Student Assignment Submission

        public ActionResult StudentAssignmentSubmission1(int LessonID, IEnumerable<HttpPostedFileBase> file)
        {
            var IsSubmitted = "";
            var UserId1 = User.Identity.GetUserId();

            AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();

            AspnetStudentAssignmentSubmission StudentAssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID && x.StudentId == Student.Id).FirstOrDefault();

            if (StudentAssignmentSubmission == null)
            {
                IsSubmitted = "Submit Assignment Successfully";

                var File = Request.Files["file"];

                var fileName = "";
                if (File.ContentLength > 0)
                {
                   var name = Path.GetFileNameWithoutExtension(File.FileName);
                   var ext = Path.GetExtension(File.FileName);
                   fileName = name + "_LS_" + LessonID + Student.Id + ext;
                   File.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);
                 }

                AspnetStudentAssignmentSubmission AssignmentSubmission = new AspnetStudentAssignmentSubmission();

                int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
                var id = User.Identity.GetUserId();
                var UserId = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault().Id;
                int StudentId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().Id;
                var AssignmentDueDate = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).FirstOrDefault().DueDate;


                TimeZone time2 = TimeZone.CurrentTimeZone;
                DateTime test = time2.ToUniversalTime(DateTime.Now);
                var pakistan = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");


                DateTime pakistantime = TimeZoneInfo.ConvertTimeFromUtc(test, pakistan);

                AssignmentSubmission.LessonId = LessonID;
                AssignmentSubmission.TopicId = TopicId;
                AssignmentSubmission.SubjectId = null;
                AssignmentSubmission.CourseType = null;
                AssignmentSubmission.StudentId = StudentId;
                AssignmentSubmission.AssignmentSubmittedDate = pakistantime;
                AssignmentSubmission.AssignmentDueDate = AssignmentDueDate;
                AssignmentSubmission.AssignmentFileName = fileName;

                db.AspnetStudentAssignmentSubmissions.Add(AssignmentSubmission);
                db.SaveChanges();

            }
            else
            {
                IsSubmitted = "Submit Assignment failed, you have already Submited Assignment";
            }


            StudentLessonTracking LessonTracking = db.StudentLessonTrackings.Where(x => x.LessonId == LessonID && x.StudentId == UserId1).FirstOrDefault();

            if (LessonTracking != null)
            {

                LessonTracking.Assignment_Status = "Submitted";
                db.SaveChanges();

            }


            return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
        }// Student Assignment Submission

        [AjaxAuthorize]
        public ActionResult SaveCommentHead(int LessonID, string Title, string Body)
        {

            var id = User.Identity.GetUserId();
            AspnetComment_Head commentHead = new AspnetComment_Head();
            string EncrID = LessonID + Title + Body + id;

            commentHead.EncryptedID = Encrpt.Encrypt(EncrID, true);


            var newString = Regex.Replace(commentHead.EncryptedID, @"[^0-9a-zA-Z]+", "s");

            //string str = newString.Substring(0, 32);


            commentHead.EncryptedID = newString;

            //Comment_Head commentHead = new Comment_Head();
            commentHead.Comment_Head = Title;
            commentHead.CommentBody = Body;
            commentHead.LessonId = LessonID;
            commentHead.CreatedBy = id;

            commentHead.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
            db.AspnetComment_Head.Add(commentHead);
            db.SaveChanges();


            var UserId = User.Identity.GetUserId();
            var UserName = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Name;
            var NotificationObj = new AspNetNotification();
            NotificationObj.Description = UserName + " asked a Question";
            NotificationObj.Subject = "Student Comment ";
            NotificationObj.SenderID = UserId;
            NotificationObj.Time = GetLocalDateTime.GetLocalDateTimeFunction();
            NotificationObj.Url = "/TeacherCommentsOnCourses/CommentsPage1/" + commentHead.Id;
            NotificationObj.NavigateText = "Reply to Student Comment";
            db.AspNetNotifications.Add(NotificationObj);
            db.SaveChanges();


            int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
            int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

            // var AllTeachers = db.Teacher_GenericSubjects.Where(x => x.SubjectId == SubjectId).Select(x => x.TeacherId);

            var AllTeachers = (from lesson in db.AspnetLessons
                               join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                               join employee in db.AspNetEmployees on enrollment.TeacherId equals employee.Id
                               where lesson.Id == LessonID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                               && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                               && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                               select new
                               {
                                   employee.UserId

                               }).Distinct().ToList();


            //var UnionTeachers = AllTeachers.Distinct();

            //var AllEmployeesUserId = from employee in db.AspNetEmployees
            //                         where AllTeachers.Contains(employee.Id)
            //                         select new
            //                         {
            //                             employee.UserId,
            //                         };

            Sea_Entities db2 = new Sea_Entities();
            var EmailParameter = "Comment Posted";
            foreach (var receiver in AllTeachers)
            {
                var Teacher = db.AspNetUsers.Where(x => x.Id == receiver.UserId).FirstOrDefault();

                var notificationRecieve = new AspNetNotification_User();


                notificationRecieve.NotificationID = NotificationObj.Id;
                notificationRecieve.UserID = Convert.ToString(receiver.UserId);
                notificationRecieve.Seen = false;
                db2.AspNetNotification_User.Add(notificationRecieve);
                try
                {

                    if (db2.SaveChanges() > 0)
                    {

                        SendMail(Teacher.Email, "Comment Posted",  EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, Body));

                        //  SendMail_old("shahzad.qasir@theriskadvisors.com", "Comment Posted", "" + EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, Body));

                    }

                }

                catch (Exception ex)
                {


                    var Msg = ex.Message;
                }


            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

        public bool SendMail(string toEmail, string subjeEnumerableDebugViewct, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                string[] EmailList = new string[] { toEmail, "seasupport@theriskadvisors.com" };
                foreach (var item in EmailList)
                {
                    SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                    client.EnableSsl = false;
                    client.Timeout = 100000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    MailMessage mailMessage = new MailMessage(senderEmail, item, subjeEnumerableDebugViewct, emailBody);
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = UTF8Encoding.UTF8;

                    client.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                var logs = new AspNetLog();
                logs.Operation = ex.Message + " -----" + ex.InnerException.Message;
                logs.UserId = User.Identity.GetUserId();
                db.AspNetLogs.Add(logs);
                db.SaveChanges();
                return false;
            }
        }


        public ActionResult AllCommentsHead(int LessonID)
        {
            var AllCommentHead = from commentHead in db.AspnetComment_Head
                                 join user in db.AspNetUsers on commentHead.CreatedBy equals user.Id
                                 where commentHead.LessonId == LessonID
                                 select new
                                 {
                                     CommentHeadId = commentHead.Id,
                                     Title = commentHead.Comment_Head,
                                     Body = commentHead.CommentBody,
                                     LessonId = commentHead.LessonId,
                                     UserName = user.Name,
                                     Date = commentHead.CreationDate,
                                     EncryptedID = commentHead.EncryptedID
                                 };

            return Json(AllCommentHead, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Student")]
        public ActionResult CommentsPage(int? CommentHeadId)
        {
            //var commentHead = db.Comment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault();

            // commentHead
            return RedirectToAction("CommentsPage1", CommentHeadId);

        }


        [Authorize(Roles = "Student")]
        public ActionResult CommentsPage1(string id)
        {
            ViewBag.CommentHeadId = db.AspnetComment_Head.Where(x => x.EncryptedID == id).FirstOrDefault().Id;
            ViewBag.EncryptedID = id;
            int? LessonId = db.AspnetComment_Head.Where(x => x.EncryptedID == id).FirstOrDefault().LessonId;
            ViewBag.LessonId = LessonId;
            ViewBag.LessonEncryptedId = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().EncryptedID;

            return View("Comments");
        }

        public ActionResult GetCommentHead(int CommentHeadId)
        {
            // var commentHead = db.Comment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault();
            var id = User.Identity.GetUserId();

            AspnetComment_Head CommentHeadObj = db.AspnetComment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault();
            var ShowCommentBox = "No";


            if (CommentHeadObj != null)
            {
                if (CommentHeadObj.CreatedBy == id)
                {

                    ShowCommentBox = "Yes";
                }
                else
                {
                    ShowCommentBox = "No";

                }


            }

            var CommentHead = (from commentHead in db.AspnetComment_Head
                               join user in db.AspNetUsers on commentHead.CreatedBy equals user.Id
                               where commentHead.Id == CommentHeadId
                               select new
                               {
                                   CommentHeadId = commentHead.Id,
                                   Title = commentHead.Comment_Head,
                                   Body = commentHead.CommentBody,
                                   LessonId = commentHead.LessonId,
                                   UserName = user.Name,
                                   Date = commentHead.CreationDate,
                               }).FirstOrDefault();



            return Json(new { CommentHead = CommentHead, ShowCommentBox = ShowCommentBox }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommentReply(int LessonId, int CommentHeadId, string UserComment)
        {

            var id = User.Identity.GetUserId();

            int count = db.AspnetComments.Count();
            AspnetComment commentobj = new AspnetComment();

            if (count == 0)
            {
                commentobj.ParentCommentId = null;
                commentobj.Comment = UserComment;
                commentobj.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                commentobj.HeadId = CommentHeadId;
                commentobj.CreatedBy = id;
                db.AspnetComments.Add(commentobj);
                db.SaveChanges();
            }
            else
            {
                int LastId = db.AspnetComments.OrderByDescending(o => o.Id).FirstOrDefault().Id;
                commentobj.ParentCommentId = LastId;
                commentobj.Comment = UserComment;
                commentobj.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                commentobj.HeadId = CommentHeadId;
                commentobj.CreatedBy = id;

                db.AspnetComments.Add(commentobj);
                db.SaveChanges();
            }


            var UserId = User.Identity.GetUserId();
            var UserName = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Name;
            var NotificationObj = new AspNetNotification();
            NotificationObj.Description = UserName + " Replied";
            NotificationObj.Subject = "Student Reply";
            NotificationObj.SenderID = UserId;
            NotificationObj.Time = GetLocalDateTime.GetLocalDateTimeFunction();
            NotificationObj.Url = "/TeacherCommentsOnCourses/CommentsPage1/" + CommentHeadId;
            NotificationObj.NavigateText = "See Student Reply";
            db.AspNetNotifications.Add(NotificationObj);
            db.SaveChanges();


            int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().TopicId;
            int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

            // var AllTeachers = db.Teacher_GenericSubjects.Where(x => x.SubjectId == SubjectId).Select(x => x.TeacherId);

            var AllTeachers = (from lesson in db.AspnetLessons
                               join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                               join employee in db.AspNetEmployees on enrollment.TeacherId equals employee.Id
                               where lesson.Id == LessonId && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                               && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId

                               select new
                               {
                                   employee.UserId

                               }).Distinct().ToList();


            //var UnionTeachers = AllTeachers.Distinct();

            //var AllEmployeesUserId = from employee in db.AspNetEmployees
            //                         where AllTeachers.Contains(employee.Id)
            //                         select new
            //                         {
            //                             employee.UserId,
            //                         };
            var EmailParameter = "Student Replied";

            Sea_Entities db2 = new Sea_Entities();
            foreach (var receiver in AllTeachers)
            {

                var Teacher = db.AspNetUsers.Where(x => x.Id == receiver.UserId).FirstOrDefault();

                var notificationRecieve = new AspNetNotification_User();
                notificationRecieve.NotificationID = NotificationObj.Id;
                notificationRecieve.UserID = Convert.ToString(receiver.UserId);
                notificationRecieve.Seen = false;
                db2.AspNetNotification_User.Add(notificationRecieve);
                try
                {
                    //  db2.SaveChanges();

                    if (db2.SaveChanges() > 0)
                    {

                        // SendMail(Teacher.Email, "Student Replied", "" + EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, UserComment), EmailParameter);

                        //  SendMail_old("shahzad.qasir@theriskadvisors.com", "Comment Posted", "" + EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, Body));

                    }

                }

                catch (Exception ex)
                {


                    var Msg = ex.Message;
                }


            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllComments(int CommentHeadId)
        {
            var Comments = from comment in db.AspnetComments
                           join user in db.AspNetUsers on comment.CreatedBy equals user.Id
                           where comment.HeadId == CommentHeadId
                           select new
                           {
                               CommentName = comment.Comment,
                               UserName = user.Name,
                               Date = comment.CreationDate,
                           };


            return Json(Comments, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Student")]
        public ActionResult StudentTests()
        {


            return View();
        }

        public ActionResult AllTestList()
        {
            var ID = User.Identity.GetUserId();

            var TestSubjects = (from enrollment in db.AspNetStudent_Enrollments
                                join x in db.TestSubjects on enrollment.SessionId equals x.BranchClassSectionId
                                where x.AspNetCours.Id == enrollment.AspNetClass_Courses.CourseId
                                select new { x.Id, x.Title, x.Description, x.StartDate, x.EndTime, x.FileName, x.AspNetCours.Name, x.TotalMarks }).Distinct();

            //var TestSubjects = db.TestSubjects.Select(x => new { x.Id, x.Title, x.Description, x.StartDate, x.EndTime, x.FileName, x.AspNetCours.Name, x.TotalMarks });

            return Json(TestSubjects, JsonRequestBehavior.AllowGet);

        }
        [Authorize(Roles = "Student")]
        public ActionResult StudentSubmitTest(int id)
        {
            var UserId = User.Identity.GetUserId();

            int StudentId = db.AspNetStudents.Where(x => x.AspNetUser.Id == UserId).FirstOrDefault().Id;

            var StudentTestSubjects = db.StudentTestSubjects.Where(x => x.TestSubjectId == id && x.StudentId == StudentId).FirstOrDefault();

            ViewBag.id = id;

            if (StudentTestSubjects != null)
            {
                ViewBag.ObtainedMarks = StudentTestSubjects.ObtainedMarks;

                ViewBag.TotalMarks = StudentTestSubjects.TestSubject.TotalMarks;

                ViewBag.StudentSubmittedFile = StudentTestSubjects.StudentSubmittedTestName;
                ViewBag.TeacherSubmittedFile = StudentTestSubjects.TeacherSubmittedTestName;
                ViewBag.TeacherComment = StudentTestSubjects.TeacherComments;
                ViewBag.TestStudentSubjectId = StudentTestSubjects.Id;

            }
            else
            {
                ViewBag.TotalMarks = null;
                ViewBag.ObtainedMarks = null;
                ViewBag.StudentSubmittedFile = null;
                ViewBag.TeacherSubmittedFile = null;
                ViewBag.TeacherComment = null;
                ViewBag.TestStudentSubjectId = null;


            }


            return View(); //Json(StudentTestSubjects);
        }
        [Authorize]
        public ActionResult DownloadTest(int id)
        {

            TestSubject SubjectTest = db.TestSubjects.Where(x => x.Id == id).FirstOrDefault();

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentTests/"), SubjectTest.FileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), SubjectTest.FileName);

        }
        [Authorize]
        public ActionResult DownloadTeacherSubmittedFile(int id)
        {

            StudentTestSubject StudentTestSubject = db.StudentTestSubjects.Where(x => x.Id == id).FirstOrDefault();

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/TeacherSubmittedTest/"), StudentTestSubject.TeacherSubmittedTestName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), StudentTestSubject.TeacherSubmittedTestName);

        }





        //public ActionResult StudentTestSubmission()
        //{



        //}

        public ActionResult StudentTestSubmission(int id)
        {
            var IsSubmitted = "";
            var UserId1 = User.Identity.GetUserId();

            AspNetStudent Student = db.AspNetStudents.Where(x => x.UserId == UserId1).FirstOrDefault();

            TestSubject TestSubjectObj = db.TestSubjects.Where(x => x.Id == id).FirstOrDefault();

            StudentTestSubject StudentSubjectTest = db.StudentTestSubjects.Where(x => x.StudentId == Student.Id && x.TestSubjectId == id).FirstOrDefault();

            if (StudentSubjectTest == null)
            {
                IsSubmitted = "Submit Test Successfully";

                var File = Request.Files["file"];

                var fileName = "";

                StudentTestSubject StudentSubjectTestToAdd = new StudentTestSubject();

                StudentSubjectTestToAdd.StudentId = Student.Id;
                StudentSubjectTestToAdd.StudentSubDate = GetLocalDateTime.GetLocalDateTimeFunction();
                // StudentSubjectTestToAdd.StudentSubmittedTestName = fileName;
                StudentSubjectTestToAdd.TestSubjectId = id;

                DateTime? LocalDateTime = GetLocalDateTime.GetLocalDateTimeFunction();

                if (LocalDateTime <= TestSubjectObj.EndTime)
                {

                    StudentSubjectTestToAdd.Status = "Sumbitted";

                }
                else
                {
                    StudentSubjectTestToAdd.Status = "OverDue";

                }




                db.StudentTestSubjects.Add(StudentSubjectTestToAdd);
                db.SaveChanges();

                if (File.ContentLength > 0)
                {
                    //fileName = Path.GetFileName(File.FileName);
                    //File.SaveAs(Server.MapPath("~/Content/StudentSubmittedTest/") + fileName);

                    var name = Path.GetFileNameWithoutExtension(File.FileName);

                    var ext = Path.GetExtension(File.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);

                    fileName = name + "_TS_" + StudentSubjectTestToAdd.Id + ext;
                    File.SaveAs(Server.MapPath("~/Content/StudentSubmittedTest/") + fileName);
                    StudentSubjectTestToAdd.StudentSubmittedTestName = fileName;
                    db.SaveChanges();
                }

            }
            else
            {
                IsSubmitted = "Submit Assignment failed, you have already Submited Assignment";

            }




            return Json(IsSubmitted, JsonRequestBehavior.AllowGet);
        }


        public class Lesson
        {
            public int LessonId { get; set; }
            public string LessonName { get; set; }
            public int LessonDuration { get; set; }

            public string LessonExistInTrackingTable { get; set; }
            public string EncryptedID { get; set; }

            public int OrderBy { get; set; }
            public int LessonCount { get; set; }
        }

        public class Topic
        {
            public int TopicId { get; set; }
            public string TopicName { get; set; }
            public int TopicDuration { get; set; }
            public int Orderby { get; set; }
            public int TotalLessons { get; set; }
            public int TotalLessons1 { get; set; }
            public List<Lesson> LessonList { get; set; }

        }
    }
}