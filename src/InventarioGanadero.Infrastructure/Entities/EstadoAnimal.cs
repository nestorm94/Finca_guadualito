using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class EstadoAnimal
{
    public int IdEstadoAnimal { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;
}
