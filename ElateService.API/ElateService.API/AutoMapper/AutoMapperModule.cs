using AutoMapper;
using ElateService.BLL.AutoMapper;
using Ninject.Modules;

namespace ElateService.API.AutoMapper
{
    public class AutoMapperModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMapper>().ToMethod(context =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new CustomerToClientDTOProfile());

                    cfg.AddProfile(new ClientDTOToCustomerProfile());                    

                    cfg.AddProfile(new ExecutorToClientDTOProfile());

                    cfg.AddProfile(new ClientDTOToExecutorProfile());

                    cfg.AddProfile(new CustomerToCustomerDTOProfile());

                    cfg.AddProfile(new CustomerDTOToCustomerProfile()); 

                    cfg.AddProfile(new ExecutorToExecutorDTOProfile());

                    cfg.AddProfile(new ExecutorDTOToExecutorProfile()); 

                    cfg.AddProfile(new IndentToIndentDTOProfile());

                    cfg.AddProfile(new IndentDTOToIndentProfile());

                    cfg.AddProfile(new RecallToRecallDTOProfile());

                    cfg.AddProfile(new RecallDTOToRecallProfile()); 

                    cfg.AddProfile(new NotificationToNotificationDTOProfile());

                    cfg.AddProfile(new NotificationDTOToNotificationProfile());

                    cfg.AddProfile(new ResponceToResponceDTOProfile());

                    cfg.AddProfile(new ResponceDTOToResponceProfile()); 

                    cfg.AddProfile(new IndentPageTOIndentDTOPage());

                    cfg.AddProfile(new ExecutorPageTOExecutorDTOPage());

                    // tell automapper to use ninject when creating value converters and resolvers
                    // cfg.ConstructServicesUsing(t => kernel.Get(t));
                });
                return config.CreateMapper();
            }).InSingletonScope();
        }
    
    }
}