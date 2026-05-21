using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Models;
using InventarioGanadero.Api.Models.Dtos;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Api.Services.Reproduccion;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class ReproduccionController(IPartoService partoService, RegistroGanaderoDbContext db) : Controller
{
    private const int TamanoPagina = 10;

    public async Task<IActionResult> Historial(int pagina = 1, CancellationToken ct = default)
    {
        var total = await partoService.ContarHistorialAsync(ct);
        var items = await partoService.ObtenerHistorialAsync(pagina, TamanoPagina, ct);
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)ListaPaginadaViewModel<PartoHistorialDto>.TamanoPagina));
        if (pagina > totalPaginas) pagina = totalPaginas;

        var vm = new ListaPaginadaViewModel<PartoHistorialDto>
        {
            Items = items,
            Pagina = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = total
        };
        ViewData["ActionName"] = "Historial";
        return View(vm);
    }

    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> RegistrarParto(CancellationToken ct = default)
    {
        await CargarCatalogosAsync(ct);
        return View(new RegistrarPartoViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Operacion)]
    public async Task<IActionResult> RegistrarParto(RegistrarPartoViewModel vm, CancellationToken ct = default)
    {
        AplicarMadreKey(vm);
        NormalizarCrias(vm);

        if (vm.Crias.Count != vm.CantidadCrias)
            ModelState.AddModelError(nameof(vm.CantidadCrias), "La cantidad de crías no coincide con los registros ingresados.");

        if (!ModelState.IsValid)
        {
            await CargarCatalogosAsync(ct, vm.MadreKey);
            return View(vm);
        }

        var dto = new RegistrarPartoDto
        {
            NumeroMadre = vm.NumeroMadre,
            AnioMadre = vm.AnioMadre,
            FechaParto = vm.FechaParto,
            CantidadCrias = vm.CantidadCrias,
            TipoParto = vm.TipoParto,
            EstadoParto = vm.EstadoParto,
            Observacion = vm.Observacion,
            Crias = vm.Crias
        };

        var (ok, error, idParto) = await partoService.RegistrarPartoAsync(dto, ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "No se pudo registrar el parto.");
            await CargarCatalogosAsync(ct, vm.MadreKey);
            return View(vm);
        }

        TempData["Ok"] = $"Parto #{idParto} registrado correctamente.";
        return RedirectToAction(nameof(Historial));
    }

    [HttpGet]
    public async Task<IActionResult> BuscarMadres(string? q, CancellationToken ct = default)
    {
        var result = await partoService.BuscarMadresAsync(q, ct);
        return Json(result);
    }

    private async Task CargarCatalogosAsync(CancellationToken ct, string? madreKey = null)
    {
        ViewBag.TipoParto = new SelectList(PartoCatalogos.TiposParto);
        ViewBag.EstadoParto = new SelectList(PartoCatalogos.EstadosParto);
        ViewBag.IdColor = new SelectList(
            await db.Colores.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync(ct),
            "IdColor", "Nombre");

        var madres = await partoService.BuscarMadresAsync(null, ct);
        var items = madres.Select(m =>
        {
            dynamic d = m;
            return new SelectListItem { Value = (string)d.key, Text = (string)d.etiqueta };
        }).ToList();
        ViewBag.MadreKey = new SelectList(items, "Value", "Text", madreKey);
    }

    private static void AplicarMadreKey(RegistrarPartoViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.MadreKey)) return;
        var p = vm.MadreKey.Split('|');
        if (p.Length == 2 && int.TryParse(p[0], out var n) && int.TryParse(p[1], out var a))
        {
            vm.NumeroMadre = n;
            vm.AnioMadre = a;
        }
    }

    private static void NormalizarCrias(RegistrarPartoViewModel vm)
    {
        vm.Crias ??= [];
        while (vm.Crias.Count < vm.CantidadCrias)
            vm.Crias.Add(new CriaPartoDto { Modo = "Nueva", Sexo = "MACHO" });
        if (vm.Crias.Count > vm.CantidadCrias)
            vm.Crias = vm.Crias.Take(vm.CantidadCrias).ToList();
    }
}
