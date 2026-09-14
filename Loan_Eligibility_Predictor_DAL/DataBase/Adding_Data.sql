Use LoanEligibilityDB
Go
-- ROLES

INSERT INTO Roles (RoleName)

VALUES ('Admin'), ('User');

-- USERS
INSERT INTO Users (FullName, Email, PasswordHash, RoleId)

VALUES 

('Admin User', 'admin@test.com', '12345678', 1),

('Nitesh Mishra', 'user1@test.com', '12345678', 2),

('Rahul Sharma', 'user2@test.com', '12345678', 2),

('Aditya Jha', 'jha@test.com', '12345678', 2);


-- LOAN PRODUCTS
INSERT INTO LoanProducts (ProductName, InterestRate, MaxAmount, TenureMonths)

VALUES

('Home Loan', 8.5, 5000000, 240),

('Car Loan', 9.0, 1000000, 60),

('Personal Loan', 12.5, 500000, 36),

('Education Loan', 10, 500000000, 300),

('Gold Loan', 12.5, 500000000, 300);


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

(2, 1, 'aadhaar.pdf', 'https://loanstorage123.blob.core.windows.net/loandocuments/aadhaar/4_aadhaar.png'),

(2, 3, 'salary_slip.pdf', 'https://loanstorage123.blob.core.windows.net/loandocuments/aadhaar/4_aadhaar.png'),

(3, 2, 'pan_card.pdf', 'https://loanstorage123.blob.core.windows.net/loandocuments/aadhaar/4_aadhaar.png');



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



