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
    [ProtegerPanel("Enfermería", "Tópico", "Alumno")]
    public class EnfermeriaController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AtencionMedicaRepository _repo;

        public EnfermeriaController(IConfiguration configuration)
        {
            _configuration = configuration;
            string conn = ConfiguracionSegura.ObtenerCadenaConexion(_configuration);
            _repo = new AtencionMedicaRepository(conn);
        }

        [HttpGet("")]
        [HttpGet("Panel")]
        [ProtegerPanel("Enfermería", "Tópico")]
        
        [HttpGet("PanelPartial")]
        public IActionResult PanelPartial()
        {
            List<MedicamentoItem> meds;
            try
            {
                meds = _repo.ListarMedicamentos();
            }
            catch
            {
                meds = new List<MedicamentoItem>();
            }
            return PartialView("~/Areas/Modulo05/Views/enfermeria/panel_enfermeria.cshtml", meds);
        }

        public IActionResult Panel()
        {
            return Redirect("/Dashboard/Topico");
        }

        [HttpGet("Create")]
        [HttpGet("FormAtencion")]
        [ProtegerPanel("Enfermería", "Tópico")]
        public IActionResult FormAtencion(bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            ViewData["Title"] = "Registro de Atención Médica";
            ViewData["Role"] = "Enfermería (Tópico)";
            try
            {
                ViewBag.Medicamentos = _repo.ListarMedicamentos();
            }
            catch
            {
                ViewBag.Medicamentos = new List<MedicamentoItem>();
            }

            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/enfermeria/form_atencion.cshtml");
            }

            return View("~/Areas/Modulo05/Views/enfermeria/form_atencion.cshtml");
        }

        [HttpGet("HistorialPaciente")]
        public IActionResult HistorialPaciente(string? dni = null, bool partial = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            ViewData["Title"] = "Historial Clínico del Paciente";
            ViewData["Role"] = "Enfermería (Tópico)";
            List<AtencionMedicaItem> lista;
            try
            {
                lista = !string.IsNullOrWhiteSpace(dni)
                    ? _repo.ObtenerHistorialPorDni(dni)
                    : new List<AtencionMedicaItem>();
            }
            catch
            {
                lista = new List<AtencionMedicaItem>();
            }

            ViewBag.DniBuscado = dni;
            if (partial || isAjax)
            {
                return PartialView("~/Areas/Modulo05/Views/enfermeria/historial_paciente.cshtml", lista);
            }
            return View("~/Areas/Modulo05/Views/enfermeria/historial_paciente.cshtml", lista);
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
                var paciente = _repo.BuscarPacientePorDni(dni);
                if (paciente != null)
                {
                    return Json(new
                    {
                        success = true,
                        idPersona = paciente.IdPersona,
                        nombres = paciente.Nombres,
                        apellidos = paciente.Apellidos,
                        carrera = "Desarrollo de Sistemas de Información",
                        cicloTurno = "IV Ciclo / Turno Noche",
                        tipoSeguro = "SIS Gratuito / EsSalud",
                        alergiasConocidas = "Ninguna reportada en el padrón"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = $"No se encontró paciente con DNI {dni} en el padrón institucional."
                });
            }
            catch
            {
                return Json(new { success = false, message = "No se pudo consultar la base de datos en este momento." });
            }
        }

        [HttpPost("GuardarAtencion")]
        [ProtegerPanel("Enfermería", "Tópico")]
        public IActionResult GuardarAtencion([FromForm] IFormCollection form)
        {
            try
            {
                string? dni = form["buscarDni"].ToString();
                if (string.IsNullOrWhiteSpace(dni)) dni = form["dni"].ToString();

                string? motivo = form["motivoAtencion"].ToString();
                if (string.IsNullOrWhiteSpace(motivo)) motivo = form["motivo"].ToString();

                string? fechaHoraStr = form["fechaHora"].ToString();
                string? medicamentosStr = form["medicamentosAdministrados"].ToString();
                string? observaciones = form["observacionesTratamiento"].ToString();
                string? seguro = form["tipoSeguroSalud"].ToString();
                bool esAlergico = !string.IsNullOrEmpty(form["reportaAlergias"]) || form["esAlergico"] == "true";
                string? detalleAlergia = form["especificarAlergias"].ToString();

                if (string.IsNullOrWhiteSpace(dni))
                {
                    return Json(new { success = false, message = "Debe buscar y validar un DNI primero." });
                }

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    return Json(new { success = false, message = "El motivo de atención es obligatorio." });
                }

                var paciente = _repo.BuscarPacientePorDni(dni);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "El paciente no está registrado en el sistema." });
                }

                DateTime fechaHora = DateTime.Now;
                if (!string.IsNullOrEmpty(fechaHoraStr) && DateTime.TryParse(fechaHoraStr, out DateTime parsed))
                {
                    fechaHora = parsed;
                }

                int? medicamentoId = null;
                if (int.TryParse(form["medicamentoId"].ToString(), out int medId) && medId > 0)
                {
                    medicamentoId = medId;
                }

                string obsCompleta = observaciones ?? "";
                if (!string.IsNullOrWhiteSpace(medicamentosStr))
                {
                    obsCompleta = $"Medicamento recetado: {medicamentosStr}\n{obsCompleta}".Trim();
                }

                int usuarioActualId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;

                var item = new AtencionMedicaItem
                {
                    PersonaId = paciente.IdPersona,
                    AtendidoPorId = usuarioActualId > 0 && usuarioActualId < 100 ? usuarioActualId : 1,
                    FechaHora = fechaHora,
                    Motivo = motivo,
                    MedicamentoId = medicamentoId,
                    EsAlergico = esAlergico,
                    DetalleAlergia = detalleAlergia,
                    SeguroSalud = string.IsNullOrWhiteSpace(seguro) ? "SIS" : seguro,
                    Observacion = obsCompleta
                };

                bool ok = _repo.RegistrarAtencion(item);
                if (ok)
                {
                    return Json(new
                    {
                        success = true,
                        message = "¡Atención clínica registrada exitosamente en el sistema de Tópico!"
                    });
                }

                return Json(new { success = false, message = "No se pudo registrar la atención." });
            }
            catch
            {
                return Json(new { success = false, message = "Error al procesar el registro en la base de datos." });
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
                var atenciones = _repo.ObtenerHistorialPorDni(dni);
                return Json(new { success = true, data = atenciones });
            }
            catch
            {
                return Json(new { success = false, message = "No se pudo consultar el historial médico." });
            }
        }

        [HttpGet("Medicamentos")]
        public IActionResult Medicamentos()
        {
            try
            {
                var lista = _repo.ListarMedicamentos();
                return Json(new { success = true, data = lista });
            }
            catch
            {
                return Json(new { success = false, message = "No se pudo listar el inventario de botica." });
            }
        }
    }
}
