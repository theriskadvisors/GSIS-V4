using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;
using Microsoft.AspNet.Identity;

namespace SEA_Application.Controllers
{
    public class CalendarController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetEvents()
        {           
                var id = User.Identity.GetUserId();

            //var events = from x in db.Events
            //             where x.UserId == id
            //             select new { _id = x.EventID, description = x.Description, end = x.End, allDay = x.IsFullDay, textColor = "#ffffff", title = x.Subject, backgroundColor = x.ThemeColor, start = TimeZoneInfo.ConvertTimeFromUtc(x.Start, TimeZoneInfo.Local), x.IsPublic, instructor = x.AspNetUser.Name, subjectClass = x.SubjectClass, x.Url, type = "Appointment", calendar = "Sales", LessonName = x.AspnetLesson.Name, className = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name, Section = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name };
            var events = db.Events.Where(x => x.UserId == id).Select(x => new
            { SecTitle = x.Sec_Title, _id = x.EventID, description = x.Description1, end = x.End, allDay = x.IsFullDay, textColor = "#ffffff", title = x.Subject1, backgroundColor = x.ThemeColor, start = x.Start, x.IsPublic, instructor = x.AspNetUser.Name, subjectClass = x.SubjectClass, x.Url, type = "Appointment", calendar = "Sales", LessonName = x.AspnetLesson.Name, className = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name, Section = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name }).ToList();

            var eventList = events.Select(x => new { SecTitle = x.SecTitle, title = x.title,  Section = x.Section, className = x.className, LessonName = x.LessonName, calendar = x.calendar, Url = x.Url, subjectClass = x.subjectClass, instructor = x.instructor, _id = x._id, description = x.description, end = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(x.end.Value , TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")),TimeZoneInfo.Local) , allDay = x.allDay, textColor = x.textColor, backgroundColor = x.backgroundColor, start = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(x.start,TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time")), TimeZoneInfo.Local), IsPublic = x.IsPublic, });

            if (User.IsInRole("Student"))
            {
                var eventstimatableList = new List<eventstimatable>();
                TimeZoneInfo PK_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, PK_ZONE);
                //today = today.AddHours(15);
                foreach (var item in eventList)
                {
                    var eventstimatable = new eventstimatable();
                    eventstimatable.title = item.title;
                    eventstimatable.SecTitle = item.SecTitle;
                    eventstimatable.Section = item.Section;
                    eventstimatable.className = item.className;
                    eventstimatable.LessonName = item.LessonName;
                    eventstimatable.calendar = item.calendar;
                    if(item.start.Date <= today.Date)
                    { eventstimatable.Url = item.Url; }
                    else
                    { eventstimatable.Url = "#"; }
                    eventstimatable.subjectClass = item.subjectClass;
                    eventstimatable.instructor = item.instructor;
                    eventstimatable._id = item._id;
                    eventstimatable.description = item.description;
                    eventstimatable.end = item.end;
                    eventstimatable.allDay = item.allDay;
                    eventstimatable.textColor = item.textColor;
                    eventstimatable.backgroundColor = item.backgroundColor;
                    eventstimatable.start = item.start;
                    eventstimatable.IsPublic = item.IsPublic;
                    eventstimatableList.Add(eventstimatable);
                }
                return new JsonResult { Data = eventstimatableList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = eventList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };   
        }

        public class eventstimatable
        {
            public string title { get; set; }
            public string SecTitle { get; set; }
            public string Section { get; set; }
            public string className { get; set; }
            public string LessonName { get; set; }
            public string calendar { get; set; }
            public string Url { get; set; }
            public string subjectClass { get; set; }
            public string instructor { get; set; }
            public int _id { get; set; }
            public string description { get; set; }
            public string textColor { get; set; }
            public string backgroundColor { get; set; }
            public bool IsPublic { get; set; }
            public bool allDay { get; set; }
            public DateTime end { get; set; }
            public DateTime start { get; set; }

        }
    }
}

//{ _id = x.EventID, description = x.Description, end = x.End , allDay = x.IsFullDay, textColor = "#ffffff", title = x.Subject, backgroundColor = x.ThemeColor, start = TimeZoneInfo.ConvertTimeFromUtc(x.Start, TimeZoneInfo.Local), x.IsPublic, instructor = x.AspNetUser.Name, subjectClass = x.SubjectClass ,x.Url, type = "Appointment", calendar = "Sales",LessonName = x.AspnetLesson.Name, className = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetClass.Name, Section = x.AspnetLesson.AspnetSubjectTopic.AspnetGenericBranchClassSubject.AspNetSection.Name }).ToList();