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
using System.Web.Hosting;

namespace DBU_Advising_Scheduler.Controllers
{
    public class DegreePlanController : BaseController
    {

        //public void getJsonData()
        //{

        //    Degrees degrees = JsonConvert.DeserializeObject<Degrees>(System.IO.File.ReadAllText(@"degrees.json"));
            
        //    foreach (char degree in degrees.CollegeOfBusiness)
        //    {
        //        Console.WriteLine(degree);
        //    }

        //}

        // GET: DegreePlan
        public ActionResult Index()
        {
            string fileName = HostingEnvironment.MapPath(@"~\degrees.json");
            Degrees degrees = JsonConvert.DeserializeObject<Degrees>(System.IO.File.ReadAllText(fileName));

            //foreach (string degree in degrees.CollegeOfBusiness)
            //{
            //    System.Diagnostics.Debug.WriteLine(degree);
            //}
            return View(degrees);
        }
    }
}