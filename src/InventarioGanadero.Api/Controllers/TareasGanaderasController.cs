using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Api.Services.Tareas;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class TareasGanaderasController(
    RegistroGanaderoDbContext db,
    ITareaGanaderaService tareaService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var lista = await db.TareasGanaderas.AsNoTracking()
            .Include(t => t.Lote)
            .Include(t => t.Detalles)
            .OrderByDescending(t => t.FechaTarea)
            .ThenByDescending(t => t.IdTarea)
            .Take(100)
            .ToListAsync(ct);

        return View(lista);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Crear(CancellationToken ct = default)
    {
        await CargarCatalogosAsync(ct);
        return View(new RegistrarTareaMasivaViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Crear(RegistrarTareaMasivaViewModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            await CargarCatalogosAsync(ct);
            return View(model);
        }

        var (ok, error, resultado) = await tareaService.AplicarTareaMasivaAsync(model, ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "No se pudo aplicar la tarea.");
            await CargarCatalogosAsync(ct);
            return View(model);
        }

        return RedirectToAction(nameof(Detalle), new { id = resultado!.IdTarea });
    }

    public async Task<IActionResult> Detalle(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var vm = await tareaService.ObtenerResultadoTareaAsync(id.Value, ct);
        return vm is null ? NotFound() : View(vm);
    }

    private async Task CargarCatalogosAsync(CancellationToken ct)
    {
        ViewBag.TipoTarea = new SelectList(GanaderoCatalogos.TiposTarea);
        ViewBag.ResultadoPalpacion = new SelectList(GanaderoCatalogos.ResultadosPalpacion);
        ViewBag.IdLote = new SelectList(
            await db.Lotes.Where(l => l.Activo).OrderBy(l => l.Nombre).ToListAsync(ct),
            "IdLote", "Nombre");
        ViewBag.IdLoteDestino = new SelectList(
            await db.Lotes.Where(l => l.Activo).OrderBy(l => l.Nombre).ToListAsync(ct),
            "IdLote", "Nombre");
        ViewBag.IdPropietario = new SelectList(
            await db.Propietarios.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync(ct),
            "IdPropietario", "Nombre");
    }
}
