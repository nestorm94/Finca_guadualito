using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Compra
{
    public int IdCompra { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha compra")]
    public DateTime FechaCompra { get; set; } = DateTime.Today;

    [Range(0, 999999999)]
    [Display(Name = "Valor compra")]
    public decimal? ValorCompra { get; set; }

    [StringLength(120)]
    public string? Vendedor { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }

    public Animal? Animal { get; set; }
}
