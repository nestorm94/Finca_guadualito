(function () {
    const buscar = document.getElementById('buscarMadre');
    const madreSelect = document.getElementById('madreKey');
    const cantidad = document.getElementById('cantidadCrias');
    const container = document.getElementById('criasContainer');
    const fechaParto = document.getElementById('fechaParto');

    if (buscar && madreSelect) {
        const options = Array.from(madreSelect.options).slice(1);
        buscar.addEventListener('input', function () {
            const q = buscar.value.trim().toLowerCase();
            madreSelect.innerHTML = '<option value="">— Seleccione la madre —</option>';
            options.forEach(function (opt) {
                if (!q || opt.text.toLowerCase().includes(q) || opt.value.includes(q.replace('/', '|'))) {
                    madreSelect.appendChild(opt.cloneNode(true));
                }
            });
        });
    }

    function toggleModo(panel) {
        const modo = panel.querySelector('.modo-cria');
        const nueva = panel.querySelector('.bloque-nueva');
        const existente = panel.querySelector('.bloque-existente');
        if (!modo || !nueva || !existente) return;
        const esExistente = modo.value === 'Existente';
        nueva.style.display = esExistente ? 'none' : '';
        existente.style.display = esExistente ? '' : 'none';
    }

    function bindPanels() {
        document.querySelectorAll('.cria-panel').forEach(function (panel) {
            const modo = panel.querySelector('.modo-cria');
            if (modo && !modo.dataset.bound) {
                modo.dataset.bound = '1';
                modo.addEventListener('change', function () { toggleModo(panel); });
                toggleModo(panel);
            }
        });
    }

    function rebuildCrias() {
        if (!cantidad || !container) return;
        const n = Math.min(10, Math.max(1, parseInt(cantidad.value, 10) || 1));
        const panels = container.querySelectorAll('.cria-panel');
        if (panels.length === n) { bindPanels(); return; }

        const template = panels[0];
        if (!template) return;

        while (container.children.length < n) {
            const clone = template.cloneNode(true);
            const idx = container.children.length;
            clone.dataset.index = idx;
            clone.querySelector('h4').textContent = 'Cría ' + (idx + 1);
            clone.querySelectorAll('[name]').forEach(function (el) {
                el.name = el.name.replace(/\[\d+\]/, '[' + idx + ']');
                if (el.tagName === 'SELECT' && el.classList.contains('modo-cria')) el.value = 'Nueva';
                else if (el.type === 'number' || el.type === 'text') el.value = '';
            });
            container.appendChild(clone);
        }
        while (container.children.length > n) {
            container.removeChild(container.lastElementChild);
        }
        bindPanels();
    }

    if (cantidad) {
        cantidad.addEventListener('change', rebuildCrias);
        rebuildCrias();
    }
    bindPanels();
})();
