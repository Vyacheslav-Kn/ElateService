using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace ElateService.Localization
{
    public class CultureAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = filterContext.HttpContext.Session["Language"];
            string cultureName = null;

            if (culture == null)
            {
                cultureName = "ru";
                filterContext.HttpContext.Session["Language"] = "ru";
            }
            else
            {
                cultureName = culture.ToString();
            }              

            List<string> cultures = new List<string>() { "ru", "en"};
            if (!cultures.Contains(cultureName))
            {
                cultureName = "ru";
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
        }

    }
}