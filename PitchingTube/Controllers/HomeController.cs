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
    public class HomeController : Controller
    {
        private BaseRepository<Participant> participantRepository = new BaseRepository<Participant>();
        public ActionResult Index()
        {
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
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            Guid userId = Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString()); 
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
            return 1;
        }
    }
}
