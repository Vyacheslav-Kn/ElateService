using ElateService.BLL.Interfaces;
using ElateService.BLL.Services;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class UserActivityServiceModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IUserActivityService>().To<UserActivityService>();
        }
    }
}
