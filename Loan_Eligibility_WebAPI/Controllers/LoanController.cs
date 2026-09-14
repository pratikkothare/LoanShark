using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Loan_Eligibility_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[ApiController]

[Route("api/[controller]")]

public class LoanController : ControllerBase
{
    private readonly LoanRepository _loanRepo;

    private readonly IBlobService _blobService;

    private readonly AzureOpenAIService _openAIService;

    private readonly AzureVisionService _visionService;

    public LoanController(LoanRepository loanRepo, IBlobService blobService, AzureOpenAIService openAIService,

     AzureVisionService visionService)
    {
        _loanRepo = loanRepo;
        _blobService = blobService;
        _openAIService = openAIService;
        _visionService = visionService;
    }

    #region ApplyLoan
    [HttpPost("apply")]
    [Consumes("multipart/form-data")]
    public async Task<JsonResult> ApplyLoan(
    [FromForm] int userId,
    [FromForm] int productId,
    [FromForm] decimal loanAmount,
    [FromForm] int tenureMonths,
    [FromForm] decimal income,
    [FromForm] int creditScore,
    IFormFile aadhaar,
    IFormFile pan,
    IFormFile salary,
    IFormFile bank)
    {
        try
        {
            var loan = new LoanApplication
            {
                UserId = userId,
                ProductId = productId,
                LoanAmount = loanAmount,
                TenureMonths = tenureMonths,
                CreatedAt = DateTime.Now
            };
            if (income < 25000)
            {
                loan.Status = "Rejected";
                loan.RejectionReason = "Income < 25000";
            }
            else if (creditScore < 650)
            {
                loan.Status = "Rejected";
                loan.RejectionReason = "Low credit score";
            }
            else
            {
                loan.Status = "Pending";
                loan.RejectionReason = "Approved by system";
            }

            int appId = _loanRepo.ApplyLoanSP(loan, null);

            var fileMap = new Dictionary<string, IFormFile>
        {
            { "aadhaar", aadhaar },
            { "pan", pan },
            { "salary", salary },
            { "bank", bank }
        };
            foreach (var item in fileMap)
            {
                var file = item.Value;
                if (file == null || file.Length == 0) continue;

                string folder = item.Key;
                string extension = Path.GetExtension(file.FileName);
                string fileName = $"{appId}_{folder}{extension}";

                var url = await _blobService.UploadFileAsync(file, folder, fileName);

                string docType = folder switch
                {
                    "aadhaar" => "Aadhaar",
                    "pan" => "PAN",
                    "salary" => "Salary",
                    "bank" => "Bank",
                    _ => "Other"
                };

                _loanRepo.AddDocumentSP(userId, appId, fileName, url, docType);
            }
            return new JsonResult(new
            {
                success = true,
                applicationId = appId,
                status = loan.Status
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }
    #endregion

    #region GetLoanProducts
    [HttpGet("products")]
    public JsonResult GetLoanProducts()
    {
        try
        {
            bool isSuccess = _loanRepo.GetLoanProducts(out List<LoanProduct> products);
            if (isSuccess)
            {
                return new JsonResult(new { success = true, data = products });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Error fetching loan products" });
            }
        }
        catch
        {
            return new JsonResult(new { success = false, message = "Something went wrong" });
        }
    }
    #endregion

    #region GetLoansByUser
    [HttpGet("getloans/{userId}")]
    public JsonResult GetLoansByUser(int userId)
    {
        try
        {
            var result = _loanRepo.GetLoansByUser(userId);

            if (result != null && result.Count > 0)
            {
                return new JsonResult(new { success = true, data = result });
            }
            else
            {
                return new JsonResult(new { success = false, message = "No loans found for this user" });
            }
        }
        catch
        {
            return new JsonResult(new { success = false, message = "Something went wrong" });
        }
    }
    #endregion

    #region GetAllLoans
    [HttpGet]
    public JsonResult GetAllLoans()
    {
        try
        {
            var loans = _loanRepo.GetAllLoans();
            if (loans == null || loans.Count == 0)
            {
                return new JsonResult(new { message = "No loan data found" });
            }
            return new JsonResult(loans);
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                message = "Something went wrong while fetching loans",
                error = ex.Message
            });
        }
    }
    #endregion

    #region EditLoanProduct
    [HttpPut("EditLoanProduct")]
    public JsonResult EditLoanProduct(LoanProduct product)
    {
        if (product == null)
        {
            return new JsonResult(new { message = "Invalid loan product data." }) { StatusCode = 400 };
        }
        try
        {
            bool isUpdated = _loanRepo.EditLoanProduct(product);

            if (isUpdated)
            {
                return new JsonResult(new { message = "Loan product updated successfully!", status = true });
            }
            else
            {
                return new JsonResult(new { message = "Loan product not found or update failed.", status = false }) { StatusCode = 404 };
            }
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = "Internal server error", detail = ex.Message }) { StatusCode = 500 };
        }
    }

    #endregion

    #region GetAllLoansForAdmin
    [HttpGet("admin")]
    public JsonResult GetAllLoansForAdmin()
    {
        try
        {
            var loans = _loanRepo.GetAllLoansForAdmin();
            if(loans== null || loans.Count==0)
            {
                return new JsonResult(new { message = "No loan data found" });
            }
            return new JsonResult(loans);
        }
        catch (Exception ex)
        {
            return new JsonResult(new { 
                message = "Something went wrong while fetching loans",
                error = ex.Message
            });
        }
    }
    #endregion

    #region UpdateLoanStatus
    [HttpPut("status/{applicationId}")]
    public JsonResult UpdateLoanStatus(int applicationId, string status)
    {
        try
        {
            if (string.IsNullOrEmpty(status))
            {
                return new JsonResult(new { message = "Invalid status" });
            }
            _loanRepo.UpdateLoanStatus(applicationId, status);
            return new JsonResult(new { message = "Status Updated" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = ex.Message });
        }
    }
    #endregion

    #region AddLoanProduct
    [HttpPost("product")]
    public JsonResult AddLoanProduct(LoanProduct product)
    {
        try
        {
            if (product == null)
                return new JsonResult(new { message = "Invalid data" });
            var result = _loanRepo.AddLoanProduct(product);
            if (!result)
                return new JsonResult(new { message = "Insert failed" });
            return new JsonResult(new { message = "Product added" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = ex.Message });
        }
    }
    #endregion

    #region EditLoanProduct
    [HttpPut("product/{id}")]
    public JsonResult EditLoanProduct(int id, LoanProduct product)
    {
        try
        {
            if (product == null)
            {
                return new JsonResult(new { message= "Invalid data"});
            }
            if(id != product.ProductId)
            {
                return new JsonResult(new { message = "ID mismatch" });
            }

                var result = _loanRepo.EditLoanProduct(product);
            if (!result)
            {
                return new JsonResult(new { message= "Product not found"});
            }

            return new JsonResult(new { succes = true, message = "Product updated" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = ex.Message });
        }
    }
    #endregion

    #region DeleteLoanProduct
    [HttpDelete("product/{id}")]
    public JsonResult DeleteLoanProduct(int id)
    {
        try
        {
            if (id <= 0)
                return new JsonResult(new { message = "Invalid ID" });

            var result = _loanRepo.DeleteLoanProduct(id);

            if (!result)
                return new JsonResult(new { message = "Delete failed" });

            return new JsonResult(new { message = "Product deleted" });
        }
        catch (Exception ex)
        {
            return new JsonResult(500, new { message = ex.Message });
        }
    }
    #endregion

    #region Analytics APIs

    //SUMMARY
    [HttpGet("analytics/summary")]
    public JsonResult GetSummary()
    {
        try
        {
            var data = _loanRepo.GetSummary();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    //APPLICATION TRENDS
    [HttpGet("analytics/application-trends")]
    public JsonResult GetApplicationTrends()
    {
        try
        {
            var data = _loanRepo.GetApplicationTrends();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    //STATUS DISTRIBUTION
    [HttpGet("analytics/status-distribution")]
    public JsonResult GetStatusDistribution()
    {
        try
        {
            var data = _loanRepo.GetStatusDistribution();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    // LOAN AMOUNT DISTRIBUTION
    [HttpGet("analytics/loan-amount-distribution")]
    public JsonResult GetLoanDistribution()
    {
        try
        {
            var data = _loanRepo.GetLoanDistribution();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    [HttpGet("analytics/user-insights")]
    public JsonResult GetUserInsights()
    {
        try
        {
            var data = _loanRepo.GetUserInsights();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    [HttpGet("recent-activity")]
    public JsonResult GetRecentActivity()
    {
        try
        {
            var data = _loanRepo.GetRecentActivity();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("top-users")]
    public JsonResult GetTopUsers()
    {
        try
        {
            var data = _loanRepo.GetTopUsers();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("product-performance")]
    public JsonResult GetProductPerformance()
    {
        try
        {
            var data = _loanRepo.GetProductPerformance();
            return new JsonResult(new { success = true, data });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region DownloadDocuments
    [HttpGet("download-documents/{appId}")]
    public async Task<IActionResult> DownloadDocuments(int appId)
    {
        try
        {
            var urls = _loanRepo.GetDocumentUrls(appId);

            if (urls == null || urls.Count == 0)
            {
                return NotFound(new { message = "No documents found" });
            }

            using var memoryStream = new MemoryStream();

            using (var archive = new System.IO.Compression.ZipArchive(
                            memoryStream,
                                        System.IO.Compression.ZipArchiveMode.Create,
                                                    true))
            {
                using var http = new HttpClient();

                foreach (var url in urls)
                {
                    if (string.IsNullOrEmpty(url)) continue;

                    try
                    {
                        var fileName = Path.GetFileName(new Uri(url).LocalPath);
                        var entry = archive.CreateEntry(fileName);

                        using var entryStream = entry.Open();

                        var bytes = await http.GetByteArrayAsync(url);
                        await entryStream.WriteAsync(bytes, 0, bytes.Length);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", $"{appId}_Documents.zip");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error while downloading documents",
                error = ex.Message
            });
        }
    }
    #endregion

    #region Withdraw Loan
    [HttpPost("withdraw")]
    public JsonResult WithdrawLoan([FromBody] int applicationId)
    {
        try
        {
            var result = _loanRepo.WithdrawLoanSP(applicationId);

            return new JsonResult(new
            {
                success = result.Success,
                message = result.Message
            });
        }
        catch
        {
            return new JsonResult(new
            {
                success = false,
                message = "Something went wrong while withdrawing loan"
            });
        }
    }
    #endregion

    #region Verify Docs
    [HttpPost("verify-docs/{applicationId}")]
    public async Task<JsonResult> VerifyDocs(int applicationId)
    {
        try
        {
            string userName = _loanRepo.GetUserNameByApplicationId(applicationId);
            var urls = _loanRepo.GetDocumentUrls(applicationId);

            if (_visionService == null)
                return new JsonResult(new { message = "VisionService NULL" });

            if (_openAIService == null)
                return new JsonResult(new { message = "OpenAIService NULL" });

            if (urls == null)
                return new JsonResult(new { message = "URLs NULL" });

            if (userName == null)
                return new JsonResult(new { message = "UserName NULL" });


            if (urls == null || urls.Count == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No documents found"
                });
            }

            string combinedText = "";

            foreach (var url in urls)
            {
                var text = await _visionService.ExtractText(url);
                combinedText += text + " ";
            }

            // OPTIONAL: OpenAI se name extract
            string extractedName = await _openAIService.ExtractNameFromText(combinedText);

            bool matched =
                extractedName.ToLower()
                .Contains(userName.ToLower());

            return new JsonResult(new
            {
                success = true,
                verified = matched,
                userName,
                extractedName,
                rawText = combinedText
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
#endregion
}