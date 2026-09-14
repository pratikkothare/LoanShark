using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]

[Route("api/[controller]")]
public class NotificationController : ControllerBase
{

    private readonly NotificationRepository _notificationRepo;
    public NotificationController(NotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    #region AddNotification
    [HttpPost("addnotification/{userId}/{message}")]
    public JsonResult AddNotification(int userId, string message)
    {
        try
        {
            bool result = _notificationRepo.AddNotification(userId, message);
            if (result)
            {
                return new JsonResult(new{success = true,message = "Notification sent successfully"});
            }
            else
            {
                return new JsonResult(new{success = false,message = "Failed to send notification"});
            }
        }
        catch
        {
            return new JsonResult(new{success = false,message = "Something went wrong"});
        }
    }
    #endregion

    #region GetNotification
    [HttpGet("getNotifications/{userId}")]
    public JsonResult GetNotifications(int userId)
    {
        try
        {
            var notifications = _notificationRepo.GetNotificationsByUserId(userId);

            var response = new
            {
                success = true,
                message = "Notifications fetched successfully",
                count = notifications.Count,
                data = notifications
            };

            return new JsonResult(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new
            {
                success = false,
                message = "Error while fetching notifications",
                error = ex.Message
            };

            return new JsonResult(errorResponse);
        }
    }
    #endregion

    #region Mark As Read
    [HttpPut("markAsRead/{id}")]
    public JsonResult MarkAsRead(int id)
    {
        try
        {
            _notificationRepo.MarkAsRead(id);

            return new JsonResult(new
            {
                success = true,
                message = "Notification marked as read"
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                message = ex.Message
            })
            {
                StatusCode = 500
            };
        }
    }

    #endregion

    #region Unread Count
    [HttpGet("unreadCount/{userId}")]
    public JsonResult GetUnreadCount(int userId)
    {
        try
        {
            var count = _notificationRepo.GetUnreadCount(userId);

            return new JsonResult(new
            {
                success = true,
                count = count,
                message = "Unread count fetched successfully"
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                count = 0,
                message = ex.Message
            });
        }
    }
    #endregion
}