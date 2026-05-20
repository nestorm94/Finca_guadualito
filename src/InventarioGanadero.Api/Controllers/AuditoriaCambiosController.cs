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
public class AuditoriaCambiosController(RegistroGanaderoDbContext db) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.AuditoriaCambios.AsNoTracking().OrderByDescending(a => a.Fecha)
            .ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.AuditoriaCambios.AsNoTracking().FirstOrDefaultAsync(a => a.IdAuditoria == id, ct);
        return item is null ? NotFound() : View(item);
    }
}
