using System.ComponentModel.DataAnnotations;
using InventarioGanadero.Api.Models.Dtos;

namespace InventarioGanadero.Api.Models.ViewModels;

public class RegistrarPartoViewModel
{
    public string? MadreKey { get; set; }

    [Display(Name = "Madre (vaca)")]
    public string? MadreBusqueda { get; set; }

    [Required(ErrorMessage = "Seleccione la madre.")]
    public int NumeroMadre { get; set; }

    public int AnioMadre { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha parto")]
    public DateTime FechaParto { get; set; } = DateTime.Today;

    [Range(1, 10)]
    [Display(Name = "Cantidad crías")]
    public int CantidadCrias { get; set; } = 1;

    [Display(Name = "Tipo parto")]
    public string TipoParto { get; set; } = "NORMAL";

    [Display(Name = "Estado parto")]
    public string EstadoParto { get; set; } = "REGISTRADO";

    [Display(Name = "Observación")]
    public string? Observacion { get; set; }

    public List<CriaPartoDto> Crias { get; set; } = [new()];
}
