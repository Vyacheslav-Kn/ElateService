using ElateService.Common;

namespace ElateService.BLL.ModelsDTO
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Context { get; set; }
        public int ToId { get; set; }
        public int FromId { get; set; }
        public string FromName { get; set; }
        public string IndentTitle { get; set; }
        public int IndentId { get; set; }
        public bool WasRead { get; set; }
        public Role RoleId { get; set; }
    }
}
