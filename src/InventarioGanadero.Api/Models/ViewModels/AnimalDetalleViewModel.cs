using InventarioGanadero.Infrastructure.Entities;

namespace InventarioGanadero.Api.Models.ViewModels;

public class AnimalDetalleViewModel
{
    public Animal Animal { get; set; } = null!;
    public string EdadTexto { get; set; } = "";
    public List<Vacunacion> Vacunaciones { get; set; } = [];
    public List<Palpacion> Palpaciones { get; set; } = [];
    public List<TratamientoVeterinario> Tratamientos { get; set; } = [];
    public List<MovimientoAnimal> Movimientos { get; set; } = [];
    public List<CriaResumenViewModel> Crias { get; set; } = [];
    public MadreResumenViewModel? Madre { get; set; }
    public bool MostrarCrias { get; set; }
    public bool MostrarMadre { get; set; }
}

public class CriaResumenViewModel
{
    public int Numero { get; set; }
    public int Anio { get; set; }
    public string Identificador => $"{Numero}/{Anio}";
    public string? Sexo { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public int? BimestreNacimiento { get; set; }
}

public class MadreResumenViewModel
{
    public int Numero { get; set; }
    public int Anio { get; set; }
    public string Identificador => $"{Numero}/{Anio}";
    public string? TipoAnimal { get; set; }
}
