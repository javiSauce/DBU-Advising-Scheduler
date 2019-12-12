using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using DBU_Advising_Scheduler.Models;
using DBU_Advising_Scheduler.Helpers;
using System.Web.Hosting;
using System.Threading.Tasks;
using DBU_Advising_Scheduler.TokenStorage;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;

namespace DBU_Advising_Scheduler.Controllers
{


    public class DegreePlanController : BaseController
    {
        private DBU_Advising_Scheduler.Models.dbuasEntities2 db = new DBU_Advising_Scheduler.Models.dbuasEntities2();



        // GET: DegreePlan
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Degree()
        //{
        //    //var degreePlan = new DegreeScraper();
        //    //var result = await degreePlan.GetDegreeTable("BS Philosophy");
        //    //var degreePlan = DegreeScraper.GetDegreeTable("BS Philosophy");
        //    //var result =  await DegreeScraper.GetDegreeTable("BS Philosophy");

        //    //ViewBag.DegreePlan = result;

        //    var result  = GetTable("BS Philosophy").Result;
        //    return Content(result);
        //}

        public async Task<ActionResult> Degree(string args)
        {
            var plan = new DegreeScraper();
            var result = await plan.GetDegreeTable(args);
            ViewBag.Plan = result;
            return View();
        }

        public ActionResult SideMenu()
        {
            string fileName = HostingEnvironment.MapPath(@"~\degrees.json");
            Degrees degrees = JsonConvert.DeserializeObject<Degrees>(System.IO.File.ReadAllText(fileName));
            return PartialView(degrees);
        }

        public ActionResult CoursesCompleted(string entry)
        {
            if (Request.IsAuthenticated)
            {
                // Get the user's token cache
                var tokenStore = new SessionTokenStore(null,
                    System.Web.HttpContext.Current, ClaimsPrincipal.Current);

                if (tokenStore.HasData())
                {
                    // Add the user to the view bag
                    var info = tokenStore.GetUserDetails();

                    var insert = db.Set<completedcours>();
                    insert.Add(new completedcours { DisplayName = info.DisplayName, Email = info.Email, Courses = entry });
                    db.SaveChanges();
                }
            }

            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Calendar");

        }
    }
}