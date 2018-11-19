using ElateService.Localization;
using ElateService.Models;
using ElateService.Safe_execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ElateService.Controllers
{    
    [Culture, Exception, AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        ///<summary>
        ///Changes application language.
        ///</summary>
        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;

            List<string> cultures = new List<string>() { "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }

            Session["Language"] = lang;

            return Redirect(returnUrl);
        }


        ///<summary>
        ///Generates enter link to user account according to logined users data.
        ///</summary>
        public ActionResult GetEnterLinkToAccount()
        {
            HelpModelForCreatingUrl url = null;

            var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string clientRole, clientName;
            int clientId;

            if (claimIdentity != null)
            {
                var roleClaim = claimIdentity.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
                var clientIdChecker = Session["Id"];

                if (roleClaim != null && clientIdChecker != null)
                {
                    clientRole = roleClaim.Value.Trim();
                    clientId = int.Parse(clientIdChecker.ToString());

                    clientName = Session["Name"].ToString();

                    if (!string.IsNullOrEmpty(clientName))
                    {
                        url = new HelpModelForCreatingUrl();
                        url.ClientId = clientId;
                        url.ClientRole = clientRole;
                        url.ClientFirstName = clientName;
                    }
                }
            }         

            return PartialView(url);
        }        

    }
}