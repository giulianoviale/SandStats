using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Security;
using System.Linq; // para LINQ en el seed

// --- SEED: crea roles y un usuario admin si no existen ---
static async Task SeedAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var runSeed = env.IsDevelopment() ||
                  (cfg["RUN_SEED"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false);
    if (!runSeed) return;

    foreach (var r in new[] { "Admin", "Coach", "Player" })
        if (!await roles.RoleExistsAsync(r))
            await roles.CreateAsync(new IdentityRole(r));

    var adminEmail = cfg["SEED_ADMIN_EMAIL"] ?? "admin@sandstats.dev";
    var adminPass = cfg["SEED_ADMIN_PASSWORD"] ?? "Admin123!";

    var admin = await users.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var res = await users.CreateAsync(admin, adminPass);
        if (!res.Succeeded)
            throw new Exception("No pude crear el admin: " + string.Join("; ", res.Errors.Select(e => e.Description)));
        await users.AddToRoleAsync(admin, "Admin");
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index"); // Home pública
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Logout");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/AccessDenied");
});


// 🔌 Conexión (nube: Postgres por env var / local: SQLite por appsettings)
var pgConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var localConn = builder.Configuration.GetConnectionString("DefaultConnection");
var conn = pgConn ?? localConn;

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    if (!string.IsNullOrWhiteSpace(pgConn))
        opt.UseNpgsql(conn);   // nube (Render)
    else
        opt.UseSqlite(conn);   // local
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🧑‍💻 Identity
builder.Services
  .AddDefaultIdentity<ApplicationUser>(o =>
  {
      o.SignIn.RequireConfirmedAccount = true;
      o.User.RequireUniqueEmail = true;
      o.Password.RequireNonAlphanumeric = false;
      o.Password.RequireUppercase = false;
      o.Password.RequireDigit = false;
      o.Password.RequiredLength = 6;
  })
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

// SignInManager custom (opcional)
builder.Services.AddScoped<SignInManager<ApplicationUser>, AppSignInManager>();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/Identity/Account/AccessDenied";
    o.SlidingExpiration = true;
});
var app = builder.Build();

// 🚀 Crear/actualizar schema al arrancar (SQLite local / Postgres nube)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// 🌐 Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 📦 MUY IMPORTANTE: servir estáticos ANTES de routing/autorización
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await SeedAsync(app);
app.Run();
