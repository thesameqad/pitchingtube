using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;
using Facebook.Web;
using PitchingTube.Data;
using System.Web.Security;

namespace PitchingTube.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ParticipantRepository participantRepository = new ParticipantRepository();
        [Authorize]
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
                ViewBag.Message = "DESCRIBE YOURSELF IN 3 WORDS";
                ViewBag.DefaulValue = "making investments";
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
            int tubeId = FindTube();
            Guid userId = GetCurrentUserId();
            participantRepository.Insert(new Participant
            {
                TubeId = tubeId,
                UserId = userId,
                Description = description
            });
            return RedirectToAction("Index", "Tube", new {tubeId = tubeId });
        }
        
    

        public ActionResult About()
        {
            return View();
        }

        private int FindTube()
        {
            string userRole = User.IsInRole("Investor") ? "Investor" : "Entrepreneur";
            participantRepository.FindBestMatchingTube(userRole);
            return 1;
        }

        private Guid GetCurrentUserId()
        {
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            return Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString()); 
        }
    }
}
