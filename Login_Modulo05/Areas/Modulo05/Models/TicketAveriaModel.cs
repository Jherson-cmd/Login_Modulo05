using System;
using System.Collections.Generic;
using Npgsql;

namespace Login_Administracion_de_Modulos.Areas.Modulo05.Models
{
    public class TicketAveriaItem
    {
        public int IdIncidencia { get; set; }
        public int IdBien { get; set; }
        public int ReportadorId { get; set; }
        public int? TecnicoId { get; set; }
        public DateTime FechaReporte { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }
        public string TipoFalla { get; set; } = string.Empty;
        public string Gravedad { get; set; } = "Media";
        public string Descripcion { get; set; } = string.Empty;
        public string? SolucionAplicada { get; set; }
        public string Estado { get; set; } = "Abierta";

        // Información relacional del bien y ambiente
        public string? CodigoBien { get; set; }
        public string? DescripcionBien { get; set; }
        public string? MarcaModelo { get; set; }
        public string? Ambiente { get; set; }
        public string? NombreReportador { get; set; }
        public string? NombreTecnico { get; set; }
    }

    public class TicketAveriaRepository
    {
        private readonly string _connectionString;

        public TicketAveriaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string NormalizarEstadoIncidencia(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "Abierta";
            string s = estado.Trim();
            if (s.Contains("Diagn", StringComparison.OrdinalIgnoreCase) || s.Contains("Atenci", StringComparison.OrdinalIgnoreCase) || s.Contains("Proceso", StringComparison.OrdinalIgnoreCase))
                return "En diagnóstico";
            if (s.Contains("Repuesto", StringComparison.OrdinalIgnoreCase) || s.Contains("Garant", StringComparison.OrdinalIgnoreCase))
                return "En espera de repuesto";
            if (s.Contains("Reparado", StringComparison.OrdinalIgnoreCase) || s.Contains("Resuelto", StringComparison.OrdinalIgnoreCase) || s.Contains("Cerrado", StringComparison.OrdinalIgnoreCase))
                return "Reparado";
            if (s.Contains("Baja", StringComparison.OrdinalIgnoreCase))
                return "Baja Definitiva";
            return "Abierta";
        }

        public static string NormalizarGravedad(string? gravedad)
        {
            if (string.IsNullOrWhiteSpace(gravedad)) return "Media";
            string g = gravedad.Trim();
            if (g.StartsWith("Baja", StringComparison.OrdinalIgnoreCase)) return "Baja";
            if (g.StartsWith("Alta", StringComparison.OrdinalIgnoreCase)) return "Alta";
            if (g.StartsWith("Cr", StringComparison.OrdinalIgnoreCase)) return "Crítica";
            return "Media";
        }

        public bool CrearReporte(TicketAveriaItem item)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            int idBienValido = item.IdBien > 0 ? item.IdBien : 1;
            using (var cmdCheckBien = new NpgsqlCommand("SELECT id_bien FROM bien WHERE id_bien = @id LIMIT 1", conn))
            {
                cmdCheckBien.Parameters.AddWithValue("@id", idBienValido);
                var res = cmdCheckBien.ExecuteScalar();
                if (res == null)
                {
                    var fallbackBien = new NpgsqlCommand("SELECT id_bien FROM bien ORDER BY id_bien LIMIT 1", conn).ExecuteScalar();
                    idBienValido = fallbackBien != null ? Convert.ToInt32(fallbackBien) : 1;
                }
            }

            int reportadorValido = item.ReportadorId > 0 ? item.ReportadorId : 1;
            using (var cmdCheckUser = new NpgsqlCommand("SELECT id_usuario FROM usuarios WHERE id_usuario = @id LIMIT 1", conn))
            {
                cmdCheckUser.Parameters.AddWithValue("@id", reportadorValido);
                var res = cmdCheckUser.ExecuteScalar();
                if (res == null)
                {
                    var fallbackUser = new NpgsqlCommand("SELECT id_usuario FROM usuarios ORDER BY id_usuario LIMIT 1", conn).ExecuteScalar();
                    reportadorValido = fallbackUser != null ? Convert.ToInt32(fallbackUser) : 1;
                }
            }

            const string sql = @"
                INSERT INTO incidencias 
                (id_bien, reportador_id, fecha_reporte, tipo_falla, gravedad, descripcion, estado)
                VALUES (@id_bien, @reportador_id, @fecha_reporte, @tipo_falla, @gravedad, @descripcion, @estado)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id_bien", idBienValido);
            cmd.Parameters.AddWithValue("@reportador_id", reportadorValido);
            cmd.Parameters.AddWithValue("@fecha_reporte", item.FechaReporte);
            cmd.Parameters.AddWithValue("@tipo_falla", item.TipoFalla ?? "Falla General");
            cmd.Parameters.AddWithValue("@gravedad", NormalizarGravedad(item.Gravedad));
            cmd.Parameters.AddWithValue("@descripcion", item.Descripcion ?? "");
            cmd.Parameters.AddWithValue("@estado", NormalizarEstadoIncidencia(item.Estado));

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<TicketAveriaItem> ObtenerReportesPorDocente(int docenteId)
        {
            var lista = new List<TicketAveriaItem>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            const string sql = @"
                SELECT i.id_incidencia, i.id_bien, i.reportador_id, i.tecnico_id, i.fecha_reporte, 
                       i.fecha_cierre, i.tipo_falla, i.gravedad, i.descripcion, i.solucion_aplicada, i.estado,
                       b.cbi, b.descripcion AS bien_desc, b.marca, b.modelo
                FROM incidencias i
                LEFT JOIN bien b ON i.id_bien = b.id_bien
                ORDER BY i.fecha_reporte DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapReader(reader));
            }
            return lista;
        }

        public List<TicketAveriaItem> ObtenerTodosTickets(string? filtroEstado = null)
        {
            var lista = new List<TicketAveriaItem>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string sql = @"
                SELECT i.id_incidencia, i.id_bien, i.reportador_id, i.tecnico_id, i.fecha_reporte, 
                       i.fecha_cierre, i.tipo_falla, i.gravedad, i.descripcion, i.solucion_aplicada, i.estado,
                       b.cbi, b.descripcion AS bien_desc, b.marca, b.modelo
                FROM incidencias i
                LEFT JOIN bien b ON i.id_bien = b.id_bien";

            string? estadoNorm = null;
            if (!string.IsNullOrEmpty(filtroEstado))
            {
                estadoNorm = NormalizarEstadoIncidencia(filtroEstado);
                sql += " WHERE i.estado = @estado";
            }
            sql += " ORDER BY i.fecha_reporte DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            if (!string.IsNullOrEmpty(estadoNorm))
            {
                cmd.Parameters.AddWithValue("@estado", estadoNorm);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapReader(reader));
            }
            return lista;
        }

        public TicketAveriaItem? ObtenerTicketPorId(int idIncidencia)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            const string sql = @"
                SELECT i.id_incidencia, i.id_bien, i.reportador_id, i.tecnico_id, i.fecha_reporte, 
                       i.fecha_cierre, i.tipo_falla, i.gravedad, i.descripcion, i.solucion_aplicada, i.estado,
                       b.cbi, b.descripcion AS bien_desc, b.marca, b.modelo
                FROM incidencias i
                LEFT JOIN bien b ON i.id_bien = b.id_bien
                WHERE i.id_incidencia = @id LIMIT 1";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idIncidencia);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapReader(reader);
            }
            return null;
        }

        public bool ActualizarEstadoTicket(int idIncidencia, string estado, string? solucionAplicada, int tecnicoId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string estadoNorm = NormalizarEstadoIncidencia(estado);

            int tecnicoValido = tecnicoId > 0 ? tecnicoId : 1;
            using (var cmdCheckUser = new NpgsqlCommand("SELECT id_usuario FROM usuarios WHERE id_usuario = @id LIMIT 1", conn))
            {
                cmdCheckUser.Parameters.AddWithValue("@id", tecnicoValido);
                if (cmdCheckUser.ExecuteScalar() == null)
                {
                    var fb = new NpgsqlCommand("SELECT id_usuario FROM usuarios ORDER BY id_usuario LIMIT 1", conn).ExecuteScalar();
                    tecnicoValido = fb != null ? Convert.ToInt32(fb) : 1;
                }
            }

            const string sql = @"
                UPDATE incidencias 
                SET estado = @estado, 
                    solucion_aplicada = @solucion, 
                    tecnico_id = @tecnico_id,
                    fecha_cierre = CASE WHEN @estado IN ('Reparado', 'Baja Definitiva') THEN NOW() ELSE fecha_cierre END
                WHERE id_incidencia = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idIncidencia);
            cmd.Parameters.AddWithValue("@estado", estadoNorm);
            cmd.Parameters.AddWithValue("@solucion", (object?)solucionAplicada ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tecnico_id", tecnicoValido);

            return cmd.ExecuteNonQuery() > 0;
        }

        private static TicketAveriaItem MapReader(NpgsqlDataReader reader)
        {
            return new TicketAveriaItem
            {
                IdIncidencia = reader.GetInt32(0),
                IdBien = reader.GetInt32(1),
                ReportadorId = reader.GetInt32(2),
                TecnicoId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                FechaReporte = reader.GetDateTime(4),
                FechaCierre = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                TipoFalla = reader.IsDBNull(6) ? "" : reader.GetString(6),
                Gravedad = reader.IsDBNull(7) ? "Media" : reader.GetString(7),
                Descripcion = reader.IsDBNull(8) ? "" : reader.GetString(8),
                SolucionAplicada = reader.IsDBNull(9) ? null : reader.GetString(9),
                Estado = reader.IsDBNull(10) ? "Abierta" : reader.GetString(10),
                CodigoBien = reader.IsDBNull(11) ? "S/C" : reader.GetString(11),
                DescripcionBien = reader.IsDBNull(12) ? "Equipo General" : reader.GetString(12),
                MarcaModelo = $"{(reader.IsDBNull(13) ? "" : reader.GetString(13))} {(reader.IsDBNull(14) ? "" : reader.GetString(14))}".Trim()
            };
        }
    }
}
