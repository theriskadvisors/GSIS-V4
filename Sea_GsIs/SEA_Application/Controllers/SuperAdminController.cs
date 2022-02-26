using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;

namespace SEA_Application.Controllers
{
    [Authorize(Roles = "Super_Admin")]
    public class SuperAdminController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }
     
        #region Sessions

        public ActionResult SessionList()
        {
            var sesssions = db.AspNetSessions.ToList();
            return View(sesssions);
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

        public ActionResult SessionDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetSession session = db.AspNetSessions.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
            if (session == null)
            {
                return HttpNotFound();
            }
            var sessions = db.AspNetSessions.ToList();
            var sessionSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not applicable",
                    Value = "0",
                    Selected = session.Next == null
                }
            };
            sessions.ForEach(s => sessionSelect.Add(new SelectListItem
            {
                Text = s.Year,
                Value = s.Id.ToString(),
                Selected = s.Id == session.Next
            }));
            ViewBag.Next = sessionSelect;
            return View(session);
        }

        public ActionResult CreateSession()
        {
            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name");
            var sessions = db.AspNetSessions.ToList();
            var sessionSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not applicable",
                    Value = "0",
                    Selected = true
                }
            };
            sessions.ForEach(session => sessionSelect.Add(new SelectListItem
            {
                Text = session.Year,
                Value = session.Id.ToString()
            }));
            ViewBag.Next = sessionSelect;
            return View();
        }
        public ActionResult UserName()
        {
            var id = User.Identity.GetUserId();
            var name = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var date = DateTime.Now;
            var result = new { date, name };

            return Json(result, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult CreateSession(AspNetSession session)
        {
            ViewBag.StatusId = new SelectList(db.AspNetStatus, "Id", "Name");

            try
            {
                var logMessage = "";
                DateTime? logStartTime = null;
                DateTime? logEndTime = null;
                if(session.Next == 0)
                {
                    session.Next = null;
                }
                if (ModelState.IsValid)
                {
                    logStartTime = DateTime.Now;
                    db.Entry(session).State = EntityState.Added;
                    logMessage += $"Session {session.Year} added. Session Id: {session.Id}.";
                    db.SaveChanges();
                    logEndTime = DateTime.Now;
                   // CreateLog(logMessage, logStartTime, logEndTime);
                    return RedirectToAction("SessionList");
                }
                else
                {
                    ModelState.AddModelError("", "Model state is invalid");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred. Details<br/>" + ex.Message);
            }
            var sessions = db.AspNetSessions.ToList();
            var sessionSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not applicable",
                    Value = "0",
                    Selected = session.Next == null
                }
            };
            sessions.ForEach(s => sessionSelect.Add(new SelectListItem
            {
                Text = s.Year,
                Value = s.Id.ToString(),
                Selected = s.Id == session.Next
            }));
            ViewBag.Next = sessionSelect;
            return View(session);
        }

        public ActionResult EditSession(int id)
        {
            var session = db.AspNetSessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(db.AspNetStatus, "Id", "Name", session.StatusId);
            var sessions = db.AspNetSessions.ToList();
            var sessionSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not applicable",
                    Value = "0",
                    Selected = session.Next == null
                }
            };
            sessions.ForEach(s => sessionSelect.Add(new SelectListItem
            {
                Text = s.Year,
                Value = s.Id.ToString(),
                Selected = s.Id == session.Next
            }));
            ViewBag.Next = sessionSelect;
            return View(session);
        }

        [HttpPost]
        public ActionResult EditSession(AspNetSession session)
        {
            var startTime = DateTime.Now;
            var logMessage = $"Edit session init. Session id: {session.Id}.";
            bool exceptionOccurred = false;
            AspNetSession dbSession = null;
            try
            {
                dbSession = db.AspNetSessions.Find(session.Id);
                if(dbSession == null)
                {
                    throw new Exception("Session not found in the database.");
                }
                var termsUnderSession = db.AspNetTerms
                    .Where(term => term.SessionId == session.Id)
                    .ToList();
                foreach (var term in termsUnderSession)
                {
                    if(session.StartDate > term.StartDate)
                    {
                        throw new Exception($"Term {term.Name}'s start date ({term.StartDate}) can't be earlier than session's start date ({session.StartDate})");
                    }
                    if(session.EndDate < term.EndDate)
                    {
                        throw new Exception($"Term {term.Name}'s end date ({term.EndDate}) can't be later than session's end date ({session.EndDate})");
                    }
                }
                bool changeOccurred = false;
                if(session.StartDate != dbSession.StartDate)
                {
                    changeOccurred = true;
                    logMessage += $" Session start date changed from {dbSession.StartDate} to {session.StartDate}.";
                    dbSession.StartDate = session.StartDate;
                }
                if(session.EndDate != dbSession.EndDate)
                {
                    changeOccurred = true;
                    logMessage += $" Session end date changed from {dbSession.EndDate} to {session.EndDate}.";
                    dbSession.EndDate = session.EndDate;
                }
                if(!session.Year.Equals(dbSession.Year, StringComparison.OrdinalIgnoreCase))
                {
                    changeOccurred = true;
                    logMessage += $" Session year changed from {dbSession.Year} to {session.Year}.";
                    dbSession.Year = session.Year;
                }
                int statusId = Convert.ToInt32(Request.Form["Status"]);
                if (dbSession.StatusId != statusId)
                {
                    changeOccurred = true;
                    logMessage += $" Session status changed from {dbSession.AspNetStatu.Name} to {db.AspNetStatus.Find(statusId).Name}.";
                    dbSession.StatusId = statusId;
                }
                if(dbSession.Next != session.Next)
                {
                    changeOccurred = true;
                    logMessage += $" Session next year changed from {dbSession.Next} to {session.Next}.";
                    dbSession.Next = session.Next == 0 ? null : session.Next;
                }
                if(!changeOccurred)
                {
                    logMessage += "No change occurred.";
                }
                else
                {
                    db.Entry(dbSession).State = EntityState.Modified;
                    db.SaveChanges();
                    logMessage += "Session edit end.";
                }
            }
            catch (Exception ex)
            {
                exceptionOccurred = true;
                logMessage += $" Error editing session. Details: {ex.Message}. {ex?.InnerException?.Message ?? ""}.";
                ModelState.AddModelError("", $"Error occurred.\n Details: \n {ex.Message} \nFurther details: \n {ex?.InnerException?.InnerException?.Message ?? ""}");
                db.Entry(dbSession).State = EntityState.Unchanged;
            }
            //CreateLog(logMessage, startTime, DateTime.Now);
            if(exceptionOccurred)
            {
                ViewBag.Status = new SelectList(db.AspNetStatus, "Id", "Name", session.StatusId);
                var sessions = db.AspNetSessions.ToList();
                var sessionSelect = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "Not applicable",
                        Value = "0",
                        Selected = session.Next == null
                    }
                };
                sessions.ForEach(s => sessionSelect.Add(new SelectListItem
                {
                    Text = s.Year,
                    Value = s.Id.ToString(),
                    Selected = s.Id == session.Next
                }));
                ViewBag.Next = sessionSelect;
                return View(session);
            }
            return RedirectToAction("SessionDetails", new { id = session.Id });
        }

        [HttpPost]
        public JsonResult DeleteSession(int id)
        {
            var startTime = DateTime.Now;
            var logMessage = $"Delete session init. Session id {id}.";
            var session = db.AspNetSessions.Find(id);
            if(session == null)
            {
                logMessage += $" Failed to delete the class. Class not found.";
                CreateLog(logMessage, startTime, DateTime.Now);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var terms = db.AspNetTerms
                .Where(term => term.SessionId == id)
                .ToList();
            foreach (var term in terms)
            {
                logMessage += $" Term {term.Name} ({term.Id}) deleted.";
                //term.IsActive = false;
                //db.Entry(term).State = EntityState.Modified;
                db.Entry(term).State = EntityState.Deleted;
            }
            db.SaveChanges();
            logMessage += $" Session deleted.";
            //session.IsActive = false;
            //db.Entry(session).State = EntityState.Modified;
            db.Entry(session).State = EntityState.Deleted;
            db.SaveChanges();
            CreateLog(logMessage, startTime, DateTime.Now);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SessionActivation(int  SessionId)
        {
            string status = "";
            var Session  =  db.AspNetSessions.Where(x => x.Id == SessionId).FirstOrDefault();
            if(Session != null)
            {                
                if (Session.StatusId == 2)
                {
                    status = "enable";
                    Session.StatusId = 1;
                    Session.IsActive = true;
                    db.SaveChanges();

                }
                else
                {
                    Session.StatusId = 2;
                    Session.IsActive = false;  
                    status = "disable";
                    db.SaveChanges();

                }

            
            }


            return Content(status);
        }
        #endregion

        #region Terms

        public ActionResult TermList()
        {
            return View(db.AspNetTerms
                .ToList());
        }

        public ActionResult CreateTerm()
        {
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year");
            return View();
        }

        [HttpPost]
        public ActionResult CreateTerm(AspNetTerm term)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var logMessage = "";
                    DateTime? logStartTime = DateTime.Now;
                    DateTime? logEndTime = null;
                    var session = db.AspNetSessions.Find(term.SessionId);
                    if(session == null)
                    {
                        throw new Exception("Session was not provided.");
                    }
                    if (!session.AspNetStatu.Name.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("Cannot add term to a staus that is not active.");
                    }
                    if(term.StartDate < session.StartDate)
                    {
                        throw new Exception($"Term start date ({term.StartDate}) can't be earlier than session start date ({session.StartDate}).");
                    }
                    if(term.EndDate > session.EndDate)
                    {
                        throw new Exception($"Term end date ({term.EndDate}) can't be later than session end date ({session.EndDate}).");
                    }
                    db.Entry(term).State = EntityState.Added;
                    db.SaveChanges();
                    logMessage += $"Term {term.Id} added in session {term.SessionId}.";
                    logEndTime = DateTime.Now;
                    CreateLog(logMessage, logStartTime, logEndTime);
                    return RedirectToAction("TermList");
                }
                else
                {
                    ModelState.AddModelError("", "Model State Error.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occuured. Details: {ex.Message}");
            }
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", term.SessionId);
            return View(term);
        }

        public ActionResult EditTerm(int id)
        {
            var term = db.AspNetTerms.Find(id);
            if(term == null)
            {
                return HttpNotFound();
            }
            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", term.SessionId);
            return View(term);
        }

        [HttpPost]
        public ActionResult EditTerm(AspNetTerm term)
        {
            var startTime = DateTime.Now;
            var logMessage = "Edit term init.";
            AspNetTerm dbTerm = null;
            try
            {
                if (ModelState.IsValid)
                {
                    dbTerm = db.AspNetTerms.Find(term.Id);
                    if(dbTerm == null)
                    {
                        throw new Exception("Could not find the term.");
                    }
                    logMessage += $" Term id: {term.Id}. Term name: {dbTerm.Name}.";
                    if (term.StartDate < dbTerm.AspNetSession.StartDate)
                    {
                        throw new Exception($"Term start date ({term.StartDate}) can't be earlier than session start date ({dbTerm.AspNetSession.StartDate}).");
                    }
                    if (term.EndDate > dbTerm.AspNetSession.EndDate)
                    {
                        throw new Exception($"Term end date ({term.EndDate}) can't be later than session end date ({dbTerm.AspNetSession.EndDate}).");
                    }
                    bool entityChanged = false;
                    if(!term.Name.Equals(dbTerm.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        entityChanged = true;
                        logMessage += $" Term name changed from {dbTerm.Name} to {term.Name}.";
                        dbTerm.Name = term.Name;
                    }
                    if(term.StartDate != dbTerm.StartDate)
                    {
                        entityChanged = true;
                        logMessage += $" Term start date changed from {dbTerm.StartDate} to {term.StartDate}.";
                        dbTerm.StartDate = term.StartDate;
                    }
                    if (term.EndDate != dbTerm.EndDate)
                    {
                        entityChanged = true;
                        logMessage += $" Term end date changed from {dbTerm.EndDate} to {term.EndDate}.";
                        dbTerm.EndDate = term.EndDate;
                    }

                    if(entityChanged)
                    {
                        db.Entry(dbTerm).State = EntityState.Modified;
                        db.SaveChanges();
                        logMessage += $" Term edit succeeded.";
                    }
                    else
                    {
                        logMessage += $" No change occurred in the term.";
                    }
                    CreateLog(logMessage, startTime, DateTime.Now);
                    return RedirectToAction("TermDetails", new { id = term.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Model state is invalid.");
                    logMessage += " Model state is invalid. No changes made. Edit failed.";
                    CreateLog(logMessage, startTime, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                logMessage += $" Error editing session. Details: {ex.Message}. {ex?.InnerException?.Message ?? ""}.";
                ModelState.AddModelError("", $"Error occurred.\n Details: \n {ex.Message} \nFurther details: \n {ex?.InnerException?.InnerException?.Message ?? ""}. Changes reverted. Edit failed.");
                db.Entry(dbTerm).State = EntityState.Unchanged;
                CreateLog(logMessage, startTime, DateTime.Now);
            }

            ViewBag.SessionId = new SelectList(db.AspNetSessions, "Id", "Year", term.SessionId);
            return View(term);
        }

        [HttpPost]
        public JsonResult DeleteTerm(int id)
        {
            var startTime = DateTime.Now;
            var logMessage = "Delete term init.";
            var term = db.AspNetTerms.Find(id);
            if(term == null)
            {
                logMessage += $" Could not find the term. Delete delete failed.";
                CreateLog(logMessage, startTime);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            logMessage += $" Term id {term.Id}. Term name: {term.Name}.";
            //term.IsActive = false;
            //db.Entry(term).State = EntityState.Modified;
            db.Entry(term).State = EntityState.Deleted;
            db.SaveChanges();
            logMessage += $" Term delete succeeded.";
            CreateLog(logMessage, startTime);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TermDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetTerm term = db.AspNetTerms.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
            if (term == null)
            {
                return HttpNotFound();
            }
            return View(term);
        }

        #endregion

        #region Departments

        public ActionResult ViewDepartments()
        {
            return View(db.AspNetDepartments
                .ToList());
        }

        public ActionResult DepartmentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetDepartment department = db.AspNetDepartments.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(AspNetDepartment department)
        {
            try
            {
                var logMessage = "";
                var startTime = DateTime.Now;
                if (ModelState.IsValid)
                {
                    department.AspNetUser = db.AspNetUsers.Find(User.Identity.GetUserId());
                    db.Entry(department).State = EntityState.Added;
                    db.SaveChanges();
                    logMessage += $"Department added. Department Id: {department.Id}.";
                    var endTime = DateTime.Now;
                    CreateLog(logMessage, startTime, endTime);
                    return RedirectToAction("ViewDepartments");
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
            return View(department);
        }

        public ActionResult EditDepartment(int id)
        {
            var department = db.AspNetDepartments.Find(id);
            if(department == null)
            {
                return HttpNotFound();
            }
            return View(department);
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
            return RedirectToAction("TeacherIndex", "AspNetEmployees");
        }

        //public void SaveImage(string base64, string ID, string btn)
        //{
        //    var b = base64.Split(',');
        //    var val = b[1];
        //    byte[] bytes = Convert.FromBase64String(val);

        //    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(val)))
        //    {
        //        using (Bitmap bm1 = new Bitmap(ms))
        //        {
        //            var uniquename = Guid.NewGuid().ToString() + ".jpg";
        //            var Path = Server.MapPath("~/Content/Images");
        //            Path = Path + "/" + uniquename;
        //            bm1.Save(Path, ImageFormat.Jpeg);

        //            AspNetUser ab = db.AspNetUsers.Where(x => x.Id == ID).FirstOrDefault();
        //            ab.Image = "../Content/Images/" + uniquename;
        //            db.SaveChanges();

        //        }
        //    }


        //}
        [HttpPost]
        public ActionResult EditDepartment(AspNetDepartment department)
        {
            var startTime = DateTime.Now;
            var logMessage = "Edit department init.";
            AspNetDepartment dbDepartment = null;
            try
            {
                if(ModelState.IsValid)
                {
                    dbDepartment = db.AspNetDepartments.Find(department.Id);
                    if(dbDepartment == null)
                    {
                        throw new Exception("Department not found.");
                    }
                    logMessage += $" Deparment id: {department.Id}.";
                    bool entityChanged = false;
                    if(!department.Name.Equals(dbDepartment.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        entityChanged = true;
                        logMessage += $" Department name changed from {dbDepartment.Name} to {department.Name}.";
                        dbDepartment.Name = department.Name;
                        db.Entry(dbDepartment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if(!entityChanged)
                    {
                        logMessage += $" No change detected. No change occurred. Edit finished.";
                    }
                    CreateLog(logMessage, startTime);
                    return RedirectToAction("DepartmentDetails", new { id = department.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Model state is invalid.");
                    logMessage += $" Model state error. No change occurred. Edit failed.";
                    CreateLog(logMessage, startTime);
                }
            }
            catch (Exception ex)
            {
                logMessage += $" Error editing deparment. Details: {ex.Message}. {ex?.InnerException?.Message ?? ""}.";
                ModelState.AddModelError("", $"Error occurred.\n Details: \n {ex.Message} \nFurther details: \n {ex?.InnerException?.InnerException?.Message ?? ""}. Changes reverted. Edit failed.");
                db.Entry(dbDepartment).State = EntityState.Unchanged;
                CreateLog(logMessage, startTime, DateTime.Now);
            }

            return View(department);
        }

        [HttpPost]
        //public JsonResult DeleteDepartment(int id)
        //{
        //    var startTime = DateTime.Now;
        //    var logMessage = "Delete department init.";
        //    var deparment = db.AspNetDepartments.Find(id);
        //    if(deparment == null)
        //    {
        //        logMessage += " Department not found. Delete failed.";
        //        CreateLog(logMessage, startTime);
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //    logMessage += $" Department id: {id}. Department name {deparment.Name}.";


        //    //var coursesUnderDepartment = db.AspNetCourses
        //    //    .Where(course => course.DepartmentId == id)
        //    //    .ToList();

        //    var classCoursesToDeactivate = new List<AspNetClass_Courses>();
        //    coursesUnderDepartment.ForEach(course => classCoursesToDeactivate.AddRange(db.AspNetClass_Courses
        //        .Where(classCourse => classCourse.CourseId == course.Id)
        //        .ToList()));

        //    classCoursesToDeactivate.ForEach(classCourse =>
        //    {
        //        logMessage += $" Course {classCourse.AspNetCours.Name} ({classCourse.CourseId}) removed from class {classCourse.AspNetClass.Name} ({classCourse.ClassId}).";
        //        db.Entry(classCourse).State = EntityState.Deleted;
        //    });
        //    db.SaveChanges();

        //    coursesUnderDepartment.ForEach(course =>
        //    {
        //        logMessage += $" Course {course.Name} ({course.Id}) deleted.";
        //        db.Entry(course).State = EntityState.Deleted;
        //    });
        //    db.SaveChanges();
            
        //    db.Entry(deparment).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    logMessage += $"Department delete succeeded.";
        //    CreateLog(logMessage, startTime);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region Branches

        public ActionResult ViewBranches()
        {
            return View(db.AspNetBranches
                .ToList());
        }

        [HttpPost]
        public ActionResult EditBranch(AspNetBranch branch)
        {
            var principalId = Request.Form["Principals"];
            branch.BranchPrincipalId = string.IsNullOrEmpty(principalId) ? null : principalId;
            db.Entry(branch).State = EntityState.Modified;
            var selectedAdminIds = Request.Form["Admins"]
                ?.Split(',');
            var branchAdmins = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.BranchId == branch.Id)
                .ToList();
            branchAdmins.ForEach(branchAdmin =>
            {
                db.Entry(branchAdmin).State = EntityState.Deleted;
            });
            AspNetBranch_Admins admin = null;
            if(!(selectedAdminIds == null || selectedAdminIds.Count() == 0))
            {
                foreach (var adminId in selectedAdminIds)
                {
                    admin = new AspNetBranch_Admins
                    {
                        AspNetBranch = branch,
                        AdminId = adminId
                    };
                    db.AspNetBranch_Admins.Add(admin);
                }
            }

            var previouslySelectedClassIds = Request.Form["PreviouslySelectedClassIds"]
                ?.Split(',')
                ?.Select(classId => Convert.ToInt32(classId))
                ?.ToList();
            var classIds = Request.Form["Classes"]  // newly selected class ids
                ?.Split(',')
                ?.Select(classId => Convert.ToInt32(classId));

            if (previouslySelectedClassIds != null && classIds != null)
            {
                
                foreach (var newClass in classIds)
                {
                    if(previouslySelectedClassIds.Contains(newClass))
                    {

                    }
                    else
                    {
                        var branchClass = new AspNetBranch_Class
                        {
                            BranchId = branch.Id,
                            ClassId = newClass
                        };
                        branch.AspNetBranch_Class.Add(branchClass);
                    }
                }

                foreach (var previousClass in previouslySelectedClassIds)
                {
                    if(classIds.Contains(previousClass))
                    {

                    }
                    else
                    {
                        var branchClasses = db.AspNetBranch_Class
                        .Where(branchClass => branchClass.BranchId == branch.Id && branchClass.ClassId == previousClass)
                        .ToList();
                        if(branchClasses != null)
                        {
                            var branchClassSections = new List<AspNetBranchClass_Sections>();
                            branchClasses.ForEach(branchClass => branchClassSections.AddRange(db.AspNetBranchClass_Sections
                                .Where(branchClassSection => branchClassSection.BranchClassId == branchClass.Id)
                                .ToList()));
                            branchClassSections.ForEach(branchClassSection =>
                            {
                                db.Entry(branchClassSection).State = EntityState.Deleted;
                            });
                            branchClasses.ForEach(branchClass =>
                            {
                                db.Entry(branchClass).State = EntityState.Deleted;
                            });
                        }
                    }
                }
            }
            else if(previouslySelectedClassIds == null && classIds != null)
            {
                foreach (var classId in classIds)
                {
                    var branchClass = new AspNetBranch_Class
                    {
                        BranchId = branch.Id,
                        ClassId = classId
                    };
                    branch.AspNetBranch_Class.Add(branchClass);
                }
            }
            else if(previouslySelectedClassIds != null && classIds == null)
            {
                foreach (var classId in previouslySelectedClassIds)
                {
                    var branchClass = new AspNetBranch_Class
                    {
                        BranchId = branch.Id,
                        ClassId = classId
                    };
                    branch.AspNetBranch_Class.Add(branchClass);
                }
            }
            
            db.SaveChanges();
            return RedirectToAction("BranchDetails", new { id = branch.Id });
        }

        public ActionResult BranchDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetBranch branch = db.AspNetBranches.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
            if (branch == null)
            {
                return HttpNotFound();
            }
            var selectedAdminIds = db.AspNetBranch_Admins
                .Where(ba => ba.BranchId == id)
                .Select(ba => ba.AdminId)
                .ToList();
            var branchAdminRole = db.AspNetRoles
                .Where(role => role.Name.Equals("branch_admin", StringComparison.OrdinalIgnoreCase))
                .First();

            var branchAdmins = context.Users
                .Where(user => user.Roles
                    .Select(role => role.RoleId)
                    .Contains(branchAdminRole.Id));
            ViewBag.Admins = new MultiSelectList(branchAdmins, "Id", "Email", selectedAdminIds);
            var selectedClassIds = db.AspNetBranch_Class
                .Where(bc => bc.BranchId == id)
                .Select(bc => bc.ClassId)
                .AsEnumerable();
            ViewBag.Classes = new MultiSelectList(db.AspNetClasses
                , "Id", "Name", selectedClassIds);

            ViewBag.SelectedClassIds = selectedClassIds;

            var branchPrincipalRoleId = db.AspNetRoles
                .Where(r => r.Name.Equals("Branch_Principal", StringComparison.OrdinalIgnoreCase))
                .First()
                .Id;
            ViewBag.Principals = new SelectList(db.AspNetUsers
                .Where(user => user.AspNetRoles
                    .Any(r => r.Id == branchPrincipalRoleId)), "Id", "Email", branch.BranchPrincipalId);
            return View(branch);
        }

        public ActionResult CreateBranch()
        {
            var branchPrincipalRole = db.AspNetRoles
                .Where(role => role.Name.Equals("Branch_Principal", StringComparison.OrdinalIgnoreCase))
                .First();

            var branchPrincipals = context.Users
                .Where(user => user.Roles
                    .Select(role => role.RoleId)
                    .Contains(branchPrincipalRole.Id));

            ViewBag.PrincipalId = new SelectList(branchPrincipals, "Id", "UserName");

            var branchAdminRole = db.AspNetRoles
                .Where(role => role.Name.Equals("branch_admin", StringComparison.OrdinalIgnoreCase))
                .First();

            var branchAdmins = context.Users
                .Where(user => user.Roles
                    .Select(role => role.RoleId)
                    .Contains(branchAdminRole.Id));

            ViewBag.AdminIds = new MultiSelectList(branchAdmins, "Id", "UserName");

            ViewBag.ClassIds = new MultiSelectList(db.AspNetClasses
                .OrderBy(c => c.Name), "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateBranch(BranchViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var branchToAdd = new AspNetBranch
                    {
                        Name = model.Name,
                        BranchPrincipalId = model.PrincipalId,
                        Address = model.Address
                    };
                    db.AspNetBranches.Add(branchToAdd);
                    db.SaveChanges();
                    var administratorIds = Request.Form["AdminIds"]?.Split(',');
                    string administratorIdsStr = null;
                    if(administratorIds != null)
                    {
                        administratorIdsStr = "";
                        foreach (var adminId in administratorIds)
                        {
                            var branchAdmin = new AspNetBranch_Admins
                            {
                                AdminId = adminId,
                                BranchId = branchToAdd.Id
                            };
                            db.AspNetBranch_Admins.Add(branchAdmin);
                            administratorIdsStr += " " + adminId;
                        }
                        db.SaveChanges();
                    }
                    var classIds = Request.Form["ClassIds"]?.Split(',')?.Select(c => Convert.ToInt32(c));
                    if(classIds != null)
                    {
                        foreach (var cid in classIds)
                        {
                            db.AspNetBranch_Class.Add(new AspNetBranch_Class
                            {
                                AspNetBranch = branchToAdd,
                                ClassId = cid
                            });
                        }
                        db.SaveChanges();
                    }
                    return RedirectToAction("ViewBranches");
                }
                else
                {
                    ModelState.AddModelError("", "Model State is not valid.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred. Details:<br/>" + ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteBranch(int id)
        {
            var startTime = DateTime.Now;
            var logMessage = $"Delete session init. Session id {id}.";
            var branch = db.AspNetBranches.Find(id);
            if (branch == null)
            {
                logMessage += $" Failed to delete the class. Class not found.";
                CreateLog(logMessage, startTime, DateTime.Now);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            db.Entry(branch).State = EntityState.Deleted;
            db.SaveChanges();
            CreateLog(logMessage, startTime, DateTime.Now);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Assessment

        public ActionResult CreateAssessment()
        {
            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title");
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAssessment([Bind(Include = "Id,AssessmentTypeId,Title,Description,Attachment,TermId,Weightage,Total,DueDate,PostingDate")] AspNetAssessment aspNetAssessment)
        {

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        file.SaveAs(path);
                    }
                }
                db.AspNetAssessments.Add(aspNetAssessment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssessmentTypeId = new SelectList(db.AspNetAssessmentTypes, "Id", "Title", aspNetAssessment.AssessmentTypeId);
            ViewBag.TermId = new SelectList(db.AspNetTerms, "Id", "Name", aspNetAssessment.TermId);
            return View(aspNetAssessment);
        }

        public ActionResult AssessmentType()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssessmentType([Bind(Include = "Title")] AspNetAssessmentType type)
        {
            if (ModelState.IsValid)
            {
                db.AspNetAssessmentTypes.Add(type);
                db.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Assessment type did not insert ";
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Users

        public ActionResult CreateAdmin()
        {
            var branches = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a branch",
                    Value = null
                }
            };
            db.AspNetBranches.ToList().ForEach(branch => branches.Add(new SelectListItem
            {
                Text = $"{branch.Name}",
                Value = branch.Id.ToString()
            }));
            ViewBag.branchList = branches;
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(RegisterViewModel model)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    var logMessage = "Admin Created.";
                    DateTime? logStartTime = DateTime.Now;
                    DateTime? logEndTime = null;
                    var requestedRole = Request.Form["admin_type"];
                    logMessage += $" Role type: {requestedRole}.";
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    if (!roleManager.RoleExists(requestedRole))
                    {
                        logMessage += $" {requestedRole} doesn't currently exist.";
                        var role = new IdentityRole()
                        {
                            Name = requestedRole
                        };
                        var result = roleManager.Create(role);
                        logMessage += $" {requestedRole} created. Role id: {role.Id}.";
                        if (!result.Succeeded)
                        {
                            var errors = "";
                            int errorCount = 0;
                            foreach (var error in result.Errors)
                            {
                                errors += $"{++errorCount} - {error}.<br/>";
                            }
                            errors += $"Total Errors: {errorCount}";
                            throw new Exception($"Could not create role '{requestedRole}'. Further details<br/> {errors}");
                        }
                    }

                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var userCreatedStatus = UserManager.Create(user, model.Password);

                    if (userCreatedStatus.Succeeded)
                    {
                        logMessage += $" User {user.UserName} created. User Id: {user.Id}.";
                        var addToRoleStatus = UserManager.AddToRole(user.Id, requestedRole);
                        logMessage += $" {user.UserName} added to role {requestedRole}.";

                        if (!addToRoleStatus.Succeeded)
                        {
                            throw new Exception($"Could not add user to '{requestedRole}' role");
                        }
                    }
                    else
                    {
                        throw new Exception("Could not create the admin");
                    }

                    var branchIdsList = Request.Form["branchList"]
                        ?.Split(',')
                        .Select(branchId => Convert.ToInt32(branchId))
                        .ToList();
                    if(branchIdsList != null)
                    {
                        if (requestedRole.Equals("Branch_Principal", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var branchId in branchIdsList)
                            {
                                var branch = db.AspNetBranches.Find(branchId);
                                branch.BranchPrincipalId = user.Id;
                                db.Entry(branch).State = EntityState.Modified;
                                logMessage += $" Branch {branch.Id} modified. Modification: {user.UserName} was made principal.";
                            }
                        }
                        else
                        {
                            foreach (var branchId in branchIdsList)
                            {
                                var branchAdmin = new AspNetBranch_Admins
                                {
                                    BranchId = branchId,
                                    AdminId = user.Id
                                };
                                db.AspNetBranch_Admins.Add(branchAdmin);
                                logMessage += $" {user.UserName} was admin of branch {branchAdmin.BranchId}. Entry added in BranchAdmin table (id: {branchAdmin.Id}).";
                            }
                        }
                        db.SaveChanges();
                        
                    }
                    logEndTime = DateTime.Now;
                    CreateLog(logMessage, logStartTime, logEndTime);
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception("Model State not valid.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Operation failed due to some error. Details<br/>{ex.Message}<br/>");
                var branches = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a branch",
                    Value = null
                }
            };
                db.AspNetBranches.ToList().ForEach(branch => branches.Add(new SelectListItem
                {
                    Text = $"{branch.Name}",
                    Value = branch.Id.ToString()
                }));
                ViewBag.branchList = branches;
                return View(model);
            }
        }

        #endregion

        #region logs

        public ActionResult LogList()
        {
            return View(db.AspNetLogs.ToList());
        }

        #endregion

        #region Classes

        public ActionResult ClassList()
        {
            return View(db.AspNetClasses
                .ToList());
        }
        public ActionResult TeacherIndex()
        {
            return View();
        }
        public ActionResult CreateClass()
        {
            var nextClassSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not Applicable",
                    Value = "0"
                }
            };
            var nextClasses = db.AspNetClasses.ToList();
            nextClasses.ForEach(@class => nextClassSelect.Add(new SelectListItem
            {
                Text = @class.Name,
                Value = @class.Id.ToString()
            }));
            ViewBag.NextClassId = nextClassSelect;
            return View();
        }

        [HttpPost]
        public ActionResult CreateClass(AspNetClass model)
        {
            var logMessage = "";
            DateTime? startTime = null;
            DateTime? endTime = null;
            if(model.NextClassId == 0)  // do it outside the model state validity check so as to validate the model
            {
                model.NextClassId = null;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    startTime = DateTime.Now;
                    db.AspNetClasses.Add(model);
                    db.SaveChanges();
                    logMessage += $"Class {model.Name} ({model.Id}) added.";
                    endTime = DateTime.Now;
                    CreateLog(logMessage, startTime, endTime);
                    return RedirectToAction("ClassList");
                }
                else
                {
                    ModelState.AddModelError("", "Model state invalid");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred: " + ex.Message);
            }

            var nextClassSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not Applicable",
                    Value = "0",
                    Selected = model.NextClassId == null
                }
            };
            var nextClasses = db.AspNetClasses.ToList();
            nextClasses.ForEach(@class => nextClassSelect.Add(new SelectListItem
            {
                Text = @class.Name,
                Value = @class.Id.ToString(),
                Selected = @class.Id == model.NextClassId
            }));
            ViewBag.NextClassId = nextClassSelect;

            return View(model);
        }

        public ActionResult EditClass(int id)
        {
            var @class = db.AspNetClasses.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            var o = db.AspNetBranch_Class
                .Where(bc => bc.ClassId == id)
                .Select(bc => new { bc.AspNetBranch, bc.AspNetSession, bc.Capacity })
                .First();
            ViewBag.Branch = o.AspNetBranch;
            ViewBag.Session = o.AspNetSession;
            ViewBag.Capacity = o.Capacity;
            return View(@class);
        }

        [HttpPost]
        public ActionResult EditClass(AspNetClass @class)
        {
            var logMessage = "Edit class init.";
            var startTime = DateTime.Now;
            var classInDb = db.AspNetClasses.Find(@class.Id);
            classInDb.Name = @class.Name;
            classInDb.NextClassId = @class.NextClassId == 0 ? null : @class.NextClassId;
            db.Entry(classInDb).State = EntityState.Modified;
            db.SaveChanges();
            logMessage += $" Name of class {@class.Id} changed from {Request.Form["PreviousName"]} to {@class.Name}.";
            CreateLog(logMessage, startTime, DateTime.Now);
            return RedirectToAction("ClassDetails", new { id = @class.Id });
        }

        public ActionResult ClassDetails(int id)
        {
            var @class = db.AspNetClasses
                .Where(c => c.Id == id)
                .First();
            if (@class == null)
            {
                return HttpNotFound("Class not found with given id");
            }
            var nextClassSelect = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Not Applicable",
                    Value = "0",
                    Selected = @class.NextClassId == null
                }
            };
            var nextClasses = db.AspNetClasses.ToList();
            foreach (var c in nextClasses)
            {
                nextClassSelect.Add(new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                    Selected = c.Id == @class.NextClassId
                });
            }
            ViewBag.NextClassId = nextClassSelect;
            return View(@class);
        }

        [HttpPost]
        public JsonResult DeleteClass(int id)
        {
            var logMessage = "Delete class initiated.";
            var startTime = DateTime.Now;
            var @class = db.AspNetClasses.Find(id);
            if (@class == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var associatedBranchClasses = db.AspNetBranch_Class
                .Where(bc => bc.ClassId == id)
                .ToList();

            foreach (var bc in associatedBranchClasses)
            {
                var associatedBranchClassSections = db.AspNetBranchClass_Sections
                    .Where(bcs => bcs.BranchClassId == bc.Id)
                    .ToList();

                foreach (var bcs in associatedBranchClassSections)
                {
                    logMessage += $" Deleted branch_class_section entry {bcs.Id}. Removed section {bcs.AspNetSection.Name} ({bcs.SectionId}) from class {bc.AspNetClass.Name} ({bc.ClassId}).";
                    bcs.IsActive = false;
                    db.Entry(bcs).State = EntityState.Modified;
                }
                db.SaveChanges();

                logMessage += $" Deleted branch_class entry {bc.Id}. Deleted class {bc.AspNetClass.Name} ({bc.ClassId}) from branch {bc.AspNetBranch.Name} ({bc.BranchId}).";
                bc.IsActive = false;
                db.Entry(bc).State = EntityState.Modified;
            }
            db.SaveChanges();
            var associatedClassCourses = db.AspNetClass_Courses
                .Where(cc => cc.ClassId == id)
                .ToList();

            foreach (var cc in associatedClassCourses)
            {
                logMessage += $" Deleted class_course entry {cc.Id}. Removed Course {cc.AspNetCours.Name} ({cc.CourseId}) from class {cc.AspNetClass.Name} ({cc.ClassId}).";
                cc.IsActive = false;
                db.Entry(cc).State = EntityState.Modified;
            }
            db.SaveChanges();

            logMessage += $" Deleted class {@class.Name} ({@class.Id}).";
            @class.IsActive = false;
            db.Entry(@class).State = EntityState.Modified;
            db.SaveChanges();
            var endTime = DateTime.Now;
            CreateLog(logMessage, startTime, endTime);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AssessmentTypes

        public ActionResult AssessmentTypeList()
        {
            return View(db.AspNetAssessmentTypes.ToList());
        }

        public ActionResult AssessmentTypeDetails(int id)
        {
            var assessment = db.AspNetAssessmentTypes.Find(id);
            if(assessment == null)
            {
                return HttpNotFound();
            }
            return View(assessment);
        }

        public ActionResult CreateAssessmentType()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAssessmentType(AspNetAssessmentType model)
        {
            if(ModelState.IsValid)
            {
                db.AspNetAssessmentTypes.Add(model);
                db.SaveChanges();
                return RedirectToAction("AssessmentTypeList");
            }
            ModelState.AddModelError("", "Invalid model state");
            return View(model);
        }

        public ActionResult EditAssessmentType(int id)
        {
            var assessment = db.AspNetAssessmentTypes.Find(id);
            if(assessment == null)
            {
                return HttpNotFound();
            }

            return View(assessment);
        }

        [HttpPost]
        public ActionResult EditAssessmentType(AspNetAssessmentType model)
        {
            if(ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssessmentTypeDetails", new { id = model.Id });
            }

            ModelState.AddModelError("", "Invalid model state");
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteAssessmentType(int id)
        {
            var assessment = db.AspNetAssessmentTypes.Find(id);
            if(assessment == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            db.Entry(assessment).State = EntityState.Deleted;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region utils

        public String IsUsernameAvailable(string username)
        {
            if(string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("username empty or null");
            }

            var checkUser = db.AspNetUsers.Where(user => user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)).Select(user => user.UserName).ToList();
            if(checkUser.Count == 0)
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
            if(string.IsNullOrWhiteSpace(rollNo))
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
            if(string.IsNullOrWhiteSpace(registration))
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

        private void CreateLog(string logMessage, DateTime? startTime, DateTime? endTime = null)
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

        #endregion
    }
}