


namespace LibraryManagerPro.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Nacionalidade { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }

        // Relação N:N
        public ICollection<LivroAutor> Livros { get; set; } = new List<LivroAutor>();
    }
}
