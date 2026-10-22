using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Login_Administracion_de_Modulos.Areas.Modulo05.Models;
using Login_Administracion_de_Modulos.Seguridad;

namespace Login_Administracion_de_Modulos.Areas.Modulo05.Controller
{
    [Area("Modulo05")]
    [Route("Modulo05/[controller]")]
    [ProtegerPanel("Docente", "Profesor", "Jefe de Soporte", "Soporte", "Alumno")]
    public class AveriaDocenteController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _configuration;
        private readonly TicketAveriaRepository _ticketRepo;
        private readonly ActivoHardwareRepository _activoRepo;

        public AveriaDocenteController(IConfiguration configuration)
        {
            _configuration = configuration;
            string conn = ConfiguracionSegura.ObtenerCadenaConexion(_configuration);
            _ticketRepo = new TicketAveriaRepository(conn);
            _activoRepo = new ActivoHardwareRepository(conn);
        }

        [HttpGet("")]
        [HttpGet("Reporte")]
        [HttpGet("Create")]
        public IActionResult FormReporte(bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            ViewData["Title"] = "Reporte Rápido de Averías de Aula";
            ViewData["Role"] = "Docente Académico";

            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/averias_docente/form_reporte.cshtml");
            }

            return View("~/Areas/Modulo05/Views/averias_docente/form_reporte.cshtml");
        }

        [HttpGet("MisReportes")]
        public IActionResult MisReportes(bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!partial && !isAjax)
            {
                return Redirect("/Dashboard/Docente");
            }

            ViewData["Title"] = "Mis Reportes de Averías";
            ViewData["Role"] = "Docente Académico";
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;
            int docenteId = (usuarioId > 0 && usuarioId < 100) ? usuarioId : 1;

            List<TicketAveriaItem> reportes;
            try
            {
                reportes = _ticketRepo.ObtenerReportesPorDocente(docenteId);
            }
            catch
            {
                reportes = new List<TicketAveriaItem>();
            }
            return PartialView("~/Areas/Modulo05/Views/averias_docente/mis_reportes.cshtml", reportes);
        }

        [HttpPost("GuardarReporte")]
        public IActionResult GuardarReporte([FromForm] IFormCollection form)
        {
            try
            {
                string? codigoBien = form["codigoBien"].ToString();
                string? ambiente = form["ambiente"].ToString();
                string? tipoFalla = form["tipoFalla"].ToString();
                string? gravedad = form["gravedad"].ToString();
                string? descripcion = form["descripcion"].ToString();

                if (string.IsNullOrWhiteSpace(tipoFalla))
                {
                    return Json(new { success = false, message = "Debe indicar el tipo de falla detectada." });
                }

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    return Json(new { success = false, message = "Debe ingresar una descripción detallada de la avería." });
                }

                int idBien = 1;
                if (!string.IsNullOrWhiteSpace(codigoBien))
                {
                    var activo = _activoRepo.BuscarPorCbiOSerie(codigoBien);
                    if (activo != null)
                    {
                        idBien = activo.IdBien;
                    }
                }

                string descCompleta = descripcion;
                if (!string.IsNullOrWhiteSpace(ambiente))
                {
                    descCompleta = $"[Ambiente/Aula: {ambiente}] {descCompleta}";
                }

                int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;
                int reportadorId = (usuarioId > 0 && usuarioId < 100) ? usuarioId : 1;

                var ticket = new TicketAveriaItem
                {
                    IdBien = idBien,
                    ReportadorId = reportadorId,
                    FechaReporte = DateTime.Now,
                    TipoFalla = tipoFalla,
                    Gravedad = string.IsNullOrWhiteSpace(gravedad) ? "Media" : gravedad,
                    Descripcion = descCompleta,
                    Estado = "Abierta"
                };

                bool ok = _ticketRepo.CrearReporte(ticket);
                if (ok)
                {
                    return Json(new
                    {
                        success = true,
                        message = "¡Reporte de avería enviado exitosamente a la Jefatura de Soporte!"
                    });
                }

                return Json(new { success = false, message = "No se pudo registrar el reporte de avería." });
            }
            catch
            {
                return Json(new { success = false, message = "Error al registrar el reporte en la base de datos." });
            }
        }

        [HttpGet("BuscarEquipo")]
        public IActionResult BuscarEquipo(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                return Json(new { success = false, message = "Ingrese un código de bien o serie." });
            }

            try
            {
                var activo = _activoRepo.BuscarPorCbiOSerie(criterio);
                if (activo != null)
                {
                    return Json(new
                    {
                        success = true,
                        idBien = activo.IdBien,
                        cbi = activo.Cbi,
                        descripcion = activo.Descripcion,
                        marcaModelo = $"{activo.Marca} {activo.Modelo}".Trim(),
                        serie = activo.Serie,
                        ambiente = activo.AmbienteAsignado
                    });
                }

                return Json(new { success = false, message = "Equipo no encontrado en inventario." });
            }
            catch
            {
                return Json(new { success = false, message = "Error al buscar equipo en la base de datos." });
            }
        }
    }
}
