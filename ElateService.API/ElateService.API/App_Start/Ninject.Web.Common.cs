[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectApi.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectApi.App_Start.NinjectWebCommon), "Stop")]

namespace NinjectApi.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using System.Web.Http;
    using Ninject.Web.WebApi;
    using Models;
    using Ninject.Web.Common.WebHost;
    using ElateService.BLL.Infrastructure;
    using ElateService.API.AutoMapper;
    using System.Configuration;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
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

            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //kernel.Bind<ITestService>().To<TestService>();
        }        
    }
}