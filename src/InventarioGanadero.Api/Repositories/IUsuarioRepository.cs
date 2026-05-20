using InventarioGanadero.Infrastructure.Entities;

namespace InventarioGanadero.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByLoginAsync(string usuarioLogin, CancellationToken ct = default);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsLoginAsync(string usuarioLogin, int? excludeId = null, CancellationToken ct = default);
    Task SaveAsync(Usuario usuario, CancellationToken ct = default);
    Task<List<Usuario>> GetPlainPasswordUsersAsync(CancellationToken ct = default);
}
