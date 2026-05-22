namespace InventarioGanadero.Api.Constants;

public static class GanaderoCatalogos
{
    public static readonly string[] TiposTarea =
        ["VACUNACION", "PALPACION", "TRATAMIENTO", "CAMBIO_LOTE"];

    public static readonly string[] ResultadosPalpacion =
        ["PREÑADA", "VACÍA", "DUDOSA", "NO APLICA", "PENDIENTE"];

    public static readonly string[] EstadosDetalleTarea =
        ["REGISTRADO", "PENDIENTE", "COMPLETADO", "OMITIDO"];
}
