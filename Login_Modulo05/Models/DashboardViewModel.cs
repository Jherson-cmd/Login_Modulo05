using System;
using System.Collections.Generic;
using Login_Administracion_de_Modulos.Seguridad;

namespace Login_Administracion_de_Modulos.Models
{
    public class MetricaCardDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Valor { get; set; } = "0";
        public string Subtitulo { get; set; } = string.Empty;
        public string Icono { get; set; } = "ph-chart-bar";
        public string Color { get; set; } = "#00f2fe";
    }

    public class TablaDinamicaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string Icono { get; set; } = "ph-table";
        public List<string> Columnas { get; set; } = new();
        public List<List<string>> Filas { get; set; } = new();
        public string? BotonAccionTexto { get; set; }
        public string? BotonAccionUrl { get; set; }
    }

    public class DashboardViewModel
    {
        public PerfilRolConfig PerfilRol { get; set; } = new();
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioDni { get; set; } = string.Empty;
        public string UsuarioCorreo { get; set; } = string.Empty;
        public string InicialesAvatar { get; set; } = "UI";
        public string ModoConexion { get; set; } = "PostgreSQL Online";
        public bool BaseDatosConectada { get; set; } = true;
        public List<MetricaCardDto> Metricas { get; set; } = new();
        public TablaDinamicaDto TablaPrincipal { get; set; } = new();
        public IReadOnlyList<PerfilRolConfig> TodosLosRoles { get; set; } = Array.Empty<PerfilRolConfig>();
    }
}
