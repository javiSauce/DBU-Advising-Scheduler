using DBU_Advising_Scheduler.Helpers;
using Microsoft.Graph;
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
            var events = await GraphHelper.GetEventsAsync();

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
        public async Task<ActionResult> AddEvents()
        {
            var @event = new Event
            {
                Subject = "Let's go for lunch",
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = "Does noon time work for you?"
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = "2017-09-04T12:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                End = new DateTimeTimeZone
                {
                    DateTime = "2017-09-04T14:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                Recurrence = new PatternedRecurrence
                {
                    Pattern = new RecurrencePattern
                    {
                        Type = RecurrencePatternType.Weekly,
                        Interval = 1,
                        DaysOfWeek = new List<Microsoft.Graph.DayOfWeek>()
                        {
                            Microsoft.Graph.DayOfWeek.Monday
                        }
                    },
                    Range = new RecurrenceRange
                    {
                        Type = RecurrenceRangeType.EndDate,
                        StartDate = new Date(2017, 9, 4),
                        EndDate = new Date(2017, 12, 31)
                    }
                },
                Location = new Location
                {
                    DisplayName = "Harry's Bar"
                }
            };

            IList<Event> events = new List<Event>();
            events.Add(@event);

            GraphServiceClient graphClient = GraphHelper.GetAuthenticatedClient();

            foreach (var ev in events)
            {
                await graphClient.Me.Events
                    .Request()
                    .AddAsync(ev);
            }

            return RedirectToAction("Index","Courses",new { area = "" });
        }
    }
}