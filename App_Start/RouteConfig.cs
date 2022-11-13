using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SocialNetwork
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "Friends",
            url: "Account/Friends/{username}",
            defaults: new { controller = "Account", action = "Friends", useranme = UrlParameter.Optional }
          );



            routes.MapRoute(
            name: "GetUser",
            url: "Account/GetUser/{userId}",
            defaults: new { controller = "Account", action = "UserId", userId = UrlParameter.Optional }
          );

            routes.MapRoute(
            name: "UserId",
            url: "Account/UserId/{userId}",
            defaults: new { controller = "Account", action = "UserId", userId = UrlParameter.Optional }
        );

            routes.MapRoute(
             name: "Profile",
             url: "Profile/{action}/{id}",
             defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional }
         );



            routes.MapRoute(
              name: "Login",
              url: "Account/Login",
              defaults: new { controller = "Account", action = "Login" }
          );


            routes.MapRoute(
               name: "LoginPartial",
               url: "Account/LoginPartial",
               defaults: new { controller = "Account", action = "LoginPartial" }
           );

            routes.MapRoute(
               name: "Logout",
               url: "Account/Logout",
               defaults: new { controller = "Account", action = "Logout" }
           );

            routes.MapRoute(
               name: "Account",
               url: "{username}",
               defaults: new { controller = "Account", action = "Username" }
           );

            routes.MapRoute(
               name: "CreateAccount",
               url: "Account/CreateAccount",
               defaults: new { controller = "Account", action = "CreateAccount" }
           );

            routes.MapRoute(
               name: "Default",
               url: "",
               defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
           );

            // routes.MapRoute(
            //     name: "Default",
            //     url: "{controller}/{action}/{id}",
            //     defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            // );
        }
    }
}
