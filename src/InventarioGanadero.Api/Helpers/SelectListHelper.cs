using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Helpers;

public static class SelectListHelper
{
    public static async Task CargarRolesAsync(RegistroGanaderoDbContext db, dynamic viewBag, CancellationToken ct = default)
    {
        var roles = await db.Roles.AsNoTracking().OrderBy(r => r.NombreRol).ToListAsync(ct);
        viewBag.IdRol = new SelectList(roles, "IdRol", "NombreRol");
    }

    public static async Task CargarCatalogosAnimalAsync(RegistroGanaderoDbContext db, dynamic viewBag, CancellationToken ct = default)
    {
        viewBag.IdPropietario = new SelectList(
            await db.Propietarios.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync(ct),
            "IdPropietario", "Nombre");

        viewBag.IdTipoAnimal = new SelectList(
            await db.TiposAnimales.OrderBy(t => t.Nombre).ToListAsync(ct),
            "IdTipoAnimal", "Nombre");

        viewBag.IdEstadoAnimal = new SelectList(
            await db.EstadosAnimal.OrderBy(e => e.Nombre).ToListAsync(ct),
            "IdEstadoAnimal", "Nombre");

        viewBag.IdLote = new SelectList(
            await db.Lotes.Where(l => l.Activo).OrderBy(l => l.Nombre).ToListAsync(ct),
            "IdLote", "Nombre");

        viewBag.IdRaza = new SelectList(
            await db.Razas.OrderBy(r => r.Nombre).ToListAsync(ct),
            "IdRaza", "Nombre");

        viewBag.IdColor = new SelectList(
            await db.Colores.OrderBy(c => c.Nombre).ToListAsync(ct),
            "IdColor", "Nombre");

        viewBag.Sexo = new SelectList(new[] { "MACHO", "HEMBRA" });
    }

    public static async Task CargarAnimalesAsync(RegistroGanaderoDbContext db, dynamic viewBag, string? selectedKey = null, CancellationToken ct = default)
    {
        var animales = await db.Animales.Where(a => a.Activo)
            .OrderBy(a => a.Anio).ThenBy(a => a.Numero)
            .Select(a => new { Key = a.Numero + "|" + a.Anio, Etiqueta = a.Numero + " / " + a.Anio })
            .ToListAsync(ct);

        viewBag.AnimalKey = new SelectList(animales, "Key", "Etiqueta", selectedKey);
    }
}
