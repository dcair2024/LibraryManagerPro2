namespace LibraryManagerPro.Models
{
    public class Livro
    {
        public int Id { get; set; }
        public string ? Titulo { get; set; }
        
        public string ? Descricao { get; set; }
        public int AnoPublicacao { get; set; }
        public decimal Preco { get; set; }

        public string? CapaUrl { get; set; }// URL da capa do livro, vai ser gerado.

        public ICollection<LivroAutor> Autores { get; set; } = new List<LivroAutor>();


    }
}
