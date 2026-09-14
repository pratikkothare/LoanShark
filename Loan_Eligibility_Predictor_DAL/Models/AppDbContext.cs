using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatbotLog> ChatbotLogs { get; set; }

    public virtual DbSet<ChatbotRule> ChatbotRules { get; set; }

    public virtual DbSet<CreditScore> CreditScores { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<EligibilityCheck> EligibilityChecks { get; set; }

    public virtual DbSet<LoanApplication> LoanApplications { get; set; }

    public virtual DbSet<LoanProduct> LoanProducts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserResponse> UserResponses { get; set; }

    public virtual DbSet<LoanResponse> LoanResponses { get; set; }

    public virtual DbSet<WithdrawResponse> WithdrawResponses { get; set; }

    public virtual DbSet<NotificationResponse> NotificationResponses { get; set; }

    public virtual DbSet<ChatbotRequest> ChatbotResponses { get; set; }

    public virtual DbSet<RegisterResponse> RegisterResponses { get; set; }

    public DbSet<AuthResponse> AuthResponses { get; set; }

    public virtual DbSet<DocumentResponse> DocumentResponses { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("LoanEligibilityDBConnectionString"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthResponse>().HasNoKey();
        modelBuilder.Entity<UserResponse>().HasNoKey();
        modelBuilder.Entity<LoanResponse>().HasNoKey();
        modelBuilder.Entity<NotificationResponse>().HasNoKey();
        modelBuilder.Entity<ChatbotRequest>().HasNoKey();
        modelBuilder.Entity<WithdrawResponse>().HasNoKey();
        modelBuilder.Entity<RegisterResponse>().HasNoKey();
        modelBuilder.Entity<DocumentResponse>().HasNoKey();
        modelBuilder.Entity<ChatbotLog>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatbotL__A9FBE7C69AD763D2");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Question)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Response)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChatbotRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__ChatbotR__110458E220D21C83");

            entity.Property(e => e.Keyword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Response)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CreditScore>(entity =>
        {
            entity.HasKey(e => e.CreditScoreId).HasName("PK__CreditSc__145DEC85E5827383");

            entity.Property(e => e.CheckedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.CreditScores)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CreditSco__UserI__38996AB5");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__1ABEEF0FC506B62C");

            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Application).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK__Documents__Appli__3D5E1FD2");

            entity.HasOne(d => d.User).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Documents__UserI__3C69FB99");
        });

        modelBuilder.Entity<EligibilityCheck>(entity =>
        {
            entity.HasKey(e => e.CheckId).HasName("PK__Eligibil__86815766E0990D59");

            entity.Property(e => e.CheckedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExistingLoans).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Income).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ResultMessage)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.EligibilityChecks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Eligibili__UserI__34C8D9D1");
        });

        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__LoanAppl__C93A4C99757FC4C8");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LoanAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Product).WithMany(p => p.LoanApplications)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__LoanAppli__Produ__30F848ED");

            entity.HasOne(d => d.User).WithMany(p => p.LoanApplications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__LoanAppli__UserI__300424B4");
        });

        modelBuilder.Entity<LoanProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__LoanProd__B40CC6CD512F6D3B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12029BE669");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4222D4EF");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AE7D6B105");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C4E03611D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341E6372CD").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleId__286302EC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
