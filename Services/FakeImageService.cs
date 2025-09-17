using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibraryManagerPro.Services
{
    public class FakeImageService : IImageService
    {
        private readonly HttpClient _http;
        private readonly Random _random = new Random();

        public FakeImageService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("http://localhost:5001");
        }

        public async Task<string> GerarCapaAsync(string titulo, string descricao)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/gerar-capa", new { titulo, descricao });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CapaResponse>();
                    return result?.Url ?? GerarFallbackUrl(titulo, descricao);
                }
            }
            catch
            {
                // Se o Flask estiver fora do ar, cai aqui
            }

            return GerarFallbackUrl(titulo, descricao);
        }

        private string GerarFallbackUrl(string titulo, string descricao)
        {
            // Usa o título e uma palavra aleatória da descrição
            var palavraChave = titulo.Split(' ').FirstOrDefault() ?? "book";
            var aleatorio = _random.Next(1000, 9999);

            // Usa Unsplash para pegar imagem aleatória relacionada
            return $"https://source.unsplash.com/500x700/?book,{palavraChave},{aleatorio}";
        }

        private class CapaResponse
        {
            public string Url { get; set; }
        }
    }
}
