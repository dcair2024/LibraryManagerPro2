namespace LibraryManagerPro.Services
{
    public interface IImageService
    {
        Task<string> GerarImagemAsync(string prompt);
    }
}
