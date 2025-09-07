using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Tomamos primero la cadena de entorno (cloud) y si no existe, usamos la del appsettings (local)
var pgConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var localConn = builder.Configuration.GetConnectionString("DefaultConnection");
var connString = pgConn ?? localConn;

// 2) Elegimos provider: si hay variable de entorno => Postgres; si no => SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(pgConn))
        options.UseNpgsql(connString);   // nube (Render)
    else
        options.UseSqlite(connString);   // local
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (simple por ahora)
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // ponelo true si después vas a confirmar email
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// 3) Pipeline
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

app.UseRouting();

app.UseAuthentication();   // <= Faltaba esto para Identity
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
