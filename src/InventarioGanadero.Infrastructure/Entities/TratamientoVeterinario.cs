using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class TratamientoVeterinario
{
    public int IdTratamiento { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha tratamiento")]
    public DateTime FechaTratamiento { get; set; } = DateTime.Today;

    [StringLength(150)]
    public string? Enfermedad { get; set; }

    [StringLength(150)]
    public string? Medicamento { get; set; }

    [StringLength(80)]
    public string? Dosis { get; set; }

    [StringLength(120)]
    public string? Responsable { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
