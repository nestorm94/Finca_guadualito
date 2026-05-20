using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Pesaje
{
    public int IdPesaje { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha pesaje")]
    public DateTime FechaPesaje { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El peso es obligatorio.")]
    [Range(0.01, 99999, ErrorMessage = "Peso debe ser mayor a 0.")]
    [Display(Name = "Peso (kg)")]
    public decimal PesoKg { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
