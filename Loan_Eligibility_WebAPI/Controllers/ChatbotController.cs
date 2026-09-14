using Loan_Eligibility_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly AzureOpenAIService _service;

    public ChatbotController(AzureOpenAIService service)
    {
        _service = service;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(ChatbotRequest req)
    {
        var answer = await _service.GetResponse(req.Prompt);

        return Ok(new ChatbotRequest
        {
            Prompt = answer
        });
    }
}