namespace InventarioGanadero.Infrastructure.Entities;

public class DetalleTareaGanadera
{
    public int IdDetalleTarea { get; set; }
    public int IdTarea { get; set; }
    public int Numero { get; set; }
    public int Anio { get; set; }

    public string EstadoRegistro { get; set; } = "REGISTRADO";

    public int? IdRegistroOrigen { get; set; }
    public string? TipoRegistroOrigen { get; set; }
    public string? Observacion { get; set; }

    public TareaGanadera? Tarea { get; set; }
    public Animal? Animal { get; set; }
}
