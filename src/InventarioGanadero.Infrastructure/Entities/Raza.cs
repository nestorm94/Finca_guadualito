using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class Raza
{
    public int IdRaza { get; set; }

    [Required(ErrorMessage = "El nombre de la raza es obligatorio.")]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;
}
