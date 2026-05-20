using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class VacunacionesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Vacunaciones.AsNoTracking().Include(v => v.Animal)
            .OrderByDescending(v => v.FechaVacunacion).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await SelectListHelper.CargarAnimalesAsync(db, ViewBag, null, ct);
        return View(new Vacunacion { FechaVacunacion = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Vacunacion model, string? animalKey, CancellationToken ct = default)
    {
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid)
        {
            await SelectListHelper.CargarAnimalesAsync(db, ViewBag, animalKey, ct);
            return View(model);
        }
        db.Vacunaciones.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Vacunaciones.FindAsync([id], ct);
        if (item is null) return NotFound();
        await SelectListHelper.CargarAnimalesAsync(db, ViewBag, $"{item.Numero}|{item.Anio}", ct);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Vacunacion model, string? animalKey, CancellationToken ct = default)
    {
        if (id != model.IdVacunacion) return NotFound();
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid)
        {
            await SelectListHelper.CargarAnimalesAsync(db, ViewBag, animalKey, ct);
            return View(model);
        }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Vacunaciones.AsNoTracking().Include(v => v.Animal)
            .FirstOrDefaultAsync(v => v.IdVacunacion == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Vacunaciones.FindAsync([id], ct);
        if (item is not null) { db.Vacunaciones.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }

    private static void AplicarAnimalKey(Vacunacion model, string? animalKey)
    {
        if (string.IsNullOrWhiteSpace(animalKey)) return;
        var p = animalKey.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
        { model.Numero = n; model.Anio = a; }
    }
}
