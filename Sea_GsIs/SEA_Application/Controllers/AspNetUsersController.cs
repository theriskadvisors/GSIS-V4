using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{

    public class AspNetUsersController : Controller
    {

        private Sea_Entities db = new Sea_Entities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AspNetUsersController()
        {

        }

        public AspNetUsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: AspNetUsers
        public ActionResult Index()
        {
            var aspNetUsers = db.AspNetUsers.Include(a => a.AspNetStatu);
            return View(aspNetUsers.ToList());
        }
        public ViewResult ParentIndex()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.data = "Parent";

            return View(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Parent") && x.StatusId != 2).ToList());
        }
        public class Students
        {
            public string Name { set; get; }
            public string RollNo { set; get; }
            public string Email { set; get; }
            public string PhoneNo { set; get; }
            public string ClassName { set; get; }
        }

        public class StudentAndSubjects
        {
            public List<Students> Students { set; get; }
            public List<Subjects> Subjects { set; get; }
        }

        public class Subjects
        {
            public int id { set; get; }
            public string Subjectname { set; get; }
            public string Classname { set; get; }
        }
        [HttpGet]
        public JsonResult SubjectsParentsByClass(int id)
        {
            //var StSu = new StudentAndSubjects();
            //var subject = db.AspNetSubjects.Where(r => r.ClassID == id).OrderByDescending(r => r.Id).Select(x => new { x.Id, x.SubjectName, x.AspNetClass.Name }).ToList();
            //List<Subjects> sub = new List<Subjects>();

            //foreach (var item in subject)
            //{
            //    Subjects s = new Subjects();
            //    s.id = item.Id;
            //    s.Subjectname = item.SubjectName;
            //    s.Classname = item.ClassName;
            //    sub.Add(s);
            //}

            //StSu.Subjects = sub;

            //StSu.Students = new List<Students>();

            //if (id != 0)
            //{
            //    var Students = (from parent in db.AspNetParent_Child
            //                    join student in db.AspNetStudents on parent.ChildID equals student.StudentID
            //                    where student.ClassID == id
            //                    select new { parent.AspNetUser1.Name, parent.AspNetUser1.UserName, parent.AspNetUser1.Email, childName = parent.AspNetUser.Name }).Distinct().ToList();

            //    foreach (var item in Students)
            //    {
            //        var student = new Students();
            //        student.Name = item.Name;
            //        student.Email = item.Email;
            //        student.RollNo = item.UserName;
            //        student.ClassName = item.childName;
            //        StSu.Students.Add(student);
            //    }
            //}

            if (id != 0)
            {
                var Students = (from parent in db.AspNetParent_Child
                                join student in db.AspNetStudents on parent.ChildID equals student.UserId
                                where (student.ClassId == id)
                                select new { parent.AspNetUser1.Name, parent.AspNetUser1.UserName, parent.AspNetUser1.Email, childName = parent.AspNetUser.Name }).Distinct().ToList();
                return Json(Students, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Students = (from parent in db.AspNetParent_Child
                                join student in db.AspNetStudents on parent.ChildID equals student.UserId

                                select new { parent.AspNetUser1.Name, parent.AspNetUser1.UserName, parent.AspNetUser1.Email, childName = parent.AspNetUser.Name }).Distinct().ToList();

                return Json(Students, JsonRequestBehavior.AllowGet);
            }

            //    foreach (var item in Students)
            //    {
            //        var student = new Students();
            //        student.Name = item.Name;
            //        student.Email = item.Email;
            //        student.RollNo = item.UserName;
            //        student.ClassName = item.childName;
            //        StSu.Students.Add(student);
            //    }
            //}

            //  return Json(Students, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ParentRegister()
        {
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");
            return View();
        }
        public JsonResult Email(string Email)
        {
            int count;
            try
            {
                var user = db.AspNetUsers.Where(x => x.Email == Email);
                if (user.Count() > 0)
                {
                    count = 1;
                }
                else
                {
                    count = 0;
                }
            }
            catch
            {
                count = 0;
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }
        public class check
        {
            public int count { get; set; }
            public string by { get; set; }
        }


        public JsonResult GetUserName(string userName)
        {
            check Check = new check();
            Check.count = 0;
            try
            {
                var user = db.AspNetUsers.Where(x => x.UserName == userName);
                if (user.Count() > 0)
                {
                    Check.count = 1;
                    Check.by = user.Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
                }
                else
                {
                    Check.count = 0;
                }
            }
            catch
            {
                Check.count = 0;
            }

            return Json(Check, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ParentRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbTransaction = db.Database.BeginTransaction();
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    IEnumerable<string> selectedstudents = Request.Form["StudentID"].Split(',');
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Name = model.Name, PhoneNumber = Request.Form["fatherCell"] };
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {

                        AspNetParent parent = new AspNetParent();
                        parent.FatherName = Request.Form["fatherName"];
                        parent.FatherCellNo = Request.Form["fatherCell"];
                        parent.FatherEmail = Request.Form["fatherEmail"];
                        parent.FatherOccupation = Request.Form["fatherOccupation"];
                        parent.FatherEmployer = Request.Form["fatherEmployer"];
                        parent.MotherName = Request.Form["motherName"];
                        parent.MotherCellNo = Request.Form["motherCell"];
                        parent.MotherEmail = Request.Form["motherEmail"];
                        parent.MotherOccupation = Request.Form["motherOccupation"];
                        parent.MotherEmployer = Request.Form["motherEmployer"];
                        parent.UserID = user.Id;
                        db.AspNetParents.Add(parent);
                        db.SaveChanges();


                        foreach (var item in selectedstudents)
                        {
                            AspNetParent_Child par_stu = new AspNetParent_Child();
                            par_stu.ChildID = item;
                            par_stu.ParentID = user.Id;
                            db.AspNetParent_Child.Add(par_stu);
                            db.SaveChanges();
                        }

                        var roleStore = new RoleStore<IdentityRole>(context);
                        var roleManager = new RoleManager<IdentityRole>(roleStore);

                        var userStore = new UserStore<ApplicationUser>(context);
                        var userManager = new UserManager<ApplicationUser>(userStore);
                        userManager.AddToRole(user.Id, "Parent");

                        dbTransaction.Commit();

                    }
                    else
                    {
                        dbTransaction.Dispose();
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.InnerException);
                    dbTransaction.Dispose();
                    return View(model);
                }
            }
            string Error = "Parent successfully saved";
            return RedirectToAction("ParentIndex", "AspNetUsers", new { Error });
        }

        public ActionResult ParentEdit(string id)
        {
            var parent = db.AspNetParents.Where(x => x.UserID == id).Select(x => x).FirstOrDefault();
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            ViewBag.Id = aspNetUser.Id;
            ViewBag.Parent = parent;
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");

            var childs = db.AspNetParent_Child.Where(x => x.ParentID == id).Select(x => x.ChildID).ToList();
            List<int> Classes = new List<int>();
            foreach (var item in childs)
            {
                var subjects = db.AspNetStudent_Subject.Where(x => x.StudentID == item).FirstOrDefault();

                var classId = (int)db.AspNetSubjects.Where(x => x.Id == subjects.SubjectID).Select(x => x.ClassID).FirstOrDefault();
                Classes.Add(classId);
            }

            var Children = db.AspNetParent_Child.Where(x => x.ParentID == id).Select(x => x.ChildID).ToList();

            ViewBag.Classes = Classes;
            ViewBag.Children = Children;
            return View(aspNetUser);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ParentEdit()
        {
            string id = Request.Form["Id"];
            var parent = db.AspNetParents.Where(x => x.UserID == id).Select(x => x).FirstOrDefault();
            var user = db.AspNetUsers.Where(x => x.Id == id).Select(x => x).FirstOrDefault();

            IEnumerable<string> selectedstudents = Request.Form["StudentID"].Split(',');
            user.UserName = Request.Form["UserName"];
            user.Name = Request.Form["Name"];
            user.Email = Request.Form["Email"];

            parent.FatherName = Request.Form["fatherName"];
            parent.FatherCellNo = Request.Form["fatherCell"];
            parent.FatherEmail = Request.Form["fatherEmail"];
            parent.FatherOccupation = Request.Form["fatherOccupation"];
            parent.FatherEmployer = Request.Form["fatherEmployer"];
            parent.MotherName = Request.Form["motherName"];
            parent.MotherCellNo = Request.Form["motherCell"];
            parent.MotherEmail = Request.Form["motherEmail"];
            parent.MotherOccupation = Request.Form["motherOccupation"];
            parent.MotherEmployer = Request.Form["motherEmployer"];

            var childs = db.AspNetParent_Child.Where(x => x.ParentID == user.Id).ToList();
            foreach (var item in childs)
            {
                db.AspNetParent_Child.Remove(item);
            }

            db.SaveChanges();

            db.AspNetUsers.Where(p => p.Id == id).FirstOrDefault().PhoneNumber = Request.Form["fatherCell"];
            db.SaveChanges();
            foreach (var item in selectedstudents)
            {
                AspNetParent_Child par_stu = new AspNetParent_Child();
                par_stu.ChildID = item;
                par_stu.ParentID = user.Id;
                db.AspNetParent_Child.Add(par_stu);
            }

            db.SaveChanges();

            return RedirectToAction("ParentIndex", "AspNetUsers");
        }

        public ViewResult ParentDetail(string UserName)
        {
            var parent = db.AspNetParents.Where(x => x.AspNetUser.UserName == UserName).Select(x => x).FirstOrDefault();
            AspNetUser aspNetUser = db.AspNetUsers.Where(x => x.UserName == UserName).Select(x => x).FirstOrDefault();
            ViewBag.Id = aspNetUser.Id;
            ViewBag.Parent = parent;
            ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");

            var childs = db.AspNetParent_Child.Where(x => x.ParentID == parent.UserID).Select(x => x).ToList();
            List<int> Classes = new List<int>();
            foreach (var item in childs)
            {
                //      var subjects = db.AspNetStudent_Subject.Where(x => x.StudentID == item.ChildID).FirstOrDefault();

                //    var classId = (int)db.AspNetSubjects.Where(x => x.Id == subjects.SubjectID).Select(x => x.ClassID).FirstOrDefault();
                var classId = (int)db.AspNetStudents.Where(x => x.UserId == item.ChildID).Select(x => x.ClassId).FirstOrDefault();

                Classes.Add(classId);
            }

            var Children = db.AspNetParent_Child.Where(x => x.ParentID == parent.UserID).Select(x => x.ChildID).ToList();

            ViewBag.Classes = Classes;
            ViewBag.Children = Children;
            return View(aspNetUser);
        }
        [HttpGet]
        public JsonResult StudentsByClass(int bdoIds)
        {
            try
            {
                //List<int?> ids = new List<int?>();
                //foreach (var item in bdoIds)
                //{
                //    int a = Convert.ToInt32(item);
                //    ids.Add(a);
                //}

                var aIDs = db.AspNetParent_Child.Select(r => r.ChildID);
                var students = (from student in db.AspNetStudents.AsEnumerable().Where(x => x.ClassId == bdoIds)
                                select new { student.AspNetUser.Id, student.Name, student.AspNetUser.UserName }).ToList();


                //var students = (from student in db.AspNetUsers.AsEnumerable()
                //                join student_subject in db.AspNetStudent_Subject on student.Id equals student_subject.StudentID
                //                join subject in db.AspNetSubjects on student_subject.SubjectID equals subject.Id
                //                where ids.Contains(subject.ClassID)
                //                orderby subject.ClassID ascending
                //                select new { student.Id, student.Name, student.UserName }).Distinct().OrderBy(x => x.Name).ToList();

                // var diff = aIDs.Except(students);


                return Json(students, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }


        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name");
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Name,StatusId")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name", aspNetUser.StatusId);
            return View(aspNetUser);
        }

        public ActionResult StudentsAndTeachersOfBranch()
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;
            if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();
            }
            else
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            List<TeacherAndStudents> TeacherAndStudentsList = new List<TeacherAndStudents>();

            var students = (from stdnt in db.AspNetStudents
                            join usr in db.AspNetUsers
                            on stdnt.UserId equals usr.Id
                            where stdnt.UserId == usr.Id && usr.StatusId != 2 && stdnt.BranchId == branchId //&& stdnt.BranchId == branchId
                            select new { usr.Id, Name = stdnt.Name + "(" + usr.UserName + ")", stdnt.RollNo, stdnt.CellNo, usr.Image }).OrderBy(x => x.Name).ToList();

            //var TopicList = db.AspnetSubjectTopics.Where(x => x.SubjectId == SubID).ToList().Select(x => new { x.Id, x.Name });
            //  var AllTeachers = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Teacher")).Select(x => new { x.Id, Name = x.Name + " (" + x.UserName + ") " }).ToList();

            var TeacherUsersIds = db.AspNetTeacher_Enrollments.Where(x => x.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetBranch.Id == branchId).Distinct().Select(x => x.AspNetEmployee.UserId).ToList();
            var AllBranchTeachers = db.AspNetUsers.Where(x => TeacherUsersIds.Contains(x.Id)).Select(x => new { x.Id, Name = x.Name + " (" + x.UserName + ") " }).ToList();


            foreach (var item in students)
            {
                TeacherAndStudentsList.Add(new TeacherAndStudents { Id = item.Id, Name = item.Name });
            }
            foreach (var item in AllBranchTeachers)
            {
                TeacherAndStudentsList.Add(new TeacherAndStudents { Id = item.Id, Name = item.Name });
            }

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(TeacherAndStudentsList);


            return Content(status);
        }

        public class TeacherAndStudents
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }

        public ActionResult ResetPassword()
        {


            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(string UserId, string Password, string CnfmPass)
        {

            var success = "No";
            var user = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();
            string Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, Code, Password);
            if (result.Succeeded)
            {
                success = "Yes";

                var RuffDataToUpdate = db.ruffdatas.Where(x => x.UserName == user.UserName).FirstOrDefault();

                if(RuffDataToUpdate == null)
                {
                    ruffdata ruffdata = new ruffdata();

                    ruffdata.Name = user.Name;
                    ruffdata.UserName = user.UserName;
                    ruffdata.Password = Password;
                    ruffdata.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();

                    db.ruffdatas.Add(ruffdata);
                    db.SaveChanges();

                }
                else
                {
                    RuffDataToUpdate.Password = Password;
                    db.SaveChanges();

                }
            }




            return Json(new { success = success, UserName = user.Name+" ("+user.UserName+")" }, JsonRequestBehavior.AllowGet);
        }


        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name", aspNetUser.StatusId);
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Name,StatusId")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name", aspNetUser.StatusId);
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
