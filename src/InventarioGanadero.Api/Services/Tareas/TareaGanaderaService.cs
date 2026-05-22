using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Services.Tareas;

public class TareaGanaderaService(RegistroGanaderoDbContext db) : ITareaGanaderaService
{
    public async Task<(bool Ok, string? Error, TareaMasivaResultadoViewModel? Resultado)> AplicarTareaMasivaAsync(
        RegistrarTareaMasivaViewModel model, CancellationToken ct = default)
    {
        if (!GanaderoCatalogos.TiposTarea.Contains(model.TipoTarea))
            return (false, "Tipo de tarea no válido.", null);

        var idEstadoActivo = await db.EstadosAnimal.AsNoTracking()
            .Where(e => e.Nombre.ToUpper() == "ACTIVO")
            .Select(e => e.IdEstadoAnimal)
            .FirstOrDefaultAsync(ct);

        if (idEstadoActivo == 0)
            return (false, "No existe el estado ACTIVO.", null);

        var query = db.Animales.Where(a => a.Activo && a.IdEstadoAnimal == idEstadoActivo && a.IdLote == model.IdLote);
        if (model.IdPropietario is > 0)
            query = query.Where(a => a.IdPropietario == model.IdPropietario);

        var animales = await query.OrderBy(a => a.Numero).ToListAsync(ct);
        if (animales.Count == 0)
            return (false, "No hay animales activos en el lote seleccionado.", null);

        if (model.TipoTarea == "VACUNACION" && string.IsNullOrWhiteSpace(model.NombreVacuna))
            return (false, "Indique el nombre de la vacuna.", null);

        if (model.TipoTarea == "TRATAMIENTO" && string.IsNullOrWhiteSpace(model.Medicamento))
            return (false, "Indique el medicamento.", null);

        if (model.TipoTarea == "CAMBIO_LOTE" && model.IdLoteDestino is null or <= 0)
            return (false, "Seleccione el lote destino.", null);

        var lote = await db.Lotes.AsNoTracking().FirstOrDefaultAsync(l => l.IdLote == model.IdLote, ct);

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var tarea = new TareaGanadera
            {
                TipoTarea = model.TipoTarea,
                FechaTarea = model.FechaTarea.Date,
                IdLote = model.IdLote,
                IdPropietario = model.IdPropietario,
                Descripcion = model.Descripcion ?? GenerarDescripcion(model, lote?.Nombre),
                Responsable = model.Responsable,
                Observacion = model.Observacion,
                NombreVacuna = model.NombreVacuna,
                Medicamento = model.Medicamento,
                Dosis = model.Dosis ?? model.DosisVacuna,
                Motivo = model.Motivo,
                IdLoteDestino = model.IdLoteDestino,
                FechaRegistro = DateTime.Now
            };

            db.TareasGanaderas.Add(tarea);
            await db.SaveChangesAsync(ct);

            var creados = 0;
            var omitidos = 0;

            foreach (var animal in animales)
            {
                var detalle = new DetalleTareaGanadera
                {
                    IdTarea = tarea.IdTarea,
                    Numero = animal.Numero,
                    Anio = animal.Anio,
                    EstadoRegistro = "REGISTRADO"
                };

                switch (model.TipoTarea)
                {
                    case "PALPACION":
                        var pal = new Palpacion
                        {
                            Numero = animal.Numero,
                            Anio = animal.Anio,
                            FechaPalpacion = model.FechaTarea.Date,
                            Resultado = model.ResultadoPalpacion,
                            MedicamentoAplicado = model.Medicamento,
                            Dosis = model.Dosis,
                            Responsable = model.Responsable,
                            Observacion = model.Observacion,
                            FechaRegistro = DateTime.Now
                        };
                        db.Palpaciones.Add(pal);
                        await db.SaveChangesAsync(ct);
                        detalle.IdRegistroOrigen = pal.IdPalpacion;
                        detalle.TipoRegistroOrigen = "PALPACION";
                        creados++;
                        break;

                    case "VACUNACION":
                        var existeVac = await db.Vacunaciones.AnyAsync(v =>
                            v.Numero == animal.Numero && v.Anio == animal.Anio &&
                            v.FechaVacunacion == model.FechaTarea.Date &&
                            v.NombreVacuna == model.NombreVacuna, ct);
                        if (existeVac)
                        {
                            omitidos++;
                            detalle.EstadoRegistro = "OMITIDO";
                            detalle.Observacion = "Vacunación duplicada (misma fecha y vacuna).";
                        }
                        else
                        {
                            var vac = new Vacunacion
                            {
                                Numero = animal.Numero,
                                Anio = animal.Anio,
                                FechaVacunacion = model.FechaTarea.Date,
                                NombreVacuna = model.NombreVacuna!,
                                Dosis = model.DosisVacuna ?? model.Dosis,
                                Responsable = model.Responsable,
                                Observacion = model.Observacion
                            };
                            db.Vacunaciones.Add(vac);
                            await db.SaveChangesAsync(ct);
                            detalle.IdRegistroOrigen = vac.IdVacunacion;
                            detalle.TipoRegistroOrigen = "VACUNACION";
                            creados++;
                        }
                        break;

                    case "TRATAMIENTO":
                        var trat = new TratamientoVeterinario
                        {
                            Numero = animal.Numero,
                            Anio = animal.Anio,
                            FechaTratamiento = model.FechaTarea.Date,
                            Medicamento = model.Medicamento,
                            Dosis = model.Dosis,
                            Enfermedad = model.Motivo,
                            Responsable = model.Responsable,
                            Observacion = model.Observacion
                        };
                        db.TratamientosVeterinarios.Add(trat);
                        await db.SaveChangesAsync(ct);
                        detalle.IdRegistroOrigen = trat.IdTratamiento;
                        detalle.TipoRegistroOrigen = "TRATAMIENTO";
                        creados++;
                        break;

                    case "CAMBIO_LOTE":
                        var loteOrigen = animal.IdLote;
                        animal.IdLote = model.IdLoteDestino;
                        db.Update(animal);
                        var mov = new MovimientoAnimal
                        {
                            Numero = animal.Numero,
                            Anio = animal.Anio,
                            FechaMovimiento = model.FechaTarea.Date,
                            TipoMovimiento = "CAMBIO_LOTE",
                            IdLoteOrigen = loteOrigen,
                            IdLoteDestino = model.IdLoteDestino,
                            Observacion = model.Observacion ?? $"Tarea masiva #{tarea.IdTarea}",
                            FechaRegistro = DateTime.Now
                        };
                        db.MovimientosAnimal.Add(mov);
                        await db.SaveChangesAsync(ct);
                        detalle.IdRegistroOrigen = mov.IdMovimiento;
                        detalle.TipoRegistroOrigen = "MOVIMIENTO";
                        creados++;
                        break;
                }

                db.DetalleTareasGanaderas.Add(detalle);
            }

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            var resultado = await ObtenerResultadoTareaAsync(tarea.IdTarea, ct);
            if (resultado is not null)
            {
                resultado.RegistrosCreados = creados;
                resultado.DuplicadosOmitidos = omitidos;
            }

            return (true, null, resultado);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return (false, ex.Message, null);
        }
    }

    public async Task<TareaMasivaResultadoViewModel?> ObtenerResultadoTareaAsync(int idTarea, CancellationToken ct = default)
    {
        var tarea = await db.TareasGanaderas.AsNoTracking()
            .Include(t => t.Lote)
            .Include(t => t.Detalles)
            .FirstOrDefaultAsync(t => t.IdTarea == idTarea, ct);

        if (tarea is null) return null;

        return new TareaMasivaResultadoViewModel
        {
            IdTarea = tarea.IdTarea,
            TipoTarea = tarea.TipoTarea,
            FechaTarea = tarea.FechaTarea,
            LoteNombre = tarea.Lote?.Nombre,
            TotalAnimales = tarea.Detalles.Count,
            RegistrosCreados = tarea.Detalles.Count(d => d.EstadoRegistro == "REGISTRADO"),
            DuplicadosOmitidos = tarea.Detalles.Count(d => d.EstadoRegistro == "OMITIDO"),
            Detalles = tarea.Detalles.OrderBy(d => d.Numero).Select(d => new DetalleTareaItemViewModel
            {
                IdDetalleTarea = d.IdDetalleTarea,
                Numero = d.Numero,
                Anio = d.Anio,
                Animal = $"{d.Numero}/{d.Anio}",
                EstadoRegistro = d.EstadoRegistro,
                IdRegistroOrigen = d.IdRegistroOrigen,
                TipoRegistroOrigen = d.TipoRegistroOrigen
            }).ToList()
        };
    }

    private static string GenerarDescripcion(RegistrarTareaMasivaViewModel m, string? loteNombre) =>
        $"{m.TipoTarea} — Lote {loteNombre ?? m.IdLote.ToString()} — {m.FechaTarea:dd/MM/yyyy}";
}
