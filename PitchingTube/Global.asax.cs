using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using PitchingTube.Data;

namespace PitchingTube
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            string userName = Membership.GetUserNameByEmail(User.Identity.Name);
            Guid userId = Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString());

            ParticipantRepository participantRepository = new ParticipantRepository();
            participantRepository.RemoveUserFromAllTubes(userId);

            HttpContext.Current.Cache[userId.ToString() + "online"] = false;

        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userName = Membership.GetUserNameByEmail(User.Identity.Name);
                Guid userId = Guid.Parse(Membership.GetUser(userName).ProviderUserKey.ToString());

                HttpContext.Current.Cache[userId.ToString() + "online"] = true;
            }
            
        }
    }
}