using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class TipoAnimal
{
    public int IdTipoAnimal { get; set; }

    [Required(ErrorMessage = "El tipo de animal es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;
}
