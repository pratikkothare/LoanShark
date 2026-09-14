using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class EligibilityCheck
{
    public int CheckId { get; set; }

    public int? UserId { get; set; }

    public decimal? Income { get; set; }

    public int? CreditScore { get; set; }

    public decimal? ExistingLoans { get; set; }

    public bool? Eligible { get; set; }

    public string? ResultMessage { get; set; }

    public DateTime? CheckedAt { get; set; }

    public virtual User? User { get; set; }
}
