using DBU_Advising_Scheduler.Helpers;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBU_Advising_Scheduler.Controllers
{
    public class CalendarController : BaseController
    {
        // GET: Calendar
        [Authorize]
        public async Task<ActionResult> Index()
        {

            var graphClient = GraphHelper.GetAuthenticatedClient();

            string id = await GraphHelper.GetCoursesCalendarId();

            var events = await graphClient.Me.Calendars[id].Events.Request()
                            //.Select("subject,organizer,start,end")
                            .OrderBy("createdDateTime DESC")
                            .GetAsync();

            // Change start and end dates from UTC to local time
            foreach (var ev in events)
            {
                ev.Start.DateTime = DateTime.Parse(ev.Start.DateTime).ToLocalTime().ToString();
                ev.Start.TimeZone = TimeZoneInfo.Local.Id;
                ev.End.DateTime = DateTime.Parse(ev.End.DateTime).ToLocalTime().ToString();
                ev.End.TimeZone = TimeZoneInfo.Local.Id;
            }

            return View(events);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteEvents(FormCollection form)
        {
            string[] eventIds = String.IsNullOrEmpty(form["eventIds"].ToString()) ? null : form["eventIds"].ToString().Substring(1).Split(',');

            if (eventIds != null)
            {
                GraphServiceClient graphClient = GraphHelper.GetAuthenticatedClient();
                foreach (var id in eventIds)
                {
                    await graphClient.Me.Events[id]
                        .Request()
                        .DeleteAsync();
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> AddEvents(FormCollection form)
        {
            GraphServiceClient graphClient = GraphHelper.GetAuthenticatedClient();

            dynamic courseList = JsonConvert.DeserializeObject(form[0]);

            IList<Event> events = new List<Event>();

            foreach (var course in courseList)
            {
                // create list for days
                string[] d = course.Days.ToString().Split(' ');
                List<Microsoft.Graph.DayOfWeek> days = new List<Microsoft.Graph.DayOfWeek>();
                if (Array.Exists(d, e => e == "M"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Monday);
                }
                if (Array.Exists(d, e => e == "T"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Tuesday);
                }
                if (Array.Exists(d, e => e == "W"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Wednesday);
                }
                if (Array.Exists(d, e => e == "TH"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Thursday);
                }
                if (Array.Exists(d, e => e == "F"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Friday);
                }
                if (Array.Exists(d, e => e == "S"))
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Saturday);
                }
                if (Array.Exists(d, e => e == "Su") || d[0] == "")
                {
                    days.Add(Microsoft.Graph.DayOfWeek.Sunday);
                }

                // get dates and times separated
                string sd = course.Start_Date.ToString();
                string st = course.Start_Time.ToString() == " - " ? "00:00:00" : course.Start_Time.ToString();
                int sdMonth = Int32.Parse(sd.Split('/')[0]);
                int sdDay = Int32.Parse(sd.Split('/')[1]);
                int sdYear = Int32.Parse(sd.Split('/')[2]);

                string ed = course.End_Date.ToString();
                string et = course.End_Time.ToString() == " - " ? "00:00:00" : course.End_Time.ToString();
                int edMonth = Int32.Parse(ed.Split('/')[0]);
                int edDay = Int32.Parse(ed.Split('/')[1]);
                int edYear = Int32.Parse(ed.Split('/')[2]);

                string startDt = sd + " " + st;
                string endDt = sd + " " + et;

                // deal with empty strings
                string building = "", room = "";
                if (!String.IsNullOrEmpty(course.Building.ToString()))
                {
                    building = course.Building.ToString();
                    room = course.Room.ToString();
                }

                var @event = new Event
                {
                    Subject = course.Department.ToString() + "*" + course.Course.ToString()
                                + "*" + course.Section.ToString() + " - " + course.Title.ToString(),//"CRJS*3302*A1H1 - Juvenile Delinquency",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = course.Notes.ToString() //"CRJS 1301 or 1302 or PSYC or SOCI 1301; Classroom Aug. 10, 17 & online"
                    },
                    Start = new DateTimeTimeZone
                    {
                        DateTime = startDt, //"2019-08-10 08:00:00",
                        TimeZone = "Central Standard Time"
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = endDt, //"2019-08-10 17:00:00",
                        TimeZone = "Central Standard Time"
                    },
                    Recurrence = new PatternedRecurrence
                    {
                        Pattern = new RecurrencePattern
                        {
                            Type = RecurrencePatternType.Weekly,
                            Interval = 1,
                            DaysOfWeek = days
                        },
                        Range = new RecurrenceRange
                        {
                            Type = RecurrenceRangeType.EndDate,
                            StartDate = new Date(sdYear, sdMonth, sdDay),
                            EndDate = new Date(edYear, edMonth, edDay)
                        }
                    },
                    Location = new Location
                    {
                        DisplayName = building + " " + room //"COLN 312"
                    },
                    Attendees = new List<Attendee>()
                {
                    new Attendee
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "none",
                            Name = course.Faculty_First_Name.ToString() + " " +
                                    course.Faculty_Last_Name.ToString() //"Jean Humphreys"
                        },
                        Type = AttendeeType.Resource
                    }
                },
                    Categories = new List<String>()
                {
                    { course.Location.ToString() } //"HYBRD" }
                }
                };

                events.Add(@event);
            }

            string id = await GraphHelper.GetCoursesCalendarId();

            foreach (var ev in events)
            {
                await graphClient.Me.Calendars[id].Events
                    .Request()
                    .AddAsync(ev);
            }
            return RedirectToAction("Index", "Courses", new { area = "" });
        }
    }
}