using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Web.Mvc;
using DBU_Advising_Scheduler.TokenStorage;


namespace DBU_Advising_Scheduler.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var tokenStore = new SessionTokenStore(null,
            //        System.Web.HttpContext.Current, ClaimsPrincipal.Current);

            //ViewBag.Message = tokenStore.GetUserDetails();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error(string message, string debug)
        {
            Flash(message, debug);
            return RedirectToAction("Index");
        }
    }
}