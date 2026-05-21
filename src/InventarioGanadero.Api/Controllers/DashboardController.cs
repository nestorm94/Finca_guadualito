using InventarioGanadero.Api.Constants;
using InventarioGanadero.Api.Helpers;
using InventarioGanadero.Api.Models.Dtos;
using InventarioGanadero.Api.Models.ViewModels;
using InventarioGanadero.Api.Services.Reproduccion;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class DashboardController(RegistroGanaderoDbContext db, IPartoService partoService) : Controller
{
    private const int TamanoPaginaEdadesVacas = 10;

    public async Task<IActionResult> Index(int? idPropietario, int paginaEdades = 1, CancellationToken ct = default)
    {
        if (paginaEdades < 1) paginaEdades = 1;

        var vm = new DashboardGanaderoViewModel
        {
            IdPropietarioSeleccionado = idPropietario,
            PaginaEdadesVacas = paginaEdades,
            TamanoPaginaEdadesVacas = TamanoPaginaEdadesVacas
        };

        try
        {
            var propietarios = await db.Propietarios.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new { p.IdPropietario, p.Nombre })
                .ToListAsync(ct);

            vm.ListaPropietarios =
            [
                new SelectListItem
                {
                    Value = "",
                    Text = "Todos los propietarios (resumen general)",
                    Selected = idPropietario is null or <= 0
                },
                .. propietarios.Select(p => new SelectListItem
                {
                    Value = p.IdPropietario.ToString(),
                    Text = p.Nombre,
                    Selected = idPropietario == p.IdPropietario
                })
            ];

            if (idPropietario is > 0)
            {
                var prop = propietarios.FirstOrDefault(p => p.IdPropietario == idPropietario);
                if (prop is null)
                    vm.IdPropietarioSeleccionado = null;
                else
                {
                    vm.EsResumenGeneral = false;
                    vm.NombrePropietarioSeleccionado = prop.Nombre;
                }
            }

            var idEstadoActivo = await db.EstadosAnimal.AsNoTracking()
                .Where(e => e.Nombre.ToUpper() == "ACTIVO")
                .Select(e => e.IdEstadoAnimal)
                .FirstOrDefaultAsync(ct);

            if (idEstadoActivo == 0)
                return View(vm);

            var idTipoVaca = await db.TiposAnimales.AsNoTracking()
                .Where(t => t.Nombre.ToUpper() == "VACA")
                .Select(t => t.IdTipoAnimal)
                .FirstOrDefaultAsync(ct);

            IQueryable<Infrastructure.Entities.Animal> query = db.Animales.AsNoTracking()
                .Where(a => a.Activo && a.IdEstadoAnimal == idEstadoActivo);

            if (vm.IdPropietarioSeleccionado is > 0)
                query = query.Where(a => a.IdPropietario == vm.IdPropietarioSeleccionado);

            var animales = await query
                .Select(a => new { a.Sexo, TipoNombre = a.TipoAnimal!.Nombre })
                .ToListAsync(ct);

            vm.TotalAnimales = animales.Count;
            vm.TotalHembras = animales.Count(a => a.Sexo == "HEMBRA");
            vm.TotalMachos = animales.Count(a => a.Sexo == "MACHO");
            vm.TotalVacas = animales.Count(a => EsTipo(a.TipoNombre, "VACA"));
            vm.TotalToros = animales.Count(a => EsTipo(a.TipoNombre, "TORO"));
            vm.TotalNovillas = animales.Count(a => EsTipo(a.TipoNombre, "NOVILLA"));
            vm.TotalNovillos = animales.Count(a => EsTipo(a.TipoNombre, "NOVILLO"));
            vm.TotalBecerras = animales.Count(a => EsTipo(a.TipoNombre, "BECERRA"));
            vm.TotalBecerros = animales.Count(a => EsTipo(a.TipoNombre, "BECERRO"));
            vm.TotalCrias = animales.Count(a => EsTipoCria(a.TipoNombre));

            try
            {
                vm.TotalPartos = await partoService.ContarPartosAsync(ct);
                vm.VacasConCrias = await partoService.ContarVacasConCriasAsync(ct);
                vm.UltimosPartos = await partoService.ObtenerUltimosPartosAsync(5, ct);
            }
            catch (Exception ex)
            {
                vm.ErrorCarga = "Reproducción: " + ex.Message;
            }

            if (idTipoVaca > 0)
            {
                var vacas = await query
                    .Where(a => a.IdTipoAnimal == idTipoVaca)
                    .Select(a => new DatoEdadAnimal
                    {
                        Numero = a.Numero,
                        Anio = a.Anio,
                        EdadMeses = a.EdadMeses,
                        FechaNacimiento = a.FechaNacimiento,
                        BimestreNacimiento = a.BimestreNacimiento,
                        TipoAnimal = a.TipoAnimal!.Nombre,
                        Propietario = a.Propietario!.Nombre
                    })
                    .ToListAsync(ct);

                CargarReporteEdadesVacas(vm, vacas, paginaEdades);
            }
        }
        catch (Exception ex)
        {
            vm.ErrorCarga = ex.Message;
        }

        return View(vm);
    }

    private static void CargarReporteEdadesVacas(DashboardGanaderoViewModel vm, List<DatoEdadAnimal> vacas, int pagina)
    {
        var reporteCompleto = vacas.Select(v =>
        {
            var tieneDato = v.EdadMeses is > 0 || v.FechaNacimiento.HasValue;
            var meses = EdadAnimalHelper.CalcularEdadMeses(v.EdadMeses, v.FechaNacimiento, v.Anio);
            return new ReporteEdadAnimalDto
            {
                Numero = v.Numero,
                Anio = v.Anio,
                TipoAnimal = v.TipoAnimal,
                Propietario = v.Propietario,
                EdadMeses = meses,
                EdadAnios = EdadAnimalHelper.CalcularEdadAnios(meses),
                EdadTexto = EdadAnimalHelper.FormatearEdad(meses),
                RangoEdad = EdadAnimalHelper.ObtenerRangoEdad(meses),
                FechaNacimiento = v.FechaNacimiento,
                BimestreNacimiento = v.BimestreNacimiento,
                EdadEstimada = !tieneDato && meses.HasValue
            };
        })
        .OrderByDescending(v => v.EdadMeses ?? -1)
        .ThenBy(v => v.Numero)
        .ToList();

        vm.TotalVacasEnReporte = reporteCompleto.Count;
        vm.TotalPaginasEdadesVacas = Math.Max(1, (int)Math.Ceiling(reporteCompleto.Count / (double)TamanoPaginaEdadesVacas));
        if (pagina > vm.TotalPaginasEdadesVacas) pagina = vm.TotalPaginasEdadesVacas;
        vm.PaginaEdadesVacas = pagina;

        vm.VacasPorEdad = reporteCompleto
            .Skip((pagina - 1) * TamanoPaginaEdadesVacas)
            .Take(TamanoPaginaEdadesVacas)
            .ToList();

        var conEdad = reporteCompleto.Where(v => v.EdadMeses.HasValue).ToList();
        if (conEdad.Count > 0)
            vm.EdadPromedioVacasAnios = Math.Round((decimal)conEdad.Average(v => v.EdadMeses!.Value) / 12m, 1);

        vm.DistribucionEdadVacas = reporteCompleto
            .GroupBy(v => v.RangoEdad)
            .Select(g => new RangoEdadResumenDto { Rango = g.Key, Cantidad = g.Count() })
            .OrderBy(r => OrdenRango(r.Rango))
            .ToList();
    }

    private static int OrdenRango(string rango) => rango switch
    {
        "0 – 3 años" => 1,
        "4 – 6 años" => 2,
        "7 – 9 años" => 3,
        "10 – 12 años" => 4,
        "13+ años" => 5,
        _ => 99
    };

    private sealed class DatoEdadAnimal
    {
        public int Numero { get; set; }
        public int Anio { get; set; }
        public int? EdadMeses { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? BimestreNacimiento { get; set; }
        public string TipoAnimal { get; set; } = "";
        public string? Propietario { get; set; }
    }

    private static bool EsTipo(string? nombre, string tipoEsperado) =>
        string.Equals(NormalizarTipo(nombre), tipoEsperado, StringComparison.OrdinalIgnoreCase);

    private static bool EsTipoCria(string? nombre) =>
        NormalizarTipo(nombre) == "CRIA";

    private static string? NormalizarTipo(string? nombre) =>
        nombre?.Trim().ToUpperInvariant().Replace("Í", "I");
}
