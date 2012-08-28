using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using PitchingTube.Data;
using PitchingTube.Models;
using OpenTok;
using System.Collections.Specialized;
using System.Configuration;




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
        private TubeRepository tubeRepository = new TubeRepository();
        private DateTime minDateTime = new DateTime(1753, 1, 1, 0, 0, 0);


        public ActionResult Index(int tubeId, string description)
        {
            Guid userId = GetCurrentUserId();
            participantRepository.Insert(new Participant
            {
                TubeId = tubeId,
                UserId = userId,
                Description = description
            });
            ViewBag.TubeId = tubeId;

           // Session["leftTime"] = 0;
            HttpContext.Application[tubeId.ToString()] = 0;

            return View();
        }

        [HttpGet]
        public ActionResult TubePeopleList(int tubeId)
        {
            var model = participantRepository.TubeParticipants(tubeId);
            var leftInvestor = 5 - model.Count(x => x.Role == "Investor");
            var leftEntrepreneur = 5 - model.Count(x => x.Role == "Entrepreneur");
            return new JsonResult { Data = new { model, leftInvestor, leftEntrepreneur }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult ShareContacts(int tubeId)
        {
            var role = Roles.GetRolesForUser(Membership.GetUserNameByEmail(User.Identity.Name)).FirstOrDefault();
            if (role != "Investor")
            {
                return null;
            }
            var userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;
            var partnerId = participantRepository.FindPartner(userId, tubeId, (int)participantRepository.UserIsInTube(userId).Mode);
            var model = personRepository.Query(x => x.UserId == userId).FirstOrDefault();

            // ViewBag.Email = PartnerId.aspnet_Users.aspnet_Membership.Email;
            var history = partnerRepository.FirstOrDefault(x => x.UserId == userId && x.PartnerId == partnerId.UserId);
            history.Contacts = "true";
            partnerRepository.Update(history);

            return Json(new { Email = partnerId.aspnet_Users.aspnet_Membership.Email, Skype = model.Skype, Phone = model.Phone }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult StartPitch()
        {

            OpenTokSDK opentok = new OpenTokSDK();
            Dictionary<string, object> options = new Dictionary<string, object>();
            //options.Add(SessionPropertyConstants.MULTIPLEXER_SWITCHTYPE, "enabled");
            string sessionId = opentok.CreateSession(Request.ServerVariables["REMOTE_ADDR"]);


            NameValueCollection appSettings = ConfigurationManager.AppSettings;


            ViewData["apiKey"] = appSettings["opentok_key"];
            ViewData["sessionId"] = sessionId;



            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;

            var tube = participantRepository.UserIsInTube(userId);

            int countPairs = participantRepository.CountPairsInTube(tube.TubeId);

            if ((int)HttpContext.Application[tube.TubeId.ToString()] == countPairs * 2)
                HttpContext.Application[tube.TubeId.ToString()] = 0;
            
            var entity = tubeRepository.Query(x => x.TubeId == tube.TubeId).FirstOrDefault();
            
            //----stub---
            if (entity.TubeMode == TubeMode.Opened && countPairs != 5)
                participantRepository.DeleteFromTubeSingleUsers(tube.TubeId);
            //-----------
            
            if((int)HttpContext.Application[tube.TubeId.ToString()] == 0)
            {
                if ((int)entity.TubeMode == countPairs)
                    entity.TubeMode = TubeMode.Nominations;
                else
                    entity.TubeMode += 1;

                tubeRepository.Update(entity);
            }
            HttpContext.Application[tube.TubeId.ToString()] = (int) HttpContext.Application[tube.TubeId.ToString()] + 1;


            if (!participantRepository.IsCanFindPartner(userId, tube.TubeId))
                return RedirectToAction("TubeExcluded", "Tube", new {tubeId = tube.TubeId});

            int roundNumber = (int)entity.TubeMode;

            if (entity.TubeMode >= TubeMode.Nominations)
            {
                if(User.IsInRole("Investor"))
                {
                    entity.TubeMode = TubeMode.Nominations;
                    return RedirectToAction("Nomination", new { tubeId = tube.TubeId });
                }
                else
                    return RedirectToAction("Results", new { tubeId = tube.TubeId });
            }

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

            
            partnerRepository.Insert(new Partner
                                            {
                                                UserId = userId,
                                                PartnerId = currentParticipant.UserId,
                                            });
            

           
            //ViewBag setup 
            ViewBag.History = Util.ConverUserDataListToUserModelList(partnerRepository.History(userId, currentParticipant.UserId, tube.TubeId));

            ViewBag.CurrentPairs = currentPairsModel;

            ViewBag.CurrentPartner = partnerModel;

            ViewBag.TubeId = tube.TubeId;

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
                var model =
                    Util.ConverUserDataListToUserModelList(participantRepository.Query(
                        x =>
                        x.TubeId == tubeId && x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur")
                                                               .Select(x => new ParticipantRepository.UserInfo()
                                                                                {
                                                                                    UserId = x.UserId,
                                                                                    Name = x.aspnet_Users.UserName,
                                                                                    Description = x.Description,
                                                                                    AvatarPath =
                                                                                        personRepository.FirstOrDefault(
                                                                                            y => y.UserId == x.UserId).
                                                                                        AvatarPath.Replace("\\", "/"),
                                                                                    Role =
                                                                                        x.aspnet_Users.aspnet_Roles.
                                                                                        FirstOrDefault().RoleName
                                                                                }
                                                               ).ToList()
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

        public ActionResult FindTube(string description)
        {
            ViewBag.Description = description;
            return View();
        }

        private Guid GetCurrentUserId()
        {
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            return Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString());
        }

        public JsonResult GetTubeTimeout(int tubeId)
        {
            return Json(new { timeOut = tubeRepository.GetTubeTimeout(tubeId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TubeExcluded(int tubeId)
        {
            var userId = GetCurrentUserId();
            var participant = participantRepository.FirstOrDefault(p => p.UserId == userId && p.TubeId == tubeId);
            ViewBag.Description = participant.Description;
            participantRepository.Delete(participant);

            return View();
        }

        public JsonResult GetCurrentTimePitch(Guid partnerId)
        {
            Guid userId = GetCurrentUserId();
            return Json(new { leftTime = partnerRepository.GetLeftTimePitch(userId, partnerId, minDateTime)}, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetCurrentTimePitch()
        //{
        //    Session["leftTime"] = (int) Session["leftTime"] - 1;
        //    return Json(new { leftTime = (int) Session["leftTime"] }, JsonRequestBehavior.AllowGet);
        //    //return Json(new {leftTime = partnerRepository.GetLeftTimePitch(userId, partnerId) - delay}, JsonRequestBehavior.AllowGet);
        //}


        [HttpGet]
        public void SetStartPitchTime(Guid partnerId)
        {
            Guid userId = GetCurrentUserId();

            Partner updatePartner = partnerRepository.FirstOrDefault(p => (p.UserId == userId && p.PartnerId == partnerId) || (p.UserId == partnerId && p.PartnerId == userId));

            if (updatePartner.BeginPitchTime == null)
            {
                updatePartner.BeginPitchTime = minDateTime;
                partnerRepository.Update(updatePartner);
            }
            else
                if (updatePartner.BeginPitchTime == minDateTime)
                {
                    updatePartner.BeginPitchTime = DateTime.Now;
                    partnerRepository.Update(updatePartner);
                }

        }

        public ActionResult IsPatrtnerOnline(Guid partnerId)
        {
            MembershipUser partner = Membership.GetUser(partnerId);
            return Json(new {isOnline = partner.IsOnline}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsSecondReady(Guid partnerId)
        {
            Guid userId = GetCurrentUserId();

            Partner pair = partnerRepository.FirstOrDefault(p => (p.UserId == userId && p.PartnerId == partnerId) || (p.UserId == partnerId && p.PartnerId == userId));

            return Json(new {isReady = pair.BeginPitchTime != null && pair.BeginPitchTime != minDateTime},
                        JsonRequestBehavior.AllowGet);
        }


    }
}
