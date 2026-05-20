using InventarioGanadero.Infrastructure.Entities;
using System.Security.Claims;

namespace InventarioGanadero.Api.Services.Auth;

public interface IAuthService
{
    Task<Usuario?> ValidateCredentialsAsync(string usuario, string password, CancellationToken ct = default);
    ClaimsPrincipal CreatePrincipal(Usuario usuario);
    Task SignInAsync(Usuario usuario, CancellationToken ct = default);
    Task SignOutAsync(CancellationToken ct = default);
    Task<int> MigratePlainPasswordsAsync(CancellationToken ct = default);
}
