using ElateService.BLL.ModelsDTO;
using System.Collections.Generic;

namespace ElateService.Models
{
    public class NotificationsViewModel
    {
        public bool isUserHasNewNotifications { get; set; }
        public IEnumerable<NotificationDTO> Notifications { get; set; }

        public NotificationsViewModel()
        {
            Notifications = new List<NotificationDTO>();
        }
    }
}