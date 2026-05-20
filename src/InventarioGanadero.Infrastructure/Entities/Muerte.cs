using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Muerte
{
    public int IdMuerte { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha muerte")]
    public DateTime FechaMuerte { get; set; } = DateTime.Today;

    [StringLength(200)]
    public string? Causa { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
