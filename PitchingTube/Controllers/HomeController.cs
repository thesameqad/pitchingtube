using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;
using Facebook.Web;
using PitchingTube.Data;
using System.Web.Security;
using System.Threading;
using System.Configuration;
using PitchingTube.Models;

namespace PitchingTube.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {

        [TubeRedirection]
        public ActionResult Index()
        {
            ViewBag.Pay = "yes";
            //Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault()

            Guid userId = GetCurrentUserId();
            HttpContext.Cache[userId.ToString()] = true;

            //if (User.IsInRole("Investor"))
            if (Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault() == "Investor")
            {
                
                if (!(personRepository.GetPay(userId) ?? false))
                {
                    ViewBag.Pay = "no";
                }
                else
                {
                    ViewBag.Message = "DESCRIBE YOURSELF IN 3 WORDS";
                    ViewBag.DefaulValue = "making investments";
                }


            }
            else
            {
                ViewBag.Message = "DESCRIBE YOUR IDEA IN 3 WORDS";
                ViewBag.DefaulValue = "website for puppies";
            }
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(string description)
        {
            return RedirectToAction("FindTube", "Tube", new { description = description});
        }

        public ActionResult About()
        {
            return View();
        }

        [NoCacheAttribute]
        public JsonResult TubeFinded()
        {
            Thread.Sleep(3000);
            return Json(new {tubeId = FindTube()},JsonRequestBehavior.AllowGet);
        }

        private int FindTube()
        {
            //I don't know why, but it is not work. It is always return "Entrepreneur"
            //string userRole = User.IsInRole("Investor") ? "Investor" : "Entrepreneur";



            string userRole = Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault() ;

            bool autoFillingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoFillingEnabled"]);
            var tubeId = participantRepository.FindBestMatchingTube(userRole, autoFillingEnabled);
            return tubeId;
        }

        public ActionResult AccountPlan()
        {
            //set trial
            personRepository.SetPay(GetCurrentUserId(), true);
            
            return View();
        }
    }
}
