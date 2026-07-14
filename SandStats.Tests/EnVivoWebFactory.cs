using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandStats.Data;

namespace SandStats.Tests
{
    public class EnVivoWebFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _conn;

        public EnVivoWebFactory()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                // Reemplazar DbContext por SQLite :memory: compartida entre requests y seeds
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlite(_conn));

                // Auth bypass: el TestAuthHandler siempre autentica sin cookies
                services.AddAuthentication()
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.PostConfigureAll<AuthenticationOptions>(opts =>
                {
                    opts.DefaultScheme               = "Test";
                    opts.DefaultAuthenticateScheme   = "Test";
                    opts.DefaultChallengeScheme      = "Test";
                    opts.DefaultForbidScheme         = "Test";
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            using var scope = host.Services.CreateScope();
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database.EnsureCreated();
            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) _conn.Close();
        }
    }
}
