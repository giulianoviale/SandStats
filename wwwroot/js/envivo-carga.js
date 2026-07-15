(function () {
    'use strict';

    const app   = document.getElementById('envivo-app');
    const setId = app.dataset.setId;

    // ── DOM refs ──────────────────────────────────────────────────────────────
    const elMarcadorD1   = document.getElementById('marcador-d1');
    const elMarcadorD2   = document.getElementById('marcador-d2');
    const elNumeroRally  = document.getElementById('numero-rally');
    const elBadgeEstado  = document.getElementById('badge-estado');
    const elBtnAbrir     = document.getElementById('btn-abrir-rally');
    const elSugerencia   = document.getElementById('panel-sugerencia');
    const elSugContenido = document.getElementById('sugerencia-contenido');
    const elTablaBody    = document.getElementById('tabla-acciones');
    const elFilaVacia    = document.getElementById('fila-vacia');
    const elError        = document.getElementById('msg-error');

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

    // ── Render ────────────────────────────────────────────────────────────────

    function claseCalidad(calidad) {
        if (!calidad) return 'calidad-null';
        if (calidad === 'DoblePositivo') return 'calidad-dp';
        if (calidad === 'Positivo')      return 'calidad-p';
        if (calidad === 'Negativo')      return 'calidad-n';
        if (calidad === 'DobleNegativo') return 'calidad-dn';
        return '';
    }

    function renderEstado(estado) {
        ocultarError();

        elMarcadorD1.textContent = estado.marcadorDupla1;
        elMarcadorD2.textContent = estado.marcadorDupla2;
        elNumeroRally.textContent = estado.numeroRally;

        if (estado.cerrado) {
            elBadgeEstado.innerHTML = '<span class="badge bg-secondary">Rally cerrado</span>';
            elBtnAbrir.style.display = '';
            elSugerencia.style.display = 'none';
        } else {
            elBadgeEstado.innerHTML = '<span class="badge bg-success">Rally en juego</span>';
            elBtnAbrir.style.display = 'none';

            if (estado.sugerencia) {
                const opc = estado.sugerencia.opciones;
                if (opc && opc.length > 0) {
                    const html = opc.map(o => {
                        const jugador = o.jugadorNombre ? `<span class="text-muted"> — ${o.jugadorNombre}</span>` : '';
                        return `<div>▸ <strong>${o.fundamento}</strong>${jugador}</div>`;
                    }).join('');
                    elSugContenido.innerHTML = html;
                    elSugerencia.style.display = '';
                } else {
                    elSugerencia.style.display = 'none';
                }
            } else {
                elSugerencia.style.display = 'none';
            }
        }

        // Acciones
        const acciones = estado.acciones || [];
        // Limpiar filas anteriores (excepto fila vacía)
        Array.from(elTablaBody.querySelectorAll('tr:not(#fila-vacia)')).forEach(r => r.remove());

        if (acciones.length === 0) {
            elFilaVacia.style.display = '';
        } else {
            elFilaVacia.style.display = 'none';
            acciones.forEach(a => {
                const tr = document.createElement('tr');
                const calidad = a.calidad || '—';
                tr.innerHTML = `
                    <td>${a.secuencia}</td>
                    <td>${a.fundamento}</td>
                    <td>${a.jugadorNombre || a.jugadorId}</td>
                    <td class="${claseCalidad(a.calidad)}">${calidad}</td>
                `;
                elTablaBody.appendChild(tr);
            });
        }
    }

    // ── API calls ─────────────────────────────────────────────────────────────

    async function fetchEstado() {
        const data = await apiFetch(`/api/envivo/sets/${setId}/estado`);
        if (data) renderEstado(data);
    }

    window.abrirRally = async function () {
        elBtnAbrir.disabled = true;
        const data = await apiFetch(`/api/envivo/sets/${setId}/rallies`, { method: 'POST' });
        elBtnAbrir.disabled = false;
        if (data) await fetchEstado();
    };

    // ── Init ──────────────────────────────────────────────────────────────────
    fetchEstado();
}());
