using Loan_Eligibility_Predictor_DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Loan_Eligibility_Predictor_DAL.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        #region RegisterUser
        public bool RegisterUser(string name, string email, string password)
        {
            try
            {
                var result = _context.Set<RegisterResponse>()
                                .FromSqlRaw("EXEC sp_RegisterUser @FullName, @Email, @PasswordHash",
                                             new SqlParameter("@FullName", name),
                                             new SqlParameter("@Email", email),
                                             new SqlParameter("@PasswordHash", password)).AsEnumerable().FirstOrDefault();
                return result != null && result.UserId > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  LoginUser
        public UserResponse LoginUser(string email, string password)
        {
            try
            {
                var result = _context.Set<UserResponse>()
                                .FromSqlRaw("EXEC sp_LoginUser @Email, @PasswordHash",
                                             new SqlParameter("@Email", email),
                                             new SqlParameter("@PasswordHash", password)).AsEnumerable().FirstOrDefault();
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region GetUserByEmail
        public bool GetUserByEmail(string email,out User user)
        {
            user = new User();
            try
            {
                user =  _context.Users.FirstOrDefault(u => u.Email == email);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error:"+ex.Message);
                return false;
            }            
        }
        #endregion

        #region AddUser
        public bool AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }
        #endregion

        #region GetUserById
        public bool GetUserById(int userId,out User user)
        {
            user = new User();
            try
            {
                user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }            
        }
        #endregion

        #region GetCreditScoreByUserId
        public CreditScore GetCreditScoreByUserId(int userId)
        {
            CreditScore response = new CreditScore();
            try
            {
                var result = _context.CreditScores
                                .FromSqlInterpolated($"EXEC sp_GetCreditScoreByUserId @UserId = {userId}").AsNoTracking().AsEnumerable()
                                .FirstOrDefault();
                if (result != null)
                {
                    response = result;
                }
            }
            catch (Exception)
            {
                response = new CreditScore();
            }
            return response;
        }
        #endregion

        #region GetUser
        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }   
        }
        #endregion

        #region Reset Password
        public bool ResetPassword(string email, string newPassword)

        {
            try {

                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)

                    return false;

                user.PasswordHash = newPassword; // simple logic 

                _context.SaveChanges();
                
                return true;

            }

            catch (Exception)
            {
                return false;
            }

            }

        #endregion

       
    }
}