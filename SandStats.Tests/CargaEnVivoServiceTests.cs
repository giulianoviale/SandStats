using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class CargaEnVivoServiceTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly ApplicationDbContext _db;
        private readonly CargaEnVivoService _svc;

        // Entidades de master data compartidas entre tests de esta clase
        private readonly Dupla _d1, _d2;
        private readonly Jugador _j1, _j2, _j3, _j4;

        public CargaEnVivoServiceTests()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_conn)
                .Options;

            _db = new ApplicationDbContext(opts);
            _db.Database.EnsureCreated(); // schema + seed ModificadoresCombinadas

            _svc = new CargaEnVivoService(_db);

            // Seed: 4 jugadores, 2 duplas
            _j1 = new Jugador { Nombre = "J1", Apellido = "D1", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            _j2 = new Jugador { Nombre = "J2", Apellido = "D1", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _j3 = new Jugador { Nombre = "J3", Apellido = "D2", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            _j4 = new Jugador { Nombre = "J4", Apellido = "D2", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _db.Jugadores.AddRange(_j1, _j2, _j3, _j4);
            _db.SaveChanges();

            _d1 = new Dupla { Jugador1Id = _j1.Id, Jugador2Id = _j2.Id };
            _d2 = new Dupla { Jugador1Id = _j3.Id, Jugador2Id = _j4.Id };
            _db.Duplas.AddRange(_d1, _d2);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
            _conn.Close();
        }

        private async Task<(PartidoEnVivo partido, SetEnVivo set)> CrearPartidoYSet()
        {
            var partido = await _svc.CrearPartidoAsync(
                _d1.Id, _d2.Id, "Torneo Test", DateTime.UtcNow);
            var set = await _svc.IniciarSetAsync(
                partido.Id, numeroSet: 1,
                sacadorInicialD1: _j1.Id,
                sacadorInicialD2: _j3.Id,
                duplaQueSacaPrimeroId: _d1.Id);
            return (partido, set);
        }

        // ── Test 1: rally completo con derivaciones y cierre ─────────────────

        [Fact]
        public async Task RallyCompleto_DerivacionesYCierre_MarcadorCorrecto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // 1. Saque sin calidad (J1/D1)
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);

            // 2. Recepcion Positivo (J3/D2) → deriva Saque→Negativo
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            // 3. Ataque sin calidad (J3/D2) con DetalleAtaque
            var detalleAtaque = new DetalleAtaque
            {
                Lado = TipoLado.Bueno,
                TipoAccion = TipoAcciones.Atq1,
                ZonaDestino = ZonaCancha.Zona1
            };
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), detalleAtaque);

            // 4. Bloqueo DobleNegativo (J1/D1) → deriva Ataque→DoblePositivo, cierra D2
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            // Recargar desde DB
            var acciones = await _db.Acciones
                .Include(a => a.DetalleAtaque)
                .Where(a => a.RallyId == rally.Id)
                .OrderBy(a => a.Secuencia)
                .ToListAsync();

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            // Derivación 1: saque tiene Calidad=Negativo
            Assert.Equal(Calidad.Negativo, acciones[0].Calidad);

            // Derivación 2: ataque tiene Calidad=DoblePositivo
            Assert.Equal(Calidad.DoblePositivo, acciones[2].Calidad);

            // DetalleAtaque persistido
            Assert.NotNull(acciones[2].DetalleAtaque);

            // Cierre: D2 ganó (Bloqueo DobleNegativo de J1/D1 → rival = D2)
            Assert.Equal(_d2.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(0, rallyActualizado.MarcadorDupla1);
            Assert.Equal(1, rallyActualizado.MarcadorDupla2);
        }

        // ── Test 2: cierre directo sin acciones ──────────────────────────────

        [Fact]
        public async Task CierreDirectoAce_SinAcciones_D1GanaYMarcadorCorrecto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // Ace: gana la dupla servante (D1 saca primero)
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Equal(_d1.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(1, rallyActualizado.MarcadorDupla1);
            Assert.Equal(0, rallyActualizado.MarcadorDupla2);
        }

        // ── Test 4: deshacer recepción Positivo → saque vuelve a null ────────

        [Fact]
        public async Task DeshacerRecepcionPositivo_SaqueVuelveANull()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            // Pre-assert: saque ya fue derivado a Negativo
            var accionesPre = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            Assert.Equal(Calidad.Negativo, accionesPre[0].Calidad);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            Assert.Single(acciones);
            Assert.Null(acciones[0].Calidad);
        }

        // ── Test 5: deshacer bloqueo que cerró → ataque null y rally reabierto

        [Fact]
        public async Task DeshacerBloqueoQueCerro_AtaqueNullYRallyReabierto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);
            // Bloqueo DobleNegativo → deriva Ataque→DoblePositivo, cierra D2
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Equal(3, acciones.Count);
            Assert.Null(acciones[2].Calidad);              // ataque vuelve a null
            Assert.Null(rallyActualizado!.DuplaGanadoraId); // rally reabierto
            Assert.Null(rallyActualizado.TipoCierre);
            Assert.Equal(0, rallyActualizado.MarcadorDupla2); // marcador decrementado
        }

        // ── Test 6: deshacer con rally abierto y vacío → excepción ───────────

        [Fact]
        public async Task DeshacerConRallyVacio_LanzaExcepcion()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _svc.DeshacerUltimaAccionAsync(rally.Id));
        }

        // ── Test 7: CierreDirecto(Ace) → Deshacer → rally reabierto 0-0 ─────

        [Fact]
        public async Task CierreDirectoAce_Deshacer_RallyReabiertoMarcadorCero()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            // Pre-assert: cerrado
            var pre = await _db.Rallies.FindAsync(rally.Id);
            Assert.NotNull(pre!.DuplaGanadoraId);
            Assert.Equal(1, pre.MarcadorDupla1);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var post = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(post!.DuplaGanadoraId);
            Assert.Null(post.TipoCierre);
            Assert.Equal(0, post.MarcadorDupla1);
            Assert.Equal(0, post.MarcadorDupla2);
        }

        // ── Test 8: CerrarSet dupla ajena → excepción; válida → persiste ─────

        [Fact]
        public async Task CerrarSet_DuplaAjenaLanzaExcepcion_DuplaValidaPersiste()
        {
            var (_, set) = await CrearPartidoYSet();

            // Dupla ajena (J1 con J3 = dupla no existente; usamos una dupla nueva)
            var j5 = new Jugador { Nombre = "J5", Apellido = "X", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j6 = new Jugador { Nombre = "J6", Apellido = "X", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _db.Jugadores.AddRange(j5, j6);
            await _db.SaveChangesAsync();
            var d3 = new Dupla { Jugador1Id = j5.Id, Jugador2Id = j6.Id };
            _db.Duplas.Add(d3);
            await _db.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _svc.CerrarSetAsync(set.Id, d3.Id));

            await _svc.CerrarSetAsync(set.Id, _d1.Id);
            var setActualizado = await _db.SetsEnVivo.FindAsync(set.Id);
            Assert.Equal(_d1.Id, setActualizado!.DuplaGanadoraId);
        }

        // ── Test 9: TipoCierre Ace ────────────────────────────────────────────

        [Fact]
        public async Task CierreDirectoAce_TipoCierreEsAce()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);
            Assert.Equal(TipoCierreRally.Ace, rallyActualizado!.TipoCierre);
        }

        // ── Test 10: TipoCierre PorJuego ─────────────────────────────────────

        [Fact]
        public async Task RegistrarBloqueo_CierrePorJuego_TipoCierreEsPorJuego()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);
            Assert.Equal(TipoCierreRally.PorJuego, rallyActualizado!.TipoCierre);
        }

        // ── Test 3: dos rallies consecutivos, NumeroRally y marcador acumulado

        [Fact]
        public async Task DosRallies_NumeroRallyYMarcadorAcumuladoCorrectos()
        {
            var (_, set) = await CrearPartidoYSet();

            // Rally 1: D1 gana por Ace
            var rally1 = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally1.Id, TipoCierreDirecto.Ace);

            // Rally 2: abre con el marcador del rally 1 como going-in
            var rally2 = await _svc.AbrirRallyAsync(set.Id);

            Assert.Equal(2, rally2.NumeroRally);
            Assert.Equal(1, rally2.MarcadorDupla1); // going-in: D1 ya lleva 1 punto
            Assert.Equal(0, rally2.MarcadorDupla2);

            // Rally 2: D2 gana por ErrorSaque (sacadora = D1, error → rival = D2)
            await _svc.CierreDirectoAsync(rally2.Id, TipoCierreDirecto.ErrorSaque);

            var rally2Actualizado = await _db.Rallies.FindAsync(rally2.Id);

            Assert.Equal(_d2.Id, rally2Actualizado!.DuplaGanadoraId);
            Assert.Equal(1, rally2Actualizado.MarcadorDupla1);
            Assert.Equal(1, rally2Actualizado.MarcadorDupla2);
        }
    }
}
