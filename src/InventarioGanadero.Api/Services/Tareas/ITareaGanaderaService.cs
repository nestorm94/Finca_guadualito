using InventarioGanadero.Api.Models.ViewModels;

namespace InventarioGanadero.Api.Services.Tareas;

public interface ITareaGanaderaService
{
    Task<(bool Ok, string? Error, TareaMasivaResultadoViewModel? Resultado)> AplicarTareaMasivaAsync(
        RegistrarTareaMasivaViewModel model, CancellationToken ct = default);

    Task<TareaMasivaResultadoViewModel?> ObtenerResultadoTareaAsync(int idTarea, CancellationToken ct = default);
}
