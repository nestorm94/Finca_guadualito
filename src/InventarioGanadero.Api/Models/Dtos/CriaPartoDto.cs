namespace InventarioGanadero.Api.Models.Dtos;

public class CriaPartoDto
{
    /// <summary>Existente | Nueva</summary>
    public string Modo { get; set; } = "Nueva";

    public string? AnimalKeyExistente { get; set; }

    public int? NumeroNuevo { get; set; }

    public string Sexo { get; set; } = "MACHO";

    public int? IdColor { get; set; }
}
