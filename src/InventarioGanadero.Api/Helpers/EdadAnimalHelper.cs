namespace InventarioGanadero.Api.Helpers;

public static class EdadAnimalHelper
{
    public static int? CalcularEdadMeses(int? edadMeses, DateTime? fechaNacimiento, int anioRegistro)
    {
        if (edadMeses is > 0)
            return edadMeses;

        if (fechaNacimiento.HasValue)
        {
            var hoy = DateTime.Today;
            var meses = (hoy.Year - fechaNacimiento.Value.Year) * 12
                + (hoy.Month - fechaNacimiento.Value.Month);
            if (hoy.Day < fechaNacimiento.Value.Day)
                meses--;
            return Math.Max(0, meses);
        }

        var aniosEstimados = DateTime.Today.Year - anioRegistro;
        return aniosEstimados > 0 ? aniosEstimados * 12 : null;
    }

    public static decimal? CalcularEdadAnios(int? edadMeses) =>
        edadMeses.HasValue ? Math.Round(edadMeses.Value / 12m, 1) : null;

    public static string FormatearEdad(int? edadMeses)
    {
        if (!edadMeses.HasValue)
            return "Sin dato";

        var años = edadMeses.Value / 12;
        var meses = edadMeses.Value % 12;

        if (años > 0 && meses > 0)
            return $"{años} año{(años > 1 ? "s" : "")} {meses} mes{(meses > 1 ? "es" : "")}";

        if (años > 0)
            return $"{años} año{(años > 1 ? "s" : "")}";

        return $"{meses} mes{(meses > 1 ? "es" : "")}";
    }

    public static string ObtenerRangoEdad(int? edadMeses) => edadMeses switch
    {
        null => "Sin dato",
        < 36 => "0 – 3 años",
        < 72 => "4 – 6 años",
        < 108 => "7 – 9 años",
        < 144 => "10 – 12 años",
        _ => "13+ años"
    };
}
