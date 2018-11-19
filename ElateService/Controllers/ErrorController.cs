using ElateService.Localization;
using ElateService.Safe_execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElateService.Controllers
{
    [Culture, Exception]
    public class ErrorController : Controller
    {
        ///<summary>
        ///This method we need only to display error page with localization
        ///</summary>
        public ActionResult Warning()
        {
            return View();
        }


        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }


        // Server can work with received data, but according to logic there is no answer
        public ActionResult UnprocessableEntity()
        {
            Response.StatusCode = 422;
            return View();
        }


        public ActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View();
        }

    }
}