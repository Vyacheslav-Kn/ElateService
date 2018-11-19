using ElateService.BLL.Interfaces;
using ElateService.BLL.Services;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class IndentServiceModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IIndentService>().To<IndentService>();
        }
    }
}
