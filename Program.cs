using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

if (env.IsProduction())
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(dbUrl))
        conn = ConvertPostgresUrlToConnectionString(dbUrl);
}

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
