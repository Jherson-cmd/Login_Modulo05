using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Login_Administracion_de_Modulos.Areas.Modulo05.Models;
using Login_Administracion_de_Modulos.Seguridad;

namespace Login_Administracion_de_Modulos.Areas.Modulo05.Controller
{
    /// <summary>
    /// Controlador del Módulo 05 para la Gestión de Requerimientos Institucionales y de TI (tabla public.requerimientos).
    /// Recicla la arquitectura visual de Modulo05 para integrarse en los paneles laterales dinámicos según rol.
    /// </summary>
    [Area("Modulo05")]
    [Route("Modulo05/[controller]")]
    [ProtegerPanel("Jefe de Soporte", "Soporte", "Docente", "Alumno", "Practicante", "Secretaría Académica", "Tesorería", "Jefe General", "Administrador")]
    public class RequerimientosController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RequerimientoRepository _repo;

        public RequerimientosController(IConfiguration configuration)
        {
            string conn = ConfiguracionSegura.ObtenerCadenaConexion(configuration);
            _repo = new RequerimientoRepository(conn);
        }

        [HttpGet("")]
        [HttpGet("Create")]
        [HttpGet("Formulario")]
        public IActionResult Create(bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            ViewData["Title"] = "Solicitud de Requerimiento y Soporte TI";
            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/requerimientos/form_requerimiento.cshtml");
            }
            return View("~/Areas/Modulo05/Views/requerimientos/form_requerimiento.cshtml");
        }

        [HttpGet("Bandeja")]
        public IActionResult Bandeja(string? estado = null, bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            ViewData["Title"] = "Bandeja de Requerimientos Institucionales";
            List<RequerimientoItem> lista;
            try
            {
                lista = _repo.ListarRequerimientos(estado);
            }
            catch
            {
                lista = new List<RequerimientoItem>();
            }
            ViewBag.FiltroEstado = estado;
            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/requerimientos/bandeja_requerimientos.cshtml", lista);
            }
            return View("~/Areas/Modulo05/Views/requerimientos/bandeja_requerimientos.cshtml", lista);
        }

        [HttpPost("Guardar")]
        public IActionResult Guardar([FromForm] IFormCollection form)
        {
            try
            {
                string tipoServicio = form["tipoServicio"].ToString();
                string prioridad = form["prioridad"].ToString();
                string descripcion = form["descripcion"].ToString();
                int.TryParse(form["idAmbiente"].ToString(), out int idAmb);

                if (string.IsNullOrWhiteSpace(tipoServicio) || string.IsNullOrWhiteSpace(descripcion))
                {
                    return Json(new { success = false, message = "Complete el tipo de servicio y la descripción detallada del requerimiento." });
                }

                int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;

                var req = new RequerimientoItem
                {
                    SolicitanteId = usuarioId,
                    IdAmbiente = idAmb > 0 ? idAmb : 1,
                    TipoServicio = tipoServicio,
                    Prioridad = prioridad,
                    Descripcion = descripcion,
                    Estado = "Pendiente"
                };

                bool ok = _repo.CrearRequerimiento(req);
                return Json(new
                {
                    success = ok,
                    message = ok ? "¡Requerimiento registrado exitosamente en PostgreSQL!" : "No se pudo registrar el requerimiento."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar en PostgreSQL: " + ex.Message });
            }
        }

        [HttpPost("ActualizarEstado")]
        public IActionResult ActualizarEstado([FromForm] int idRequerimiento, [FromForm] string estado, [FromForm] string? observacion)
        {
            try
            {
                int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;
                bool ok = _repo.ActualizarEstado(idRequerimiento, estado, observacion, usuarioId);
                return Json(new
                {
                    success = ok,
                    message = ok ? $"Requerimiento #{idRequerimiento} actualizado a '{estado}'." : "No se encontró el requerimiento."
                });
            }
            catch
            {
                return Json(new { success = false, message = "Error al actualizar el estado del requerimiento." });
            }
        }
    }
}
