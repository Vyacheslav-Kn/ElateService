using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using ElateService.Api.Providers;
using Ninject;
using ElateService.BLL.Interfaces;
using ElateService.BLL.Infrastructure;
using ElateService.API.AutoMapper;
using System.Configuration;

[assembly: OwinStartup(typeof(NinjectApi.Startup))]

namespace NinjectApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }


        public void ConfigureOAuth(IAppBuilder app)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ElateServiceConnection"].ConnectionString;
            var kernel = new StandardKernel(new Ninject.Modules.INinjectModule[] {
                new CustomerServiceModule(), new ExecutorServiceModule(), new IndentServiceModule(), new UserActivityServiceModule(),
                    new UnitOfWorkModule(connectionString), new UnitOfUserActivityModule(connectionString), new AutoMapperModule()
            });

            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
                Provider = new APIOAuthAuthorizationServerProvider(kernel.Get<ICustomerService>(), kernel.Get<IExecutorService>())
            };
            //app.UseOAuthBearerTokens(oAuthServerOptions);
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
