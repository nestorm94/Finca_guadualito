using InventarioGanadero.Api.Models.ViewModels;

namespace InventarioGanadero.Api.Services.Historial;

public interface IAnimalHistorialService
{
    Task<AnimalDetalleViewModel?> ObtenerDetalleAsync(int numero, int anio, CancellationToken ct = default);
}
