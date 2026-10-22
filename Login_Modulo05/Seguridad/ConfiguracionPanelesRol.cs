using System;
using System.Collections.Generic;
using System.Linq;

namespace Login_Administracion_de_Modulos.Seguridad
{
    public class OpcionSidebar
    {
        public string Id { get; set; } = string.Empty;
        public string Seccion { get; set; } = "MENÚ PRINCIPAL";
        public string Titulo { get; set; } = string.Empty;
        public string Icono { get; set; } = "ph-squares-four";
        public string? UrlModulo { get; set; }
        public string? VistaInterna { get; set; }
        public string? BadgeTexto { get; set; }
        public string? BadgeColor { get; set; }
        public bool EsActivoPorDefecto { get; set; }
    }

    public class PerfilRolConfig
    {
        public string Slug { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string RutaDashboard { get; set; } = string.Empty;
        public string IconoRol { get; set; } = "ph-user-circle";
        public string ColorPrimario { get; set; } = "#00f2fe";
        public string ColorSecundario { get; set; } = "#0284c7";
        public string UsuarioDemoNombre { get; set; } = string.Empty;
        public string UsuarioDemoDni { get; set; } = string.Empty;
        public string UsuarioDemoCorreo { get; set; } = string.Empty;
        public int UsuarioDemoId { get; set; } = 1;
        public string EspecialidadFiltro { get; set; } = string.Empty;
        public bool EsDestacadoDemo { get; set; }
        public string[] TablasRelacionadas { get; set; } = Array.Empty<string>();
        public List<OpcionSidebar> OpcionesSidebar { get; set; } = new();
    }

    /// <summary>
    /// Motor centralizado de Paneles Laterales Dinámicos (Sidebars) para los 14 roles del sistema.
    /// Evita duplicar layouts estáticos y recicla los formularios del Módulo 05 según permisos de cada rol.
    /// </summary>
    public static class ConfiguracionPanelesRol
    {
        private static readonly List<PerfilRolConfig> _roles = new()
        {
            // 1. ENFERMERÍA (TÓPICO) - Destacado Demo 1
            new PerfilRolConfig
            {
                Slug = "topico",
                NombreRol = "Enfermería (Tópico)",
                Subtitulo = "Módulo de Salud y Triaje",
                RutaDashboard = "/Dashboard/Topico",
                IconoRol = "ph-plus-circle",
                ColorPrimario = "#10b981",
                ColorSecundario = "#06b6d4",
                UsuarioDemoNombre = "Lic. Rosa María García",
                UsuarioDemoDni = "44556677",
                UsuarioDemoCorreo = "rmgarcia@instituto.edu.pe",
                UsuarioDemoId = 2,
                EsDestacadoDemo = true,
                TablasRelacionadas = new[] { "atenciones_topico", "botica_medicamentos", "personas" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "resumen", Seccion = "SALUD Y TRIAJE", Titulo = "Dashboard / Resumen", Icono = "ph-heartbeat", UrlModulo = "/Modulo05/Enfermeria/PanelPartial", EsActivoPorDefecto = true },
                    new() { Id = "atencion-clinica", Seccion = "SALUD Y TRIAJE", Titulo = "Atención Clínica", Icono = "ph-stethoscope", UrlModulo = "/Modulo05/Enfermeria/FormAtencion?partial=true", BadgeTexto = "Mód 05", BadgeColor = "#10b981" },
                    new() { Id = "botica", Seccion = "SALUD Y TRIAJE", Titulo = "Botica y Medicamentos", Icono = "ph-pill", UrlModulo = "/Modulo05/Enfermeria/PanelPartial" },
                    new() { Id = "historial-pacientes", Seccion = "SALUD Y TRIAJE", Titulo = "Historial de Pacientes", Icono = "ph-folder-Simple-user", UrlModulo = "/Modulo05/Enfermeria/HistorialPaciente?partial=true" },
                    new() { Id = "requerimientos-topico", Seccion = "GESTIÓN INTERNA", Titulo = "Solicitar Insumos / Req.", Icono = "ph-clipboard-text", UrlModulo = "/Modulo05/Requerimientos/Create?partial=true" }
                }
            },

            // 2. PSICOLOGÍA - Destacado Demo 2
            new PerfilRolConfig
            {
                Slug = "psicologia",
                NombreRol = "Psicología",
                Subtitulo = "Bienestar Estudiantil y Casos",
                RutaDashboard = "/Dashboard/Psicologia",
                IconoRol = "ph-brain",
                ColorPrimario = "#6366f1",
                ColorSecundario = "#a855f7",
                UsuarioDemoNombre = "Lic. Carlos Alberto Mendoza",
                UsuarioDemoDni = "48123456",
                UsuarioDemoCorreo = "cmendoza@instituto.edu.pe",
                UsuarioDemoId = 3,
                EsDestacadoDemo = true,
                TablasRelacionadas = new[] { "atenciones_psicologia", "personas" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "bienestar", Seccion = "BIENESTAR ESTUDIANTIL", Titulo = "Dashboard / Bienestar", Icono = "ph-brain", UrlModulo = "/Modulo05/Psicologia/PanelPartial", EsActivoPorDefecto = true },
                    new() { Id = "atencion-psico", Seccion = "BIENESTAR ESTUDIANTIL", Titulo = "Atención Psicológica", Icono = "ph-user-focus", UrlModulo = "/Modulo05/Psicologia/FormSesion?partial=true", BadgeTexto = "Mód 05", BadgeColor = "#6366f1" },
                    new() { Id = "citas-programadas", Seccion = "BIENESTAR ESTUDIANTIL", Titulo = "Citas Programadas", Icono = "ph-calendar-check", UrlModulo = "/Modulo05/Psicologia/PanelPartial" },
                    new() { Id = "historial-clinico", Seccion = "BIENESTAR ESTUDIANTIL", Titulo = "Historial Clínico", Icono = "ph-lock-key", UrlModulo = "/Modulo05/Psicologia/ExpedienteAlumno?partial=true" },
                    new() { Id = "padron-pacientes", Seccion = "BIENESTAR ESTUDIANTIL", Titulo = "Padrón de Pacientes", Icono = "ph-users-three", VistaInterna = "padron_personas" }
                }
            },

            // 3. DOCENTE - Destacado Demo 3
            new PerfilRolConfig
            {
                Slug = "docente",
                NombreRol = "Docente",
                Subtitulo = "Portal Docente y Reporte de Averías",
                RutaDashboard = "/Dashboard/Docente",
                IconoRol = "ph-chalkboard-teacher",
                ColorPrimario = "#f59e0b",
                ColorSecundario = "#ef4444",
                UsuarioDemoNombre = "Mg. Ricardo Torres (Docente)",
                UsuarioDemoDni = "74114320",
                UsuarioDemoCorreo = "richard.yancced@istpargentina.edu.pe",
                UsuarioDemoId = 6,
                EsDestacadoDemo = true,
                TablasRelacionadas = new[] { "personas", "ambiente", "incidencias", "requerimientos" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "resumen-docente", Seccion = "PORTAL ACADÉMICO", Titulo = "Dashboard", Icono = "ph-squares-four", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "mis-cursos", Seccion = "PORTAL ACADÉMICO", Titulo = "Mis Cursos", Icono = "ph-books", VistaInterna = "cursos_docente" },
                    new() { Id = "asistencia", Seccion = "PORTAL ACADÉMICO", Titulo = "Asistencia", Icono = "ph-check-square-offset", VistaInterna = "asistencia_docente" },
                    new() { Id = "calificaciones", Seccion = "PORTAL ACADÉMICO", Titulo = "Calificaciones", Icono = "ph-exam", VistaInterna = "calificaciones_docente" },
                    new() { Id = "incidencias-aula", Seccion = "SOPORTE Y LABORATORIOS (MÓD 05)", Titulo = "Incidencias de Aula", Icono = "ph-warning-octagon", UrlModulo = "/Modulo05/AveriaDocente/Reporte?partial=true", BadgeTexto = "Reportar", BadgeColor = "#f59e0b" },
                    new() { Id = "mis-reportes-aula", Seccion = "SOPORTE Y LABORATORIOS (MÓD 05)", Titulo = "Mis Reportes Enviados", Icono = "ph-list-Checks", UrlModulo = "/Modulo05/AveriaDocente/MisReportes?partial=true" },
                    new() { Id = "soporte-ti-req", Seccion = "SOPORTE Y LABORATORIOS (MÓD 05)", Titulo = "Soporte TI (Requerimientos)", Icono = "ph-headset", UrlModulo = "/Modulo05/Requerimientos/Create?partial=true" }
                }
            },

            // 4. JEFE DE SOPORTE - Destacado Demo 4
            new PerfilRolConfig
            {
                Slug = "soporte",
                NombreRol = "Jefe de Soporte",
                Subtitulo = "Jefatura TI, Consola e Inventario",
                RutaDashboard = "/Dashboard/Soporte",
                IconoRol = "ph-shield-check",
                ColorPrimario = "#0ea5e9",
                ColorSecundario = "#0284c7",
                UsuarioDemoNombre = "Ing. Paulo Carreón (Jefe Soporte TI)",
                UsuarioDemoDni = "60914536",
                UsuarioDemoCorreo = "paulo.carreons@istpargentina.edu.pe",
                UsuarioDemoId = 5,
                EsDestacadoDemo = true,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "asignacion", "transferencia", "inventario_anual", "incidencias", "requerimientos" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "dashboard-ti", Seccion = "JEFATURA DE SOPORTE TI", Titulo = "Dashboard TI", Icono = "ph-cpu", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?partial=true", EsActivoPorDefecto = true },
                    new() { Id = "incidencias-tecnicas", Seccion = "JEFATURA DE SOPORTE TI", Titulo = "Incidencias Técnicas", Icono = "ph-wrench", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?partial=true", BadgeTexto = "Mód 05", BadgeColor = "#0ea5e9" },
                    new() { Id = "requerimientos-ti", Seccion = "JEFATURA DE SOPORTE TI", Titulo = "Requerimientos", Icono = "ph-clipboard-text", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true", BadgeTexto = "Nuevo", BadgeColor = "#10b981" },
                    new() { Id = "inventario-bienes", Seccion = "PATRIMONIO Y LABORATORIOS", Titulo = "Inventario de Bienes", Icono = "ph-hard-drives", UrlModulo = "/Modulo05/Soporte/HistorialEquipos?partial=true" },
                    new() { Id = "ambientes-labs", Seccion = "PATRIMONIO Y LABORATORIOS", Titulo = "Ambientes y Labs", Icono = "ph-buildings", VistaInterna = "tabla_ambientes" },
                    new() { Id = "asignaciones", Seccion = "PATRIMONIO Y LABORATORIOS", Titulo = "Asignaciones", Icono = "ph-arrows-left-right", VistaInterna = "tabla_asignaciones" },
                    new() { Id = "transferencias", Seccion = "PATRIMONIO Y LABORATORIOS", Titulo = "Transferencias", Icono = "ph-Swap", VistaInterna = "tabla_transferencias" },
                    new() { Id = "inventario-anual", Seccion = "PATRIMONIO Y LABORATORIOS", Titulo = "Inventario Anual", Icono = "ph-calendar-check", VistaInterna = "tabla_inventario_anual" }
                }
            },

            // 5. ALUMNO
            new PerfilRolConfig
            {
                Slug = "alumno",
                NombreRol = "Alumno",
                Subtitulo = "Portal del Estudiante y Servicios",
                RutaDashboard = "/Dashboard/Alumno",
                IconoRol = "ph-student",
                ColorPrimario = "#38bdf8",
                ColorSecundario = "#6366f1",
                UsuarioDemoNombre = "Juan Diego Pérez",
                UsuarioDemoDni = "74296138",
                UsuarioDemoCorreo = "jperez@instituto.edu.pe",
                UsuarioDemoId = 1,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "personas", "especialidad", "atenciones_topico", "atenciones_psicologia", "requerimientos" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "mi-portal", Seccion = "PORTAL ESTUDIANTIL", Titulo = "Mi Portal", Icono = "ph-house-line", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "mis-cursos", Seccion = "PORTAL ESTUDIANTIL", Titulo = "Mis Cursos", Icono = "ph-notebook", VistaInterna = "cursos_alumno" },
                    new() { Id = "calificaciones", Seccion = "PORTAL ESTUDIANTIL", Titulo = "Calificaciones", Icono = "ph-medal", VistaInterna = "calificaciones_alumno" },
                    new() { Id = "asistencias", Seccion = "PORTAL ESTUDIANTIL", Titulo = "Asistencias", Icono = "ph-calendar-check", VistaInterna = "asistencias_alumno" },
                    new() { Id = "citas-topico", Seccion = "BIENESTAR Y SALUD (MÓD 05)", Titulo = "Citas de Tópico", Icono = "ph-first-aid", UrlModulo = "/Modulo05/Enfermeria/HistorialPaciente?dni=74296138&partial=true" },
                    new() { Id = "citas-psicologia", Seccion = "BIENESTAR Y SALUD (MÓD 05)", Titulo = "Citas de Psicología", Icono = "ph-brain", UrlModulo = "/Modulo05/Psicologia/ExpedienteAlumno?dni=74296138&partial=true" },
                    new() { Id = "soporte-ti-alumno", Seccion = "SERVICIOS TI (MÓD 05)", Titulo = "Soporte TI / Requerimientos", Icono = "ph-laptop", UrlModulo = "/Modulo05/Requerimientos/Create?partial=true" }
                }
            },

            // 6. SECRETARÍA ACADÉMICA
            new PerfilRolConfig
            {
                Slug = "secretaria",
                NombreRol = "Secretaría Académica",
                Subtitulo = "Motor Operativo, Trámites y Padrón",
                RutaDashboard = "/Dashboard/Secretaria",
                IconoRol = "ph-files",
                ColorPrimario = "#ec4899",
                ColorSecundario = "#db2777",
                UsuarioDemoNombre = "Lic. Diana Contreras (Secretaría)",
                UsuarioDemoDni = "63229403",
                UsuarioDemoCorreo = "diana.contrerasl@istpargentina.edu.pe",
                UsuarioDemoId = 7,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "personas", "especialidad", "requerimientos" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "dashboard-sec", Seccion = "GESTIÓN ACADÉMICA", Titulo = "Dashboard", Icono = "ph-chart-pie-slice", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "padron-personas", Seccion = "GESTIÓN ACADÉMICA", Titulo = "Padrón de Personas", Icono = "ph-Address-book", VistaInterna = "padron_personas" },
                    new() { Id = "especialidades", Seccion = "GESTIÓN ACADÉMICA", Titulo = "Especialidades", Icono = "ph-graduation-cap", VistaInterna = "tabla_especialidades" },
                    new() { Id = "matriculas-tramites", Seccion = "TRÁMITES DOCUMENTARIOS", Titulo = "Matrículas y Trámites", Icono = "ph-file-text", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true" },
                    new() { Id = "certificados", Seccion = "TRÁMITES DOCUMENTARIOS", Titulo = "Certificados", Icono = "ph-certificate", VistaInterna = "certificados_secretaria" },
                    new() { Id = "reportes-generales", Seccion = "TRÁMITES DOCUMENTARIOS", Titulo = "Reportes Generales", Icono = "ph-chart-bar", VistaInterna = "reportes_institucionales" }
                }
            },

            // 7. PRACTICANTE / AUXILIAR SOPORTE
            new PerfilRolConfig
            {
                Slug = "practicante",
                NombreRol = "Auxiliar Soporte",
                Subtitulo = "Practicante de Soporte Técnico y Labs",
                RutaDashboard = "/Dashboard/Practicante",
                IconoRol = "ph-user-gear",
                ColorPrimario = "#06b6d4",
                ColorSecundario = "#0891b2",
                UsuarioDemoNombre = "Josías Toro (Auxiliar Soporte)",
                UsuarioDemoDni = "72910115",
                UsuarioDemoCorreo = "josias.torog@istpargentina.edu.pe",
                UsuarioDemoId = 8,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "incidencias", "requerimientos", "ambiente", "bien" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "mis-tareas", Seccion = "OPERACIONES DE CAMPO", Titulo = "Mis Tareas", Icono = "ph-list-checks", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?partial=true", EsActivoPorDefecto = true },
                    new() { Id = "incidencias-asignadas", Seccion = "OPERACIONES DE CAMPO", Titulo = "Incidencias Asignadas", Icono = "ph-wrench", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?estado=Abierta&partial=true", BadgeTexto = "Activas", BadgeColor = "#06b6d4" },
                    new() { Id = "requerimientos-prac", Seccion = "OPERACIONES DE CAMPO", Titulo = "Requerimientos", Icono = "ph-clipboard-text", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true" },
                    new() { Id = "revision-ambientes", Seccion = "OPERACIONES DE CAMPO", Titulo = "Revisión de Ambientes", Icono = "ph-buildings", VistaInterna = "tabla_ambientes" },
                    new() { Id = "soluciones-aplicadas", Seccion = "OPERACIONES DE CAMPO", Titulo = "Soluciones Aplicadas", Icono = "ph-check-circle", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?estado=Reparado&partial=true" }
                }
            },

            // 8. TESORERÍA
            new PerfilRolConfig
            {
                Slug = "tesoreria",
                NombreRol = "Tesorería",
                Subtitulo = "Recaudación, Vouchers y Catálogo TUPA",
                RutaDashboard = "/Dashboard/Tesoreria",
                IconoRol = "ph-bank",
                ColorPrimario = "#3b82f6",
                ColorSecundario = "#2563eb",
                UsuarioDemoNombre = "CPC. Martín Vargas (Tesorería)",
                UsuarioDemoDni = "44556677",
                UsuarioDemoCorreo = "tesoreria@istpargentina.edu.pe",
                UsuarioDemoId = 2,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "personas", "usuarios", "roles", "requerimientos" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "inicio-tesoreria", Seccion = "RECAUDACIÓN INSTITUCIONAL", Titulo = "Inicio", Icono = "ph-house", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "bandeja-vouchers", Seccion = "RECAUDACIÓN INSTITUCIONAL", Titulo = "Bandeja de vouchers", Icono = "ph-receipt", VistaInterna = "tesoreria_vouchers", BadgeTexto = "1", BadgeColor = "#ef4444" },
                    new() { Id = "recibos", Seccion = "RECAUDACIÓN INSTITUCIONAL", Titulo = "Recibos", Icono = "ph-file-C-sharp", VistaInterna = "tesoreria_recibos" },
                    new() { Id = "matriculas-tes", Seccion = "RECAUDACIÓN INSTITUCIONAL", Titulo = "Matrículas", Icono = "ph-Identification-badge", VistaInterna = "tesoreria_matriculas" },
                    new() { Id = "estudiantes-tes", Seccion = "DIRECTORIO Y ATENCIÓN", Titulo = "Estudiantes", Icono = "ph-users", VistaInterna = "padron_personas" },
                    new() { Id = "buzon-tes", Seccion = "DIRECTORIO Y ATENCIÓN", Titulo = "Buzón", Icono = "ph-tray", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true" },
                    new() { Id = "whatsapp-tes", Seccion = "DIRECTORIO Y ATENCIÓN", Titulo = "WhatsApp", Icono = "ph-whatsapp-logo", VistaInterna = "tesoreria_whatsapp" },
                    new() { Id = "catalogo-tupa", Seccion = "NORMATIVA", Titulo = "Catálogo TUPA", Icono = "ph-book-bookmark", VistaInterna = "tesoreria_tupa" }
                }
            },

            // 9. ADMINISTRADOR
            new PerfilRolConfig
            {
                Slug = "administrador",
                NombreRol = "Administrador",
                Subtitulo = "Seguridad, Usuarios, Roles y Auditoría",
                RutaDashboard = "/Dashboard/Administrador",
                IconoRol = "ph-shield-star",
                ColorPrimario = "#f43f5e",
                ColorSecundario = "#e11d48",
                UsuarioDemoNombre = "Ing. Jherson Arias (Admin General)",
                UsuarioDemoDni = "73928172",
                UsuarioDemoCorreo = "jherson.ariasv@istpargentina.edu.pe",
                UsuarioDemoId = 4,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "usuarios", "roles", "usuarios_roles", "permisos_modulos", "roles_permisos", "sesiones_login" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "dashboard-central", Seccion = "ADMINISTRACIÓN CENTRAL", Titulo = "Dashboard Central", Icono = "ph-gauge", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "gestion-usuarios", Seccion = "CONTROL DE ACCESOS", Titulo = "Gestión de Usuarios", Icono = "ph-users-four", VistaInterna = "tabla_usuarios" },
                    new() { Id = "roles-perfiles", Seccion = "CONTROL DE ACCESOS", Titulo = "Roles y Perfiles", Icono = "ph-user-switch", VistaInterna = "tabla_roles" },
                    new() { Id = "permisos-modulos", Seccion = "CONTROL DE ACCESOS", Titulo = "Permisos y Módulos", Icono = "ph-key", VistaInterna = "tabla_permisos" },
                    new() { Id = "auditoria-sesiones", Seccion = "SEGURIDAD E INTEGRACIÓN", Titulo = "Auditoría de Sesiones", Icono = "ph-fingerprint", VistaInterna = "tabla_sesiones" },
                    new() { Id = "configuracion-global", Seccion = "SEGURIDAD E INTEGRACIÓN", Titulo = "Configuración Global (Google/BD)", Icono = "ph-gear-six", VistaInterna = "configuracion_sistema" }
                }
            },

            // 10. JEFE GENERAL
            new PerfilRolConfig
            {
                Slug = "jefegeneral",
                NombreRol = "Jefe General",
                Subtitulo = "Dirección General de Patrimonio y Operaciones",
                RutaDashboard = "/Dashboard/JefeGeneral",
                IconoRol = "ph-crown",
                ColorPrimario = "#ef4444",
                ColorSecundario = "#dc2626",
                UsuarioDemoNombre = "Dr. Roberto Alcántara (Jefe General)",
                UsuarioDemoDni = "73928172",
                UsuarioDemoCorreo = "jefatura.general@istpargentina.edu.pe",
                UsuarioDemoId = 4,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "especialidad", "asignacion", "transferencia", "inventario_anual", "usuarios" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "jg-dashboard", Seccion = "GENERAL", Titulo = "Dashboard", Icono = "ph-chart-line-up", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "jg-bienes", Seccion = "INVENTARIO", Titulo = "Bienes Patrimoniales", Icono = "ph-cube", UrlModulo = "/Modulo05/Soporte/HistorialEquipos?partial=true" },
                    new() { Id = "jg-almacenes", Seccion = "INVENTARIO", Titulo = "Almacenes", Icono = "ph-warehouse", VistaInterna = "tabla_almacenes" },
                    new() { Id = "jg-ambientes", Seccion = "INVENTARIO", Titulo = "Ambientes", Icono = "ph-buildings", VistaInterna = "tabla_ambientes" },
                    new() { Id = "jg-especialidades", Seccion = "INVENTARIO", Titulo = "Especialidades", Icono = "ph-graduation-cap", VistaInterna = "tabla_especialidades" },
                    new() { Id = "jg-asignaciones", Seccion = "OPERACIONES", Titulo = "Asignaciones", Icono = "ph-arrows-left-right", VistaInterna = "tabla_asignaciones" },
                    new() { Id = "jg-transferencias", Seccion = "OPERACIONES", Titulo = "Transferencias", Icono = "ph-swap", VistaInterna = "tabla_transferencias" },
                    new() { Id = "jg-inv-anual", Seccion = "OPERACIONES", Titulo = "Inventario Anual", Icono = "ph-calendar-check", VistaInterna = "tabla_inventario_anual" },
                    new() { Id = "jg-incidencias", Seccion = "MÓDULO 05", Titulo = "Consola de Incidencias", Icono = "ph-wrench", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?partial=true" },
                    new() { Id = "jg-requerimientos", Seccion = "MÓDULO 05", Titulo = "Bandeja de Requerimientos", Icono = "ph-clipboard-text", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true" },
                    new() { Id = "jg-usuarios", Seccion = "ADMINISTRACIÓN", Titulo = "Usuarios", Icono = "ph-users", VistaInterna = "tabla_usuarios" }
                }
            },

            // 11. JEFE TALLER COMPUTACIÓN
            new PerfilRolConfig
            {
                Slug = "jefecomputacion",
                NombreRol = "Jefe Taller Computación",
                Subtitulo = "Área de Computación e Informática",
                RutaDashboard = "/Dashboard/JefeComputacion",
                IconoRol = "ph-desktop-tower",
                ColorPrimario = "#0ea5e9",
                ColorSecundario = "#0284c7",
                UsuarioDemoNombre = "Ing. Marcos Salazar (Jefe Computación)",
                UsuarioDemoDni = "60914536",
                UsuarioDemoCorreo = "computacion@istpargentina.edu.pe",
                UsuarioDemoId = 5,
                EspecialidadFiltro = "Computación",
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "especialidad", "asignacion", "transferencia", "inventario_anual" },
                OpcionesSidebar = CrearOpcionesJefeTaller("Computación e Informática")
            },

            // 12. JEFE TALLER ADMINISTRACIÓN
            new PerfilRolConfig
            {
                Slug = "jefeadministracion",
                NombreRol = "Jefe Taller Administración",
                Subtitulo = "Área de Administración de Empresas",
                RutaDashboard = "/Dashboard/JefeAdministracion",
                IconoRol = "ph-chart-bar",
                ColorPrimario = "#10b981",
                ColorSecundario = "#059669",
                UsuarioDemoNombre = "Lic. Sofía Medina (Jefe Administración)",
                UsuarioDemoDni = "63229403",
                UsuarioDemoCorreo = "administracion@istpargentina.edu.pe",
                UsuarioDemoId = 7,
                EspecialidadFiltro = "Administración",
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "especialidad", "asignacion", "transferencia", "inventario_anual" },
                OpcionesSidebar = CrearOpcionesJefeTaller("Administración de Empresas")
            },

            // 13. JEFE TALLER CONTABILIDAD
            new PerfilRolConfig
            {
                Slug = "jefecontabilidad",
                NombreRol = "Jefe Taller Contabilidad",
                Subtitulo = "Área de Contabilidad",
                RutaDashboard = "/Dashboard/JefeContabilidad",
                IconoRol = "ph-calculator",
                ColorPrimario = "#f59e0b",
                ColorSecundario = "#d97706",
                UsuarioDemoNombre = "CPC. Jorge Paredes (Jefe Contabilidad)",
                UsuarioDemoDni = "74114320",
                UsuarioDemoCorreo = "contabilidad@istpargentina.edu.pe",
                UsuarioDemoId = 6,
                EspecialidadFiltro = "Contabilidad",
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "especialidad", "asignacion", "transferencia", "inventario_anual" },
                OpcionesSidebar = CrearOpcionesJefeTaller("Contabilidad")
            },

            // 14. ENCARGADO DE INVENTARIO
            new PerfilRolConfig
            {
                Slug = "encargadoinventario",
                NombreRol = "Encargado de Inventario",
                Subtitulo = "Gestión General de Almacenes y Patrimonio",
                RutaDashboard = "/Dashboard/EncargadoInventario",
                IconoRol = "ph-package",
                ColorPrimario = "#a855f7",
                ColorSecundario = "#7c3aed",
                UsuarioDemoNombre = "Lic. Héctor Campos (Patrimonio)",
                UsuarioDemoDni = "72910115",
                UsuarioDemoCorreo = "inventario@istpargentina.edu.pe",
                UsuarioDemoId = 8,
                EsDestacadoDemo = false,
                TablasRelacionadas = new[] { "bien", "almacen", "ambiente", "especialidad", "asignacion", "transferencia", "inventario_anual" },
                OpcionesSidebar = new List<OpcionSidebar>
                {
                    new() { Id = "inv-dashboard", Seccion = "GENERAL", Titulo = "Dashboard Inventario", Icono = "ph-package", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                    new() { Id = "inv-bienes", Seccion = "INVENTARIO", Titulo = "Bienes y Equipos", Icono = "ph-hard-drives", UrlModulo = "/Modulo05/Soporte/HistorialEquipos?partial=true" },
                    new() { Id = "inv-almacenes", Seccion = "INVENTARIO", Titulo = "Almacenes", Icono = "ph-warehouse", VistaInterna = "tabla_almacenes" },
                    new() { Id = "inv-ambientes", Seccion = "INVENTARIO", Titulo = "Ambientes", Icono = "ph-buildings", VistaInterna = "tabla_ambientes" },
                    new() { Id = "inv-especialidades", Seccion = "INVENTARIO", Titulo = "Especialidades", Icono = "ph-graduation-cap", VistaInterna = "tabla_especialidades" },
                    new() { Id = "inv-asignaciones", Seccion = "OPERACIONES", Titulo = "Asignaciones", Icono = "ph-arrows-left-right", VistaInterna = "tabla_asignaciones" },
                    new() { Id = "inv-transferencias", Seccion = "OPERACIONES", Titulo = "Transferencias", Icono = "ph-swap", VistaInterna = "tabla_transferencias" },
                    new() { Id = "inv-anual", Seccion = "OPERACIONES", Titulo = "Inventario Anual", Icono = "ph-calendar-check", VistaInterna = "tabla_inventario_anual" },
                    new() { Id = "inv-incidencias", Seccion = "MÓDULO 05", Titulo = "Incidencias de Equipos", Icono = "ph-wrench", UrlModulo = "/Modulo05/Soporte/ConsolaPrincipal?partial=true" }
                }
            }
        };

        private static List<OpcionSidebar> CrearOpcionesJefeTaller(string areaNombre)
        {
            return new List<OpcionSidebar>
            {
                new() { Id = "jt-dashboard", Seccion = $"JEFATURA: {areaNombre.ToUpperInvariant()}", Titulo = "Dashboard de Área", Icono = "ph-squares-four", VistaInterna = "resumen_rol", EsActivoPorDefecto = true },
                new() { Id = "jt-bienes", Seccion = "INVENTARIO DE ESPECIALIDAD", Titulo = "Bienes y Equipos", Icono = "ph-desktop", UrlModulo = "/Modulo05/Soporte/HistorialEquipos?partial=true" },
                new() { Id = "jt-almacenes", Seccion = "INVENTARIO DE ESPECIALIDAD", Titulo = "Almacenes", Icono = "ph-warehouse", VistaInterna = "tabla_almacenes" },
                new() { Id = "jt-ambientes", Seccion = "INVENTARIO DE ESPECIALIDAD", Titulo = "Ambientes y Talleres", Icono = "ph-buildings", VistaInterna = "tabla_ambientes" },
                new() { Id = "jt-asignaciones", Seccion = "OPERACIONES", Titulo = "Asignaciones", Icono = "ph-arrows-left-right", VistaInterna = "tabla_asignaciones" },
                new() { Id = "jt-transferencias", Seccion = "OPERACIONES", Titulo = "Transferencias", Icono = "ph-swap", VistaInterna = "tabla_transferencias" },
                new() { Id = "jt-inv-anual", Seccion = "OPERACIONES", Titulo = "Inventario Anual", Icono = "ph-calendar-check", VistaInterna = "tabla_inventario_anual" },
                new() { Id = "jt-averias", Seccion = "MÓDULO 05 (INCIDENCIAS)", Titulo = "Reportar Avería de Taller", Icono = "ph-warning-octagon", UrlModulo = "/Modulo05/AveriaDocente/Reporte?partial=true" },
                new() { Id = "jt-requerimientos", Seccion = "MÓDULO 05 (INCIDENCIAS)", Titulo = "Requerimientos del Área", Icono = "ph-clipboard-text", UrlModulo = "/Modulo05/Requerimientos/Bandeja?partial=true" }
            };
        }

        public static IReadOnlyList<PerfilRolConfig> ObtenerTodosLosRoles() => _roles;

        public static PerfilRolConfig ObtenerPorRolOSlug(string? identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
                return _roles[0];

            string clean = identificador.Trim();

            var exactSlug = _roles.FirstOrDefault(r => r.Slug.Equals(clean, StringComparison.OrdinalIgnoreCase));
            if (exactSlug != null) return exactSlug;

            var exactName = _roles.FirstOrDefault(r => r.NombreRol.Equals(clean, StringComparison.OrdinalIgnoreCase));
            if (exactName != null) return exactName;

            if (clean.Contains("Alumno", StringComparison.OrdinalIgnoreCase) || clean.Contains("Estudiante", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "alumno");
            if (clean.Contains("Docente", StringComparison.OrdinalIgnoreCase) || clean.Contains("Profesor", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "docente");
            if (clean.Contains("Psicolog", StringComparison.OrdinalIgnoreCase) || clean.Contains("Psicología", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "psicologia");
            if (clean.Contains("Tópico", StringComparison.OrdinalIgnoreCase) || clean.Contains("Topico", StringComparison.OrdinalIgnoreCase) || clean.Contains("Enfermer", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "topico");
            if (clean.Contains("Practicante", StringComparison.OrdinalIgnoreCase) || clean.Contains("Auxiliar", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "practicante");
            if (clean.Contains("Soporte", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "soporte");
            if (clean.Contains("Secretar", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "secretaria");
            if (clean.Contains("Tesorer", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "tesoreria");
            if (clean.Contains("Computac", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "jefecomputacion");
            if (clean.Contains("Jefe Admin", StringComparison.OrdinalIgnoreCase) || clean.Contains("Taller Admin", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "jefeadministracion");
            if (clean.Contains("Contabil", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "jefecontabilidad");
            if (clean.Contains("Inventario", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "encargadoinventario");
            if (clean.Contains("General", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "jefegeneral");
            if (clean.Contains("Admin", StringComparison.OrdinalIgnoreCase))
                return _roles.First(r => r.Slug == "administrador");

            return _roles[0];
        }
    }
}
