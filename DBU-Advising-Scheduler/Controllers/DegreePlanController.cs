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

namespace DBU_Advising_Scheduler.Controllers
{
    public class DegreePlanController : BaseController
    {
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

        public async Task<ActionResult> Degree()
        {
            var plan = new DegreeScraper();
            var result = await plan.GetDegreeTable("BS Philosophy");

            return Content(result);
        }

        public ActionResult SideMenu()
        {
            string fileName = HostingEnvironment.MapPath(@"~\degrees.json");
            Degrees degrees = JsonConvert.DeserializeObject<Degrees>(System.IO.File.ReadAllText(fileName));
            return PartialView(degrees);
        }
    }
}