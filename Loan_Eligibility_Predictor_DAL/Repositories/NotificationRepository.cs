using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loan_Eligibility_Predictor_DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Loan_Eligibility_Predictor_DAL.Repositories
{
    public class NotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        #region AddNotification
        public bool AddNotification(int userId, string message)
        {
            try
            {
                var result = _context.Set<NotificationResponse>()
                    .FromSqlRaw("EXEC sp_AddNotification @UserId, @Message",
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@Message", message))
                    .AsEnumerable()
                    .FirstOrDefault();
                return result != null && result.Result == 1;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Get Notification
        public List<Notification> GetNotificationsByUserId(int userId)
        {
            try
            {
                return _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching notifications: " + ex.Message);
            }
        }


        #endregion

        #region Mark As Read
        public void MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);

            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }
        #endregion

        #region Unread Count
        public int GetUnreadCount(int userId)
        {
            return _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).Count();
        }
        #endregion
    }
}
