CREATE DATABASE LoanEligibilityDB;

GO

USE LoanEligibilityDB;

GO

-- 1. Roles Table

CREATE TABLE Roles (

    RoleId INT PRIMARY KEY IDENTITY,

    RoleName VARCHAR(50) NOT NULL -- Admin / User

);

-- 2. Users Table

CREATE TABLE Users (

    UserId INT PRIMARY KEY IDENTITY,

    FullName VARCHAR(100),

    Email VARCHAR(100) UNIQUE,

    PasswordHash VARCHAR(255),

    RoleId INT,

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)

);

-- 3. Loan Products (Admin Managed)

CREATE TABLE LoanProducts (

    ProductId INT PRIMARY KEY IDENTITY,

    ProductName VARCHAR(100),

    InterestRate DECIMAL(5,2),

    MaxAmount DECIMAL(18,2),

    TenureMonths INT,

    CreatedAt DATETIME DEFAULT GETDATE()

);

-- 4. Loan Applications

CREATE TABLE LoanApplications (

    ApplicationId INT PRIMARY KEY IDENTITY,

    UserId INT,

    ProductId INT,

    LoanAmount DECIMAL(18,2),

    TenureMonths INT,

    Status VARCHAR(50) DEFAULT 'Pending', -- Approved/Rejected

    CreatedAt DATETIME DEFAULT GETDATE(),

    RejectionReason NVARCHAR(255) NULL,

    DocumentUrl NVARCHAR(500),

    FOREIGN KEY (UserId) REFERENCES Users(UserId),

    FOREIGN KEY (ProductId) REFERENCES LoanProducts(ProductId)

);

-- 5. Eligibility Checks

CREATE TABLE EligibilityChecks (

    CheckId INT PRIMARY KEY IDENTITY,

    UserId INT,

    Income DECIMAL(18,2),

    CreditScore INT,

    ExistingLoans DECIMAL(18,2),

    Eligible BIT,

    ResultMessage VARCHAR(255),

    CheckedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)

);

-- 6. Credit Score

CREATE TABLE CreditScores (

    CreditScoreId INT PRIMARY KEY IDENTITY,

    UserId INT,

    Score INT,

    CheckedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)

);

-- 7. Documents Upload

CREATE TABLE Documents (

    DocumentId INT PRIMARY KEY IDENTITY,

    UserId INT,

    ApplicationId INT,

    FileName VARCHAR(255),

    FilePath VARCHAR(500),

    UploadedAt DATETIME DEFAULT GETDATE(),

    DocumentType VARCHAR(50),

    FOREIGN KEY (UserId) REFERENCES Users(UserId),

    FOREIGN KEY (ApplicationId) REFERENCES LoanApplications(ApplicationId)

);

-- 8. Notifications

CREATE TABLE Notifications (

    NotificationId INT PRIMARY KEY IDENTITY,

    UserId INT,

    Message VARCHAR(255),

    IsRead BIT DEFAULT 0,

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId)

);

-- 9. Chatbot Logs (Rule-based)

CREATE TABLE ChatbotLogs (

    ChatId INT PRIMARY KEY IDENTITY,

    UserId INT NULL,

    Question VARCHAR(500),

    Response VARCHAR(1000),

    CreatedAt DATETIME DEFAULT GETDATE()

);

