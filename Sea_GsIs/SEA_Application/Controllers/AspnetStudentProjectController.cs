using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Text;
namespace SEA_Application.Controllers
{
    public class AspnetStudentProjectController : Controller
    {

        private Sea_Entities db = new Sea_Entities();
        private string TeacherID;
        // GET: AspnetStudentProject

        public AspnetStudentProjectController()
        {

            TeacherID = Convert.ToString(System.Web.HttpContext.Current.Session["TeacherID"]);
        }
        // GET: AspNetProject
        public ActionResult Index()
        {
            if (User.IsInRole("Teacher"))
            {
                ViewBag.ClassID = new SelectList(db.AspNetSubjects.Where(x => x.TeacherID == TeacherID).Select(x => x.AspNetClass).Distinct(), "Id", "ClassName");
            }
            else
            {
                ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");
            }

            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");
            return View();
        }

        public ActionResult GetSubjectsByClass()
        {
            var SubjectsByClass = db.GenericSubjects.ToList().Select(x => new { x.Id, x.SubjectName });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(SubjectsByClass);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);
        }


        public ActionResult AllBranches()
        {
            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            select new
                            {
                                branch.Id,
                                branch.Name,

                            }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);

            return Content(status);

        }



        public ActionResult ClassesByBranch(int BranchId)
        {
            // var BranchClasses  =  db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });

            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           where (branchclasssubject.BranchId == BranchId)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);
        }

        public ActionResult SectionByClasses(int ClassId)
        {


            var Sections = (from section in db.AspNetSections
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on section.Id equals branchclasssubject.SectionId
                            where (branchclasssubject.ClassId == ClassId)
                            select new
                            {
                                section.Id,
                                section.Name,

                            }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);

            return Json(status, JsonRequestBehavior.AllowGet);


        }

        public ActionResult SubjectsByClass(int SectionId)
        {
            // var BranchClasses  =  db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });

            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            where (branchclasssubject.SectionId == SectionId)
                            select new
                            {
                                subject.Id,
                                subject.Name,

                            }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Subjects);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);
        }

        public ActionResult GetSubjects(int BranchId, int ClassId, int SubjectId, int SectionId)
        {

            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SubjectId == SubjectId && x.SectionId == SectionId).FirstOrDefault();

            if (Generic != null)
            {

                var id = Generic.Id;

                var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == id).ToList().Select(x => new { x.Id, x.Name });



                string AllTopics = Newtonsoft.Json.JsonConvert.SerializeObject(Topics);
                return Content(AllTopics);
            }


            return Json("", JsonRequestBehavior.AllowGet);


        }



        public ActionResult GetSessions()
        {

            var AllSessions = db.AspNetSessions.ToList().Select(x => new { x.Id });

            string Sessions = Newtonsoft.Json.JsonConvert.SerializeObject(AllSessions);
            return Content(Sessions);
        }

        public ActionResult GetLession(int TopID)
        {
            var TopicList = db.AspnetLessons.Where(x => x.TopicId == TopID).ToList().Select(x => new { x.Id, x.Name });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(TopicList);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);

        }
        public ActionResult GetTopic(int SubID)
        {
            var TopicList = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubID).ToList().Select(x => new { x.Id, x.Name });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(TopicList);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);

        }



        public ViewResult Project_Submission()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");

            return View();
        }

        // GET: AspNetProject/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetProject aspNetProject = db.AspNetProjects.Find(id);
            if (aspNetProject == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.AspNetSubjects.Where(x => x.TeacherID == TeacherID).Select(x => x.AspNetClass).Distinct(), "Id", "ClassName");
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");
            return View(aspNetProject);
        }

        // GET: AspNetProject/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.AspNetBranches.Distinct(), "Id", "Name");


            // var teacherClass = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == TeacherID).ToList();
            // List<Class_Subject> ClassSubject = new List<Class_Subject>();
            var tid = User.Identity.GetUserId();
            var student = db.AspNetStudents.Where(x => x.UserId == tid).FirstOrDefault();



           // var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student.Id).Select(x => new { Name = x.AspNetClass_Courses.AspNetClass.Name, Id = x.AspNetClass_Courses.AspNetClass.Id }).Distinct().ToList();

            //ViewBag.ClassID = new SelectList(db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).Select(x=> x.AspNetClass_Courses.AspNetClass.Name).Distinct());
            //ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName");
            return View();
        }

        public JsonResult GetChildren()
        {
            string ParentID = User.Identity.GetUserId();
            var children = (from child in db.AspNetUsers
                            join parent_children in db.AspNetParent_Child on child.Id equals parent_children.ChildID
                            where parent_children.ParentID == ParentID
                            select new { child.Id, child.Name }).FirstOrDefault().Id;

            return Json(children, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClasseList()
        {
            ViewBag.BranchID = new SelectList(db.AspNetBranches.Distinct(), "Id", "Name");
            var tid = User.Identity.GetUserId();
            var student = db.AspNetStudents.Where(x => x.UserId == tid).FirstOrDefault();
           // var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetStudent_Enrollments.Where(x => x.StudentId == student.Id).Select(x => new { Name = x.AspNetClass_Courses.AspNetClass.Name, Id = x.AspNetClass_Courses.AspNetClass.Id }).Distinct().ToList();
            var status = Newtonsoft.Json.JsonConvert.SerializeObject(List);
            return Content(status);
        }
        private List<int> GetAdministratedBranchIds()
        {
            var currentlyLoggedInUser = User.Identity.GetUserId();
            return db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
        }
        public ActionResult GetSubjectList1()
        {
            var childid = GetChildren().Data;
            int studentd = db.AspNetStudents.Where(x => x.UserId == childid).FirstOrDefault().Id;
            //var branchIds = GetAdministratedBranchIds();
            //var enrollments = db.AspNetStudent_Enrollments
            //    .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId) && enrollment.StudentId == 1022)
            //    .ToList();

            var branchIds = GetAdministratedBranchIds();
            var enrollments = db.AspNetStudent_Enrollments
                .Where(x => x.StudentId == studentd).Select(x => new { Name = x.AspNetClass_Courses.AspNetCours.Name, Id = x.AspNetClass_Courses.AspNetCours.Id })
                .ToList();
            var status = Newtonsoft.Json.JsonConvert.SerializeObject(enrollments);
            return Content(status);
            //   return View(status);
        }

        public ActionResult GetSubjectList(int classid)
        {
            ViewBag.BranchID = new SelectList(db.AspNetBranches.Distinct(), "Id", "Name");
            var tid = User.Identity.GetUserId();
            var student = db.AspNetStudents.Where(x => x.UserId == tid).FirstOrDefault();

          //  var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetStudent_Enrollments.Where(x => x.AspNetClass_Courses.AspNetClass.Id == classid).Select(x => new { Name = x.AspNetClass_Courses.AspNetCours.Name, Id = x.AspNetClass_Courses.AspNetCours.Id }).ToList();
            var status = Newtonsoft.Json.JsonConvert.SerializeObject(List);
            return Content(status);
        }

        // POST: AspNetProject/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AspNetProject aspNetProject)
        {
            HttpPostedFileBase file = Request.Files["attachment"];
            if (ModelState.IsValid)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Homework"), fileName);
                    file.SaveAs(path);
                    aspNetProject.FileName = fileName;
                }
                else
                {
                    aspNetProject.FileName = "-/-";
                }
                if (db.AspNetProjects.Where(x => x.FileName == aspNetProject.FileName).Count() > 0)
                {

                    TempData["Error"] = "Document already exists kindly change document name";
                    return RedirectToAction("Create");
                }
                else

                {
                    var tid = User.Identity.GetUserId();
                    aspNetProject.CreatedBy = tid;
                    //  aspNetProject.BranchID = 5;
                    aspNetProject.CreationDate = DateTime.Now;
                    db.AspNetProjects.Add(aspNetProject);
                    db.SaveChanges();
                }
                int ProjectID = db.AspNetProjects.Max(x => x.Id);
                List<string> StudentIDs = db.AspNetStudent_Subject.Where(s => s.SubjectID == aspNetProject.SubjectID).Select(s => s.StudentID).ToList();
                foreach (var item in StudentIDs)
                {
                    AspNetStudent_Project student_project = new AspNetStudent_Project();
                    student_project.StudentID = item;
                    student_project.ProjectID = ProjectID;
                    student_project.SubmissionStatus = false;
                    student_project.SubmittedFileName = "-/-";
                    db.AspNetStudent_Project.Add(student_project);
                    db.SaveChanges();
                }
                /////////////////////////////////////////////////NOTIFICATION/////////////////////////////////////

                //var NotificationObj = new AspNetNotification();
                //NotificationObj.Description = aspNetProject.Description;
                //NotificationObj.Subject = aspNetProject.Title;
                //NotificationObj.SenderID = User.Identity.GetUserId();
                //NotificationObj.Time = DateTime.Now;
                //NotificationObj.Url = "/AspNetProject/Details/" + aspNetProject.Id;
                //db.AspNetNotifications.Add(NotificationObj);
                //db.SaveChanges();

                //var NotificationID = db.AspNetNotifications.Max(x => x.Id);
                //var students = db.AspNetStudent_Project.Where(sp => sp.ProjectID == aspNetProject.Id).Select(x => x.StudentID).ToList();

                //var users = new List<String>();

                //foreach (var item in students)
                //{
                //    var parentID = db.AspNetParent_Child.Where(x => x.ChildID == item).Select(x => x.ParentID).FirstOrDefault();
                //    users.Add(parentID);
                //}

                //var allusers = users.Union(students);

                //foreach (var receiver in allusers)
                //{
                //    var notificationRecieve = new AspNetNotification_User();
                //    notificationRecieve.NotificationID = NotificationID;
                //    notificationRecieve.UserID = Convert.ToString(receiver);
                //    notificationRecieve.Seen = false;
                //    db.AspNetNotification_User.Add(notificationRecieve);
                //    db.SaveChanges();
                //}

                /////////////////////////////////////Email/////////////////////////////////////////

                //var subject = db.AspNetSubjects.Where(x => x.Id == aspNetProject.SubjectID).Select(x => x.SubjectName).FirstOrDefault();
                //var StudentEmail = db.AspNetStudent_Project.Where(sp => sp.ProjectID == aspNetProject.Id).Select(x => x.StudentID).ToList();
                //var StudentRoll = db.AspNetStudent_Project.Where(sp => sp.ProjectID == aspNetProject.Id).Select(x => x.AspNetUser.UserName).ToList();
                //string[] studentRollList = new string[StudentRoll.Count];
                //int c = 0;
                //foreach (var item in StudentRoll)
                //{
                //    studentRollList[c] = item;
                //    c++;
                //}
                //var StudentName = db.AspNetStudent_Project.Where(sp => sp.ProjectID == aspNetProject.Id).Select(x => x.AspNetUser.Name).ToList();

                //string[] studentNamelist = new string[StudentName.Count];
                //int i = 0;
                //foreach (var item in StudentName)
                //{
                //    studentNamelist[i] = item;
                //    i++;
                //}

                //var Users = new List<String>();
                //foreach (var item in StudentEmail)
                //{
                //    Users.Add(db.AspNetParent_Child.Where(x => x.ChildID == item).Select(x => x.ParentID).FirstOrDefault());
                //}

                //Message start
                //var classe = db.AspNetClasses.Where(p => p.Id == aspNetHomework.ClassId).FirstOrDefault();
                // Utility obj = new Utility();
                //  obj.SMSToOffitialsp("Dear Principal, Project has been assigned. IPC NGS Preschool, Aziz Avenue, Lahore.");
                //  obj.SMSToOffitialsa("Dear Admin, Project has been assigned. IPC NGS Preschool, Aziz Avenue, Lahore.");
                //   AspNetMessage oob = new AspNetMessage();
                //   oob.Message = "Dear Parents, The thematic project has been assigned to your child on portal. IPC NGS Preschool, Aziz Avenue, Lahore.";// Title : " + aspNetProject.Title + ", For discription login to Portal please  -
                //  obj.SendSMS(oob, Users);
                //Message end


                //List<string> EmailList = new List<string>();
                //foreach (var sender in Users)
                //{
                //    EmailList.Add(db.AspNetUsers.Where(x => x.Id == sender).Select(x => x.Email).FirstOrDefault());
                //}
                //var j = 0;
                //foreach (var toEmail in EmailList)
                //{
                //    try
                //    {
                //        NotificationObj.Subject = studentNamelist[j] + "(" + studentRollList[j] + ") " + " Subject:" + subject + " Title:" + aspNetProject.Title;
                //        j++;
                //        string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                //        string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                //        SmtpClient client = new SmtpClient("smtpout.secureserver.net", 25);
                //        client.EnableSsl = false;
                //        client.Timeout = 100000;
                //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //        client.UseDefaultCredentials = false;
                //        client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                //        MailMessage mailMessage = new MailMessage(senderEmail, toEmail, NotificationObj.Subject, NotificationObj.Description);
                //        mailMessage.CC.Add(new MailAddress(senderEmail));
                //        mailMessage.IsBodyHtml = true;
                //        mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                //        client.Send(mailMessage);
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //}

                return RedirectToAction("Index");
            }

            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName", aspNetProject.SubjectID);
            TempData["Create"] = "Project has been created";
            return View(aspNetProject);
        }

        // GET: AspNetProject/Edit/5`
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetProject aspNetProject = db.AspNetProjects.Find(id);
            if (aspNetProject == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName", aspNetProject.SubjectID);
            return View(aspNetProject);
        }

        // POST: AspNetProject/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,PublishDate,DueDate,FileName,AcceptSubmission,SubjectID")] AspNetProject aspNetProject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetProject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");
            ViewBag.SubjectID = new SelectList(db.AspNetSubjects, "Id", "SubjectName", aspNetProject.SubjectID);
            return View(aspNetProject);
        }

        // GET: AspNetProject/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetProject aspNetProject = db.AspNetProjects.Find(id);
            if (aspNetProject == null)
            {
                return HttpNotFound();
            }
            return View(aspNetProject);
        }

        // POST: AspNetProject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetProject aspNetProject = db.AspNetProjects.Find(id);
            db.AspNetProjects.Remove(aspNetProject);
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

        public JsonResult ProjectBySubject(int subjectID)
        {
            //  db.Configuration.ProxyCreationEnabled = false;
            //db.Configuration.LazyLoadingEnabled = false;
            // var Projects = db.AspNetProjects.Where(x=> x.)
            //var projects = (from project in db.AspNetProjects
            //                join t4 in db.AspNetSubjects on project.SubjectID equals t4.Id


            //                where project.SubjectID == subjectID
            //                select project).ToList();

            //  Get
            List<GetProjectList_Result> list = new List<GetProjectList_Result>();

            list = db.GetProjectList(subjectID.ToString()).ToList();
            var tid = User.Identity.GetUserId();
            var projects = db.AspNetProjects.Where(x => x.CreatedBy == tid && x.SubjectID == subjectID).ToList();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StudentProjectBySubject(int subjectID)
        {


            var projects = db.AspNetProjects.Where(x => x.SubjectID == subjectID).ToList();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public FileResult downloadProjectFile(int id)
        {
            AspNetProject Project = db.AspNetProjects.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/App_Data/Projects/"), Project.FileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), Project.FileName);

        }

        public FileResult downloadStudentSubmittedFile(int id)
        {
            AspNetStudent_Project Student_Project = db.AspNetStudent_Project.Find(id);

            var filepath = System.IO.Path.Combine(Server.MapPath("~/App_Data/ProjectsSubmission/"), Student_Project.SubmittedFileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), Student_Project.SubmittedFileName);

        }

        public JsonResult ProjectBySubjectAcceptSubmission(int subjectID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            var projects = (from project in db.AspNetProjects
                            where project.SubjectID == subjectID && project.AcceptSubmission == true
                            select project).ToList();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SubmissionByProject(int ProjectID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var projects = (from projectsubmission in db.AspNetStudent_Project
                            where projectsubmission.ProjectID == ProjectID && projectsubmission.AspNetUser.StatusId != 1
                            select new { projectsubmission, projectsubmission.AspNetUser.Name, projectsubmission.AspNetProject.AcceptSubmission }).ToList();

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

    }
}