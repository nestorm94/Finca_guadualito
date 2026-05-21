namespace InventarioGanadero.Api.Models.Dtos;

public class PartoHistorialDto
{
    public int IdParto { get; set; }
    public string Madre { get; set; } = "";
    public DateTime FechaParto { get; set; }
    public int CantidadCrias { get; set; }
    public string TipoParto { get; set; } = "";
    public string EstadoParto { get; set; } = "";
    public List<string> CriasAsociadas { get; set; } = [];
}
