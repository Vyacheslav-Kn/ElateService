using AutoMapper;
using ElateService.BLL.AutoMapper;
using Ninject.Modules;

namespace ElateService.AutoMapper
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

                    cfg.AddProfile(new RegistrationViewModelToClientDTOProfile());

                    cfg.AddProfile(new LoginViewModelToClientLoginDTOProfile());

                    cfg.AddProfile(new CustomerToCustomerDTOProfile());

                    cfg.AddProfile(new CustomerDTOToCustomerProfile()); 

                    cfg.AddProfile(new CustomerDTOToCustomerPropertiesForEditionViewModelProfile());

                    cfg.AddProfile(new CustomerPropertiesForEditionViewModelToCustomerDTOProfile()); 

                    cfg.AddProfile(new ExecutorToExecutorDTOProfile());

                    cfg.AddProfile(new ExecutorDTOToExecutorProfile()); 

                    cfg.AddProfile(new ExecutorDTOToExecutorPropertiesForEditionViewModelProfile());

                    cfg.AddProfile(new ExecutorPropertiesForEditionViewModelToExecutorDTOProfile());

                    cfg.AddProfile(new IndentToIndentDTOProfile());

                    cfg.AddProfile(new IndentDTOToIndentProfile());

                    cfg.AddProfile(new IndentViewModelToIndentDTOProfile());

                    cfg.AddProfile(new IndentDTOToIndentViewModelProfile());

                    cfg.AddProfile(new RecallToRecallDTOProfile());

                    cfg.AddProfile(new RecallDTOToRecallProfile()); 

                    cfg.AddProfile(new NotificationToNotificationDTOProfile());

                    cfg.AddProfile(new NotificationDTOToNotificationProfile());

                    cfg.AddProfile(new ResponceToResponceDTOProfile());

                    cfg.AddProfile(new ResponceDTOToResponceProfile()); 

                    cfg.AddProfile(new UserActivityViewModelToNotificationDTOProfile());

                    cfg.AddProfile(new CustomerDTOToUserProfileViewModelProfile());

                    cfg.AddProfile(new ExecutorDTOToUserProfileViewModelProfile());

                    cfg.AddProfile(new IndentPageTOIndentDTOPage());

                    cfg.AddProfile(new IndentDTOPageToIndentPageViewModelProfile());

                    cfg.AddProfile(new ExecutorPageTOExecutorDTOPage());

                    cfg.AddProfile(new ExecutorDTOPageToExecutorPageViewModelProfile());

                    cfg.AddProfile(new ExecutorDTOToExecutorViewModelProfile()); 

                    // tell automapper to use ninject when creating value converters and resolvers
                    // cfg.ConstructServicesUsing(t => kernel.Get(t));
                });
                return config.CreateMapper();
            }).InSingletonScope();
        }
    
    }
}