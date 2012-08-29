using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PitchingTube.Data;
using System.Web.Routing;
using System.Web.Security;

namespace PitchingTube.Models
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }

    public class TubeRedirectionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var currentRouteValueDictionary = filterContext.Controller.ControllerContext.RouteData.Values;
            Tube tube = filterContext.HttpContext.Session["currentTube"] as Tube;
            if (tube == null)
            {
                ParticipantRepository participantRepository = new ParticipantRepository();
                string userName = Membership.GetUserNameByEmail(filterContext.HttpContext.User.Identity.Name);
                Guid userId = Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString());
                tube = participantRepository.UserIsInTube(userId);
            }
            if (tube != null)
            {
                
                if (tube.TubeMode == TubeMode.Opened)
                {
                    var newRouteValueDictionary = new RouteValueDictionary
                    {
                        {"controller", "Tube"},
                        {"action", "Index"},
                        {"tubeId", tube.TubeId}
                    };
                    if (currentRouteValueDictionary["controller"].ToString() != newRouteValueDictionary["controller"].ToString() || currentRouteValueDictionary["action"].ToString() != newRouteValueDictionary["action"].ToString())
                        filterContext.Result = new RedirectToRouteResult(newRouteValueDictionary);

                }
                else if (tube.TubeMode == TubeMode.FirstPitch || tube.TubeMode == TubeMode.SecondPitch || tube.TubeMode == TubeMode.ThirdPitch || tube.TubeMode == TubeMode.FourthPitch || tube.TubeMode == TubeMode.FifthPitch)
                {
                    var newRouteValueDictionary = new RouteValueDictionary
                    {
                        {"controller", "Tube"},
                        {"action", "StartPitch"},
                        {"mode", (int)tube.TubeMode}
                    };
                    if (currentRouteValueDictionary["controller"].ToString() != newRouteValueDictionary["controller"].ToString() || currentRouteValueDictionary["action"].ToString() != newRouteValueDictionary["action"].ToString())
                        filterContext.Result = new RedirectToRouteResult(newRouteValueDictionary);
                    
                }
                else if (tube.TubeMode == TubeMode.Nominations)
                {

                    var newRouteValueDictionary = new RouteValueDictionary
                    {
                        {"controller", "Tube"},
                        {"action", "Results"},
                        {"tubeId", tube.TubeId}
                    };

                    if (currentRouteValueDictionary["controller"].ToString() != newRouteValueDictionary["controller"].ToString() || currentRouteValueDictionary["action"].ToString() != newRouteValueDictionary["action"].ToString())
                        filterContext.Result = new RedirectToRouteResult(newRouteValueDictionary);
                }

                filterContext.HttpContext.Session["currentTube"] = tube;
            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ParticipantRepository participantRepository = new ParticipantRepository();
            string userName = Membership.GetUserNameByEmail(filterContext.HttpContext.User.Identity.Name);
            Guid userId = Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString());
            filterContext.HttpContext.Session["currentTube"] = participantRepository.UserIsInTube(userId);
        }
    }

     
}