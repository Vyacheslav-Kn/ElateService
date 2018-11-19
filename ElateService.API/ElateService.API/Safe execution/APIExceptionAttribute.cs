using ElateService.BLL.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace NinjectApi.Safe_execution
{
    public class APIExceptionAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (!(context.Exception is ValidationException))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                context.Response.Content = new StringContent("Oops, something gone wrong...");
            }
        }
    }
}