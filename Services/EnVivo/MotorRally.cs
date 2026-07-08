namespace SandStats.Services.EnVivo
{
    public class MotorRally
    {
        public int QuienSaca(ContextoRally ctx)
        {
            int duplaServante = ctx.GanadoresRalliesPrevios.Count == 0
                ? ctx.DuplaQueSacaPrimeroId
                : ctx.GanadoresRalliesPrevios[^1];

            // Contar adquisiciones del saque por dupla
            var adquisiciones = new Dictionary<int, int>
            {
                [ctx.Dupla1Id] = 0,
                [ctx.Dupla2Id] = 0
            };
            adquisiciones[ctx.DuplaQueSacaPrimeroId] = 1;

            int sacadorActual = ctx.DuplaQueSacaPrimeroId;
            foreach (int ganador in ctx.GanadoresRalliesPrevios)
            {
                if (ganador != sacadorActual)
                {
                    sacadorActual = ganador;
                    adquisiciones[ganador]++;
                }
            }

            int sacadorInicial = duplaServante == ctx.Dupla1Id
                ? ctx.SacadorInicialDupla1JugadorId
                : ctx.SacadorInicialDupla2JugadorId;

            // Impar → sacador inicial; par → compañero
            if (adquisiciones[duplaServante] % 2 == 1)
                return sacadorInicial;

            return ctx.JugadoresPorDupla[duplaServante]
                .First(j => j.JugadorId != sacadorInicial)
                .JugadorId;
        }
    }
}
