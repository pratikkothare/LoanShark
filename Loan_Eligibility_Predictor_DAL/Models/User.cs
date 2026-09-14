using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public int? RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<CreditScore> CreditScores { get; set; } = new List<CreditScore>();

    [JsonIgnore]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [JsonIgnore]
    public virtual ICollection<EligibilityCheck> EligibilityChecks { get; set; } = new List<EligibilityCheck>();

    [JsonIgnore]
    public virtual ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();

    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [JsonIgnore]
    public virtual Role Role { get; set; }
}
