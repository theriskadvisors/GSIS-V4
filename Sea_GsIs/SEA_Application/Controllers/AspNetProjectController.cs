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
    public class AspNetProjectController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        // int SessionID = Int32.Parse(SessionIDStaticController.GlobalSessionID);

        private string TeacherID;

        public AspNetProjectController()
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
            var ID = User.Identity.GetUserId();

            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            if (UserRole == "Branch_Admin" || UserRole == "Branch_Principal")
            {
                var BranchId = db.AspNetEmployees.Where(x => x.UserId == ID).FirstOrDefault().BranchId;
                var Branches = db.AspNetBranches.Where(x => x.Id == BranchId).Select(x => new { x.Id, x.Name }).ToList();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);
                return Content(status);
            }

            else
            {

                var Branches = (from branch in db.AspNetBranches
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                                where enrollment.AspNetEmployee.UserId == ID
                                select new
                                {
                                    branch.Id,
                                    branch.Name,
                                }).Distinct();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);
                return Content(status);
            }

        }

        public ActionResult ClassesByBranch(int BranchId)
        {
            // var BranchClasses  =  db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });

            var ID = User.Identity.GetUserId();

            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            if (UserRole == "Branch_Admin" || UserRole == "Branch_Principal")
            {
                var Classes = (from classs in db.AspNetClasses
                               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                               where (branchclasssubject.BranchId == BranchId)
                               select new
                               {
                                   classs.Id,
                                   classs.Name,
                               }).Distinct();

                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
                return Content(status);
            }


            else
            {
                var Classes = (from classs in db.AspNetClasses
                               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                               join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                               where (branchclasssubject.BranchId == BranchId
                               && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id == BranchId
                               && enrollment.AspNetEmployee.UserId == ID)
                               select new
                               {
                                   classs.Id,
                                   classs.Name,
                               }).Distinct();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
                return Content(status);
            }



            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            //  return Content(status);
        }

        public ActionResult SectionByClasses(int ClassId, int BranchId)
        {
            var ID = User.Identity.GetUserId();

            //var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID).Select(x => new { x.AspNetBranchClass_Sections.AspNetSection.Id, x.AspNetBranchClass_Sections.AspNetSection.Name }).Distinct();

            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            if (UserRole == "Branch_Admin" || UserRole == "Branch_Principal")
            {

                var Sections = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.ClassId == ClassId && x.AspNetBranch_Class.BranchId == BranchId).Select(x => new { x.AspNetSection.Id, x.AspNetSection.Name }).Distinct(); //  ClassId == ClassId).Select(x => new
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);

                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == ClassId && x.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == BranchId).Select(x => new
                {
                    Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                    Name = x.AspNetBranchClass_Sections.AspNetSection.Name
                }).Distinct();

                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);
                return Json(status, JsonRequestBehavior.AllowGet);
            }

            //var Sections = (from section in db.AspNetSections
            //                join branchclasssubject in db.AspnetGenericBranchClassSubjects on section.Id equals branchclasssubject.AspNetSection.Id
            //                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetSection.Id equals enrollment.AspNetBranchClass_Sections.AspNetSection.Id
            //                where (branchclasssubject.ClassId == ClassId && enrollment.AspNetEmployee.UserId == ID)
            //                select new
            //                {
            //                    section.Id,
            //                    section.Name,

            //                }).Distinct();

        }


        public ActionResult SubjectsByClass(int BranchId, int ClassId, int SectionId)
        {
            // var BranchClasses  =  db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });

            var ID = User.Identity.GetUserId();

            //var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID).Select(x => new { x.AspNetBranchClass_Sections.AspNetSection.Id, x.AspNetBranchClass_Sections.AspNetSection.Name }).Distinct();

            var UserRole = db.GetUserRoleById(ID).FirstOrDefault();

            if (UserRole == "Branch_Admin" || UserRole == "Branch_Principal")
            {

                var Subjects = (from subject in db.AspNetCourses
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                                where (branchclasssubject.SectionId == SectionId && branchclasssubject.BranchId == BranchId && branchclasssubject.ClassId == ClassId)
                                select new
                                {
                                    subject.Id,
                                    subject.Name,
                                }).Distinct();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Subjects);
                // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
                return Content(status);

            }
            else
            {
                // var ID = User.Identity.GetUserId();
                var Subjects = (from subject in db.AspNetCourses
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                                where (branchclasssubject.SectionId == SectionId && branchclasssubject.BranchId == BranchId && branchclasssubject.ClassId == ClassId
                                && enrollment.AspNetBranchClass_Sections.SectionId == SectionId && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == BranchId
                                && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == ClassId
                                && enrollment.AspNetEmployee.UserId == ID)
                                select new
                                {
                                    subject.Id,
                                    subject.Name,
                                }).Distinct();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(Subjects);
                // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
                return Content(status);

            }

        }

        public ActionResult SubjectsByMultiSections(int[] SectionIds, int BranchId, int ClassId)
        {
            var ID = User.Identity.GetUserId();

            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                            // where (branchclasssubject.SectionId == SectionId && enrollment.AspNetEmployee.UserId == ID)
                            where (SectionIds.Contains(branchclasssubject.SectionId.Value) && branchclasssubject.BranchId == BranchId && branchclasssubject.ClassId == ClassId

                             && SectionIds.Contains(enrollment.AspNetBranchClass_Sections.SectionId) && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == BranchId
                             && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == ClassId
                             && enrollment.AspNetEmployee.UserId == ID)


                            //   where (branchclasssubject.SectionId == SectionId && branchclasssubject.BranchId == BranchId && branchclasssubject.ClassId == ClassId
                            //   && enrollment.AspNetBranchClass_Sections.SectionId == SectionId && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == BranchId
                            // && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == ClassId



                            select new
                            {
                                subject.Id,
                                subject.Name,
                                // Name = branchclasssubject.AspNetCours.Name + " - " +branchclasssubject.AspNetSection.Name,  
                            }).Distinct();
            //    string status = Newtonsoft.Json.JsonConvert.SerializeObject(Subjects);
            return Json(Subjects, JsonRequestBehavior.AllowGet);
            //return Content(status);


        }
        public ActionResult GetSubjectsByMultiValues(int BranchId, int ClassId, int SubjectId, int[] SectionIds)
        {

            //    var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SubjectId == SubjectId && x.SectionId == SectionId).FirstOrDefault();

            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == BranchId && x.ClassId == ClassId && x.SubjectId == SubjectId && SectionIds.Contains(x.SectionId.Value)).ToList();

            // if(Generic != null)

            if (Generic.Count() != 0)
            {

                // var id = Generic.Id;

                var GenericIds = Generic.Select(x => x.Id).ToList();

                var Topics = db.AspnetSubjectTopics.Where(x => GenericIds.Contains(x.GenericBranchClassSubjectId.Value)).ToList().Select(x => new { x.Id, Name = x.Name + "-" + x.AspnetGenericBranchClassSubject.AspNetSection.Name });

                //                string AllTopics = Newtonsoft.Json.JsonConvert.SerializeObject(Topics);

                return Json(Topics, JsonRequestBehavior.AllowGet);

                //              return Content(AllTopics);
            }

            return Json("", JsonRequestBehavior.AllowGet);

            //  return Json("", JsonRequestBehavior.AllowGet);


        }


        public ActionResult AllClasses_LP()
        {
            var ID = User.Identity.GetUserId();
            var BranchID = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
                            where enrollment.AspNetEmployee.UserId == ID
                            select new
                            {
                                branch.Id,

                            }).Distinct().FirstOrDefault().Id;

            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                           where (branchclasssubject.BranchId == BranchID && enrollment.AspNetEmployee.UserId == ID)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Classes);
            return Content(status);
        }

        public ActionResult BranchClassSectionStudents(int BranchId, int ClassId, int SectionId)
        {
            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;

            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == SectionId).FirstOrDefault().Id;

            var AllStudents = (from enrollment in db.AspNetStudent_Enrollments.Where(x => x.SectionId == BranchClassSectionId)
                               join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                               where student.AspNetUser.StatusId != 2
                               select new
                               {
                                   student.Id,
                                   student.Name,

                               }).Distinct();

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(AllStudents);
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


        public ActionResult GetSubjectsForLessonPlan(int BranchId, int ClassId, int SubjectId, int SectionId)
        {
            var ID = User.Identity.GetUserId();
            //var BranchId = (from branch in db.AspNetBranches
            //                join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
            //                join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId
            //                where enrollment.AspNetEmployee.UserId == ID

            //                select new
            //                {
            //                    branch.Id,

            //                }).Distinct().FirstOrDefault().Id;
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

        public ActionResult AllTeachersList()
        {
            //var TopicList = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubID).ToList().Select(x => new { x.Id, x.Name });
            var AllTeachers = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Teacher")).Select(x => new { x.Id, Name = x.Name + " (" + x.UserName+") " }).ToList();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(AllTeachers);

            return Content(status);
        }
        public ActionResult AllBranchClassList()
        {

            List<AllBranchClass> AllBranchClasses = new List<AllBranchClass>();

            var AllBranches = db.AspNetBranches.ToList();
            var AllClasses = db.AspNetClasses.ToList();



            foreach (var branch in AllBranches)
            {
                var BranchClasses = db.AspNetBranch_Class.Where(x => x.BranchId == branch.Id).ToList();

                foreach (var branchClass in BranchClasses)
                {
                    AllBranchClass obj = new AllBranchClass();

                    obj.BranchClassId = branchClass.Id;
                    obj.BranchClassName = branchClass.AspNetBranch.Name + "-" + branchClass.AspNetClass.Name;

                    AllBranchClasses.Add(obj);

                }


            }

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(AllBranchClasses);

            return Content(status);

        }



        public class AllBranchClass
        {
            public int BranchClassId { get; set; }
            public string BranchClassName { get; set; }
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


            var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).Select(x => new { Name = x.AspNetClass_Courses.AspNetClass.Name, Id = x.AspNetClass_Courses.AspNetClass.Id }).Distinct().ToList();

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

            var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetTeacher_Enrollments.Where(x => x.TeacherId == teacherid).Select(x => new { Name = x.AspNetClass_Courses.AspNetClass.Name, Id = x.AspNetClass_Courses.AspNetClass.Id }).Distinct().ToList();
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

            var teacherid = db.AspNetEmployees.Where(x => x.UserId == tid).Select(x => x.Id).FirstOrDefault();
            var List = db.AspNetTeacher_Enrollments.Where(x => x.AspNetClass_Courses.AspNetClass.Id == classid).Select(x => new { Name = x.AspNetClass_Courses.AspNetCours.Name, Id = x.AspNetClass_Courses.AspNetCours.Id }).ToList();
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
