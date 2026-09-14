using System;

using Loan_Eligibility_Predictor_DAL.Models;

using Loan_Eligibility_Predictor_DAL.Repositories;


class Program

{
    static void Main(string[] args)
    {
        Console.WriteLine("====== Loan Eligibility System ======\n");
        // Initialize
        var context = new AppDbContext();
        var userRepo = new UserRepository(context);
        var loanRepo = new LoanRepository(context);
        // ===== USER INPUT =====
        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();
        Console.Write("Enter Income: ");
        decimal income = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Enter Credit Score: ");

        int creditScore = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Loan Amount: ");

        decimal loanAmount = Convert.ToDecimal(Console.ReadLine());

        Console.Write("Enter Tenure (months): ");

        int tenure = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("\nChecking eligibility...\n");

        // ===== SAVE USER =====

        User user;
        bool result = repo.GetUserByEmail("test@gmail.com", out user);

        if (result && user != null)
        {
            Console.WriteLine("User Found: " + user.FullName);
        }
        else
        {
            Console.WriteLine("User Not Found");
        }

        // ===== CREATE LOAN =====

        var loan = new LoanApplication

        {

            UserId = user.UserId,

            ProductId = 1,

            LoanAmount = loanAmount,

            TenureMonths = tenure

        };


        // ===== OUTPUT =====

        Console.WriteLine("===== RESULT =====");

        Console.WriteLine($"Status: {loan.Status}");

        if (loan.Status == "Rejected")

        {

            Console.WriteLine($"Reason: {loan.RejectionReason}");

        }

        Console.WriteLine("\n===== Test Completed Successfully =====");

    }

}