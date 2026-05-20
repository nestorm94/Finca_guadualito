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
public class PesajesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Pesajes.AsNoTracking().Include(p => p.Animal)
            .OrderByDescending(p => p.FechaPesaje).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await CargarAnimalesAsync(ct);
        return View(new Pesaje());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Pesaje model, string? animalKey, CancellationToken ct = default)
    {
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarAnimalesAsync(ct, animalKey); return View(model); }
        db.Pesajes.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Pesajes.FindAsync([id], ct);
        if (item is null) return NotFound();
        await CargarAnimalesAsync(ct, $"{item.Numero}|{item.Anio}");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Pesaje model, string? animalKey, CancellationToken ct = default)
    {
        if (id != model.IdPesaje) return NotFound();
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarAnimalesAsync(ct, animalKey); return View(model); }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Pesajes.AsNoTracking().Include(p => p.Animal)
            .FirstOrDefaultAsync(p => p.IdPesaje == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Pesajes.FindAsync([id], ct);
        if (item is not null) { db.Pesajes.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarAnimalesAsync(CancellationToken ct, string? selected = null)
    {
        var animales = await db.Animales.Where(a => a.Activo).OrderBy(a => a.Anio).ThenBy(a => a.Numero)
            .Select(a => new { Key = a.Numero + "|" + a.Anio, Etiqueta = a.Numero + "/" + a.Anio }).ToListAsync(ct);
        ViewBag.AnimalKey = new SelectList(animales, "Key", "Etiqueta", selected);
    }

    private static void AplicarAnimalKey(Pesaje model, string? animalKey)
    {
        var k = ParseAnimalKey(animalKey);
        if (k is not null) { model.Numero = k.Value.Numero; model.Anio = k.Value.Anio; }
    }

    private static (int Numero, int Anio)? ParseAnimalKey(string? animalKey)
    {
        if (string.IsNullOrWhiteSpace(animalKey)) return null;
        var p = animalKey.Split('|');
        return p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a) ? (n, a) : null;
    }
}
