using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Models;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Api.Repositories;
using InventarioGanadero.Api.Services.Security;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Administrador)]
public class UsuariosController(
    RegistroGanaderoDbContext db,
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher) : Controller
{
    public async Task<IActionResult> Index(int pagina = 1, CancellationToken ct = default)
    {
        var vm = await db.Usuarios.AsNoTracking().Include(u => u.Rol).OrderBy(u => u.NombreCompleto)
            .ToListaPaginadaAsync(pagina, ct);
        return View(vm);
    }

    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await SelectListHelper.CargarRolesAsync(db, ViewBag, ct);
        return View(new UsuarioFormViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioFormViewModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Password))
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError(nameof(model.Password), "La contraseña es obligatoria.");
            await SelectListHelper.CargarRolesAsync(db, ViewBag, ct);
            return View(model);
        }

        if (await usuarioRepository.ExistsLoginAsync(model.UsuarioLogin, ct: ct))
        {
            ModelState.AddModelError(nameof(model.UsuarioLogin), "Ese usuario ya existe.");
            await SelectListHelper.CargarRolesAsync(db, ViewBag, ct);
            return View(model);
        }

        var entity = MapToEntity(model);
        entity.PasswordHash = passwordHasher.Hash(model.Password!);
        entity.LegacyClave = null;
        entity.FechaRegistro = DateTime.Now;
        await usuarioRepository.SaveAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await usuarioRepository.GetByIdAsync(id.Value, ct);
        if (item is null) return NotFound();
        await SelectListHelper.CargarRolesAsync(db, ViewBag, ct);
        return View(MapToForm(item));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioFormViewModel model, CancellationToken ct = default)
    {
        if (id != model.IdUsuario) return NotFound();

        if (await usuarioRepository.ExistsLoginAsync(model.UsuarioLogin, id, ct))
            ModelState.AddModelError(nameof(model.UsuarioLogin), "Ese usuario ya existe.");

        if (!ModelState.IsValid)
        {
            await SelectListHelper.CargarRolesAsync(db, ViewBag, ct);
            return View(model);
        }

        var entity = await usuarioRepository.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();

        entity.NombreCompleto = model.NombreCompleto;
        entity.UsuarioLogin = model.UsuarioLogin;
        entity.IdRol = model.IdRol;
        entity.Activo = model.Activo;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            entity.PasswordHash = passwordHasher.Hash(model.Password);
            entity.LegacyClave = null;
        }

        await usuarioRepository.SaveAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id is null) return NotFound();
        var item = await db.Usuarios.AsNoTracking().Include(u => u.Rol).FirstOrDefaultAsync(u => u.IdUsuario == id, ct);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var item = await db.Usuarios.FindAsync([id], ct);
        if (item is not null)
        {
            db.Usuarios.Remove(item);
            await db.SaveChangesAsync(ct);
        }
        return RedirectToAction(nameof(Index));
    }

    private static Usuario MapToEntity(UsuarioFormViewModel model) => new()
    {
        IdUsuario = model.IdUsuario,
        NombreCompleto = model.NombreCompleto,
        UsuarioLogin = model.UsuarioLogin,
        IdRol = model.IdRol,
        Activo = model.Activo
    };

    private static UsuarioFormViewModel MapToForm(Usuario u) => new()
    {
        IdUsuario = u.IdUsuario,
        NombreCompleto = u.NombreCompleto,
        UsuarioLogin = u.UsuarioLogin,
        IdRol = u.IdRol,
        Activo = u.Activo
    };
}
