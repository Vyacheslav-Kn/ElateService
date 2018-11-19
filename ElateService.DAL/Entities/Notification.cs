using ElateService.Common;
using System;

namespace ElateService.DAL.Entities
{
    public class Notification
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


        public bool Equals(Notification other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return NotificationId.Equals(other.NotificationId) && ToId.Equals(other.ToId);
        }


        public override int GetHashCode()
        {
            int hashToId = ToId.GetHashCode();

            int hashNotificationId = NotificationId.GetHashCode();

            return hashToId ^ hashNotificationId;
        }
    }
}
