using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SandStats.Models;
using SandStats.Models.SandStats.Models;

namespace SandStats.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Normaliza DateTime/DateTime? a UTC antes de guardar
    private void NormalizeDateTimesToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var prop in entry.Properties)
            {
                // DateTime (no nullable)
                if (prop.Metadata.ClrType == typeof(DateTime))
                {
                    if (prop.CurrentValue is DateTime dt)
                    {
                        if (dt.Kind == DateTimeKind.Local)
                            prop.CurrentValue = dt.ToUniversalTime();
                        else if (dt.Kind == DateTimeKind.Unspecified)
                            prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
                // DateTime? (nullable)
                else if (prop.Metadata.ClrType == typeof(DateTime?))
                {
                    // EF boxea el DateTime? con valor como DateTime
                    if (prop.CurrentValue is DateTime v)
                    {
                        if (v.Kind == DateTimeKind.Local)
                            prop.CurrentValue = v.ToUniversalTime();
                        else if (v.Kind == DateTimeKind.Unspecified)
                            prop.CurrentValue = DateTime.SpecifyKind(v, DateTimeKind.Utc);
                    }
                }
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }
    public DbSet<Jugador> Jugadores { get; set; }
    public DbSet<Dupla> Duplas { get; set; }
    public DbSet<Partido> Partidos { get; set; }
    public DbSet<Set> Sets { get; set; }

    public DbSet<EstadisticaAtaque> EstadisticaAtaque { get; set; }
    public DbSet<EstadisticaRecepcion> EstadisticaRecepcion { get; set; }
    public DbSet<EstadisticaK2> EstadisticaK2 { get; set; }
    public DbSet<VideoLinksJugadorPartido> VideoLinksJugadorPartido { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dupla>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(d => d.Jugador1).WithMany()
                  .HasForeignKey(d => d.Jugador1Id)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            entity.HasOne(d => d.Jugador2).WithMany()
                  .HasForeignKey(d => d.Jugador2Id)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            entity.HasIndex(d => new { d.Jugador1Id, d.Jugador2Id })
                  .IsUnique()
                  .HasDatabaseName("UX_Dupla_J1_J2");

            
        });

        // (si además usás Jugador.DuplaId)
        modelBuilder.Entity<Jugador>()
            .HasOne(j => j.Dupla)
            .WithMany() // o .WithMany(d => d.Jugadores) si tenés la colección
            .HasForeignKey(j => j.DuplaId)
            .OnDelete(DeleteBehavior.SetNull); // “disolver” deja a los jugadores sin dupla

        // --- Partido ↔ Dupla: NO cascades (no borrar duplas al borrar partido)
        modelBuilder.Entity<Partido>()
            .HasOne(p => p.Dupla1)
            .WithMany()
            .HasForeignKey(p => p.Dupla1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Partido>()
            .HasOne(p => p.Dupla2)
            .WithMany()
            .HasForeignKey(p => p.Dupla2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Partido>()
        .Property(p => p.CreatedOn)
        .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // --- Estadísticas ↔ Partido: CASCADE (al borrar partido, borrar stats)
        modelBuilder.Entity<EstadisticaAtaque>()
            .HasOne(e => e.Partido)
            .WithMany()
            .HasForeignKey(e => e.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EstadisticaRecepcion>()
            .HasOne(e => e.Partido)
            .WithMany()
            .HasForeignKey(e => e.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EstadisticaK2>()
            .HasOne(e => e.Partido)
            .WithMany()
            .HasForeignKey(e => e.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Estadísticas ↔ Jugador: RESTRICT (evitar borrar un jugador con histórico)
        modelBuilder.Entity<EstadisticaAtaque>()
            .HasOne(e => e.Jugador)
            .WithMany()
            .HasForeignKey(e => e.JugadorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EstadisticaRecepcion>()
            .HasOne(e => e.Jugador)
            .WithMany()
            .HasForeignKey(e => e.JugadorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EstadisticaK2>()
            .HasOne(e => e.Jugador)
            .WithMany()
            .HasForeignKey(e => e.JugadorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Set>()
            .HasOne(s => s.Partido)
            .WithMany(p => p.Sets)          // si no tenés la colección, podés usar .WithMany()
            .HasForeignKey(s => s.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);

        //LinkVideos
        modelBuilder.Entity<VideoLinksJugadorPartido>()
            .HasOne(v => v.Partido)
            .WithMany() // si no vas a navegar desde Partido
            .HasForeignKey(v => v.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VideoLinksJugadorPartido>()
            .HasOne(v => v.Jugador)
            .WithMany() // si no vas a navegar desde Jugador
            .HasForeignKey(v => v.JugadorId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
