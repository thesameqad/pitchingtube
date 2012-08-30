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
using System.Web.Caching;




namespace PitchingTube.Controllers
{
    [Authorize]
    public class TubeController : BaseController
    {
        // variables
        private DateTime minDateTime = new DateTime(1753, 1, 1, 0, 0, 0);

        //Sync Actions

        [TubeRedirection]
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

        [HttpGet,TubeRedirection]
        public ActionResult StartPitch()
        {
            //options.Add(SessionPropertyConstants.MULTIPLEXER_SWITCHTYPE, "enabled");

            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;

            var tube = Session["currentTube"] as Tube;

            tube = tubeRepository.FirstOrDefault(t => t.TubeId == tube.TubeId);


            if (!participantRepository.IsCanFindPartner(userId, tube.TubeId))
                return RedirectToAction("TubeExcluded", "Tube", new { tubeId = tube.TubeId });

            int countPairs = participantRepository.CountPairsInTube(tube.TubeId);

            //Tube tubeWasChanged = HttpContext.Cache.Get("TubeChanged") == null ? null : (Tube)HttpContext.Cache["TubeChanged"];

            //if (tubeWasChanged == null)
            //{
            //    if ((int)tube.TubeMode == countPairs)
            //        tube.TubeMode = TubeMode.Nominations;
            //    else
            //        tube.TubeMode = (TubeMode)Enum.ToObject(typeof(TubeMode), mode);

            //    tubeRepository.Update(tube);
            //    HttpContext.Cache["TubeChanged"] = tube;
            //    Session["currentTube"] = tube;
            //}
            //else if (tube.TubeMode != tubeWasChanged.TubeMode && tube.TubeId == tubeWasChanged.TubeId)
            //{
            //    if ((int)tube.TubeMode == countPairs)
            //        tube.TubeMode = TubeMode.Nominations;
            //    else
            //        tube.TubeMode = (TubeMode)Enum.ToObject(typeof(TubeMode), mode); ;

            //    tubeRepository.Update(tube);
            //    Session["currentTube"] = tube;
            //}

            //if ((int)HttpContext.Application[tube.TubeId.ToString()] == countPairs * 2)
            //    HttpContext.Application[tube.TubeId.ToString()] = 0;
            
            ////----stub---
            //if (tube.TubeMode == TubeMode.Opened && countPairs != 5)
            //    participantRepository.DeleteFromTubeSingleUsers(tube.TubeId);
            ////-----------
            
            //if((int)HttpContext.Application[tube.TubeId.ToString()] == 0)
            //{
            //    if ((int)tube.TubeMode == countPairs)
            //        tube.TubeMode = TubeMode.Nominations;
            //    else
            //        tube.TubeMode += 1;

            //    tubeRepository.Update(tube);
            //}
            //HttpContext.Application[tube.TubeId.ToString()] = (int) HttpContext.Application[tube.TubeId.ToString()] + 1;



            int roundNumber = (int)tube.TubeMode;

            if ((int)tube.TubeMode > countPairs)
            {
                SetTubeMode((int)TubeMode.Nominations);

                if(User.IsInRole("Investor"))
                    return RedirectToAction("Nomination", new { tubeId = tube.TubeId });
                else
                    return RedirectToAction("Results", new { tubeId = tube.TubeId });
            }

            Participant currentParticipant = participantRepository.FindPartner(userId, tube.TubeId, roundNumber);

            string sessionId = HttpContext.Cache[userId.ToString()] == null ? null : HttpContext.Cache[userId.ToString()].ToString();

            if (sessionId == null)
            {
                OpenTokSDK opentok = new OpenTokSDK();
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add(SessionPropertyConstants.P2P_PREFERENCE, "enabled");
                sessionId = opentok.CreateSession(Request.ServerVariables["REMOTE_ADDR"]);
                HttpContext.Cache[currentParticipant.UserId.ToString()] = sessionId;
            }

            NameValueCollection appSettings = ConfigurationManager.AppSettings;


            ViewData["apiKey"] = appSettings["opentok_key"];
            ViewData["sessionId"] = sessionId;

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
            ViewBag.NextMode = roundNumber + 1;

            return View();

        }

        [HttpGet,TubeRedirection]
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
                                    AvatarPath = personRepository.FirstOrDefault(y => y.UserId == x.UserId).AvatarPath.Replace("\\", "/"),
                                    Role = x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName
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

        [HttpGet,TubeRedirection]
        public ActionResult Results(int tubeId)
        {
            var results = Util.ConverUserDataListToUserModelList(participantRepository.GetResult(tubeId));
            ViewBag.TubeId = tubeId;
            return View(results);
        }

        [HttpGet]
        public ActionResult FindTube(string description)
        {
            ViewBag.Description = description;
            return View();
        }
        
        [HttpGet]
        public ActionResult TubeExcluded(int tubeId)
        {
            var userId = GetCurrentUserId();
            var participant = participantRepository.FirstOrDefault(p => p.UserId == userId && p.TubeId == tubeId);
            ViewBag.Description = participant.Description;
            participantRepository.Delete(participant);

            return View();
        }

        //Async actions

        public JsonResult IsPatrtnerOnline(Guid partnerId)
        {
            MembershipUser partner = Membership.GetUser(partnerId);
            return Json(new { isOnline = partner.IsOnline }, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetRatings(int tubeId)
        {
            return new JsonResult
                       {
                           Data = participantRepository.getNominationAndPendingStatus(tubeId),
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                       };

        }
       
        public JsonResult GetTubeTimeout(int tubeId)
        {
            return Json(new { timeOut = tubeRepository.GetTubeTimeout(tubeId) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrentTimePitch(Guid partnerId)
        {
            Guid userId = GetCurrentUserId();
            return Json(new { leftTime = partnerRepository.GetLeftTimePitch(userId, partnerId, minDateTime)}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsSecondReady(Guid partnerId)
        {
            Guid userId = GetCurrentUserId();

            Partner pair = partnerRepository.FirstOrDefault(p => (p.UserId == userId && p.PartnerId == partnerId) || (p.UserId == partnerId && p.PartnerId == userId));

            return Json(new { isReady = pair.BeginPitchTime != null && pair.BeginPitchTime != minDateTime },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SetTubeMode(int mode)
        {
            var tube = Session["currentTube"] as Tube;
            if (tube == null) return;
            if(tube.Mode != mode)
            {
                tube = tubeRepository.FirstOrDefault(t => t.TubeId == tube.TubeId);
                tube.TubeMode = (TubeMode)Enum.ToObject(typeof(TubeMode), mode);
                tubeRepository.Update(tube);
            }
            
            
            Session["currentTube"] = tube;
        }

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




    }
}
