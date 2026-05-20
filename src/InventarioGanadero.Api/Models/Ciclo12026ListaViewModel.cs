using InventarioGanadero.Infrastructure.Entities;

namespace InventarioGanadero.Api.Models;

public class Ciclo12026ListaViewModel
{
    public IList<Ciclo12026> Registros { get; init; } = [];
    public string? FiltroPropietario { get; init; }
    public string? FiltroLote { get; init; }
    public int Pagina { get; init; } = 1;
    public int TotalPaginas { get; init; }
    public int TotalRegistros { get; init; }
    public const int TamanoPagina = 10;
}
