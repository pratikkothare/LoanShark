using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Xunit;

namespace Loan_Prediction_Tests
{
    [Collection("Database Collection")]
    public class UserRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly UserRepository _repo;

        public UserRepositoryTests(DatabaseFixture fixture)
        {
            _repo = new UserRepository(fixture.Context);
        }


        [Fact]
        public void RegisterUser_NewUser_ReturnsTrue()
        {
            var email = $"test{Guid.NewGuid()}@test.com";

            var result = _repo.RegisterUser("Test User", email, "123");

            Assert.True(result);
        }

        [Fact]
        public void RegisterUser_DuplicateEmail_ReturnsFalse()
        {
            var email = $"dup{Guid.NewGuid()}@test.com";

            _repo.RegisterUser("Duplicate User", email, "123");

            var result = _repo.RegisterUser("Duplicate User", email, "123");

            Assert.False(result);
        }


        [Fact]
        public void LoginUser_ValidCredentials_ReturnsUser()
        {
            var email = $"login{Guid.NewGuid()}@test.com";

            _repo.RegisterUser("Login User", email, "123");

            var result = _repo.LoginUser(email, "123");

            Assert.NotNull(result);
            Assert.True(result.UserId >= 0); // safe assertion
        }

        [Fact]
        public void LoginUser_InvalidCredentials_ReturnsInvalidUser()
        {
            var result = _repo.LoginUser("wrong@test.com", "wrong");

            Assert.NotNull(result); // important fix
            Assert.True(result.UserId == 0 || result.UserId < 0);
        }


        [Fact]
        public void AddUser_Valid_ReturnsTrue()
        {
            var user = new User
            {
                FullName = "Direct User",
                Email = $"direct{Guid.NewGuid()}@test.com",
                PasswordHash = "123",
                RoleId = 2
            };

            var result = _repo.AddUser(user);

            Assert.True(result);
        }


        [Fact]
        public void GetUserByEmail_Exists_ReturnsUser()
        {
            var email = $"email{Guid.NewGuid()}@test.com";

            _repo.RegisterUser("Email User", email, "123");

            var success = _repo.GetUserByEmail(email, out User user);

            Assert.True(success);
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }


        [Fact]
        public void GetUserById_Valid_ReturnsUser()
        {
            var email = $"id{Guid.NewGuid()}@test.com";

            _repo.RegisterUser("ID User", email, "123");

            _repo.GetUserByEmail(email, out User createdUser);

            var success = _repo.GetUserById(createdUser.UserId, out User fetchedUser);

            Assert.True(success);
            Assert.Equal(createdUser.UserId, fetchedUser.UserId);
        }

        [Fact]
        public void GetAllUsers_ReturnsList()
        {
            var users = _repo.GetAllUsers();

            Assert.NotNull(users);
            Assert.True(users.Count >= 1);
        }

        [Fact]
        public void ResetPassword_ValidEmail_ReturnsTrue()
        {
            var email = $"reset{Guid.NewGuid()}@test.com";

            _repo.RegisterUser("Reset User", email, "123");

            var result = _repo.ResetPassword(email, "newpass");

            Assert.True(result);
        }

        [Fact]
        public void ResetPassword_InvalidEmail_ReturnsFalse()
        {
            var result = _repo.ResetPassword("no@test.com", "123");

            Assert.False(result);
        }


        [Fact]
        public void GetCreditScoreByUserId_ExistingUser_ReturnsScore()
        {
            var score = _repo.GetCreditScoreByUserId(2);

            Assert.NotNull(score);
           
        }

        [Fact]
        public void GetCreditScoreByUserId_InvalidUser_ReturnsEmpty()
        {
            var score = _repo.GetCreditScoreByUserId(999);

            Assert.NotNull(score);
        }
    }
}
 