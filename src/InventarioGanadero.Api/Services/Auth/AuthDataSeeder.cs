using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Services.Security;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Services.Auth;

public static class AuthDataSeeder
{
    public static async Task SeedAsync(RegistroGanaderoDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
    {
        foreach (var nombre in new[] { RoleNames.Administrador, RoleNames.Operador, RoleNames.Consulta })
        {
            if (!await db.Roles.AnyAsync(r => r.NombreRol == nombre, ct))
                db.Roles.Add(new Rol { NombreRol = nombre });
        }
        await db.SaveChangesAsync(ct);

        if (await db.Usuarios.AnyAsync(ct))
            return;

        var adminRol = await db.Roles.FirstAsync(r => r.NombreRol == RoleNames.Administrador, ct);
        db.Usuarios.Add(new Usuario
        {
            NombreCompleto = "Administrador del sistema",
            UsuarioLogin = "admin",
            PasswordHash = hasher.Hash("Admin123!"),
            IdRol = adminRol.IdRol,
            Activo = true,
            FechaRegistro = DateTime.Now
        });
        await db.SaveChangesAsync(ct);
    }
}
