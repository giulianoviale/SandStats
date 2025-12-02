using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;   // 👈 IMPORTANTE
using SandStats.Data;

var builder = WebApplication.CreateBuilder(args);

// === Detect environment & select DB ===
var env = builder.Environment;
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Detect PostgreSQL on Render (DATABASE_URL)
if (env.IsProduction())
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(dbUrl))
        conn = ConvertPostgresUrlToConnectionString(dbUrl);
}

// === Register DbContext ===
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    // Elegir proveedor según la connection string
    if (conn.Contains("Host="))
        opt.UseNpgsql(conn);
    else
        opt.UseSqlite(conn);

    // 👇 ESTA ES LA CLAVE:
    // Ignorar el warning de "pending model changes"
    // que aparece cuando el modelo corre con otro proveedor (SQLite vs PostgreSQL)
    opt.ConfigureWarnings(w =>
        w.Ignore(RelationalEventId.PendingModelChangesWarning));
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
    options.Conventions.AllowAnonymousToPage("/Index"); // home público (si querés)
});

builder.Services.AddAuthorization();

var app = builder.Build();

// === APLICAR MIGRACIONES AUTOMÁTICAMENTE AL ARRANCAR ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();   // ahora ya no va a tirar la excepción por PendingModelChanges
}

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
