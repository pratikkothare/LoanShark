using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public int? UserId { get; set; }

    public int? ApplicationId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual LoanApplication? Application { get; set; }

    public virtual User? User { get; set; }
}
