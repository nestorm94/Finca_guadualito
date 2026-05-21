namespace InventarioGanadero.Api.Helpers;

/// <summary>
/// Bimestres: 1=Ene-Feb, 2=Mar-Abr, 3=May-Jun, 4=Jul-Ago, 5=Sep-Oct, 6=Nov-Dic.
/// </summary>
public static class BimestreNacimientoHelper
{
    public static int Calcular(DateTime fecha) => fecha.Month switch
    {
        1 or 2 => 1,
        3 or 4 => 2,
        5 or 6 => 3,
        7 or 8 => 4,
        9 or 10 => 5,
        11 or 12 => 6,
        _ => 1
    };

    public static string Descripcion(int bimestre) => bimestre switch
    {
        1 => "Enero - Febrero",
        2 => "Marzo - Abril",
        3 => "Mayo - Junio",
        4 => "Julio - Agosto",
        5 => "Septiembre - Octubre",
        6 => "Noviembre - Diciembre",
        _ => "—"
    };
}
