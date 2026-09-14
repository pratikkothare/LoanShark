using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class ChatbotLog
{
    public int ChatId { get; set; }

    public int? UserId { get; set; }

    public string? Question { get; set; }

    public string? Response { get; set; }

    public DateTime? CreatedAt { get; set; }
}
