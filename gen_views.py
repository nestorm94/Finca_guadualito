# -*- coding: utf-8 -*-
from pathlib import Path

V = Path(r"C:\Users\Asus\.cursor\projects\empty-window\InventarioGanadero\src\InventarioGanadero.Api\Views")

def write(rel, text):
    p = V / rel
    p.parent.mkdir(parents=True, exist_ok=True)
    p.write_text(text.strip() + "\n", encoding="utf-8")

D = "div"

def field(prop, typ="text", step=""):
    step_attr = f' step="{step}"' if step else ""
    if typ == "textarea":
        inp = f'<textarea asp-for="{prop}" rows="3"></textarea>'
    elif typ == "checkbox":
        inp = f'<input asp-for="{prop}" type="checkbox" />'
    else:
        inp = f'<input asp-for="{prop}" type="{typ}"{step_attr} />'
    return (
        f'<{D} class="form-group">\n'
        f'  <label asp-for="{prop}"></label>\n'
        f'  {inp}\n'
        f'  <span asp-validation-for="{prop}" class="field-validation-error"></span>\n'
        f'</{D}>'
    )

def select_field(prop, items, optional=False):
    opt = '<option value="">—</option>' if optional else ""
    return (
        f'<{D} class="form-group">\n'
        f'  <label asp-for="{prop}"></label>\n'
        f'  <select asp-for="{prop}" asp-items="{items}">{opt}</select>\n'
        f'  <span asp-validation-for="{prop}" class="field-validation-error"></span>\n'
        f'</{D}>'
    )

ANIMAL = '<partial name="_AnimalKeyField" />'

MADRE = (
    f'<{D} class="form-group">\n'
    f'  <label>Madre</label>\n'
    f'  <select name="animalKey" asp-items="ViewBag.AnimalKey">\n'
    f'    <option value="">-- Seleccione madre --</option>\n'
    f'  </select>\n'
    f'</{D}>'
)

def form(entity, title, body, action="Create", hidden="", btn="Guardar"):
    return f"""@model {entity}
@{{ ViewData["Title"] = "{title}"; }}
<h2 class="page-title">{title}</h2>
<form asp-action="{action}" method="post" class="form-card">
  <{D} asp-validation-summary="ModelOnly" class="text-danger"></{D}>
  {hidden}
  {body}
  <{D} class="form-actions">
    <button type="submit">{btn}</button>
    <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
  </{D}>
</form>
@section Scripts {{ <partial name="_ValidationScriptsPartial" /> }}"""

def idx(name, entity, idp, title, head, row, edit_extra="", delete_extra="", show_id=True):
    id_th = "<th>ID</th>" if show_id else ""
    id_td = f"<td>@item.{idp}</td>" if show_id else ""
    er = edit_extra or f'asp-route-id="@item.{idp}"'
    dr = delete_extra or f'asp-route-id="@item.{idp}"'
    return f"""@model ListaPaginadaViewModel<{entity}>
@{{ ViewData["Title"] = "{title}"; ViewData["ControllerName"] = "{name}"; }}
<h2 class="page-title">{title}</h2>
<p><a asp-action="Create" class="btn">Nuevo</a></p>
<{D} class="table-wrap">
<table>
<thead><tr>{id_th}{head}<th>Acciones</th></tr></thead>
<tbody>
@foreach (var item in Model.Items) {{
<tr>
  {id_td}
  {row}
  <td class="actions">
    <a asp-action="Edit" {er}>Editar</a>
    <a asp-action="Delete" {dr}>Eliminar</a>
  </td>
</tr>
}}
</tbody>
</table>
</{D}>
<partial name="_Paginacion" model="Model" />"""

def delv(entity, idp, title, dl, extra_hidden=""):
    return f"""@model {entity}
@{{ ViewData["Title"] = "Eliminar — {title}"; }}
<h2 class="page-title">¿Eliminar registro?</h2>
<dl class="details-grid">{dl}</dl>
<form asp-action="Delete" method="post" class="form-actions" style="margin-top:1.5rem">
  <input type="hidden" asp-for="{idp}" />
  {extra_hidden}
  <button type="submit" class="btn btn-danger">Sí, eliminar</button>
  <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
</form>"""

def crud(name, entity, idp, title, form_body, head, row, dl, edit_extra="", delete_extra="", del_extra="", show_id=True):
    write(f"{name}/Index.cshtml", idx(name, entity, idp, title, head, row, edit_extra, delete_extra, show_id))
    write(f"{name}/Create.cshtml", form(entity, f"Nuevo — {title}", form_body))
    write(f"{name}/Edit.cshtml", form(entity, f"Editar — {title}", form_body, "Edit", f'<input type="hidden" asp-for="{idp}" />', "Guardar cambios"))
    write(f"{name}/Delete.cshtml", delv(entity, idp, title, dl, del_extra))

# Catalogs
for n, e, i, t in [
    ("Razas", "Raza", "IdRaza", "Razas"),
    ("Colores", "Color", "IdColor", "Colores"),
    ("TiposAnimales", "TipoAnimal", "IdTipoAnimal", "Tipos de animal"),
    ("EstadosAnimal", "EstadoAnimal", "IdEstadoAnimal", "Estados de animal"),
]:
    crud(n, e, i, t, field("Nombre"), "<th>Nombre</th>", "<td>@item.Nombre</td>", f"<{D}><dt>Nombre</dt><dd>@Model.Nombre</dd></{D}>")

crud("Roles", "Rol", "IdRol", "Roles", field("NombreRol"), "<th>Nombre del rol</th>", "<td>@item.NombreRol</td>",
     f"<{D}><dt>Nombre del rol</dt><dd>@Model.NombreRol</dd></{D}>")

crud("Lotes", "Lote", "IdLote", "Lotes",
     field("Nombre") + field("Descripcion", "textarea") + field("Activo", "checkbox"),
     "<th>Nombre</th><th>Descripción</th><th>Activo</th>",
     '<td>@item.Nombre</td><td>@(item.Descripcion ?? "—")</td><td>@(item.Activo ? "Sí" : "No")</td>',
     f"<{D}><dt>Nombre</dt><dd>@Model.Nombre</dd></{D}><{D}><dt>Descripción</dt><dd>@(Model.Descripcion ?? "—")</dd></{D}>")

# Usuarios
write("Usuarios/Index.cshtml", idx("Usuarios", "Usuario", "IdUsuario", "Usuarios",
    "<th>Nombre completo</th><th>Usuario</th><th>Rol</th><th>Activo</th>",
    '<td>@item.NombreCompleto</td><td>@item.NombreUsuario</td><td>@(item.Rol?.NombreRol ?? "—")</td><td>@(item.Activo ? "Sí" : "No")</td>'))
uf = (field("NombreCompleto") + field("NombreUsuario") + field("Clave", "password") +
      select_field("IdRol", "ViewBag.IdRol") + field("Activo", "checkbox"))
write("Usuarios/Create.cshtml", form("Usuario", "Nuevo — Usuarios", uf))
write("Usuarios/Edit.cshtml", form("Usuario", "Editar — Usuarios",
    uf + f'<p class="text-muted">Deje la clave vacía para mantener la actual.</p>', "Edit",
    '<input type="hidden" asp-for="IdUsuario" />', "Guardar cambios"))
write("Usuarios/Delete.cshtml", delv("Usuario", "IdUsuario", "Usuarios",
    f"<{D}><dt>Nombre completo</dt><dd>@Model.NombreCompleto</dd></{D}><{D}><dt>Usuario</dt><dd>@Model.NombreUsuario</dd></{D}><{D}><dt>Rol</dt><dd>@(Model.Rol?.NombreRol ?? "—")</dd></{D}>"))

# Auditoria (read-only)
write("AuditoriaCambios/Index.cshtml", f"""@model ListaPaginadaViewModel<AuditoriaCambio>
@{{ ViewData["Title"] = "Auditoría de cambios"; ViewData["ControllerName"] = "AuditoriaCambios"; }}
<h2 class="page-title">Auditoría de cambios</h2>
<{D} class="table-wrap">
<table>
<thead><tr><th>ID</th><th>Tabla</th><th>Acción</th><th>Usuario</th><th>Fecha</th><th>Acciones</th></tr></thead>
<tbody>
@foreach (var item in Model.Items) {{
<tr>
  <td>@item.IdAuditoria</td>
  <td>@item.Tabla</td>
  <td>@item.Accion</td>
  <td>@(item.Usuario ?? "—")</td>
  <td>@item.Fecha.ToString("dd/MM/yyyy HH:mm")</td>
  <td class="actions"><a asp-action="Details" asp-route-id="@item.IdAuditoria">Ver</a></td>
</tr>
}}
</tbody>
</table>
</{D}>
<partial name="_Paginacion" model="Model" />""")
write("AuditoriaCambios/Details.cshtml", f"""@model AuditoriaCambio
@{{ ViewData["Title"] = "Detalle auditoría"; }}
<h2 class="page-title">Detalle de auditoría</h2>
<dl class="details-grid">
  <{D}><dt>Tabla</dt><dd>@Model.Tabla</dd></{D}>
  <{D}><dt>Acción</dt><dd>@Model.Accion</dd></{D}>
  <{D}><dt>Descripción</dt><dd>@(Model.Descripcion ?? "—")</dd></{D}>
  <{D}><dt>Usuario</dt><dd>@(Model.Usuario ?? "—")</dd></{D}>
  <{D}><dt>Fecha</dt><dd>@Model.Fecha.ToString("dd/MM/yyyy HH:mm")</dd></{D}>
</dl>
<a asp-action="Index" class="btn btn-secondary">Volver</a>""")

# Pesajes
crud("Pesajes", "Pesaje", "IdPesaje", "Pesajes",
     ANIMAL + field("FechaPesaje", "date") + field("PesoKg", "number", "0.01") + field("Observacion", "textarea"),
     "<th>Animal</th><th>Fecha pesaje</th><th>Peso (kg)</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaPesaje.ToString("dd/MM/yyyy")</td><td>@item.PesoKg</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}><{D}><dt>Fecha pesaje</dt><dd>@Model.FechaPesaje.ToString("dd/MM/yyyy")</dd></{D}>")

# MovimientosAnimal
mov_f = (ANIMAL + field("FechaMovimiento", "date") +
         select_field("TipoMovimiento", "ViewBag.TipoMovimiento") +
         select_field("IdLoteOrigen", "ViewBag.IdLoteOrigen", True) +
         select_field("IdLoteDestino", "ViewBag.IdLoteDestino", True) +
         field("Observacion", "textarea"))
crud("MovimientosAnimal", "MovimientoAnimal", "IdMovimiento", "Movimientos de animal", mov_f,
     "<th>Animal</th><th>Fecha</th><th>Tipo</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaMovimiento.ToString("dd/MM/yyyy")</td><td>@item.TipoMovimiento</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# TratamientosVeterinarios
trat_f = ANIMAL + field("FechaTratamiento", "date") + field("Enfermedad") + field("Medicamento") + field("Dosis") + field("Responsable") + field("Observacion", "textarea")
crud("TratamientosVeterinarios", "TratamientoVeterinario", "IdTratamiento", "Tratamientos veterinarios", trat_f,
     "<th>Animal</th><th>Fecha</th><th>Enfermedad</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaTratamiento.ToString("dd/MM/yyyy")</td><td>@(item.Enfermedad ?? "—")</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# Compras
comp_f = ANIMAL + field("FechaCompra", "date") + field("ValorCompra", "number", "0.01") + field("Vendedor") + field("Observacion", "textarea")
crud("Compras", "Compra", "IdCompra", "Compras", comp_f,
     "<th>Animal</th><th>Fecha</th><th>Valor</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaCompra.ToString("dd/MM/yyyy")</td><td>@(item.ValorCompra?.ToString("N2") ?? "—")</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# Ventas
vent_f = ANIMAL + field("FechaVenta", "date") + field("ValorVenta", "number", "0.01") + field("Comprador") + field("Observacion", "textarea")
crud("Ventas", "Venta", "IdVenta", "Ventas", vent_f,
     "<th>Animal</th><th>Fecha</th><th>Valor</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaVenta.ToString("dd/MM/yyyy")</td><td>@(item.ValorVenta?.ToString("N2") ?? "—")</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# Muertes
muer_f = ANIMAL + field("FechaMuerte", "date") + field("Causa") + field("Observacion", "textarea")
crud("Muertes", "Muerte", "IdMuerte", "Muertes", muer_f,
     "<th>Animal</th><th>Fecha</th><th>Causa</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaMuerte.ToString("dd/MM/yyyy")</td><td>@(item.Causa ?? "—")</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# Partos
part_f = MADRE + field("FechaParto", "date") + field("CantidadCrias", "number") + field("Observacion", "textarea")
crud("Partos", "Parto", "IdParto", "Partos", part_f,
     "<th>Madre</th><th>Fecha</th><th>Crías</th>",
     '<td>@item.NumeroMadre/@item.AnioMadre</td><td>@item.FechaParto.ToString("dd/MM/yyyy")</td><td>@item.CantidadCrias</td>',
     f"<{D}><dt>Madre</dt><dd>@Model.NumeroMadre/@Model.AnioMadre</dd></{D}>")

# Vacunaciones
vac_f = ANIMAL + field("FechaVacunacion", "date") + field("NombreVacuna") + field("Dosis") + field("Responsable") + field("Observacion", "textarea")
crud("Vacunaciones", "Vacunacion", "IdVacunacion", "Vacunaciones", vac_f,
     "<th>Animal</th><th>Fecha</th><th>Vacuna</th>",
     '<td>@item.Numero/@item.Anio</td><td>@item.FechaVacunacion.ToString("dd/MM/yyyy")</td><td>@item.NombreVacuna</td>',
     f"<{D}><dt>Animal</dt><dd>@Model.Numero/@Model.Anio</dd></{D}>")

# Animales
write("Animales/Index.cshtml", idx("Animales", "Animal", "Numero", "Animales",
    "<th>Número</th><th>Año</th><th>Propietario</th><th>Sexo</th><th>Activo</th>",
    '<td>@item.Numero</td><td>@item.Anio</td><td>@(item.Propietario?.Nombre ?? "—")</td><td>@item.Sexo</td><td>@(item.Activo ? "Sí" : "No")</td>',
    'asp-route-numero="@item.Numero" asp-route-anio="@item.Anio"',
    'asp-route-numero="@item.Numero" asp-route-anio="@item.Anio"',
    show_id=False))
write("Animales/Create.cshtml", form("Animal", "Nuevo — Animales", '<partial name="_AnimalFormFields" />'))
anim_edit_body = (
    f'<{D} class="form-group"><label>Número / Año</label><p>@Model.Numero / @Model.Anio</p></{D}>'
    '<partial name="_AnimalFormFieldsEdit" />'
)
write("Animales/Edit.cshtml", form("Animal", "Editar — Animales", anim_edit_body, "Edit",
    '<input type="hidden" asp-for="Numero" /><input type="hidden" asp-for="Anio" />', "Guardar cambios"))
write("Animales/Delete.cshtml", f"""@model Animal
@{{ ViewData["Title"] = "Eliminar — Animales"; }}
<h2 class="page-title">¿Eliminar animal?</h2>
<dl class="details-grid">
  <{D}><dt>Número</dt><dd>@Model.Numero</dd></{D}>
  <{D}><dt>Año</dt><dd>@Model.Anio</dd></{D}>
  <{D}><dt>Propietario</dt><dd>@(Model.Propietario?.Nombre ?? "—")</dd></{D}>
  <{D}><dt>Sexo</dt><dd>@Model.Sexo</dd></{D}>
</dl>
<form asp-action="Delete" method="post" class="form-actions" style="margin-top:1.5rem">
  <input type="hidden" asp-for="Numero" />
  <input type="hidden" asp-for="Anio" />
  <button type="submit" class="btn btn-danger">Sí, eliminar</button>
  <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
</form>""")

created = sorted(p.relative_to(V).as_posix() for p in V.rglob("*.cshtml"))
print(f"Done: {len(created)} views")
for f in created:
    print(f)
