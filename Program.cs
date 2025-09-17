using LibraryManagerPro.Data;
using LibraryManagerPro.Models;
using LibraryManagerPro.Services;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1️⃣ Configurações de DbContext
// -----------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddDbContext<LibraryContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("LibraryConnection")));

// -----------------------------
// 2️⃣ Configura Identity
// -----------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Para testes: não exigir confirmação de email
    options.SignIn.RequireConfirmedAccount = false;

    // Regras de senha (você pode deixar mais fortes depois)
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IImageService, FakeImageService>();





var app = builder.Build();

// -----------------------------
// 3️⃣ Seeding de dados
// -----------------------------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var appDbContext = services.GetRequiredService<ApplicationDbContext>();
    var libraryContext = services.GetRequiredService<LibraryContext>();

    // ✅ Apenas cria se não existir
    appDbContext.Database.EnsureCreated();
    libraryContext.Database.EnsureCreated();

    // 🔹 Cria Role Admin se não existir
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // 🔹 Cria usuário Admin se não existir
    var adminEmail = "cairo@teste.com";
    var adminPassword = "C@iro123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createResult.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
        else
            throw new Exception("Erro ao criar usuário admin: " +
             string.Join(", ", createResult.Errors.Select(e => e.Description)));
    }
    else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // ---- Autores ----
    if (!libraryContext.Autores.Any()) // ✅ só insere se estiver vazio
    {
        var autores = new List<Autor>
        {
            new Autor { Nome = "Paulo Coelho", Nacionalidade = "Brasil", DataNascimento = new DateTime(1947, 8, 24) },
            new Autor { Nome = "J.K. Rowling", Nacionalidade = "Reino Unido", DataNascimento = new DateTime(1965, 7, 31) },
            new Autor { Nome = "George R.R. Martin", Nacionalidade = "EUA", DataNascimento = new DateTime(1948, 9, 20) }
        };

        libraryContext.Autores.AddRange(autores);
        await libraryContext.SaveChangesAsync();
    }

    // ---- Livros ----
    if (!libraryContext.Livros.Any()) // ✅ só insere se estiver vazio
    {
        var livro1 = new Livro
        {
            Titulo = "O Alquimista",
            Descricao = "Romance inspirador",
            AnoPublicacao = 1988,
            Preco = 29.90M,
            CapaUrl = "https://images.unsplash.com/photo-1544931369-9f0a8a17f6a3?w=500"
        };

        var livro2 = new Livro
        {
            Titulo = "Harry Potter e a Pedra Filosofal",
            Descricao = "Fantasia",
            AnoPublicacao = 1997,
            Preco = 39.90M,
            CapaUrl = "https://images.unsplash.com/photo-1529651737248-dad5e20a9f5f?w=500"
        };

        var livro3 = new Livro
        {
            Titulo = "A Guerra dos Tronos",
            Descricao = "Fantasia épica",
            AnoPublicacao = 1996,
            Preco = 50.00M,
            CapaUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=500"
        };

        livro1.Autores.Add(new LivroAutor { AutorId = libraryContext.Autores.First(a => a.Nome == "Paulo Coelho").Id });
        livro2.Autores.Add(new LivroAutor { AutorId = libraryContext.Autores.First(a => a.Nome == "J.K. Rowling").Id });
        livro3.Autores.Add(new LivroAutor { AutorId = libraryContext.Autores.First(a => a.Nome == "George R.R. Martin").Id });

        libraryContext.Livros.AddRange(livro1, livro2, livro3);
        await libraryContext.SaveChangesAsync();
    }
}


// -----------------------------
// 4️⃣ Pipeline
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStatusCodePagesWithReExecute("/Identity/Account/AccessDenied");

app.Run();