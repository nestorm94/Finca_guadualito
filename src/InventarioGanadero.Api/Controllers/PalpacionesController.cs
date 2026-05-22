using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class PalpacionesController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Palpaciones.AsNoTracking().Include(p => p.Animal)
            .OrderByDescending(p => p.FechaPalpacion).ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await CargarListasAsync(ct);
        return View(new Palpacion { FechaPalpacion = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Create(Palpacion model, string? animalKey, CancellationToken ct = default)
    {
        AplicarAnimalKey(model, animalKey);
        if (!ModelState.IsValid) { await CargarListasAsync(ct, animalKey); return View(model); }
        model.FechaRegistro = DateTime.Now;
        db.Palpaciones.Add(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Palpaciones.FindAsync([id], ct);
        if (item is null) return NotFound();
        await CargarListasAsync(ct, $"{item.Numero}|{item.Anio}");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> Edit(int id, Palpacion model, CancellationToken ct = default)
    {
        if (id != model.IdPalpacion) return NotFound();
        if (!ModelState.IsValid) { await CargarListasAsync(ct, $"{model.Numero}|{model.Anio}"); return View(model); }
        db.Update(model);
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(CancellationToken ct, string? animalKey = null)
    {
        await SelectListHelper.CargarAnimalesAsync(db, ViewBag, animalKey, ct);
        ViewBag.Resultado = new SelectList(GanaderoCatalogos.ResultadosPalpacion);
    }

    private static void AplicarAnimalKey(Palpacion model, string? animalKey)
    {
        if (string.IsNullOrWhiteSpace(animalKey)) return;
        var p = animalKey.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
        { model.Numero = n; model.Anio = a; }
    }
}
