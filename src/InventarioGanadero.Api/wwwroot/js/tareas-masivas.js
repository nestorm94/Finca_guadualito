(function () {
    const tipo = document.getElementById('tipoTarea');
    const panels = {
        VACUNACION: document.getElementById('panelVacunacion'),
        PALPACION: document.getElementById('panelPalpacion'),
        TRATAMIENTO: document.getElementById('panelTratamiento'),
        CAMBIO_LOTE: document.getElementById('panelCambioLote')
    };
    function toggle() {
        const v = tipo?.value || 'PALPACION';
        Object.keys(panels).forEach(function (k) {
            if (panels[k]) panels[k].style.display = k === v ? '' : 'none';
        });
    }
    if (tipo) {
        tipo.addEventListener('change', toggle);
        toggle();
    }
})();
