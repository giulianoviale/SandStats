using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    if (conn.Contains("Host="))
        opt.UseNpgsql(conn);
    else
        opt.UseSqlite(conn);
});

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

// Expone Program al proyecto de tests para WebApplicationFactory<Program>
public partial class Program { }
