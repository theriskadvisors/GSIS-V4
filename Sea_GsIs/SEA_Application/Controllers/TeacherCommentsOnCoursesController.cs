using Microsoft.AspNet.Identity;
using SEA_Application.CustomModel;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class TeacherCommentsOnCoursesController : Controller
    {

        private Sea_Entities db = new Sea_Entities();
        //  int SessionID = Convert.ToInt32(SessionIDStaticController.GlobalSessionID);
      //  db.AspNetSessions.Where(x => x.StatusId == 1).FirstOrDefault().Year;

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult AllSubjectsOfTeacher()
        {


            List<Class_Subject> ClassSubject = new List<Class_Subject>();
            var tid = User.Identity.GetUserId();


            var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var enrolmentlist = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).ToList();


            foreach (var item in enrolmentlist)
            {
                //var classcourse = db.AspNetClass_Courses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                Class_Subject cs = new Class_Subject();
                cs.Class = item.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Name;
                cs.Section = item.AspNetBranchClass_Sections.AspNetSection.Name;
                cs.Subject = item.AspNetClass_Courses.AspNetCours.Name;
                cs.SubjectId = item.AspNetClass_Courses.AspNetCours.Id;
                
                //cs.Class = classcourse.AspNetClass.Name;
                //cs.Subject = classcourse.AspNetCours.Name;
                //int BC_ID = db.AspNetBranch_Class.Where(x => x.ClassId == classcourse.ClassId).FirstOrDefault().Id;
                //int SectionID =  db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BC_ID).FirstOrDefault().SectionId;
                //cs.Section = db.AspNetSections.Where(x => x.Id == SectionID).FirstOrDefault().Name;

                ClassSubject.Add(cs);
            }


                //var SubjectofCurrentSessionTeacher = from subject in db.GenericSubjects
                //                                     join TeacherSubject in db.Teacher_GenericSubjects on subject.Id equals TeacherSubject.SubjectId
                //                                     join employee in db.AspNetEmployees on TeacherSubject.TeacherId equals employee.Id
                //                                     where employee.UserId == userID
                //                                     select new
                //                                     {
                //                                         subject.Id,
                //                                         subject.SubjectName,
                //                                     };

                return Json(ClassSubject, JsonRequestBehavior.AllowGet);


        }

        public class Class_Subject
        {
            public string Class { get; set; }
            public string Subject { get; set; }
            public string Section { get; set; }

            public int SubjectId { get; set; }
        }

        public ActionResult SubjectTopics(int id)
        {


            ViewBag.SubjectId = id;


            return View();
        }
        [Authorize]
        public ActionResult StudentLessons(int id)
        {


            ViewBag.LessonID = id;

            ViewBag.SubjectId = 20;

            return View();
        }

        public ActionResult GetLessonViewContent(int LessonID)
        {
            var Lesson = db.AspnetLessons.Where(x => x.Id == LessonID).Select(x => new { LessonId = x.Id, LessonName = x.Name, LessonVideo = x.Video_Url, LessonDescription = x.Description, ContentType = x.ContentType, MeetingLink = x.MeetingLink, LessonImg = x.LessonIMG }).FirstOrDefault();

            return Json(Lesson, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSubjectTopicsAndLessons(int LessonId)
        {

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

          int? TopicId =   db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault().TopicId;

          int? GenericId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;



            var SubjectsTopics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == GenericId).ToList();


            List<Topic> TopicListObj = new List<Topic>();

            int Count = 0;
            foreach (var a in SubjectsTopics)
            {
                int count1 = 0;
                Topic TopicObj = new Topic();

                var list = db.AspnetLessons.Where(x => x.TopicId == a.Id).ToList();

                TopicObj.TopicId = a.Id;
                TopicObj.TopicName = a.Name;

                List<Lesson> LessonsList = new List<Lesson>();
                foreach (var lesson in list)
                {
                    Lesson lessonobj = new Lesson();
                    lessonobj.LessonId = lesson.Id;
                    lessonobj.LessonName = lesson.Name;
                    lessonobj.LessonDuration = Int32.Parse(lesson.DurationMinutes.ToString());

                    LessonsList.Add(lessonobj);
                    Count++;
                    count1++;
                }


                List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.LessonName).ToList();
                TopicObj.LessonList = OrderByLessons;

                TopicObj.TotalLessons = Count;
                TopicObj.TotalLessons1 = count1;

                TopicListObj.Add(TopicObj);
            }


            // return Json(TopicListObj, JsonRequestBehavior.AllowGet);
            return Json(TopicListObj.OrderBy(x => x.TopicName).ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCourseContent(int LessonID)
        {

            var TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
            var SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

            var SubjectsTopics = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubjectId).ToList();

            List<Topic> TopicListObj = new List<Topic>();

            int Count = 0;
            foreach (var a in SubjectsTopics)
            {
                int count1 = 0;
                Topic TopicObj = new Topic();

                var list = db.AspnetLessons.Where(x => x.TopicId == a.Id).ToList();

                TopicObj.TopicId = a.Id;
                TopicObj.TopicName = a.Name;

                List<Lesson> LessonsList = new List<Lesson>();


                foreach (var lesson in list)
                {
                    Lesson lessonobj = new Lesson();
                    lessonobj.LessonId = lesson.Id;
                    lessonobj.LessonName = lesson.Name;
                    lessonobj.LessonDuration = Int32.Parse(lesson.DurationMinutes.ToString());

                    LessonsList.Add(lessonobj);
                    Count++;
                    count1++;
                }

                List<Lesson> OrderByLessons = LessonsList.OrderBy(x => x.LessonName).ToList();

                TopicObj.LessonList = OrderByLessons;
                TopicObj.TotalLessons = Count;
                TopicObj.TotalLessons1 = count1;

                TopicListObj.Add(TopicObj);
            }


            //  return Json(TopicListObj, JsonRequestBehavior.AllowGet);

            return Json(TopicListObj.OrderBy(x => x.TopicName).ToList(), JsonRequestBehavior.AllowGet);

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


            var a = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonID).Select(x => new { x.Id, x.FileName, x.DueDate, x.Name, x.Description }).FirstOrDefault();

            var AssignmentSubmission = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonID).FirstOrDefault();

            var FileName = "";
            var DueDate = "";
            var AssignmentId = "";
            var AssignName = "";
            var AssignDesc = "";
            if (a != null)
            {
                FileName = a.FileName;
                AssignName = a.Name;
                AssignDesc = a.Description;

                if (a.DueDate != null)
                {

                    DueDate = Convert.ToString(a.DueDate.Value.Date);
                }

                AssignmentId = Convert.ToString(a.Id);


            }

            var TeacherComments = "";
            if (AssignmentSubmission != null)
            {
                TeacherComments = AssignmentSubmission.TeacherComments;
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



            return Json(new { AssignDesc= AssignDesc, StudentAssigmentName = AssignName, FileName = FileName, StudentAssignmentDueDate = DueDate, StudentAssignmentId = AssignmentId, TeacherComments = TeacherComments }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReadingMaterials(int LessonID)
        {

            var StudentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonID).Select(x => new { AttachmentId = x.Id, AttachmentName = x.Name, AttachmentPath = x.Path }).ToList();

            var StudentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == LessonID).Select(x => new { LinkUrl = x.URL }).ToList();


            return Json(new { StuAttachment = StudentAttachments, StuLinks = StudentLinks }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(int id)
        {

            AspnetStudentAttachment studentAttachment = db.AspnetStudentAttachments.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAttachments/"), studentAttachment.Path);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), studentAttachment.Path);


        }
        public ActionResult DownloadFileOfAssignment(string id)
        {
            int idd = Convert.ToInt32(id);
            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Find(idd);
            var filepath = System.IO.Path.Combine(Server.MapPath("~/Content/StudentAssignments/"), studentAssignment.FileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), studentAssignment.FileName);

        }
        public ActionResult StudentAssignmentSubmission(int LessonID)
        {

            var File = Request.Files["file"];

            var fileName = "";
            if (File.ContentLength > 0)
            {
                fileName = Path.GetFileName(File.FileName);
                File.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);

            }

            AspnetStudentAssignmentSubmission AssignmentSubmission = new AspnetStudentAssignmentSubmission();

            int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
            int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;

            //  db.AspnetSubjectTopics.Where(x => x.Id == LessonID);

            var Subject = db.AspNetSubjects.Where(x => x.Id == SubjectId).FirstOrDefault();

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
            AssignmentSubmission.SubjectId = SubjectId;
            AssignmentSubmission.ClassId = Subject.ClassID;
          //  AssignmentSubmission.CourseType = Subject.CourseType;
            AssignmentSubmission.StudentId = StudentId;
            AssignmentSubmission.AssignmentSubmittedDate = pakistantime;
            AssignmentSubmission.AssignmentDueDate = AssignmentDueDate;
            AssignmentSubmission.AssignmentFileName = fileName;

            db.AspnetStudentAssignmentSubmissions.Add(AssignmentSubmission);
            db.SaveChanges();


            return Json("Submitted Successfully", JsonRequestBehavior.AllowGet);
        }// Student Assignment Submission
        public ActionResult SaveCommentHead(int LessonID, string Title, string Body)
        {

            var id = User.Identity.GetUserId();
            AspnetComment_Head commentHead = new AspnetComment_Head();

            //Comment_Head commentHead = new Comment_Head();
            commentHead.Comment_Head = Title;
            commentHead.CommentBody = Body;
            commentHead.LessonId = LessonID;
            commentHead.CreatedBy = id;
            commentHead.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
            db.AspnetComment_Head.Add(commentHead);
            db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
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
                                 };



            return Json(AllCommentHead, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CommentsPage(int? CommentHeadId)
        {
            //var commentHead = db.Comment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault();

            // commentHead
            return RedirectToAction("CommentsPage1", CommentHeadId);

        }

        public ActionResult CommentsPage1(int id)
        {
            ViewBag.CommentHeadId = id;

            ViewBag.LessonId = db.AspnetComment_Head.Where(x => x.Id == id).FirstOrDefault().LessonId;

            return View("Comments");
        }

        public ActionResult GetCommentHead(int CommentHeadId)
        {
            // var commentHead = db.Comment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault();

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



            return Json(CommentHead, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommentReply(int CommentHeadId, string UserComment)
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

            var CommentHeadEncryptedId = db.AspnetComment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault().EncryptedID;

            var UserNameLog = User.Identity.Name;
            AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);

            var UserId = User.Identity.GetUserId();
            var UserName = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault().Name;
            var NotificationObj = new AspNetNotification();
            NotificationObj.Description = "Teacher " + UserName + " Replied";
            NotificationObj.Subject = "Reply To Student Comment";
            NotificationObj.SenderID = UserId;
            NotificationObj.Time = GetLocalDateTime.GetLocalDateTimeFunction();
            //  NotificationObj.Url = "/StudentCourses/CommentsPage1/" + CommentHeadId;
            NotificationObj.NavigateText = "See Teacher Reply";
            NotificationObj.Url = "/StudentCourses/CommentsPage1/" + CommentHeadEncryptedId;
            try
            {

            db.AspNetNotifications.Add(NotificationObj);

            db.SaveChanges();


            }

            catch(Exception ex )
            {
                    var a = ex.Message;
            }
            //int? LessonID = db.AspnetComment_Head.Where(x => x.Id == CommentHeadId).FirstOrDefault().LessonId;
            //int? TopicId = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault().TopicId;
            //int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;


            //var AllStudents = db.Student_GenericSubjects.Where(x => x.GenericSubjectId == SubjectId).Select(x => x.StudentId);

            //var DistinctStudents = AllStudents.Distinct();

            //foreach (var receiver in DistinctStudents)
            //{
            var receiver = db.AspnetComment_Head.Where(x => x.Id == CommentHeadId).Select(x => x.CreatedBy).FirstOrDefault();

            var EmailParameter = "Comment Posted";

            Sea_Entities db2 = new Sea_Entities();


            var notificationRecieve = new AspNetNotification_User();
            notificationRecieve.NotificationID = NotificationObj.Id;
            notificationRecieve.UserID = Convert.ToString(receiver);
            notificationRecieve.Seen = false;
            db2.AspNetNotification_User.Add(notificationRecieve);
            db2.SaveChanges();

            //if (db2.SaveChanges() > 0)
            //{

            ////    SendMail(Teacher.Email, "Teacher Replied", "" + EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, UserComment), EmailParameter);

            //    //  SendMail_old("shahzad.qasir@theriskadvisors.com", "Comment Posted", "" + EmailDesign.CommentsEmailTemplate(Teacher.Name, UserName, Body));

            //}

            // }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public bool SendMail(string toEmail, string subjeEnumerableDebugViewct, string emailBody, string EmailParameter)
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

                    MailMessage mailMessage = new MailMessage(senderEmail, item, "GSIS VLEP " + EmailParameter, emailBody);
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





        public class Lesson
        {
            public int LessonId { get; set; }
            public string LessonName { get; set; }
            public int LessonDuration { get; set; }
            public int OrderBy { get; set; }
            public int LessonCount { get; set; }
        }

        public class Topic
        {
            public int TopicId { get; set; }
            public string TopicName { get; set; }
            public int TopicDuration { get; set; }

            public int TotalLessons { get; set; }
            public int TotalLessons1 { get; set; }
            public List<Lesson> LessonList { get; set; }

        }







    }
}