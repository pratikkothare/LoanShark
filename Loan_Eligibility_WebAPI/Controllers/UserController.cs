using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

[ApiController]

[Route("api/[controller]")]

public class UserController : ControllerBase

{

    private readonly UserRepository _userRepo;

    public UserController(UserRepository userRepo)

    {

        _userRepo = userRepo;

    }

    #region Register
    [HttpPost("register")]
    public JsonResult Register(string name, string email, string password)
    {
        try
        {
            bool result = _userRepo.RegisterUser(name, email, password);

            if (result)
            {
                return new JsonResult(new { success = true, message = "User registered successfully" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "User already exists or failed" });
            }
        }
        catch
        {
            return new JsonResult(new { success = false, message = "Something went wrong" });
        }
    }
    #endregion

    #region Login
    [HttpPost("login")]
    public JsonResult Login(string email, string password)
    {
        try
        {
            var user = _userRepo.LoginUser(email, password);
            if (user == null || user.UserId == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid email or password"
                });
            }
            return new JsonResult(new
            {
                success = true,
                message = "Login successful",
                user = new
                {
                    fullName = user.FullName,
                    userId = user.UserId,
                    email = user.Email,
                    roleId = user.RoleId
                },
                token = "dummy - token"
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Something went wrong"
        });
        }
    }    
    #endregion

    #region GetCreditScoreByUserId
    [HttpGet("getCreditScore/{userId}")]
    public JsonResult GetCreditScoreByUserId(int userId)
    {
        try
        {
            var data = _userRepo.GetCreditScoreByUserId(userId);
            if (data.CreditScoreId == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No credit score found"
                });
            }
            return new JsonResult(new{success = true,data = data});
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Error occurred",
                error = ex.Message
            });
        }
    }
    #endregion

    #region GetAllUsers
    [HttpGet]
    public JsonResult GetAllUsers()
    {
        try
        {
            var users = _userRepo.GetAllUsers();

            if (users == null)
            {
                return new JsonResult(new List<User>());
            }
            return new JsonResult(users);
        }
        catch (Exception ex)
        {
            return new JsonResult(new List<User>());
        }
    }
    #endregion

    #region Reset Password
    [HttpPost("reset-password")]
    public JsonResult ResetPassword(string email, string newPassword)
    {
        try
        {
            bool result = _userRepo.ResetPassword(email, newPassword);
            if (!result)
            {
                return new JsonResult(new {
                    success = false,
                    message = "User does not exist"
                });
            }
            return new JsonResult(new
            {
                success = true,
                message = "Password updated successfully"
            });
        }
        catch (Exception)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Error occurred"
            });
        }
    }
    #endregion
}

