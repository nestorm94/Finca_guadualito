using InventarioGanadero.Api.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventarioGanadero.Api.Models.ViewModels;

public class DashboardGanaderoViewModel
{
    public int? IdPropietarioSeleccionado { get; set; }

    public List<SelectListItem> ListaPropietarios { get; set; } = [];

    public string? NombrePropietarioSeleccionado { get; set; }

    public bool EsResumenGeneral { get; set; } = true;

    public int TotalAnimales { get; set; }

    public int TotalHembras { get; set; }

    public int TotalMachos { get; set; }

    public int TotalVacas { get; set; }

    public int TotalToros { get; set; }

    public int TotalNovillas { get; set; }

    public int TotalNovillos { get; set; }

    public int TotalBecerras { get; set; }

    public int TotalBecerros { get; set; }

    public int TotalCrias { get; set; }

    public int TotalPartos { get; set; }

    public int VacasConCrias { get; set; }

    public List<UltimoPartoDto> UltimosPartos { get; set; } = [];

    public List<ReporteEdadAnimalDto> VacasPorEdad { get; set; } = [];

    public List<RangoEdadResumenDto> DistribucionEdadVacas { get; set; } = [];

    public int TotalVacasEnReporte { get; set; }

    public int PaginaEdadesVacas { get; set; } = 1;

    public int TotalPaginasEdadesVacas { get; set; } = 1;

    public int TamanoPaginaEdadesVacas { get; set; } = 10;

    public decimal? EdadPromedioVacasAnios { get; set; }

    public string? ErrorCarga { get; set; }
}
