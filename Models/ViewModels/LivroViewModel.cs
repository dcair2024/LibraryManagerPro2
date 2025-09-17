using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagerPro.Models.ViewModels
{
    public class LivroViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        [Display(Name = "Ano de Publicação")]
        public int AnoPublicacao { get; set; }

        [Display(Name = "Preço")]
        public decimal Preco { get; set; }


        [Display(Name = "URL da Capa")]
        public string? CapaUrl { get; set; }

        // ✅ Multi-select
        [Display(Name = "Autores")]
        public List<int> AutoresSelecionados { get; set; } = new List<int>();

        public List<SelectListItem> AutoresDisponiveis { get; set; } = new List<SelectListItem>();

        // Para exibir na lista/details
        public List<string> AutoresNomes { get; set; } = new List<string>();
    }
}
