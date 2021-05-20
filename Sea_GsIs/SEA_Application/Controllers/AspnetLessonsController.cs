using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Utilities;
using Microsoft.Ajax.Utilities;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Text;

namespace SEA_Application.Controllers
{
    public class AspnetLessonsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspnetLessons
        public ActionResult Index()
        {
            var aspnetLessons = db.AspnetLessons.Include(a => a.AspnetSubjectTopic);
            return View(aspnetLessons.ToList());
        }

        public ActionResult DisableStudent(string StudentID)
        {
            string status = "";
            if (StudentID != null)
            {
                AspNetUser user = db.AspNetUsers.Where(x => x.UserName == StudentID).FirstOrDefault();
                //                user.StatusId = 2;
                if (user.StatusId == 2)
                {
                    status = "enable";
                    user.StatusId = 1;
                    db.SaveChanges();

                }
                else
                {
                    user.StatusId = 2;
                    status = "disable";
                    db.SaveChanges();

                }

            }

            return Content(status);
        }



        public ActionResult ViewLessonsToAdmin()
        {



            //   List<AspnetLesson> lessons = db.AspnetLessons


            return View();
        }

        public ActionResult AllLessonForAdmin()
       {

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];


            int pageNo = 1;

            if (start >= length)
            {

                pageNo = (start / length) + 1;

            }

            var LessonCreationDate = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();


            var LessonStartDate = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var LessonClass = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var LessonSection = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var LessonSubject = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var LessonTopicName = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var LessonName = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            var LessonStatus = Request.Form.GetValues("columns[8][search][value]").FirstOrDefault();


            string status = LessonStatus.ToLower();
            string Published = "published";
            string InActive = "inactive";
            string Created = "created";

            bool? LessonStatusbool = null;
            bool? LessonIsActive = null;
            if (LessonStatus != "")
            {

                if (Published.Contains(status))
                {
                    LessonStatusbool = true;
                    //LessonIsActive = true;
                    LessonIsActive = true;
                }
                else if (InActive.Contains(status))
                {
                    LessonStatusbool = false;
                    LessonIsActive = false;
                }
                else
                {
                    LessonStatusbool = false;
                    LessonIsActive = true;
                }

            }


            //if(statusSearch)

            //  var datetimeday = DateTime.Now.Day;
            ///  var datetimeMonth = DateTime.Now.Month;
            // var datetimeyear = DateTime.Now.Year;

            var CreationDate = LessonCreationDate.Split('/').ToList();
            var Startdate = LessonStartDate.Split('/').ToList();


            // string day, month, year = "";
            string day = "", month = "", year = "";

            var countForCreationDate = CreationDate.Count();

            if (countForCreationDate == 1)
            {
                month = CreationDate[0];
                //  day = CreationDate[0];
                //   year = CreationDate[0]; 
            }
            else if (countForCreationDate == 2)
            {
                month = CreationDate[0];
                day = CreationDate[1];
                //  year = CreationDate[0];
            }
            else
            {
                month = CreationDate[0];
                day = CreationDate[1];
                year = CreationDate[2];
            }



            var countForStartDate = Startdate.Count();
            string dayStartDate = "", monthStartDate = "", yearStartDate = "";

            if (countForStartDate == 1)
            {
                monthStartDate = Startdate[0];
                //  day = CreationDate[0];
                //   year = CreationDate[0]; 
            }
            else if (countForStartDate == 2)
            {
                monthStartDate = Startdate[0];
                dayStartDate = Startdate[1];
                //  year = CreationDate[0];
            }
            else
            {
                monthStartDate = Startdate[0];
                dayStartDate = Startdate[1];
                yearStartDate = Startdate[2];
            }

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


            try
            {
                int totalrows = 0;

                // int totalrows = AllLessons.Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList().Count();
                //      if (LessonStartDate == "" && LessonClass == "" && LessonSection == "" && LessonSubject == "" && LessonTopicName == "" && LessonName == "" && LessonStatus == "")
                if (LessonStartDate == "" && LessonCreationDate == "" && LessonStatus == "")
                {

                    totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                 select new
                                 {
                                     LessonId = lesson.Id,
                                     LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                     LessonName = lesson.Name,
                                     LessonVidoeUrl = lesson.Video_Url,
                                     LessonDuration = lesson.DurationMinutes,
                                     LessonDescription = lesson.Description,
                                     LessonStatus = lesson.Status,
                                     LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                     LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                     LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                     LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                     LessonDate = lesson.CreationDate,
                                     LessonStartDate = lesson.StartDate,
                                     LessonIsActive = lesson.IsActive
                                 }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                    var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                      select new
                                      {
                                          LessonId = lesson.Id,
                                          LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                          LessonName = lesson.Name,
                                          LessonVidoeUrl = lesson.Video_Url,
                                          LessonDuration = lesson.DurationMinutes,
                                          LessonDescription = lesson.Description,
                                          LessonStatus = lesson.Status,
                                          LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                          LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                          LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                          LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                          //  LessonDate = lesson.CreationDate.ToString()
                                          LessonDate = lesson.CreationDate,
                                          LessonStartDate = lesson.StartDate,
                                          LessonIsActive = lesson.IsActive
                                          //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                      }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                    int totalrowsafterfiltering = totalrows;
                    return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                }

                //  LessonStatusbool
                // LessonIsActive 
                //     else if ((LessonStatusbool != null || LessonIsActive != null) )
                else if (LessonStatus != "" && LessonCreationDate == "" && LessonStartDate == "")
                {

                    totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                 select new
                                 {
                                     LessonId = lesson.Id,
                                     LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                     LessonName = lesson.Name,
                                     LessonVidoeUrl = lesson.Video_Url,
                                     LessonDuration = lesson.DurationMinutes,
                                     LessonDescription = lesson.Description,
                                     LessonStatus = lesson.Status,
                                     LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                     LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                     LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                     LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                     LessonDate = lesson.CreationDate,
                                     LessonStartDate = lesson.StartDate,
                                     LessonIsActive = lesson.IsActive
                                 }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                    var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                      select new
                                      {
                                          LessonId = lesson.Id,
                                          LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                          LessonName = lesson.Name,
                                          LessonVidoeUrl = lesson.Video_Url,
                                          LessonDuration = lesson.DurationMinutes,
                                          LessonDescription = lesson.Description,
                                          LessonStatus = lesson.Status,
                                          LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                          LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                          LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                          LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                          //  LessonDate = lesson.CreationDate.ToString()
                                          LessonDate = lesson.CreationDate,
                                          LessonStartDate = lesson.StartDate,
                                          LessonIsActive = lesson.IsActive
                                          //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                      }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                    int totalrowsafterfiltering = totalrows;
                    return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    // AllLessons = AllLessons.Where(x => x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive);
                    // totalrows = AllLessons.Count();

                }

                else if (LessonStatus == "" && LessonCreationDate != "" && LessonStartDate == "")
                {
                    if (countForCreationDate == 1)
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }


                }

                else if (LessonStatus == "" && LessonCreationDate == "" && LessonStartDate != "")
                {


                    if (countForStartDate == 1)
                    {
                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();


                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();


                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }

                }
                else if (LessonStatus != "" && LessonCreationDate != "" && LessonStartDate == "")
                {
                    if (countForCreationDate == 1)
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();




                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();




                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }


                }
                else if (LessonStatus != "" && LessonCreationDate == "" && LessonStartDate != "")
                {

                    if (countForStartDate == 1)
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }
                    else

                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }
                }
                else if (LessonStatus == "" && LessonCreationDate != "" && LessonStartDate != "")
                {

                    //  if (countForStartDate == 1)
                    //{

                    if (countForCreationDate == 1 && countForStartDate == 1)
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) &&  (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) &&  x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }
                    
                    else if (countForCreationDate == 1 && countForStartDate != 1)
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) &&  x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) &&  x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);



                    }

                    else if (countForCreationDate != 1 && countForStartDate == 1)
                    {



                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                   }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) &&  x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) &&  x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }
                    else if (countForCreationDate != 1 && countForStartDate != 1)
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) &&  x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }



                }
                else if (LessonStatus != "" && LessonCreationDate != "" && LessonStartDate != "")
                {

                    if (countForCreationDate == 1 && countForStartDate == 1)
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }

                    else if (countForCreationDate == 1 && countForStartDate != 1)
                    {

                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) &&  x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)) && x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive  && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);



                    }

                    else if (countForCreationDate != 1 && countForStartDate == 1)
                    {



                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)) && x.LessonDate.Value.Day.ToString().Contains(day)  && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                    }
                    else if (countForCreationDate != 1 && countForStartDate != 1)
                    {


                        totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                     select new
                                     {
                                         LessonId = lesson.Id,
                                         LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                         LessonName = lesson.Name,
                                         LessonVidoeUrl = lesson.Video_Url,
                                         LessonDuration = lesson.DurationMinutes,
                                         LessonDescription = lesson.Description,
                                         LessonStatus = lesson.Status,
                                         LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                         LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                         LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                         LessonDate = lesson.CreationDate,
                                         LessonStartDate = lesson.StartDate,
                                         LessonIsActive = lesson.IsActive
                                     }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                        var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                          select new
                                          {
                                              LessonId = lesson.Id,
                                              LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                              LessonName = lesson.Name,
                                              LessonVidoeUrl = lesson.Video_Url,
                                              LessonDuration = lesson.DurationMinutes,
                                              LessonDescription = lesson.Description,
                                              LessonStatus = lesson.Status,
                                              LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                              LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                              LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                              LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                              //  LessonDate = lesson.CreationDate.ToString()
                                              LessonDate = lesson.CreationDate,
                                              LessonStartDate = lesson.StartDate,
                                              LessonIsActive = lesson.IsActive
                                              //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                          }).Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate) && x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonStatus == LessonStatusbool && x.LessonIsActive == LessonIsActive &&  x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                        int totalrowsafterfiltering = totalrows;
                        return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


                    }
                }
                else
                {

                    totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                 select new
                                 {
                                     LessonId = lesson.Id,
                                     LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                     LessonName = lesson.Name,
                                     LessonVidoeUrl = lesson.Video_Url,
                                     LessonDuration = lesson.DurationMinutes,
                                     LessonDescription = lesson.Description,
                                     LessonStatus = lesson.Status,
                                     LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                     LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                     LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                     LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                     LessonDate = lesson.CreationDate,
                                     LessonStartDate = lesson.StartDate,
                                     LessonIsActive = lesson.IsActive
                                 }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                    var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                      select new
                                      {
                                          LessonId = lesson.Id,
                                          LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                          LessonName = lesson.Name,
                                          LessonVidoeUrl = lesson.Video_Url,
                                          LessonDuration = lesson.DurationMinutes,
                                          LessonDescription = lesson.Description,
                                          LessonStatus = lesson.Status,
                                          LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                          LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                          LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                          LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                          //  LessonDate = lesson.CreationDate.ToString()
                                          LessonDate = lesson.CreationDate,
                                          LessonStartDate = lesson.StartDate,
                                          LessonIsActive = lesson.IsActive
                                          //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                      }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                    int totalrowsafterfiltering = totalrows;
                    return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);

                }



                //if (LessonCreationDate != "")
                //{
                //    if (countForCreationDate == 1)
                //    {




                //        //x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month))

                //        //  AllLessons = AllLessons.Where(x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month)));
                //        // totalrows = AllLessons.Count();
                //    }
                //    else
                //    {



                //        // AllLessons = AllLessons.Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year));
                //        //  totalrows = AllLessons.Count();
                //    }
                //}


                //if (LessonStartDate != "")
                //{

                //    if (countForStartDate == 1)
                //    {


                //        //x => (x.LessonDate.Value.Month.ToString().Contains(month) || x.LessonDate.Value.Day.ToString().Contains(month) || x.LessonDate.Value.Year.ToString().Contains(month))

                //        // AllLessons = AllLessons.Where(x => (x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Day.ToString().Contains(monthStartDate) || x.LessonStartDate.Value.Year.ToString().Contains(monthStartDate)));
                //        // totalrows = AllLessons.Count();
                //    }
                //    else
                //    {



                //        //  AllLessons = AllLessons.Where(x => x.LessonStartDate.Value.Day.ToString().Contains(dayStartDate) && x.LessonStartDate.Value.Month.ToString().Contains(monthStartDate) && x.LessonStartDate.Value.Year.ToString().Contains(yearStartDate));
                //        // totalrows = AllLessons.Count();
                //    }
                //}



                // AllLessons.Count;
                // var  AllLessons1 = AllLessons.Skip(start).Take(length).ToList();

                // var AllLessons1 = AllLessons.OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();
                //var jsonResult = Json(AllLessons, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;
                //return jsonResult;

                //    int totalrowsafterfiltering = totalrows;
                //  return Json(new { data = AllLessons1, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);




            } // end of try block
            catch (Exception ex1)
            {
                var ex = ex1.Message;


              int   totalrows = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                             select new
                             {
                                 LessonId = lesson.Id,
                                 LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                 LessonName = lesson.Name,
                                 LessonVidoeUrl = lesson.Video_Url,
                                 LessonDuration = lesson.DurationMinutes,
                                 LessonDescription = lesson.Description,
                                 LessonStatus = lesson.Status,
                                 LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                 LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                 LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                 LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                 LessonDate = lesson.CreationDate,
                                 LessonStartDate = lesson.StartDate,
                                 LessonIsActive = lesson.IsActive
                             }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().Count();


                var AllLessons = (from lesson in db.AspnetLessons.Where(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetBranch.Id == branchId)
                                  select new
                                  {
                                      LessonId = lesson.Id,
                                      LessonSubjectTopicName = lesson.AspnetSubjectTopic.Name,
                                      LessonName = lesson.Name,
                                      LessonVidoeUrl = lesson.Video_Url,
                                      LessonDuration = lesson.DurationMinutes,
                                      LessonDescription = lesson.Description,
                                      LessonStatus = lesson.Status,
                                      LessonStatus1 = lesson.Status.ToString() + "-" + lesson.IsActive.ToString(),
                                      LessonSubject = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
                                      LessonClass = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                      LessonSection = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name,
                                      //  LessonDate = lesson.CreationDate.ToString()
                                      LessonDate = lesson.CreationDate,
                                      LessonStartDate = lesson.StartDate,
                                      LessonIsActive = lesson.IsActive
                                      //  }).Where(x => x.LessonDate.Value.Day.ToString().Contains(day) && x.LessonDate.Value.Month.ToString().Contains(month) && x.LessonDate.Value.Year.ToString().Contains(year) && x.LessonStartDate.ToString().Contains(LessonStartDate) && x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).Distinct().ToList();
                                  }).Where(x => x.LessonClass.ToLower().Contains(LessonClass) && x.LessonSection.ToLower().Contains(LessonSection) && x.LessonSubject.ToLower().Contains(LessonSubject) && x.LessonSubjectTopicName.ToLower().ToLower().Contains(LessonTopicName) && x.LessonName.ToLower().Contains(LessonName)).OrderBy(x => x.LessonName).Skip((pageNo - 1) * length).Take(length).Distinct().ToList();

                int totalrowsafterfiltering = totalrows;
                return Json(new { data = AllLessons, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);


            }


            return Json("", JsonRequestBehavior.AllowGet);

        }

        //public class AdminLessons
        //{
        //    public int LessonsId { get; set; }
        //    public DateTime DateCreated { get; set; }
        //    public DateTime StartDate { get; set; }
        //    public string ClassName { get; set; }
        //    public string SectionName { get; set; }
        //    public string SubjectName { get; set; }
        //    public string TopicName { get; set; }
        //    public string LessonsName { get; set; }
        //    public bool Status { get; set; }


        //}

        public ActionResult ViewAttendance(int id, string BranchName, string ClassName, string SectionName, string SubjectName, string LessonName, string StartDate, string Type)
        {
            ViewBag.BranchName = BranchName;
            ViewBag.ClassName = ClassName;
            ViewBag.SectionName = SectionName;
            ViewBag.LessonID = id;
            ViewBag.Type = Type;
            ViewBag.SubjectName = SubjectName;
            ViewBag.LessonName = LessonName;
            ViewBag.StartDate = StartDate;


            return View();
        }
        public ActionResult GetLessonAttendance(int LessonID)
        {
            var lessonRecord = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault();
            List<LessonAttendance_Result> list = new List<LessonAttendance_Result>();
            if (lessonRecord.ContentType == "Link")
            {
                var data = (from track in db.StudentLessonTrackings.Where(x => x.LessonId == LessonID && x.MeetingJoinStatus != null)
                            join user in db.AspNetUsers on track.StudentId equals user.Id
                            select new
                            {
                                StudentName = user.Name,
                                LessonStatus = track.MeetingJoinStatus,
                                LessonStartDate = track.MeetingJoinTime.ToString()
                            }).ToList();

                var students = (from enrollment in db.AspNetStudent_Enrollments
                                join lesson in db.AspnetLessons on enrollment.AspNetClass_Courses.CourseId equals lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId
                                join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                                where enrollment.AspNetClass_Courses.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                                && enrollment.AspNetBranchClass_Sections.SectionId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                                && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                                && lesson.Id == LessonID
                                where student.AspNetUser.StatusId != 2
                                select new { StudentName = enrollment.AspNetStudent.AspNetUser.Name, LessonStatus = "Absent", LessonStartDate = "" }).Distinct().ToList();
                foreach (var item in students)
                {
                    var std = data.Where(x => x.StudentName.ToString() == item.StudentName.ToString()).FirstOrDefault();
                    if (std == null)
                    {
                        data.Add(item);
                    }
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = (from track in db.StudentLessonTrackings.Where(x => x.LessonId == LessonID && x.LessonStatus != null)
                            join user in db.AspNetUsers on track.StudentId equals user.Id
                            select new
                            {
                                StudentName = user.Name,
                                LessonStatus = track.LessonStatus,
                                LessonStartDate = track.StartDate.ToString()
                            }).ToList();
                var students = (from enrollment in db.AspNetStudent_Enrollments
                                join lesson in db.AspnetLessons on enrollment.AspNetClass_Courses.CourseId equals lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId
                                join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                                where enrollment.AspNetClass_Courses.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                                && enrollment.AspNetBranchClass_Sections.SectionId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                                && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                                && lesson.Id == LessonID
                                where student.AspNetUser.StatusId != 2
                                select new { StudentName = enrollment.AspNetStudent.AspNetUser.Name, LessonStatus = "Absent", LessonStartDate = "" }).Distinct().ToList();

                foreach (var item in students)
                {
                    var std = data.Where(x => x.StudentName.ToString() == item.StudentName.ToString()).FirstOrDefault();
                    if (std == null)
                    {
                        data.Add(item);
                    }
                }

                return Json(data, JsonRequestBehavior.AllowGet);
            }


            //return View();
        }

        public ActionResult PublishChecker(int LessonID)
        {
            string Status = "NP";
            AspnetLesson Lesson = db.AspnetLessons.Where(x => x.Id == LessonID).FirstOrDefault();
            if (Lesson != null)
            {
                if (Lesson.Status == true)
                {
                    Status = "P";
                }
            }
            return Content(Status);
        }

        public ActionResult StudentAttendance()
        {

            return View();
        }
        public class StudentLessonAttendance
        {
            public string LessonName { get; set; }
            public string StudentName { get; set; }
            public string SubjectName { get; set; }
            public string LessonType { get; set; }
            public string LessonStatus { get; set; }
            public TimeSpan Time { get; set; }
            public string StartDate { get; set; }
        }

        //public ActionResult GetStudentAttendance(int? StudentId)
        //{

        //    var StudentUserId = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().UserId;

        //    var List = (from track in db.StudentLessonTrackings.Where(x => x.StudentId == StudentUserId )
        //                 join lesson in db.AspnetLessons on track.LessonId equals lesson.Id
        //                 join user in db.AspNetUsers on track.StudentId equals user.Id

        //                 select new
        //                 {
        //                     LessonName = lesson.Name,
        //                     LessonType = lesson.ContentType,
        //                     SubjectName = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name,
        //                     StudentName = user.Name,
        //                     LessonMeetingStatus = track.MeetingJoinStatus,
        //                     LessonStatus  = track.LessonStatus,
        //                     LessonStartDate = track.StartDate.ToString()

        //                 }).ToList();

        //    List<StudentLessonAttendance> StudentAttendanceList = new List<StudentLessonAttendance>();

        //    foreach ( var item in List)
        //    {
        //        StudentLessonAttendance studentLessonAttendance = new StudentLessonAttendance();

        //        studentLessonAttendance.LessonName = item.LessonName;
        //        studentLessonAttendance.LessonType = item.LessonType;
        //        studentLessonAttendance.StartDate = item.LessonStartDate;
        //        studentLessonAttendance.SubjectName = item.SubjectName;
        //        studentLessonAttendance.StudentName = item.StudentName;

        //        if (item.LessonType == "Link")
        //           {


        //            if (item.LessonMeetingStatus  != null)
        //            {

        //                studentLessonAttendance.LessonStatus = item.LessonMeetingStatus;
        //            }
        //            else
        //            {
        //                studentLessonAttendance.LessonStatus = "Absent";
        //            }

        //            StudentAttendanceList.Add(studentLessonAttendance);
        //        }
        //        else
        //        {
        //            if (item.LessonStatus != null)
        //            {
        //                studentLessonAttendance.LessonStatus = item.LessonStatus;
        //            }
        //            else
        //            {
        //                studentLessonAttendance.LessonStatus = "Absent";
        //            }

        //            StudentAttendanceList.Add(studentLessonAttendance);

        //        }


        //    }

        //    return Json(StudentAttendanceList, JsonRequestBehavior.AllowGet);


        //}

        public ActionResult GetStudentAttendance(int BranchId, int ClassId, int SectionId, int SubjectId, int StudentId)
        {
            ViewBag.BranchId = BranchId;
            ViewBag.ClassId = ClassId;
            ViewBag.SectionId = SectionId;
            ViewBag.SubjectId = SubjectId;
            ViewBag.StudentId = StudentId;

            var BranchName = db.AspNetBranches.Where(x => x.Id == BranchId).FirstOrDefault().Name;
            var ClassName = db.AspNetClasses.Where(x => x.Id == ClassId).FirstOrDefault().Name;
            var SectionName = db.AspNetSections.Where(x => x.Id == SectionId).FirstOrDefault().Name;
            var SubjectName = db.AspNetCourses.Where(x => x.Id == SubjectId).FirstOrDefault().Name;
            var StudentName = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().Name;

            ViewBag.BranchName = BranchName;
            ViewBag.ClassName = ClassName;

            ViewBag.SectionName = SectionName;
            ViewBag.SectionName = SectionName;
            ViewBag.SubjectName = SubjectName;

            ViewBag.StudentName = StudentName;



            return View();

        }
        public ActionResult StudentAttendanceList(int SubjectId, int StudentId)
        {

            var StudentUserId = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().UserId;
            var students = (from enrollment in db.AspNetStudent_Enrollments
                                //  join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                            join lesson in db.AspnetLessons on enrollment.AspNetClass_Courses.CourseId equals lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId
                            join lessonTracking in db.StudentLessonTrackings on lesson.Id equals lessonTracking.LessonId into egroup
                            from lessonTracking in egroup.DefaultIfEmpty()
                            where enrollment.AspNetClass_Courses.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                            && enrollment.AspNetBranchClass_Sections.SectionId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                            && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                            where lesson.Status == true && lesson.IsActive == true && lesson.ContentType == "Link" && enrollment.StudentId == StudentId && lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == SubjectId// && student.AspNetUser.StatusId != 2
                            select new { LessonName = lesson.Name, LessonType = lesson.ContentType, SubjectName = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name, StudentName = enrollment.AspNetStudent.AspNetUser.Name, LessonStatus = lessonTracking.MeetingJoinStatus != null ? lessonTracking.MeetingJoinStatus : "Absent", LessonStartDate = "" }).Distinct().ToList();


            var students1 = (from enrollment in db.AspNetStudent_Enrollments
                                 // join student in db.AspNetStudents on enrollment.StudentId equals student.Id
                             join lesson in db.AspnetLessons on enrollment.AspNetClass_Courses.CourseId equals lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId
                             join lessonTracking in db.StudentLessonTrackings on lesson.Id equals lessonTracking.LessonId into egroup
                             from lessonTracking in egroup.DefaultIfEmpty()
                             where enrollment.AspNetClass_Courses.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                             && enrollment.AspNetBranchClass_Sections.SectionId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                             && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.BranchId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId
                             where lesson.Status == true && lesson.IsActive == true && lesson.ContentType != "Link" && enrollment.StudentId == StudentId && lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId == SubjectId //&& student.AspNetUser.StatusId != 2
                             select new { LessonName = lesson.Name, LessonType = lesson.ContentType, SubjectName = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name, StudentName = enrollment.AspNetStudent.AspNetUser.Name, LessonStatus = lessonTracking.LessonStatus != null ? lessonTracking.LessonStatus : "Absent", LessonStartDate = "" }).Distinct().ToList();

            students1.AddRange(students);

            return Json(students1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PublishSpecifiedLessonsList()
        {

            // db.AspnetLessons.Where(x=> x.StartDate> )

            DateTime AfterDate = Convert.ToDateTime("2021-03-28");
            DateTime BeforeDate = Convert.ToDateTime("2021-04-08");


            var AllLessonIdsToDelete = db.AspnetLessons.Where(x => x.StartDate > AfterDate && x.StartDate < BeforeDate && x.Status == false).Select(x => x.Id).ToList();

            foreach (var id in AllLessonIdsToDelete)
            {

                PublishLessonsFunction(id);


            }

         //   PublishLessonsFunction(42351);


            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void PublishLessonsFunction(int? id)
        {


            var loggedUserID = User.Identity.GetUserId();
            var loggedUser = db.AspNetUsers.Where(x => x.Id == loggedUserID).FirstOrDefault();
            AspnetLesson Lesson = db.AspnetLessons.Where(x => x.Id == id).FirstOrDefault();
            try
            {
                if (Lesson != null && Lesson.Status == false)
                {
                    Lesson.Status = true;
                    Lesson.IsActive = true;
                    db.SaveChanges();

                    var branchID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId;
                    var ClassID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId;
                    var SectionID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId;
                    var SubjectID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId;

                    var BCId = db.AspNetBranch_Class.Where(x => x.ClassId == ClassID && x.BranchId == branchID).Select(x => x.Id).FirstOrDefault();
                    var BCSId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCId && x.SectionId == SectionID).Select(x => x.Id).FirstOrDefault();
                    var CSId = db.AspNetClass_Courses.Where(x => x.ClassId == ClassID && x.CourseId == SubjectID).Select(x => x.Id).FirstOrDefault();

                    var Students = db.AspNetStudent_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId && x.AspNetStudent.AspNetUser.StatusId != 2).Select(x => x.AspNetStudent.AspNetUser.Id).ToList();
                    var subjectName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name).FirstOrDefault();
                    var ClassName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name).FirstOrDefault();
                    var SectionName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name).FirstOrDefault();

                    var eventList = db.Events.Where(x => x.LessonID == id).FirstOrDefault();

                    var branchid = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId).FirstOrDefault();
                    var AllBranchManagerList = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Branch_Principal") && x.AspNetBranches.Any(z => z.Id == branchid)).ToList();


                    if (eventList == null)
                    {
                        var events = new Event();
                        events.UserId = null;
                        events.ThemeColor = "Green";
                        events.Subject = "";
                        events.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                        events.Sec_Title = Lesson.Name;
                        events.Description1 = Lesson.Description;
                        events.Start = Lesson.StartTime.Value;
                        events.End = Lesson.EndTime;
                        events.LessonID = Lesson.Id;
                        events.IsFullDay = false;
                        events.IsPublic = false;
                        events.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        events.Url = "/TeacherCommentsOnCourses/StudentLessons/?id=" + Lesson.Id;

                        db.Events.Add(events);
                        db.SaveChanges();

                        var event_user = new AspnetEvent_User();
                        event_user.eventid = events.EventID;
                        event_user.userid = User.Identity.GetUserId();
                        db.AspnetEvent_User.Add(event_user);
                        db.SaveChanges();

                        var teacher = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId).Select(x => x.AspNetEmployee.UserId).ToList();

                        foreach (var item in teacher)
                        {
                            var event_teacher = new AspnetEvent_User();
                            event_teacher.eventid = events.EventID;
                            event_teacher.userid = item;
                            db.AspnetEvent_User.Add(event_teacher);
                            db.SaveChanges();
                        }

                        foreach (var item in AllBranchManagerList)
                        {

                            var event_user1 = new AspnetEvent_User();
                            event_user1.userid = item.Id;
                            event_user1.eventid = events.EventID;
                            db.AspnetEvent_User.Add(event_user1);
                            db.SaveChanges();

                        }

                    }
                    else
                    {
                        eventList.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        eventList.ThemeColor = "Green";
                        db.SaveChanges();


                        foreach (var item in AllBranchManagerList)
                        {

                            var event_user1 = new AspnetEvent_User();
                            event_user1.userid = item.Id;
                            event_user1.eventid = eventList.EventID;
                            db.AspnetEvent_User.Add(event_user1);
                            db.SaveChanges();

                        }


                    }

                    var events1 = new Event();
                    events1.UserId = null;
                    events1.ThemeColor = "Green";
                    events1.Subject = "";
                    events1.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                    events1.Sec_Title = Lesson.Name;
                    events1.Description1 = Lesson.Description;
                    events1.Start = Lesson.StartTime.Value;
                    events1.End = Lesson.EndTime;
                    events1.LessonID = Lesson.Id;
                    events1.IsFullDay = false;
                    events1.IsPublic = false;
                    events1.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                    events1.Url = "/StudentCourses/StudentLessons/?id=" + Lesson.EncryptedID;

                    db.Events.Add(events1);
                    db.SaveChanges();

                    foreach (var item in Students)
                    {
                        var event_user = new AspnetEvent_User();
                        event_user.eventid = events1.EventID;
                        event_user.userid = item;
                        db.AspnetEvent_User.Add(event_user);
                        db.SaveChanges();
                    }

                    //var teacher = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId).Select(x => x.AspNetEmployee.UserId).ToList();

                    //foreach (var item in teacher)
                    //{
                    //    var user = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                    //    SendMail(user.SecondaryEmail, "Lesson Published", "" + CustomModel.EmailDesign.TecherLessonTemplate (user.Name, loggedUser.Name, Lesson.Name));
                    //}
                }
            }
            catch (Exception e)
            {
                var logs = new AspNetLog();
                logs.Operation = "Lesson Publish -- Exception: " + e.Message + "--- Inner Exception: " + e.InnerException;
                logs.OperationStartTime = DateTime.Now;
                logs.UserId = User.Identity.GetUserId();
            }
        }

        public ActionResult PubishLesson(int? id)
        {
            var loggedUserID = User.Identity.GetUserId();
            var loggedUser = db.AspNetUsers.Where(x => x.Id == loggedUserID).FirstOrDefault();
            AspnetLesson Lesson = db.AspnetLessons.Where(x => x.Id == id).FirstOrDefault();
            try
            {
                if (Lesson != null && Lesson.Status == false)
                {
                    Lesson.Status = true;
                    Lesson.IsActive = true;
                    db.SaveChanges();

                    var branchID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId;
                    var ClassID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId;
                    var SectionID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId;
                    var SubjectID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId;

                    var BCId = db.AspNetBranch_Class.Where(x => x.ClassId == ClassID && x.BranchId == branchID).Select(x => x.Id).FirstOrDefault();
                    var BCSId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCId && x.SectionId == SectionID).Select(x => x.Id).FirstOrDefault();
                    var CSId = db.AspNetClass_Courses.Where(x => x.ClassId == ClassID && x.CourseId == SubjectID).Select(x => x.Id).FirstOrDefault();

                    var Students = db.AspNetStudent_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId && x.AspNetStudent.AspNetUser.StatusId != 2).Select(x => x.AspNetStudent.AspNetUser.Id).ToList();
                    var subjectName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name).FirstOrDefault();
                    var ClassName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name).FirstOrDefault();
                    var SectionName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name).FirstOrDefault();

                    var eventList = db.Events.Where(x => x.LessonID == id).FirstOrDefault();

                    var branchid = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId).FirstOrDefault();
                    var AllBranchManagerList = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Branch_Principal") && x.AspNetBranches.Any(z => z.Id == branchid)).ToList();


                    if (eventList == null)
                    {
                        var events = new Event();
                        events.UserId = null;
                        events.ThemeColor = "Green";
                        events.Subject = "";
                        events.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                        events.Sec_Title = Lesson.Name;
                        events.Description1 = Lesson.Description;
                        events.Start = Lesson.StartTime.Value;
                        events.End = Lesson.EndTime;
                        events.LessonID = Lesson.Id;
                        events.IsFullDay = false;
                        events.IsPublic = false;
                        events.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        events.Url = "/TeacherCommentsOnCourses/StudentLessons/?id=" + Lesson.Id;

                        db.Events.Add(events);
                        db.SaveChanges();

                        var event_user = new AspnetEvent_User();
                        event_user.eventid = events.EventID;
                        event_user.userid = User.Identity.GetUserId();
                        db.AspnetEvent_User.Add(event_user);
                        db.SaveChanges();

                        var teacher = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId).Select(x => x.AspNetEmployee.UserId).ToList();

                        foreach (var item in teacher)
                        {
                            var event_teacher = new AspnetEvent_User();
                            event_teacher.eventid = events.EventID;
                            event_teacher.userid = item;
                            db.AspnetEvent_User.Add(event_teacher);
                            db.SaveChanges();
                        }

                        foreach (var item in AllBranchManagerList)
                        {

                            var event_user1 = new AspnetEvent_User();
                            event_user1.userid = item.Id;
                            event_user1.eventid = events.EventID;
                            db.AspnetEvent_User.Add(event_user1);
                            db.SaveChanges();

                        }

                    }
                    else
                    {
                        eventList.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        eventList.ThemeColor = "Green";
                        db.SaveChanges();


                        foreach (var item in AllBranchManagerList)
                        {

                            var event_user1 = new AspnetEvent_User();
                            event_user1.userid = item.Id;
                            event_user1.eventid = eventList.EventID;
                            db.AspnetEvent_User.Add(event_user1);
                            db.SaveChanges();

                        }


                    }

                    var events1 = new Event();
                    events1.UserId = null;
                    events1.ThemeColor = "Green";
                    events1.Subject = "";
                    events1.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                    events1.Sec_Title = Lesson.Name;
                    events1.Description1 = Lesson.Description;
                    events1.Start = Lesson.StartTime.Value;
                    events1.End = Lesson.EndTime;
                    events1.LessonID = Lesson.Id;
                    events1.IsFullDay = false;
                    events1.IsPublic = false;
                    events1.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                    events1.Url = "/StudentCourses/StudentLessons/?id=" + Lesson.EncryptedID;

                    db.Events.Add(events1);
                    db.SaveChanges();

                    foreach (var item in Students)
                    {
                        var event_user = new AspnetEvent_User();
                        event_user.eventid = events1.EventID;
                        event_user.userid = item;
                        db.AspnetEvent_User.Add(event_user);
                        db.SaveChanges();
                    }

                    //var teacher = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId).Select(x => x.AspNetEmployee.UserId).ToList();

                    //foreach (var item in teacher)
                    //{
                    //    var user = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                    //    SendMail(user.SecondaryEmail, "Lesson Published", "" + CustomModel.EmailDesign.TecherLessonTemplate (user.Name, loggedUser.Name, Lesson.Name));
                    //}
                }
            }
            catch (Exception e)
            {
                var logs = new AspNetLog();
                logs.Operation = "Lesson Publish -- Exception: " + e.Message + "--- Inner Exception: " + e.InnerException;
                logs.OperationStartTime = DateTime.Now;
                logs.UserId = User.Identity.GetUserId();
            }
            return RedirectToAction("ViewLessonsToAdmin");
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


        // GET: AspnetLessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            if (aspnetLesson == null)
            {
                return HttpNotFound();
            }
            return View(aspnetLesson);
        }

        // GET: AspnetLessons/Create

        public ActionResult LoadSectionIdDDL()
        {
            var ClassList = db.AspNetSessions.ToList().Select(x => new { x.Id, x.Year });

            string status = Newtonsoft.Json.JsonConvert.SerializeObject(ClassList);

            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);

        }

        public ActionResult Create(int id)
        {
            if (id != 0)
            {
                AspnetSubjectTopic aspnetSubjectTopic = db.AspnetSubjectTopics.Find(id);

                int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == aspnetSubjectTopic.Id).FirstOrDefault().GenericBranchClassSubjectId;

                var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();


                ViewBag.TopicExist = 1;

                ViewBag.BranchId = GenericObject.BranchId;
                ViewBag.ClassId = GenericObject.ClassId;
                ViewBag.SectionId = GenericObject.SectionId;
                ViewBag.SubId = GenericObject.SubjectId;
                ViewBag.TopicId = aspnetSubjectTopic.Id;

                // ViewBag.CTId = Subject.SubjectType;

                return View();

            }
            else
            {

                ViewBag.BranchId = null;
                ViewBag.ClassId = null;
                ViewBag.SectionId = null;
                ViewBag.SubId = null;
                ViewBag.TopicId = null;
                // ViewBag.CTId = null;
                ViewBag.TopicExist = 0;
                //   ViewBag.TopicId = new SelectList(db.AspnetSubjectTopics, "Id", "Name");
                // ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");

                return View();
            }

        }

        public ActionResult CheckLessonOrderBy(string TopicId, string OrderBy)
        {

            int TopId = Convert.ToInt32(TopicId);
            int OrderByValue = Convert.ToInt32(OrderBy);

            var TopicExist = "";
            AspnetLesson Lesson = db.AspnetLessons.Where(x => x.TopicId == TopId && x.OrderBy == OrderByValue).FirstOrDefault();

            if (Lesson == null)
            {
                TopicExist = "No";
            }
            else
            {
                TopicExist = "Yes";
            }


            return Json(TopicExist, JsonRequestBehavior.AllowGet);
        }

        // POST: AspnetLessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LessonViewModel LessonViewModel)
        {

            AspnetLesson Lesson = new AspnetLesson();

            Lesson.Name = LessonViewModel.LessonName;
            Lesson.Video_Url = LessonViewModel.LessonVideoURL;
            Lesson.TopicId = LessonViewModel.TopicId;
            Lesson.DurationMinutes = LessonViewModel.LessonDuration;
            // Lesson.IsActive = LessonViewModel.IsActive;
            Lesson.IsActive = true;
            Lesson.CreationDate = LessonViewModel.CreationDate;
            Lesson.Description = LessonViewModel.LessonDescription;
            Lesson.OrderBy = LessonViewModel.OrderBy;

            Lesson.StartDate = LessonViewModel.StartDate;

            DateTime startDate = DateTime.Parse(LessonViewModel.StartDate.ToString());
            DateTime start = DateTime.Parse(LessonViewModel.StartTime.ToString());
            Lesson.StartTime = startDate.Date.Add(start.TimeOfDay);
            Lesson.EndTime = Lesson.StartTime.Value.AddMinutes(60);

            TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime PKTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PK_ZONE);
            Lesson.CreationDate = PKTime.Date;

            Lesson.MeetingLink = LessonViewModel.MeetingLink;
            Lesson.ContentType = LessonViewModel.ContentType;

            Lesson.Status = false;

            string EncrID = Lesson.Name + Lesson.StartTime + Lesson.Description;

            string LessonID = Encrpt.Encrypt(EncrID, true);


            var newString = Regex.Replace(LessonID, @"[^0-9a-zA-Z]+", "s");

            // Lesson.EncryptedID.Replace('/', 's').Replace('-','s').Replace('+','s').Replace('%','s').Replace('&','s');

            var stringlength = newString.Length;

            if (newString.Length>1500)
            {


              newString = newString.Substring(0, 1500);
         
            }

            Lesson.EncryptedID = newString;


            //Lesson_Session lessonSession = new Lesson_Session();
            //lessonSession.LessonId = Lesson.Id;
            //lessonSession.SessionId = LessonViewModel.SessionId;
            //lessonSession.StartDate = LessonViewModel.StartDate;
            //lessonSession.DueDate = LessonViewModel.DueDate;
            //db.Lesson_Session.Add(lessonSession);

            //db.SaveChanges();



            HttpPostedFileBase Assignment = Request.Files["Assignment"];
            HttpPostedFileBase Attachment1 = Request.Files["Attachment1"];
            HttpPostedFileBase Attachment2 = Request.Files["Attachment2"];
            HttpPostedFileBase Attachment3 = Request.Files["Attachment3"];
            HttpPostedFileBase AttachmentImage = Request.Files["AttachmentImage"];

            db.AspnetLessons.Add(Lesson);
            db.SaveChanges();

            var EncryptedtoAppend = db.AspnetLessons.Where(x => x.Id == Lesson.Id).FirstOrDefault();
            EncryptedtoAppend.EncryptedID = EncryptedtoAppend.EncryptedID + EncryptedtoAppend.Id;
            db.SaveChanges();

            if (LessonViewModel.ContentType == "Image")
            {
                // var fileName = Path.GetFileName(AttachmentImage.FileName);
                //AttachmentImage.SaveAs(Server.MapPath("~/Content/LessonImage/") + fileName);


                var name = Path.GetFileNameWithoutExtension(AttachmentImage.FileName);

                var ext = Path.GetExtension(AttachmentImage.FileName);

                //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                var fileName = name + "_LI_" + Lesson.Id + ext;



                AttachmentImage.SaveAs(Server.MapPath("~/Content/LessonImage/") + fileName);

                Lesson.LessonIMG = fileName;
                db.SaveChanges();
            }


            var subjectName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name).FirstOrDefault();
            var ClassName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name).FirstOrDefault();
            var SectionName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name).FirstOrDefault();

            var events = new Event();
            events.UserId = null;
            events.ThemeColor = "Gray";
            events.Subject = "";
            events.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
            events.Sec_Title = Lesson.Name;
            events.Description1 = Lesson.Description;
            events.Start = Lesson.StartTime.Value;
            events.End = Lesson.EndTime;
            events.LessonID = Lesson.Id;
            events.IsFullDay = false;
            events.IsPublic = false;
            events.SubjectClass = "Not Published";
            events.Url = "/TeacherCommentsOnCourses/StudentLessons/?id=" + Lesson.Id;

            db.Events.Add(events);
            db.SaveChanges();

            var event_user = new AspnetEvent_User();
            event_user.userid = User.Identity.GetUserId();
            event_user.eventid = events.EventID;
            db.AspnetEvent_User.Add(event_user);

            db.SaveChanges();

            var branchid = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId).FirstOrDefault();
            var branchAdmin = db.AspNetBranch_Admins.Where(x => x.BranchId == branchid).Select(x => x.AdminId).ToList();

            //  var parents = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Parent")  || x.AspNetRoles.Select(y=>y.Name).Contains("Teacher")  && x.StatusId == null).ToList();
            //ViewBag.BranchPrincipalId = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Branch_Principal")), "Id", "Email"); C: \Users\TRA\Documents\GitHub\GSIS - V4\Sea_GsIs\SEA_Application\Controllers\AspNetBranchesController.cs  41  82

            foreach (var item in branchAdmin)
            {
                var event_user1 = new AspnetEvent_User();
                event_user1.userid = item;
                event_user1.eventid = events.EventID;
                db.AspnetEvent_User.Add(event_user1);
                db.SaveChanges();
            }

            string fileName1 = null;
            if (Assignment.ContentLength > 0)
            {
                // var fileName = Path.GetFileName(Assignment.FileName);
                //Assignment.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);
                var name = Path.GetFileNameWithoutExtension(Assignment.FileName);
                var ext = Path.GetExtension(Assignment.FileName);
                //   var fileName = Path.GetFileName(AttachmentFile.FileName);
                fileName1 = name + "_LA_" + Lesson.Id + ext;
                Assignment.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName1);
            }

            AspnetStudentAssignment studentAssignment = new AspnetStudentAssignment();
            studentAssignment.FileName = fileName1;
            studentAssignment.Name = LessonViewModel.AssignmentName;
            studentAssignment.TotalMarks = LessonViewModel.TotalMarks;
            studentAssignment.AssignmentType = LessonViewModel.AssignmentType;

            string DueDate = Convert.ToString(LessonViewModel.AssignmentDueDate);

            if (DueDate == "1/1/0001 12:00:00 AM")
            {
                studentAssignment.DueDate = null;
            }
            else
            {
                studentAssignment.DueDate = LessonViewModel.AssignmentDueDate;
            }

            studentAssignment.Description = LessonViewModel.AssignmentDescription;
            //TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            studentAssignment.CreationDate = PKTime.Date;
            studentAssignment.LessonId = Lesson.Id;

            if (studentAssignment.DueDate != null)
            {
                db.AspnetStudentAssignments.Add(studentAssignment);
                db.SaveChanges();
            }

            if (Attachment1.ContentLength > 0)
            {
                //var fileName = Path.GetFileName(Attachment1.FileName);
                // Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

                AspnetStudentAttachment studentAttachment1 = new AspnetStudentAttachment();

                studentAttachment1.Name = LessonViewModel.AttachmentName1;
                studentAttachment1.CreationDate = DateTime.Now;
                studentAttachment1.LessonId = Lesson.Id;
                db.AspnetStudentAttachments.Add(studentAttachment1);
                db.SaveChanges();


                var name = Path.GetFileNameWithoutExtension(Attachment1.FileName);

                var ext = Path.GetExtension(Attachment1.FileName);

                //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                var fileName = name + "_LM_" + studentAttachment1.Id + ext;


                Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

                studentAttachment1.Path = fileName;

                db.SaveChanges();

            }
            if (Attachment2.ContentLength > 0)
            {

                // var fileName = Path.GetFileName(Attachment2.FileName);
                // Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

                AspnetStudentAttachment studentAttachment2 = new AspnetStudentAttachment();

                studentAttachment2.Name = LessonViewModel.AttachmentName2;
                //studentAttachment2.Path = fileName;
                studentAttachment2.CreationDate = DateTime.Now;
                studentAttachment2.LessonId = Lesson.Id;
                db.AspnetStudentAttachments.Add(studentAttachment2);

                db.SaveChanges();


                var name = Path.GetFileNameWithoutExtension(Attachment2.FileName);

                var ext = Path.GetExtension(Attachment2.FileName);

                //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                var fileName = name + "_LM_" + studentAttachment2.Id + ext;


                Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + Attachment2);

                studentAttachment2.Path = fileName;
                db.SaveChanges();


            }

            if (Attachment3.ContentLength > 0)
            {

                //var fileName = Path.GetFileName(Attachment3.FileName);
                // Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

                AspnetStudentAttachment studentAttachment3 = new AspnetStudentAttachment();

                studentAttachment3.Name = LessonViewModel.AttachmentName3;
                //studentAttachment3.Path = fileName;
                studentAttachment3.CreationDate = DateTime.Now;
                studentAttachment3.LessonId = Lesson.Id;
                db.AspnetStudentAttachments.Add(studentAttachment3);
                db.SaveChanges();



                var name = Path.GetFileNameWithoutExtension(Attachment3.FileName);

                var ext = Path.GetExtension(Attachment3.FileName);

                //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                var fileName = name + "_LM_" + studentAttachment3.Id + ext;

                Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

                studentAttachment3.Path = fileName;

                db.SaveChanges();


            }

            if (LessonViewModel.LinkUrl1 != null)
            {
                AspnetStudentLink link1 = new AspnetStudentLink();

                link1.URL = LessonViewModel.LinkUrl1;
                link1.CreationDate = DateTime.Now;
                link1.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link1);
                db.SaveChanges();
            }

            if (LessonViewModel.LinkUrl2 != null)
            {
                AspnetStudentLink link2 = new AspnetStudentLink();

                link2.URL = LessonViewModel.LinkUrl2;
                link2.CreationDate = DateTime.Now;
                link2.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link2);
                db.SaveChanges();
            }


            if (LessonViewModel.LinkUrl3 != null)
            {
                AspnetStudentLink link3 = new AspnetStudentLink();

                link3.URL = LessonViewModel.LinkUrl3;
                link3.CreationDate = DateTime.Now;
                link3.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link3);
                db.SaveChanges();
            }



            TempData["LessonCreated"] = "Created";
            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });

        }

        public ActionResult populateStudentEvents()
        {
            var Lessons = db.AspnetLessons.Where(x => x.Status == true).ToList();
            try
            {
                foreach (var Lesson in Lessons)
                {
                    var branchID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId;
                    var ClassID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId;
                    var SectionID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId;
                    var SubjectID = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId;

                    var BCId = db.AspNetBranch_Class.Where(x => x.ClassId == ClassID && x.BranchId == branchID).Select(x => x.Id).FirstOrDefault();
                    var BCSId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCId && x.SectionId == SectionID).Select(x => x.Id).FirstOrDefault();
                    var CSId = db.AspNetClass_Courses.Where(x => x.ClassId == ClassID && x.CourseId == SubjectID).Select(x => x.Id).FirstOrDefault();

                    var Students = db.AspNetStudent_Enrollments.Where(x => x.SectionId == BCSId && x.CourseId == CSId).Select(x => x.AspNetStudent.AspNetUser.Id).ToList();

                    foreach (var item in Students)
                    {
                        var user = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                        if (user == null)
                        {
                            continue;
                        }
                        var events1 = new Event();
                        events1.UserId = item;
                        events1.ThemeColor = "Green";
                        events1.Subject = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name + "-" + Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name + "-" + Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name;
                        events1.Sec_Title = Lesson.Name;
                        events1.Description = Lesson.Description;
                        events1.Start = Lesson.StartTime.Value;
                        if (Lesson.StartTime.Value >= Lesson.EndTime)
                        {
                            events1.End = Lesson.StartTime.Value.AddHours(1);
                        }
                        else
                        {
                            events1.End = Lesson.EndTime;
                        }
                        events1.LessonID = Lesson.Id;
                        events1.IsFullDay = false;
                        events1.IsPublic = false;
                        events1.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        // Lesson.EncryptedID
                        events1.Url = "/StudentCourses/StudentLessons/?id=" + Lesson.EncryptedID;


                        db.Events.Add(events1);
                        db.SaveChanges();
                    }
                }


            }
            catch (Exception e)
            {
                var logs = new AspNetLog();
                logs.Operation = "Lesson Publish -- Exception: " + e.Message + "--- Inner Exception: " + e.InnerException;
                logs.OperationStartTime = DateTime.Now;
                logs.UserId = User.Identity.GetUserId();
            }

            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");
        }
        public ActionResult populateBranchEvents()
        {
            try
            {
                var Lessons = db.AspnetLessons.ToList();

                foreach (var Lesson in Lessons)
                {
                    var branchid = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.BranchId).FirstOrDefault();
                    var branchAdmin = db.AspNetBranch_Admins.Where(x => x.BranchId == branchid).Select(x => x.AdminId).ToList();
                    foreach (var item in branchAdmin)
                    {
                        var user = db.AspNetUsers.Where(x => x.Id == item).FirstOrDefault();
                        if (user == null)
                        {
                            continue;
                        }
                        var events1 = new Event();
                        events1.UserId = item;
                        events1.ThemeColor = "Gray";
                        events1.Subject = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name + "-" + Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name + "-" + Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name;
                        events1.Sec_Title = Lesson.Name;
                        events1.Description = Lesson.Description;
                        events1.Start = Lesson.StartTime.Value;
                        if (Lesson.StartTime.Value >= Lesson.EndTime)
                        {
                            events1.End = Lesson.StartTime.Value.AddHours(1);
                        }
                        else
                        {
                            events1.End = Lesson.EndTime;
                        }
                        events1.LessonID = Lesson.Id;
                        events1.IsFullDay = false;
                        events1.IsPublic = false;
                        if (Lesson.Status == true)
                        {
                            events1.SubjectClass = Lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name;
                        }
                        else
                        {
                            events1.SubjectClass = "Not Published";
                        }
                        events1.Url = "/TeacherCommentsOnCourses/StudentLessons/?id=" + Lesson.Id;

                        db.Events.Add(events1);
                        db.SaveChanges();
                        //break;
                    }
                    //break;
                }
            }
            catch (Exception ex)
            {
                int a = 0;
            }

            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");
        }

        public JsonResult SessionByLesson(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var subs = (from session in db.Lesson_Session
                        where session.LessonId == id
                        select new { session.SessionId, session.LessonId }).ToList();


            return Json(subs, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LessonSessionView()
        {


            return View("LessonSessionView");
        }

        public ActionResult GetLessonSessions()
        {
            var ID = User.Identity.GetUserId();

            var AllLessonSessions = (from lesson in db.AspnetLessons
                                     join lessonsesion in db.Lesson_Session on lesson.Id equals lessonsesion.LessonId
                                     join enrollment in db.AspNetTeacher_Enrollments on lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SubjectId equals enrollment.AspNetClass_Courses.AspNetCours.Id
                                     where enrollment.AspNetEmployee.UserId == ID && enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.ClassId
                                     && enrollment.AspNetBranchClass_Sections.AspNetSection.Id == lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.SectionId
                                     select new
                                     {
                                         lessonsesion.Id,
                                         lesson.Name,
                                         lessonsesion.AspNetSession.Year,
                                         lessonsesion.StartDate,
                                         lessonsesion.DueDate,
                                         classs = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name,
                                         SubjectName = lesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name
                                     });


            return Json(AllLessonSessions, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EditLessonSession(int id)
        {

            ViewBag.Id = id;

            var LS = db.Lesson_Session.Where(x => x.Id == id).FirstOrDefault();

            if (LS != null)
            {


                int? TopicId = db.AspnetLessons.Where(x => x.Id == LS.LessonId).FirstOrDefault().TopicId;



                int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;



                var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();


                var Branches = (from branch in db.AspNetBranches
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                                select new
                                {
                                    branch.Id,
                                    branch.Name,

                                }).Distinct();
                var Classes = (from classs in db.AspNetClasses
                               join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                               where (branchclasssubject.BranchId == GenericObject.BranchId)
                               select new
                               {
                                   classs.Id,
                                   classs.Name,
                               }).Distinct();


                var Sections = (from section in db.AspNetSections
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on section.Id equals branchclasssubject.SectionId
                                where (branchclasssubject.ClassId == GenericObject.ClassId)
                                select new
                                {
                                    section.Id,
                                    section.Name,

                                }).Distinct();




                var Subjects = (from subject in db.AspNetCourses
                                join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                                where (branchclasssubject.SectionId == GenericObject.SectionId)
                                select new
                                {
                                    subject.Id,
                                    subject.Name,

                                }).Distinct();


                var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == GenericObject.BranchId && x.ClassId == GenericObject.ClassId && x.SubjectId == GenericObject.SubjectId && x.SectionId == GenericObject.SectionId).FirstOrDefault();

                var genericTableIid = Generic.Id;

                var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == genericTableIid).ToList().Select(x => new { x.Id, x.Name });


                ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
                ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
                ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
                ViewBag.SubId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);
                ViewBag.TopicId = new SelectList(Topics, "Id", "Name", TopicId);


                ViewBag.LessonId = new SelectList(db.AspnetLessons.Where(x => x.TopicId == TopicId), "Id", "Name", LS.LessonId);


                var StartDate = Convert.ToDateTime(LS.StartDate);

                var StartDateInString = StartDate.ToString("yyyy-MM-dd");

                ViewBag.LessonStartDate = StartDateInString;

                ////Due Date
                var DueDate = Convert.ToDateTime(LS.DueDate);

                var DueDateInString = DueDate.ToString("yyyy-MM-dd");


                ViewBag.LessonDueDate = DueDateInString;

            }


            return View();
        }
        [HttpPost]
        public ActionResult EditLessonSession(int SubId, int TopicId)
        {


            var LessonSessionId = Convert.ToInt64(Request.Form["LessonSessionId"]);

            var LessonSessionToDelete = db.Lesson_Session.Where(x => x.Id == LessonSessionId).FirstOrDefault();

            db.Lesson_Session.Remove(LessonSessionToDelete);
            db.SaveChanges();



            //   var SessionId1 = Request.Form["SessionId"];
            var LessonId = Request.Form["LessonId"];
            var StartDate = Request.Form["StartDate"];
            var DueDate = Request.Form["DueDate"];



            Lesson_Session ls = new Lesson_Session();

            ls.LessonId = Convert.ToInt32(LessonId);
            ls.SessionId = null;
            ls.StartDate = Convert.ToDateTime(StartDate);
            ls.DueDate = Convert.ToDateTime(DueDate);


            db.Lesson_Session.Add(ls);
            db.SaveChanges();

            return RedirectToAction("LessonSessionView");


        }

        public ActionResult CreateLessonSession()
        {

            //ViewBag.TopicId = new SelectList(db.AspnetSubjectTopics, "Id", "Name");

            //ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName");

            //ViewBag.LessonId = new SelectList(db.AspnetLessons, "Id", "Name");







            return View();

        }
        [HttpPost]
        public ActionResult CreateLessonSession(int SectionId)
        {

            // var SessionId1 = Request.Form["SessionId"];
            var LessonId = Request.Form["LessonId"];
            var StartDate = Request.Form["StartDate"];
            var DueDate = Request.Form["DueDate"];



            Lesson_Session ls = new Lesson_Session();

            ls.LessonId = Convert.ToInt32(LessonId);
            ls.SessionId = null;
            ls.StartDate = Convert.ToDateTime(StartDate);
            ls.DueDate = Convert.ToDateTime(DueDate);


            db.Lesson_Session.Add(ls);
            db.SaveChanges();

            return RedirectToAction("LessonSessionView");
        }




        public ActionResult LessonDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            if (aspnetLesson == null)
            {
                return HttpNotFound();
            }

            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Where(x => x.LessonId == aspnetLesson.Id).FirstOrDefault();
            List<AspnetStudentAttachment> studentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            List<AspnetStudentLink> studentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.LessonDescription = aspnetLesson.Description;
            lessonViewModel.LessonVideoURL = aspnetLesson.Video_Url;
            lessonViewModel.LessonName = aspnetLesson.Name;
            lessonViewModel.LessonDuration = aspnetLesson.DurationMinutes;

            //    Lesson_Session LessonSession = db.Lesson_Session.Where(x => x.LessonId == id).FirstOrDefault();

            lessonViewModel.IsActive = Convert.ToBoolean(aspnetLesson.IsActive);

            //var StartDate = Convert.ToDateTime(LessonSession.StartDate);

            //var StartDateInString = StartDate.ToString("yyyy-MM-dd");

            //ViewBag.LessonStartDate = StartDateInString;

            ////Due Date
            //var DueDate = Convert.ToDateTime(LessonSession.DueDate);

            //var DueDateInString = DueDate.ToString("yyyy-MM-dd");


            //ViewBag.LessonDueDate = DueDateInString;


            int? TopicId = aspnetLesson.TopicId;

            // ViewBag.LessonDuration = aspnetLesson.DurationMinutes;

            //int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;
            //GenericSubject Subject = db.GenericSubjects.Where(x => x.Id == SubjectId).FirstOrDefault();



            // var CourseType = Subject.SubjectType;

            lessonViewModel.Id = aspnetLesson.Id;



            if (studentAssignment != null)
            {
                lessonViewModel.AssignmentName = studentAssignment.Name;
                lessonViewModel.AssignmentDescription = studentAssignment.Description;
                DateTime Date = Convert.ToDateTime(studentAssignment.DueDate);
                string date = Date.ToString("yyyy-MM-dd");

                ViewBag.AssignmentFileName = studentAssignment.FileName;

                lessonViewModel.AssignmentDueDate = studentAssignment.DueDate;
                ViewBag.Date = date;


            }


            int count = 1;

            foreach (var link in studentLinks)
            {

                if (count == 1)
                {

                    lessonViewModel.LinkUrl1 = link.URL;

                }
                else if (count == 2)
                {

                    lessonViewModel.LinkUrl2 = link.URL;

                }
                else if (count == 3)
                {
                    lessonViewModel.LinkUrl3 = link.URL;


                }
                else
                {

                }

                count++;

            }

            count = 1;
            foreach (var attachment in studentAttachments)
            {

                if (count == 1)
                {

                    lessonViewModel.AttachmentName1 = attachment.Name;
                    ViewBag.Attachment1FileName = attachment.Path;

                }
                else if (count == 2)
                {

                    lessonViewModel.AttachmentName2 = attachment.Name;
                    ViewBag.Attachment2FileName = attachment.Path;


                }
                else if (count == 3)
                {
                    lessonViewModel.AttachmentName3 = attachment.Name;
                    ViewBag.Attachment3FileName = attachment.Path;



                }
                else
                {

                }

                count++;

            }



            int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;

            //    ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName", Subject.ClassID);
            //     ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x=>x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            // ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x => x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();



            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            select new
                            {
                                branch.Id,
                                branch.Name,

                            }).Distinct();


            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           where (branchclasssubject.BranchId == GenericObject.BranchId)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();


            var Sections = (from section in db.AspNetSections
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on section.Id equals branchclasssubject.SectionId
                            where (branchclasssubject.ClassId == GenericObject.ClassId)
                            select new
                            {
                                section.Id,
                                section.Name,

                            }).Distinct();




            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            where (branchclasssubject.SectionId == GenericObject.SectionId)
                            select new
                            {
                                subject.Id,
                                subject.Name,

                            }).Distinct();


            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == GenericObject.BranchId && x.ClassId == GenericObject.ClassId && x.SubjectId == GenericObject.SubjectId && x.SectionId == GenericObject.SectionId).FirstOrDefault();

            var genericTableIid = Generic.Id;

            var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == genericTableIid).ToList().Select(x => new { x.Id, x.Name });

            //  string AllTopics = Newtonsoft.Json.JsonConvert.SerializeObject(Topics);


            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
            ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
            ViewBag.SubId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);
            ViewBag.TopicId = new SelectList(Topics, "Id", "Name", aspnetLesson.TopicId);


            //  ViewBag.CTId = Subject.SubjectType;

            ViewBag.OrderBy = aspnetLesson.OrderBy;
            ViewBag.LessonDuration = aspnetLesson.DurationMinutes;
            return View(lessonViewModel);
        }
        // GET: AspnetLessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            if (aspnetLesson == null)
            {
                return HttpNotFound();
            }

            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Where(x => x.LessonId == aspnetLesson.Id).FirstOrDefault();
            List<AspnetStudentAttachment> studentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            List<AspnetStudentLink> studentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.LessonDescription = aspnetLesson.Description;
            lessonViewModel.LessonVideoURL = aspnetLesson.Video_Url;
            lessonViewModel.LessonName = aspnetLesson.Name;
            lessonViewModel.LessonDuration = aspnetLesson.DurationMinutes;
            lessonViewModel.MeetingLink = aspnetLesson.MeetingLink;


            if (aspnetLesson.StartDate != null && aspnetLesson.StartTime != null)
            {


                DateTime LessonStartDate = Convert.ToDateTime(aspnetLesson.StartDate);
                string StartDateOfLEsson = LessonStartDate.ToString("yyyy-MM-dd");
                ViewBag.LessonStartDate = StartDateOfLEsson;
                string StartTimeOfLEsson = aspnetLesson.StartTime.Value.TimeOfDay.ToString();
                ViewBag.LessonStartTime = StartTimeOfLEsson;


            }


            if (aspnetLesson.ContentType != null)
            {
                lessonViewModel.ContentType = aspnetLesson.ContentType;

                if (aspnetLesson.ContentType == "Image")
                {
                    ViewBag.ContentTypeImage = aspnetLesson.LessonIMG;
                }
            }
            else
            {
                lessonViewModel.ContentType = "";

            }


            //    Lesson_Session LessonSession = db.Lesson_Session.Where(x => x.LessonId == id).FirstOrDefault();

            lessonViewModel.IsActive = Convert.ToBoolean(aspnetLesson.IsActive);

            //var StartDate = Convert.ToDateTime(LessonSession.StartDate);

            //var StartDateInString = StartDate.ToString("yyyy-MM-dd");

            //ViewBag.LessonStartDate = StartDateInString;

            ////Due Date
            //var DueDate = Convert.ToDateTime(LessonSession.DueDate);

            //var DueDateInString = DueDate.ToString("yyyy-MM-dd");


            //ViewBag.LessonDueDate = DueDateInString;


            int? TopicId = aspnetLesson.TopicId;

            // ViewBag.LessonDuration = aspnetLesson.DurationMinutes;

            //int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;
            //GenericSubject Subject = db.GenericSubjects.Where(x => x.Id == SubjectId).FirstOrDefault();



            // var CourseType = Subject.SubjectType;

            lessonViewModel.Id = aspnetLesson.Id;


            if (studentAssignment != null)
            {
                lessonViewModel.AssignmentName = studentAssignment.Name;
                lessonViewModel.AssignmentDescription = studentAssignment.Description;
                lessonViewModel.TotalMarks = Convert.ToDouble(studentAssignment.TotalMarks);
                lessonViewModel.AssignmentType = studentAssignment.AssignmentType;

                DateTime Date = Convert.ToDateTime(studentAssignment.DueDate);
                string date;

                if (studentAssignment.DueDate == null)
                {
                    date = "0000-00-00";
                }
                else
                {
                    date = Date.ToString("yyyy-MM-dd");
                }


                ViewBag.AssignmentFileName = studentAssignment.FileName;
                lessonViewModel.AssignmentDueDate = studentAssignment.DueDate;
                ViewBag.Date = date;


            }


            int count = 1;

            foreach (var link in studentLinks)
            {

                if (count == 1)
                {

                    lessonViewModel.LinkUrl1 = link.URL;

                }
                else if (count == 2)
                {

                    lessonViewModel.LinkUrl2 = link.URL;

                }
                else if (count == 3)
                {
                    lessonViewModel.LinkUrl3 = link.URL;


                }
                else
                {

                }

                count++;

            }

            count = 1;
            foreach (var attachment in studentAttachments)
            {

                if (count == 1)
                {

                    lessonViewModel.AttachmentName1 = attachment.Name;
                    ViewBag.Attachment1FileName = attachment.Path;

                }
                else if (count == 2)
                {

                    lessonViewModel.AttachmentName2 = attachment.Name;
                    ViewBag.Attachment2FileName = attachment.Path;


                }
                else if (count == 3)
                {
                    lessonViewModel.AttachmentName3 = attachment.Name;
                    ViewBag.Attachment3FileName = attachment.Path;



                }
                else
                {

                }

                count++;

            }




            int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;

            //    ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName", Subject.ClassID);
            //     ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x=>x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);

            // ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x => x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);

            var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();

            int? GenericBranchId = GenericObject.BranchId;
            int? GenericClassId = GenericObject.ClassId;
            int? GenericSectionId = GenericObject.SectionId;
            int? GenericSubjectId = GenericObject.SubjectId;


            int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == GenericBranchId && x.ClassId == GenericClassId).FirstOrDefault().Id;

            int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == GenericSectionId).FirstOrDefault().Id;

            int ClassCoursesID = db.AspNetClass_Courses.Where(x => x.ClassId == GenericObject.ClassId && x.CourseId == GenericSubjectId).Select(x => x.Id).FirstOrDefault();

            int TeacherId = db.AspNetTeacher_Enrollments.Where(x => x.SectionId == BranchClassSectionId && x.CourseId == ClassCoursesID).Select(x => x.TeacherId).FirstOrDefault();

            var TeacherUserId = db.AspNetEmployees.Where(x => x.Id == TeacherId).FirstOrDefault().UserId;


            var ID = User.Identity.GetUserId();

            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetEmployee.BranchId
                            where enrollment.AspNetEmployee.UserId == TeacherUserId
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();


            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                           where (branchclasssubject.BranchId == GenericObject.BranchId && enrollment.AspNetEmployee.UserId == TeacherUserId)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();



            var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == TeacherUserId && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId).Select(x => new
            {
                Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                Name = x.AspNetBranchClass_Sections.AspNetSection.Name
            }).Distinct();


            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                            where (branchclasssubject.SectionId == GenericObject.SectionId && enrollment.AspNetEmployee.UserId == TeacherUserId)
                            select new
                            {
                                subject.Id,
                                subject.Name,
                            }).Distinct();


            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == GenericObject.BranchId && x.ClassId == GenericObject.ClassId && x.SubjectId == GenericObject.SubjectId && x.SectionId == GenericObject.SectionId).FirstOrDefault();

            var genericTableIid = Generic.Id;

            var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == genericTableIid).ToList().Select(x => new { x.Id, x.Name });

            // var AssignmentType = db.AspnetStudentAssignments.ToList().Select(x => new { Id = x.AssignmentType, Name = x.AssignmentType }).Distinct();

             var AssignmentType = db.AspnetStudentAssignments.Select(x => new { Id = x.AssignmentType, Name = x.AssignmentType }).Distinct().Where(x=>x.Name !="" && x.Id !="" && x.Name != null && x.Id !=null).ToList();


            //  string AllTopics = Newtonsoft.Json.JsonConvert.SerializeObject(Topics);


            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
            ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
            ViewBag.SubId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);
            ViewBag.TopicId = new SelectList(Topics, "Id", "Name", aspnetLesson.TopicId);
            ViewBag.LessonStatus = aspnetLesson.Status;
            ViewBag.AssignmentType = new SelectList(AssignmentType, "Id", "Name", lessonViewModel.AssignmentType);

            //  ViewBag.CTId = Subject.SubjectType;

            ViewBag.OrderBy = aspnetLesson.OrderBy;
            ViewBag.LessonDuration = aspnetLesson.DurationMinutes;
            return View(lessonViewModel);
        }

        // POST: AspnetLessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LessonViewModel LessonViewModel)
        {
            AspnetLesson Lesson = db.AspnetLessons.Where(x => x.Id == LessonViewModel.Id).FirstOrDefault();
            Lesson.Name = LessonViewModel.LessonName;
            Lesson.Video_Url = LessonViewModel.LessonVideoURL;
            Lesson.TopicId = LessonViewModel.TopicId;
            Lesson.DurationMinutes = LessonViewModel.LessonDuration;
            Lesson.Description = LessonViewModel.LessonDescription;
            //Lesson.IsActive = LessonViewModel.IsActive;
            Lesson.OrderBy = LessonViewModel.OrderBy;
            // Lesson.Status = false;
            Lesson.ContentType = LessonViewModel.ContentType;
            Lesson.MeetingLink = LessonViewModel.MeetingLink;

            Lesson.StartDate = LessonViewModel.StartDate;

            DateTime startDate = DateTime.Parse(LessonViewModel.StartDate.ToString());

            DateTime start = DateTime.Parse(LessonViewModel.StartTime.ToString());

            Lesson.StartTime = startDate.Date.Add(start.TimeOfDay);
            Lesson.EndTime = Lesson.StartTime.Value.AddMinutes(60);

            var subjectName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name).FirstOrDefault();
            var ClassName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name).FirstOrDefault();
            var SectionName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name).FirstOrDefault();

            var eventList = db.Events.Where(x => x.LessonID == Lesson.Id).ToList();

            if (eventList == null)
            {
                // the below code creates event if it's not created or deleted 
                //var subjectName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetCours.Name).FirstOrDefault();
                //var ClassName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name).FirstOrDefault();
                //var SectionName = db.AspnetLessons.Where(x => x.Id == Lesson.Id).Select(x => x.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name).FirstOrDefault();

                //var events = new Event();
                //events.UserId = null;
                //events.ThemeColor = "Red";
                //events.Subject = "";
                //events.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                //events.Sec_Title = Lesson.Name;
                //events.Description1 = Lesson.Description;
                //events.Start = Lesson.StartTime.Value;
                //events.End = Lesson.EndTime;
                //events.LessonID = Lesson.Id;
                //events.IsFullDay = false;
                //events.IsPublic = false;
                //events.SubjectClass = ClassName;
                //events.Url = "/TeacherCommentsOnCourses/StudentLessons/?id=" + Lesson.Id;

                //db.Events.Add(events);
                //db.SaveChanges();

                //var event_user = new AspnetEvent_User();
                //event_user.userid = User.Identity.GetUserId();
                //event_user.eventid = events.EventID;
                //db.AspnetEvent_User.Add(event_user);

                //db.SaveChanges();
            }
            else
            {
                foreach (var item in eventList)
                {
                    item.Subject1 = subjectName + "-" + ClassName + "-" + SectionName;
                    item.Sec_Title = Lesson.Name;
                    item.Description1 = Lesson.Description;
                    item.Start = Lesson.StartTime.Value;
                    item.End = Lesson.EndTime;
                    db.SaveChanges();
                }
            }


            if (LessonViewModel.ContentType == "Image")
            {
                Lesson.MeetingLink = null;
                Lesson.Video_Url = null;
            }

            if (LessonViewModel.ContentType == null)
            {
                Lesson.MeetingLink = null;
                Lesson.Video_Url = null;
                Lesson.LessonIMG = null;
            }

            db.SaveChanges();

            HttpPostedFileBase Assignment = Request.Files["Assignment"];
            HttpPostedFileBase AttachmentImage = Request.Files["AttachmentImage"];
            HttpPostedFileBase Attachment1 = Request.Files["Attachment1"];
            HttpPostedFileBase Attachment2 = Request.Files["Attachment2"];
            HttpPostedFileBase Attachment3 = Request.Files["Attachment3"];

            var fileName = "";
            if (Assignment.ContentLength > 0)
            {
                //   fileName = Path.GetFileName(Assignment.FileName);
                // Assignment.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);


                var name = Path.GetFileNameWithoutExtension(Assignment.FileName);

                var ext = Path.GetExtension(Assignment.FileName);

                //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                fileName = name + "_LA_" + Lesson.Id + ext;


                Assignment.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);



            }
            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Where(x => x.LessonId == Lesson.Id).FirstOrDefault();

            if (studentAssignment != null)
            {

                if (fileName != "")
                {

                    studentAssignment.FileName = fileName;

                }

                studentAssignment.Name = LessonViewModel.AssignmentName;
                string DueDate = Convert.ToString(LessonViewModel.AssignmentDueDate);

                if (DueDate == "1/1/0001 12:00:00 AM")
                {
                    studentAssignment.DueDate = null;
                }
                else
                {
                    studentAssignment.DueDate = LessonViewModel.AssignmentDueDate;

                }

                studentAssignment.Description = LessonViewModel.AssignmentDescription;
                studentAssignment.TotalMarks = LessonViewModel.TotalMarks;

                db.SaveChanges();
            }
            else
            {
                if (Assignment.ContentLength > 0)
                {

                    AspnetStudentAssignment studentAssignment1 = new AspnetStudentAssignment();

                    studentAssignment1.FileName = fileName;

                    studentAssignment1.Name = LessonViewModel.AssignmentName;


                    string DueDate = Convert.ToString(LessonViewModel.AssignmentDueDate);


                    if (DueDate == "1/1/0001 12:00:00 AM")
                    {
                        studentAssignment1.DueDate = null;

                    }
                    else
                    {

                        studentAssignment1.DueDate = LessonViewModel.AssignmentDueDate;

                    }

                    studentAssignment1.TotalMarks = LessonViewModel.TotalMarks;

                    studentAssignment1.Description = LessonViewModel.AssignmentDescription;
                    studentAssignment1.CreationDate = DateTime.Now;
                    studentAssignment1.LessonId = Lesson.Id;

                    db.AspnetStudentAssignments.Add(studentAssignment1);
                    db.SaveChanges();



                }

            }

            if (LessonViewModel.ContentType == "Image")
            {
                // if (AttachmentImage != null)
                // {

                if (AttachmentImage.ContentLength > 0)
                {
                    // fileName = Path.GetFileName(AttachmentImage.FileName);
                    // AttachmentImage.SaveAs(Server.MapPath("~/Content/LessonImage/") + fileName);


                    var name = Path.GetFileNameWithoutExtension(AttachmentImage.FileName);

                    var ext = Path.GetExtension(AttachmentImage.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    fileName = name + "_LI_" + Lesson.Id + ext;


                    AttachmentImage.SaveAs(Server.MapPath("~/Content/LessonImage/") + fileName);


                    Lesson.LessonIMG = fileName;
                    db.SaveChanges();

                }
            }

            //}

            List<AspnetStudentAttachment> studentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == Lesson.Id).ToList();
            List<AspnetStudentLink> studentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == Lesson.Id).ToList();

            //db.AspnetStudentAttachments.RemoveRange(studentAttachments);
            //db.SaveChanges();

            db.AspnetStudentLinks.RemoveRange(studentLinks);
            db.SaveChanges();

            Sea_Entities db1 = new Sea_Entities();

            List<AspnetStudentAttachment> studentAttachments1 = db1.AspnetStudentAttachments.Where(x => x.LessonId == Lesson.Id).ToList();

            int TotalAttachments = studentAttachments1.Count;

            if (TotalAttachments == 0)
            {
                if (Attachment1.ContentLength > 0)
                {
                    // var fileName1 = Path.GetFileName(Attachment1.FileName);
                    // Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                    AspnetStudentAttachment studentAttachment1 = new AspnetStudentAttachment();

                    studentAttachment1.Name = LessonViewModel.AttachmentName1;
                    // studentAttachment1.Path = fileName1;
                    studentAttachment1.CreationDate = DateTime.Now;
                    studentAttachment1.LessonId = Lesson.Id;
                    db.AspnetStudentAttachments.Add(studentAttachment1);
                    db.SaveChanges();


                    var name = Path.GetFileNameWithoutExtension(Attachment1.FileName);

                    var ext = Path.GetExtension(Attachment1.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    var FileNameForAttachment1 = name + "_LM_" + studentAttachment1.Id + ext;
                    Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + FileNameForAttachment1);
                    studentAttachment1.Path = FileNameForAttachment1;


                    db.SaveChanges();


                }
                if (Attachment2.ContentLength > 0)
                {

                    // var fileName1 = Path.GetFileName(Attachment2.FileName);
                    // Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                    AspnetStudentAttachment studentAttachment2 = new AspnetStudentAttachment();

                    studentAttachment2.Name = LessonViewModel.AttachmentName2;
                    // studentAttachment2.Path = fileName1;
                    studentAttachment2.CreationDate = DateTime.Now;
                    studentAttachment2.LessonId = Lesson.Id;
                    db.AspnetStudentAttachments.Add(studentAttachment2);

                    db.SaveChanges();


                    var name = Path.GetFileNameWithoutExtension(Attachment2.FileName);

                    var ext = Path.GetExtension(Attachment2.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    var Attachment2FileName = name + "_LM_" + studentAttachment2.Id + ext;


                    Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + Attachment2FileName);

                    studentAttachment2.Path = Attachment2FileName;

                    db.SaveChanges();





                }

                if (Attachment3.ContentLength > 0)
                {

                    // var fileName1 = Path.GetFileName(Attachment3.FileName);
                    //Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                    AspnetStudentAttachment studentAttachment3 = new AspnetStudentAttachment();

                    studentAttachment3.Name = LessonViewModel.AttachmentName3;
                    //  studentAttachment3.Path = fileName1;
                    studentAttachment3.CreationDate = DateTime.Now;
                    studentAttachment3.LessonId = Lesson.Id;
                    db.AspnetStudentAttachments.Add(studentAttachment3);
                    db.SaveChanges();



                    var name = Path.GetFileNameWithoutExtension(Attachment3.FileName);

                    var ext = Path.GetExtension(Attachment3.FileName);

                    //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                    var StudentAttachment3FileName = name + "_LM_" + studentAttachment3.Id + ext;

                    Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + StudentAttachment3FileName);


                    studentAttachment3.Path = StudentAttachment3FileName;
                    db.SaveChanges();


                }


            }
            else
            {

                if (TotalAttachments == 1)
                {
                    var FirstElement = studentAttachments1.ElementAt(0);
                    FirstElement.Name = LessonViewModel.AttachmentName1;

                    var FileName = FirstElement.Path;

                    if (Attachment1.ContentLength > 0)
                    {
                        //   var fileName1 = Path.GetFileName(Attachment1.FileName);
                        //   FileName = fileName1;
                        //   Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        var name = Path.GetFileNameWithoutExtension(Attachment1.FileName);

                        var ext = Path.GetExtension(Attachment1.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName1 = name + "_LM_" + FirstElement.Id + ext;

                        FileName = fileName1;
                        Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);



                    }
                    FirstElement.Path = FileName;
                    db1.SaveChanges();

                    if (Attachment2.ContentLength > 0)
                    {

                        //  var fileName1 = Path.GetFileName(Attachment2.FileName);
                        // Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        AspnetStudentAttachment studentAttachment2 = new AspnetStudentAttachment();

                        studentAttachment2.Name = LessonViewModel.AttachmentName2;
                        //  studentAttachment2.Path = fileName1;
                        studentAttachment2.CreationDate = DateTime.Now;
                        studentAttachment2.LessonId = Lesson.Id;
                        db.AspnetStudentAttachments.Add(studentAttachment2);

                        db.SaveChanges();



                        var name = Path.GetFileNameWithoutExtension(Attachment2.FileName);

                        var ext = Path.GetExtension(Attachment2.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName1 = name + "_LM_" + studentAttachment2.Id + ext;


                        Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        studentAttachment2.Path = fileName1;

                        db.SaveChanges();


                    }

                    if (Attachment3.ContentLength > 0)
                    {

                        // var fileName1 = Path.GetFileName(Attachment3.FileName);
                        // Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        AspnetStudentAttachment studentAttachment3 = new AspnetStudentAttachment();

                        studentAttachment3.Name = LessonViewModel.AttachmentName3;
                        //   studentAttachment3.Path = fileName1;
                        studentAttachment3.CreationDate = DateTime.Now;
                        studentAttachment3.LessonId = Lesson.Id;
                        db.AspnetStudentAttachments.Add(studentAttachment3);
                        db.SaveChanges();




                        var name = Path.GetFileNameWithoutExtension(Attachment3.FileName);

                        var ext = Path.GetExtension(Attachment3.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);

                        var fileName1 = name + "_LM_" + studentAttachment3.Id + ext;

                        Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        studentAttachment3.Path = fileName1;
                        db.SaveChanges();


                    }

                }

                else if (TotalAttachments == 2)
                {

                    var FirstElement = studentAttachments1.ElementAt(0);
                    FirstElement.Name = LessonViewModel.AttachmentName1;

                    var FileName0 = FirstElement.Path;

                    if (Attachment1.ContentLength > 0)
                    {
                        // var fileName1 = Path.GetFileName(Attachment1.FileName);
                        // FileName0 = fileName1;
                        // Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);



                        var name = Path.GetFileNameWithoutExtension(Attachment1.FileName);

                        var ext = Path.GetExtension(Attachment1.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName1 = name + "_LM_" + FirstElement.Id + ext;
                        FileName0 = fileName1;

                        Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);



                    }

                    FirstElement.Path = FileName0;
                    db1.SaveChanges();


                    var SecondElement = studentAttachments1.ElementAt(1);
                    SecondElement.Name = LessonViewModel.AttachmentName2;

                    var FileName1 = SecondElement.Path;

                    if (Attachment2.ContentLength > 0)
                    {
                        //    var fileName2 = Path.GetFileName(Attachment2.FileName);
                        //   FileName1 = fileName2;
                        // Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName2);


                        var name = Path.GetFileNameWithoutExtension(Attachment2.FileName);

                        var ext = Path.GetExtension(Attachment2.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName2 = name + "_LM_" + SecondElement.Id + ext;

                        FileName1 = fileName2;

                        Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName2);


                    }

                    SecondElement.Path = FileName1;
                    db1.SaveChanges();



                    if (Attachment3.ContentLength > 0)
                    {

                        //var fileName1 = Path.GetFileName(Attachment3.FileName);
                        // Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);

                        AspnetStudentAttachment studentAttachment3 = new AspnetStudentAttachment();

                        studentAttachment3.Name = LessonViewModel.AttachmentName3;
                        //  studentAttachment3.Path = fileName1;
                        studentAttachment3.CreationDate = DateTime.Now;
                        studentAttachment3.LessonId = Lesson.Id;
                        db.AspnetStudentAttachments.Add(studentAttachment3);
                        db.SaveChanges();



                        var name = Path.GetFileNameWithoutExtension(Attachment3.FileName);

                        var ext = Path.GetExtension(Attachment3.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName1 = name + "_LM_" + studentAttachment3.Id + ext;


                        Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);


                        studentAttachment3.Path = fileName1;

                        db.SaveChanges();


                    }

                }

                else
                {

                    var FirstElement = studentAttachments1.ElementAt(0);
                    FirstElement.Name = LessonViewModel.AttachmentName1;

                    var FileName0 = FirstElement.Path;

                    if (Attachment1.ContentLength > 0)
                    {
                        // var fileName1 = Path.GetFileName(Attachment1.FileName);


                        //FileName0 = fileName1;
                        // Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);


                        var name = Path.GetFileNameWithoutExtension(Attachment1.FileName);

                        var ext = Path.GetExtension(Attachment1.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName1 = name + "_LM_" + FirstElement.Id + ext;
                        FileName0 = fileName1;

                        Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName1);



                    }
                    FirstElement.Path = FileName0;
                    db1.SaveChanges();


                    var SecondElement = studentAttachments1.ElementAt(1);
                    SecondElement.Name = LessonViewModel.AttachmentName2;

                    var FileName1 = SecondElement.Path;

                    if (Attachment2.ContentLength > 0)
                    {
                        // var fileName2  = Path.GetFileName(Attachment2.FileName);
                        //              FileName1 = fileName2;
                        //   Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName2);



                        var name = Path.GetFileNameWithoutExtension(Attachment2.FileName);

                        var ext = Path.GetExtension(Attachment2.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);


                        var fileName2 = name + "_LM_" + SecondElement.Id + ext;
                        FileName1 = fileName2;


                        Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName2);



                    }
                    SecondElement.Path = FileName1;
                    db1.SaveChanges();


                    var ThirdElement = studentAttachments1.ElementAt(2);
                    ThirdElement.Name = LessonViewModel.AttachmentName3;

                    var FileName2 = ThirdElement.Path;

                    if (Attachment3.ContentLength > 0)
                    {
                        //var fileName3 = Path.GetFileName(Attachment3.FileName);
                        //FileName2 = fileName3;
                        //Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName3);



                        var name = Path.GetFileNameWithoutExtension(Attachment3.FileName);

                        var ext = Path.GetExtension(Attachment3.FileName);

                        //   var fileName = Path.GetFileName(AttachmentFile.FileName);

                        var fileName3 = name + "_LM_" + ThirdElement.Id + ext;

                        FileName2 = fileName3;

                        Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName3);



                    }
                    ThirdElement.Path = FileName2;
                    db1.SaveChanges();


                }


            }



            if (LessonViewModel.LinkUrl1 != null)
            {
                AspnetStudentLink link1 = new AspnetStudentLink();

                link1.URL = LessonViewModel.LinkUrl1;
                link1.CreationDate = DateTime.Now;
                link1.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link1);
                db.SaveChanges();
            }

            if (LessonViewModel.LinkUrl2 != null)
            {
                AspnetStudentLink link2 = new AspnetStudentLink();

                link2.URL = LessonViewModel.LinkUrl2;
                link2.CreationDate = DateTime.Now;
                link2.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link2);
                db.SaveChanges();
            }


            if (LessonViewModel.LinkUrl3 != null)
            {
                AspnetStudentLink link3 = new AspnetStudentLink();

                link3.URL = LessonViewModel.LinkUrl3;
                link3.CreationDate = DateTime.Now;
                link3.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link3);
                db.SaveChanges();
            }

            if (User.IsInRole("Branch_Admin"))
            {
                return RedirectToAction("Edit", new { id = Lesson.Id });
                //return RedirectToAction("ViewLessonsToAdmin", "AspnetLessons");
            }
            else
            {
                //return RedirectToAction("Edit", new { id = Lesson.Id });
                TempData["LessonUpdate"] = "Updated";
                return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });
                //return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");
            }

        }

        [HttpGet]
        public ActionResult LessonCopy(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            if (aspnetLesson == null)
            {
                return HttpNotFound();
            }

            AspnetStudentAssignment studentAssignment = db.AspnetStudentAssignments.Where(x => x.LessonId == aspnetLesson.Id).FirstOrDefault();
            List<AspnetStudentAttachment> studentAttachments = db.AspnetStudentAttachments.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            List<AspnetStudentLink> studentLinks = db.AspnetStudentLinks.Where(x => x.LessonId == aspnetLesson.Id).ToList();
            LessonViewModel lessonViewModel = new LessonViewModel();
            lessonViewModel.LessonDescription = aspnetLesson.Description;
            lessonViewModel.LessonVideoURL = aspnetLesson.Video_Url;
            lessonViewModel.LessonName = aspnetLesson.Name;
            lessonViewModel.LessonDuration = aspnetLesson.DurationMinutes;
            lessonViewModel.MeetingLink = aspnetLesson.MeetingLink;

            if (aspnetLesson.StartDate != null && aspnetLesson.StartTime != null)
            {
                DateTime LessonStartDate = Convert.ToDateTime(aspnetLesson.StartDate);
                string StartDateOfLEsson = LessonStartDate.ToString("yyyy-MM-dd");

                ViewBag.LessonStartDate = StartDateOfLEsson;


                string StartTimeOfLEsson = aspnetLesson.StartTime.Value.TimeOfDay.ToString();


                ViewBag.LessonStartTime = StartTimeOfLEsson;

            }
            if (aspnetLesson.ContentType != null)
            {
                lessonViewModel.ContentType = aspnetLesson.ContentType;

                if (aspnetLesson.ContentType == "Image")
                {
                    ViewBag.ContentTypeImage = aspnetLesson.LessonIMG;
                }
            }
            else
            {
                lessonViewModel.ContentType = "";

            }

            //    Lesson_Session LessonSession = db.Lesson_Session.Where(x => x.LessonId == id).FirstOrDefault();

            lessonViewModel.IsActive = Convert.ToBoolean(aspnetLesson.IsActive);

            //var StartDate = Convert.ToDateTime(LessonSession.StartDate);

            //var StartDateInString = StartDate.ToString("yyyy-MM-dd");

            //ViewBag.LessonStartDate = StartDateInString;

            ////Due Date
            //var DueDate = Convert.ToDateTime(LessonSession.DueDate);

            //var DueDateInString = DueDate.ToString("yyyy-MM-dd");


            //ViewBag.LessonDueDate = DueDateInString;


            int? TopicId = aspnetLesson.TopicId;

            // ViewBag.LessonDuration = aspnetLesson.DurationMinutes;

            //int? SubjectId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().SubjectId;
            //GenericSubject Subject = db.GenericSubjects.Where(x => x.Id == SubjectId).FirstOrDefault();



            // var CourseType = Subject.SubjectType;

            lessonViewModel.Id = aspnetLesson.Id;



            if (studentAssignment != null)
            {
                lessonViewModel.AssignmentName = studentAssignment.Name;
                lessonViewModel.AssignmentDescription = studentAssignment.Description;
                DateTime Date = Convert.ToDateTime(studentAssignment.DueDate);
                string date = Date.ToString("yyyy-MM-dd");

                ViewBag.AssignmentFileName = studentAssignment.FileName;

                lessonViewModel.AssignmentDueDate = studentAssignment.DueDate;
                ViewBag.Date = date;


            }


            int count = 1;

            foreach (var link in studentLinks)
            {

                if (count == 1)
                {

                    lessonViewModel.LinkUrl1 = link.URL;

                }
                else if (count == 2)
                {

                    lessonViewModel.LinkUrl2 = link.URL;

                }
                else if (count == 3)
                {
                    lessonViewModel.LinkUrl3 = link.URL;


                }
                else
                {

                }

                count++;

            }

            count = 1;
            foreach (var attachment in studentAttachments)
            {

                if (count == 1)
                {

                    lessonViewModel.AttachmentName1 = attachment.Name;
                    ViewBag.Attachment1FileName = attachment.Path;

                }
                else if (count == 2)
                {

                    lessonViewModel.AttachmentName2 = attachment.Name;
                    ViewBag.Attachment2FileName = attachment.Path;


                }
                else if (count == 3)
                {
                    lessonViewModel.AttachmentName3 = attachment.Name;
                    ViewBag.Attachment3FileName = attachment.Path;



                }
                else
                {

                }

                count++;

            }



            int? GenericBranchClassSubjectSectionId = db.AspnetSubjectTopics.Where(x => x.Id == TopicId).FirstOrDefault().GenericBranchClassSubjectId;

            //    ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "ClassName", Subject.ClassID);
            //     ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x=>x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            // ViewBag.SubjectId = new SelectList(db.GenericSubjects.Where(x => x.SubjectType == Subject.SubjectType), "Id", "SubjectName", aspnetSubjectTopic.SubjectId);


            var GenericObject = db.AspnetGenericBranchClassSubjects.Where(x => x.Id == GenericBranchClassSubjectSectionId).FirstOrDefault();

            var ID = User.Identity.GetUserId();
            var Branches = (from branch in db.AspNetBranches
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.BranchId equals enrollment.AspNetEmployee.BranchId
                            where enrollment.AspNetEmployee.UserId == ID
                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();


            var Classes = (from classs in db.AspNetClasses
                           join branchclasssubject in db.AspnetGenericBranchClassSubjects on classs.Id equals branchclasssubject.ClassId
                           join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.ClassId equals enrollment.AspNetBranchClass_Sections.AspNetBranch_Class.AspNetClass.Id
                           where (branchclasssubject.BranchId == GenericObject.BranchId && enrollment.AspNetEmployee.UserId == ID)
                           select new
                           {
                               classs.Id,
                               classs.Name,
                           }).Distinct();


            var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID && x.AspNetBranchClass_Sections.AspNetBranch_Class.ClassId == GenericObject.ClassId).Select(x => new
            {
                Id = x.AspNetBranchClass_Sections.AspNetSection.Id,
                Name = x.AspNetBranchClass_Sections.AspNetSection.Name
            }).Distinct();

            var Subjects = (from subject in db.AspNetCourses
                            join branchclasssubject in db.AspnetGenericBranchClassSubjects on subject.Id equals branchclasssubject.SubjectId
                            join enrollment in db.AspNetTeacher_Enrollments on branchclasssubject.AspNetCours.Id equals enrollment.AspNetClass_Courses.CourseId
                            where (branchclasssubject.SectionId == GenericObject.SectionId && enrollment.AspNetEmployee.UserId == ID)
                            select new
                            {
                                subject.Id,
                                subject.Name,
                            }).Distinct();


            var Generic = db.AspnetGenericBranchClassSubjects.Where(x => x.BranchId == GenericObject.BranchId && x.ClassId == GenericObject.ClassId && x.SubjectId == GenericObject.SubjectId && x.SectionId == GenericObject.SectionId).FirstOrDefault();

            var genericTableIid = Generic.Id;

            var Topics = db.AspnetSubjectTopics.Where(x => x.GenericBranchClassSubjectId == genericTableIid).ToList().Select(x => new { x.Id, x.Name });

            //  string AllTopics = Newtonsoft.Json.JsonConvert.SerializeObject(Topics);


            ViewBag.BranchId = new SelectList(Branches, "Id", "Name", GenericObject.BranchId);
            ViewBag.ClassId = new SelectList(Classes, "Id", "Name", GenericObject.ClassId);
            ViewBag.SectionId = new SelectList(Sections, "Id", "Name", GenericObject.SectionId);
            ViewBag.SubId = new SelectList(Subjects, "Id", "Name", GenericObject.SubjectId);
            ViewBag.TopicId = new SelectList(Topics, "Id", "Name", aspnetLesson.TopicId);


            //  ViewBag.CTId = Subject.SubjectType;

            ViewBag.OrderBy = aspnetLesson.OrderBy;
            ViewBag.LessonDuration = aspnetLesson.DurationMinutes;
            return View(lessonViewModel);

        }

        [HttpPost]
        public ActionResult LessonCopy(LessonViewModel LessonViewModel)
        {


            AspnetLesson Lesson = new AspnetLesson();
            Lesson.ContentType = LessonViewModel.ContentType;
            Lesson.Name = LessonViewModel.LessonName;
            Lesson.Video_Url = LessonViewModel.LessonVideoURL;
            Lesson.TopicId = LessonViewModel.TopicId;
            Lesson.DurationMinutes = LessonViewModel.LessonDuration;
            Lesson.IsActive = true;
            Lesson.CreationDate = LessonViewModel.CreationDate;
            Lesson.Description = LessonViewModel.LessonDescription;
            Lesson.OrderBy = LessonViewModel.OrderBy;
            Lesson.CreationDate = DateTime.Now;
            Lesson.Video_Url = LessonViewModel.LessonVideoURL;
            Lesson.MeetingLink = LessonViewModel.MeetingLink;


            Lesson.Status = false;


            Lesson.StartDate = LessonViewModel.StartDate;

            DateTime startDate = DateTime.Parse(LessonViewModel.StartDate.ToString());
            DateTime start = DateTime.Parse(LessonViewModel.StartTime.ToString());
            Lesson.StartTime = startDate.Date.Add(start.TimeOfDay);
            Lesson.EndTime = start.AddMinutes(60);

            //Lesson.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();


            string EncrID = Lesson.Name + Lesson.StartTime + Lesson.Description;

            string LessonID = Encrpt.Encrypt(EncrID, true);


            var newString = Regex.Replace(LessonID, @"[^0-9a-zA-Z]+", "s");

            // Lesson.EncryptedID.Replace('/', 's').Replace('-','s').Replace('+','s').Replace('%','s').Replace('&','s');

            var stringlength = newString.Length;

            if (newString.Length > 1500)
            {


                newString = newString.Substring(0, 1500);

            }

            Lesson.EncryptedID = newString;

            // Lesson.EncryptedID.Replace('/', 's').Replace('-','s').Replace('+','s').Replace('%','s').Replace('&','s');

            //Lesson.EncryptedID = newString;
            //  db.AspnetLessons.Add(Lesson);
            // db.SaveChanges();
            HttpPostedFileBase AttachmentImage = Request.Files["AttachmentImage"];

            if (LessonViewModel.ContentType == "Image")
            {
                var fileName = Path.GetFileName(AttachmentImage.FileName) + Lesson.Id;
                AttachmentImage.SaveAs(Server.MapPath("~/Content/LessonImage/") + fileName);

                Lesson.LessonIMG = fileName;


            }

            db.AspnetLessons.Add(Lesson);
            db.SaveChanges();

            var EncryptedtoAppend = db.AspnetLessons.Where(x => x.Id == Lesson.Id).FirstOrDefault();
            EncryptedtoAppend.EncryptedID = EncryptedtoAppend.EncryptedID + EncryptedtoAppend.Id;
            db.SaveChanges();

            //Lesson_Session lessonSession = new Lesson_Session();
            //lessonSession.LessonId = Lesson.Id;
            //lessonSession.SessionId = LessonViewModel.SessionId;
            //lessonSession.StartDate = LessonViewModel.StartDate;
            //lessonSession.DueDate = LessonViewModel.DueDate;
            //db.Lesson_Session.Add(lessonSession);

            //db.SaveChanges();



            //HttpPostedFileBase Assignment = Request.Files["Assignment"];
            //HttpPostedFileBase Attachment1 = Request.Files["Attachment1"];
            //HttpPostedFileBase Attachment2 = Request.Files["Attachment2"];
            //HttpPostedFileBase Attachment3 = Request.Files["Attachment3"];



            HttpPostedFileBase Assignment = Request.Files["Assignment"];

            //    if (Assignment != null)
            // {

            if (Assignment.ContentLength > 0)
            {
                var fileName = Path.GetFileName(Assignment.FileName);
                Assignment.SaveAs(Server.MapPath("~/Content/StudentAssignments/") + fileName);
                AspnetStudentAssignment studentAssignment = new AspnetStudentAssignment();

                studentAssignment.FileName = fileName;

                studentAssignment.Name = LessonViewModel.AssignmentName;


                string DueDate = Convert.ToString(LessonViewModel.AssignmentDueDate);


                if (DueDate == "1/1/0001 12:00:00 AM")
                {
                    studentAssignment.DueDate = null;

                }
                else
                {

                    studentAssignment.DueDate = LessonViewModel.AssignmentDueDate;

                }


                studentAssignment.Description = LessonViewModel.AssignmentDescription;
                studentAssignment.CreationDate = DateTime.Now;
                studentAssignment.LessonId = Lesson.Id;

                db.AspnetStudentAssignments.Add(studentAssignment);
                db.SaveChanges();


            }
            // }

            else
            {
                AspnetStudentAssignment SA = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonViewModel.Id).FirstOrDefault();

                if (SA != null)
                {
                    AspnetStudentAssignment studentAssignment = new AspnetStudentAssignment();

                    studentAssignment.FileName = SA.FileName;
                    //studentAssignment.Name = SA.Name;
                    studentAssignment.Name = LessonViewModel.AssignmentName;
                    studentAssignment.Description = LessonViewModel.AssignmentDescription;


                    string DueDate = Convert.ToString(LessonViewModel.AssignmentDueDate);


                    if (DueDate == "1/1/0001 12:00:00 AM")
                    {
                        studentAssignment.DueDate = null;

                    }
                    else
                    {

                        studentAssignment.DueDate = LessonViewModel.AssignmentDueDate;

                    }



                    //   studentAssignment.DueDate = SA.DueDate;


                    studentAssignment.CreationDate = DateTime.Now;
                    studentAssignment.LessonId = Lesson.Id;
                    db.AspnetStudentAssignments.Add(studentAssignment);
                    db.SaveChanges();

                }

            }

            var StudentAttachmentList = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonViewModel.Id).ToList();


            if (StudentAttachmentList != null)
            {
                foreach (var attachment in StudentAttachmentList)
                {


                    AspnetStudentAttachment studentAttachment = new AspnetStudentAttachment();

                    studentAttachment.Name = attachment.Name;
                    studentAttachment.Path = attachment.Path;
                    studentAttachment.CreationDate = DateTime.Now;
                    studentAttachment.LessonId = Lesson.Id;
                    db.AspnetStudentAttachments.Add(studentAttachment);
                    db.SaveChanges();

                }


            }


            //if (Attachment1.ContentLength > 0)
            //{
            //    var fileName = Path.GetFileName(Attachment1.FileName);
            //    Attachment1.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

            //    AspnetStudentAttachment studentAttachment1 = new AspnetStudentAttachment();

            //    studentAttachment1.Name = LessonViewModel.AttachmentName1;
            //    studentAttachment1.Path = fileName;
            //    studentAttachment1.CreationDate = DateTime.Now;
            //    studentAttachment1.LessonId = Lesson.Id;
            //    db.AspnetStudentAttachments.Add(studentAttachment1);
            //    db.SaveChanges();


            //}
            //if (Attachment2.ContentLength > 0)
            //{

            //    var fileName = Path.GetFileName(Attachment2.FileName);
            //    Attachment2.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

            //    AspnetStudentAttachment studentAttachment2 = new AspnetStudentAttachment();

            //    studentAttachment2.Name = LessonViewModel.AttachmentName2;
            //    studentAttachment2.Path = fileName;
            //    studentAttachment2.CreationDate = DateTime.Now;
            //    studentAttachment2.LessonId = Lesson.Id;
            //    db.AspnetStudentAttachments.Add(studentAttachment2);

            //    db.SaveChanges();

            //}

            //if (Attachment3.ContentLength > 0)
            //{

            //    var fileName = Path.GetFileName(Attachment3.FileName);
            //    Attachment3.SaveAs(Server.MapPath("~/Content/StudentAttachments/") + fileName);

            //    AspnetStudentAttachment studentAttachment3 = new AspnetStudentAttachment();

            //    studentAttachment3.Name = LessonViewModel.AttachmentName3;
            //    studentAttachment3.Path = fileName;
            //    studentAttachment3.CreationDate = DateTime.Now;
            //    studentAttachment3.LessonId = Lesson.Id;
            //    db.AspnetStudentAttachments.Add(studentAttachment3);
            //    db.SaveChanges();

            //}

            if (LessonViewModel.LinkUrl1 != null)
            {
                AspnetStudentLink link1 = new AspnetStudentLink();

                link1.URL = LessonViewModel.LinkUrl1;
                link1.CreationDate = DateTime.Now;
                link1.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link1);
                db.SaveChanges();
            }

            if (LessonViewModel.LinkUrl2 != null)
            {
                AspnetStudentLink link2 = new AspnetStudentLink();

                link2.URL = LessonViewModel.LinkUrl2;
                link2.CreationDate = DateTime.Now;
                link2.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link2);
                db.SaveChanges();
            }


            if (LessonViewModel.LinkUrl3 != null)
            {
                AspnetStudentLink link3 = new AspnetStudentLink();

                link3.URL = LessonViewModel.LinkUrl3;
                link3.CreationDate = DateTime.Now;
                link3.LessonId = Lesson.Id;
                db.AspnetStudentLinks.Add(link3);
                db.SaveChanges();
            }


            TempData["LessonCreated"] = "Created";
            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });
            //return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");

        }

        public ActionResult DeleteLessonsBeforeAugust()
        {

            // db.AspnetLessons.Where(x=>x.CreationDate < '2020-08-01')

            DateTime date = Convert.ToDateTime("2020-08-01");


           var AllLessonIdsToDelete =  db.AspnetLessons.Where(x => x.CreationDate < date).Select(x=>x.Id).ToList();
            //16426

           // DeleteLessonFunction(16426);

            foreach (var id in AllLessonIdsToDelete)
            {


             DeleteLessonFunction(id);

            }


            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteLessons(int? id)
        {
            int? LessonId = id;

            IEnumerable<AspnetComment> CommentsToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).SelectMany(x => x.AspnetComments);
            db.AspnetComments.RemoveRange(CommentsToDelete);
            db.SaveChanges();

            List<AspnetComment_Head> ListCommentHeadToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetComment_Head.RemoveRange(ListCommentHeadToDelete);
            db.SaveChanges();

            var AssignmentToDelete = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonId).FirstOrDefault();
            if (AssignmentToDelete != null)
            {

                db.AspnetStudentAssignments.Remove(AssignmentToDelete);
                db.SaveChanges();
            }

            List<AspnetStudentAttachment> StudentAttachmentListToDelete = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentAttachments.RemoveRange(StudentAttachmentListToDelete);
            db.SaveChanges();


            List<AspnetStudentLink> StudentLinkListToDelete = db.AspnetStudentLinks.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentLinks.RemoveRange(StudentLinkListToDelete);
            db.SaveChanges();


            List<Event> AllEvents = db.Events.Where(x => x.LessonID == LessonId).ToList();

            foreach (var item in AllEvents)
            {
                var event_users = db.AspnetEvent_User.Where(x => x.eventid == item.EventID).ToList();
                db.AspnetEvent_User.RemoveRange(event_users);
                db.SaveChanges();
            }

            db.Events.RemoveRange(AllEvents);
            db.SaveChanges();

            List<AspnetStudentAssignmentSubmission> StudentAssignmentSubmissionListToDelete = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentAssignmentSubmissions.RemoveRange(StudentAssignmentSubmissionListToDelete);
            db.SaveChanges();

            List<StudentLessonTracking> StudentLessonTrackingListToDelete = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId).ToList();
            db.StudentLessonTrackings.RemoveRange(StudentLessonTrackingListToDelete);
            db.SaveChanges();

            List<Student_Quiz_Scoring> StudentQuizScoringToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).SelectMany(x => x.Student_Quiz_Scoring).ToList();
            db.Student_Quiz_Scoring.RemoveRange(StudentQuizScoringToDelete);
            db.SaveChanges();

            List<Lesson_Session> LessonSessionToDelete = db.Lesson_Session.Where(x => x.LessonId == LessonId).ToList();
            db.Lesson_Session.RemoveRange(LessonSessionToDelete);
            db.SaveChanges();

            List<Quiz_Topic_Questions> QuizTopicQuestionnsToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).SelectMany(x => x.Quiz_Topic_Questions).ToList();
            db.Quiz_Topic_Questions.RemoveRange(QuizTopicQuestionnsToDelete);
            db.SaveChanges();

            List<AspnetQuestion> QuestionListToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetQuestions.RemoveRange(QuestionListToDelete);
            db.SaveChanges();

            AspnetLesson LessonToDelete = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault();
            if (LessonToDelete != null)
            {
                db.AspnetLessons.Remove(LessonToDelete);
                db.SaveChanges();

            }

            TempData["LessonDeleted"] = "Deleted";
            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });
            // return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");

        }


        public void DeleteLessonFunction(int? id)
        {
            var dbTransaction = db.Database.BeginTransaction();

            try
            {


            int? LessonId = id;

            IEnumerable<AspnetComment> CommentsToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).SelectMany(x => x.AspnetComments);
            db.AspnetComments.RemoveRange(CommentsToDelete);
            db.SaveChanges();

            List<AspnetComment_Head> ListCommentHeadToDelete = db.AspnetComment_Head.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetComment_Head.RemoveRange(ListCommentHeadToDelete);
            db.SaveChanges();

            var AssignmentToDelete = db.AspnetStudentAssignments.Where(x => x.LessonId == LessonId).FirstOrDefault();
            if (AssignmentToDelete != null)
            {

                db.AspnetStudentAssignments.Remove(AssignmentToDelete);
                db.SaveChanges();
            }

            List<AspnetStudentAttachment> StudentAttachmentListToDelete = db.AspnetStudentAttachments.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentAttachments.RemoveRange(StudentAttachmentListToDelete);
            db.SaveChanges();


            List<AspnetStudentLink> StudentLinkListToDelete = db.AspnetStudentLinks.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentLinks.RemoveRange(StudentLinkListToDelete);
            db.SaveChanges();


            List<Event> AllEvents = db.Events.Where(x => x.LessonID == LessonId).ToList();

            foreach (var item in AllEvents)
            {
                var event_users = db.AspnetEvent_User.Where(x => x.eventid == item.EventID).ToList();
                db.AspnetEvent_User.RemoveRange(event_users);
                db.SaveChanges();
            }

            db.Events.RemoveRange(AllEvents);
            db.SaveChanges();

            List<AspnetStudentAssignmentSubmission> StudentAssignmentSubmissionListToDelete = db.AspnetStudentAssignmentSubmissions.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetStudentAssignmentSubmissions.RemoveRange(StudentAssignmentSubmissionListToDelete);
            db.SaveChanges();

            List<StudentLessonTracking> StudentLessonTrackingListToDelete = db.StudentLessonTrackings.Where(x => x.LessonId == LessonId).ToList();
            db.StudentLessonTrackings.RemoveRange(StudentLessonTrackingListToDelete);
            db.SaveChanges();

            List<Student_Quiz_Scoring> StudentQuizScoringToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).SelectMany(x => x.Student_Quiz_Scoring).ToList();
            db.Student_Quiz_Scoring.RemoveRange(StudentQuizScoringToDelete);
            db.SaveChanges();

            List<Lesson_Session> LessonSessionToDelete = db.Lesson_Session.Where(x => x.LessonId == LessonId).ToList();
            db.Lesson_Session.RemoveRange(LessonSessionToDelete);
            db.SaveChanges();

            List<Quiz_Topic_Questions> QuizTopicQuestionnsToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).SelectMany(x => x.Quiz_Topic_Questions).ToList();
            db.Quiz_Topic_Questions.RemoveRange(QuizTopicQuestionnsToDelete);
            db.SaveChanges();

            List<AspnetQuestion> QuestionListToDelete = db.AspnetQuestions.Where(x => x.LessonId == LessonId).ToList();
            db.AspnetQuestions.RemoveRange(QuestionListToDelete);
            db.SaveChanges();

            AspnetLesson LessonToDelete = db.AspnetLessons.Where(x => x.Id == LessonId).FirstOrDefault();
            if (LessonToDelete != null)
            {
                db.AspnetLessons.Remove(LessonToDelete);
                db.SaveChanges();

            }

                dbTransaction.Commit();
            }//end of try block

            catch(Exception ex )
            {
                dbTransaction.Dispose();

                var msg = ex.Message;
            }
            //TempData["LessonDeleted"] = "Deleted";
            //  return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });
            // return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics");

        }

        public ActionResult InactiveSpecificLesson()
        {

            DateTime date = Convert.ToDateTime("2021-03-28");


            var AllLessonIdsToDelete = db.AspnetLessons.Where(x => x.CreationDate > date).Select(x => x.Id).ToList();


          //  DeleteInactiveLessonsList(new List<int> { 4742 });

           // DeleteInactiveLessonsList(AllLessonIdsToDelete);




            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public void DeleteInactiveLessonsList(List<int> Ids)
        {

         //   var LessonsIds = idlist.Split(',');
            

            foreach (var lessonID in Ids)
            {
                int Id = Convert.ToInt32(lessonID);


                var EventToDelete = db.Events.Where(x => x.LessonID == Id).ToList();

                foreach (var item in EventToDelete)
                {

                    var DeleteAllEventUsers = db.AspnetEvent_User.Where(x => x.eventid == item.EventID).ToList();

                    db.AspnetEvent_User.RemoveRange(DeleteAllEventUsers);
                    db.SaveChanges();

                }


                db.Events.RemoveRange(EventToDelete);
                db.SaveChanges();

            }


            foreach (var LessonId in Ids)
            {
                int Id = Convert.ToInt32(LessonId);

                AspnetLesson LessonToUpdate = db.AspnetLessons.Where(x => x.Id == Id).FirstOrDefault();

                LessonToUpdate.Status = false;
                LessonToUpdate.IsActive = false;
                db.SaveChanges();

            }




        }

        public ActionResult InactiveLessons(string idlist)
        {


            var LessonsIds = idlist.Split(',');
            // var LessonIdsInt = LessonsIds.ToList<int>();

            foreach (var lessonID in LessonsIds)
            {
                int Id = Convert.ToInt32(lessonID);


                var EventToDelete = db.Events.Where(x => x.LessonID == Id).ToList();

                foreach (var item in EventToDelete)
                {

                    var DeleteAllEventUsers = db.AspnetEvent_User.Where(x => x.eventid == item.EventID).ToList();

                    db.AspnetEvent_User.RemoveRange(DeleteAllEventUsers);
                    db.SaveChanges();

                }


                db.Events.RemoveRange(EventToDelete);
                db.SaveChanges();

            }


            foreach (var LessonId in LessonsIds)
            {
                int Id = Convert.ToInt32(LessonId);

                AspnetLesson LessonToUpdate = db.AspnetLessons.Where(x => x.Id == Id).FirstOrDefault();

                LessonToUpdate.Status = false;
                LessonToUpdate.IsActive = false;
                db.SaveChanges();

            }


            return RedirectToAction("ViewTopicsAndLessons", "AspnetSubjectTopics", new { @NavigateTo = "Lesson" });

        }

        // GET: AspnetLessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            if (aspnetLesson == null)
            {
                return HttpNotFound();
            }
            return View(aspnetLesson);
        }

        // POST: AspnetLessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspnetLesson aspnetLesson = db.AspnetLessons.Find(id);
            db.AspnetLessons.Remove(aspnetLesson);
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
