using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Services.Historial;

public class AnimalHistorialService(RegistroGanaderoDbContext db) : IAnimalHistorialService
{
    public async Task<AnimalDetalleViewModel?> ObtenerDetalleAsync(int numero, int anio, CancellationToken ct = default)
    {
        var animal = await db.Animales.AsNoTracking()
            .Include(a => a.Propietario)
            .Include(a => a.TipoAnimal)
            .Include(a => a.EstadoAnimal)
            .Include(a => a.Lote)
            .Include(a => a.Color)
            .Include(a => a.Raza)
            .FirstOrDefaultAsync(a => a.Numero == numero && a.Anio == anio, ct);

        if (animal is null) return null;

        var tipo = animal.TipoAnimal?.Nombre?.Trim().ToUpperInvariant().Replace("Í", "I") ?? "";
        var esVaca = tipo == "VACA";
        var esCria = tipo is "CRIA" or "CRÍA";

        var vm = new AnimalDetalleViewModel
        {
            Animal = animal,
            EdadTexto = EdadAnimalHelper.FormatearEdad(
                EdadAnimalHelper.CalcularEdadMeses(animal.EdadMeses, animal.FechaNacimiento, animal.Anio)),
            Vacunaciones = await db.Vacunaciones.AsNoTracking()
                .Where(v => v.Numero == numero && v.Anio == anio)
                .OrderByDescending(v => v.FechaVacunacion).ToListAsync(ct),
            Palpaciones = await db.Palpaciones.AsNoTracking()
                .Where(p => p.Numero == numero && p.Anio == anio)
                .OrderByDescending(p => p.FechaPalpacion).ToListAsync(ct),
            Tratamientos = await db.TratamientosVeterinarios.AsNoTracking()
                .Where(t => t.Numero == numero && t.Anio == anio)
                .OrderByDescending(t => t.FechaTratamiento).ToListAsync(ct),
            Movimientos = await db.MovimientosAnimal.AsNoTracking()
                .Include(m => m.LoteOrigen).Include(m => m.LoteDestino)
                .Where(m => m.Numero == numero && m.Anio == anio)
                .OrderByDescending(m => m.FechaMovimiento).ToListAsync(ct),
            MostrarCrias = esVaca,
            MostrarMadre = esCria
        };

        if (esVaca)
        {
            vm.Crias = await (
                from r in db.RelacionesMadreCria.AsNoTracking()
                join c in db.Animales.AsNoTracking() on new { r.NumeroCria, r.AnioCria } equals new { NumeroCria = c.Numero, AnioCria = c.Anio }
                where r.NumeroMadre == numero && r.AnioMadre == anio
                select new CriaResumenViewModel
                {
                    Numero = c.Numero,
                    Anio = c.Anio,
                    Sexo = c.Sexo,
                    FechaNacimiento = c.FechaNacimiento,
                    BimestreNacimiento = c.BimestreNacimiento
                }).ToListAsync(ct);
        }

        if (esCria)
        {
            var rel = await db.RelacionesMadreCria.AsNoTracking()
                .FirstOrDefaultAsync(r => r.NumeroCria == numero && r.AnioCria == anio, ct);

            if (rel is not null)
            {
                var madre = await db.Animales.AsNoTracking()
                    .Include(a => a.TipoAnimal)
                    .FirstOrDefaultAsync(a => a.Numero == rel.NumeroMadre && a.Anio == rel.AnioMadre, ct);

                if (madre is not null)
                {
                    vm.Madre = new MadreResumenViewModel
                    {
                        Numero = madre.Numero,
                        Anio = madre.Anio,
                        TipoAnimal = madre.TipoAnimal?.Nombre
                    };
                    vm.MostrarMadre = true;
                }
            }
        }

        return vm;
    }
}
