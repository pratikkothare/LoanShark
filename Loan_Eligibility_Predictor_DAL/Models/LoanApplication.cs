using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class LoanApplication
{
  
    public int ApplicationId { get; set; } 
    public int UserId { get; set; }

    public int ProductId { get; set; }

    public decimal LoanAmount { get; set; }

    public int TenureMonths { get; set; }

    public string Status { get; set; }

    public string RejectionReason { get; set; }

    public string DocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public User User { get; set; }

    [JsonIgnore]
    public LoanProduct Product { get; set; }

    [JsonIgnore]
    public ICollection<Document> Documents { get; set; }
}
