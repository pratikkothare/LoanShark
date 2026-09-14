using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Xunit;

namespace Loan_Prediction_Tests
{
    [Collection("Database Collection")]
    public class LoanRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly LoanRepository _repo;

        public LoanRepositoryTests(DatabaseFixture fixture)
        {
            _repo = new LoanRepository(fixture.Context);
        }

    

        [Fact]
        public void ApplyLoan_Valid_ReturnsApplicationId()
        {
            var loan = new LoanApplication
            {
                UserId = 2,
                ProductId = 1,
                LoanAmount = 100000,
                TenureMonths = 12,
                Status = "Pending",
                RejectionReason = null
            };

            var result = _repo.ApplyLoanSP(loan);

            Assert.True(result > 0);
        }


        [Fact]
        public void GetLoanProducts_ReturnsList()
        {
            var success = _repo.GetLoanProducts(out List<LoanProduct> products);

            Assert.True(success);
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
        }


        [Fact]
        public void GetLoansByUser_ValidUser_ReturnsLoans()
        {
            var result = _repo.GetLoansByUser(2);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void GetLoansByUser_InvalidUser_ReturnsEmpty()
        {
            var result = _repo.GetLoansByUser(999);

            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }


        [Fact]
        public void GetAllLoans_ReturnsList()
        {
            var result = _repo.GetAllLoans();

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }


        [Fact]
        public void GetAllLoansForAdmin_ReturnsData()
        {
            var result = _repo.GetAllLoansForAdmin();

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }


        [Fact]
        public void UpdateLoanStatus_Valid_UpdatesSuccessfully()
        {
            var loans = _repo.GetLoansByUser(2);
            var loanId = loans.First().ApplicationId;

            _repo.UpdateLoanStatus(loanId, "Approved");

            var updated = _repo.GetLoansByUser(2)
                                           .First(x => x.ApplicationId == loanId);

            Assert.Equal("Approved", updated.Status);
        }


        [Fact]
        public void AddLoanProduct_Valid_ReturnsTrue()
        {
            var product = new LoanProduct
            {
                ProductName = "Test Loan",
                InterestRate = 10,
                MaxAmount = 500000,
                TenureMonths = 12
            };

            var result = _repo.AddLoanProduct(product);

            Assert.True(result);
        }


        [Fact]
        public void EditLoanProduct_Valid_ReturnsTrue()
        {
            _repo.GetLoanProducts(out List<LoanProduct> products);

            var product = products.First();

            product.ProductName = "Updated Loan";

            var result = _repo.EditLoanProduct(product);

            Assert.True(result);
        }


        [Fact]
        public void DeleteLoanProduct_Valid_ReturnsTrue()
        {
            var product = new LoanProduct
            {
                ProductName = "Delete Loan",
                InterestRate = 9,
                MaxAmount = 200000,
                TenureMonths = 6
            };

            _repo.AddLoanProduct(product);

            _repo.GetLoanProducts(out List<LoanProduct> products);

            var added = products.Last();

            var result = _repo.DeleteLoanProduct(added.ProductId);

            Assert.True(result);
        }


        [Fact]
        public void GetSummary_ReturnsData()
        {
            var result = _repo.GetSummary();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetProductPerformance_ReturnsList()
        {
            var result = _repo.GetProductPerformance();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetStatusDistribution_ReturnsData()
        {
            var result = _repo.GetStatusDistribution();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetApplicationTrends_ReturnsData()
        {
            var result = _repo.GetApplicationTrends();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetLoanDistribution_ReturnsData()
        {
            var result = _repo.GetLoanDistribution();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserInsights_ReturnsData()
        {
            var result = _repo.GetUserInsights();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetRecentActivity_ReturnsList()
        {
            var result = _repo.GetRecentActivity();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetTopUsers_ReturnsList()
        {
            var result = _repo.GetTopUsers();

            Assert.NotNull(result);
        }
    }
}
         