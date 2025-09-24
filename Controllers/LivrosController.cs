using LibraryManagerPro.Data;
using LibraryManagerPro.Models;
using LibraryManagerPro.Models.ViewModels;
using LibraryManagerPro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagerPro.Utils;

[Authorize]
public class LivrosController : Controller
{
    private readonly LibraryContext _context;
    private readonly IImageService _imageService;

    public LivrosController(LibraryContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    // GET: Livros
    [AllowAnonymous]
    public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 6)
    {
        ViewBag.CurrentSort = sortOrder;
        ViewBag.TitleSort = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
        ViewBag.PriceSort = sortOrder == "price" ? "price_desc" : "price";
        ViewBag.YearSort = sortOrder == "year" ? "year_desc" : "year";

        var query = _context.Livros
            .Include(l => l.Autores).ThenInclude(la => la.Autor)
            .AsQueryable();

        // 🔎 Busca (case-insensitive, com trim)
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var normalized = searchString.Trim().NormalizeToCompare();
            query = query.Where(l =>
                l.Titulo.NormalizeToCompare().Contains(normalized) ||
                l.Autores.Any(a => a.Autor.Nome.NormalizeToCompare().Contains(normalized))
            );
        }

        // 📊 Ordenação
        query = sortOrder switch
        {
            "title_desc" => query.OrderByDescending(l => l.Titulo),
            "price" => query.OrderBy(l => l.Preco),
            "price_desc" => query.OrderByDescending(l => l.Preco),
            "year" => query.OrderBy(l => l.AnoPublicacao),
            "year_desc" => query.OrderByDescending(l => l.AnoPublicacao),
            _ => query.OrderBy(l => l.Titulo)
        };

        // 📄 Paginação
        var totalItems = await query.CountAsync();
        var livros = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LivroViewModel
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Descricao = l.Descricao,
                AnoPublicacao = l.AnoPublicacao,
                Preco = l.Preco,
                CapaUrl = l.CapaUrl,
                AutoresNomes = l.Autores.Select(la => la.Autor.Nome).ToList()
            })
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.SearchString = searchString;

        return View(livros);
    }

    // GET: Livros/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var livro = await _context.Livros
            .Include(l => l.Autores).ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null) return NotFound("Livro não encontrado.");

        var viewModel = new LivroViewModel
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            AnoPublicacao = livro.AnoPublicacao,
            Preco = livro.Preco,
            Descricao = livro.Descricao,
            CapaUrl = livro.CapaUrl,
            AutoresNomes = livro.Autores.Select(la => la.Autor.Nome).ToList()
        };

        var autorIds = livro.Autores.Select(la => la.AutorId).ToList();

        var relacionados = await _context.Livros
            .Include(l => l.Autores)
            .Where(l => l.Id != livro.Id && l.Autores.Any(la => autorIds.Contains(la.AutorId)))
            .Take(3)
            .Select(l => new LivroViewModel
            {
                Id = l.Id,
                Titulo = l.Titulo,
                CapaUrl = l.CapaUrl
            })
            .ToListAsync();

        if (!relacionados.Any())
        {
            relacionados = await _context.Livros
                .Where(l => l.Id != livro.Id)
                .OrderByDescending(l => l.AnoPublicacao)
                .Take(3)
                .Select(l => new LivroViewModel
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    CapaUrl = l.CapaUrl
                })
                .ToListAsync();
        }

        ViewBag.Relacionados = relacionados;
        return View(viewModel);
    }

    // GET: Livros/Create
    // GET: Livros/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var vm = new LivroViewModel
        {
            AutoresDisponiveis = await _context.Autores
                .OrderBy(a => a.Nome)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync()
        };
        return View(vm);
    }

    // POST: Livros/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(LivroViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            // Recarrega lista de autores se houver erro de validação
            vm.AutoresDisponiveis = await _context.Autores
                .OrderBy(a => a.Nome)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync();

            return View(vm);
        }

        try
        {
            // 🛡️ Proteção contra duplicatas (mesmo título)
            if (await _context.Livros.AnyAsync(l => l.Titulo == vm.Titulo))
            {
                ModelState.AddModelError("Titulo", "Já existe um livro com este título.");
                vm.AutoresDisponiveis = await _context.Autores
                    .OrderBy(a => a.Nome)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Nome
                    })
                    .ToListAsync();
                return View(vm);
            }

            // 🎨 Geração automática da capa do livro
            var prompt = !string.IsNullOrWhiteSpace(vm.Descricao)
                ? $"{vm.Titulo} {vm.Descricao}"
                : vm.Titulo;

            string capaUrl;
            try
            {
                capaUrl = await _imageService.GerarImagemAsync(prompt);
            }
            catch (Exception imgEx)
            {
                Console.WriteLine($"[Create] Erro ao gerar imagem: {imgEx.Message}");
                capaUrl = "https://via.placeholder.com/300x400.png?text=Erro+na+Geração";
            }

            var livro = new Livro
            {
                Titulo = vm.Titulo,
                Descricao = vm.Descricao,
                AnoPublicacao = vm.AnoPublicacao,
                Preco = vm.Preco,
                CapaUrl = capaUrl
            };

            // Associa autores selecionados ao livro
            if (vm.AutoresSelecionados?.Any() == true)
            {
                foreach (var aid in vm.AutoresSelecionados)
                {
                    livro.Autores.Add(new LivroAutor { AutorId = aid });
                }
            }

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            TempData["Success"] = "📚 Livro adicionado com sucesso! Capa gerada automaticamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Create] Erro ao salvar livro: {ex.Message}");
            TempData["Error"] = "❌ Erro ao criar livro. Tente novamente.";
        }

        // Recarrega lista de autores em caso de erro
        vm.AutoresDisponiveis = await _context.Autores
            .OrderBy(a => a.Nome)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nome
            })
            .ToListAsync();

        return View(vm);
    }



    // GET: Livros/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var livro = await _context.Livros
            .Include(l => l.Autores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null) return NotFound("Livro não encontrado.");

        var vm = new LivroViewModel
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Descricao = livro.Descricao,
            AnoPublicacao = livro.AnoPublicacao,
            Preco = livro.Preco,
            CapaUrl = livro.CapaUrl,
            AutoresSelecionados = livro.Autores.Select(a => a.AutorId).ToList(),
            AutoresDisponiveis = await _context.Autores
                .OrderBy(a => a.Nome)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync()
        };
        return View(vm);
    }

    // POST: Livros/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, LivroViewModel vm)
    {
        if (id != vm.Id) return NotFound("ID do livro não corresponde.");

        if (!ModelState.IsValid)
        {
            vm.AutoresDisponiveis = await _context.Autores
                .OrderBy(a => a.Nome)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync();
            return View(vm);
        }

        var livro = await _context.Livros
            .Include(l => l.Autores)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null) return NotFound("Livro não encontrado.");

        try
        {
            livro.Titulo = vm.Titulo;
            livro.Descricao = vm.Descricao;
            livro.AnoPublicacao = vm.AnoPublicacao;
            livro.Preco = vm.Preco;
            
            // 🎨 Opção: Regenerar capa apenas se título ou descrição mudaram
            // Descomente as linhas abaixo se quiser essa funcionalidade
            /*
            if (livro.Titulo != vm.Titulo || livro.Descricao != vm.Descricao)
            {
                var prompt = !string.IsNullOrWhiteSpace(vm.Descricao) 
                    ? $"{vm.Titulo} {vm.Descricao}" 
                    : vm.Titulo;
                livro.CapaUrl = await _imageService.GerarImagemAsync(prompt);
            }
            else
            {
                livro.CapaUrl = vm.CapaUrl; // Mantém a capa atual
            }
            */
            
            livro.CapaUrl = vm.CapaUrl; // Mantém a capa atual

            // Atualiza autores
            livro.Autores.Clear();
            foreach (var aid in vm.AutoresSelecionados)
            {
                livro.Autores.Add(new LivroAutor { AutorId = aid });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✏️ Livro atualizado com sucesso!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Livros.Any(e => e.Id == vm.Id))
                return NotFound("Livro não existe mais.");
            throw;
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erro ao atualizar livro. Tente novamente.";
            // Para debug: TempData["Error"] = $"Erro: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Livros/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound("ID inválido.");

        var livro = await _context.Livros
            .Include(l => l.Autores).ThenInclude(la => la.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null) return NotFound("Livro não encontrado.");

        var vm = new LivroViewModel
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Descricao = livro.Descricao,
            AnoPublicacao = livro.AnoPublicacao,
            Preco = livro.Preco,
            CapaUrl = livro.CapaUrl,
            AutoresNomes = livro.Autores.Select(la => la.Autor.Nome).ToList()
        };

        return View(vm);
    }

    // POST: Livros/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro != null)
        {
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
            TempData["Success"] = "🗑️ Livro excluído com sucesso!";
        }
        else
        {
            TempData["Error"] = "Livro não encontrado para exclusão.";
        }

        return RedirectToAction(nameof(Index));
    }

    // 🆕 Método adicional para regenerar capa de um livro existente
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegenerarCapa(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null)
        {
            TempData["Error"] = "Livro não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var prompt = !string.IsNullOrWhiteSpace(livro.Descricao) 
                ? $"{livro.Titulo} {livro.Descricao}" 
                : livro.Titulo;
            
            livro.CapaUrl = await _imageService.GerarImagemAsync(prompt);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "🎨 Capa regenerada com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erro ao regenerar capa. Tente novamente.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}