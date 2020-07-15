using SEA_Application.Models;
using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class SubjectGroupsController : Controller
    {
        Sea_Entities db = new Sea_Entities();
        // GET: SubjectGroups
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public JsonResult Details()
        {
           var coursegroup= db.AspNetCoursePackages.ToList();
            List<SubjectGroup> group = new List<SubjectGroup>();
            foreach (var item in coursegroup)
            {
                SubjectGroup sb = new SubjectGroup();
                sb.Subject = db.AspNetCourses.Where(x => x.Id == item.CourseId).Select(x => x.Name).FirstOrDefault();
                sb.Group = db.AspNetPackages.Where(x => x.Id == item.PackageId).Select(x => x.Title).FirstOrDefault();
                group.Add(sb);
            }
            return Json(group,JsonRequestBehavior.AllowGet);
        }
        public class SubjectGroup
        {
            public string Subject { get; set; }
            public string Group { get; set; }
        }
        public ActionResult Class_CourseIndex()
        {
            return View();
        }
        public ActionResult Class_CourseCreate()
        {
            return View();
        }
        public JsonResult Class_CourseDetails()
        {
            var loggedInUserId = User.Identity.GetUserId();
            if (User.IsInRole("Branch_Admin"))
            {
                var branchId = db.AspNetBranch_Admins
                    .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                    .Select(branchAdmin => branchAdmin.BranchId)
                    .FirstOrDefault();

                var coursegroup = (from BC in db.AspNetBranch_Class
                                   join CC in db.AspNetClass_Courses on BC.ClassId equals CC.ClassId
                                   where BC.BranchId == branchId
                                   select new
                                   {
                                       Course = CC.AspNetCours.Name,
                                       Class = CC.AspNetClass.Name
                                   }).ToList();

                return Json(coursegroup, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var branchId = db.AspNetBranches.Where(x=> x.BranchPrincipalId == loggedInUserId).Select(x=> x.Id).FirstOrDefault();

                var coursegroup = (from BC in db.AspNetBranch_Class
                                   join CC in db.AspNetClass_Courses on BC.ClassId equals CC.ClassId
                                   where BC.BranchId == branchId
                                   select new
                                   {
                                       Course = CC.AspNetCours.Name,
                                       Class = CC.AspNetClass.Name
                                   }).ToList();

                return Json(coursegroup, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Class_SectionDetails()
        {
            
                var loggedInUserId = User.Identity.GetUserId();
            if (User.IsInRole("Branch_Admin"))
            {
                var branchId = db.AspNetBranch_Admins
                    .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                    .Select(branchAdmin => branchAdmin.BranchId)
                    .FirstOrDefault();

                var sections = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.BranchId == branchId).Select(x => new
                {
                    Branch = x.AspNetBranch_Class.AspNetBranch.Name,
                    Class = x.AspNetBranch_Class.AspNetClass.Name,
                    Section = x.AspNetSection.Name
                }).ToList();

                return Json(sections, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();

                var sections = db.AspNetBranchClass_Sections.Where(x => x.AspNetBranch_Class.BranchId == branchId).Select(x => new
                {
                    Branch = x.AspNetBranch_Class.AspNetBranch.Name,
                    Class = x.AspNetBranch_Class.AspNetClass.Name,
                    Section = x.AspNetSection.Name
                }).ToList();

                return Json(sections, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Teacher_ClassDetails()
        {

            var loggedInUserId = User.Identity.GetUserId();
            if (User.IsInRole("Branch_Admin"))
            {
                var branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .FirstOrDefault();

                var teachers = (from teacher in db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.BranchId == branchId)
                                join user in db.AspNetUsers on teacher.AspNetEmployee.UserId equals user.Id
                                select new
                                {
                                    TeacherId = user.UserName,
                                    Teacher = teacher.AspNetEmployee.Name,
                                    Class = teacher.AspNetClass_Courses.AspNetClass.Name + " " + teacher.AspNetBranchClass_Sections.AspNetSection.Name,
                                    Subject = teacher.AspNetClass_Courses.AspNetCours.Name,
                                    Id = teacher.Id
                                }).ToList();

                return Json(teachers, JsonRequestBehavior.AllowGet);
            }else
            {
                var branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();

                var teachers = (from teacher in db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.BranchId == branchId)
                                join user in db.AspNetUsers on teacher.AspNetEmployee.UserId equals user.Id
                                select new
                                {
                                    TeacherId = user.UserName,
                                    Teacher = teacher.AspNetEmployee.Name,
                                    Class = teacher.AspNetClass_Courses.AspNetClass.Name + " " + teacher.AspNetBranchClass_Sections.AspNetSection.Name,
                                    Subject = teacher.AspNetClass_Courses.AspNetCours.Name,
                                    Id = teacher.Id
                                }).ToList();

                return Json(teachers, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteEnrollment(int Id)
        {
            var loggedInUserId = User.Identity.GetUserId();
            int branchId;
            if (User.IsInRole("Branch_Admin"))
            {
                branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .FirstOrDefault();
            }
            {
                branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();
            }

            var enrollment = db.AspNetTeacher_Enrollments.Where(x => x.Id == Id).FirstOrDefault();
            db.AspNetTeacher_Enrollments.Remove(enrollment);
            db.SaveChanges();

            var teachers = (from teacher in db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.BranchId == branchId)
                     join user in db.AspNetUsers on teacher.AspNetEmployee.UserId equals user.Id
                     select new
                     {
                         TeacherId = user.UserName,
                         Teacher = teacher.AspNetEmployee.Name,
                         Class = teacher.AspNetClass_Courses.AspNetClass.Name + " " + teacher.AspNetBranchClass_Sections.AspNetSection.Name,
                         Subject = teacher.AspNetClass_Courses.AspNetCours.Name,
                         Id = teacher.Id
                     }).ToList();

            //var teachers = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.BranchId == branchId).Select(x => new
            //{
            //    Teacher = x.AspNetEmployee.Name,
            //    Class = x.AspNetClass_Courses.AspNetClass.Name + " " + x.AspNetBranchClass_Sections.AspNetSection.Name,
            //    Subject = x.AspNetClass_Courses.AspNetCours.Name,
            //    Id = x.Id
            //}).ToList();

            return Json(teachers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BranchClass_Details()
        {
            var loggedInUserId = User.Identity.GetUserId();

            if (User.IsInRole("Branch_Admin"))
            {
                var branchId = db.AspNetBranch_Admins
                .Where(branchAdmin => branchAdmin.AdminId.Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
                .Select(branchAdmin => branchAdmin.BranchId)
                .FirstOrDefault();

                var classes = db.AspNetBranch_Class.Where(x => x.BranchId == branchId).Select(x => new { Class = x.AspNetClass.Name, Branch = x.AspNetBranch.Name }).ToList();

                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                var branchId = db.AspNetBranches.Where(x => x.BranchPrincipalId == loggedInUserId).Select(x => x.Id).FirstOrDefault();

                var classes = db.AspNetBranch_Class.Where(x => x.BranchId == branchId).Select(x => new { Class = x.AspNetClass.Name, Branch = x.AspNetBranch.Name }).ToList();

                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult BranchClass()
        {
            return View();
        }
        public ActionResult BranchClassCreate()
        {
            return View();
        }



        public ActionResult BranchClassSection()
        {
            return View();
        }
        public ActionResult BranchclassSectoinCreate()
        {
            return View();
        }
        public class ClassCourse
        {
            public string Class { get; set; }
            public string Course { get; set; }
        }
    }
}