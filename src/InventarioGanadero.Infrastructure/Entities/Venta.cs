using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Venta
{
    public int IdVenta { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha venta")]
    public DateTime FechaVenta { get; set; } = DateTime.Today;

    [Range(0, 999999999)]
    [Display(Name = "Valor venta")]
    public decimal? ValorVenta { get; set; }

    [StringLength(120)]
    public string? Comprador { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
