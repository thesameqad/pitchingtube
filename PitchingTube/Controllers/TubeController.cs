﻿using System;
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
            else
            {
                var repository = new ParticipantRepository();
                var repositoryP = new BaseRepository<Person>();
                var model = repository.Query(x => x.TubeId == tubeId && x.aspnet_Users.aspnet_Roles.FirstOrDefault().RoleName == "Entrepreneur")
                      .Select(x => new ParticipantRepository.UserInfo()
                      {
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
        [HttpGet]
        public ActionResult StartPitch(int tubeId)
        {
            Guid userId = (Guid)Membership.GetUser(Membership.GetUserNameByEmail(User.Identity.Name)).ProviderUserKey;

            //int currTube

            //just a showcase. Will be removed in the future
            ViewBag.CurrentPartnerId = participantRepository.FindPartner(userId, tubeId/*(int)Session["currentTube"]*/, 5).UserId;

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
