using InventarioGanadero.Api.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioGanadero.Api.Controllers;

[Authorize(Roles = RoleNames.Todos)]
public class PartosController : Controller
{
    public IActionResult Index() => RedirectToAction("Historial", "Reproduccion");

    public IActionResult Create() => RedirectToAction("RegistrarParto", "Reproduccion");

    public IActionResult Edit(int? id) => RedirectToAction("Historial", "Reproduccion");

    public IActionResult Delete(int? id) => RedirectToAction("Historial", "Reproduccion");
}
