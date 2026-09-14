using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class LoanProduct
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal? InterestRate { get; set; }

    public decimal? MaxAmount { get; set; }

    public int? TenureMonths { get; set; }

    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();
}
