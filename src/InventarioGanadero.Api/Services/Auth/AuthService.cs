using System.Security.Claims;
using InventarioGanadero.Api.Repositories;
using InventarioGanadero.Api.Services.Security;
using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Services.Auth;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    RegistroGanaderoDbContext db,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public async Task<Usuario?> ValidateCredentialsAsync(string usuario, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await usuarioRepository.GetByLoginAsync(usuario.Trim(), ct);
        if (user is null || !user.Activo || user.Rol is null)
            return null;

        if (!string.IsNullOrEmpty(user.PasswordHash) && passwordHasher.IsHashed(user.PasswordHash))
        {
            if (!passwordHasher.Verify(password, user.PasswordHash))
                return null;
        }
        else if (!string.IsNullOrEmpty(user.LegacyClave))
        {
            if (!string.Equals(user.LegacyClave, password, StringComparison.Ordinal))
                return null;

            user.PasswordHash = passwordHasher.Hash(password);
            user.LegacyClave = null;
            await usuarioRepository.SaveAsync(user, ct);
        }
        else
        {
            return null;
        }

        return user;
    }

    public ClaimsPrincipal CreatePrincipal(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new("usuario", usuario.UsuarioLogin),
            new(ClaimTypes.Role, usuario.Rol!.NombreRol.ToUpperInvariant())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    public async Task SignInAsync(Usuario usuario, CancellationToken ct = default)
    {
        var principal = CreatePrincipal(usuario);
        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        var ctx = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext no disponible.");

        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
    }

    public async Task SignOutAsync(CancellationToken ct = default)
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is not null)
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<int> MigratePlainPasswordsAsync(CancellationToken ct = default)
    {
        var users = await db.Usuarios
            .Where(u => (u.PasswordHash == null || u.PasswordHash == "") && u.LegacyClave != null && u.LegacyClave != "")
            .ToListAsync(ct);

        foreach (var user in users)
        {
            user.PasswordHash = passwordHasher.Hash(user.LegacyClave!);
            user.LegacyClave = null;
        }

        if (users.Count > 0)
            await db.SaveChangesAsync(ct);

        return users.Count;
    }
}
