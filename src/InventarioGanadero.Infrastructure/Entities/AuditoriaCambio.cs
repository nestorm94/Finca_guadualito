using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class AuditoriaCambio
{
    public int IdAuditoria { get; set; }

    [Required]
    [StringLength(100)]
    public string Tabla { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Accion { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [StringLength(100)]
    public string? Usuario { get; set; }

    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;
}
