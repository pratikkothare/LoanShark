Use LoanEligibilityDB
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

  @RejectionReason  NVARCHAR(255),

  @DocumentUrl NVARCHAR(500)

AS

BEGIN

  SET NOCOUNT ON;

  INSERT INTO LoanApplications

  (UserId, ProductId, LoanAmount, TenureMonths, Status, CreatedAt, RejectionReason, DocumentUrl)

  VALUES

  (@UserId, @ProductId, @LoanAmount, @TenureMonths, @Status, GETDATE(), @RejectionReason, @DocumentUrl);

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
      RejectionReason,
      DocumentUrl
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

-- UPDATE LOAN PRODUCTS
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

-- ADD LOAN PRODUCTS
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
Go

CREATE or ALTER PROCEDURE AddDocumentSP
    @UserId INT,
    @ApplicationId INT,
    @FileName VARCHAR(255),
    @FilePath VARCHAR(500),
    @DocumentType VARCHAR(50)
    AS
    BEGIN
    SET NOCOUNT ON;

    INSERT INTO Documents (
    UserId,
    ApplicationId,
    FileName,
    FilePath,
    DocumentType,
    UploadedAt
    )
    VALUES (
    @UserId,
    @ApplicationId,
    @FileName,
    @FilePath,
    @DocumentType,
    GETDATE()
    );
END
Go

-- WITHDRAW APPLIED LOAN
CREATE OR ALTER PROCEDURE sp_WithdrawLoanApplication
    @ApplicationId INT
    AS
    BEGIN
        SET NOCOUNT ON;

            -- Check if Application Exists
            IF NOT EXISTS (SELECT 1 FROM LoanApplications WHERE ApplicationId = @ApplicationId)
            BEGIN
            SELECT 0 AS Success,
                   'Loan application not found' AS Message
                   RETURN
            END
            -- Check Current Status
            DECLARE @CurrentStatus VARCHAR(50)
            SELECT @CurrentStatus = Status FROM LoanApplications
            WHERE ApplicationId = @ApplicationId
            -- Allow Withdraw only if Pending
            IF(@CurrentStatus = 'Pending')
            BEGIN
            UPDATE LoanApplications SET Status = 'Withdrawn' WHERE ApplicationId = @ApplicationId
            SELECT  1 AS Success,
            'Loan application withdrawn successfully' AS Message
            END
            ELSE
            BEGIN
            SELECT 0 AS Success,
            'Loan cannot be withdrawn because it is already processed' AS Message
            END
     END
GO
--EXEC sp_WithdrawLoanApplication 1
--SELECT ApplicationId, Status from LoanApplications