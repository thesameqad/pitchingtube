using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PitchingTube.Data;
using PitchingTube.Models;



namespace PitchingTube.Controllers
{
    [Authorize]
    public class TubeController : Controller
    {
        //
        // GET: /Tube/

        private ParticipantRepository participantRepository = new ParticipantRepository();
        private PartnerRepository partnerRepository = new PartnerRepository();
        private PersonRepository personRepository = new PersonRepository();

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
            Participant currentParticipant = participantRepository.FindPartner(userId, tubeId/*(int)Session["currentTube"]*/, roundNumber);

            ParticipantRepository.UserInfo partnerModel = new ParticipantRepository.UserInfo
            {
                UserId = currentParticipant.UserId,
                Name = currentParticipant.aspnet_Users.UserName,
                Description = currentParticipant.Description,
                AvatarPath = personRepository.FirstOrDefault(x => x.UserId == currentParticipant.UserId).AvatarPath,
                Role = User.IsInRole("Entrepreneur") ? "Investor" : "Entrepreneur"//currentParticipant.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName
            };
          
            List<UserInfo> currentPairsModel = Util.ConverUserDataListToUserModelList(participantRepository.FindCurrentPairs(tubeId, roundNumber));
            
            partnerRepository.Insert(new Partner
            {
                UserId = userId,
                PartnerId = currentParticipant.UserId
            });

            //ViewBag setup 
            ViewBag.History = Util.ConverUserDataListToUserModelList(partnerRepository.History(userId));

            ViewBag.CurrentPairs = currentPairsModel;

            ViewBag.CurrentPartner = partnerModel; 

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
