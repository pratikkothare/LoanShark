using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class ChatbotRule
{
    public int RuleId { get; set; }

    public string? Keyword { get; set; }

    public string? Response { get; set; }
}
