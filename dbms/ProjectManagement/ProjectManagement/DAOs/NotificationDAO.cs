using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Mappers.Implement;
using System.Data.SqlClient;
using ProjectManagement.Enums;
using ProjectManagement.Utils;
using Microsoft.VisualBasic.ApplicationServices;
using ProjectManagement.MetaData;
using ProjectManagement.Mappers;
using Guna.UI2.AnimatorNS;

namespace ProjectManagement.DAOs
{
    internal class NotificationDAO : DBConnection
    {
        
        #region SELECT NOTIFICATION

        // 1.
        public static List<NotificationMeta> SelectList(string userId)
        {
            string sqlStr = "EXEC PROC_GetNotificationsByUserId @UserId";
            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@UserId", userId) };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<NotificationMeta> list = new List<NotificationMeta>();
            NotificationMapper notificationMapper = new NotificationMapper();
            List<string> favoriteNotifications = GetFavoriteNotificationByUserId(userId);

            foreach (DataRow row in dataTable.Rows)
            {
                Notification notification = notificationMapper.MapRow(row);
                bool isSaw = row["seen"].ToString() == "True" ? true : false;

                list.Add(new NotificationMeta(notification, isSaw, favoriteNotifications.Contains(notification.NotificationId)));
            }

            return list.OrderByDescending(n => n.Notification.CreatedAt).ToList();
        }
        // 2.
        private static List<string> GetFavoriteNotificationByUserId(string userId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.FavoriteNotification,
            [
                new ("userId", userId),
            ]);

            List<string> favoriteProjects = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                favoriteProjects.Add(row["notificationId"].ToString());
            }

            return favoriteProjects;
        }

        #endregion

        #region NOTIFICATION DAO EXECUTION

        // 3.
        public static void InsertOnly(Notification notification)
        {
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("notificationId", notification.NotificationId),
                new KeyValuePair<string, string>("title", notification.Title),
                new KeyValuePair<string, string>("content", notification.Content),
                new KeyValuePair<string, string>("type", EnumUtil.GetDisplayName(notification.Type)),
                new KeyValuePair<string, string>("createdAt", notification.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
            };
            DBExecution.InsertDynamic(DBTableNames.Notification, conditions);
        }

        public static void Insert(Notification notification, string userId)
        {
            InsertOnly(notification);
            InsertViewNotification(userId, notification.NotificationId, false);
        }
        public static void InsertFollowTeam(string teamId, string content, ENotificationType type)
        {
            Notification notification = new Notification("Notification", content, type, DateTime.Now);
            InsertOnly(notification);

            List<Member> members = TeamDAO.GetMembersByTeamId(teamId);

            foreach (Member member in members)
            {
                InsertViewNotification(member.User.UserId, notification.NotificationId, false);
            }
        }
        public static void InsertFollowPeoples(List<Users> peoples, string content, ENotificationType type)
        {
            Notification notification = new Notification("Notification", content, type, DateTime.Now);
            InsertOnly(notification);

            foreach (Users people in peoples)
            {
                InsertViewNotification(people.UserId, notification.NotificationId, false);
            }
        }

        // 4.
        public static void InsertViewNotification(string userId, string notificationId, bool seen)
        {
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("notificationId", notificationId),
                new KeyValuePair<string, string>("userId", userId),
                new KeyValuePair<string, string>("seen", (seen ? 1 : 0).ToString())
            };
            DBExecution.InsertDynamic(DBTableNames.ViewNotification, conditions);
        }

        //5. 
        public static void DeleteFavouriteNotification(string notificationId)
        {
            DBExecution.DeleteDynamic(DBTableNames.FavoriteNotification,
            [
               new ("notificationId", notificationId),
            ]);
        }

        // 6.
        public static void DeleteViewNotification(string notificationId)
        {
            DBExecution.DeleteDynamic(DBTableNames.ViewNotification,
            [
              new ("notificationId", notificationId),
            ]);
        }
        public static void Delete(string notificationId)
        {
            DeleteFavouriteNotification(notificationId);
            DeleteViewNotification(notificationId);
        }

        // 7.
        public static void UpdateIsSaw(string userId, string notificationId, bool flag)
        {
            List<KeyValuePair<string, string>> setValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("seen", (flag ? 1 : 0).ToString())
            };
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("userId", userId),
                new KeyValuePair<string, string>("notificationId", notificationId),
            };
            DBExecution.UpdateDynamic(DBTableNames.ViewNotification, setValues, conditions);
        }

        // 8.
        public static void UpdateFavorite(string userId, string notificationId, bool isFavorite)
        {
            string sqlStr = string.Format("EXEC dbo.PROC_UpdateFavoriteNotification @IsFavorite, @UserId, @NotificationId");

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@IsFavorite", isFavorite ? 1 : 0),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@NotificationId", notificationId)
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        #endregion

    }
}
