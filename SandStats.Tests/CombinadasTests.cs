using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class CombinadasTests
    {
        private const int D1 = 1, D2 = 2, A = 10;

        private static readonly IReadOnlyList<ModificadorCombinada> TodasLasCombinadas =
        [
            new() { Id =  1, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Negativo      },
            new() { Id =  2, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Negativo      },
            new() { Id =  3, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Exclamativa   },
            new() { Id =  4, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Slash         },
            new() { Id =  5, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Positivo      },
            new() { Id =  6, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.DoblePositivo },
            new() { Id =  7, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Slash         },
            new() { Id =  8, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id =  9, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 10, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 11, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
            new() { Id = 12, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id = 13, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id = 14, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 15, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 16, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 17, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
        ];

        public static IEnumerable<object[]> FilasSeed =>
        [
            [Fundamento.Recepcion, Calidad.DoblePositivo, Fundamento.Saque,  Calidad.Negativo     ],
            [Fundamento.Recepcion, Calidad.Positivo,      Fundamento.Saque,  Calidad.Negativo     ],
            [Fundamento.Recepcion, Calidad.Exclamativa,   Fundamento.Saque,  Calidad.Exclamativa  ],
            [Fundamento.Recepcion, Calidad.Slash,         Fundamento.Saque,  Calidad.Slash        ],
            [Fundamento.Recepcion, Calidad.Negativo,      Fundamento.Saque,  Calidad.Positivo     ],
            [Fundamento.Recepcion, Calidad.DobleNegativo, Fundamento.Saque,  Calidad.DoblePositivo],
            [Fundamento.Bloqueo,   Calidad.DoblePositivo, Fundamento.Ataque, Calidad.Slash        ],
            [Fundamento.Bloqueo,   Calidad.Positivo,      Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Bloqueo,   Calidad.Slash,         Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Bloqueo,   Calidad.Negativo,      Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Bloqueo,   Calidad.DobleNegativo, Fundamento.Ataque, Calidad.DoblePositivo],
            [Fundamento.Defensa,   Calidad.DoblePositivo, Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Defensa,   Calidad.Positivo,      Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Defensa,   Calidad.Exclamativa,   Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.Slash,         Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.Negativo,      Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.DobleNegativo, Fundamento.Ataque, Calidad.DoblePositivo],
        ];

        private static ContextoRally CtxConCombinadas() => new()
        {
            Dupla1Id = D1,
            Dupla2Id = D2,
            JugadoresPorDupla = new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
            {
                [D1] = [new JugadorEnCancha(A, PosicionJugador.Bloqueador), new JugadorEnCancha(11, PosicionJugador.Defensor)],
                [D2] = [new JugadorEnCancha(20, PosicionJugador.Bloqueador), new JugadorEnCancha(21, PosicionJugador.Defensor)]
            },
            SacadorInicialDupla1JugadorId = A,
            SacadorInicialDupla2JugadorId = 20,
            DuplaQueSacaPrimeroId = D1,
            GanadoresRalliesPrevios = Array.Empty<int>(),
            AccionesRallyActual = Array.Empty<Accion>(),
            Combinadas = TodasLasCombinadas
        };

        [Theory]
        [MemberData(nameof(FilasSeed))]
        public void Combinada_DerivаLaCalidadCorrecta(
            Fundamento fc, Calidad cc, Fundamento fd, Calidad cd)
        {
            var resultado = new MotorRally().Registrar(CtxConCombinadas(), new CargaAccion(fc, cc, A, false));
            Assert.Equal(new Derivacion(fd, cd), resultado.Derivacion);
        }
    }
}
