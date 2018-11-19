using Dapper;
using ElateService.Common;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ElateService.DAL.Repositories
{
    public class NotificationRepository: INotificationRepository
    {
        private string _connectionString { get; set; }

        public NotificationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task Create(Notification notification)
        {
            string sqlQuery = @"INSERT INTO Notification VALUES (@Context, @ToId, @RoleId, @WasRead, @FromId, @FromName, @IndentTitle, @IndentId)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    notification.Context,
                    notification.ToId,
                    notification.RoleId,
                    notification.WasRead,
                    notification.FromId,
                    notification.FromName,
                    notification.IndentTitle,
                    notification.IndentId
                });
            }
        }


        public Notification GetSingleNotificationByUserRoleAndId(Role role, int id)
        {
            string sqlQuery = @" SELECT TOP 1 * FROM Notification WHERE ToId = @ToId AND RoleId = @RoleId AND WasRead = 0";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                Notification notification = connection.QueryFirstOrDefault<Notification>(sqlQuery, new
                {
                    ToId = id,
                    RoleId = role,
                });

                return notification;
            }
        }


        public async Task<IEnumerable<Notification>> GetNotificationsByUserRoleAndId(Role role, int id)
        {
            string sqlQuery = @" IF EXISTS (SELECT TOP 1 * FROM Notification WHERE ToId = @ToId AND RoleId = @RoleId AND WasRead = 0)
                    BEGIN
                    SELECT * FROM Notification WHERE ToId = @ToId AND RoleId = @RoleId AND WasRead = 0 ORDER BY NotificationId DESC
                    UPDATE Notification SET WasRead = 1 WHERE ToId = @ToId AND RoleId = @RoleId AND WasRead = 0
                    END
                    ELSE
                    BEGIN
                    SELECT * FROM Notification WHERE ToId = @ToId AND RoleId = @RoleId ORDER BY NotificationId DESC
                    END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                IEnumerable<Notification> notifications = await connection.QueryAsync<Notification>(sqlQuery, new
                {
                    ToId = id,
                    RoleId = role,
                });

                return notifications;
            }
        }

    }
}
