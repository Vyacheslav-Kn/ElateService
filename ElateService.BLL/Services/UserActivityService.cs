using AutoMapper;
using ElateService.BLL.Interfaces;
using ElateService.BLL.ModelsDTO;
using ElateService.Common;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.BLL.Services
{
    public class UserActivityService: IUserActivityService
    {
        private IUnitOfUserActivity _database;
        private readonly IMapper _mapper;

        public UserActivityService(IUnitOfUserActivity unitOfUserActivity, IMapper mapper)
        {
            _database = unitOfUserActivity;
            _mapper = mapper;
        }


        public async Task SaveCustomerRecallForExecutor(RecallDTO recallDTO)
        {
            if(recallDTO.CustomerCommentForExecutor == null)
            {

            }

            Recall recall = _mapper.Map<Recall>(recallDTO);

            await _database.Recalls.AddCustomerRecallPropertiesForExecutor(recall);
        }


        public async Task SaveExecutorRecallForCustomer(RecallDTO recallDTO)
        {
            Recall recall = _mapper.Map<Recall>(recallDTO);

            await _database.Recalls.AddExecutorRecallPropertiesForCustomer(recall);
        }


        public async Task SaveNotification(NotificationDTO notificationDTO)
        {
            Notification notification = _mapper.Map<Notification>(notificationDTO);

            await _database.Notifications.Create(notification);
        }


        public async Task SaveResponce(ResponceDTO responceDTO)
        {
            Responce responce = _mapper.Map<Responce>(responceDTO);

            await _database.Responces.Create(responce);
        }


        public async Task SaveExecutorForIndent(IndentDTO indentDTO, int executorId)
        {
            Indent indent = _mapper.Map<Indent>(indentDTO);
            indent.Executor = new Executor()
            {
                ExecutorId = executorId
            };

            await _database.Indents.SetExecutorId(indent);
        }


        public async Task<IEnumerable<NotificationDTO>> GetNotifications(Role clientRole, int clientId)
        {
            IEnumerable<Notification> notifications = await _database.Notifications.GetNotificationsByUserRoleAndId(clientRole, clientId);

            IEnumerable<NotificationDTO> notificationsDTO = _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationDTO>>(notifications);           

            return notificationsDTO;
        }


        public NotificationDTO CheckForNewNotifications(Role clientRole, int clientId)
        {
            Notification notification = _database.Notifications.GetSingleNotificationByUserRoleAndId(clientRole, clientId);

            NotificationDTO notificationDTO = _mapper.Map<NotificationDTO>(notification);

            return notificationDTO;
        }


        public async Task<IEnumerable<IndentDTO>> GetCustomerIndents(int id)
        {
            IEnumerable<Indent> indents = await _database.Indents.GetIndentsByCustomerId(id);

            IEnumerable<IndentDTO> indentsDTO = _mapper.Map<IEnumerable<Indent>, IEnumerable<IndentDTO>>(indents);

            return indentsDTO;
        }


        public async Task<IEnumerable<IndentDTO>> GetIndentsWithExecutorResponce(int executorId)
        {
            IEnumerable<Indent> indents = await _database.Indents.GetIndentsWithExecutorResponce(executorId);

            IEnumerable<IndentDTO> indentsDTO = _mapper.Map<IEnumerable<Indent>, IEnumerable<IndentDTO>>(indents);

            return indentsDTO;
        }


        public async Task<IEnumerable<IndentDTO>> GetFreeCustomerIndentsForExecutor(int customerId, int executorId)
        {
            IEnumerable<Indent> indents = await _database.Indents.GetFreeCustomerIndentsForExecutor(customerId, executorId);

            IEnumerable<IndentDTO> indentsDTO = _mapper.Map<IEnumerable<Indent>, IEnumerable<IndentDTO>>(indents);

            return indentsDTO;
        }

    }
}
