using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace SEA_Application.Controllers
{
    [Authorize(Roles = "Branch_Admin,Super_Admin,Branch_Principal")]
    public class AspNetEmployeesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private Sea_Entities db = new Sea_Entities();

        public AspNetEmployeesController()
        {

        }

        public AspNetEmployeesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        ////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public JsonResult SubjectsByClass(int[] id)
        {
            List<SubjectClass> cs = new List<SubjectClass>();
            db.Configuration.ProxyCreationEnabled = false;
            foreach (var itemid in id)
            {
                var sub = db.AspNetClass_Courses.Where(r => r.ClassId == itemid).ToList();
                foreach (var item in sub)
                {
                    var c = db.AspNetCourses.Where(x => x.Id == item.CourseId).FirstOrDefault();
                    var s = db.AspNetClasses.Where(x => x.Id == item.ClassId).FirstOrDefault();
                    SubjectClass cours = new SubjectClass();
                    cours.id = item.Id;
                    cours.Subject = c.Name;
                    cours.Class = s.Name;
                    cs.Add(cours);
                }
            }
            //var ssss=(from ccs in db.AspNetClass_Courses
            //join c in db.AspNetCourses on ccs.CourseId equals c.Id
            //where ccs.ClassId == id
            //select new { c.Name,c.Id }).ToList();

            ViewBag.Subjects = cs;
           // ViewBag.Subjects = sub;
            return Json(cs, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Position,DateAvailable,Name,BirthDate,NationalityId,ReligionId,GenderId,CellNo,Landline,SpouseName,SpouseHighestDegree,SpouseOccupation,GrossSalary,BasicSalary,MedicalAllowance,Accomodation,ProvidedFund,Tax,EOP,Salary,JoiningDate,BranchId,Illness,Address,Spouse_Address,File,UserId")] AspNetEmployee aspNetEmployee)
        {
           // IEnumerable<string> selectedsubjects = Request.Form["subjects"].Split(',');

            var Position = db.AspNetEmployeePositions.Where(x => x.Id == aspNetEmployee.Position).Select(x => x.PositionName).FirstOrDefault();
            var Password = Request.Form["Password"];
            var FName = Request.Form["FName"];
            var MName = Request.Form["MName"];
            var LName = Request.Form["LName"];
            var FullName = FName + " " + MName + " " + LName;
            if (Position == "Teacher")
            {
             ApplicationDbContext context = new ApplicationDbContext();
            var user = new ApplicationUser { UserName = Request.Form["UserName"], Email = Request.Form["Email"], Name = FullName, PhoneNumber = Request.Form["cellNo"] };
            var result = await UserManager.CreateAsync(user, Password);

                AspNetUser teacher_user = db.AspNetUsers.Where(x => x.UserName == user.UserName).FirstOrDefault();
                teacher_user.Name = user.Name;
                teacher_user.UserName = user.UserName;
                teacher_user.Email = user.Email;
                teacher_user.PasswordHash = user.PasswordHash;
                teacher_user.StatusId = 1;
                teacher_user.PhoneNumber = Request.Form["cellNo"];

                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Teaching Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.UserId = user.Id;
                aspNetEmployee.Name = FullName;
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole(user.Id, "Teacher");
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    char[] charArray = FName.ToCharArray();
                    var fletter = charArray[0].ToString();
                    var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                    AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetEmployee.UserId).FirstOrDefault();
                    student.Image = image;
                    db.SaveChanges();
                    //foreach (var item in selectedsubjects)
                    //{
                    //    var Clscoursid = db.AspNetClass_Courses.Where(x=>x.Id==Int32.Parse(item)).FirstOrDefault();

                    //    AspNetTeacher_Enrollments stu_sub = new AspNetTeacher_Enrollments();
                    //    stu_sub.TeacherId = aspNetEmployee.Id;
                    //    stu_sub.CourseId = Clscoursid.Id;
                    //    stu_sub.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                    //    var BCid = db.AspNetBranch_Class.Where(x => x.BranchId == aspNetEmployee.BranchId && x.ClassId == Clscoursid.ClassId).Select(x => x.Id).FirstOrDefault();
                    //    stu_sub.SectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid).Select(x => x.Id).FirstOrDefault();
                    //    db.AspNetTeacher_Enrollments.Add(stu_sub);
                    //    db.SaveChanges();
                    //}
                    string Error = "Teacher successfully saved.";
                    return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });
                }
            }
            else if(Position=="Accountant")
            {
                ApplicationDbContext context = new ApplicationDbContext();

                var user = new ApplicationUser { UserName = Request.Form["UserName"], Email = Request.Form["Email"], Name = FullName, PhoneNumber = Request.Form["cellNo"] };
                var result = await UserManager.CreateAsync(user, Password);

                AspNetUser accountant_user = db.AspNetUsers.Where(x => x.UserName == user.UserName).FirstOrDefault();
                accountant_user.Name = user.Name;
                accountant_user.UserName = user.UserName;
                accountant_user.Email = user.Email;
                accountant_user.StatusId = 1;
                accountant_user.PhoneNumber = Request.Form["cellNo"];

                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Management Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.UserId = user.Id;
                aspNetEmployee.Name = FullName;
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole(user.Id, "Accountant");
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    char[] charArray = FName.ToCharArray();
                    var fletter = charArray[0].ToString();
                    var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                    AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetEmployee.UserId).FirstOrDefault();
                    student.Image = image;
                    db.SaveChanges();
                    string Error = "Accountant successfully saved.";
                    return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });
                }
            }
            else if (Position == "Supervisor")
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var user = new ApplicationUser { UserName = Request.Form["UserName"], Email = Request.Form["Email"], Name = FullName, PhoneNumber = Request.Form["cellNo"] };
                var result = await UserManager.CreateAsync(user, Password);

                AspNetUser accountant_user = db.AspNetUsers.Where(x => x.UserName == user.UserName).FirstOrDefault();
                accountant_user.Name = user.Name;
                accountant_user.UserName = user.UserName;
                accountant_user.Email = user.Email;
                accountant_user.PasswordHash = user.PasswordHash;
                accountant_user.StatusId = 1;
                accountant_user.PhoneNumber = Request.Form["cellNo"];

                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Management Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.UserId = user.Id;
                aspNetEmployee.Name = FullName;
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole(user.Id, "Accountant");
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    char[] charArray = FName.ToCharArray();
                    var fletter = charArray[0].ToString();
                    var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                    AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetEmployee.UserId).FirstOrDefault();
                    student.Image = image;
                    db.SaveChanges();
                    string Error = "Accountant successfully saved.";
                    return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });
                }
            }
            else if(Position=="Branch Admin")
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var user = new ApplicationUser { UserName = Request.Form["UserName"], Email = Request.Form["Email"], Name = FullName, PhoneNumber = Request.Form["cellNo"] };
                var result = await UserManager.CreateAsync(user, Password);
                try
                {
                 AspNetUser Barnchadmin_user = db.AspNetUsers.Where(x => x.UserName == user.UserName).FirstOrDefault();
                Barnchadmin_user.StatusId = 1;

           
                    aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Directive Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.UserId = user.Id;
                aspNetEmployee.Name = FullName;
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole(user.Id, "Branch_Admin");
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    AspNetBranch_Admins br_ad = new AspNetBranch_Admins();
                    br_ad.AdminId = user.Id;
                    br_ad.BranchId = aspNetEmployee.BranchId??default(int);
                    db.AspNetBranch_Admins.Add(br_ad);
                    db.SaveChanges();
                        char[] charArray = FName.ToCharArray();
                        var fletter = charArray[0].ToString();
                        var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                        AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetEmployee.UserId).FirstOrDefault();
                        student.Image = image;
                        db.SaveChanges();
                        string Error = "Branch Admin successfully saved.";
                    return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });
                   }
                   

                }
                catch (Exception e)
                {

                    return RedirectToAction("Create", "AspNetEmployees");
                }
            }
            else if(Position== "Branch Principal")
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var user = new ApplicationUser { UserName = Request.Form["UserName"], Email = Request.Form["Email"], Name = FullName, PhoneNumber = Request.Form["cellNo"] };
                var result = await UserManager.CreateAsync(user, Password);

                AspNetUser branchprincipal_user = db.AspNetUsers.Where(x => x.UserName == user.UserName).FirstOrDefault();
                branchprincipal_user.Name = user.Name;
                branchprincipal_user.UserName = user.UserName;
                branchprincipal_user.Email = user.Email;
                branchprincipal_user.PasswordHash = user.PasswordHash;
                branchprincipal_user.StatusId = 1;
                branchprincipal_user.PhoneNumber = Request.Form["cellNo"];

                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Directive Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.UserId = user.Id;
                aspNetEmployee.Name = FullName;
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole(user.Id, "Branch_Principal");
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    char[] charArray = FName.ToCharArray();
                    var fletter = charArray[0].ToString();
                    var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                    AspNetUser student = db.AspNetUsers.Where(x => x.Id == aspNetEmployee.UserId).FirstOrDefault();
                    student.Image = image;
                    db.SaveChanges();
                    string Error = "Branch Admin successfully saved.";
                    return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });
                }
            }
            else if (Position == "Guard")
            {               
                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Non Directive Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.Name = FullName;
                    db.AspNetEmployees.Add(aspNetEmployee);
                    db.SaveChanges();
                    string Error = "Branch Admin successfully saved.";
                return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });

            }
            else if (Position == "Sweeper")
            {
                aspNetEmployee.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Non Directive Staff").Select(x => x.Id).FirstOrDefault();
                aspNetEmployee.Name = FullName;
                db.AspNetEmployees.Add(aspNetEmployee);
                db.SaveChanges();
                string Error = "Branch Admin successfully saved.";
                return RedirectToAction("TeacherIndex", "AspNetEmployees", new { Error });

            }

            return View();
        }



        [HttpGet]
        // GET: AspNetEmployees
        public ActionResult Index()
        {
            var aspNetEmployees = db.AspNetEmployees.Include(a => a.AspNetBranch).Include(a => a.AspNetGender).Include(a => a.AspNetNationality).Include(a => a.AspNetEmployeePosition).Include(a => a.AspNetReligion);
            return View(aspNetEmployees.ToList());
        }
        public ActionResult TeacherIndex()
        {
            ViewBag.Position = new SelectList(db.AspNetEmployeePositions, "Id", "PositionName");
            return View();
        }
        public ActionResult DisableEmployee()
        {
            return View();
        }
        public JsonResult GetDisabledEmployee()
        {
            var emp = db.AspNetEmployees.ToList();
            List<EmployeeList> emplist = new List<EmployeeList>();
            foreach (var item in emp)
            {
                var users = db.AspNetUsers.Where(x => x.Id == item.UserId).FirstOrDefault();
                if (users.StatusId ==2 )
                {
                    EmployeeList el = new EmployeeList();
                    el.Name = item.Name;
                    el.Position = item.AspNetEmployeePosition.PositionName;
                    el.id = item.Id;
                    el.Gender = item.AspNetGender.Title;
                   // el.Image = users.Image;
                    el.UserName = users.UserName;
                    emplist.Add(el);
                }
            }
            return Json(emplist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployee()
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

            var emp=db.AspNetEmployees.Where(x=> x.BranchId == branchId).ToList();

            List<EmployeeList> emplist = new List<EmployeeList>();
            
            foreach (var item in emp)
            {
                var users = db.AspNetUsers.Where(x => x.Id == item.UserId).FirstOrDefault();
                if(users.StatusId !=2)
                {
                    EmployeeList el = new EmployeeList();
                    el.Name = item.Name;
                    el.Position = item.AspNetEmployeePosition.PositionName;
                    el.id = item.Id;
                    el.Gender = item.AspNetGender.Title;
                    el.DateofJoining = item.JoiningDate;
                    el.Image = users.Image;
                    el.UserName = users.UserName;
                    emplist.Add(el);
                }               
            }
            return Json(emplist,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Disable_Employee(int id)
        {
            var uid=db.AspNetEmployees.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            AspNetUser users = db.AspNetUsers.Where(x => x.Id == uid).FirstOrDefault();
            users.StatusId = 2;
            db.SaveChanges();
            return RedirectToAction("TeacherIndex","AspNetEmployees");
        }
        public ActionResult EnableEmployee(int id)
        {
            var uid = db.AspNetEmployees.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
            AspNetUser users = db.AspNetUsers.Where(x => x.Id == uid).FirstOrDefault();
            users.StatusId = 1;
            db.SaveChanges();
            return RedirectToAction("TeacherIndex", "AspNetEmployees");
        }
        public JsonResult Employeelist(int id)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).FirstOrDefault();

            List<EmployeeList> emplist = new List<EmployeeList>();

            if (id != 0)
            {
                var student = db.AspNetEmployees.Where(x => x.Position == id && x.BranchId == branchId).ToList();
                foreach (var item in student)
                {
                    var users = db.AspNetUsers.Where(x => x.Id == item.UserId).FirstOrDefault();
                    if(users.StatusId==1)
                    {
                        EmployeeList std = new EmployeeList();
                        std.Name = item.Name;
                        std.Position = item.AspNetEmployeePosition.PositionName;
                        std.id = item.Id;
                        std.Gender = item.AspNetGender.Title;
                        std.Image = users.Image;
                        emplist.Add(std);
                    }
                
                }
                return Json(emplist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var emp = db.AspNetEmployees.ToList();
                foreach (var item in emp)
                {
                    var users = db.AspNetUsers.Where(x => x.Id == item.UserId).FirstOrDefault();
                    if(users.StatusId==1)
                    {
                        EmployeeList el = new EmployeeList();
                        el.Name = item.Name;
                        el.Position = item.AspNetEmployeePosition.PositionName;
                        el.id = id;
                        el.Gender = item.AspNetGender.Title;
                        el.Image = users.Image;
                        emplist.Add(el);
                    }
               
                }
                return Json(emplist, JsonRequestBehavior.AllowGet);
            }
          
        }
        public class EmployeeList
        {
            public string Name { get; set; }
            public string Position { get; set; }
            public string Gender { get; set; }
            public int id { get; set; }
            public string Image { get; set; }
            public string UserName { get; set; }
            public string DateofJoining { get; set; }


        }
        // GET: AspNetEmployees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetEmployee aspNetEmployee = db.AspNetEmployees.Find(id);
            if (aspNetEmployee == null)
            {
                return HttpNotFound();
            }
            return View(aspNetEmployee);
        }
        [HttpPost]
        public ActionResult Savefile(string ImageName)
        {
            
                var uniquename = "";
                if (Request.Files["Image"] != null)
                {

                    var file = Request.Files["Image"];
                    if (file.FileName != null)
                    {
                        var ext = System.IO.Path.GetExtension(file.FileName);
                        uniquename = Guid.NewGuid().ToString() + ext;
                        var rootpath = Server.MapPath("~/Content/Images");
                        var filesavepath = rootpath + "/" + uniquename;
                        file.SaveAs(filesavepath);
                        AspNetBackGround bg = new AspNetBackGround();
                        bg.Picture = "../Content/Images/" + uniquename;
                        bg.Name = ImageName;
                        db.AspNetBackGrounds.Add(bg);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Create", "AspNetEmployees");
            
           

        }
        // GET: AspNetEmployees/Create
        public ActionResult Create()
           
        {
            ViewBag.InternalError = TempData["InternalError"] as string;
            
            ViewBag.LoaderError = TempData["ErrorMessage"] as string;
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name");
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "ClassName");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");
            var id=User.Identity.GetUserId();
            var rolename = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
            if (rolename=="Super_Admin")
            {
                ViewBag.Position = new SelectList(db.AspNetEmployeePositions.Where(x=>x.PositionName=="Branch Admin" ||x.PositionName=="Branch Principal"), "Id", "PositionName");

            }
            else if(rolename=="Branch_Admin"){
                ViewBag.Position = new SelectList(db.AspNetEmployeePositions.Where(x => x.PositionName != "Branch Admin" && x.PositionName != "Branch Principal"), "Id", "PositionName");

            }
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");
            return View();
        }

        // POST: AspNetEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Position,DateAvailable,Name,BirthDate,NationalityId,ReligionId,GenderId,CellNo,Landline,SpouseName,SpouseHighestDegree,SpouseOccupation,GrossSalary,BasicSalary,MedicalAllowance,Accomodation,ProvidedFund,Tax,EOP,Salary,JoiningDate,Illness,BranchId,Address,Spouse_Address,File,UserId")] AspNetEmployee aspNetEmployee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.AspNetEmployees.Add(aspNetEmployee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetEmployee.BranchId);
        //    ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetEmployee.GenderId);
        //    ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetEmployee.NationalityId);
        //    ViewBag.Position = new SelectList(db.AspNetEmployeePositions, "Id", "PositionName", aspNetEmployee.Position);
        //    ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetEmployee.ReligionId);
        //    return View(aspNetEmployee);
        //}

        // GET: AspNetEmployees/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
               
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AspNetEmployee aspNetEmployee = db.AspNetEmployees.Find(id);
                ViewBag.PositionName = aspNetEmployee.AspNetEmployeePosition.PositionName;
                ViewBag.BranchName = aspNetEmployee.AspNetBranch.Name;
                var employee = (from emp in db.AspNetEmployees
                                join user in db.AspNetUsers on emp.UserId equals user.Id
                                where user.Id == aspNetEmployee.UserId
                                select new { user.Email, user.UserName,user.Image,user.AspNetStatu.Name }).FirstOrDefault();
                ViewBag.Email = employee.Email;
                ViewBag.UserName = employee.UserName;
                ViewBag.Image = employee.Image;
                ViewBag.Status = employee.Name;
                ViewBag.JoiningDate = aspNetEmployee.JoiningDate;
                if (aspNetEmployee == null)
                {
                    
                    return RedirectToAction("TeacherIndex");

                }
                ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetEmployee.BranchId);
                ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetEmployee.GenderId);
                ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetEmployee.NationalityId);
                ViewBag.Position = new SelectList(db.AspNetEmployeePositions, "Id", "PositionName", aspNetEmployee.Position);
                ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetEmployee.ReligionId);
                return View(aspNetEmployee);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
  
                return RedirectToAction("TeacherIndex");
            }
          
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

        public class check
        {
            public int count { get; set; }
            public string by { get; set; }
        }

        public void SaveImage(string base64, string ID, string btn)
        {
            var b = base64.Split(',');
            var val = b[1];
            byte[] bytes = Convert.FromBase64String(val);
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(val)))
            {
                using (Bitmap bm1 = new Bitmap(ms))
                {
                    var uniquename = Guid.NewGuid().ToString() + ".jpg";
                    var Path = Server.MapPath("~/Content/Images");
                    Path = Path + "/" + uniquename;
                    bm1.Save(Path, ImageFormat.Jpeg);
                    AspNetUser ab = db.AspNetUsers.Where(x => x.Id == ID).FirstOrDefault();
                    ab.Image = "../Content/Images/" + uniquename;
                    db.SaveChanges();
                }
            }
        }

        // POST: AspNetEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Position,DateAvailable,Name,BirthDate,NationalityId,ReligionId,GenderId,CellNo,Landline,SpouseName,SpouseHighestDegree,SpouseOccupation,GrossSalary,BasicSalary,MedicalAllowance,Accomodation,ProvidedFund,Tax,EOP,Salary,JoiningDate,Illness,BranchId,Address,Spouse_Address,VirtualRoleId,File,UserId")] AspNetEmployee aspNetEmployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetEmployee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TeacherIndex");
            }
            ViewBag.BranchId = new SelectList(db.AspNetBranches, "Id", "Name", aspNetEmployee.BranchId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", aspNetEmployee.GenderId);
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", aspNetEmployee.NationalityId);
            ViewBag.Position = new SelectList(db.AspNetEmployeePositions, "Id", "PositionName", aspNetEmployee.Position);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", aspNetEmployee.ReligionId);
            return View(aspNetEmployee);
        }

        // GET: AspNetEmployees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetEmployee aspNetEmployee = db.AspNetEmployees.Find(id);
            if (aspNetEmployee == null)
            {
                return HttpNotFound();
            }
            return View(aspNetEmployee);
        }

        // POST: AspNetEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetEmployee aspNetEmployee = db.AspNetEmployees.Find(id);
            db.AspNetEmployees.Remove(aspNetEmployee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public class SubjectClass
        {
            public int id { get; set; }
            public string Subject { get; set; }
            public string Class { get; set; }
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
