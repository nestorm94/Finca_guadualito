namespace InventarioGanadero.Api.Services.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public bool IsHashed(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.StartsWith("$2", StringComparison.Ordinal);
}
