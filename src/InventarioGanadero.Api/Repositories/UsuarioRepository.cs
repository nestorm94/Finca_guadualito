using InventarioGanadero.Infrastructure.Entities;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Repositories;

public class UsuarioRepository(RegistroGanaderoDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> GetByLoginAsync(string usuarioLogin, CancellationToken ct = default) =>
        db.Usuarios.Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.UsuarioLogin == usuarioLogin, ct);

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.IdUsuario == id, ct);

    public async Task<bool> ExistsLoginAsync(string usuarioLogin, int? excludeId = null, CancellationToken ct = default)
    {
        var query = db.Usuarios.Where(u => u.UsuarioLogin == usuarioLogin);
        if (excludeId.HasValue)
            query = query.Where(u => u.IdUsuario != excludeId.Value);
        return await query.AnyAsync(ct);
    }

    public async Task SaveAsync(Usuario usuario, CancellationToken ct = default)
    {
        if (usuario.IdUsuario == 0)
            db.Usuarios.Add(usuario);
        else
            db.Update(usuario);
        await db.SaveChangesAsync(ct);
    }

    public Task<List<Usuario>> GetPlainPasswordUsersAsync(CancellationToken ct = default) =>
        db.Usuarios.Where(u => u.PasswordHash == null || u.PasswordHash == "").ToListAsync(ct);
}
