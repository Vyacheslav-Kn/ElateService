using ElateService.BLL.Interfaces;
using ElateService.BLL.Services;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class CustomerServiceModule: NinjectModule
    {
        public override void Load()
        {
            Bind<ICustomerService>().To<CustomerService>();
        }
    }
}
