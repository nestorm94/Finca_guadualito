using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using InventarioGanadero.Api.Models;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class LotesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Lotes.AsNoTracking().OrderBy(l => l.Nombre).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    public IActionResult Create() => View(new Lote());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Lote model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(model);
        db.Lotes.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Lotes.FindAsync([id], ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Lote model, CancellationToken ct = default)
    {
        if (id != model.IdLote) return NotFound();
        if (!ModelState.IsValid) return View(model);
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Lotes.AsNoTracking().FirstOrDefaultAsync(l => l.IdLote == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Lotes.FindAsync([id], ct);
        if (item is not null) { db.Lotes.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }
}
