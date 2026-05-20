using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using InventarioGanadero.Api.Models;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Administrador)]
public class RolesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Roles.AsNoTracking().OrderBy(r => r.NombreRol).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    public IActionResult Create() => View(new Rol());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rol model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(model);
        db.Roles.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Roles.FindAsync([id], ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rol model, CancellationToken ct = default)
    {
        if (id != model.IdRol) return NotFound();
        if (!ModelState.IsValid) return View(model);
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.IdRol == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Roles.FindAsync([id], ct);
        if (item is not null) { db.Roles.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }
}
