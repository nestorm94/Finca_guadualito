namespace InventarioGanadero.Api.Models.Dtos;

public class UltimoPartoDto
{
    public int IdParto { get; set; }
    public string Madre { get; set; } = "";
    public DateTime FechaParto { get; set; }
    public int CantidadCrias { get; set; }
}
