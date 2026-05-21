using InventarioGanadero.Api.Models.Dtos;

namespace InventarioGanadero.Api.Services.Reproduccion;

public interface IPartoService
{
    Task<(bool Ok, string? Error, int? IdParto)> RegistrarPartoAsync(RegistrarPartoDto dto, CancellationToken ct = default);

    Task<List<PartoHistorialDto>> ObtenerHistorialAsync(int pagina, int tamanoPagina, CancellationToken ct = default);

    Task<int> ContarHistorialAsync(CancellationToken ct = default);

    Task<int> ContarPartosAsync(CancellationToken ct = default);

    Task<int> ContarVacasConCriasAsync(CancellationToken ct = default);

    Task<List<UltimoPartoDto>> ObtenerUltimosPartosAsync(int cantidad = 5, CancellationToken ct = default);

    Task<List<object>> BuscarMadresAsync(string? termino, CancellationToken ct = default);
}
