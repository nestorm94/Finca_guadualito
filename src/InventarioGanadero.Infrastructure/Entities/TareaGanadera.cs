using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Infrastructure.Entities;

public class TareaGanadera
{
    public int IdTarea { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Tipo de tarea")]
    public string TipoTarea { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha tarea")]
    public DateTime FechaTarea { get; set; } = DateTime.Today;

    [Display(Name = "Lote")]
    public int? IdLote { get; set; }

    [Display(Name = "Propietario")]
    public int? IdPropietario { get; set; }

    [StringLength(300)]
    public string? Descripcion { get; set; }

    [StringLength(120)]
    public string? Responsable { get; set; }

    [StringLength(400)]
    public string? Observacion { get; set; }

    [StringLength(120)]
    [Display(Name = "Vacuna")]
    public string? NombreVacuna { get; set; }

    [StringLength(150)]
    public string? Medicamento { get; set; }

    [StringLength(80)]
    public string? Dosis { get; set; }

    [StringLength(200)]
    public string? Motivo { get; set; }

    [Display(Name = "Lote destino")]
    public int? IdLoteDestino { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Lote? Lote { get; set; }
    public Propietario? Propietario { get; set; }
    public Lote? LoteDestino { get; set; }
    public ICollection<DetalleTareaGanadera> Detalles { get; set; } = [];
}
