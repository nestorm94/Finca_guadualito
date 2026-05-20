namespace InventarioGanadero.Api.Models;

public class ListaPaginadaViewModel<T>
{
    public IList<T> Items { get; init; } = [];
    public int Pagina { get; init; } = 1;
    public int TotalPaginas { get; init; }
    public int TotalRegistros { get; init; }
    public const int TamanoPagina = 10;
}
