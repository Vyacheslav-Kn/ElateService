using ElateService.DAL.Interfaces;
using ElateService.DAL.Repositories;
using Ninject.Modules;

namespace ElateService.BLL.Infrastructure
{
    public class UnitOfWorkModule: NinjectModule
    {
        private string _connectionString;

        public UnitOfWorkModule(string connection)
        {
            _connectionString = connection;
        }


        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument(_connectionString);
        }
    }
}
