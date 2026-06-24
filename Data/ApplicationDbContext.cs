using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SandStats.Models;
using SandStats.Models.SandStats.Models;
using SandStats.Models.EnVivo;

namespace SandStats.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Normaliza todos los DateTime a UTC ---
        private void NormalizeDateTimesToUtc()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.ClrType == typeof(DateTime) && prop.CurrentValue is DateTime dt)
                    {
                        if (dt.Kind == DateTimeKind.Local)
                            prop.CurrentValue = dt.ToUniversalTime();
                        else if (dt.Kind == DateTimeKind.Unspecified)
                            prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                    else if (prop.Metadata.ClrType == typeof(DateTime?) && prop.CurrentValue is DateTime v)
                    {
                        if (v.Kind == DateTimeKind.Local)
                            prop.CurrentValue = v.ToUniversalTime();
                        else if (v.Kind == DateTimeKind.Unspecified)
                            prop.CurrentValue = DateTime.SpecifyKind(v, DateTimeKind.Utc);
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

        // --- DbSets ---
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Dupla> Duplas { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<EstadisticaAtaque> EstadisticaAtaque { get; set; }
        public DbSet<EstadisticaRecepcion> EstadisticaRecepcion { get; set; }
        public DbSet<EstadisticaK2> EstadisticaK2 { get; set; }
        public DbSet<VideoLinksJugadorPartido> VideoLinksJugadorPartido { get; set; } = default!;
        public DbSet<JugadorLinks> JugadorLinks { get; set; } = default!;

        // --- EnVivo ---
        public DbSet<PartidoEnVivo> PartidosEnVivo { get; set; }
        public DbSet<SetEnVivo> SetsEnVivo { get; set; }
        public DbSet<Rally> Rallies { get; set; }
        public DbSet<Accion> Acciones { get; set; }
        public DbSet<DetalleSaque> DetallesSaque { get; set; }
        public DbSet<DetalleRecepcion> DetallesRecepcion { get; set; }
        public DbSet<DetalleAtaque> DetallesAtaque { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Dupla ↔ Jugadores
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

            // Jugador ↔ Dupla
            modelBuilder.Entity<Jugador>()
                .HasOne(j => j.Dupla)
                .WithMany()
                .HasForeignKey(j => j.DuplaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Partido ↔ Dupla
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

           

            // Relaciones de estadísticas
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

            // Sets
            modelBuilder.Entity<Set>()
                .HasOne(s => s.Partido)
                .WithMany(p => p.Sets)
                .HasForeignKey(s => s.PartidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Links de videos
            modelBuilder.Entity<VideoLinksJugadorPartido>()
                .HasOne(v => v.Partido)
                .WithMany()
                .HasForeignKey(v => v.PartidoId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<VideoLinksJugadorPartido>()
                .HasOne(v => v.Jugador)
                .WithMany()
                .HasForeignKey(v => v.JugadorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── EnVivo ────────────────────────────────────────────────

            // PartidoEnVivo → Duplas (Restrict)
            modelBuilder.Entity<PartidoEnVivo>()
                .HasOne(p => p.Dupla1)
                .WithMany()
                .HasForeignKey(p => p.Dupla1Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PartidoEnVivo>()
                .HasOne(p => p.Dupla2)
                .WithMany()
                .HasForeignKey(p => p.Dupla2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // PartidoEnVivo → SetEnVivo (Cascade)
            modelBuilder.Entity<SetEnVivo>()
                .HasOne(s => s.PartidoEnVivo)
                .WithMany(p => p.Sets)
                .HasForeignKey(s => s.PartidoEnVivoId)
                .OnDelete(DeleteBehavior.Cascade);

            // SetEnVivo → SacadorInicialJugador (Restrict)
            modelBuilder.Entity<SetEnVivo>()
                .HasOne(s => s.SacadorInicialJugador)
                .WithMany()
                .HasForeignKey(s => s.SacadorInicialJugadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // SetEnVivo → Rally (Cascade)
            modelBuilder.Entity<Rally>()
                .HasOne(r => r.SetEnVivo)
                .WithMany(s => s.Rallies)
                .HasForeignKey(r => r.SetEnVivoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rally → DuplaGanadora (Restrict, nullable)
            modelBuilder.Entity<Rally>()
                .HasOne(r => r.DuplaGanadora)
                .WithMany()
                .HasForeignKey(r => r.DuplaGanadoraId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Rally → Accion (Cascade)
            modelBuilder.Entity<Accion>()
                .HasOne(a => a.Rally)
                .WithMany(r => r.Acciones)
                .HasForeignKey(a => a.RallyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Accion → Jugador (Restrict)
            modelBuilder.Entity<Accion>()
                .HasOne(a => a.Jugador)
                .WithMany()
                .HasForeignKey(a => a.JugadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Accion → DetalleSaque (1:1, Cascade)
            modelBuilder.Entity<Accion>()
                .HasOne(a => a.DetalleSaque)
                .WithOne(d => d.Accion)
                .HasForeignKey<DetalleSaque>(d => d.AccionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DetalleSaque>()
                .HasIndex(d => d.AccionId)
                .IsUnique();

            // Accion → DetalleRecepcion (1:1, Cascade)
            modelBuilder.Entity<Accion>()
                .HasOne(a => a.DetalleRecepcion)
                .WithOne(d => d.Accion)
                .HasForeignKey<DetalleRecepcion>(d => d.AccionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DetalleRecepcion>()
                .HasIndex(d => d.AccionId)
                .IsUnique();

            // Accion → DetalleAtaque (1:1, Cascade)
            modelBuilder.Entity<Accion>()
                .HasOne(a => a.DetalleAtaque)
                .WithOne(d => d.Accion)
                .HasForeignKey<DetalleAtaque>(d => d.AccionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DetalleAtaque>()
                .HasIndex(d => d.AccionId)
                .IsUnique();
        }
    }
}
