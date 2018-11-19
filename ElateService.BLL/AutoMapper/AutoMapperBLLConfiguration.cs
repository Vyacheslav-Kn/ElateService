using AutoMapper;
using ElateService.BLL.Models;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using ElateService.Common;
using ElateService.DAL.Entities;
using ElateService.DAL.PaginationEntity;
using System;
using System.Linq;

namespace ElateService.BLL.AutoMapper
{
    public class CustomerToClientDTOProfile : Profile
    {
        public CustomerToClientDTOProfile()
        {
            CreateMap<Customer, ClientDTO>()
                .ForMember(i => i.ClientId, opt => opt.MapFrom(m => m.CustomerId))
                .ForMember(i => i.Password, opt => opt.MapFrom(m => m.PasswordHash));
        }
    }


    public class ClientDTOToCustomerProfile : Profile
    {
        public ClientDTOToCustomerProfile()
        {
            CreateMap<ClientDTO, Customer>()
                .ForMember(i => i.CustomerId, opt => opt.MapFrom(m => m.ClientId))
                .ForMember(i => i.PasswordHash, opt => opt.MapFrom(m => m.Password));
        }
    }


    public class ExecutorToClientDTOProfile : Profile
    {
        public ExecutorToClientDTOProfile()
        {
            CreateMap<Executor, ClientDTO>()
                .ForMember(i => i.ClientId, opt => opt.MapFrom(m => m.ExecutorId))
                .ForMember(i => i.Password, opt => opt.MapFrom(m => m.PasswordHash));
        }
    }


    public class ClientDTOToExecutorProfile : Profile
    {
        public ClientDTOToExecutorProfile()
        {
            CreateMap<ClientDTO, Executor>()
                .ForMember(i => i.ExecutorId, opt => opt.MapFrom(m => m.ClientId))
                .ForMember(i => i.PasswordHash, opt => opt.MapFrom(m => m.Password));
        }
    }


    public class CustomerToCustomerDTOProfile : Profile
    {
        public CustomerToCustomerDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<Customer, CustomerDTO>();
        }
    }


    public class CustomerDTOToCustomerProfile : Profile
    {
        public CustomerDTOToCustomerProfile()
        {
            AllowNullCollections = true;
            CreateMap<CustomerDTO, Customer>();
        }
    }


    public class ExecutorToExecutorDTOProfile : Profile
    {
        public ExecutorToExecutorDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<Executor, ExecutorDTO>()
                .ForMember(i => i.Categories, opt => opt.MapFrom(m => m.Categories.Any() ?
                    m.Categories.Select(s => (Category)s.CategoryId) : null));
        }
    }


    public class ExecutorDTOToExecutorProfile : Profile
    {
        public ExecutorDTOToExecutorProfile()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorDTO, Executor>()
                .ForMember(i => i.Categories, s => s.Ignore())
                .ForMember(i => i.RoleId, s => s.Ignore());
        }
    }


    public class IndentToIndentDTOProfile : Profile
    {
        public IndentToIndentDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<Indent, IndentDTO>();
        }
    }


    public class IndentDTOToIndentProfile : Profile
    {
        public IndentDTOToIndentProfile()
        {
            AllowNullCollections = true;
            CreateMap<IndentDTO, Indent>();
        }
    }


    public class RecallToRecallDTOProfile : Profile
    {
        public RecallToRecallDTOProfile()
        {
            CreateMap<Recall, RecallDTO>()
                .ForMember(i => i.RecallId, opt => opt.MapFrom(m => m.Indent.IndentId));
        }
    }


    public class RecallDTOToRecallProfile : Profile
    {
        public RecallDTOToRecallProfile()
        {
            CreateMap<RecallDTO, Recall>()
                .ForPath(i => i.Indent.IndentId, opt => opt.MapFrom(m => m.RecallId)); 
        }
    }


    public class NotificationToNotificationDTOProfile : Profile
    {
        public NotificationToNotificationDTOProfile()
        {
            AllowNullCollections = true;
            CreateMap<Notification, NotificationDTO>();
        }
    }


    public class NotificationDTOToNotificationProfile : Profile
    {
        public NotificationDTOToNotificationProfile()
        {
            CreateMap<NotificationDTO, Notification>();
        }
    }


    public class ResponceToResponceDTOProfile : Profile
    {
        public ResponceToResponceDTOProfile()
        {
            CreateMap<Responce, ResponceDTO>()
                .ForMember(i => i.IndentId, opt => opt.MapFrom(m => m.Indent.IndentId));
        }
    }


    public class ResponceDTOToResponceProfile : Profile
    {
        public ResponceDTOToResponceProfile()
        {
            CreateMap<ResponceDTO, Responce>()
                .ForPath(i => i.Indent.IndentId, opt => opt.MapFrom(m => m.IndentId));
        }
    }


    public class IndentPageTOIndentDTOPage : Profile
    {
        public IndentPageTOIndentDTOPage()
        {
            AllowNullCollections = true;
            CreateMap<IndentPage, IndentDTOPage>();
        }
    }


    public class ExecutorPageTOExecutorDTOPage : Profile
    {
        public ExecutorPageTOExecutorDTOPage()
        {
            AllowNullCollections = true;
            CreateMap<ExecutorPage, ExecutorDTOPage>();
        }
    }

}
