using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;
using Facebook.Web;

namespace PitchingTube.Controllers
{
    public class HomeController : Controller
    {
         public ActionResult Index()
        {
            return View();
        }

        [FacebookAuthorize(LoginUrl = "/Account/Register")]
        public ActionResult Profile()
        {
            var client = new FacebookWebClient();

            dynamic me = client.Get("me");
            ViewBag.Name = me.name;
            ViewBag.Id = me.id;
            ViewBag.Email = me.email;
            ViewBag.Picture = me.picture;

            return View();
        }
    

        public ActionResult About()
        {
            return View();
        }
    }
}
