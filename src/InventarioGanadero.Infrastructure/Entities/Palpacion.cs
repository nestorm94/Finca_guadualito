using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Palpacion
{
    public int IdPalpacion { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha palpación")]
    public DateTime FechaPalpacion { get; set; } = DateTime.Today;

    [Required]
    [StringLength(30)]
    [Display(Name = "Resultado")]
    public string Resultado { get; set; } = "PENDIENTE";

    [Display(Name = "Meses gestación")]
    public int? MesesGestacion { get; set; }

    [StringLength(150)]
    [Display(Name = "Medicamento aplicado")]
    public string? MedicamentoAplicado { get; set; }

    [StringLength(80)]
    public string? Dosis { get; set; }

    [StringLength(120)]
    public string? Responsable { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Animal? Animal { get; set; }
}
