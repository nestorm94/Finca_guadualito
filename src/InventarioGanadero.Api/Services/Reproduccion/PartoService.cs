using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Models.Dtos;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Services.Reproduccion;

public class PartoService(RegistroGanaderoDbContext db) : IPartoService
{
    public async Task<(bool Ok, string? Error, int? IdParto)> RegistrarPartoAsync(
        RegistrarPartoDto dto, CancellationToken ct = default)
    {
        if (dto.Crias.Count != dto.CantidadCrias)
            return (false, "Debe registrar una cría por cada cantidad indicada.", null);

        var madre = await db.Animales
            .Include(a => a.TipoAnimal)
            .FirstOrDefaultAsync(a => a.Numero == dto.NumeroMadre && a.Anio == dto.AnioMadre && a.Activo, ct);

        if (madre is null)
            return (false, "La madre no existe o no está activa.", null);

        if (madre.Sexo != "HEMBRA")
            return (false, "El animal seleccionado debe ser hembra.", null);

        var idEstadoActivo = await db.EstadosAnimal.AsNoTracking()
            .Where(e => e.Nombre.ToUpper() == "ACTIVO")
            .Select(e => e.IdEstadoAnimal)
            .FirstOrDefaultAsync(ct);

        if (idEstadoActivo == 0)
            return (false, "No existe el estado ACTIVO en el catálogo.", null);

        var tipos = await db.TiposAnimales.AsNoTracking().ToListAsync(ct);
        var idBecerro = tipos.FirstOrDefault(t => Normalizar(t.Nombre) == "BECERRO")?.IdTipoAnimal;
        var idBecerra = tipos.FirstOrDefault(t => Normalizar(t.Nombre) == "BECERRA")?.IdTipoAnimal;

        if (idBecerro is null || idBecerra is null)
            return (false, "Debe existir BECERRO y BECERRA en TiposAnimales.", null);

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var parto = new Parto
            {
                NumeroMadre = dto.NumeroMadre,
                AnioMadre = dto.AnioMadre,
                FechaParto = dto.FechaParto.Date,
                CantidadCrias = dto.CantidadCrias,
                TipoParto = dto.TipoParto,
                EstadoParto = dto.EstadoParto,
                Observacion = dto.Observacion,
                FechaRegistro = DateTime.Now
            };

            db.Partos.Add(parto);
            await db.SaveChangesAsync(ct);

            foreach (var criaDto in dto.Crias)
            {
                int numeroCria;
                int anioCria = dto.FechaParto.Year;

                if (string.Equals(criaDto.Modo, "Existente", StringComparison.OrdinalIgnoreCase))
                {
                    var key = ParseKey(criaDto.AnimalKeyExistente);
                    if (key is null)
                    {
                        await tx.RollbackAsync(ct);
                        return (false, "Seleccione la cría existente.", null);
                    }

                    var existente = await db.Animales.FindAsync([key.Value.Numero, key.Value.Anio], ct);
                    if (existente is null)
                    {
                        await tx.RollbackAsync(ct);
                        return (false, $"La cría {key.Value.Numero}/{key.Value.Anio} no existe.", null);
                    }

                    numeroCria = key.Value.Numero;
                    anioCria = key.Value.Anio;
                }
                else
                {
                    if (criaDto.Sexo is not ("MACHO" or "HEMBRA"))
                    {
                        await tx.RollbackAsync(ct);
                        return (false, "Sexo de la cría debe ser MACHO o HEMBRA.", null);
                    }

                    numeroCria = criaDto.NumeroNuevo ?? await SiguienteNumeroAsync(anioCria, ct);
                    if (await db.Animales.AnyAsync(a => a.Numero == numeroCria && a.Anio == anioCria, ct))
                    {
                        await tx.RollbackAsync(ct);
                        return (false, $"Ya existe el animal {numeroCria}/{anioCria}.", null);
                    }

                    var idTipo = criaDto.Sexo == "MACHO" ? idBecerro.Value : idBecerra.Value;
                    var bimestre = BimestreNacimientoHelper.Calcular(dto.FechaParto);

                    var nuevaCria = new Animal
                    {
                        Numero = numeroCria,
                        Anio = anioCria,
                        IdPropietario = madre.IdPropietario,
                        IdTipoAnimal = idTipo,
                        IdEstadoAnimal = idEstadoActivo,
                        IdLote = madre.IdLote,
                        IdRaza = madre.IdRaza,
                        IdColor = criaDto.IdColor,
                        Sexo = criaDto.Sexo,
                        FechaNacimiento = dto.FechaParto.Date,
                        BimestreNacimiento = bimestre,
                        FechaIngreso = dto.FechaParto.Date,
                        Activo = true,
                        FechaRegistro = DateTime.Now,
                        Observacion = $"Cría registrada por parto #{parto.IdParto}"
                    };

                    db.Animales.Add(nuevaCria);
                    await db.SaveChangesAsync(ct);
                }

                db.RelacionesMadreCria.Add(new RelacionMadreCria
                {
                    IdParto = parto.IdParto,
                    NumeroMadre = dto.NumeroMadre,
                    AnioMadre = dto.AnioMadre,
                    NumeroCria = numeroCria,
                    AnioCria = anioCria
                });
            }

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, null, parto.IdParto);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return (false, ex.Message, null);
        }
    }

    public async Task<List<PartoHistorialDto>> ObtenerHistorialAsync(
        int pagina, int tamanoPagina, CancellationToken ct = default)
    {
        var skip = (pagina - 1) * tamanoPagina;

        var partos = await db.Partos.AsNoTracking()
            .Include(p => p.Madre!)
                .ThenInclude(m => m.TipoAnimal)
            .Include(p => p.RelacionesCrias)
                .ThenInclude(r => r.Cria)
            .OrderByDescending(p => p.FechaParto)
            .ThenByDescending(p => p.IdParto)
            .Skip(skip)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return partos.Select(p => new PartoHistorialDto
        {
            IdParto = p.IdParto,
            Madre = $"{p.NumeroMadre}/{p.AnioMadre}" + (p.Madre?.TipoAnimal != null ? $" ({p.Madre.TipoAnimal.Nombre})" : ""),
            FechaParto = p.FechaParto,
            CantidadCrias = p.CantidadCrias,
            TipoParto = p.TipoParto,
            EstadoParto = p.EstadoParto,
            CriasAsociadas = p.RelacionesCrias
                .Select(r => $"{r.NumeroCria}/{r.AnioCria}")
                .ToList()
        }).ToList();
    }

    public Task<int> ContarHistorialAsync(CancellationToken ct = default) =>
        db.Partos.AsNoTracking().CountAsync(ct);

    public Task<int> ContarPartosAsync(CancellationToken ct = default) =>
        db.Partos.AsNoTracking().CountAsync(ct);

    public async Task<int> ContarVacasConCriasAsync(CancellationToken ct = default)
    {
        var idsVacas = await db.TiposAnimales.AsNoTracking()
            .Where(t => t.Nombre.ToUpper() == "VACA")
            .Select(t => t.IdTipoAnimal)
            .ToListAsync(ct);

        return await (
            from r in db.RelacionesMadreCria.AsNoTracking()
            join a in db.Animales.AsNoTracking()
                on new { r.NumeroMadre, r.AnioMadre } equals new { NumeroMadre = a.Numero, AnioMadre = a.Anio }
            where idsVacas.Contains(a.IdTipoAnimal) && a.Activo
            select new { a.Numero, a.Anio }
        ).Distinct().CountAsync(ct);
    }

    public async Task<List<UltimoPartoDto>> ObtenerUltimosPartosAsync(int cantidad = 5, CancellationToken ct = default) =>
        await db.Partos.AsNoTracking()
            .OrderByDescending(p => p.FechaParto)
            .ThenByDescending(p => p.IdParto)
            .Take(cantidad)
            .Select(p => new UltimoPartoDto
            {
                IdParto = p.IdParto,
                Madre = p.NumeroMadre + "/" + p.AnioMadre,
                FechaParto = p.FechaParto,
                CantidadCrias = p.CantidadCrias
            })
            .ToListAsync(ct);

    public async Task<List<object>> BuscarMadresAsync(string? termino, CancellationToken ct = default)
    {
        var query = db.Animales.AsNoTracking()
            .Include(a => a.TipoAnimal)
            .Where(a => a.Activo && a.Sexo == "HEMBRA");

        if (!string.IsNullOrWhiteSpace(termino))
        {
            termino = termino.Trim();
            if (termino.Contains('/'))
            {
                var p = termino.Split('/');
                if (int.TryParse(p[0], out var n) && int.TryParse(p[1], out var y))
                    query = query.Where(a => a.Numero == n && a.Anio == y);
            }
            else if (int.TryParse(termino, out var num))
                query = query.Where(a => a.Numero == num || a.Anio == num);
        }

        var lista = await query
            .OrderBy(a => a.Anio).ThenBy(a => a.Numero)
            .Take(50)
            .Select(a => new
            {
                key = a.Numero + "|" + a.Anio,
                etiqueta = a.Numero + "/" + a.Anio + " — " + (a.TipoAnimal != null ? a.TipoAnimal.Nombre : "Hembra")
            })
            .ToListAsync(ct);

        return lista.Cast<object>().ToList();
    }

    private async Task<int> SiguienteNumeroAsync(int anio, CancellationToken ct)
    {
        var max = await db.Animales.Where(a => a.Anio == anio).MaxAsync(a => (int?)a.Numero, ct);
        return (max ?? 0) + 1;
    }

    private static (int Numero, int Anio)? ParseKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        var p = key.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
            return (n, a);
        return null;
    }

    private static string Normalizar(string nombre) =>
        nombre.Trim().ToUpperInvariant().Replace("Í", "I");
}
