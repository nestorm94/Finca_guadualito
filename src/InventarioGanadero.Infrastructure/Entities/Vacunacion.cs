using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Vacunacion
{
    public int IdVacunacion { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required(ErrorMessage = "La fecha de vacunación es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha vacunación")]
    public DateTime FechaVacunacion { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
    [StringLength(120)]
    [Display(Name = "Vacuna")]
    public string NombreVacuna { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Dosis { get; set; }

    [StringLength(120)]
    public string? Responsable { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
