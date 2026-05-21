namespace InventarioGanadero.Infrastructure.Entities;

public class RelacionMadreCria
{
    public int IdRelacion { get; set; }
    public int IdParto { get; set; }
    public int NumeroMadre { get; set; }
    public int AnioMadre { get; set; }
    public int NumeroCria { get; set; }
    public int AnioCria { get; set; }

    public Parto? Parto { get; set; }
    public Animal? Cria { get; set; }
}
