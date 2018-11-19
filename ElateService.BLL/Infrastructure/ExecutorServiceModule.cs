using ElateService.BLL.Interfaces;
using ElateService.BLL.Services;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class ExecutorServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IExecutorService>().To<ExecutorService>();
        }
    }
}
