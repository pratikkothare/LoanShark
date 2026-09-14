using Loan_Eligibility_Predictor_DAL.Models;

using Microsoft.Data.SqlClient;

using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Loan_Eligibility_Predictor_DAL.Repositories
{
    public class LoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context)

        {
            _context = context;
        }

        #region ApplyLoan
    public int ApplyLoanSP(LoanApplication loan, string documentUrl)
        {
            try
            {
                using (var con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    using (var cmd = new SqlCommand("sp_ApplyLoan", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", loan.UserId);
                        cmd.Parameters.AddWithValue("@ProductId", loan.ProductId);
                        cmd.Parameters.AddWithValue("@LoanAmount", loan.LoanAmount);
                        cmd.Parameters.AddWithValue("@TenureMonths", loan.TenureMonths);
                        cmd.Parameters.AddWithValue("@Status", loan.Status);
                        cmd.Parameters.AddWithValue("@RejectionReason", (object)loan.RejectionReason ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DocumentUrl", (object)documentUrl ?? DBNull.Value);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetInt32(0);
                            }
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    #endregion

        #region GetLoanProducts
    public bool GetLoanProducts(out List<LoanProduct> products)
        {
            products = new List<LoanProduct>();
            try
            {
                products = _context.LoanProducts.FromSqlRaw("EXEC sp_GetLoanProducts").ToList();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region GetLoansByUser
        public List<object> GetLoansByUser(int userId)
        {
            try
            {
                var data = _context.LoanApplications
                    .Where(l => l.UserId == userId)
                    .Select(loan => new
                    {
                        applicationId = loan.ApplicationId,
                        loanAmount = loan.LoanAmount,
                        tenureMonths = loan.TenureMonths,
                        status = loan.Status,
                        productId = loan.ProductId,

                        documentUrl = _context.Documents
                            .Where(d => d.ApplicationId == loan.ApplicationId)
                            .Select(d => d.FilePath)
                            .FirstOrDefault()
                    })
                    .ToList<object>();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<object>();
            }
        }
        #endregion

        #region GetAllLoans
        public List<LoanApplication> GetAllLoans()
        {
            try
            {
                return _context.LoanApplications.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<LoanApplication>();
            }
        }
        #endregion

        #region GetAllLoansForAdmin
        public List<object> GetAllLoansForAdmin()
        {
            try
            {
                var data = _context.LoanApplications
                    .Select(loan => new
                    {
                        applicationId = loan.ApplicationId,
                        userId = loan.UserId,
                        userName = _context.Users
                                        .Where(u => u.UserId == loan.UserId)
                                        .Select(u => u.FullName)
                                        .FirstOrDefault(),

                        loanAmount = loan.LoanAmount,
                        status = loan.Status,
                        createdAt = loan.CreatedAt,

                        documentUrl = _context.Documents
                            .Where(d => d.ApplicationId == loan.ApplicationId)
                            .Select(d => d.FilePath)
                            .FirstOrDefault()
                    })
                    .ToList<object>();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<object>();
            }
        }
        #endregion

        #region UpdateLoanStatus
        public void UpdateLoanStatus(int applicationId, string status)
        {
            try
            {
                var loan = _context.LoanApplications.FirstOrDefault(x => x.ApplicationId == applicationId);
                if (loan == null)
                    throw new Exception("Loan not found");
                loan.Status = status;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }   
        }
        #endregion

        #region AddLoanProduct
        public bool AddLoanProduct(LoanProduct product)
        {
            try
            {
                var rows = _context.Database.ExecuteSqlRaw(
                    "EXEC sp_AddLoanProduct @ProductName, @InterestRate, @MaxAmount, @TenureMonths",
                    new SqlParameter("@ProductName", product.ProductName),
                    new SqlParameter("@InterestRate", product.InterestRate),
                    new SqlParameter("@MaxAmount", product.MaxAmount),
                    new SqlParameter("@TenureMonths", product.TenureMonths)
                );
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("DAL Error: could not add product: " + ex.Message);
            }
        }
        #endregion

        #region EditLoanProduct
        public bool EditLoanProduct(LoanProduct product)
        {
            try
            {
                var rowsAffected = _context.Database.ExecuteSqlRaw(
                  "EXEC sp_UpdateLoanProduct @ProductId, @ProductName, @InterestRate, @MaxAmount, @TenureMonths",
                                                new SqlParameter("@ProductId", product.ProductId),
                                                new SqlParameter("@ProductName", product.ProductName),
                                                new SqlParameter("@InterestRate", product.InterestRate),
                                                new SqlParameter("@MaxAmount", product.MaxAmount),
                                                new SqlParameter("@TenureMonths", product.TenureMonths)
                );
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("DAL Error: could not update loan product", ex);
            }
        }
        #endregion

        #region DeleteLoanProduct
        public bool DeleteLoanProduct(int productId)
        {
            try
            {
                var rows = _context.Database.ExecuteSqlRaw(
                    "EXEC sp_DeleteLoanProduct @ProductId",
                    new SqlParameter("@ProductId", productId)
                );
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("DAL Error: could not delete product: " + ex.Message);
            }
        }
        #endregion

        #region AdminAnalytics
        public object GetSummary()
        {
            var totalProducts = _context.LoanProducts.Count();
            var totalApplications = _context.LoanApplications.Count();

            var approved = _context.LoanApplications.Count(x => x.Status == "Approved");
            var rejected = _context.LoanApplications.Count(x => x.Status == "Rejected");

            var approvalRate = totalApplications == 0 ? 0 :
                (approved * 100.0 / totalApplications);

            var totalDisbursed = _context.LoanApplications
                .Where(x => x.Status == "Approved")
                .Sum(x => (decimal?)x.LoanAmount) ?? 0;

            return new
            {
                totalProducts,
                totalApplications,
                approved,
                rejected,
                approvalRate,
                totalDisbursed
            };
        }


        public List<object> GetProductPerformance()
        {
            try
            {
                var data = (from loan in _context.LoanApplications
                            join product in _context.LoanProducts
                            on loan.ProductId equals product.ProductId
                            group loan by product.ProductName into g
                            select new
                            {
                                productName = g.Key,
                                totalApplications = g.Count(),
                                approvalRate = g.Count(x => x.Status == "Approved") * 100.0 / g.Count()
                            }).ToList<object>();

                return data;
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        public object GetStatusDistribution()
        {
            return new
            {
                approved = _context.LoanApplications.Count(x => x.Status == "Approved"),
                rejected = _context.LoanApplications.Count(x => x.Status == "Rejected"),
                pending = _context.LoanApplications.Count(x => x.Status == "Pending")
            };
        }

        public object GetApplicationTrends()
        {
            return _context.LoanApplications
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key,
                    count = g.Count()
                })
                .OrderBy(x => x.date)
                .ToList();
        }

        public object GetLoanDistribution()
        {
            return new
            {
                range1 = _context.LoanApplications.Count(x => x.LoanAmount <= 100000),
                range2 = _context.LoanApplications.Count(x => x.LoanAmount > 100000 && x.LoanAmount <= 500000),
                range3 = _context.LoanApplications.Count(x => x.LoanAmount > 500000 && x.LoanAmount <= 1000000),
                range4 = _context.LoanApplications.Count(x => x.LoanAmount > 1000000)
            };
        }
        public object GetUserInsights()
        {
            return _context.LoanApplications
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    userId = g.Key,
                    totalApplications = g.Count(),
                    totalLoanAmount = g.Sum(x => (decimal?)x.LoanAmount) ?? 0
                })
                .OrderByDescending(x => x.totalApplications)
                .Take(5)
                .ToList();
        }

        public List<object> GetRecentActivity()
        {
            try
            {
                var data = GetAllLoansForAdmin()
                    .OrderByDescending(x => ((dynamic)x).createdAt)
                    .Take(10)
                    .ToList();

                return data;
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        public List<object> GetTopUsers()
        {
            try
            {
                var data = GetAllLoansForAdmin()
                    .GroupBy(x => new
                    {
                        userId = ((dynamic)x).userId,
                        userName = ((dynamic)x).userName
                    })
                    .Select(g => new
                    {
                        userId = g.Key.userId,
                        userName = g.Key.userName,
                        applications = g.Count(),
                        totalLoan = g.Sum(x => (decimal)((dynamic)x).loanAmount)
                    })
                    .OrderByDescending(x => x.applications)
                    .Take(5)
                    .ToList<object>();

                return data;
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }
        #endregion

        #region AddDocumentSP
        public void AddDocumentSP(int userId, int appId, string fileName, string filePath, string docType)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(
                "EXEC AddDocumentSP @UserId, @ApplicationId, @FileName, @FilePath, @DocumentType",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ApplicationId", appId),
                new SqlParameter("@FileName", fileName),
                new SqlParameter("@FilePath", filePath),
                new SqlParameter("@DocumentType", docType)
            );
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddDocumentSP Error" + ex.Message );
            }
            
        }
        #endregion

        #region GetDocumentUrls
        public List<string> GetDocumentUrls(int appId)
        {
            try
            {
                return _context.Documents.Where(d => d.ApplicationId == appId).Select(d => d.FilePath).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
        #endregion

        #region WithdrawAppliedLoan
        public WithdrawResponse WithdrawLoanSP(int applicationId)
        {
            try
            {
                var result = _context.Set<WithdrawResponse>().FromSqlRaw
                    ("EXEC sp_WithdrawLoanApplication @ApplicationId",
                      new SqlParameter("@ApplicationId", applicationId))
                    .AsEnumerable()
                    .FirstOrDefault();

                return result;
            }
            catch
            {
                return new WithdrawResponse
                {
                    Success = 0,
                    Message = "Error while withdrawing loan"
                };
            }
        }
        #endregion

        #region Verify Docs
        public string GetUserNameByApplicationId(int applivcationId)
        {
            return _context.LoanApplications.Where(l => l.ApplicationId == applivcationId).Select(l => l.User.FullName).FirstOrDefault();
        }
        #endregion
    }
}
