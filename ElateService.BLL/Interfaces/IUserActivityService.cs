using ElateService.BLL.ModelsDTO;
using ElateService.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElateService.BLL.Interfaces
{
    public interface IUserActivityService
    {
        Task SaveCustomerRecallForExecutor(RecallDTO recallDTO);
        Task SaveExecutorRecallForCustomer(RecallDTO recallDTO);
        Task SaveNotification(NotificationDTO notificationDTO);
        Task SaveResponce(ResponceDTO notificationDTO);
        Task SaveExecutorForIndent(IndentDTO indentDTO, int executorId);

        ///<summary>
        ///Checks if client has notifications which weren't read. Returns first unread notification.
        ///</summary>
        NotificationDTO CheckForNewNotifications(Role role, int id);

        ///<summary>
        ///Returns notifications which weren't read, if there is no such notifications, returns all.
        ///</summary>
        Task<IEnumerable<NotificationDTO>> GetNotifications(Role role, int id);

        Task<IEnumerable<IndentDTO>> GetCustomerIndents(int id);
        Task<IEnumerable<IndentDTO>> GetIndentsWithExecutorResponce(int id);

        ///<summary>
        ///Returns customers indents which don't have any executor and have category in range of future executor(executorId).
        ///</summary>
        Task<IEnumerable<IndentDTO>> GetFreeCustomerIndentsForExecutor(int customerId, int executorId);
    }
}
