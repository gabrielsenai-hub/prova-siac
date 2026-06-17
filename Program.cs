using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProvaSiac.Data;
using ProvaSiac.Models;
using ProvaSiac.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 44));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion,
        b => b.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

builder.Services.AddDefaultIdentity<Usuario>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // --- Lógica Adicionada para Criar/Migrar o Banco de Dados ---
    try
    {
        // 1. Aplica todas as migrações pendentes no banco de dados.
        // Se o banco não existir, ele será criado.
        await context.Database.MigrateAsync();

        // 2. Continua com o seeding das roles.
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();

        await Seed.SeedRoles(roleManager);
        await Seed.SeedMasterUser(userManager);
    }
    catch (Exception ex)
    {
        // Trate erros, como problemas de conexão ou migrações inválidas.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao criar ou migrar o banco de dados.");
        // Você pode optar por re-lançar ou encerrar o aplicativo aqui.
    }
}

app.Run();
