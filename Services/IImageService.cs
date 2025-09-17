public interface IImageService
{
    Task<string> GerarCapaAsync(string titulo, string descricao);
}
