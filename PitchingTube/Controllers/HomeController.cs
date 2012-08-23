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

namespace PitchingTube.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ParticipantRepository participantRepository = new ParticipantRepository();

        public ActionResult Index()
        {
            var tube = participantRepository.UserIsInTube(GetCurrentUserId());

            if (tube != null)
            {
                switch (tube.TubeMode)
                {
                    case TubeMode.Opened:
                        return RedirectToAction("Index", "Tube", new { tube.TubeId });
                    case TubeMode.FirstPitch:
                        //Redirect to FirstPitch page
                        break;
                    case TubeMode.Closed:
                        break;
                    default:
                        break;
                }

                Session["currentTube"] = tube;
            }

            if (User.IsInRole("Investor"))
            {
                Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;
                if (participantRepository.GetPay(userId) == "no")
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
            var tubeId = participantRepository.FindBestMatchingTube(userRole);
            return tubeId;
        }

        private Guid GetCurrentUserId()
        {
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            return Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString()); 
        }
    }
}
