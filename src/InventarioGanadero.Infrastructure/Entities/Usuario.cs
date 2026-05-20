using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioGanadero.Infrastructure.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(120)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50)]
    [Column("Usuario")]
    [Display(Name = "Usuario")]
    public string UsuarioLogin { get; set; } = string.Empty;

    [StringLength(255)]
    public string? PasswordHash { get; set; }

    /// <summary>Columna legacy Clave; solo para migración desde texto plano.</summary>
    [Column("Clave")]
    public string? LegacyClave { get; set; }

    [Required(ErrorMessage = "Seleccione un rol.")]
    [Display(Name = "Rol")]
    public int IdRol { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Fecha registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Rol? Rol { get; set; }
}
