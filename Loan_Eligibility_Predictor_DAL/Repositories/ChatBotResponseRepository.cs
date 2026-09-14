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
    public class ChatBotResponseRepository
    {
        private readonly AppDbContext _context;
        public ChatBotResponseRepository(AppDbContext context)
        {
            _context = context;
        }

        #region GetChatbotResponse
        public string GetChatbotResponse(string query)
        {
            try
            {
                var result = _context.Set<ChatbotRequest>()
                    .FromSqlRaw("EXEC sp_GetChatbotResponse @UserQuery",
                        new SqlParameter("@UserQuery", query)).AsEnumerable().FirstOrDefault();
                return result?.Prompt;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
