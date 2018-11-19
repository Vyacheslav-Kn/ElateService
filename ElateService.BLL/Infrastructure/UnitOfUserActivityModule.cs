using ElateService.DAL.Interfaces;
using ElateService.DAL.Repositories;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class UnitOfUserActivityModule: NinjectModule
    {
        private string _connectionString;

        public UnitOfUserActivityModule(string connection)
        {
            _connectionString = connection;
        }


        public override void Load()
        {
            Bind<IUnitOfUserActivity>().To<UnitOfUserActivity>().WithConstructorArgument(_connectionString);
        }
    }
}
