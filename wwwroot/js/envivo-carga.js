(function () {
    'use strict';

    // ── Constantes desde el DOM ───────────────────────────────────────────────
    const app       = document.getElementById('envivo-app');
    const setId     = +app.dataset.setId;
    const d1Id      = +app.dataset.dupla1Id;
    const d2Id      = +app.dataset.dupla2Id;
    const partidoId = +app.dataset.partidoId;
    const d1Nombre  = app.dataset.dupla1Nombre;
    const d2Nombre  = app.dataset.dupla2Nombre;
    const jugadores = JSON.parse(app.dataset.jugadores || '{}');

    // ── Estado mutable ────────────────────────────────────────────────────────
    let ultimoRallyId = null;

    // ── DOM refs ──────────────────────────────────────────────────────────────
    const elMarcadorD1      = document.getElementById('marcador-d1');
    const elMarcadorD2      = document.getElementById('marcador-d2');
    const elNumeroRally     = document.getElementById('numero-rally');
    const elBadgeEstado     = document.getElementById('badge-estado');
    const elBtnAbrir        = document.getElementById('btn-abrir-rally');
    const elPanelCarga      = document.getElementById('panel-carga');
    const elPanelCierre     = document.getElementById('panel-cierre');
    const elBannerGanadora  = document.getElementById('banner-ganadora');
    const elAvisoCambioLado = document.getElementById('aviso-cambio-lado');
    const elBtnSiguiente    = document.getElementById('btn-siguiente-rally');
    const elPanelFinSet     = document.getElementById('panel-fin-set');
    const elPanelFinPartido = document.getElementById('panel-fin-partido');
    const elTablaBody       = document.getElementById('tabla-acciones');
    const elFilaVacia       = document.getElementById('fila-vacia');
    const elBtnDeshacer     = document.getElementById('btn-deshacer');
    const elError           = document.getElementById('msg-error');

    // ── Fetch helpers ─────────────────────────────────────────────────────────

    async function apiFetch(url, options) {
        let res;
        try {
            res = await fetch(url, options);
        } catch (e) {
            mostrarError('Error de red: ' + e.message);
            return null;
        }

        if (res.status === 401) {
            mostrarError('Sesión vencida. <a href="/Identity/Account/Login">Volvé a entrar</a>.');
            return null;
        }

        if (!res.ok) {
            let detalle = res.statusText;
            try {
                const prob = await res.json();
                detalle = prob.detail || prob.title || detalle;
            } catch (_) { /* no es JSON */ }
            mostrarError('Error ' + res.status + ': ' + detalle);
            return null;
        }

        if (res.status === 204) return {};
        return res.json();
    }

    function mostrarError(html) {
        elError.innerHTML = html;
        elError.style.display = '';
    }

    function ocultarError() {
        elError.style.display = 'none';
    }

    // ── Funciones de API (globales para onclick) ──────────────────────────────

    window.registrarAccion = async function (payload) {
        ocultarError();
        const data = await apiFetch(`/api/envivo/rallies/${ultimoRallyId}/acciones`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        if (data) renderEstado(data);
    };

    window.cierreDirecto = async function (tipo, duplaGanadoraId, extras) {
        ocultarError();
        const data = await apiFetch(`/api/envivo/rallies/${ultimoRallyId}/cierre-directo`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ tipo, duplaGanadoraId, ...extras })
        });
        if (data) renderEstado(data);
    };

    window.cierreRapido = function (duplaGanadoraId) {
        window.cierreDirecto('CierreRapido', duplaGanadoraId);
    };

    window.abrirRally = async function () {
        ocultarError();
        elBtnAbrir.disabled = true;
        const data = await apiFetch(`/api/envivo/sets/${setId}/rallies`, { method: 'POST' });
        elBtnAbrir.disabled = false;
        if (data) {
            ultimoRallyId = data.id;
            await fetchEstado();
        }
    };

    window.siguienteRally = async function () {
        ocultarError();
        const data = await apiFetch(`/api/envivo/sets/${setId}/rallies`, { method: 'POST' });
        if (data) {
            ultimoRallyId = data.id;
            await fetchEstado();
        }
    };

    window.confirmarFinSet = async function (duplaGanadoraId) {
        ocultarError();
        const ok = await apiFetch(`/api/envivo/sets/${setId}/cerrar`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ duplaGanadoraId })
        });
        if (ok !== null) {
            window.location.href = `/EnVivo/IniciarSet/${partidoId}`;
        }
    };

    window.deshacer = async function () {
        ocultarError();
        const data = await apiFetch(`/api/envivo/rallies/${ultimoRallyId}/deshacer`, {
            method: 'POST'
        });
        if (data) renderEstado(data);
    };

    // ── Render principal ──────────────────────────────────────────────────────

    function claseCalidad(calidad) {
        if (!calidad) return 'calidad-null';
        switch (calidad) {
            case 'DoblePositivo': return 'calidad-dp';
            case 'Positivo':     return 'calidad-p';
            case 'Exclamativa':  return 'calidad-ex';
            case 'Slash':        return 'calidad-sl';
            case 'Negativo':     return 'calidad-n';
            case 'DobleNegativo': return 'calidad-dn';
        }
        return '';
    }

    function simboloCalidad(calidad) {
        if (!calidad) return '—';
        switch (calidad) {
            case 'DoblePositivo': return '#';
            case 'Positivo':     return '+';
            case 'Exclamativa':  return '!';
            case 'Slash':        return '/';
            case 'Negativo':     return '−';
            case 'DobleNegativo': return '=';
        }
        return calidad;
    }

    function renderEstado(estado) {
        ocultarError();

        if (estado.rallyId > 0) ultimoRallyId = estado.rallyId;

        // Marcador y rally
        elMarcadorD1.textContent  = estado.marcadorDupla1;
        elMarcadorD2.textContent  = estado.marcadorDupla2;
        elNumeroRally.textContent = estado.numeroRally || '—';

        // Tabla de acciones
        Array.from(elTablaBody.querySelectorAll('tr:not(#fila-vacia)')).forEach(r => r.remove());
        const acciones = estado.acciones || [];
        if (acciones.length === 0) {
            elFilaVacia.style.display = '';
        } else {
            elFilaVacia.style.display = 'none';
            acciones.forEach(a => {
                const tr = document.createElement('tr');
                const cls = claseCalidad(a.calidad);
                const sim = simboloCalidad(a.calidad);
                tr.innerHTML = `
                    <td>${a.secuencia}</td>
                    <td>${a.fundamento}</td>
                    <td>${a.jugadorNombre || a.jugadorId}</td>
                    <td class="${cls}">${sim}</td>`;
                elTablaBody.appendChild(tr);
            });
        }

        const m = estado.marcador || {};
        const setTerminado    = m.setTerminado    || false;
        const partidoTerminado = m.partidoTerminado || false;

        if (estado.cerrado) {
            // Badge
            elBadgeEstado.innerHTML = '<span class="badge bg-secondary">Rally cerrado</span>';
            elBtnAbrir.style.display = 'none';

            // Ocultar panel carga, mostrar cierre
            elPanelCarga.style.display  = 'none';
            elPanelCierre.style.display = '';

            // Banner ganadora
            const ganadoraId = estado.duplaGanadoraId;
            const ganadoraNombre = ganadoraId === d1Id ? d1Nombre : d2Nombre;
            elBannerGanadora.textContent = ganadoraNombre ? `Ganó ${ganadoraNombre}` : '';

            // Cambio de lado
            elAvisoCambioLado.style.display = m.cambioDeLado ? '' : 'none';

            // Botón siguiente rally
            if (setTerminado || partidoTerminado) {
                elBtnSiguiente.style.display = 'none';
            } else {
                elBtnSiguiente.style.display = '';
            }

            // Fin de set
            if (setTerminado && !partidoTerminado) {
                elPanelFinSet.style.display  = '';
                elPanelFinSet.innerHTML =
                    `<button class="btn btn-warning w-100 mt-1"
                        onclick="confirmarFinSet(${m.duplaGanadoraSetId})">
                        Confirmar fin de set
                    </button>`;
            } else {
                elPanelFinSet.style.display = 'none';
            }

            // Fin de partido
            elPanelFinPartido.style.display = partidoTerminado ? '' : 'none';

            // Deshacer siempre visible cuando hay un rally cerrado
            elBtnDeshacer.style.display = ultimoRallyId ? '' : 'none';

        } else {
            // Badge
            elBadgeEstado.innerHTML = '<span class="badge bg-success">Rally en juego</span>';
            elBtnAbrir.style.display = 'none';

            // Panel carga
            elPanelCierre.style.display = 'none';
            elPanelCarga.style.display  = '';
            renderPanel(estado.sugerencia);

            // Deshacer solo si hay acciones
            elBtnDeshacer.style.display = acciones.length > 0 ? '' : 'none';
        }
    }

    // ── Panel dinámico según sugerencia ──────────────────────────────────────

    function renderPanel(sug) {
        elPanelCarga.innerHTML = '';
        if (!sug || !sug.opciones || sug.opciones.length === 0) return;

        const f = sug.opciones[0].fundamento;
        if (f === 'Saque')     buildPanelSaque(sug);
        else if (f === 'Recepcion') buildPanelRecepcion(sug);
        else if (f === 'Ataque')   buildPanelAtaque(sug);
        else                        buildPanelBloqueoDef(sug);
    }

    // ── Helpers de construcción de HTML ──────────────────────────────────────

    function makeBtn(texto, clases, attrs) {
        const b = document.createElement('button');
        b.type = 'button';
        b.className = clases;
        b.innerHTML = texto;
        if (attrs) Object.entries(attrs).forEach(([k, v]) => b.setAttribute(k, v));
        return b;
    }

    function seccion(titulo) {
        const d = document.createElement('div');
        d.className = 'text-muted small mb-1 mt-2';
        d.textContent = titulo;
        return d;
    }

    // Grupo de botones exclusivos con toggle .btn-sel
    function grupoBotones(items, claseBtn, onSelect) {
        // items: [{valor, texto}]
        const wrap = document.createElement('div');
        wrap.className = 'd-flex gap-2 flex-wrap mb-2';
        items.forEach(item => {
            const b = makeBtn(item.texto, `btn ${claseBtn}`);
            b.addEventListener('click', () => {
                wrap.querySelectorAll('button').forEach(x => x.classList.remove('btn-sel'));
                b.classList.add('btn-sel');
                onSelect(item.valor, b);
            });
            wrap.appendChild(b);
        });
        return wrap;
    }

    function botonesCalidad(onCalidad) {
        const CALIDADES = [
            { val: 'DoblePositivo', sim: '#', cls: 'cal-dp' },
            { val: 'Positivo',      sim: '+', cls: 'cal-p'  },
            { val: 'Exclamativa',   sim: '!', cls: 'cal-ex' },
            { val: 'Slash',         sim: '/', cls: 'cal-sl' },
            { val: 'Negativo',      sim: '−', cls: 'cal-n'  },
            { val: 'DobleNegativo', sim: '=', cls: 'cal-dn' }
        ];
        const wrap = document.createElement('div');
        wrap.className = 'd-flex gap-1 flex-wrap';
        CALIDADES.forEach(c => {
            const b = makeBtn(c.sim, `btn btn-sm ${c.cls} fw-bold`);
            b.title = c.val;
            b.style.minWidth = '2.4rem';
            b.addEventListener('click', () => onCalidad(c.val));
            wrap.appendChild(b);
        });
        return wrap;
    }

    // ── buildPanelSaque ───────────────────────────────────────────────────────

    function buildPanelSaque(sug) {
        const opc = sug.opciones[0];
        let zona = null, tipo = null;

        const el = elPanelCarga;
        el.innerHTML = `<div class="fw-semibold mb-2">Saque — ${opc.jugadorNombre || ''}</div>`;

        // Zona
        el.appendChild(seccion('Zona de saque'));
        el.appendChild(grupoBotones(
            [{ valor: '1', texto: '1' }, { valor: '5', texto: '5' }, { valor: '6', texto: '6' }],
            'btn-outline-secondary btn-lg',
            (v) => { zona = v; checkEnJuego(); }
        ));

        // Tipo
        el.appendChild(seccion('Tipo'));
        el.appendChild(grupoBotones(
            [{ valor: 'Flotado', texto: 'Flotado' }, { valor: 'Potencia', texto: 'Potencia' }],
            'btn-outline-secondary',
            (v) => { tipo = v; checkEnJuego(); }
        ));

        // Botones de desenlace
        const wrapDes = document.createElement('div');
        wrapDes.className = 'd-grid gap-2 mt-3';

        const btnError = makeBtn('Error de saque', 'btn btn-danger');
        btnError.addEventListener('click', () => {
            if (!zona || !tipo) { mostrarError('Elegí zona y tipo primero.'); return; }
            ocultarError();
            window.cierreDirecto('ErrorSaque', null, { zonaSaque: 'Zona' + zona, tipoSaque: tipo });
        });

        const btnAce = makeBtn('Ace', 'btn btn-success');
        btnAce.addEventListener('click', () => {
            if (!zona || !tipo) { mostrarError('Elegí zona y tipo primero.'); return; }
            ocultarError();
            window.cierreDirecto('Ace', null, { zonaSaque: 'Zona' + zona, tipoSaque: tipo });
        });

        const btnEJ = makeBtn('En juego', 'btn btn-primary');
        btnEJ.id = 'btn-en-juego';
        btnEJ.disabled = true;
        btnEJ.addEventListener('click', () => {
            window.registrarAccion({
                fundamento: 'Saque', calidad: null,
                jugadorId: opc.jugadorSugeridoId, esDe2da: false,
                detalle: { zonaSaque: 'Zona' + zona, tipoSaque: tipo }
            });
        });

        wrapDes.appendChild(btnError);
        wrapDes.appendChild(btnAce);
        wrapDes.appendChild(btnEJ);
        el.appendChild(wrapDes);

        function checkEnJuego() {
            btnEJ.disabled = !(zona && tipo);
        }
    }

    // ── buildPanelRecepcion ───────────────────────────────────────────────────

    function buildPanelRecepcion(sug) {
        let jugadorId = null, tipo = null;
        const dupJugadores = jugadores[sug.duplaId] || [];

        const el = elPanelCarga;
        el.innerHTML = '<div class="fw-semibold mb-2">Recepción</div>';

        // Jugadores
        el.appendChild(seccion('Receptor'));
        const wrapJug = document.createElement('div');
        wrapJug.className = 'd-flex gap-2 mb-2';
        dupJugadores.forEach(j => {
            const b = makeBtn(j.nombre, 'btn btn-outline-primary btn-lg flex-fill');
            b.addEventListener('click', () => {
                wrapJug.querySelectorAll('button').forEach(x => x.classList.remove('btn-sel'));
                b.classList.add('btn-sel');
                jugadorId = j.id;
            });
            wrapJug.appendChild(b);
        });
        el.appendChild(wrapJug);

        // Tipo recepción
        el.appendChild(seccion('Tipo'));
        el.appendChild(grupoBotones(
            ['Adelante', 'Cuerpo', 'Externo', 'Interno'].map(t => ({ valor: t, texto: t })),
            'btn-outline-secondary btn-sm',
            (v) => { tipo = v; }
        ));

        // Calidades
        el.appendChild(seccion('Calidad'));
        el.appendChild(botonesCalidad((calidad) => {
            if (!jugadorId || !tipo) {
                mostrarError('Elegí receptor y tipo antes de registrar la calidad.');
                return;
            }
            ocultarError();
            window.registrarAccion({
                fundamento: 'Recepcion', calidad,
                jugadorId, esDe2da: false,
                detalle: { tipoRecepcion: tipo }
            });
        }));
    }

    // ── buildPanelAtaque ──────────────────────────────────────────────────────

    function buildPanelAtaque(sug) {
        const opc = sug.opciones[0];
        let jugadorId = opc.jugadorSugeridoId;
        let esDe2da   = false;
        let lado = null, golpe = null, zona = null;
        let esVarilla = false;

        const PREFIJOS = { Spike: 'Atq', LineTip: 'Tl', CrossTip: 'Td' };

        const el = elPanelCarga;
        el.innerHTML = '<div class="fw-semibold mb-2">Ataque</div>';

        // Jugador / De 2da
        el.appendChild(seccion('Atacante'));
        const wrapJug = document.createElement('div');
        wrapJug.className = 'd-flex gap-2 flex-wrap mb-2';

        const dupJug = jugadores[sug.duplaId] || [];
        let btn2da = null;

        dupJug.forEach(j => {
            // Si tiene botón "2da" propio, no duplicar como botón genérico
            if (sug.permiteDe2da && j.id === sug.jugadorDe2daId) return;
            const esSugerido = j.id === opc.jugadorSugeridoId;
            const b = makeBtn(j.nombre, `btn btn-outline-primary${esSugerido ? ' btn-sel' : ''}`);
            b.addEventListener('click', () => {
                wrapJug.querySelectorAll('button').forEach(x => x.classList.remove('btn-sel'));
                b.classList.add('btn-sel');
                jugadorId = j.id;
                esDe2da   = false;
                if (btn2da) btn2da.classList.remove('btn-sel');
            });
            wrapJug.appendChild(b);
        });

        if (sug.permiteDe2da && sug.jugadorDe2daNombre) {
            btn2da = makeBtn('2da: ' + sug.jugadorDe2daNombre, 'btn btn-outline-secondary');
            btn2da.addEventListener('click', () => {
                wrapJug.querySelectorAll('button').forEach(x => x.classList.remove('btn-sel'));
                btn2da.classList.add('btn-sel');
                jugadorId = sug.jugadorDe2daId;
                esDe2da   = true;
            });
            wrapJug.appendChild(btn2da);
        }
        el.appendChild(wrapJug);

        // Lado
        el.appendChild(seccion('Lado'));
        const elSecArmado = document.createElement('div'); // referencia para show/hide
        el.appendChild(grupoBotones(
            [{ valor: 'Bueno', texto: 'Bueno' }, { valor: 'Medio', texto: 'Medio' }, { valor: 'Atras', texto: 'Atrás' }],
            'btn-outline-secondary',
            (v) => {
                lado = v;
                elSecArmado.style.display = v === 'Medio' ? 'none' : '';
                checkRegistrar();
            }
        ));

        // Golpe
        el.appendChild(seccion('Golpe'));
        el.appendChild(grupoBotones(
            [{ valor: 'Spike', texto: 'Spike' }, { valor: 'LineTip', texto: 'Line tip' }, { valor: 'CrossTip', texto: 'Cross tip' }],
            'btn-outline-secondary',
            (v) => { golpe = v; checkRegistrar(); }
        ));

        // Zona destino
        el.appendChild(seccion('Zona destino'));
        const zonaGrid = document.createElement('div');
        zonaGrid.className = 'zona-grid mb-2';
        // Layout: fila superior 4-3-2, media 7-8-9, inferior 5-6-1
        [4, 3, 2, 7, 8, 9, 5, 6, 1].forEach(z => {
            const b = makeBtn(String(z), 'btn btn-outline-secondary btn-sm');
            b.addEventListener('click', () => {
                zonaGrid.querySelectorAll('button').forEach(x => x.classList.remove('btn-sel'));
                b.classList.add('btn-sel');
                zona = z;
                checkRegistrar();
            });
            zonaGrid.appendChild(b);
        });
        el.appendChild(zonaGrid);

        // Armado (oculto si Medio)
        const lblArmado = seccion('Armado');
        const wrapArmado = document.createElement('div');
        wrapArmado.className = 'd-flex gap-2 mb-2';

        const btnVarilla = makeBtn('Varilla', 'btn btn-outline-secondary btn-sm');
        btnVarilla.addEventListener('click', () => {
            if (esVarilla) {
                esVarilla = false;
                btnVarilla.classList.remove('btn-sel');
            } else {
                esVarilla = true;
                btnVarilla.classList.add('btn-sel');
                btnEntrePosicion.classList.remove('btn-sel');
            }
        });

        const btnEntrePosicion = makeBtn('Entre posición', 'btn btn-outline-secondary btn-sm');
        btnEntrePosicion.addEventListener('click', () => {
            // Entre posición no activa esVarilla ni esEspecial — reset a ambos false
            esVarilla = false;
            btnVarilla.classList.remove('btn-sel');
            if (btnEntrePosicion.classList.contains('btn-sel')) {
                btnEntrePosicion.classList.remove('btn-sel');
            } else {
                btnEntrePosicion.classList.add('btn-sel');
            }
        });

        wrapArmado.appendChild(btnVarilla);
        wrapArmado.appendChild(btnEntrePosicion);

        elSecArmado.appendChild(lblArmado);
        elSecArmado.appendChild(wrapArmado);
        el.appendChild(elSecArmado);

        // Botones de resultado
        const wrapResult = document.createElement('div');
        wrapResult.className = 'd-flex gap-2 flex-wrap mt-3';

        const btnRegistrar = makeBtn('Registrar (sigue)', 'btn btn-outline-primary');
        btnRegistrar.disabled = true;
        btnRegistrar.addEventListener('click', () => enviarAtaque(null));

        const btnPunto = makeBtn('# Punto directo', 'btn btn-success');
        btnPunto.disabled = true;
        btnPunto.addEventListener('click', () => enviarAtaque('DoblePositivo'));

        const btnError = makeBtn('= Error propio', 'btn btn-danger');
        btnError.disabled = true;
        btnError.addEventListener('click', () => enviarAtaque('DobleNegativo'));

        wrapResult.appendChild(btnRegistrar);
        wrapResult.appendChild(btnPunto);
        wrapResult.appendChild(btnError);
        el.appendChild(wrapResult);

        function checkRegistrar() {
            const habilitado = !!(lado && golpe && zona);
            btnRegistrar.disabled = !habilitado;
            btnPunto.disabled     = !habilitado;
            btnError.disabled     = !habilitado;
        }

        function enviarAtaque(calidad) {
            const tipoAccion = PREFIJOS[golpe] + zona;
            window.registrarAccion({
                fundamento: 'Ataque', calidad,
                jugadorId, esDe2da,
                detalle: {
                    lado,
                    tipoAccion,
                    zonaDestino: 'Zona' + zona,
                    esVarilla,
                    esEspecial: false
                }
            });
        }
    }

    // ── buildPanelBloqueoDef ──────────────────────────────────────────────────

    function buildPanelBloqueoDef(sug) {
        const el = elPanelCarga;
        el.innerHTML = '<div class="fw-semibold mb-2">Bloqueo / Defensa</div>';

        const wrap = document.createElement('div');
        wrap.className = 'd-flex gap-3';

        sug.opciones.forEach(opc => {
            let jugadorIdLocal = opc.jugadorSugeridoId;
            const dupJugLocal  = jugadores[sug.duplaId] || [];
            const companero    = dupJugLocal.find(j => j.id !== opc.jugadorSugeridoId);

            const col = document.createElement('div');
            col.className = 'flex-fill';

            const hdr = document.createElement('div');
            hdr.className = 'fw-semibold small mb-1';

            const spanJug = document.createElement('span');
            spanJug.textContent = `${opc.fundamento} — ${opc.jugadorNombre || ''}`;
            hdr.appendChild(spanJug);

            if (companero) {
                const btnCambio = makeBtn(`↔ ${companero.nombre}`, 'btn btn-link btn-sm p-0 ms-2');
                btnCambio.addEventListener('click', () => {
                    if (jugadorIdLocal === opc.jugadorSugeridoId) {
                        jugadorIdLocal = companero.id;
                        spanJug.textContent = `${opc.fundamento} — ${companero.nombre}`;
                        btnCambio.textContent = `↔ ${opc.jugadorNombre || ''}`;
                    } else {
                        jugadorIdLocal = opc.jugadorSugeridoId;
                        spanJug.textContent = `${opc.fundamento} — ${opc.jugadorNombre || ''}`;
                        btnCambio.textContent = `↔ ${companero.nombre}`;
                    }
                });
                hdr.appendChild(btnCambio);
            }
            col.appendChild(hdr);

            col.appendChild(botonesCalidad((calidad) => {
                window.registrarAccion({
                    fundamento: opc.fundamento, calidad,
                    jugadorId: jugadorIdLocal, esDe2da: false,
                    detalle: null
                });
            }));

            wrap.appendChild(col);
        });

        el.appendChild(wrap);
    }

    // ── Estado inicial ────────────────────────────────────────────────────────

    async function fetchEstado() {
        const data = await apiFetch(`/api/envivo/sets/${setId}/estado`);
        if (!data) return;

        // Estado sin rallies (set recién creado): mostrar botón abrir rally
        if (data.cerrado && data.rallyId === 0) {
            elBtnAbrir.style.display = '';
            elPanelCarga.style.display  = 'none';
            elPanelCierre.style.display = 'none';
            elBadgeEstado.innerHTML = '<span class="badge bg-secondary">Sin rally abierto</span>';
            elMarcadorD1.textContent = '0';
            elMarcadorD2.textContent = '0';
            elNumeroRally.textContent = '—';
            return;
        }

        elBtnAbrir.style.display = 'none';
        renderEstado(data);
    }

    fetchEstado();
}());
