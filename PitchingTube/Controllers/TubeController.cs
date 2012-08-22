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
            var model = participantRepository.TubeParticipants(tubeId);
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
                Guid Userid = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;
                var PartnerId = participantRepository.FindPartner(Userid, tubeId, (int)participantRepository.UserIsInTube(Userid).Mode);
                var model = personRepository.Query(x => x.UserId == Userid).FirstOrDefault();

                ViewBag.Email = PartnerId.aspnet_Users.aspnet_Membership.Email;

                return View(model);
            }

        }

        [HttpGet]
        public ActionResult StartPitch()
        {
            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;

            var tube = participantRepository.UserIsInTube(userId);

            var repository = new BaseRepository<Tube>();
            var entity = repository.Query(x => x.TubeId == tube.TubeId).FirstOrDefault();
            entity.TubeMode += 1;
            repository.Update(entity);

            int roundNumber = (int)entity.TubeMode;

            if (entity.TubeMode >= TubeMode.Nominations)
            {
                if(User.IsInRole("Investor"))
                    return RedirectToAction("Nomination", new { tubeId = tube.TubeId });
                else
                    return RedirectToAction("Results", new { tubeId = tube.TubeId });
            }

            //just a showcase. Will be removed in the future
            Participant currentParticipant = participantRepository.FindPartner(userId, tube.TubeId, roundNumber);

            UserInfo partnerModel = new UserInfo
            {
                UserId = currentParticipant.UserId,
                Name = currentParticipant.aspnet_Users.UserName,
                Description = currentParticipant.Description,
                AvatarPath = personRepository.FirstOrDefault(x => x.UserId == currentParticipant.UserId).AvatarPath,
                Role = User.IsInRole("Entrepreneur") ? "Investor" : "Entrepreneur"//currentParticipant.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName
            };

            List<UserInfo> currentPairsModel = Util.ConverUserDataListToUserModelList(participantRepository.FindCurrentPairs(userId,currentParticipant.UserId, tube.TubeId, roundNumber));

            partnerRepository.Insert(new Partner()
            {
                UserId = userId,
                PartnerId = currentParticipant.UserId
            });

            //ViewBag setup 
            ViewBag.History = Util.ConverUserDataListToUserModelList(partnerRepository.History(userId, currentParticipant.UserId));

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
                return RedirectToAction("Results", new { tubeId = tubeId});
            }
            else
            {
                var model = participantRepository.Query(x => x.TubeId == tubeId && x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur")
                    .Select(x => new ParticipantRepository.UserInfo()
                    {
                        UserId = x.UserId,
                        Name = x.aspnet_Users.UserName,
                        Description = x.Description,
                        AvatarPath = personRepository.FirstOrDefault(y => y.UserId == x.UserId).AvatarPath.Replace("\\", "/"),
                        Role = x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName
                    }
                    );
                ViewData["tubeId"] = tubeId;
                return View(model);
            }

        }
        [HttpPost]
        public ActionResult Nomination(IEnumerable<string> ids, IEnumerable<string> ratings, int tubeId)
        {
            var repository = new BaseRepository<Nomination>();
            for (var i = 0; i < ids.Count(); i++)
            {
                int rating = 0;
                int.TryParse(ratings.ElementAt(i), out rating);
                repository.Insert(new Nomination()
                {
                    TubeId = tubeId,
                    InvestorId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey,
                    EnterepreneurId = Guid.Parse(ids.ElementAt(i)),
                    Rating = rating,
                    Panding = Convert.ToInt32(true)
                });
            }

            return RedirectToAction("Results", new { tubeId = tubeId});
        }

        public ActionResult Results(int tubeId)
        {
            var results = Util.ConverUserDataListToUserModelList(participantRepository.GetResult(tubeId));
            ViewBag.TubeId = tubeId;
            return View(results);
        }

        public JsonResult GetRatings(int tubeId)
        {
            return new JsonResult
                       {
                           Data = participantRepository.getNominationAndPendingStatus(tubeId),
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                       };

        }
    }
}
