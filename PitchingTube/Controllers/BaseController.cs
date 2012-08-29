using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PitchingTube.Data;
using System.Web.Security;

namespace PitchingTube.Controllers
{
    public class BaseController : Controller
    {
        protected ParticipantRepository participantRepository = new ParticipantRepository();
        protected PartnerRepository partnerRepository = new PartnerRepository();
        protected PersonRepository personRepository = new PersonRepository();
        protected TubeRepository tubeRepository = new TubeRepository();


        protected Guid GetCurrentUserId()
        {
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            return Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString()); 
        }

        public ActionResult RedirectToTubeMode()
        {
            var tube = participantRepository.UserIsInTube(GetCurrentUserId());

            if (tube != null)
            {
                if (tube.TubeMode == TubeMode.Opened)
                {
                    return RedirectToAction("Index", "Tube", new { tube.TubeId });
                }
                else if (tube.TubeMode == TubeMode.FirstPitch || tube.TubeMode == TubeMode.SecondPitch || tube.TubeMode == TubeMode.ThirdPitch || tube.TubeMode == TubeMode.FourthPitch || tube.TubeMode == TubeMode.FifthPitch)
                {
                    return RedirectToAction("StartPitch", "Tube", new { tube.TubeId });
                }
                else if (tube.TubeMode == TubeMode.Nominations)
                {
                    return RedirectToAction("Results", "Tube", new { tube.TubeId });
                }

                Session["currentTube"] = tube;
            }
            return null;
        }

    }
}
