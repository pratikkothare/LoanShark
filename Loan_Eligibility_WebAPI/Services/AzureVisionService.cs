using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class AzureVisionService
{
    private readonly string endpoint = "https://loaniq-chatbot.cognitiveservices.azure.com/";
    private readonly string key;

    public AzureVisionService(IConfiguration configuration)
    {
        // Reads from User Secrets (locally) or Environment Variables / appsettings.json (production)
        key = configuration["AzureOpenAI:Key"] ?? "YOUR_AZURE_OPENAI_KEY_HERE";
    }

    public async Task<string> ExtractText(string imageUrl)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

        var body = new { url = imageUrl };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(
            $"{endpoint}/vision/v3.2/read/analyze",
            content
        );
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return "VISION API ERROR: " + error;
        }

        if (!response.Headers.TryGetValues("Operation-Location", out var values))
        {
            return "Operation-Location header missing. API failed.";
        }

        var opUrl = values.First();

        await Task.Delay(4000);

        var result = await client.GetAsync(opUrl);
        var json = await result.Content.ReadAsStringAsync();

        return ExtractTextFromJson(json);
    }

    private string ExtractTextFromJson(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("analyzeResult", out var analyze))
            return "No text found";

        var lines = analyze
            .GetProperty("readResults")[0]
            .GetProperty("lines");

        string text = "";

        foreach (var line in lines.EnumerateArray())
        {
            text += line.GetProperty("text").GetString() + " ";
        }

        return text;
    }
}