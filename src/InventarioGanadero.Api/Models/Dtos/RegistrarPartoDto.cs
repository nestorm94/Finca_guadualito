namespace InventarioGanadero.Api.Models.Dtos;

public class RegistrarPartoDto
{
    public int NumeroMadre { get; set; }
    public int AnioMadre { get; set; }
    public DateTime FechaParto { get; set; } = DateTime.Today;
    public int CantidadCrias { get; set; } = 1;
    public string TipoParto { get; set; } = "NORMAL";
    public string EstadoParto { get; set; } = "REGISTRADO";
    public string? Observacion { get; set; }
    public List<CriaPartoDto> Crias { get; set; } = [];
}
