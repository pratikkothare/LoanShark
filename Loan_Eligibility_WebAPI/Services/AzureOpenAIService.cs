using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Loan_Eligibility_WebAPI.Services
{
    public class AzureOpenAIService
    {
        private readonly IConfiguration _config;

        public AzureOpenAIService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetResponse(string prompt)
        {
            string endpoint = _config["AzureOpenAI:Endpoint"];
            string key = _config["AzureOpenAI:Key"];
            string deployment = _config["AzureOpenAI:Deployment"];

            var client = new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            ChatClient chatClient =
                client.GetChatClient(deployment);

            var response = await chatClient.CompleteChatAsync(
                new SystemChatMessage(
                   "You are LoanIQ assistant helping users with loans."),
                new UserChatMessage(prompt)
            );

            return response.Value.Content[0].Text;
        }

        public async Task<string>ExtractNameFromText(string text)
        {
            string prompt = $"Extract only the person's full name from this text:\n{text}\nReturn only name.";
                return await GetResponse(prompt);
        }
    }
}