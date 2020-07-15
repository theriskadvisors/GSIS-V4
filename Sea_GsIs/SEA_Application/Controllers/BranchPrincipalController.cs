using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System.Data.Entity;
using System.IO;

namespace SEA_Application.Controllers
{
    [Authorize(Roles = "Branch_Principal")]
    public class BranchPrincipalController : Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();
        private Sea_Entities db = new Sea_Entities();

        // GET: BranchPrincipal
        public ActionResult Index()
        {
            var branchIds = GetPrincipaledBranchIds();
            ViewBag.Branches = db.AspNetBranches.Where(branch => branchIds.Contains(branch.Id)).ToList();
            return View();
        }


        #region Sections

        public ActionResult SectionList()
        {
            var principaledBranchIds = GetPrincipaledBranchIds();
            var modelList = new List<SectionDisplayViewModel>();
            foreach (var id in principaledBranchIds)
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

        public ActionResult SectionDetails(int id)
        {
            var section = db.AspNetBranchClass_Sections
                .Where(bcs => bcs.SectionId == id)
                .ToList();
            return View(section);
        }

        #endregion

        #region Students

        //public ActionResult StudentsList()
        //{
        //    var branchIds = GetPrincipaledBranchIds();
        //    var studentsList = new List<AspNetStudent>();
        //    foreach (var branchId in branchIds)
        //    {
        //        var studentsInCurrentBranch = db.AspNetStudents
        //            .Where(student => student.BranchId == branchId)
        //            .ToList();
        //        studentsInCurrentBranch.ForEach(student => studentsList.Add(student));
        //    }
        //    return View(studentsList);
        //}
        
        //public ActionResult StudentDetails(int id)
        //{
        //    var student = db.AspNetStudents.Find(id);
        //    if (student == null)
        //    {
        //        return HttpNotFound("Could not find the student with given id.");
        //    }
        //    var user = db.AspNetUsers.Find(student.UserId);
        //    ViewBag.Username = user.UserName;
        //    ViewBag.Email = user.Email;
        //    var enrollment = db.AspNetStudent_Enrollments.Where(en => en.StudentId == id).Count() == 0 ? null : db.AspNetStudent_Enrollments.Where(en => en.StudentId == id).First();
        //    if (enrollment == null)
        //    {
        //        ViewBag.Enrollment = null;
        //    }
        //    else
        //    {
        //        ViewBag.Enrollment = enrollment;
        //    }
        //    if (student.File == null)
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

        #endregion

        #region Teachers

        public ActionResult TeachersList()
        {
            var branchIds = GetPrincipaledBranchIds();
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

            var branchIds = GetPrincipaledBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
            ViewBag.Id = id;
            return View(model);
        }

        #endregion

        #region Courses

        public ActionResult CourseList()
        {
            var branchIds = GetPrincipaledBranchIds();
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
        
        public ActionResult CourseDetails(int id)
        {
            var course = db.AspNetCourses.Find(id);
            if (course == null)
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

        #endregion

        #region Employees

        

       

        

        public ActionResult ViewEmployee()
        {
            return View(db.AspNetEmployees.ToList());
        }


        public ActionResult EmployeeDetails(int id)
        {
            var employee = db.AspNetEmployees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.NationalityId = new SelectList(db.AspNetNationalities, "Id", "Title", employee.NationalityId);
            ViewBag.ReligionId = new SelectList(db.AspNetReligions, "Id", "Title", employee.ReligionId);
            ViewBag.GenderId = new SelectList(db.AspNetGenders, "Id", "Title", employee.GenderId);

            var branchIds = GetPrincipaledBranchIds();
            var branches = new List<AspNetBranch>();
            branchIds.ForEach(branchId => branches.Add(db.AspNetBranches.Find(branchId)));
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", employee.BranchId);
            return View(employee);
        }


        #endregion

        #region StudentEnrollments

        public ActionResult StudentEnrollmentList()
        {
            var branchIds = GetPrincipaledBranchIds();
            var enrollments = db.AspNetStudent_Enrollments
                .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList();
            return View(enrollments);
        }

        public ActionResult StudentEnrollmentDetails(int id)
        {
            var enrollment = db.AspNetStudent_Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }


        #endregion

        #region TeacherEnrollments

        public ActionResult TeacherEnrollmentList()
        {
            var branchIds = GetPrincipaledBranchIds();
            var enrollments = db.AspNetTeacher_Enrollments
                .Where(enrollment => branchIds.Contains(enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList();
            return View(enrollments);
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

        #endregion

        #region StudentHistories

        public ActionResult StudentHistoriesList()
        {
            var branchIds = GetPrincipaledBranchIds();
            return View(db.AspNetStudent_Histories
                .Where(history => branchIds.Contains(history.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId))
                .ToList());
        }

        #endregion



        #region Utils

        List<int> GetPrincipaledBranchIds()
        {
            var loggedInUserId = User.Identity.GetUserId();
            return db.AspNetBranches
                .Where(branch => branch.BranchPrincipalId.Equals(loggedInUserId))
                .Select(branch => branch.Id)
                .ToList();
        }

        #endregion
    }
}