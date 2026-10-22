using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Login_Administracion_de_Modulos.Models;
using Login_Administracion_de_Modulos.Seguridad;

namespace Login_Administracion_de_Modulos.Controlador
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = ConfiguracionSegura.ObtenerCadenaConexion(configuration);
        }

        [HttpGet("/Dashboard")]
        [HttpGet("/Dashboard/Index")]
        public IActionResult Index()
        {
            string rolActual = HttpContext.Session.GetString("UsuarioRolSlug")
                            ?? HttpContext.Session.GetString("UsuarioRol")
                            ?? "topico";
            var perfil = ConfiguracionPanelesRol.ObtenerPorRolOSlug(rolActual);
            return Redirect(perfil.RutaDashboard);
        }

        // Rutas oficiales de los 14 roles según CONTEXTO_PANELES_ROLES.md
        [HttpGet("/Dashboard/Alumno")]
        public IActionResult Alumno() => RenderizarPanelPorRol("alumno", "~/Vista/Dashboard/Alumno.cshtml");

        [HttpGet("/Dashboard/Docente")]
        public IActionResult Docente() => RenderizarPanelPorRol("docente", "~/Vista/Dashboard/Docente.cshtml");

        [HttpGet("/Dashboard/Soporte")]
        public IActionResult Soporte() => RenderizarPanelPorRol("soporte", "~/Vista/Dashboard/Soporte.cshtml");

        [HttpGet("/Dashboard/Practicante")]
        public IActionResult Practicante() => RenderizarPanelPorRol("practicante", "~/Vista/Dashboard/Practicante.cshtml");

        [HttpGet("/Dashboard/Secretaria")]
        public IActionResult Secretaria() => RenderizarPanelPorRol("secretaria", "~/Vista/Dashboard/Secretaria.cshtml");

        [HttpGet("/Dashboard/Tesoreria")]
        public IActionResult Tesoreria() => RenderizarPanelPorRol("tesoreria", "~/Vista/Dashboard/Tesoreria.cshtml");

        [HttpGet("/Dashboard/Administrador")]
        public IActionResult Administrador() => RenderizarPanelPorRol("administrador", "~/Vista/Dashboard/Administrador.cshtml");

        [HttpGet("/Dashboard/JefeGeneral")]
        public IActionResult JefeGeneral() => RenderizarPanelPorRol("jefegeneral", "~/Vista/Dashboard/JefeGeneral.cshtml");

        [HttpGet("/Dashboard/JefeComputacion")]
        public IActionResult JefeComputacion() => RenderizarPanelPorRol("jefecomputacion", "~/Vista/Dashboard/JefeComputacion.cshtml");

        [HttpGet("/Dashboard/JefeAdministracion")]
        public IActionResult JefeAdministracion() => RenderizarPanelPorRol("jefeadministracion", "~/Vista/Dashboard/JefeAdministracion.cshtml");

        [HttpGet("/Dashboard/JefeContabilidad")]
        public IActionResult JefeContabilidad() => RenderizarPanelPorRol("jefecontabilidad", "~/Vista/Dashboard/JefeContabilidad.cshtml");

        [HttpGet("/Dashboard/EncargadoInventario")]
        public IActionResult EncargadoInventario() => RenderizarPanelPorRol("encargadoinventario", "~/Vista/Dashboard/EncargadoInventario.cshtml");

        [HttpGet("/Dashboard/Topico")]
        public IActionResult Topico() => RenderizarPanelPorRol("topico", "~/Vista/Dashboard/Topico.cshtml");

        [HttpGet("/Dashboard/Psicologia")]
        public IActionResult Psicologia() => RenderizarPanelPorRol("psicologia", "~/Vista/Dashboard/Psicologia.cshtml");

        [HttpGet("/Dashboard/Panel/{rolSlug}")]
        public IActionResult Panel(string rolSlug) => RenderizarPanelPorRol(rolSlug, "~/Vista/Dashboard/PanelDinamico.cshtml");

        /// <summary>
        /// Endpoint que carga dinámicamente las vistas internas y tablas de PostgreSQL en el contenedor SPA
        /// cuando el usuario navega por las opciones del Sidebar Dinámico.
        /// </summary>
        [HttpGet("/Dashboard/VistaInterna")]
        public IActionResult VistaInterna(string vista = "resumen_rol", string? rol = null, string? filtro = null)
        {
            string rolSlug = rol ?? HttpContext.Session.GetString("UsuarioRolSlug") ?? "administrador";
            var perfil = ConfiguracionPanelesRol.ObtenerPorRolOSlug(rolSlug);
            var vm = ConstruirViewModel(perfil);
            vm.TablaPrincipal = ConstruirTablaPorVistaInterna(vista, perfil, filtro);
            ViewBag.VistaInternaActual = vista;
            ViewBag.FiltroActual = filtro;
            return PartialView("~/Vista/Dashboard/_ContenidoDinamicoPartial.cshtml", vm);
        }

        private IActionResult RenderizarPanelPorRol(string slug, string viewPath)
        {
            var perfil = ConfiguracionPanelesRol.ObtenerPorRolOSlug(slug);
            CargarDatosSesion(perfil);
            var vm = ConstruirViewModel(perfil);
            ViewData["Title"] = $"Panel {perfil.NombreRol} - IESTP Argentina";
            ViewData["Role"] = perfil.NombreRol;
            ViewBag.DashboardModel = vm;
            return View(viewPath, vm);
        }

        private void CargarDatosSesion(PerfilRolConfig perfil)
        {
            string? rolEnSesion = HttpContext.Session.GetString("UsuarioRolSlug");
            if (string.IsNullOrWhiteSpace(rolEnSesion) || !rolEnSesion.Equals(perfil.Slug, StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetInt32("UsuarioId", perfil.UsuarioDemoId);
                HttpContext.Session.SetInt32("PersonaId", perfil.UsuarioDemoId);
                HttpContext.Session.SetString("UsuarioDni", perfil.UsuarioDemoDni);
                HttpContext.Session.SetString("UsuarioNombre", perfil.UsuarioDemoNombre);
                HttpContext.Session.SetString("UsuarioCorreo", perfil.UsuarioDemoCorreo);
                HttpContext.Session.SetString("UsuarioRol", perfil.NombreRol);
                HttpContext.Session.SetString("UsuarioRolSlug", perfil.Slug);
            }

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("UsuarioNombre") ?? perfil.UsuarioDemoNombre;
            ViewBag.UsuarioRol = perfil.NombreRol;
            ViewBag.UsuarioDni = HttpContext.Session.GetString("UsuarioDni") ?? perfil.UsuarioDemoDni;
            ViewBag.UsuarioCorreo = HttpContext.Session.GetString("UsuarioCorreo") ?? perfil.UsuarioDemoCorreo;
        }

        private DashboardViewModel ConstruirViewModel(PerfilRolConfig perfil)
        {
            string nombre = HttpContext.Session.GetString("UsuarioNombre") ?? perfil.UsuarioDemoNombre;
            string dni = HttpContext.Session.GetString("UsuarioDni") ?? perfil.UsuarioDemoDni;
            string correo = HttpContext.Session.GetString("UsuarioCorreo") ?? perfil.UsuarioDemoCorreo;
            string modo = HttpContext.Session.GetString("ModoConexion") ?? "PostgreSQL Online";

            var partes = nombre.Replace("Lic.", "").Replace("Ing.", "").Replace("Mg.", "").Replace("Dr.", "").Replace("CPC.", "")
                               .Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string iniciales = partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpperInvariant()
                : (partes.Length == 1 && partes[0].Length >= 2 ? partes[0].Substring(0, 2).ToUpperInvariant() : "UI");

            var vm = new DashboardViewModel
            {
                PerfilRol = perfil,
                UsuarioNombre = nombre,
                UsuarioDni = dni,
                UsuarioCorreo = correo,
                InicialesAvatar = iniciales,
                ModoConexion = modo,
                TodosLosRoles = ConfiguracionPanelesRol.ObtenerTodosLosRoles()
            };

            CargarMetricasYResumenDesdePostgres(vm, perfil);
            return vm;
        }

        private void CargarMetricasYResumenDesdePostgres(DashboardViewModel vm, PerfilRolConfig perfil)
        {
            int totalPersonas = 10, totalUsuarios = 9, totalBienes = 3, totalIncidencias = 2, totalRequerimientos = 2, totalTopico = 3, totalPsico = 2;
            bool dbOnline = false;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                dbOnline = true;

                const string sqlCounts = @"
                    SELECT
                        (SELECT COUNT(*) FROM personas),
                        (SELECT COUNT(*) FROM usuarios),
                        (SELECT COUNT(*) FROM bien),
                        (SELECT COUNT(*) FROM incidencias),
                        (SELECT COUNT(*) FROM requerimientos),
                        (SELECT COUNT(*) FROM atenciones_topico),
                        (SELECT COUNT(*) FROM atenciones_psicologia);";

                using var cmd = new NpgsqlCommand(sqlCounts, conn);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    totalPersonas = Convert.ToInt32(r.GetInt64(0));
                    totalUsuarios = Convert.ToInt32(r.GetInt64(1));
                    totalBienes = Convert.ToInt32(r.GetInt64(2));
                    totalIncidencias = Convert.ToInt32(r.GetInt64(3));
                    totalRequerimientos = Convert.ToInt32(r.GetInt64(4));
                    totalTopico = Convert.ToInt32(r.GetInt64(5));
                    totalPsico = Convert.ToInt32(r.GetInt64(6));
                }
            }
            catch
            {
                dbOnline = false;
            }

            vm.BaseDatosConectada = dbOnline;
            vm.Metricas = new List<MetricaCardDto>
            {
                new() { Titulo = "Padrón Personas", Valor = totalPersonas.ToString(), Subtitulo = "Registrados en PostgreSQL", Icono = "ph-users-three", Color = "#00f2fe" },
                new() { Titulo = "Activos / Bienes", Valor = totalBienes.ToString(), Subtitulo = string.IsNullOrEmpty(perfil.EspecialidadFiltro) ? "Inventario Institucional" : $"Área: {perfil.EspecialidadFiltro}", Icono = "ph-hard-drives", Color = "#10b981" },
                new() { Titulo = "Incidencias (Mód 05)", Valor = totalIncidencias.ToString(), Subtitulo = $"{totalRequerimientos} Requerimientos TI", Icono = "ph-wrench", Color = "#f59e0b" },
                new() { Titulo = "Salud y Bienestar", Valor = (totalTopico + totalPsico).ToString(), Subtitulo = $"{totalTopico} Tópico • {totalPsico} Psicología", Icono = "ph-heartbeat", Color = "#a855f7" }
            };

            vm.TablaPrincipal = ConstruirTablaPorVistaInterna("resumen_rol", perfil, null);
        }

        private TablaDinamicaDto ConstruirTablaPorVistaInterna(string vista, PerfilRolConfig perfil, string? filtro)
        {
            var tabla = new TablaDinamicaDto
            {
                Titulo = $"Registros en Tiempo Real — {perfil.NombreRol}",
                Subtitulo = $"Sincronizado con tablas de bd.sql: {string.Join(", ", perfil.TablasRelacionadas)}",
                Icono = perfil.IconoRol
            };

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                switch (vista)
                {
                    case "tabla_usuarios":
                        tabla.Titulo = "Gestión de Usuarios Institucionales (Tabla: usuarios + roles)";
                        tabla.Columnas = new List<string> { "ID", "DNI Login", "Nombres y Apellidos", "Correo Institucional", "Rol Asignado", "Estado" };
                        string sqlU = @"
                            SELECT u.id_usuario, u.dni_login, p.nombres || ' ' || p.apellidos, u.correo, COALESCE(r.nombre, 'Alumno'), u.estado
                            FROM usuarios u
                            INNER JOIN personas p ON u.id_persona = p.id_persona
                            LEFT JOIN usuarios_roles ur ON u.id_usuario = ur.id_usuario
                            LEFT JOIN roles r ON ur.id_rol = r.id_rol
                            ORDER BY u.id_usuario ASC LIMIT 50";
                        using (var cmd = new NpgsqlCommand(sqlU, conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#{rd.GetInt32(0)}",
                                    rd.IsDBNull(1) ? "-" : rd.GetString(1),
                                    rd.IsDBNull(2) ? "-" : rd.GetString(2),
                                    rd.IsDBNull(3) ? "-" : rd.GetString(3),
                                    rd.IsDBNull(4) ? "Alumno" : rd.GetString(4),
                                    rd.IsDBNull(5) ? "Activo" : rd.GetString(5)
                                });
                            }
                        }
                        break;

                    case "tabla_roles":
                        tabla.Titulo = "Catálogo de Roles y Perfiles de Acceso (Tabla: roles)";
                        tabla.Columnas = new List<string> { "ID Rol", "Nombre del Rol", "Descripción Operativa", "Estado", "Fecha Creación" };
                        using (var cmd = new NpgsqlCommand("SELECT id_rol, nombre, COALESCE(descripcion,''), estado, creado_el FROM roles ORDER BY id_rol ASC", conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3),
                                    rd.IsDBNull(4) ? "-" : rd.GetDateTime(4).ToString("dd/MM/yyyy")
                                });
                            }
                        }
                        break;

                    case "tabla_permisos":
                        tabla.Titulo = "Permisos de Módulos del Sistema (Tabla: permisos_modulos)";
                        tabla.Columnas = new List<string> { "ID Permiso", "Módulo", "Nivel de Acceso", "Descripción" };
                        using (var cmd = new NpgsqlCommand("SELECT id_permiso, nombre_modulo, nivel_acceso, COALESCE(descripcion,'') FROM permisos_modulos ORDER BY id_permiso ASC", conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3)
                                });
                            }
                        }
                        break;

                    case "tabla_sesiones":
                        tabla.Titulo = "Auditoría de Sesiones de Login y Google SSO (Tabla: sesiones_login)";
                        tabla.Columnas = new List<string> { "UUID Sesión", "Usuario (DNI)", "Método Autenticación", "IP Origen", "Inicio Sesión", "Estado" };
                        string sqlSes = @"
                            SELECT s.id_sesion::text, COALESCE(u.dni_login, 'N/A') || ' - ' || COALESCE(u.correo, ''), s.metodo_autenticacion, COALESCE(s.ip_origen, '127.0.0.1'), s.fecha_inicio, s.estado
                            FROM sesiones_login s
                            LEFT JOIN usuarios u ON s.id_usuario = u.id_usuario
                            ORDER BY s.fecha_inicio DESC LIMIT 30";
                        using (var cmd = new NpgsqlCommand(sqlSes, conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    rd.GetString(0).Substring(0, 8) + "...",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3),
                                    rd.GetDateTime(4).ToString("dd/MM/yyyy HH:mm"),
                                    rd.GetString(5)
                                });
                            }
                        }
                        break;

                    case "tabla_ambientes":
                        tabla.Titulo = "Ambientes, Aulas y Laboratorios (Tabla: ambiente + especialidad)";
                        tabla.Columnas = new List<string> { "ID Ambiente", "Nombre del Ambiente / Laboratorio", "Especialidad", "Estado" };
                        string sqlAmb = @"
                            SELECT a.id_ambiente, a.nombre, COALESCE(e.nombre, 'General'), a.estado::text
                            FROM ambiente a
                            LEFT JOIN especialidad e ON a.id_especialidad = e.id_especialidad
                            ORDER BY a.id_ambiente ASC";
                        using (var cmd = new NpgsqlCommand(sqlAmb, conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#AMB-{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3)
                                });
                            }
                        }
                        break;

                    case "tabla_almacenes":
                        tabla.Titulo = "Almacenes Institucionales (Tabla: almacen)";
                        tabla.Columnas = new List<string> { "ID Almacén", "Nombre", "Ubicación Física", "Responsable", "Especialidad", "Estado" };
                        string sqlAlm = @"
                            SELECT al.id_almacen, al.nombre, COALESCE(al.ubicacion, '-'), COALESCE(al.responsable, '-'), COALESCE(e.nombre, 'General'), al.estado::text
                            FROM almacen al
                            LEFT JOIN especialidad e ON al.id_especialidad = e.id_especialidad
                            ORDER BY al.id_almacen ASC";
                        using (var cmd = new NpgsqlCommand(sqlAlm, conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#ALM-{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3),
                                    rd.GetString(4),
                                    rd.GetString(5)
                                });
                            }
                        }
                        break;

                    case "tabla_especialidades":
                        tabla.Titulo = "Programas de Estudio y Especialidades (Tabla: especialidad)";
                        tabla.Columnas = new List<string> { "ID", "Programa de Estudios / Especialidad", "Estado" };
                        using (var cmd = new NpgsqlCommand("SELECT id_especialidad, nombre, estado::text FROM especialidad ORDER BY id_especialidad ASC", conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#ESP-{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2)
                                });
                            }
                        }
                        break;

                    case "padron_personas":
                    default:
                        tabla.Titulo = $"Padrón Institucional y Actividad Reciente ({perfil.NombreRol})";
                        tabla.Columnas = new List<string> { "ID Persona", "Tipo Doc.", "N° Documento (DNI)", "Nombres", "Apellidos", "Cuenta Vinculada" };
                        string sqlPer = @"
                            SELECT p.id_persona, p.tipo_documento, p.numero_documento, p.nombres, p.apellidos, COALESCE(u.correo, 'Sin correo asignado')
                            FROM personas p
                            LEFT JOIN usuarios u ON p.id_persona = u.id_persona
                            ORDER BY p.id_persona ASC LIMIT 50";
                        using (var cmd = new NpgsqlCommand(sqlPer, conn))
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                tabla.Filas.Add(new List<string>
                                {
                                    $"#{rd.GetInt32(0)}",
                                    rd.GetString(1),
                                    rd.GetString(2),
                                    rd.GetString(3),
                                    rd.GetString(4),
                                    rd.GetString(5)
                                });
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                tabla.Columnas = new List<string> { "Estado", "Detalle" };
                tabla.Filas.Add(new List<string> { "Modo Local", $"Consulta local activa ({ex.Message})" });
            }

            return tabla;
        }
    }
}
