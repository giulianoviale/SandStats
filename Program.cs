using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SandStats.Data;
using SandStats.Endpoints.EnVivo;
using SandStats.Services.EnVivo;

var builder = WebApplication.CreateBuilder(args);

// === Detect environment & select DB ===
var env = builder.Environment;
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Detect PostgreSQL on Render
if (env.IsProduction())
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(dbUrl))
        conn = ConvertPostgresUrlToConnectionString(dbUrl);
}

// === Register DbContext ===
// En el entorno "Testing" no se registra ningún proveedor: EnVivoWebFactory registra
// el DbContext con SQLite en memoria. Si acá se registrara Npgsql (los user secrets
// apuntan a Postgres) quedarían dos proveedores en el mismo contenedor y EF Core lo
// rechaza; los servicios internos del proveedor no se pueden quitar de forma confiable
// desde la factory.
if (!env.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    {
        if (conn.Contains("Host="))
            opt.UseNpgsql(conn);
        else
            opt.UseSqlite(conn);

        // Las 21 migraciones se generaron con el proveedor SQLite. Al construir el
        // modelo con Npgsql, EF detecta diferencias cosméticas de tipos (INTEGER vs
        // integer) contra el snapshot y las reporta como cambios pendientes. No lo
        // son: Npgsql traduce esos tipos correctamente y las dos apps corren así en
        // producción desde hace meses. Se suprime la advertencia para poder usar
        // PostgreSQL también en desarrollo. Contrapartida: si en el futuro cambia el
        // modelo y falta la migración, EF no avisa — hay que generarla conscientemente.
        //opt.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    });
}

// === Identity ===
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // protege todas las páginas
    options.Conventions.AllowAnonymousToFolder("/Identity"); // deja libre el login/register
    options.Conventions.AllowAnonymousToPage("/Index"); // opcional, si querés que el home sea público
});
//faltaba esta linea para que funcione el logueo
builder.Services.AddAuthorization();

builder.Services.AddScoped<CargaEnVivoService>();

// Para rutas /api/*, responder con 401/403 en vez de redirigir al login/acceso denegado.
// CSRF mitigado por SameSite=Lax (decisión consciente, sin antiforgery en el grupo API):
// la UI es same-origin y el estado mutable queda protegido por la cookie de Identity.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));

var app = builder.Build();

// === Pipeline ===
if (!app.Environment.IsDevelopment())
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
app.MapEnVivoEndpoints();

await SeedAsync(app);
app.Run();

// === Helper ===
static string ConvertPostgresUrlToConnectionString(string dbUrl)
{
    // Ejemplo: postgres://user:pass@host:port/dbname
    var uri = new Uri(dbUrl);
    var user = uri.UserInfo.Split(':')[0];
    var pass = uri.UserInfo.Split(':')[1];
    var host = uri.Host;
    var port = uri.Port;
    var db = uri.AbsolutePath.Trim('/');

    return $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
}

// Bootstraps roles y usuario admin para entornos nuevos.
// El registro de usuarios requiere un Admin previo: sin este seed, un entorno
// vacío quedaría inutilizable porque nadie podría crear el primer usuario.
static async Task SeedAsync(WebApplication app)
{
    if (app.Environment.IsEnvironment("Testing"))
        return;

    var logger = app.Logger;
    using var scope = app.Services.CreateScope();
    var cfg   = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var env   = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var runSeed = env.IsDevelopment() ||
                  (cfg["RUN_SEED"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false);

    if (!runSeed)
    {
        logger.LogInformation("[Seed] Omitido: no es Development y RUN_SEED no está habilitado.");
        return;
    }

    foreach (var rol in new[] { "Admin", "Coach" })
        if (!await roles.RoleExistsAsync(rol))
            await roles.CreateAsync(new IdentityRole(rol));

    var adminEmail = cfg["SEED_ADMIN_EMAIL"] ?? "admin@sandstats.dev";
    var adminPass  = cfg["SEED_ADMIN_PASSWORD"] ?? "Admin123!";

    if (await users.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName       = adminEmail,
            Email          = adminEmail,
            EmailConfirmed = true
        };
        var res = await users.CreateAsync(admin, adminPass);
        if (!res.Succeeded)
            throw new Exception("[Seed] No se pudo crear el usuario admin: " +
                string.Join("; ", res.Errors.Select(e => e.Description)));
        await users.AddToRoleAsync(admin, "Admin");
        logger.LogInformation("[Seed] Usuario admin creado: {Email}", adminEmail);
    }
}

// Expone Program al proyecto de tests para WebApplicationFactory<Program>
public partial class Program { }
