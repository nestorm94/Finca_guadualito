using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Parto
{
    public int IdParto { get; set; }

    [Display(Name = "Número madre")]
    public int NumeroMadre { get; set; }

    [Display(Name = "Año madre")]
    public int AnioMadre { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha parto")]
    public DateTime FechaParto { get; set; } = DateTime.Today;

    [Range(1, 20, ErrorMessage = "Cantidad de crías entre 1 y 20.")]
    [Display(Name = "Cantidad crías")]
    public int CantidadCrias { get; set; } = 1;

    [StringLength(400)]
    public string? Observacion { get; set; }

    public Animal? Madre { get; set; }
}
