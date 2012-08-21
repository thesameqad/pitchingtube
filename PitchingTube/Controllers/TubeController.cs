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
        public ActionResult ShareContacts(int tubeId)
        {
            var role = Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault();        
            if (role != "Investor")
            {
                return null;
            }
            {
                var repository = new BaseRepository<Person>();
                Guid Userid = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;
                var PartnerId = participantRepository.FindPartner(Userid, tubeId, (int)participantRepository.UserIsInTube(Userid).Mode);
                var model = repository.Query(x => x.UserId == Userid).FirstOrDefault();

                ViewBag.Email = PartnerId.aspnet_Users.aspnet_Membership.Email;
                
                return View(model);
            }

        }

        [HttpGet]
        public ActionResult StartPitch()
        {
            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;

            var tube = participantRepository.UserIsInTube(userId);
            int tubeId = tube.TubeId;

            tube.TubeMode += 1;

            //BaseRepository<Tube> tubeRepository = new BaseRepository<Tube>();
            //tubeRepository.Update(tube);
            //new BaseRepository<Tube>().Update(tube);

            int roundNumber = (int)tube.TubeMode;

            if (tube.TubeMode == TubeMode.Nominations)
                return RedirectToAction("Nomination", new { tubeId = tubeId });

            //just a showcase. Will be removed in the future
            ViewBag.CurrentPartnerId = participantRepository.FindPartner(userId, tubeId/*(int)Session["currentTube"]*/, roundNumber);

            List<ParticipantRepository.UserInfo> model = participantRepository.FindCurrentPairs(tubeId, roundNumber);

            BaseRepository<Partner> partner = new BaseRepository<Partner>();
            partner.Insert(new Partner()
            {
                UserId = userId,
                PartnerId = ViewBag.CurrentPartnerId.UserId
            });
            ViewBag.History = History(userId);
            return View(model);

        }

        [HttpGet]
        public ActionResult History(Guid UserId)
        {
            var repository = new PartnerRepository();
            var model = repository.History(UserId);
            return View();
        }

        [HttpGet]
        public ActionResult Nomination(int tubeId)
        {
            
            var role = Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault();
            if (role != "Investor")
            {
                return null;
            }
            else
            {
                var repository = new ParticipantRepository();
                var repositoryP = new BaseRepository<Person>();
                var model = repository.Query(x => x.TubeId == tubeId && x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur")
                    .Select(x => new ParticipantRepository.UserInfo() { 
                        UserId = x.UserId, 
                        Name = x.aspnet_Users.UserName, 
                        Description = x.Description,
                        AvatarPath = repositoryP.FirstOrDefault(y => y.UserId == x.UserId).AvatarPath.Replace("\\", "/"),
                        Role = x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName
                    }
                    );
                ViewData["tubeId"] = tubeId;
                return View(model);
            }
           
        }
        [HttpPost]
        public ActionResult Nomination(IEnumerable<Guid> Id, IEnumerable<int> Rating, int tubeId)
        {
            var repository = new BaseRepository<Nomination>();
            for (var i = 0; i < Id.Count(); i++)
            {

                repository.Insert(new Nomination()
                {
                    TubeId = tubeId,
                    InvestorId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey,
                    EnterepreneurId = Id.ElementAt(i),
                    Rating = Rating.ElementAt(i),
                    Panding = Convert.ToInt32(true)
                });
            }
            
                return null;
        }
    }
}
