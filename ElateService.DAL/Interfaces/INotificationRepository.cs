using ElateService.Common;
using ElateService.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElateService.DAL.Interfaces
{
    public interface INotificationRepository
    {
        Task Create(Notification notification);

        ///<summary>
        ///Returns all unread notifications or if there is no such notifications, returns just all.
        ///</summary>
        Task<IEnumerable<Notification>> GetNotificationsByUserRoleAndId(Role role, int id);

        ///<summary>
        ///Checks if user has unread notifications and returns first unread notification.
        ///</summary>
        Notification GetSingleNotificationByUserRoleAndId(Role role, int id);
    }
}
