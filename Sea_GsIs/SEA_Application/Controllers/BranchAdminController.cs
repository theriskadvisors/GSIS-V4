using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;

namespace SEA_Application.Controllers
{
    [Authorize(Roles = "Branch_Admin,Branch_Principal")]
    public class BranchAdminController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private Sea_Entities db = new Sea_Entities();

        public BranchAdminController()
        {

        }

        public BranchAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public ActionResult Test22()
        {
            var tracking = db.StudentLessonTrackings.Where(x => x.LessonStatus == null).ToList();
            foreach (var item in tracking)
            {

                if (item.StartDate.Value.Date == item.AspnetLesson.StartTime.Value.Date)
                {
                    item.LessonStatus = "Present";
                }
                else
                {
                    item.LessonStatus = "Absent";
                }

                db.SaveChanges();
                //break;
            }

            return RedirectToAction("Index");
        }
        // GET: BranchAdmin
        public ActionResult Index()
        {
            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.Branches = branches;

            var ID = User.Identity.GetUserId();
            var branchID = db.AspNetBranch_Admins.Where(x => x.AdminId == ID).Select(x => x.BranchId).FirstOrDefault();
            var Classes1 = db.AspNetBranch_Class.Where(x => x.BranchId == branchID).Select(x => x.AspNetClass.Name).Distinct().ToList();

            
            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
            classes.Add("Not Published");
            //  classes.Add("Not Published");
            //  classes.Add("ClassEight");
            //classes.Add("ClassNine");
            //classes.Add("ClassTen");



            ViewBag.AllClasses = classes;

            return View();
        }
          public JsonResult GetEvents()
        {
            using (Sea_Entities dc = new Sea_Entities())
            {
                var id = User.Identity.GetUserId();
                var events = dc.Events.Where(x=>x.UserId==id || x.IsPublic==true).Select(x => new { x.Description ,x.End,x.EventID,x.IsFullDay,x.Subject, x.ThemeColor,x.Start,x.IsPublic}).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult getClasses()
        {
            var ID = User.Identity.GetUserId();
            var branchID = db.AspNetBranch_Admins.Where(x => x.AdminId == ID).Select(x => x.BranchId).FirstOrDefault();
            var Classes = db.AspNetBranch_Class.Where(x => x.BranchId == branchID).Select(x => x.AspNetClass.Name).Distinct().ToList();
            return new JsonResult { Data = Classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
           e.UserId=User.Identity.GetUserId();
           var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.Events.Add(e);
                }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
        

        #region Sections

        public ActionResult SectionList()
        {
            var currentlyLoggedInUser = User.Identity.GetUserId();
            var administratedBranchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var modelList = new List<SectionDisplayViewModel>();
            foreach (var id in administratedBranchIds)
            {
                var currentBranch = db.AspNetBranches.Find(id);

               

                var branchClassSectionsUnderCurrentBranch = db.AspNetBranchClass_Sections
                    .Where(branchClassSection => branchClassSection.AspNetBranch_Class.BranchId == currentBranch.Id)
                    .ToList();

                foreach (var branchClassSection in branchClassSectionsUnderCurrentBranch)
                {
                    var model = new SectionDisplayViewModel()
                    {
                        Branch = currentBranch,
                        Class = db.AspNetClasses
                            .Where(@class => @class.Id == branchClassSection.AspNetBranch_Class.ClassId)
                            .First(),
                        Section = db.AspNetSections
                            .Where(section => section.Id == branchClassSection.SectionId)
                            .First(),
                        Session = branchClassSection.AspNetBranch_Class.AspNetSession
                    };
                    modelList.Add(model);
                }
            }
            return View(modelList);
        }
     
        public ActionResult CreateSection()
        {
            var currentlyLoggedInUser = User.Identity.GetUserId();
            var administratedBranchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var branchClassList = new List<AspNetBranch_Class>();
            foreach (var id in administratedBranchIds)
            {
                var list = db.AspNetBranch_Class
                    .Where(branchClass => branchClass.BranchId == id)
                    .ToList();
                branchClassList.AddRange(list);
            }
            branchClassList = branchClassList.Distinct().ToList();
            
            var branchClassItems = new List<SelectListItem>();
            foreach (var item in branchClassList)
            {
                branchClassItems.Add(new SelectListItem
                {
                    Text = $"{item.AspNetClass.Name}, {item.AspNetBranch.Name}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.BrancheClasses = branchClassItems;

            return View();
        }
        public ActionResult CalendarNotification()
        {
            var id = User.Identity.GetUserId();
            var checkdate = DateTime.Now;
            var date = TimeZoneInfo.ConvertTime(DateTime.UtcNow.ToUniversalTime(), TimeZoneInfo.Local);
            var name = "";

            name = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();

            var day = date.DayOfWeek;
            var dd = date.Day;
            var mm = date.Month;
            var yy = date.Year;
            string[] array = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            var Date = day + ", " + dd + " " + array[mm - 1] + " " + yy;
            var result = new { checkdate, Date, name };
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UserName()
        {
            var id = User.Identity.GetUserId();
            var name = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var date = DateTime.Now;
            var result = new { date, name };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateSection(BranchClassSectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var startTime = DateTime.Now;
                var logMessage = "";
                var c = new AspNetBranchClass_Sections
                {
                    AspNetSection = new AspNetSection
                    {
                        Name = model.SectionName
                    },
                    BranchClassId = model.BranchClassId
                };
                db.AspNetBranchClass_Sections.Add(c);
                db.SaveChanges();
                var branchClass = db.AspNetBranch_Class
                    .Where(bc => bc.Id == c.BranchClassId)
                    .First();
                logMessage += $"Section {c.AspNetSection.Name} ({c.SectionId}) added in class {branchClass.AspNetClass.Name} ({branchClass.ClassId}) of branch {branchClass.AspNetBranch.Name} ({branchClass.BranchId}).";
                var endTime = DateTime.Now;
                CreateLog(logMessage, startTime, endTime);
                return RedirectToAction("SectionList");
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        public ActionResult SectionDetails(int id)
        {
            var section = db.AspNetBranchClass_Sections
                .Where(bcs => bcs.SectionId == id)
                .ToList();
            return View(section);
        }

        [HttpPost]
        public JsonResult DeleteSection(int id)
        {
            var logMessage = "Delete class initiated.";
            var startTime = DateTime.Now;
            var section = db.AspNetSections.Find(id);
            if (section == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var associatedBranchClassSections = db.AspNetBranchClass_Sections
                .Where(bcs => bcs.SectionId == section.Id)
                .ToList();

            foreach (var bcs in associatedBranchClassSections)
            {
                logMessage += $" Deleted branch_class_section entry {bcs.Id}. Removed section {bcs.AspNetSection.Name} ({bcs.SectionId}) from class {bcs.AspNetBranch_Class.AspNetClass.Name} ({bcs.AspNetBranch_Class.ClassId}) of branch {bcs.AspNetBranch_Class.AspNetBranch.Name} ({bcs.AspNetBranch_Class.BranchId}).";
                db.Entry(bcs).State = EntityState.Deleted;
                db.SaveChanges();
            }

            //logMessage += $" Deleted section {section.Name} ({section.Id}).";
            //db.Entry(section).State = EntityState.Deleted;
            //db.SaveChanges();
            var endTime = DateTime.Now;
            CreateLog(logMessage, startTime, endTime);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditSection(int id)
        {
            var section = db.AspNetSections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            var branchClassSection = db.AspNetBranchClass_Sections
                .Where(s => s.SectionId == section.Id)
                .First();
            ViewBag.BranchClassSection = branchClassSection;
            return View(section);
        }

        [HttpPost]
        public ActionResult EditSection(AspNetSection section)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var logMessage = "Edit section init.";
                    var startTime = DateTime.Now;
                    var sectionInDb = db.AspNetSections.Find(section.Id);
                    sectionInDb.Name = section.Name;
                    db.Entry(sectionInDb).State = EntityState.Modified;
                    db.SaveChanges();
                    logMessage += $" Name of section {section.Name} ({section.Id}) changed from {Request.Form["PreviousName"]} to {section.Name}.";
                    CreateLog(logMessage, startTime, DateTime.Now);
                    return RedirectToAction("SectionDetails", new { id = section.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Model state is invalid.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            var branchClassSection = db.AspNetBranchClass_Sections
                .Where(s => s.SectionId == section.Id)
                .First();
            ViewBag.BranchClassSection = branchClassSection;
            return View(section);
        }

        #endregion

        #region Students

        public ActionResult StudentsList()
        {
            var loggedInUserId = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).ToList();
            var studentsList = new List<AspNetStudent>();
            foreach (var branchId in branchIds)
            {
                var studentsInCurrentBranch = db.AspNetStudents
                    .Where(student => student.BranchId == branchId)
                    .ToList();
                studentsInCurrentBranch.ForEach(student => studentsList.Add(student));
            }
            return View(studentsList);
        }


        public ActionResult CreateStudent()
        {
            var currentlyLoggedInUser = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var branchList = new List<AspNetBranch>();
            foreach (var id in branchIds)
            {
                var currentBranch = db.AspNetBranches
                    .Where(branch => branch.Id == id)
                    .First();
                branchList.Add(currentBranch);
            }
            var branches = new List<SelectListItem>();
            foreach (var item in branchList)
            {
                branches.Add(new SelectListItem
                {
                    Text = $"{item.Name}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Branches = branches;

            ViewBag.Class = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.Section = new SelectList(db.AspNetSections, "Id", "Name");
            ViewBag.Session = new SelectList(db.AspNetSessions, "Id", "Year");

            return View();
        }

        [HttpPost]
        //public ActionResult CreateStudent(StudentUserViewModel model)
        //{

        //    bool userCreated = false;
        //    bool userAddedToRole = false;
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        //    ApplicationUser user = null;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var logMessage = "";
        //            var startTime = DateTime.Now;

        //            user = new ApplicationUser
        //            {
        //                UserName = model.UserName,
        //                Email = model.Email
        //            };

        //            var userCreatedStatus = UserManager.Create(user, model.Password);

        //            if (userCreatedStatus.Succeeded)
        //            {
        //                userCreated = true;
        //                logMessage += $" User ({user.UserName}) created. User id: {user.Id}.";
        //                var addToRoleStatus = UserManager.AddToRole(user.Id, "Student");
        //                logMessage += $" {user.UserName} added to Role \"Student\"";
        //                if (!addToRoleStatus.Succeeded)
        //                {
        //                    throw new Exception("Could not add user to student role");
        //                }
        //                userAddedToRole = false;
        //            }
        //            else
        //            {
        //                throw new Exception("Could not create the student");
        //            }

        //            var image = Request.Files[0];
        //            string imageName = null;
        //            string serverPath = null;
        //            string fullPath = null;
        //            string newPath = null;
        //            if (image != null && image.ContentLength > 0)
        //            {
        //                imageName = Path.GetFileName(image.FileName);
        //                serverPath = Server.MapPath("~/Content/Profile/Student");
        //                fullPath = Path.Combine(serverPath, imageName);
        //                image.SaveAs(fullPath);
        //            }

        //            var student = new AspNetStudent
        //            {
        //                Name = model.Name,
        //                RollNo = model.RollNo,
        //                UserId = user.Id,
        //                BranchId = model.BranchId,
        //                Address = model.Address,
        //                File = newPath
        //            };

        //            db.AspNetStudents.Add(student);
        //            db.SaveChanges();
        //            logMessage += $" Student created. Student id: {student.Id}. Student roll number: {student.RollNo}.";

        //            if (image != null && image.ContentLength > 0)
        //            {
        //                newPath = Path.Combine(serverPath, $"{student.Id}.{Path.GetExtension(fullPath)}");
        //                System.IO.File.Move(fullPath, newPath);
        //                student.File = Path.GetFileName(newPath);
        //                db.Entry(student).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //            // Create default enrollments
        //            var enrollments = new List<AspNetStudent_Enrollments>();
        //            var mandatoryCourses = db.AspNetClass_Courses
        //                .Where(classCourse => classCourse.ClassId == model.Class && classCourse.IsMandatory)
        //                .Select(classCourse => classCourse.AspNetCours)
        //                .ToList();
        //            foreach (var course in mandatoryCourses)
        //            {
        //                var enrollment = new AspNetStudent_Enrollments
        //                {
        //                    StudentId = student.Id,
        //                    CourseId = course.Id,
        //                    SectionId = db.AspNetBranchClass_Sections
        //                        .Where(branchClassSection => branchClassSection.AspNetBranch_Class.BranchId == model.BranchId && branchClassSection.AspNetBranch_Class.ClassId == model.Class && branchClassSection.SectionId == model.Section)
        //                        .First()
        //                        .Id,
        //                    SessionId = model.Session
        //                };
        //                db.AspNetStudent_Enrollments.Add(enrollment);
        //            }
        //            db.SaveChanges();

        //            var endTime = DateTime.Now;
        //            CreateLog(logMessage, startTime, endTime);
        //            return RedirectToAction("StudentsList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if(userAddedToRole)
        //        {
        //            UserManager.RemoveFromRole(user.Id, "Student");
        //        }

        //        if(userCreated)
        //        {
        //            UserManager.Delete(user);
        //        }
        //    }
        //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        //}

        //public ActionResult EditStudent(int id)
        //{
        //    var student = db.AspNetStudents.Find(id);
        //    if(student == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var model = new StudentUserViewModel
        //    {
        //        Id = student.Id,
        //        Name = student.Name,
        //        RollNo = student.RollNo,
        //        BranchId = student.BranchId,
        //        UserName = student.AspNetUser.UserName,
        //        Email = student.AspNetUser.Email,
        //        Password = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        //    };

        //    var currentlyLoggedInUser = User.Identity.GetUserId();
        //    var branchIds = db.AspNetBranch_Admins
        //        .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
        //        .Select(branchAdmin => branchAdmin.BranchId)
        //        .ToList();
        //    var branchList = new List<AspNetBranch>();
        //    foreach (var branchId in branchIds)
        //    {
        //        var currentBranch = db.AspNetBranches
        //            .Where(branch => branch.Id == branchId)
        //            .First();
        //        branchList.Add(currentBranch);
        //    }
        //    var branches = new List<SelectListItem>();
        //    foreach (var item in branchList)
        //    {
        //        branches.Add(new SelectListItem
        //        {
        //            Text = $"{item.Name}",
        //            Value = item.Id.ToString()
        //        });
        //    }
        //    ViewBag.Branches = branches;

        //    return View(model);
        //}

        //public ActionResult EditStudent(StudentUserViewModel model)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        var startTime = DateTime.Now;
        //        var logMessage = $"Edit student init. Target student name: {Request.Form["PreviousName"]}. Target student id: {model.Id}.";
        //        var changeOccurred = false;
        //        var studentInDb = db.AspNetStudents.Find(model.Id);
        //        if(!model.Name.Equals(Request.Form["PreviousName"], StringComparison.OrdinalIgnoreCase))
        //        {
        //            changeOccurred = true;
        //            logMessage += $" Name of student changed from '{Request.Form["PreviousName"]}' to '{model.Name}'.";
        //            studentInDb.Name = model.Name;
        //        }

        //        if(!model.RollNo.Equals(Request.Form["PreviousRollNo"], StringComparison.OrdinalIgnoreCase))
        //        {
        //            changeOccurred = true;
        //            logMessage += $" Roll of student changed from '{Request.Form["PreviousName"]}' to '{model.RollNo}'.";
        //            studentInDb.RollNo = model.RollNo;
        //        }

        //        if(!model.UserName.Equals(Request.Form["PreviousUsername"], StringComparison.OrdinalIgnoreCase))
        //        {
        //            changeOccurred = true;
        //            logMessage += $" Username of student changed from '{Request.Form["PreviousUsername"]}' to '{model.UserName}'.";
        //            studentInDb.AspNetUser.UserName = model.UserName;
        //        }

        //        if(!model.Email.Equals(Request.Form["PreviousEmail"], StringComparison.OrdinalIgnoreCase))
        //        {
        //            changeOccurred = true;
        //            logMessage += $" Email address of student changed from '{Request.Form["PreviousEmail"]}' to '{model.Email}'.";
        //            studentInDb.AspNetUser.Email = model.Email;
        //        }

        //        if(!model.Address.Equals(studentInDb.Address, StringComparison.OrdinalIgnoreCase))
        //        {
        //            changeOccurred = true;
        //            logMessage += $" Adddress of student changed from '{studentInDb.Address}' to '{model.Address}'.";
        //            studentInDb.Address = model.Address;
        //        }

        //        if(Convert.ToInt32(Request.Form["PreviousBranch"]) != model.BranchId)
        //        {
        //            changeOccurred = true;
        //            var previousBranch = db.AspNetBranches.Find(Convert.ToInt32(Request.Form["PreviousBranch"]));
        //            var updatedBranch = db.AspNetBranches.Find(model.BranchId);
        //            logMessage += $" Student branch changed from '{previousBranch.Name} ({previousBranch.Id})' to '{updatedBranch.Name} ({updatedBranch.Id})'.";
        //            studentInDb.BranchId = model.BranchId;
        //        }
        //        if(changeOccurred)
        //        {
        //            db.Entry(studentInDb).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            logMessage += " No change occurred in the student.";
        //        }
        //        CreateLog(logMessage, startTime, DateTime.Now);
        //        return RedirectToAction("StudentDetails", new { id = model.Id });
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Model state is invalid");
        //        var currentlyLoggedInUser = User.Identity.GetUserId();
        //        var branchIds = db.AspNetBranch_Admins
        //            .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
        //            .Select(branchAdmin => branchAdmin.BranchId)
        //            .ToList();
        //        var branchList = new List<AspNetBranch>();
        //        foreach (var branchId in branchIds)
        //        {
        //            var currentBranch = db.AspNetBranches
        //                .Where(branch => branch.Id == branchId)
        //                .First();
        //            branchList.Add(currentBranch);
        //        }
        //        var branches = new List<SelectListItem>();
        //        foreach (var item in branchList)
        //        {
        //            branches.Add(new SelectListItem
        //            {
        //                Text = $"{item.Name}",
        //                Value = item.Id.ToString(),
        //                Selected = item.Id == model.BranchId
        //            });
        //        }
        //        ViewBag.Branches = branches;
        //        return View(model);
        //    }
        //}

        //public ActionResult StudentDetails(int id)
        //{
        //    var student = db.AspNetStudents.Find(id);
        //    if(student == null)
        //    {
        //        return HttpNotFound("Could not find the student with given id.");
        //    }
        //    var user = db.AspNetUsers.Find(student.UserId);
        //    ViewBag.Username = user.UserName;
        //    ViewBag.Email = user.Email;
        //    var enrollment = db.AspNetStudent_Enrollments.Where(en => en.StudentId == id).Count() == 0 ? null : db.AspNetStudent_Enrollments.Where(en => en.StudentId == id).First();
        //    if(enrollment == null)
        //    {
        //        ViewBag.Enrollment = null;
        //    }
        //    else
        //    {
        //        ViewBag.Enrollment = enrollment;
        //    }
        //    if(student.File == null)
        //    {
        //        ViewBag.ImagePath = null;
        //    }
        //    else
        //    {
        //        var imagePath = Path.Combine("~/Content/Profile/Student", student.File);
        //        ViewBag.ImagePath = imagePath;
        //    }
        //    return View(student);
        //}

        //public JsonResult DeleteStudent(int id)
        //{
        //    var logMessage = "";
        //    var startTime = DateTime.Now;
        //    try
        //    {
        //        var student = db.AspNetStudents.Find(id);
        //        if (student == null)
        //        {
        //            return Json(false, JsonRequestBehavior.AllowGet);
        //        }
        //        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        //        var applicationUser = userManager.FindById(student.UserId);
        //        logMessage += $" Removed student {student.Name} ({student.Id}) from role student.";
        //        var removeFromRoleResult = userManager.RemoveFromRole(student.UserId, "Student");
        //        if (!removeFromRoleResult.Succeeded)
        //        {
        //            string errors = "";
        //            foreach (var error in removeFromRoleResult.Errors)
        //            {
        //                errors += error + Environment.NewLine;
        //            }
        //            throw new Exception(errors);
        //        }

        //        db.AspNetStudent_Enrollments
        //            .Where(enrollment => enrollment.StudentId == id)
        //            .ToList()
        //            .ForEach(enrollment => db.Entry(enrollment).State = EntityState.Deleted);

        //        logMessage += $" Deleted student {student.Name} ({student.Id}) from students table.";
        //        db.Entry(student).State = System.Data.Entity.EntityState.Deleted;
        //        db.SaveChanges();
        //        logMessage += $" Deleted student {student.Name} ({student.Id}) (application user id: {applicationUser.Id}) from users table.";
        //        var deleteResult = userManager.Delete(applicationUser);
        //        if(!deleteResult.Succeeded)
        //        {
        //            string errors = "";
        //            foreach (var error in deleteResult.Errors)
        //            {
        //                errors += error + Environment.NewLine;
        //            }
        //            throw new Exception(errors);
        //        }
        //        CreateLog(logMessage, startTime, DateTime.Now);
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion

        #region Teachers

        public ActionResult TeachersList()
        {
            var loggedInUserId = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId).ToList();
            var teachersList = new List<AspNetTeacher>();
            foreach (var branchId in branchIds)
            {
                var teachersInCurrentBranch = db.AspNetTeachers
                    .Where(teacher => teacher.AspNetEmployee.BranchId == branchId)
                    .ToList();
                teachersInCurrentBranch.ForEach(teacher => teachersList.Add(teacher));
            }
            return View(teachersList);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentsfromFile(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                

                HttpPostedFileBase file = Request.Files["students"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var studentList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    string[] arr = new string[noOfRow-1];
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {                      
                            var student= new RegisterViewModel();
                            var Fname = workSheet.Cells[rowIterator, 1].Value.ToString();
                            var Mname = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var Lname = workSheet.Cells[rowIterator, 3].Value.ToString();
                            if (Lname == "-")
                            {
                                Lname = "";
                            }
                            if (Mname == "-")
                            {
                                Mname = "";
                            }
                            var FullName= Fname + " " + Mname + " " + Lname;
                        student.Name = FullName;
                        student.UserName = workSheet.Cells[rowIterator, 4].Value.ToString();
                        student.Email = workSheet.Cells[rowIterator, 5].Value.ToString();
                        student.Password = workSheet.Cells[rowIterator, 6].Value.ToString();
                        student.ConfirmPassword = workSheet.Cells[rowIterator, 7].Value.ToString();
                        var user = new ApplicationUser { UserName = student.UserName, Email = student.Email, Name = student.Name };
                        var result = await UserManager.CreateAsync(user, student.Password);

                            if (result.Succeeded)
                            {
                                AspNetStudent studentDetail = new AspNetStudent();
                                studentDetail.UserId = user.Id;
                                arr[rowIterator - 2] = studentDetail.UserId;
                                studentDetail.Name = FullName;
                                studentDetail.RollNo = student.UserName;
                                var Nname = workSheet.Cells[rowIterator, 8].Value.ToString();
                                studentDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nname).Select(x => x.Id).FirstOrDefault();
                               // var RName = workSheet.Cells[rowIterator, 9].Value.ToString();
                                //studentDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == RName).Select(x => x.Id).FirstOrDefault();
                                var Gname = workSheet.Cells[rowIterator, 9].Value.ToString();
                                studentDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gname).Select(x => x.Id).FirstOrDefault();
                                studentDetail.CellNo = workSheet.Cells[rowIterator, 10].Value.ToString();
                                studentDetail.Address = workSheet.Cells[rowIterator, 11].Value.ToString();
                                studentDetail.Birthdate = workSheet.Cells[rowIterator, 12].Value.ToString();
                                var branchname = workSheet.Cells[rowIterator, 13].Value.ToString();
                                studentDetail.BranchId = db.AspNetBranches.Where(x => x.Name == branchname).Select(x => x.Id).FirstOrDefault();
                                var classname = workSheet.Cells[rowIterator, 14].Value.ToString();
                                studentDetail.ClassId = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                                db.AspNetStudents.Add(studentDetail);

                                var roleStore = new RoleStore<IdentityRole>(context);
                                var roleManager = new RoleManager<IdentityRole>(roleStore);
                                var userStore = new UserStore<ApplicationUser>(context);
                                var userManager = new UserManager<ApplicationUser>(userStore);
                                userManager.AddToRole(user.Id, "Student");
                                db.SaveChanges();

                            //var courselist = db.AspNetClass_Courses.Where(x => x.ClassId == studentDetail.ClassId).Select(x => x.Id).ToList();
                            //foreach (var item in courselist)
                            //{

                            //    var SessioName = workSheet.Cells[rowIterator, 15].Value.ToString();
                            //    int SectionID = db.AspNetSections.Where(x=>x.Name == SessioName).FirstOrDefault().Id;

                            //    AspNetStudent_Enrollments se = new AspNetStudent_Enrollments();
                            //    se.StudentId = studentDetail.Id;
                            //    se.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                            //    se.CourseId = item;

                            //    var BCid = db.AspNetBranch_Class.Where(x => x.ClassId == studentDetail.ClassId && x.BranchId == studentDetail.BranchId).Select(x => x.Id).FirstOrDefault();
                            //    se.SectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid && x.SectionId == SectionID).Select(x => x.Id).FirstOrDefault();
                            //    db.AspNetStudent_Enrollments.Add(se);
                            //    db.SaveChanges();
                            //}

                        }
                        else
                            {
                                dbTransaction.Dispose();
                                AddErrors(result);
                                return RedirectToAction("Create", "AspNetStudents", model);
                            }
                                               
                    }
                    dbTransaction.Commit();
                    LoadImages(arr);
                    return RedirectToAction("StudentIndex", "AspNetStudents");
                }
            }
            catch (Exception e)
            {
                dbTransaction.Dispose();
                return RedirectToAction("Create", "AspNetStudents", model);

            }
        }
        public void LoadImages(string[] idList)
        {
           
               // var userid = idList.Split(',');
            foreach (var item in idList)
            {

                var name = db.AspNetUsers.Where(x => x.Id == item).Select(x => x.Name).FirstOrDefault();
                char[] charArray = name.ToCharArray();
                var fletter = charArray[0].ToString();
                var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
                AspNetUser std = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                std.Image = image;
                db.SaveChanges();
            }
          
        }
        //public async Task<ActionResult> StudentsfromFile(RegisterViewModel model)
        //{
        //    ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");
        //    // if (ModelState.IsValid)
        //    var dbTransaction = db.Database.BeginTransaction();
        //    try
        //    {
        //        HttpPostedFileBase file = Request.Files["students"];
        //        if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //        {
        //            string fileName = file.FileName;
        //            string fileContentType = file.ContentType;
        //            byte[] fileBytes = new byte[file.ContentLength];
        //            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //        }
        //        var studentList = new List<RegisterViewModel>();
        //        using (var package = new ExcelPackage(file.InputStream))
        //        {
        //            var currentSheet = package.Workbook.Worksheets;
        //            var workSheet = currentSheet.First();
        //            var noOfCol = workSheet.Dimension.End.Column;
        //            var noOfRow = workSheet.Dimension.End.Row;

        //            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //            {
        //                var student = new RegisterViewModel();

        //                var Fname = workSheet.Cells[rowIterator, 1].Value.ToString();
        //                var Mname = workSheet.Cells[rowIterator, 2].Value.ToString();
        //                var Lname = workSheet.Cells[rowIterator, 3].Value.ToString();
        //                if (Lname == "-")
        //                {
        //                    Lname = "";
        //                }
        //                if (Mname == "-")
        //                {
        //                    Mname = "";
        //                }
        //                var FullName = Fname + " " + Mname + " " + Lname;
        //                student.Name = FullName;
        //                student.UserName = workSheet.Cells[rowIterator, 4].Value.ToString();
        //                student.Email = workSheet.Cells[rowIterator, 5].Value.ToString();
        //                student.Password = workSheet.Cells[rowIterator, 6].Value.ToString();
        //                student.ConfirmPassword = workSheet.Cells[rowIterator, 7].Value.ToString();

        //                ApplicationDbContext context = new ApplicationDbContext();
        //                var user = new ApplicationUser { UserName = student.UserName, Email = student.Email, Name = student.Name };
        //                var result = await UserManager.CreateAsync(user, student.Password);

        //                if (result.Succeeded)
        //                {
        //                    AspNetStudent studentDetail = new AspNetStudent();
        //                    studentDetail.UserId = user.Id;
        //                    studentDetail.Name = FullName;
        //                    studentDetail.RollNo = student.UserName;
        //                var Nname = workSheet.Cells[rowIterator, 8].Value.ToString();
        //                studentDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nname).Select(x => x.Id).FirstOrDefault();
        //                var RName = workSheet.Cells[rowIterator, 9].Value.ToString();
        //                studentDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == RName).Select(x => x.Id).FirstOrDefault();
        //                var Gname = workSheet.Cells[rowIterator, 10].Value.ToString();
        //                studentDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gname).Select(x => x.Id).FirstOrDefault();
        //                studentDetail.CellNo = workSheet.Cells[rowIterator, 11].Value.ToString();
        //                studentDetail.Address = workSheet.Cells[rowIterator, 12].Value.ToString();
        //                studentDetail.Birthdate = workSheet.Cells[rowIterator, 13].Value.ToString();
        //                var branchname =workSheet.Cells[rowIterator, 14].Value.ToString();
        //                studentDetail.BranchId= db.AspNetBranches.Where(x => x.Name == branchname).Select(x => x.Id).FirstOrDefault();
        //                var classname = workSheet.Cells[rowIterator, 15].Value.ToString();
        //                studentDetail.ClassId = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
        //                db.AspNetStudents.Add(studentDetail);
        //                db.SaveChanges();

        //                var roleStore = new RoleStore<IdentityRole>(context);
        //                var roleManager = new RoleManager<IdentityRole>(roleStore);
        //                var userStore = new UserStore<ApplicationUser>(context);
        //                var userManager = new UserManager<ApplicationUser>(userStore);
        //                userManager.AddToRole(user.Id, "Student");

        //                char[] charArray = Fname.ToCharArray();
        //                var fletter = charArray[0].ToString();
        //                var image = db.AspNetBackGrounds.Where(x => x.Name == fletter).Select(x => x.Picture).FirstOrDefault();
        //                AspNetUser std = db.AspNetUsers.Where(x => x.Id == studentDetail.UserId).FirstOrDefault();
        //                std.Image = image;
        //                db.SaveChanges();

        //                var courselist = db.AspNetClass_Courses.Where(x => x.ClassId == studentDetail.ClassId).Select(x => x.Id).ToList();
        //                foreach (var item in courselist)
        //                 {
        //                    AspNetStudent_Enrollments se = new AspNetStudent_Enrollments();
        //                    se.StudentId = studentDetail.Id;
        //                    se.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
        //                    se.CourseId = item;
        //                    var BCid = db.AspNetBranch_Class.Where(x => x.ClassId == studentDetail.ClassId && x.BranchId == studentDetail.BranchId).Select(x => x.Id).FirstOrDefault();
        //                    se.SectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid).Select(x => x.Id).FirstOrDefault();
        //                    db.AspNetStudent_Enrollments.Add(se);
        //                    db.SaveChanges();
        //                 }
        //                }
        //                else
        //                {
        //                    dbTransaction.Dispose();
        //                    AddErrors(result);
        //                    return RedirectToAction("Create","AspNetStudents",model);
        //                }
        //            }
        //            dbTransaction.Commit();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        dbTransaction.Dispose();
        //        return RedirectToAction("Create", "AspNetStudents", model);
        //    }
        //    return RedirectToAction("StudentIndex", "AspNetStudents");
        //}

        public ActionResult CreateTeacher()
        {
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");

            return View(@"~\Views\BranchAdmin\C_Teacher.cshtml");
        }

        [HttpPost]
        public ActionResult CreateTeacher(TeacherUserEmployeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.Password.Equals(model.ConfirmPassword))
                    {
                        throw new Exception("Password and Confirm Password fields don't match.");
                    }

                    ApplicationDbContext context = new ApplicationDbContext();
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    var user = new ApplicationUser
                    {
                        UserName = model.Username,
                        Email = model.Email
                    };

                    var userCreatedStatus = userManager.Create(user, model.Password);
                    if(userCreatedStatus.Succeeded)
                    {
                        var addToRoleStatus = userManager.AddToRole(user.Id, "Teacher");
                        if(!addToRoleStatus.Succeeded)
                        {
                            throw new Exception("Cannot add user to the teacher role.");
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot create the application user for the teacher.");
                    }

                    var image = Request.Files[0];
                    string imageName = null;
                    string serverPath = null;
                    string fullPath = null;
                    string newPath = null;
                    if (image != null && image.ContentLength > 0)
                    {
                        imageName = Path.GetFileName(image.FileName);
                        serverPath = Server.MapPath("~/Content/Profile/Teacher");
                        fullPath = Path.Combine(serverPath, imageName);
                        image.SaveAs(fullPath);
                    }

                    var employee = new AspNetEmployee
                    {
                        Name = model.Name,
                        //BirthDate = model.BirthDate,
                        //JoiningDate = model.JoiningDate,
                        NationalityId = model.NationalityId,
                        ReligionId = model.ReligionId,
                        GenderId = model.GenderId,
                        Address = model.Address,
                        Landline = model.Landline,
                        CellNo = model.CellNo,
                        GrossSalary = model.GrossSalary,
                        BasicSalary = model.BasicSalary,
                        MedicalAllowance = model.MedicalAllowance,
                        ProvidedFund = model.ProvidedFund,
                        EOP = model.EOP,
                        Tax = model.Tax,
                        BranchId = model.BranchId,
                        File = newPath
                    };

                    db.AspNetEmployees.Add(employee);
                    db.SaveChanges();

                    var teacher = new AspNetTeacher
                    {
                        UserId = user.Id,
                        RegistrationNo = model.RegistrationNo,
                        EmployeeId = employee.Id
                    };

                    db.AspNetTeachers.Add(teacher);
                    db.SaveChanges();
                    if (image != null && image.ContentLength > 0)
                    {
                        newPath = Path.Combine(serverPath, $"{teacher.Id}.{Path.GetExtension(fullPath)}");
                        System.IO.File.Move(fullPath, newPath);
                        teacher.AspNetEmployee.File = Path.GetFileName(newPath);
                        db.Entry(teacher).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("TeachersList");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}...{ex?.InnerException?.Message}");
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);

            return View(@"~\Views\BranchAdmin\C_Teacher.cshtml", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CoursePackagefromFile(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                HttpPostedFileBase file = Request.Files["Subject_Groups"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                            AspNetCoursePackage coursepkg = new AspNetCoursePackage();
                            
                            var course = workSheet.Cells[rowIterator, 1].Value.ToString();
                            coursepkg.CourseId = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();
                            var pkg= workSheet.Cells[rowIterator, 2].Value.ToString();
                            coursepkg.PackageId = db.AspNetPackages.Where(x => x.Title == pkg).Select(x => x.Id).FirstOrDefault();
                            db.AspNetCoursePackages.Add(coursepkg);
                            db.SaveChanges();         
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("Index", "SubjectGroups");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("Create", "SubjectGroups", model);

            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClassCoursefromFile(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                HttpPostedFileBase file = Request.Files["Class_Course"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        AspNetClass_Courses classcourse = new AspNetClass_Courses();

                        var Class = workSheet.Cells[rowIterator, 1].Value.ToString();
                        classcourse.ClassId = db.AspNetClasses.Where(x => x.Name == Class).Select(x => x.Id).FirstOrDefault();
                        var course = workSheet.Cells[rowIterator, 2].Value.ToString();
                        classcourse.CourseId = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();

                        db.AspNetClass_Courses.Add(classcourse);
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("Class_CourseIndex", "SubjectGroups");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("Class_CourseCreate", "SubjectGroups", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TeacherfromFile(RegisterViewModel model)
        {
             int ErrorLine = 0;
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {

                
                HttpPostedFileBase file = Request.Files["teachers"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                   
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var position = workSheet.Cells[rowIterator, 1].Value.ToString();
                        ErrorLine++;
                        if(position=="Teacher")
                        {
                            var teacher = new RegisterViewModel();

                            
                            var Fname = workSheet.Cells[rowIterator, 2].Value ?? string.Empty;
                            var Mname = workSheet.Cells[rowIterator, 3].Value ?? string.Empty;
                            var Lname = workSheet.Cells[rowIterator, 4].Value ?? string.Empty;

                            teacher.Name = Fname + " " + Mname + " " + Lname;
                            teacher.UserName = workSheet.Cells[rowIterator, 5].Value.ToString();
                            teacher.Email = workSheet.Cells[rowIterator, 6].Value.ToString();
                            teacher.Password = workSheet.Cells[rowIterator, 7].Value.ToString();
                            teacher.ConfirmPassword = workSheet.Cells[rowIterator, 8].Value.ToString();
                            string number = workSheet.Cells[rowIterator, 9].Value.ToString();
                            var user = new ApplicationUser { UserName = teacher.UserName, Email = teacher.Email, Name = teacher.Name, PhoneNumber = number };
                            var result = await UserManager.CreateAsync(user, teacher.Password);
                            if (result.Succeeded)
                            {
                                AspNetEmployee teacherDetail = new AspNetEmployee();
                                teacherDetail.Name = teacher.Name;
                                teacherDetail.UserId = user.Id;
                             //   var pos = workSheet.Cells[rowIterator, 1].Value.ToString();
                                teacherDetail.Position = db.AspNetEmployeePositions.Where(x => x.PositionName == position).Select(x => x.Id).FirstOrDefault();
                                teacherDetail.JoiningDate =workSheet.Cells[rowIterator, 10].Value.ToString();
                                teacherDetail.DateAvailable = workSheet.Cells[rowIterator, 11].Value.ToString();
                                teacherDetail.BirthDate =workSheet.Cells[rowIterator,12].Value.ToString();
                                var brnch= workSheet.Cells[rowIterator, 13].Value.ToString();
                                teacherDetail.BranchId = db.AspNetBranches.Where(x => x.Name == brnch).Select(x => x.Id).FirstOrDefault();
                                var Nationality = workSheet.Cells[rowIterator, 14].Value.ToString();
                                teacherDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nationality).Select(x => x.Id).FirstOrDefault();
                                var Religion = workSheet.Cells[rowIterator, 15].Value.ToString();
                                teacherDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == Religion).Select(x => x.Id).FirstOrDefault();
                                var Gender = workSheet.Cells[rowIterator, 16].Value.ToString();
                                teacherDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gender).Select(x => x.Id).FirstOrDefault();
                                teacherDetail.CellNo = workSheet.Cells[rowIterator, 9].Value.ToString();
                                teacherDetail.Landline = workSheet.Cells[rowIterator, 17].Value.ToString();
                                teacherDetail.Address = workSheet.Cells[rowIterator, 18].Value.ToString();
                                teacherDetail.SpouseName = workSheet.Cells[rowIterator, 19].Value.ToString();
                                teacherDetail.SpouseHighestDegree = workSheet.Cells[rowIterator, 20].Value.ToString();
                                teacherDetail.SpouseOccupation = workSheet.Cells[rowIterator, 21].Value.ToString();
                                teacherDetail.Spouse_Address = workSheet.Cells[rowIterator, 22].Value.ToString();
                                teacherDetail.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Teaching Staff").Select(x => x.Id).FirstOrDefault();
                                db.AspNetEmployees.Add(teacherDetail);
                                var roleStore = new RoleStore<IdentityRole>(context);
                                var roleManager = new RoleManager<IdentityRole>(roleStore);
                                var userStore = new UserStore<ApplicationUser>(context);
                                var userManager = new UserManager<ApplicationUser>(userStore);
                                userManager.AddToRole(user.Id, "Teacher");
                                db.SaveChanges();

                            }
                            else
                            {
                                dbTransaction.Dispose();
                                TempData["InternalError"]  =result.Errors.LastOrDefault() + "At Row #" + ErrorLine;
                                
                                AddErrors(result);
                                return RedirectToAction("Create","AspNetEmployees", model);
                            }

                        }
                        else if(position=="Accountant")
                        {

                            var accountant = new RegisterViewModel();
                            var Fname = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var Mname = workSheet.Cells[rowIterator, 3].Value.ToString();
                            var Lname = workSheet.Cells[rowIterator, 4].Value.ToString();
                            accountant.Name = Fname + " " + Mname + " " + Lname;
                            accountant.UserName = workSheet.Cells[rowIterator, 5].Value.ToString();
                            accountant.Email = workSheet.Cells[rowIterator, 6].Value.ToString();
                            accountant.Password = workSheet.Cells[rowIterator, 7].Value.ToString();
                            accountant.ConfirmPassword = workSheet.Cells[rowIterator, 8].Value.ToString();
                            string number = workSheet.Cells[rowIterator, 9].Value.ToString();
                            var user = new ApplicationUser { UserName = accountant.UserName, Email = accountant.Email, Name = accountant.Name, PhoneNumber = number };
                            var result = await UserManager.CreateAsync(user, accountant.Password);
                            if (result.Succeeded)
                            {
                                AspNetEmployee teacherDetail = new AspNetEmployee();
                                teacherDetail.Name = accountant.Name;
                                teacherDetail.UserId = user.Id;
                                var pos = workSheet.Cells[rowIterator, 1].Value.ToString();
                                teacherDetail.Position = db.AspNetEmployeePositions.Where(x => x.PositionName == position).Select(x => x.Id).FirstOrDefault();
                                teacherDetail.JoiningDate =workSheet.Cells[rowIterator, 10].Value.ToString();
                                teacherDetail.DateAvailable = workSheet.Cells[rowIterator, 11].Value.ToString();
                                teacherDetail.BirthDate =workSheet.Cells[rowIterator, 12].Value.ToString();
                                var brnch = workSheet.Cells[rowIterator, 13].Value.ToString();
                                teacherDetail.BranchId = db.AspNetBranches.Where(x => x.Name == brnch).Select(x => x.Id).FirstOrDefault();

                                var Nationality = workSheet.Cells[rowIterator, 14].Value.ToString();
                                teacherDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nationality).Select(x => x.Id).FirstOrDefault();
                                var Religion = workSheet.Cells[rowIterator, 15].Value.ToString();
                                teacherDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == Religion).Select(x => x.Id).FirstOrDefault();
                                var Gender = workSheet.Cells[rowIterator, 16].Value.ToString();
                                teacherDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gender).Select(x => x.Id).FirstOrDefault();
                                teacherDetail.CellNo = workSheet.Cells[rowIterator, 9].Value.ToString();
                                teacherDetail.Landline = workSheet.Cells[rowIterator, 17].Value.ToString();
                                teacherDetail.Address = workSheet.Cells[rowIterator, 18].Value.ToString();

                                teacherDetail.SpouseName = workSheet.Cells[rowIterator, 19].Value.ToString();
                                teacherDetail.SpouseHighestDegree = workSheet.Cells[rowIterator, 20].Value.ToString();
                                teacherDetail.SpouseOccupation = workSheet.Cells[rowIterator, 21].Value.ToString();
                                teacherDetail.Spouse_Address = workSheet.Cells[rowIterator, 22].Value.ToString();
                                teacherDetail.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Management Staff").Select(x => x.Id).FirstOrDefault();
                                db.AspNetEmployees.Add(teacherDetail);

                                var roleStore = new RoleStore<IdentityRole>(context);
                                var roleManager = new RoleManager<IdentityRole>(roleStore);
                                var userStore = new UserStore<ApplicationUser>(context);
                                var userManager = new UserManager<ApplicationUser>(userStore);
                                userManager.AddToRole(user.Id, "Accountant");
                                db.SaveChanges();

                            }
                            else
                            {
                                dbTransaction.Dispose();

                                TempData["InternalError"]  =result.Errors.LastOrDefault() + "At Row #" + ErrorLine;
                                AddErrors(result);
                                return RedirectToAction("Create", "AspNetEmployees", model);
                            }

                        }
                        else if(position=="Guard")
                        {
                            AspNetEmployee teacherDetail = new AspNetEmployee();

                            var Fname = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var Mname = workSheet.Cells[rowIterator, 3].Value.ToString();
                            var Lname = workSheet.Cells[rowIterator, 4].Value.ToString();
                            teacherDetail.Name = Fname + " " + Mname + " " + Lname;
                            var UserName = workSheet.Cells[rowIterator, 5].Value.ToString();
                            var Email = workSheet.Cells[rowIterator, 6].Value.ToString();
                            var Password = workSheet.Cells[rowIterator, 7].Value.ToString();
                            var ConfirmPassword = workSheet.Cells[rowIterator, 8].Value.ToString();

                            var pos = workSheet.Cells[rowIterator, 1].Value.ToString();
                            teacherDetail.Position = db.AspNetEmployeePositions.Where(x => x.PositionName == position).Select(x => x.Id).FirstOrDefault();
                            teacherDetail.JoiningDate =workSheet.Cells[rowIterator, 10].Value.ToString();
                            teacherDetail.DateAvailable = workSheet.Cells[rowIterator, 11].Value.ToString();
                            teacherDetail.BirthDate =workSheet.Cells[rowIterator, 12].Value.ToString();
                            var brnch = workSheet.Cells[rowIterator, 13].Value.ToString();
                            teacherDetail.BranchId = db.AspNetBranches.Where(x => x.Name == brnch).Select(x => x.Id).FirstOrDefault();

                            var Nationality = workSheet.Cells[rowIterator, 14].Value.ToString();
                            teacherDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nationality).Select(x => x.Id).FirstOrDefault();
                            var Religion = workSheet.Cells[rowIterator, 15].Value.ToString();
                            teacherDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == Religion).Select(x => x.Id).FirstOrDefault();
                            var Gender = workSheet.Cells[rowIterator, 16].Value.ToString();
                            teacherDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gender).Select(x => x.Id).FirstOrDefault();
                            teacherDetail.CellNo = workSheet.Cells[rowIterator, 9].Value.ToString();
                            teacherDetail.Landline = workSheet.Cells[rowIterator, 17].Value.ToString();
                            teacherDetail.Address = workSheet.Cells[rowIterator, 18].Value.ToString();

                            teacherDetail.SpouseName = workSheet.Cells[rowIterator, 19].Value.ToString();
                            teacherDetail.SpouseHighestDegree = workSheet.Cells[rowIterator, 20].Value.ToString();
                            teacherDetail.SpouseOccupation = workSheet.Cells[rowIterator, 21].Value.ToString();
                            teacherDetail.Spouse_Address = workSheet.Cells[rowIterator, 22].Value.ToString();
                            teacherDetail.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Non Directive Staff").Select(x => x.Id).FirstOrDefault();
                            db.AspNetEmployees.Add(teacherDetail);
                            
                            db.SaveChanges();
                        }
                        else if(position=="Sweeper")
                        {
                            AspNetEmployee teacherDetail = new AspNetEmployee();

                            var Fname = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var Mname = workSheet.Cells[rowIterator, 3].Value.ToString();
                            var Lname = workSheet.Cells[rowIterator, 4].Value.ToString();
                            teacherDetail.Name = Fname + " " + Mname + " " + Lname;
                            var UserName = workSheet.Cells[rowIterator, 5].Value.ToString();
                            var Email = workSheet.Cells[rowIterator, 6].Value.ToString();
                            var Password = workSheet.Cells[rowIterator, 7].Value.ToString();
                            var ConfirmPassword = workSheet.Cells[rowIterator, 8].Value.ToString();

                            var pos = workSheet.Cells[rowIterator, 1].Value.ToString();
                            teacherDetail.Position = db.AspNetEmployeePositions.Where(x => x.PositionName == position).Select(x => x.Id).FirstOrDefault();
                            teacherDetail.JoiningDate =workSheet.Cells[rowIterator, 10].Value.ToString();
                            teacherDetail.DateAvailable = workSheet.Cells[rowIterator, 11].Value.ToString();
                            teacherDetail.BirthDate =workSheet.Cells[rowIterator, 12].Value.ToString();
                            var brnch = workSheet.Cells[rowIterator, 13].Value.ToString();
                            teacherDetail.BranchId = db.AspNetBranches.Where(x => x.Name == brnch).Select(x => x.Id).FirstOrDefault();

                            var Nationality = workSheet.Cells[rowIterator, 14].Value.ToString();
                            teacherDetail.NationalityId = db.AspNetNationalities.Where(x => x.Title == Nationality).Select(x => x.Id).FirstOrDefault();
                            var Religion = workSheet.Cells[rowIterator, 15].Value.ToString();
                            teacherDetail.ReligionId = db.AspNetReligions.Where(x => x.Title == Religion).Select(x => x.Id).FirstOrDefault();
                            var Gender = workSheet.Cells[rowIterator, 16].Value.ToString();
                            teacherDetail.GenderId = db.AspNetGenders.Where(x => x.Title == Gender).Select(x => x.Id).FirstOrDefault();
                            teacherDetail.CellNo = workSheet.Cells[rowIterator, 9].Value.ToString();
                            teacherDetail.Landline = workSheet.Cells[rowIterator, 17].Value.ToString();
                            teacherDetail.Address = workSheet.Cells[rowIterator, 18].Value.ToString();

                            teacherDetail.SpouseName = workSheet.Cells[rowIterator, 19].Value.ToString();
                            teacherDetail.SpouseHighestDegree = workSheet.Cells[rowIterator, 20].Value.ToString();
                            teacherDetail.SpouseOccupation = workSheet.Cells[rowIterator, 21].Value.ToString();
                            teacherDetail.Spouse_Address = workSheet.Cells[rowIterator, 22].Value.ToString();
                            teacherDetail.VirtualRoleId = db.AspNetVirtualRoles.Where(x => x.Name == "Non Directive Staff").Select(x => x.Id).FirstOrDefault();
                            db.AspNetEmployees.Add(teacherDetail);

                            db.SaveChanges();
                        }
                      
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("TeacherIndex", "AspNetEmployees");
                }
            }
            catch (Exception e)
            {

                 TempData["ErrorMessage"] = "Error! Empty Cell Found." + "At Row #" + ErrorLine;
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("Create","AspNetEmployees", model);

            }
        }

        public ActionResult TeacherClassSubject()
        {
            return View();
        }
        public ActionResult GetTeacherClassSubject()
        {
            var tcs = db.AspNetTeacher_Enrollments.ToList();
            List<AspNetTeacher_Enrollments> teacher_enrolment = new List<AspNetTeacher_Enrollments>();
            foreach (var item in tcs)
            {
                AspNetTeacher_Enrollments te = new AspNetTeacher_Enrollments();
                
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult TeacherClassSubject_Create()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TeacherClassSubjectfromFile(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                HttpPostedFileBase file = Request.Files["Teacher_Class_Course"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        AspNetTeacher_Enrollments classcourse = new AspNetTeacher_Enrollments();
                        var generic = new AspnetGenericBranchClassSubject();

                        var teacher = workSheet.Cells[rowIterator, 1].Value ?? null;
                        if(teacher == null)
                        {
                            continue;
                        }
                        var ID = db.AspNetUsers.Where(x => x.UserName == teacher.ToString()).Select(x => x.Id).FirstOrDefault();
                        classcourse.TeacherId = db.AspNetEmployees.Where(x => x.UserId == ID).Select(x => x.Id).FirstOrDefault();
                        var course = workSheet.Cells[rowIterator, 2].Value.ToString();
                        var classname = workSheet.Cells[rowIterator, 3].Value.ToString();
                        var courseid = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();
                        var classid = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                        classcourse.CourseId = db.AspNetClass_Courses.Where(x => x.ClassId == classid && x.CourseId==courseid).Select(x => x.Id).FirstOrDefault();
                        classcourse.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();

                        var section = workSheet.Cells[rowIterator, 4].Value.ToString();
                        var sectionID = db.AspNetSections.Where(x => x.Name == section).Select(x => x.Id).FirstOrDefault();

                        var branch = workSheet.Cells[rowIterator, 5].Value.ToString();
                        var branchid = db.AspNetBranches.Where(x => x.Name == branch).Select(x => x.Id).FirstOrDefault();
                        var BCid = db.AspNetBranch_Class.Where(x => x.BranchId == branchid && x.ClassId == classid).Select(x => x.Id).FirstOrDefault();
                        classcourse.SectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid && x.SectionId == sectionID).Select(x => x.Id).FirstOrDefault();

                        generic.BranchId = branchid;
                        generic.ClassId = classid;
                        generic.SectionId = sectionID;
                        generic.SubjectId = courseid;

                        var generictest = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == generic.BranchId && x.ClassId == generic.ClassId &&
                                            x.SectionId == generic.SectionId && x.SubjectId == generic.SubjectId).FirstOrDefault();

                        if(generictest == null)
                        {
                            db.AspnetGenericBranchClassSubjects.Add(generic);
                        }

                        db.AspNetTeacher_Enrollments.Add(classcourse);
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("TeacherClassSubject", "BranchAdmin");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("TeacherClassSubject_Create", "BranchAdmin", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BranchClass(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                HttpPostedFileBase file = Request.Files["Branch_Class"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        AspNetBranch_Class classcourse = new AspNetBranch_Class();

                        var Branch = workSheet.Cells[rowIterator, 1].Value.ToString();
                        classcourse.BranchId = db.AspNetBranches.Where(x => x.Name == Branch).Select(x => x.Id).FirstOrDefault();
                        var classname = workSheet.Cells[rowIterator, 2].Value.ToString();
                        classcourse.ClassId = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                        classcourse.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                        classcourse.IsActive = true;
                        db.AspNetBranch_Class.Add(classcourse);
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("BranchClass", "SubjectGroups");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("BranchClass", "SubjectGroups", model);
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BranchClassSection(RegisterViewModel model)
        {
            // if (ModelState.IsValid)
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                HttpPostedFileBase file = Request.Files["Branch_Class_Section"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                var teacherList = new List<RegisterViewModel>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        
                        AspNetBranchClass_Sections branchclasssection = new AspNetBranchClass_Sections();
                        AspNetBranch_Class classcourse = new AspNetBranch_Class();

                        var Branch = workSheet.Cells[rowIterator, 1].Value.ToString();
                        var branchid =  classcourse.BranchId = db.AspNetBranches.Where(x => x.Name == Branch).Select(x => x.Id).FirstOrDefault();
                        var classname = workSheet.Cells[rowIterator, 2].Value.ToString();
                        var classid =  classcourse.ClassId = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                        classcourse.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                        classcourse.IsActive = true;
                   
                       var branchclassid= db.AspNetBranch_Class.Where(x=>x.ClassId == classid && x.BranchId == branchid).FirstOrDefault().Id;
                        branchclasssection.BranchClassId = branchclassid;
                        var section = workSheet.Cells[rowIterator, 3].Value.ToString();
                        branchclasssection.SectionId = db.AspNetSections.Where(x => x.Name ==section).Select(x => x.Id).FirstOrDefault();
                        branchclasssection.IsActive = true;
                        db.AspNetBranchClass_Sections.Add(branchclasssection);
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                    return RedirectToAction("BranchClassSection", "SubjectGroups");
                }
            }
            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();
                return RedirectToAction("BranchClassSection", "SubjectGroups", model);
            }
        }

        public ActionResult TeacherDetails(int id)
        {
            var teacher = db.AspNetTeachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound("Could not find the teacher with given id.");
            }
            var employee = db.AspNetEmployees.Find(teacher.EmployeeId);
            var model = new TeacherUserEmployeeViewModel
            {
                BranchId = employee.BranchId?? default(int),
                Address = employee.Address,
                BasicSalary = employee.BasicSalary,
                //BirthDate = employee.BirthDate,
                CellNo = employee.CellNo,
                Email = teacher.AspNetUser.Email,
                EOP = employee.EOP,
                GenderId = employee.GenderId,
                GrossSalary = employee.GrossSalary,
                //JoiningDate = employee.JoiningDate,
                Landline = employee.Landline,
                MedicalAllowance = employee.MedicalAllowance,
                Name = employee.Name,
                NationalityId = employee.NationalityId,
                ProvidedFund = employee.ProvidedFund,
                RegistrationNo = teacher.RegistrationNo,
                ReligionId = employee.ReligionId,
                Tax = employee.Tax,
                Username = teacher.AspNetUser.UserName
            };
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;

            if (teacher.AspNetEmployee.File == null)
            {
                ViewBag.ImagePath = null;
            }
            else
            {
                var imagePath = Path.Combine("~/Content/Profile/Teacher", teacher.AspNetEmployee.File);
                ViewBag.ImagePath = imagePath;
            }

            return View(model);
        }

        public ActionResult EditTeacher(int id)
        {
            var teacher = db.AspNetTeachers.Find(id);
            if(teacher == null)
            {
                return HttpNotFound();
            }
            var employee = db.AspNetEmployees.Find(teacher.EmployeeId);
            var model = new TeacherUserEmployeeViewModel
            {
                BranchId = employee.BranchId?? default(int),
                Address = employee.Address,
                BasicSalary = employee.BasicSalary,
                ////BirthDate = employee.BirthDate,
                CellNo = employee.CellNo,
                Email = teacher.AspNetUser.Email,
                EOP = employee.EOP,
                GenderId = employee.GenderId,
                GrossSalary = employee.GrossSalary,
                //JoiningDate = employee.JoiningDate,
                Landline = employee.Landline,
                MedicalAllowance = employee.MedicalAllowance,
                Name = employee.Name,
                NationalityId = employee.NationalityId,
                ProvidedFund = employee.ProvidedFund,
                RegistrationNo = teacher.RegistrationNo,
                ReligionId = employee.ReligionId,
                Tax = employee.Tax,
                Username = teacher.AspNetUser.UserName
            };
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditTeacher(TeacherUserEmployeeViewModel model)
        {
            var id = Convert.ToInt32(Request.Form["Id"]);
            if (ModelState.IsValid)
            {
                var teacher = db.AspNetTeachers.Find(id);

                teacher.AspNetEmployee.Name = model.Name;
                //teacher.AspNetEmployee.BirthDate = model.BirthDate;
                teacher.AspNetEmployee.NationalityId = model.NationalityId;
                teacher.AspNetEmployee.ReligionId = model.ReligionId;
                teacher.AspNetEmployee.GenderId = model.GenderId;
                teacher.AspNetEmployee.CellNo = model.CellNo;
                teacher.AspNetEmployee.Landline = model.Landline;
                teacher.AspNetEmployee.GrossSalary = model.GrossSalary;
                teacher.AspNetEmployee.BasicSalary = model.BasicSalary;
                teacher.AspNetEmployee.MedicalAllowance = model.MedicalAllowance;
                teacher.AspNetEmployee.ProvidedFund = model.ProvidedFund;
                teacher.AspNetEmployee.EOP = model.EOP;
                //teacher.AspNetEmployee.JoiningDate = model.JoiningDate;
                teacher.AspNetEmployee.BranchId = model.BranchId;
                teacher.AspNetEmployee.Address = model.Address;
                teacher.AspNetEmployee.Tax = model.Tax;

                teacher.AspNetUser.Email = model.Email;
                teacher.AspNetUser.UserName = model.Username;

                teacher.RegistrationNo = model.RegistrationNo;

                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TeacherDetails", new { id });
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteTeacher(int id)
        {
            try
            {
                var teacher = db.AspNetTeachers.Find(id);
                if (teacher == null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var applicationUser = userManager.FindById(teacher.UserId);
                var removeFromRoleResult = userManager.RemoveFromRole(teacher.UserId, "Teacher");
                if (!removeFromRoleResult.Succeeded)
                {
                    string errors = "";
                    foreach (var error in removeFromRoleResult.Errors)
                    {
                        errors += error + Environment.NewLine;
                    }
                    throw new Exception(errors);
                }
                db.Entry(teacher.AspNetEmployee).State = EntityState.Deleted;
                db.Entry(teacher).State = EntityState.Deleted;
                db.SaveChanges();
                var deleteResult = userManager.Delete(applicationUser);
                if (!deleteResult.Succeeded)
                {
                    string errors = "";
                    foreach (var error in deleteResult.Errors)
                    {
                        errors += error + Environment.NewLine;
                    }
                    throw new Exception(errors);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Accountants

        public ActionResult AccountantsList()
        {
            var loggedInUserId = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var accountantsList = new List<AspNetAccountant>();
            foreach (var branchId in branchIds)
            {
                var accountantsInCurrentBranch = db.AspNetAccountants
                    .Where(accountant => accountant.AspNetEmployee.BranchId == branchId)
                    .AsEnumerable();
                accountantsList.AddRange(accountantsInCurrentBranch);
            }
            return View(accountantsList);
        }

        public ActionResult CreateAccountant()
        {
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAccountant(AccountantUserEmployeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.Password.Equals(model.ConfirmPassword))
                    {
                        throw new Exception("Password and Confirm Password fields don't match.");
                    }

                    ApplicationDbContext context = new ApplicationDbContext();
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    var user = new ApplicationUser
                    {
                        UserName = model.Username,
                        Email = model.Email
                    };

                    var userCreatedStatus = userManager.Create(user, model.Password);
                    if (userCreatedStatus.Succeeded)
                    {
                        var addToRoleStatus = userManager.AddToRole(user.Id, "Accountant");
                        if (!addToRoleStatus.Succeeded)
                        {
                            throw new Exception("Cannot add user to the accountant role.");
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot create the application user for the accountant.");
                    }

                    var image = Request.Files[0];
                    string imageName = null;
                    string serverPath = null;
                    string fullPath = null;
                    string newPath = null;
                    if (image != null && image.ContentLength > 0)
                    {
                        imageName = Path.GetFileName(image.FileName);
                        serverPath = Server.MapPath("~/Content/Profile/Accountant");
                        fullPath = Path.Combine(serverPath, imageName);
                        image.SaveAs(fullPath);
                    }

                    var employee = new AspNetEmployee
                    {
                        Name = model.Name,
                        //BirthDate = model.BirthDate,
                        //JoiningDate = model.JoiningDate,
                        NationalityId = model.NationalityId,
                        ReligionId = model.ReligionId,
                        GenderId = model.GenderId,
                        Address = model.Address,
                        Landline = model.Landline,
                        CellNo = model.CellNo,
                        GrossSalary = model.GrossSalary,
                        BasicSalary = model.BasicSalary,
                        MedicalAllowance = model.MedicalAllowance,
                        ProvidedFund = model.ProvidedFund,
                        EOP = model.EOP,
                        Tax = model.Tax,
                        BranchId = model.BranchId,
                        File = newPath
                    };

                    db.AspNetEmployees.Add(employee);
                    db.SaveChanges();

                    var accountant = new AspNetAccountant
                    {
                        UserId = user.Id,
                        RegistrationNo = model.RegistrationNo,
                        EmployeeId = employee.Id
                    };

                    db.AspNetAccountants.Add(accountant);
                    db.SaveChanges();
                    if (image != null && image.ContentLength > 0)
                    {
                        newPath = Path.Combine(serverPath, $"{accountant.Id}.{Path.GetExtension(fullPath)}");
                        System.IO.File.Move(fullPath, newPath);
                        accountant.AspNetEmployee.File = Path.GetFileName(newPath);
                        db.Entry(accountant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("AccountantsList");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}...{ex?.InnerException?.Message}");
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);

            return View(model);
        }

        public ActionResult AccountantDetails(int id)
        {
            var accountant = db.AspNetAccountants.Find(id);
            if (accountant == null)
            {
                return HttpNotFound("Could not find the accountant with given id.");
            }
            var employee = db.AspNetEmployees.Find(accountant.EmployeeId);
            var model = new AccountantUserEmployeeViewModel
            {
                BranchId = employee.BranchId?? default(int),
                Address = employee.Address,
                BasicSalary = employee.BasicSalary,
                //BirthDate = employee.BirthDate,
                CellNo = employee.CellNo,
                Email = accountant.AspNetUser.Email,
                EOP = employee.EOP,
                GenderId = employee.GenderId,
                GrossSalary = employee.GrossSalary,
                //JoiningDate = employee.JoiningDate,
                Landline = employee.Landline,
                MedicalAllowance = employee.MedicalAllowance,
                Name = employee.Name,
                NationalityId = employee.NationalityId,
                ProvidedFund = employee.ProvidedFund,
                RegistrationNo = accountant.RegistrationNo,
                ReligionId = employee.ReligionId,
                Tax = employee.Tax,
                Username = accountant.AspNetUser.UserName
            };
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;

            if (accountant.AspNetEmployee.File == null)
            {
                ViewBag.ImagePath = null;
            }
            else
            {
                var imagePath = Path.Combine("~/Content/Profile/Accountant", accountant.AspNetEmployee.File);
                ViewBag.ImagePath = imagePath;
            }

            return View(model);
        }

        public ActionResult EditAccountant(int id)
        {
            var accountant = db.AspNetAccountants.Find(id);
            if (accountant == null)
            {
                return HttpNotFound();
            }
            var employee = db.AspNetEmployees.Find(accountant.EmployeeId);
            var model = new AccountantUserEmployeeViewModel
            {
                BranchId = employee.BranchId?? default(int),
                Address = employee.Address,
                BasicSalary = employee.BasicSalary,
                //BirthDate = employee.BirthDate,
                CellNo = employee.CellNo,
                Email = accountant.AspNetUser.Email,
                EOP = employee.EOP,
                GenderId = employee.GenderId,
                GrossSalary = employee.GrossSalary,
                //JoiningDate = employee.JoiningDate,
                Landline = employee.Landline,
                MedicalAllowance = employee.MedicalAllowance,
                Name = employee.Name,
                NationalityId = employee.NationalityId,
                ProvidedFund = employee.ProvidedFund,
                RegistrationNo = accountant.RegistrationNo,
                ReligionId = employee.ReligionId,
                Tax = employee.Tax,
                Username = accountant.AspNetUser.UserName
            };
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAccountant(AccountantUserEmployeeViewModel model)
        {
            var id = Convert.ToInt32(Request.Form["Id"]);

            if (ModelState.IsValid)
            {
                var accountant = db.AspNetAccountants.Find(id);

                accountant.AspNetEmployee.Name = model.Name;
                //accountant.AspNetEmployee.BirthDate = model.BirthDate;
                accountant.AspNetEmployee.NationalityId = model.NationalityId;
                accountant.AspNetEmployee.ReligionId = model.ReligionId;
                accountant.AspNetEmployee.GenderId = model.GenderId;
                accountant.AspNetEmployee.CellNo = model.CellNo;
                accountant.AspNetEmployee.Landline = model.Landline;
                accountant.AspNetEmployee.GrossSalary = model.GrossSalary;
                accountant.AspNetEmployee.BasicSalary = model.BasicSalary;
                accountant.AspNetEmployee.MedicalAllowance = model.MedicalAllowance;
                accountant.AspNetEmployee.ProvidedFund = model.ProvidedFund;
                accountant.AspNetEmployee.EOP = model.EOP;
                //accountant.AspNetEmployee.JoiningDate = model.JoiningDate;
                accountant.AspNetEmployee.BranchId = model.BranchId;
                accountant.AspNetEmployee.Address = model.Address;
                accountant.AspNetEmployee.Tax = model.Tax;

                accountant.AspNetUser.Email = model.Email;
                accountant.AspNetUser.UserName = model.Username;

                accountant.RegistrationNo = model.RegistrationNo;

                db.Entry(accountant.AspNetEmployee).State = EntityState.Modified;
                db.Entry(accountant.AspNetUser).State = EntityState.Modified;
                db.Entry(accountant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AccountantDetails", new { id });
            }

            ModelState.AddModelError("", "Model state is invalid");
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteAccountant(int id)
        {
            try
            {
                var accountant = db.AspNetAccountants.Find(id);
                if (accountant == null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var applicationUser = userManager.FindById(accountant.UserId);
                var removeFromRoleResult = userManager.RemoveFromRole(accountant.UserId, "Accountant");
                if (!removeFromRoleResult.Succeeded)
                {
                    string errors = "";
                    foreach (var error in removeFromRoleResult.Errors)
                    {
                        errors += error + Environment.NewLine;
                    }
                    throw new Exception(errors);
                }
                db.Entry(accountant.AspNetEmployee).State = EntityState.Deleted;
                db.Entry(accountant).State = EntityState.Deleted;
                db.SaveChanges();
                var deleteResult = userManager.Delete(applicationUser);
                if (!deleteResult.Succeeded)
                {
                    string errors = "";
                    foreach (var error in deleteResult.Errors)
                    {
                        errors += error + Environment.NewLine;
                    }
                    throw new Exception(errors);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Parents

        public ActionResult ParentsList()
        {
            var loggedInUserId = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var studentsInCurrentBranch = new List<AspNetStudent>();
            branchIds.ForEach(branchId => studentsInCurrentBranch.AddRange(db.AspNetStudents
                .Where(student => branchIds.Contains(student.BranchId?? default(int)))));

            var parentsInCurrentBranch = new List<AspNetParent>();
         //   studentsInCurrentBranch.ForEach(student => parentsInCurrentBranch.Add(student.AspNetParent));
            parentsInCurrentBranch = parentsInCurrentBranch.Distinct().ToList();

            return View(parentsInCurrentBranch);
        }

        #endregion

        #region Courses

        public ActionResult CourseList()
        {
            var branchIds = GetAdministratedBranchIds();
            var classes = new List<AspNetClass>();
            foreach (var branchId in branchIds)
            {
                classes.AddRange(db.AspNetBranch_Class
                .Where(branchClass => branchClass.BranchId == branchId)
                .Select(branchClass => branchClass.AspNetClass));
            }

            classes = classes.Distinct().ToList();
            var courses = new List<AspNetCours>();
            classes.ForEach(@class => courses.AddRange(db.AspNetClass_Courses
                .Where(classCourse => classCourse.ClassId == @class.Id)
                .Select(classCourse => classCourse.AspNetCours)));
            courses = courses.Distinct().ToList();
            return View(courses);
        }


        public ActionResult CreateCourse()
        {
            var branchIds = GetAdministratedBranchIds();
            var departmentList = db.AspNetDepartments.ToList();

            var departmentSelectList = new List<SelectListItem>();
            foreach (var item in departmentList)
            {
                departmentSelectList.Add(new SelectListItem
                {
                    Text = $"{item.Name}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Departments = departmentSelectList;

            //var classIds = GetClassIdsFromBranchIds(branchIds);
            //classIds = classIds.Distinct().ToList();
            //var classes = new List<AspNetClass>();
            //classIds.ForEach(classId => classes.Add(db.AspNetClasses.Find(classId)));

            //var classItems = new List<SelectListItem>();
            //foreach (var item in classes)
            //{
            //    classItems.Add(new SelectListItem
            //    {
            //        Text = $"{item.Name}",
            //        Value = item.Id.ToString()
            //    });
            //}
            //ViewBag.Classes = classItems;


            return View();
        }

        [HttpPost]
        public ActionResult CreateCourse(AspNetCours course)
        {
            List<int> classIds = null;
            try
            {
                var logMessage = "";
                var startTime = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.AspNetCourses.Add(course);
                    db.SaveChanges();
                  //  logMessage += $"Course added in department {course.DepartmentId}. Course id: {course.Id}.";
                    //classIds = Request.Form["Classes"]
                    //    ?.Split(',')
                    //    ?.Select(classId => Convert.ToInt32(classId))
                    //    ?.ToList();
                    //if (classIds != null)
                    //{
                    //    var isMadatory = Request.Form["IsMandatory"].Contains(",");
                    //    classIds.ForEach(classId =>
                    //    {
                    //        db.AspNetClass_Courses.Add(new AspNetClass_Courses
                    //        {
                    //            ClassId = classId,
                    //            CourseId = course.Id,
                    //            IsMandatory = isMadatory
                    //        });
                    //        logMessage += $"Course {course.Id} added to class {classId}.";
                    //    });
                    //    db.SaveChanges();
                    //}
                    var endTime = DateTime.Now;
                    CreateLog(logMessage, startTime, endTime);
                    return RedirectToAction("CourseList");
                }
                else
                {
                    ModelState.AddModelError("", "Model State is invalid");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred. Details: " + ex.Message);
            }

            var branchIds = GetAdministratedBranchIds();
            var departmentList = db.AspNetDepartments.ToList();

            var departmentSelectList = new List<SelectListItem>();
            foreach (var item in departmentList)
            {
                departmentSelectList.Add(new SelectListItem
                {
                    Text = $"{item.Name}",
                    Value = item.Id.ToString(),
                   // Selected = item.Id == course.DepartmentId
                });
            }
            ViewBag.Departments = departmentSelectList;

            //var classes = new List<AspNetClass>();
            //var classItems = new List<SelectListItem>();
            //foreach (var item in classes)
            //{
            //    classItems.Add(new SelectListItem
            //    {
            //        Text = $"{item.Name}, {item.AspNetBranch_Class.Where(bc => bc.ClassId == item.Id).Select(bc => bc.AspNetBranch).First().Name}",
            //        Value = item.Id.ToString(),
            //        Selected = classIds == null ? false : classIds.Contains(item.Id)
            //    });
            //}
            //ViewBag.Classes = classItems;

            //CourseViewModel model = new CourseViewModel
            //{
            //    DepartmentId = course.DepartmentId,
            //    IsMandatory = false,
            //    Name = course.Name
            //};

            return View(course);
        }

        public ActionResult CourseDetails(int id)
        {
            var course = db.AspNetCourses.Find(id);
            if(course == null)
            {
                return HttpNotFound();
            }
            var classIds = course.AspNetClass_Courses
                .Where(classCourse => classCourse.CourseId == course.Id)
                .Select(classCourse => classCourse.ClassId)
                .ToList();
            var classes = new List<AspNetClass>();
            var branches = new List<AspNetBranch>();
            classIds.ForEach(classId =>
            {
                classes.Add(db.AspNetClasses
                    .Find(classId));
                branches.AddRange(db.AspNetBranch_Class
                    .Where(branchClass => branchClass.ClassId == classId)
                    .Select(branchClass => branchClass.AspNetBranch)
                    .ToList());
            });
            ViewBag.Classes = classes;
            ViewBag.Branches = branches.Distinct().ToList();
            return View(course);
        }

        public ActionResult EditCourse(int id)
        {
            var course = db.AspNetCourses.Find(id);
            if(course == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.AspNetDepartments, "Id", "Name");
            return View(course);
        }

        [HttpPost]
        public ActionResult EditCourse(AspNetCours model)
        {
            if(ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseList");
            }
            ViewBag.DepartmentId = new SelectList(db.AspNetDepartments, "Id", "Name");
            return View(model);
        }

        #endregion

        #region ClassCourses

        public ActionResult CreateClassCourse()
        {
            ViewBag.ClassId = new SelectList(db.AspNetClasses, "Id", "Name");
            ViewBag.Courses = new MultiSelectList(db.AspNetCourses.OrderBy(c => c.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ActionName("CreateClassCourse")]
        public ActionResult CreateClassCourseSubmit()
        {
            var classId = Convert.ToInt32(Request.Form["ClassId"]);
            var courseIds = Request.Form["Courses"]
                .Split(',')
                .Select(id => Convert.ToInt32(id))
                .ToList();
            foreach (var courseId in courseIds)
            {
                var mandatory = Request[$"course_{courseId}"] != null;
                var courseToAdd = new AspNetClass_Courses
                {
                    ClassId = classId,
                    CourseId = courseId,
                    IsMandatory = mandatory
                };
                db.AspNetClass_Courses.Add(courseToAdd);
            }
            db.SaveChanges();

            return null;
        }

        #endregion

        #region Course Packages

        public ActionResult PackageList()
        {
            return View(db.AspNetPackages.ToList());
        }


        public ActionResult CreatePackage()
        {
            var departments = new List<SelectListItem>();
            foreach (var item in db.AspNetDepartments.ToList())
            {
                departments.Add(new SelectListItem
                {
                    //Text = $"{item.Name}, {item.AspNetSchool.Name}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Departments = departments;

            var courses = new List<SelectListItem>();
            foreach (var item in db.AspNetCourses.ToList())
            {
                courses.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Courses = courses;

            ViewBag.CourseList = db.AspNetCourses.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult CreatePackage(AspNetPackage packge)
        {

            if (ModelState.IsValid)
            {
                var logMessage = "";
                var startTime = DateTime.Now;
                db.AspNetPackages.Add(packge);
                logMessage += $"Package added. Package id: {packge.Id}.";
                var courseIds = Request.Form["courselist"].Split(',');
                foreach (var courseId in courseIds)
                {
                    var cp = new AspNetCoursePackage
                    {
                        AspNetPackage = packge,
                        CourseId = int.Parse(courseId)
                    };
                    db.AspNetCoursePackages.Add(cp);
                    logMessage += $" Course {courseId} added to package {packge.Id}.";
                }
                db.SaveChanges();
                var endTime = DateTime.Now;
                CreateLog(logMessage, startTime, endTime);
                return RedirectToAction("PackageList");
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        public ActionResult _PackageDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.PackageName = db.AspNetPackages
                .Where(package => package.Id == id)
                .Select(package => package.Title)
                .FirstOrDefault();
            return PartialView(db.AspNetCoursePackages.Where(c => c.PackageId == id).ToList());
        }

        #endregion

        #region Employees

        public ActionResult CreateEmployee1()
        {
            ViewBag.PositionId = new SelectList(db.AspNetEmployeePositions, "Id", "PositionName");

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");

            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");

            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");

            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name");

            var currentlyLoggedInUser = User.Identity.GetUserId();
            var branchIds = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
            var branchList = new List<AspNetBranch>();
            foreach (var id in branchIds)
            {
                var currentBranch = db.AspNetBranches
                    .Where(branch => branch.Id == id)
                    .First();
                branchList.Add(currentBranch);
            }
            var branches = new List<SelectListItem>();
            foreach (var item in branchList)
            {
                branches.Add(new SelectListItem
                {
                    Text = $"{item.Name}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Branches = branches;


            return View();
        }
        
        public ActionResult CreateEmployee()
        {
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title");
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title");
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title");

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");

            return View(@"~\Views\BranchAdmin\C_Employee.cshtml");
        }

        [HttpPost]
        public ActionResult CreateEmployee(AspNetEmployee model)
        {
            if(ModelState.IsValid)
            {
                db.AspNetEmployees.Add(model);
                db.SaveChanges();
                var image = Request.Files[0];
                string imageName = null;
                string serverPath = null;
                string fullPath = null;
                string newPath = null;
                if (image != null && image.ContentLength > 0)
                {
                    imageName = Path.GetFileName(image.FileName);
                    serverPath = Server.MapPath("~/Content/Profile/Employee");
                    fullPath = Path.Combine(serverPath, imageName);
                    image.SaveAs(fullPath);
                }

                if (image != null && image.ContentLength > 0)
                {
                    newPath = Path.Combine(serverPath, $"{model.Id}.{Path.GetExtension(fullPath)}");
                    System.IO.File.Move(fullPath, newPath);
                    model.File = Path.GetFileName(newPath);
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("ViewEmployee");
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", model.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", model.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", model.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);

            return View(@"~\Views\BranchAdmin\C_Employee.cshtml", model);
        }

        public ActionResult ViewEmployee()
        {
            return View(db.AspNetEmployees.ToList());
        }


        public ActionResult EmployeeDetails(int id)
        {
            var employee = db.AspNetEmployees.Find(id);
            if(employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", employee.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", employee.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", employee.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", employee.BranchId);
            if (employee.File == null)
            {
                ViewBag.ImagePath = null;
            }
            else
            {
                var imagePath = Path.Combine("~/Content/Profile/Employee", employee.File);
                ViewBag.ImagePath = imagePath;
            }
            return View(employee);
        }

        public ActionResult EditEmployee(int id)
        {
            var employee = db.AspNetEmployees.Find(id);
            if(employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", employee.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", employee.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", employee.GenderId);

            var branchIds = GetAdministratedBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", employee.BranchId);
            return View(employee);
        }

        [HttpPost]
        public ActionResult EditEmployee(AspNetEmployee model)
        {
            if(ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("EmployeeDetails", new { id = model.Id });
        }

        [HttpPost]
        public JsonResult DeleteEmployee(int id)
        {
            var employee = db.AspNetEmployees.Find(id);
            if(employee == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            db.Entry(employee).State = EntityState.Deleted;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region StudentEnrollments

        public ActionResult StudentEnrollmentList()
        {
            var branchIds = GetAdministratedBranchIds();
            var enrollments = db.AspNetStudent_Enrollments
                .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList();
            return View(enrollments);
        }
             
        public ActionResult GetSubjectList()
        {
            var branchIds = GetAdministratedBranchIds();
            var enrollments = db.AspNetStudent_Enrollments 
                .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId) && enrollment.StudentId == 1)
                .ToList();
            return View(enrollments);
        }

        public ActionResult CreateStudentEnrollment()
        {
            var branchIds = GetAdministratedBranchIds();
            ViewBag.StudentId = new SelectList(db.AspNetStudents
                .Where(student => branchIds.Contains(student.BranchId?? default(int))), "Id", "Name");
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => classes.Contains(cc.ClassId)).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses.Where(c => false), "Id", "Name");
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"{bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}, {bcs.AspNetBranch_Class.AspNetBranch.Name}",
                    Value = bcs.Id.ToString()
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year");
            ViewBag.BranchClassSections = branchClassSections;
            return View();
        }

        public JsonResult GetSectionStudents(int SectionId)
        {

            var students = db.AspNetStudent_Enrollments.Where(x => x.AspNetBranchClass_Sections.Id == SectionId).Select(x => new
            {
                Name = x.AspNetStudent.Name,
                Id = x.AspNetStudent.Id
            }).ToList();

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateStudentEnrollment(AspNetStudent_Enrollments enrollment)
        {
            db.AspNetStudent_Enrollments.Add(enrollment);
            db.SaveChanges();
            return RedirectToAction("StudentEnrollmentList");
        }

        public ActionResult StudentEnrollmentDetails(int id)
        {
            var enrollment = db.AspNetStudent_Enrollments.Find(id);
            if(enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        public ActionResult EditStudentEnrollment(int id)
        {
            var enrollment = db.AspNetStudent_Enrollments.Find(id);
            if(enrollment == null)
            {
                return HttpNotFound();
            }
            var branchIds = GetAdministratedBranchIds();
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => classes.Contains(cc.ClassId)).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses.Distinct(), "Id", "Name", enrollment.CourseId);
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"Section {bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}",
                    Value = bcs.Id.ToString(),
                    Selected = bcs.Id == enrollment.SectionId
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", enrollment.SessionId);
            return View(enrollment);
        }

        [HttpPost]
        public ActionResult EditStudentEnrollment(AspNetStudent_Enrollments enrollment)
        {
            if(ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentEnrollmentList");
            }
            var branchIds = GetAdministratedBranchIds();
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => classes.Contains(cc.ClassId)).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses.Distinct(), "Id", "Name", enrollment.CourseId);
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"Section {bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}",
                    Value = bcs.Id.ToString(),
                    Selected = bcs.Id == enrollment.SectionId
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", enrollment.SessionId);
            return View(enrollment);
        }

        [HttpPost]
        public JsonResult DeleteStudentEnrollment(int id)
        {
            var enrollment = db.AspNetStudent_Enrollments.Find(id);
            if(enrollment == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Entry(enrollment).State = EntityState.Deleted;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region StudentPromotion

        public ActionResult PromoteStudent(int studentId)
        {
            AspNetStudent_Enrollments enrollment = null;
            try
            {
                enrollment = db.AspNetStudent_Enrollments
                    .Where(enroll => enroll.StudentId == studentId)
                    .First();
            }
            catch
            {
                return HttpNotFound();
            }
            var nextClass = db.AspNetClasses.Find(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.NextClassId);
            if(nextClass == null)
            {
                return Content("Can't promote to next class since there is no next class available for the current class.");
            }
            var nextSession = db.AspNetSessions.Find(enrollment.AspNetSession.Next);
            if(nextSession == null)
            {
                return Content("Can't promote to next session since there is no next session availabe for the current session.");
            }
            var branchClassSectionsWithNextClass = db.AspNetBranchClass_Sections.Where(bcs => bcs.AspNetBranch_Class.ClassId == nextClass.Id).ToList();
            var branchClassSelect = new List<SelectListItem>();
            foreach (var bcs in branchClassSectionsWithNextClass)
            {
                branchClassSelect.Add(new SelectListItem
                {
                    Text = $"Class {bcs.AspNetBranch_Class.AspNetClass.Name}, {bcs.AspNetSection.Name}, {bcs.AspNetBranch_Class.AspNetBranch.Name}",
                    Value = bcs.Id.ToString()
                });
            }
            ViewBag.BranchClassSectionId = branchClassSelect;

            var model = new StudentPromotionViewModel
            {
                StudentId = studentId,
                NextClassId = nextClass.Id,
                NextSessionId = nextSession.Id
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PromoteStudent(StudentPromotionViewModel model)
        {
            if(ModelState.IsValid)
            {
                var previousEnrollments = db.AspNetStudent_Enrollments
                    .Where(enrollment => enrollment.StudentId == model.StudentId)
                    .ToList();
                foreach (var enrollment in previousEnrollments)
                {
                    var history = new AspNetStudent_Histories
                    {
                        StudentId = enrollment.StudentId,
                        SessionId = enrollment.SessionId,
                        SectionId = enrollment.SectionId,
                        CourseId = enrollment.CourseId,
                        AdmissionStatusId = true
                    };
                    db.AspNetStudent_Histories.Add(history);
                    db.Entry(enrollment).State = EntityState.Deleted;
                }
                db.SaveChanges();

                var freshEnrollments = new List<AspNetStudent_Enrollments>();
                var mandatoryCourses = db.AspNetClass_Courses
                    .Where(classCourse => classCourse.ClassId == model.NextClassId && classCourse.IsMandatory)
                    .Select(classCourse => classCourse.AspNetCours)
                    .ToList();
                foreach (var course in mandatoryCourses)
                {
                    var enrollment = new AspNetStudent_Enrollments
                    {
                        StudentId = model.StudentId,
                        CourseId = course.Id,
                        SectionId = model.BranchClassSectionId,
                        SessionId = model.NextSessionId
                    };
                    db.AspNetStudent_Enrollments.Add(enrollment);
                }
                db.SaveChanges();

                return RedirectToAction("StudentEnrollmentList");

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult DemoteStudent()
        {
            List<AspNetStudent_Enrollments> previousEnrollments = null;
            int studentId = Convert.ToInt32(Request.Form["DemoteStudentId"]);
            try
            {
                previousEnrollments = db.AspNetStudent_Enrollments
                    .Where(enroll => enroll.StudentId == studentId)
                    .ToList();
            }
            catch
            {
                return HttpNotFound();
            }
            var nextSession = db.AspNetSessions.Find(previousEnrollments.First().AspNetSession.Next);
            if (nextSession == null)
            {
                return Content("Can't demote to next session since there is no next session availabe for the current session.");
            }
            var currentEnrollmentInfo = new StudentPromotionViewModel
            {
                StudentId = studentId,
                NextClassId = previousEnrollments.First().AspNetBranchClass_Sections.AspNetBranch_Class.ClassId,
                BranchClassSectionId = previousEnrollments.First().SectionId,
                NextSessionId = nextSession.Id
            };
            foreach (var enrollment in previousEnrollments)
            {
                var history = new AspNetStudent_Histories
                {
                    StudentId = enrollment.StudentId,
                    SessionId = enrollment.SessionId,
                    SectionId = enrollment.SectionId,
                    CourseId = enrollment.CourseId,
                    AdmissionStatusId = true
                };
                db.AspNetStudent_Histories.Add(history);
                db.Entry(enrollment).State = EntityState.Deleted;
            }
            db.SaveChanges();

            var freshEnrollments = new List<AspNetStudent_Enrollments>();
            var mandatoryCourses = db.AspNetClass_Courses
                .Where(classCourse => classCourse.ClassId == currentEnrollmentInfo.NextClassId && classCourse.IsMandatory)
                .Select(classCourse => classCourse.AspNetCours)
                .ToList();
            foreach (var course in mandatoryCourses)
            {
                var enrollment = new AspNetStudent_Enrollments
                {
                    StudentId = currentEnrollmentInfo.StudentId,
                    CourseId = course.Id,
                    SectionId = currentEnrollmentInfo.BranchClassSectionId,
                    SessionId = currentEnrollmentInfo.NextSessionId
                };
                db.AspNetStudent_Enrollments.Add(enrollment);
            }
            db.SaveChanges();

            return RedirectToAction("StudentEnrollmentList");
        }

        #endregion

        #region TeacherEnrollments

        public ActionResult TeacherEnrollmentList()
        {
            var branchIds = GetAdministratedBranchIds();
            var enrollments = db.AspNetTeacher_Enrollments
                .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList();
            return View(enrollments);
        }

        public ActionResult CreateTeacherEnrollment()
        {
            var branchIds = GetAdministratedBranchIds();
            ViewBag.TeacherId = new SelectList(db.AspNetTeachers
                .Where(teacher => branchIds.Contains(teacher.AspNetEmployee.BranchId??default(int))).Select(teacher => new { teacher.Id, teacher.AspNetEmployee.Name }), "Id", "Name");
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => false).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses, "Id", "Name");
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"{bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}, {bcs.AspNetBranch_Class.AspNetBranch.Name}",
                    Value = bcs.Id.ToString()
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year");
            ViewBag.BranchClassSections = branchClassSections;
            return View();
        }

        [HttpPost]
        public ActionResult CreateTeacherEnrollment(AspNetTeacher_Enrollments model)
        {
            if(ModelState.IsValid)
            {
                db.AspNetTeacher_Enrollments.Add(model);
                db.SaveChanges();
                return RedirectToAction("TeacherEnrollmentList");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult TeacherEnrollmentDetails(int id)
        {
            var enrollment = db.AspNetTeacher_Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        public ActionResult EditTeacherEnrollment(int id)
        {
            var enrollment = db.AspNetTeacher_Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            var branchIds = GetAdministratedBranchIds();
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => classes.Contains(cc.ClassId)).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses.Distinct(), "Id", "Name", enrollment.CourseId);
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"Section {bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}",
                    Value = bcs.Id.ToString(),
                    Selected = bcs.Id == enrollment.SectionId
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", enrollment.SessionId);
            return View(enrollment);
        }

        [HttpPost]
        public ActionResult EditTeacherEnrollment(AspNetTeacher_Enrollments enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TeacherEnrollmentList");
            }
            var branchIds = GetAdministratedBranchIds();
            var classes = db.AspNetBranch_Class.Where(bc => branchIds.Contains(bc.BranchId)).Select(bc => bc.ClassId).ToList();
            var courses = db.AspNetClass_Courses.Where(cc => classes.Contains(cc.ClassId)).Select(cc => cc.AspNetCours);
            ViewBag.CourseId = new SelectList(courses.Distinct(), "Id", "Name", enrollment.CourseId);
            var sectionSelect = new List<SelectListItem>();
            var branchClassSections = db.AspNetBranchClass_Sections.Where(bcs => branchIds.Contains(bcs.AspNetBranch_Class.BranchId)).ToList();
            foreach (var bcs in branchClassSections)
            {
                sectionSelect.Add(new SelectListItem
                {
                    Text = $"Section {bcs.AspNetSection.Name}, Class {bcs.AspNetBranch_Class.AspNetClass.Name}",
                    Value = bcs.Id.ToString(),
                    Selected = bcs.Id == enrollment.SectionId
                });
            }
            ViewBag.SectionId = sectionSelect;
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", enrollment.SessionId);
            return View(enrollment);
        }

        [HttpPost]
        public JsonResult DeleteTeacherEnrollment(int id)
        {
            var enrollment = db.AspNetTeacher_Enrollments.Find(id);
            if (enrollment == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Entry(enrollment).State = EntityState.Deleted;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region StudentHistories

        public ActionResult StudentHistoriesList()
        {
            var branchIds = GetAdministratedBranchIds();
            return View(db.AspNetStudent_Histories
                .Where(history => branchIds.Contains(history.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList());
        }

        #endregion

        #region utils

        public String IsUsernameAvailable(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("username empty or null");
            }

            var checkUser = db.AspNetUsers.Where(user => user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)).Select(user => user.UserName).ToList();
            if (checkUser.Count == 0)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public String IsEmailAvailable(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("email empty or null");
            }

            var checkEmail = db.AspNetUsers.Where(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Select(user => user.Email).ToList();
            return checkEmail.Count == 0 ? "true" : "false";
        }

        public String IsRollNumberAvailableInBranch(string rollNo, int branchId)
        {
            if (string.IsNullOrWhiteSpace(rollNo))
            {
                throw new Exception("Roll Number is either null or empty");
            }

            var studentsInBranch = db.AspNetStudents
                .Where(student => student.BranchId == branchId);
            var studentsWithRollNo = studentsInBranch
                .Where(student => student.RollNo.Equals(rollNo, StringComparison.OrdinalIgnoreCase)).ToList();
            return studentsWithRollNo.Count == 0 ? "true" : "false";
        }

        public String IsRegistrationNumberAvailableInBranch(string registration, int branchId)
        {
            if (string.IsNullOrWhiteSpace(registration))
            {
                throw new Exception("Registration number can't be null or empty");
            }

            var teachersInBranch = db.AspNetTeachers
                .Where(teacher => teacher.AspNetEmployee.BranchId == branchId);
            var teachersWithRegistrationNo = teachersInBranch
                .Where(teacher => teacher.RegistrationNo.Equals(registration, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return teachersWithRegistrationNo.Count == 0 ? "true" : "false";
        }

        private List<int> GetAdministratedBranchIds()
        {
            var currentlyLoggedInUser = User.Identity.GetUserId();
            return db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(currentlyLoggedInUser, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .ToList();
        }

        //private List<int> GetDepartmentsFromSchool(int schoolId)
        //{
        //    return db.AspNetSchoolDepartments
        //        .Where(schoolDepartment => schoolDepartment.SchoolId == schoolId)
        //        .Select(schoolDepartment => schoolDepartment.DepartmentId)
        //        .Distinct()
        //        .ToList();
        //}

        //private List<int> GetDepartmentsFromSchools(List<int> schoolIds)
        //{
        //    var departmentIds = new List<int>();
        //    schoolIds.ForEach(schoolId => departmentIds.AddRange(GetDepartmentsFromSchool(schoolId)));
        //    return departmentIds;
        //}

        //private List<AspNetCours> GetCoursesFromDepartment(int departmentId)
        //{
        //    return db.AspNetCourses
        //        .Where(course => course.DepartmentId == departmentId)
        //        .Distinct()
        //        .ToList();
        //}

        //private List<AspNetCours> GetCoursesFromDepartments(List<int> departmentIds)
        //{
        //    var courseList = new List<AspNetCours>();
        //    departmentIds.ForEach(departmentId => courseList.AddRange(GetCoursesFromDepartment(departmentId)));
        //    return courseList;
        //}

        private List<int> GetClassIdsFromBranchId(int branchId)
        {
            return db.AspNetBranch_Class
                .Where(bc => bc.BranchId == branchId)
                .Select(bc => bc.ClassId)
                .ToList();
        }

        private List<int> GetClassIdsFromBranchIds(List<int> branchIds)
        {
            var classIds = new List<int>();
            branchIds.ForEach(branchId => classIds.AddRange(GetClassIdsFromBranchId(branchId)));
            return classIds;
        }

        private List<int> GetClassIdsFromCourseId(int id)
        {
            return db.AspNetClass_Courses
                .Where(cc => cc.CourseId == id)
                .Select(cc => cc.ClassId)
                .ToList();
        } 

        private List<int> GetClassIdsFromCourseIds(List<int> ids)
        {
            var classIds = new List<int>();
            ids.ForEach(id => classIds.AddRange(GetClassIdsFromCourseId(id)));
            return classIds;
        }

        public JsonResult GetCoursesFromBranchClassSectionId(int id)
        {
            var branchClassSection = db.AspNetBranchClass_Sections.Find(id);
            if(branchClassSection == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var classes = db.AspNetClass_Courses
                .Where(classCourse => classCourse.ClassId == branchClassSection.AspNetBranch_Class.ClassId)
                .Select(classCourse => classCourse.AspNetCours)
                .ToList();
            classes = classes.Distinct().ToList();
            var list = new List<CourseModel>();
            classes.ForEach(c => list.Add(new CourseModel
            {
                Id = c.Id,
                Name = c.Name
            }));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClassesFromBranchId(int id)
        {
            var branch = db.AspNetBranches.Find(id);
            if(branch == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var classes = db.AspNetBranch_Class
                .Where(branchClass => branchClass.BranchId == id)
                .Select(branchClass => new { branchClass.AspNetClass.Id, branchClass.AspNetClass.Name  })
                .ToList();
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectionsFromBranchClassId(int branchId, int classId)
        {
            var sections = db.AspNetBranchClass_Sections
                .Where(branchClassSection => branchClassSection.AspNetBranch_Class.BranchId == branchId && branchClassSection.AspNetBranch_Class.ClassId == classId)
                .Select(branchClassSection => new { branchClassSection.AspNetSection.Id, branchClassSection.AspNetSection.Name })
                .ToList();
            if(sections == null || sections.Count == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(sections, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsMandatoryTest()
        {
            var mandatoryStatus = Request.Form["IsMandatory"];
            if(mandatoryStatus.Contains(","))
            {
                mandatoryStatus = mandatoryStatus.Split(',')[0];
            }
            return Content("Recived: " + mandatoryStatus );
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        private void CreateLog(string logMessage, DateTime? startTime, DateTime? endTime)
        {
            db.AspNetLogs.Add(new AspNetLog
            {
                UserId = User.Identity.GetUserId(),
                Operation = logMessage,
                OperationStartTime = startTime ?? DateTime.Now,
                OperationEndTime = endTime ?? DateTime.Now
            });
            db.SaveChanges();
        }

        class CourseModel
        {
            public int Id { get; set; }
            public string Name { get; set; }

        }

        #endregion
    }
}