using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Login_Administracion_de_Modulos.Areas.Modulo05.Models;
using Login_Administracion_de_Modulos.Seguridad;

namespace Login_Administracion_de_Modulos.Areas.Modulo05.Controller
{
    /// <summary>
    /// Controlador unificado con AtencionMedicaRepository y protegido por sesión/rol.
    /// Delega las consultas SQL al repositorio en lugar de duplicar SQL crudo en el controlador.
    /// </summary>
    [Area("Modulo05")]
    [Route("Modulo05/[controller]")]
    [ProtegerPanel("Enfermería", "Tópico", "Alumno")]
    public class AtencionClinicaController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AtencionMedicaRepository _repo;

        public AtencionClinicaController(IConfiguration configuration)
        {
            string conn = ConfiguracionSegura.ObtenerCadenaConexion(configuration);
            _repo = new AtencionMedicaRepository(conn);
        }

        [HttpGet("Create")]
        [ProtegerPanel("Enfermería", "Tópico")]
        public IActionResult Create(bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/enfermeria/form_atencion.cshtml");
            }

            ViewData["Title"] = "Registro de Atención Clínica";
            ViewData["Role"] = "Enfermería (Tópico)";
            return View("~/Areas/Modulo05/Views/enfermeria/form_atencion.cshtml");
        }

        [HttpGet("BuscarPaciente")]
        [ProtegerPanel("Enfermería", "Tópico")]
        public IActionResult BuscarPaciente(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Json(new { success = false, message = "Ingrese un número de DNI válido." });
            }

            try
            {
                var paciente = _repo.BuscarPacientePorDni(dni.Trim());
                if (paciente != null)
                {
                    return Json(new
                    {
                        success = true,
                        nombres = paciente.Nombres,
                        apellidos = paciente.Apellidos,
                        carrera = "Desarrollo de Sistemas de Información",
                        cicloTurno = "IV Ciclo / Turno Noche"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = $"No se encontró registro para el DNI {dni} en el padrón."
                });
            }
            catch
            {
                return Json(new { success = false, message = "Error al consultar paciente en la base de datos." });
            }
        }

        [HttpPost("GuardarAtencion")]
        [ProtegerPanel("Enfermería", "Tópico")]
        public IActionResult GuardarAtencion([FromForm] IFormCollection form)
        {
            try
            {
                string? dni = form["buscarDni"];
                string? motivo = form["motivoAtencion"];
                string? fechaHoraStr = form["fechaHora"];
                string? medicamentos = form["medicamentosAdministrados"];
                string? observaciones = form["observacionesTratamiento"];
                string? tipoSeguroSalud = form["tipoSeguroSalud"];
                bool esAlergico = !string.IsNullOrEmpty(form["reportaAlergias"]);
                string? detalleAlergia = form["especificarAlergias"];

                if (string.IsNullOrWhiteSpace(dni))
                {
                    return Json(new { success = false, message = "Debe buscar y validar un DNI primero." });
                }

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    return Json(new { success = false, message = "El motivo de atención es obligatorio." });
                }

                var paciente = _repo.BuscarPacientePorDni(dni.Trim());
                if (paciente == null)
                {
                    return Json(new { success = false, message = "El DNI no está registrado en el sistema." });
                }

                DateTime fechaHora = DateTime.Now;
                if (!string.IsNullOrEmpty(fechaHoraStr) && DateTime.TryParse(fechaHoraStr, out DateTime parsedDate))
                {
                    fechaHora = parsedDate;
                }

                string observacionFinal = observaciones ?? "";
                if (!string.IsNullOrWhiteSpace(medicamentos))
                {
                    observacionFinal = $"Medicamentos: {medicamentos}\n{observacionFinal}".Trim();
                }

                int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;

                var item = new AtencionMedicaItem
                {
                    PersonaId = paciente.IdPersona,
                    AtendidoPorId = usuarioId > 0 && usuarioId < 100 ? usuarioId : 1,
                    FechaHora = fechaHora,
                    Motivo = motivo,
                    EsAlergico = esAlergico,
                    DetalleAlergia = detalleAlergia,
                    SeguroSalud = tipoSeguroSalud,
                    Observacion = string.IsNullOrEmpty(observacionFinal) ? null : observacionFinal
                };

                bool ok = _repo.RegistrarAtencion(item);
                return Json(new
                {
                    success = ok,
                    message = ok ? "¡Atención clínica registrada exitosamente en el sistema de Tópico!" : "No se pudo registrar la atención."
                });
            }
            catch
            {
                return Json(new { success = false, message = "Error al procesar el registro clínico." });
            }
        }

        [HttpGet("Historial")]
        public IActionResult Historial(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Json(new { success = false, message = "DNI no proporcionado." });
            }

            try
            {
                var historial = _repo.ObtenerHistorialPorDni(dni.Trim());
                var atenciones = new List<object>();
                foreach (var a in historial)
                {
                    atenciones.Add(new
                    {
                        fechaHora = a.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                        motivo = a.Motivo,
                        seguroSalud = a.SeguroSalud ?? "Ninguno",
                        observacion = a.Observacion ?? "",
                        esAlergico = a.EsAlergico,
                        detalleAlergia = a.DetalleAlergia ?? ""
                    });
                }
                return Json(new { success = true, data = atenciones });
            }
            catch
            {
                return Json(new { success = false, message = "Error al consultar el historial clínico." });
            }
        }
    }
}
