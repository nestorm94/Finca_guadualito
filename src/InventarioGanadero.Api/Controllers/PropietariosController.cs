using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class PropietariosController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Propietarios.AsNoTracking().OrderBy(p => p.Nombre).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    public IActionResult Create() => View(new Propietario());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Propietario model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(model);
        model.FechaRegistro = DateTime.Now;
        db.Propietarios.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Propietarios.FindAsync([id], ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Propietario model, CancellationToken ct = default)
    {
        if (id != model.IdPropietario) return NotFound();
        if (!ModelState.IsValid) return View(model);
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Propietarios.AsNoTracking().FirstOrDefaultAsync(p => p.IdPropietario == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Propietarios.FindAsync([id], ct);
        if (item is not null) { db.Propietarios.Remove(item); await db.SaveChangesAsync(ct); }
        return RedirectToAction(nameof(Index));
    }
}
