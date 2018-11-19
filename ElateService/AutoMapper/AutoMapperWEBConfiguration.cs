using AutoMapper;
using ElateService.BLL.Models;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using ElateService.Models;
using System;
using System.Linq;

namespace ElateService.AutoMapper
{
    public class RegistrationViewModelToClientDTOProfile: Profile
    {
        public RegistrationViewModelToClientDTOProfile()
        {
            CreateMap<RegistrationViewModel, ClientDTO>(); 
        }
    }


    public class LoginViewModelToClientLoginDTOProfile: Profile
    {
        public LoginViewModelToClientLoginDTOProfile()
        {
            CreateMap<LoginViewModel, ClientDTO>();
        }
    }


    public class CustomerDTOToCustomerPropertiesForEditionViewModelProfile : Profile
    {
        public CustomerDTOToCustomerPropertiesForEditionViewModelProfile()
        {
            CreateMap<CustomerDTO, CustomerPropertiesForEditionViewModel>();
        }
    }


    public class CustomerPropertiesForEditionViewModelToCustomerDTOProfile : Profile
    {
        public CustomerPropertiesForEditionViewModelToCustomerDTOProfile()
        {
            CreateMap<CustomerPropertiesForEditionViewModel, CustomerDTO>();
        }
    }


    public class ExecutorDTOToExecutorPropertiesForEditionViewModelProfile : Profile
    {
        public ExecutorDTOToExecutorPropertiesForEditionViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorDTO, ExecutorPropertiesForEditionViewModel>(); 
        }
    }


    public class ExecutorPropertiesForEditionViewModelToExecutorDTOProfile : Profile
    {
        public ExecutorPropertiesForEditionViewModelToExecutorDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorPropertiesForEditionViewModel, ExecutorDTO>();
        }
    }


    public class IndentViewModelToIndentDTOProfile : Profile
    {
        public IndentViewModelToIndentDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<IndentViewModel, IndentDTO>()
                .ForMember(i => i.CategoryId, opt => opt.MapFrom(m => m.Category));
        }
    }


    public class IndentDTOToIndentViewModelProfile : Profile
    {
        public IndentDTOToIndentViewModelProfile()
        { 
            AllowNullCollections = true;
            CreateMap<IndentDTO, IndentViewModel>()
                .ForMember(i => i.Category, opt => opt.MapFrom(m => m.CategoryId));
        }
    }


    public class UserActivityViewModelToNotificationDTOProfile : Profile
    {
        public UserActivityViewModelToNotificationDTOProfile()
        {
            CreateMap<UserActivityViewModel, NotificationDTO>()
                .ForMember(i => i.ToId, opt => opt.MapFrom(m => m.UserOpponentId));
        }
    }


    public class CustomerDTOToUserProfileViewModelProfile : Profile
    {
        public CustomerDTOToUserProfileViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<CustomerDTO, UserProfileViewModel>()
                .ForMember(i => i.UserId, opt => opt.MapFrom(m => m.CustomerId)); 
        }
    }


    public class ExecutorDTOToUserProfileViewModelProfile : Profile
    {
        public ExecutorDTOToUserProfileViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorDTO, UserProfileViewModel>()
                .ForMember(i => i.UserId, opt => opt.MapFrom(m => m.ExecutorId));
        }
    }    


    public class IndentDTOPageToIndentPageViewModelProfile : Profile
    {
        public IndentDTOPageToIndentPageViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<IndentDTOPage, IndentPageViewModel>();
        }
    }


    public class ExecutorDTOPageToExecutorPageViewModelProfile : Profile
    {
        public ExecutorDTOPageToExecutorPageViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorDTOPage, ExecutorPageViewModel>();
        }
    }


    public class ExecutorDTOToExecutorViewModelProfile : Profile
    {
        public ExecutorDTOToExecutorViewModelProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorDTO, ExecutorViewModel>();
        }
    }

}