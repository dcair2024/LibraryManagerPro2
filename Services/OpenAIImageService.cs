using LibraryManagerPro.Services;
using OpenAI;
using OpenAI.Images;

public class OpenAIImageService : IImageService 
{
    private readonly OpenAIClient _client;

    public OpenAIImageService(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"];
        Console.WriteLine($"[DEBUG] OpenAI API Key encontrada? {!string.IsNullOrEmpty(apiKey)}");

        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Chave da API OpenAI não encontrada. Verifique seu appsettings.json.");

        _client = new OpenAIClient(new OpenAIAuthentication(apiKey));
    }

    public async Task<string> GerarImagemAsync(string prompt)
    {
        try
        {
            var request = new ImageGenerationRequest(prompt);
            var result = await _client.ImagesEndPoint.GenerateImageAsync(request);
            var url = result.FirstOrDefault()?.Url;

            Console.WriteLine($"[OpenAIImageService] URL gerada: {url}");
            return url ?? "https://via.placeholder.com/300x400.png?text=Erro+na+API";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OpenAIImageService] Erro: {ex.Message}");
            return "https://via.placeholder.com/300x400.png?text=Erro+na+Geração";
        }
    }
}
