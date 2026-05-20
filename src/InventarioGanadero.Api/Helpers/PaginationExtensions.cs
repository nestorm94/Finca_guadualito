using InventarioGanadero.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Helpers;

public static class PaginationExtensions
{
    public static async Task<ListaPaginadaViewModel<T>> ToListaPaginadaAsync<T>(
        this IQueryable<T> query,
        int pagina,
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1) pagina = 1;

        var total = await query.CountAsync(cancellationToken);
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)ListaPaginadaViewModel<T>.TamanoPagina));
        if (pagina > totalPaginas) pagina = totalPaginas;

        var items = await query
            .Skip((pagina - 1) * ListaPaginadaViewModel<T>.TamanoPagina)
            .Take(ListaPaginadaViewModel<T>.TamanoPagina)
            .ToListAsync(cancellationToken);

        return new ListaPaginadaViewModel<T>
        {
            Items = items,
            Pagina = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = total
        };
    }
}
