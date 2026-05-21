namespace InventarioGanadero.Api.Models.Dtos;

public class ReporteEdadAnimalDto
{
    public int Numero { get; set; }
    public int Anio { get; set; }
    public string Identificador => $"{Numero}/{Anio}";
    public string TipoAnimal { get; set; } = "";
    public string? Propietario { get; set; }
    public int? EdadMeses { get; set; }
    public decimal? EdadAnios { get; set; }
    public string EdadTexto { get; set; } = "";
    public string RangoEdad { get; set; } = "";
    public DateTime? FechaNacimiento { get; set; }
    public int? BimestreNacimiento { get; set; }
    public bool EdadEstimada { get; set; }
}

public class RangoEdadResumenDto
{
    public string Rango { get; set; } = "";
    public int Cantidad { get; set; }
}
