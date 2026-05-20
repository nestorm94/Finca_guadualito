using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class MovimientoAnimal
{
    public int IdMovimiento { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha movimiento")]
    public DateTime FechaMovimiento { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Tipo movimiento")]
    public string TipoMovimiento { get; set; } = string.Empty;

    [Display(Name = "Lote origen")]
    public int? IdLoteOrigen { get; set; }

    [Display(Name = "Lote destino")]
    public int? IdLoteDestino { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Animal? Animal { get; set; }
    public Lote? LoteOrigen { get; set; }
    public Lote? LoteDestino { get; set; }
}
