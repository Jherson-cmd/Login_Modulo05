using System;
using System.Collections.Generic;
using Npgsql;

namespace Login_Administracion_de_Modulos.Areas.Modulo05.Models
{
    public class RequerimientoItem
    {
        public int IdRequerimiento { get; set; }
        public int SolicitanteId { get; set; }
        public int? TecnicoId { get; set; }
        public int? IdAmbiente { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public DateTime? FechaAtencion { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string? ObservacionResolucion { get; set; }

        public string? SolicitanteNombre { get; set; }
        public string? AmbienteNombre { get; set; }
    }

    public class RequerimientoRepository
    {
        private readonly string _connectionString;

        public RequerimientoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string NormalizarPrioridad(string? p)
        {
            if (string.IsNullOrWhiteSpace(p)) return "Media";
            if (p.StartsWith("Baja", StringComparison.OrdinalIgnoreCase)) return "Baja";
            if (p.StartsWith("Alta", StringComparison.OrdinalIgnoreCase)) return "Alta";
            if (p.StartsWith("Urgente", StringComparison.OrdinalIgnoreCase)) return "Urgente";
            return "Media";
        }

        public static string NormalizarEstado(string? e)
        {
            if (string.IsNullOrWhiteSpace(e)) return "Pendiente";
            if (e.Contains("revisi", StringComparison.OrdinalIgnoreCase)) return "En revisión";
            if (e.Contains("Aprobado", StringComparison.OrdinalIgnoreCase)) return "Aprobado";
            if (e.Contains("Atendido", StringComparison.OrdinalIgnoreCase)) return "Atendido";
            if (e.Contains("Rechazado", StringComparison.OrdinalIgnoreCase)) return "Rechazado";
            return "Pendiente";
        }

        public bool CrearRequerimiento(RequerimientoItem item)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            int solicitanteValido = item.SolicitanteId > 0 ? item.SolicitanteId : 1;
            using (var cmdU = new NpgsqlCommand("SELECT id_usuario FROM usuarios WHERE id_usuario = @id LIMIT 1", conn))
            {
                cmdU.Parameters.AddWithValue("@id", solicitanteValido);
                if (cmdU.ExecuteScalar() == null)
                {
                    var fb = new NpgsqlCommand("SELECT id_usuario FROM usuarios ORDER BY id_usuario LIMIT 1", conn).ExecuteScalar();
                    solicitanteValido = fb != null ? Convert.ToInt32(fb) : 1;
                }
            }

            int? idAmbValido = item.IdAmbiente;
            if (!idAmbValido.HasValue || idAmbValido.Value <= 0)
            {
                var fbAmb = new NpgsqlCommand("SELECT id_ambiente FROM ambiente ORDER BY id_ambiente LIMIT 1", conn).ExecuteScalar();
                if (fbAmb != null) idAmbValido = Convert.ToInt32(fbAmb);
            }

            const string sql = @"
                INSERT INTO requerimientos
                (solicitante_id, id_ambiente, tipo_servicio, prioridad, descripcion, fecha_solicitud, estado)
                VALUES (@sol, @amb, @tipo, @prio, @desc, CURRENT_TIMESTAMP, @est)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sol", solicitanteValido);
            cmd.Parameters.AddWithValue("@amb", (object?)idAmbValido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tipo", string.IsNullOrWhiteSpace(item.TipoServicio) ? "Soporte Técnico General" : item.TipoServicio.Trim());
            cmd.Parameters.AddWithValue("@prio", NormalizarPrioridad(item.Prioridad));
            cmd.Parameters.AddWithValue("@desc", item.Descripcion ?? "");
            cmd.Parameters.AddWithValue("@est", NormalizarEstado(item.Estado));

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<RequerimientoItem> ListarRequerimientos(string? estadoFiltro = null)
        {
            var lista = new List<RequerimientoItem>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT r.id_requerimiento, r.solicitante_id, r.tecnico_id, r.id_ambiente,
                       r.tipo_servicio, r.prioridad, r.descripcion, r.fecha_solicitud,
                       r.fecha_atencion, r.estado, r.observacion_resolucion,
                       COALESCE(p.nombres || ' ' || p.apellidos, u.correo, 'Usuario Institucional') as solicitante_nombre,
                       COALESCE(a.nombre, 'Oficina / Laboratorio General') as ambiente_nombre
                FROM requerimientos r
                LEFT JOIN usuarios u ON r.solicitante_id = u.id_usuario
                LEFT JOIN personas p ON u.id_persona = p.id_persona
                LEFT JOIN ambiente a ON r.id_ambiente = a.id_ambiente";

            if (!string.IsNullOrWhiteSpace(estadoFiltro))
            {
                sql += " WHERE r.estado = @est";
            }
            sql += " ORDER BY r.fecha_solicitud DESC LIMIT 100";

            using var cmd = new NpgsqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(estadoFiltro))
            {
                cmd.Parameters.AddWithValue("@est", NormalizarEstado(estadoFiltro));
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new RequerimientoItem
                {
                    IdRequerimiento = rd.GetInt32(0),
                    SolicitanteId = rd.GetInt32(1),
                    TecnicoId = rd.IsDBNull(2) ? null : rd.GetInt32(2),
                    IdAmbiente = rd.IsDBNull(3) ? null : rd.GetInt32(3),
                    TipoServicio = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Prioridad = rd.IsDBNull(5) ? "Media" : rd.GetString(5),
                    Descripcion = rd.IsDBNull(6) ? "" : rd.GetString(6),
                    FechaSolicitud = rd.GetDateTime(7),
                    FechaAtencion = rd.IsDBNull(8) ? null : rd.GetDateTime(8),
                    Estado = rd.IsDBNull(9) ? "Pendiente" : rd.GetString(9),
                    ObservacionResolucion = rd.IsDBNull(10) ? null : rd.GetString(10),
                    SolicitanteNombre = rd.IsDBNull(11) ? "Usuario" : rd.GetString(11),
                    AmbienteNombre = rd.IsDBNull(12) ? "General" : rd.GetString(12)
                });
            }
            return lista;
        }

        public bool ActualizarEstado(int idRequerimiento, string nuevoEstado, string? observacion, int tecnicoId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string estNorm = NormalizarEstado(nuevoEstado);
            const string sql = @"
                UPDATE requerimientos
                SET estado = @est,
                    observacion_resolucion = COALESCE(@obs, observacion_resolucion),
                    fecha_atencion = CASE WHEN @est IN ('Atendido', 'Aprobado', 'Rechazado') THEN CURRENT_TIMESTAMP ELSE fecha_atencion END
                WHERE id_requerimiento = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@est", estNorm);
            cmd.Parameters.AddWithValue("@obs", (object?)observacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", idRequerimiento);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
