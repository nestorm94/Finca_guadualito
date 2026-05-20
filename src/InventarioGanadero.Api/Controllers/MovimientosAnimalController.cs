using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using InventarioGanadero.Api.Models;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class MovimientosAnimalController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.MovimientosAnimal.AsNoTracking().Include(m => m.Animal)
            .OrderByDescending(m => m.FechaMovimiento).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await CargarListasAsync(ct);
        return View(new MovimientoAnimal());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(MovimientoAnimal model, string? animalKey, CancellationToken ct = default)
    {
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarListasAsync(ct, animalKey); return View(model); }
        model.FechaRegistro = DateTime.Now;
        db.MovimientosAnimal.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.MovimientosAnimal.FindAsync([id], ct);
        if (item is null) return NotFound();
        await CargarListasAsync(ct, $"{item.Numero}|{item.Anio}");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, MovimientoAnimal model, string? animalKey, CancellationToken ct = default)
    {
        if (id != model.IdMovimiento) return NotFound();
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarListasAsync(ct, animalKey); return View(model); }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.MovimientosAnimal.AsNoTracking().Include(m => m.Animal)
            .FirstOrDefaultAsync(m => m.IdMovimiento == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.MovimientosAnimal.FindAsync([id], ct);
        if (item is not null) { db.MovimientosAnimal.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(CancellationToken ct, string? animalKey = null)
    {
        var animales = await db.Animales.Where(a => a.Activo).OrderBy(a => a.Anio).ThenBy(a => a.Numero)
            .Select(a => new { Key = a.Numero + "|" + a.Anio, Etiqueta = a.Numero + "/" + a.Anio }).ToListAsync(ct);
        ViewBag.AnimalKey = new SelectList(animales, "Key", "Etiqueta", animalKey);
        var lotes = await db.Lotes.Where(l => l.Activo).OrderBy(l => l.Nombre).ToListAsync(ct);
        ViewBag.IdLoteOrigen = new SelectList(lotes, "IdLote", "Nombre");
        ViewBag.IdLoteDestino = new SelectList(lotes, "IdLote", "Nombre");
        ViewBag.TipoMovimiento = new SelectList(new[] { "TRASLADO", "INGRESO", "SALIDA", "CAMBIO_LOTE" });
    }

    private static void AplicarAnimalKey(MovimientoAnimal model, string? animalKey)
    {
        if (string.IsNullOrWhiteSpace(animalKey)) return;
        var p = animalKey.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
        { model.Numero = n; model.Anio = a; }
    }
}
