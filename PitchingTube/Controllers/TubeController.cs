using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PitchingTube.Data;


namespace PitchingTube.Controllers
{
    public class TubeController : Controller
    {
        //
        // GET: /Tube/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate()
        {
            var repository = new BaseRepository<Participant>();

            var tubeId = 06082012;
            for (int i = 0; i < 6; i++)
            {
                var entity = new Participant() { TubeId = tubeId, UserId = new Random().Next(06082012, 96082012) };
                repository.Insert(entity);
            }
            return RedirectToAction("Index");
        }

        public ActionResult TubePeopleList()
        {
            var guid = 06082012;
            var repository = new BaseRepository<Participant>();
            var model = repository.Query(x => x.TubeId == guid).ToList();
            ViewData["Left"] = 10 - model.Count;
            ViewBag.Participant = model;
            return View(ViewBag);
        }
    }
}
