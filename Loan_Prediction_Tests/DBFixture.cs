using Loan_Eligibility_Predictor_DAL.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Loan_Test_DB;Trusted_Connection=True;TrustServerCertificate=True;")
                                .Options;

        Context = new AppDbContext(options);

        // ❌ NO EnsureDeleted (permission issue)
        // Context.Database.EnsureDeleted();

        // ✅ Just ensure created
        Context.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        if (!Context.Roles.Any())
        {
            Context.Database.ExecuteSqlRaw(@"
                                INSERT INTO Roles (RoleName) VALUES ('Admin'), ('User');
                                
                                            INSERT INTO Users (FullName, Email, PasswordHash, RoleId)
                                                        VALUES 
                                                                    ('Admin User', 'admin@test.com', '12345678', 1),
                                                                                ('Nitesh Mishra', 'user1@test.com', '12345678', 2),
                                                                                            ('Rahul Sharma', 'user2@test.com', '12345678', 2);
                                                                                            
                                                                                                        INSERT INTO CreditScores (UserId, Score)
                                                                                                                    VALUES
                                                                                                                                (2, 720),
                                                                                                                                            (3, 600);
                                                                                                                                                    ");
        }
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
             