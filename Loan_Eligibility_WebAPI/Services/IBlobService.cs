namespace Loan_Eligibility_WebAPI.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder , string fileName);
    }
}
