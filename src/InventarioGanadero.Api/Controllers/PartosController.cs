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
public class PartosController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Partos.AsNoTracking().Include(p => p.Madre)
            .OrderByDescending(p => p.FechaParto).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await CargarMadresAsync(ct);
        return View(new Parto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Parto model, string? animalKey, CancellationToken ct = default)
    {
        AplicarMadreKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarMadresAsync(ct, animalKey); return View(model); }
        db.Partos.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Partos.FindAsync([id], ct);
        if (item is null) return NotFound();
        await CargarMadresAsync(ct, $"{item.NumeroMadre}|{item.AnioMadre}");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Parto model, string? animalKey, CancellationToken ct = default)
    {
        if (id != model.IdParto) return NotFound();
        AplicarMadreKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarMadresAsync(ct, animalKey); return View(model); }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Partos.AsNoTracking().Include(p => p.Madre)
            .FirstOrDefaultAsync(p => p.IdParto == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Partos.FindAsync([id], ct);
        if (item is not null) { db.Partos.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarMadresAsync(CancellationToken ct, string? selected = null)
    {
        var animales = await db.Animales.Where(a => a.Activo && a.Sexo == "HEMBRA")
            .OrderBy(a => a.Anio).ThenBy(a => a.Numero)
            .Select(a => new { Key = a.Numero + "|" + a.Anio, Etiqueta = a.Numero + "/" + a.Anio }).ToListAsync(ct);
        ViewBag.AnimalKey = new SelectList(animales, "Key", "Etiqueta", selected);
    }

    private static void AplicarMadreKey(Parto model, string? animalKey)
    {
        if (string.IsNullOrWhiteSpace(animalKey)) return;
        var p = animalKey.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
        { model.NumeroMadre = n; model.AnioMadre = a; }
    }
}
