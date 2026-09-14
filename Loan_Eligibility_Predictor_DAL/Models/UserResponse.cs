using System;
using System.Collections.Generic;

namespace Loan_Eligibility_Predictor_DAL.Models;

public class UserResponse
{
    public string FullName { get; set; }
    public int UserId { get; set; } 

    public string Email { get; set; }

    public int RoleId {get; set;}
    public string Message { get; set; }
}
