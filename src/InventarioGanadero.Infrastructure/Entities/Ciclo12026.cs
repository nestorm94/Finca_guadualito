using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioGanadero.Infrastructure.Entities;

[Table("ciclo_1_2026", Schema = "dbo")]
public class Ciclo12026
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NUMERO")]
    public short? Numero { get; set; }

    [Column("AÑO")]
    public short? Ano { get; set; }

    [Column("PROPIETARIO")]
    public string? Propietario { get; set; }

    [Column("COLOR")]
    public string? Color { get; set; }

    [Column("OBSERVACION")]
    public string? Observacion { get; set; }

    [Column("FECHA_VACUNA")]
    public DateTime? FechaVacuna { get; set; }

    [Column("SEXO")]
    public string? Sexo { get; set; }

    [Column("LOTE")]
    public string? Lote { get; set; }
}
