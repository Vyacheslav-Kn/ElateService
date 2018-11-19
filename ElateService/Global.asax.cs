using ElateService.AutoMapper;
using ElateService.BLL.Infrastructure;
using Ninject;
using Ninject.Web.Mvc;
using System.Configuration;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ElateService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultRoleClaimType;
            
            string connectionString = ConfigurationManager.ConnectionStrings["ElateServiceConnection"].ConnectionString;
            CustomerServiceModule customerServiceModule = new CustomerServiceModule();
            ExecutorServiceModule executorServiceModule = new ExecutorServiceModule();
            IndentServiceModule indentServiceModule = new IndentServiceModule();
            UserActivityServiceModule userActivityModule = new UserActivityServiceModule();
            UnitOfWorkModule unitOfWorkModule = new UnitOfWorkModule(connectionString);
            UnitOfUserActivityModule unitOfUserActivityModule = new UnitOfUserActivityModule(connectionString);
            AutoMapperModule mapperModule = new AutoMapperModule();
            var kernel = new StandardKernel(customerServiceModule, executorServiceModule, indentServiceModule, userActivityModule,
                unitOfWorkModule, unitOfUserActivityModule, mapperModule);
            // Web Api
            // System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Ninject.Web.WebApi.NinjectDependencyResolver(kernel);

            // MVC 
            // System.Web.Mvc.DependencyResolver.SetResolver(new Ninject.Web.Mvc.NinjectDependencyResolver(kernel));
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
