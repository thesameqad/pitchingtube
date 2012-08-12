using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PitchingTube.Data;


namespace PitchingTube.Controllers
{
    [Authorize]
    public class TubeController : Controller
    {
        //
        // GET: /Tube/

        private ParticipantRepository participantRepository = new ParticipantRepository();

        public ActionResult Index(int tubeId)
        {
            ViewBag.TubeId = tubeId;
            return View();
        }

        [HttpGet]
        public ActionResult TubePeopleList(int tubeId)
        {
            var repository = new ParticipantRepository();
            var model = repository.TubeParticipants(tubeId);
            var leftInvestor = 5 - model.Count(x => x.Role == "Investor");
            var leftEntrepreneur = 5 - model.Count(x => x.Role == "Entrepreneur");
            return new JsonResult() { Data = new { model, leftInvestor, leftEntrepreneur }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpGet]
        public ActionResult StartPitch()
        {
            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;
            //just a showcase. Will be removed in the future
            ViewBag.CurrentPartnerId = participantRepository.FindPartner(userId, (int)Session["currentTube"], 0);

            return View();

        }
    }
}
