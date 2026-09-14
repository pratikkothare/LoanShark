using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class CreditScore
{
    public int CreditScoreId { get; set; }

    public int? UserId { get; set; }

    public int? Score { get; set; }

    public DateTime? CheckedAt { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
