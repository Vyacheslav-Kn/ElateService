using ElateService.BLL.Infrastructure;
using System.Web.Mvc;
using System.Web.Routing;

namespace ElateService.Safe_execution
{
    public class ExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled && !(exceptionContext.Exception is ValidationException))
            {
                exceptionContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "Warning" }
                });
                exceptionContext.ExceptionHandled = true;
            }
        }
    }
}