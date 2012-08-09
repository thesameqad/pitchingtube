using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PitchingTube.Data;


namespace PitchingTube.Controllers
{
    public class TubeController : Controller
    {
        //
        // GET: /Tube/

        public ActionResult Index(int tubeId)
        {
            ViewBag.TubeId = tubeId;
            return View();
        }

        public ActionResult Generate()
        {
            var repository = new BaseRepository<Participant>();

            var tubeId = 1;
            for (int i = 0; i < 6; i++)
            {
                var entity = new Participant() { TubeId = tubeId, UserId = Guid.NewGuid() };
                repository.Insert(entity);
            }
            return RedirectToAction("Index");
        }

        public ActionResult TubePeopleList(int tubeId)
        {
            var repository = new ParticipantRepository();
            var model = repository.TubeParticipants(tubeId);
            ViewData["LeftInvestor"] = 5 - model.Count(x => x.Role=="Investor");
            ViewData["LeftEntrepreneur"] = 5 - model.Count(x => x.Role == "Entrepreneur");
           // ViewBag.Participant = model;
            return View(model);
        }
    }
}
