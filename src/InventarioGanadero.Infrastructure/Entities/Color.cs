using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Color
{
    public int IdColor { get; set; }

    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;
}
