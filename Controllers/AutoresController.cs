using LibraryManagerPro.Data;
using LibraryManagerPro.Models;
using LibraryManagerPro.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AutoresController : Controller
{
    private readonly LibraryContext _context;

    public AutoresController(LibraryContext context)
    {
        _context = context;
    }

    // GET: Autores
    [AllowAnonymous]
    public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1, int pageSize = 6)
    {
        var query = _context.Autores.AsQueryable();

        // 🔍 Filtro de busca
        if (!string.IsNullOrEmpty(searchString))
        {
            var normalizedSearch = searchString.NormalizeToCompare();
            query = query.AsEnumerable() // força para memória
                .Where(a =>
                    a.Nome.NormalizeToCompare().Contains(normalizedSearch) ||
                    a.Nacionalidade.NormalizeToCompare().Contains(normalizedSearch))
                .AsQueryable();
        }

        // 📊 Ordenação (segura para SQLite)
        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(a => a.Nome),
            "date" => query.AsEnumerable().OrderBy(a => a.DataNascimento).AsQueryable(),
            "date_desc" => query.AsEnumerable().OrderByDescending(a => a.DataNascimento).AsQueryable(),
            _ => query.OrderBy(a => a.Nome)
        };

        var totalItems = query.Count();
        var autores = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // 📤 Envia dados para a View
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.SortOrder = sortOrder;

        return View(autores);
    }

    // GET: Autores/Create
    public IActionResult Create() => View();

    // POST: Autores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Nacionalidade,DataNascimento")] Autor autor)
    {
        if (!ModelState.IsValid) return View(autor);

        if (_context.Autores.Any(a => a.Nome == autor.Nome && a.DataNascimento == autor.DataNascimento))
        {
            ModelState.AddModelError(string.Empty, "Já existe um autor com o mesmo nome e data de nascimento.");
            return View(autor);
        }

        _context.Autores.Add(autor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
        if (autor == null) return NotFound();

        return View(autor);
    }

    // GET: Autores/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _context.Autores.FindAsync(id);
        if (autor == null) return NotFound();

        return View(autor);
    }

    // POST: Autores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Nacionalidade,DataNascimento")] Autor autor)
    {
        if (id != autor.Id) return NotFound();
        if (!ModelState.IsValid) return View(autor);

        if (_context.Autores.Any(a => a.Id != autor.Id && a.Nome == autor.Nome && a.DataNascimento == autor.DataNascimento))
        {
            ModelState.AddModelError(string.Empty, "Já existe um autor com o mesmo nome e data de nascimento.");
            return View(autor);
        }

        try
        {
            _context.Update(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Autores.Any(a => a.Id == autor.Id)) return NotFound();
            throw;
        }
    }

    // GET: Autores/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var autor = await _context.Autores.FirstOrDefaultAsync(m => m.Id == id);
        if (autor == null) return NotFound();

        return View(autor);
    }

    // POST: Autores/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor != null)
        {
            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
