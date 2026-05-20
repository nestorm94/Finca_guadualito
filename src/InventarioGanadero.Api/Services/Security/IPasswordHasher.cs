namespace InventarioGanadero.Api.Services.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
    bool IsHashed(string? value);
}
