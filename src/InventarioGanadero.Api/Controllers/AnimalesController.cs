using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class AnimalesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Animales.AsNoTracking()
            .Include(a => a.Propietario).Include(a => a.TipoAnimal).Include(a => a.Lote)
            .OrderByDescending(a => a.Anio).ThenBy(a => a.Numero)
            .ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await SelectListHelper.CargarCatalogosAnimalAsync(db, ViewBag, ct);
        return View(new Animal { Anio = DateTime.Now.Year, FechaIngreso = DateTime.Today });
    }

    [Authorize(Roles = RoleNames.Operacion)]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Animal model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            await SelectListHelper.CargarCatalogosAnimalAsync(db, ViewBag, ct);
            return View(model);
        }
        model.FechaRegistro = DateTime.Now;
        db.Animales.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? numero, int? anio, CancellationToken ct = default)
    {
        if (numero is null || anio is null) return NotFound();
        var item = await db.Animales.FindAsync([numero, anio], ct);
        if (item is null) return NotFound();
        await SelectListHelper.CargarCatalogosAnimalAsync(db, ViewBag, ct);
        return View(item);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int numero, int anio, Animal model, CancellationToken ct = default)
    {
        if (numero != model.Numero || anio != model.Anio) return NotFound();
        if (!ModelState.IsValid)
        {
            await SelectListHelper.CargarCatalogosAnimalAsync(db, ViewBag, ct);
            return View(model);
        }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? numero, int? anio, CancellationToken ct = default)
    {
        if (numero is null || anio is null) return NotFound();
        var item = await db.Animales.AsNoTracking().Include(a => a.Propietario)
            .FirstOrDefaultAsync(a => a.Numero == numero && a.Anio == anio, ct);
        return item is null ? NotFound() : View(item);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int numero, int anio, CancellationToken ct = default)
    {
        var item = await db.Animales.FindAsync([numero, anio], ct);
        if (item is not null) { db.Animales.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }
}
