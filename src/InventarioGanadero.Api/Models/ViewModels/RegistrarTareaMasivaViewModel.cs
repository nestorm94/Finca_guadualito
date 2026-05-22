using System.ComponentModel.DataAnnotations;

namespace InventarioGanadero.Api.Models.ViewModels;

public class RegistrarTareaMasivaViewModel
{
    [Required]
    [Display(Name = "Tipo de tarea")]
    public string TipoTarea { get; set; } = "PALPACION";

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime FechaTarea { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Seleccione un lote.")]
    [Display(Name = "Lote")]
    public int IdLote { get; set; }

    [Display(Name = "Propietario (opcional)")]
    public int? IdPropietario { get; set; }

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Responsable")]
    public string? Responsable { get; set; }

    [Display(Name = "Observación general")]
    public string? Observacion { get; set; }

    [Display(Name = "Vacuna")]
    public string? NombreVacuna { get; set; }

    [Display(Name = "Dosis vacuna")]
    public string? DosisVacuna { get; set; }

    [Display(Name = "Medicamento")]
    public string? Medicamento { get; set; }

    [Display(Name = "Dosis")]
    public string? Dosis { get; set; }

    [Display(Name = "Motivo / Enfermedad")]
    public string? Motivo { get; set; }

    [Display(Name = "Resultado palpación (inicial)")]
    public string ResultadoPalpacion { get; set; } = "PENDIENTE";

    [Display(Name = "Lote destino")]
    public int? IdLoteDestino { get; set; }
}

public class TareaMasivaResultadoViewModel
{
    public int IdTarea { get; set; }
    public string TipoTarea { get; set; } = "";
    public DateTime FechaTarea { get; set; }
    public string? LoteNombre { get; set; }
    public int TotalAnimales { get; set; }
    public int RegistrosCreados { get; set; }
    public int DuplicadosOmitidos { get; set; }
    public List<DetalleTareaItemViewModel> Detalles { get; set; } = [];
}

public class DetalleTareaItemViewModel
{
    public int IdDetalleTarea { get; set; }
    public string Animal { get; set; } = "";
    public int Numero { get; set; }
    public int Anio { get; set; }
    public string EstadoRegistro { get; set; } = "";
    public int? IdRegistroOrigen { get; set; }
    public string? TipoRegistroOrigen { get; set; }
}
