using LibraryManagerPro.Models;

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace LibraryManagerPro.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
            
        }
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<LivroAutor> LivroAutores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LivroAutor>()
                .HasKey(la => new { la.LivroId, la.AutorId });

            modelBuilder.Entity<LivroAutor>()
                .HasOne(la => la.Livro)
                .WithMany(l => l.Autores)
                .HasForeignKey(la => la.LivroId);

            modelBuilder.Entity<LivroAutor>()
                .HasOne(la => la.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(la => la.AutorId);

            modelBuilder.Entity<Autor>()
                .HasIndex(a => new { a.Nome, a.DataNascimento })
                .IsUnique();
        }
        



    }
}
