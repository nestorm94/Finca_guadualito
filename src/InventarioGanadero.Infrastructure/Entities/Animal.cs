using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioGanadero.Infrastructure.Entities;

public class Animal
{
    [Display(Name = "Número")]
    public int Numero { get; set; }

    [Display(Name = "Año")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "Seleccione un propietario.")]
    [Display(Name = "Propietario")]
    public int IdPropietario { get; set; }

    [Required(ErrorMessage = "Seleccione el tipo de animal.")]
    [Display(Name = "Tipo")]
    public int IdTipoAnimal { get; set; }

    [Required(ErrorMessage = "Seleccione el estado.")]
    [Display(Name = "Estado")]
    public int IdEstadoAnimal { get; set; }

    [Display(Name = "Lote")]
    public int? IdLote { get; set; }

    [Display(Name = "Raza")]
    public int? IdRaza { get; set; }

    [Display(Name = "Color")]
    public int? IdColor { get; set; }

    [Required(ErrorMessage = "El sexo es obligatorio.")]
    [RegularExpression("^(MACHO|HEMBRA)$", ErrorMessage = "Sexo debe ser MACHO o HEMBRA.")]
    [StringLength(10)]
    public string Sexo { get; set; } = "MACHO";

    [Range(0, 600, ErrorMessage = "Edad en meses no válida.")]
    [Display(Name = "Edad (meses)")]
    public int? EdadMeses { get; set; }

    [Display(Name = "Fecha nacimiento")]
    [DataType(DataType.Date)]
    public DateTime? FechaNacimiento { get; set; }

    [Display(Name = "Fecha ingreso")]
    [DataType(DataType.Date)]
    public DateTime? FechaIngreso { get; set; }

    [Range(0, 99999, ErrorMessage = "Peso no válido.")]
    [Display(Name = "Peso actual (kg)")]
    public decimal? PesoActualKg { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Propietario? Propietario { get; set; }
    public TipoAnimal? TipoAnimal { get; set; }
    public EstadoAnimal? EstadoAnimal { get; set; }
    public Lote? Lote { get; set; }
    public Raza? Raza { get; set; }
    public Color? Color { get; set; }
}
