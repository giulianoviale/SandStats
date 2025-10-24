using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SandStats.Data;
using SandStats.Security;
using System.Linq;

// --- Seed roles y usuario admin ---
// trigger redeploy for Render

static async Task SeedAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (env.IsDevelopment())
        await db.Database.EnsureCreatedAsync();
    else
        await db.Database.MigrateAsync();

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

// --- Configuración de autorización ---
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Logout");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/AccessDenied");
});

var conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(conn);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- Identity ---
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

builder.Services.AddScoped<SignInManager<ApplicationUser>, AppSignInManager>();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/Identity/Account/AccessDenied";
    o.SlidingExpiration = true;
});

var app = builder.Build();

// --- Pipeline HTTP ---
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
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// --- Inicialización y seed ---
await SeedAsync(app);
app.Run();
