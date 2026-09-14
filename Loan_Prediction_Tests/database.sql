

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

-- ROLES

INSERT INTO Roles (RoleName)

VALUES ('Admin'), ('User');

-- USERS
INSERT INTO Users (FullName, Email, PasswordHash, RoleId)

VALUES 

('Admin User', 'admin@test.com', '12345678', 1),

('Nitesh Mishra', 'user1@test.com', '12345678', 2),

('Rahul Sharma', 'user2@test.com', '12345678', 2);


-- LOAN PRODUCTS
INSERT INTO LoanProducts (ProductName, InterestRate, MaxAmount, TenureMonths)

VALUES

('Home Loan', 8.5, 5000000, 240),

('Car Loan', 9.0, 1000000, 60),

('Personal Loan', 12.5, 500000, 36);


-- LOAN APPLICATIONS
INSERT INTO LoanApplications (UserId, ProductId, LoanAmount, TenureMonths, Status)

VALUES

(2, 1, 2000000, 180, 'Pending'),

(3, 2, 500000, 48, 'Pending'),

(2, 3, 100000, 24, 'Rejected');


-- ELIGIBILITY CHECKS
INSERT INTO EligibilityChecks (UserId, Income, CreditScore, ExistingLoans, Eligible, ResultMessage)

VALUES

(2, 50000, 720, 10000, 1, 'Eligible for loan'),

(3, 20000, 600, 20000, 0, 'Low income and credit score'),

(2, 40000, 680, 5000, 1, 'Eligible with conditions');


-- CREDIT SCORES
INSERT INTO CreditScores (UserId, Score)

VALUES

(2, 720),

(3, 600);


-- DOCUMENTS
INSERT INTO Documents (UserId, ApplicationId, FileName, FilePath)

VALUES

(2, 1, 'aadhaar.pdf', '/docs/aadhaar_user2.pdf'),

(2, 3, 'salary_slip.pdf', '/docs/salary_user2.pdf'),

(3, 2, 'pan_card.pdf', '/docs/pan_user3.pdf');



-- NOTIFICATIONS
INSERT INTO Notifications (UserId, Message, IsRead)

VALUES

(2, 'Your loan application is under review', 0),

(3, 'Your loan has been approved', 1),

(2, 'Your loan was rejected due to eligibility', 0);



-- CHATBOT LOGS
INSERT INTO ChatbotLogs (UserId, Question, Response)

VALUES

(2, 'What is eligibility?', 'You need income > 25k and credit score > 650'),

(NULL, 'What loans do you offer?', 'We offer home, car, and personal loans');
Go

-- USER REGISTER
CREATE OR ALTER PROCEDURE sp_RegisterUser

  @FullName NVARCHAR(100),

  @Email NVARCHAR(100),

  @PasswordHash NVARCHAR(200)

AS

BEGIN

  SET NOCOUNT ON;

  IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)

  BEGIN

      SELECT

          0 AS UserId,

          'User already registered. Please login.' AS Message;

      RETURN;

  END

  INSERT INTO Users (FullName, Email, PasswordHash, RoleId, CreatedAt)

  VALUES (@FullName, @Email, @PasswordHash, 2, GETDATE());

  SELECT

      CAST(SCOPE_IDENTITY() AS INT) AS UserId,

      'User registered successfully. Please login.' AS Message;

END

GO




-- USER LOGIN
CREATE OR ALTER PROCEDURE sp_LoginUser
 @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200)
    AS
    BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    SELECT @UserId = UserId
    FROM Users
    WHERE Email = @Email AND PasswordHash = @PasswordHash;

    IF @UserId IS NULL
    BEGIN
        SELECT 
         0 AS UserId,
		 '' AS FullName,
        '' AS Email,
         0 AS RoleId,
        'Invalid email or password' AS Message;
          END
     ELSE
     BEGIN
 SELECT 
 UserId,
 FullName,
 Email,
 RoleId,
'Login successful' AS Message
 FROM Users
 WHERE UserId = @UserId;
 END
 END
 GO


-- APPLY LOAN
CREATE OR ALTER PROCEDURE sp_ApplyLoan

  @UserId INT,

  @ProductId INT,

  @LoanAmount DECIMAL(18,2),

  @TenureMonths INT,

  @Status NVARCHAR(50),

  @RejectionReason  NVARCHAR(255)

AS

BEGIN

  SET NOCOUNT ON;

  INSERT INTO LoanApplications

  (UserId, ProductId, LoanAmount, TenureMonths, Status, CreatedAt, RejectionReason)

  VALUES

  (@UserId, @ProductId, @LoanAmount, @TenureMonths, @Status, GETDATE(), @RejectionReason);

  SELECT

      CAST(SCOPE_IDENTITY() AS INT) AS ApplicationId,

      'Loan applied successfully' AS Message;

END

GO


-- GET LOANS BY USER
CREATE OR ALTER PROCEDURE sp_GetLoansByUser
  @UserId INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
      ApplicationId,
      ProductId,
      UserId,
      LoanAmount,
      TenureMonths,
      Status,
      CreatedAt,
      RejectionReason
  FROM LoanApplications
  WHERE UserId = @UserId;
END
GO

-- ADD NOTIFICATION
CREATE OR ALTER PROCEDURE sp_AddNotification

  @UserId INT,

  @Message NVARCHAR(200)

AS

BEGIN

  SET NOCOUNT ON;

  INSERT INTO Notifications (UserId, Message, CreatedAt)

  VALUES (@UserId, @Message, GETDATE());

  SELECT

      1 AS Result,

      'Notification sent' AS Message;

END

GO

-- CHATBOT RESPONSE (RULE BASED)
CREATE OR ALTER PROCEDURE sp_GetChatbotResponse

  @UserQuery NVARCHAR(200)

AS

BEGIN

  SET NOCOUNT ON;

  DECLARE @Response NVARCHAR(500);

  IF @UserQuery LIKE '%loan%'

      SET @Response = 'You can apply for loan using Apply Loan option.';

  ELSE IF @UserQuery LIKE '%status%'

      SET @Response = 'Check your loan status in dashboard.';

  ELSE

      SET @Response = 'Sorry, I did not understand your query.';

  SELECT @Response AS Response;

END

GO

--  GetLoanProducts
CREATE or ALTER PROCEDURE sp_GetLoanProducts
AS
BEGIN
     SET NOCOUNT ON;
     SELECT
        ProductId,
        ProductName,
        InterestRate,
        MaxAmount,
        TenureMonths,
        CreatedAt
    FROM LoanProducts
END
GO

CREATE or ALTER PROCEDURE sp_GetCreditScoreByUserId
    @UserId INT
    AS
    BEGIN
        SELECT TOP 1
        CreditScoreId,
        UserId,
        Score,
        CheckedAt
        FROM dbo.CreditScores
        WHERE UserId = @UserId
        ORDER BY CheckedAt DESC
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateLoanProduct
    @ProductId INT,
    @ProductName VARCHAR(100),
    @InterestRate DECIMAL(5,2),
    @MaxAmount DECIMAL(18,2),
    @TenureMonths INT
AS
BEGIN
    SET NOCOUNT OFF;
    
    UPDATE LoanProducts
    SET ProductName = @ProductName,
        InterestRate = @InterestRate,
        MaxAmount = @MaxAmount,
        TenureMonths = @TenureMonths
    WHERE ProductId = @ProductId;

    -- Return the number of rows affected
    SELECT @@ROWCOUNT;
END
GO

CREATE PROCEDURE sp_AddLoanProduct
    @ProductName NVARCHAR(100),
    @InterestRate DECIMAL(10,2),
    @MaxAmount DECIMAL(18,2),
    @TenureMonths INT
AS
BEGIN
    INSERT INTO LoanProducts (ProductName, InterestRate, MaxAmount, TenureMonths, CreatedAt)
    VALUES (@ProductName, @InterestRate, @MaxAmount, @TenureMonths, GETDATE())
END
GO

CREATE PROCEDURE sp_DeleteLoanProduct
    @ProductId INT
AS
BEGIN
    DELETE FROM LoanProducts
    WHERE ProductId = @ProductId
END