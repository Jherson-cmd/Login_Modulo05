using System;
using Npgsql;

namespace Login_Administracion_de_Modulos.Seguridad
{
    /// <summary>
    /// Verifica y asegura que las tablas operativas de PostgreSQL tengan datos semilla válidos
    /// respetando todas las restricciones CHECK y llaves foráneas de bd.sql / ruben.sql.
    /// </summary>
    public static class InicializadorBaseDatos
    {
        private static bool _inicializado = false;

        public static void AsegurarDatosSemilla(string connectionString)
        {
            if (_inicializado) return;
            _inicializado = true;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // 1. Especialidades
            const string sqlEsp = @"
                INSERT INTO especialidad (nombre, estado)
                SELECT 'Computación e Informática', 'Activo'::estado_general
                WHERE NOT EXISTS (SELECT 1 FROM especialidad);
                INSERT INTO especialidad (nombre, estado)
                SELECT 'Contabilidad', 'Activo'::estado_general
                WHERE (SELECT COUNT(*) FROM especialidad) < 2;
                INSERT INTO especialidad (nombre, estado)
                SELECT 'Administración de Empresas', 'Activo'::estado_general
                WHERE (SELECT COUNT(*) FROM especialidad) < 3;";
            using (var cmd = new NpgsqlCommand(sqlEsp, conn)) { cmd.ExecuteNonQuery(); }

            int idEsp = Convert.ToInt32(new NpgsqlCommand("SELECT id_especialidad FROM especialidad ORDER BY id_especialidad LIMIT 1", conn).ExecuteScalar() ?? 1);

            // 2. Almacenes
            string sqlAlm = $@"
                INSERT INTO almacen (id_especialidad, nombre, ubicacion, responsable, estado)
                SELECT {idEsp}, 'Almacén Central de TI y Redes', 'Pabellón A - Piso 1', 'Ing. Paulo Carreón', 'Activo'::estado_general
                WHERE NOT EXISTS (SELECT 1 FROM almacen);";
            using (var cmd = new NpgsqlCommand(sqlAlm, conn)) { cmd.ExecuteNonQuery(); }

            int idAlm = Convert.ToInt32(new NpgsqlCommand("SELECT id_almacen FROM almacen ORDER BY id_almacen LIMIT 1", conn).ExecuteScalar() ?? 1);

            // 3. Ambientes
            string sqlAmb = $@"
                INSERT INTO ambiente (id_especialidad, nombre, estado)
                SELECT {idEsp}, 'Laboratorio de Cómputo 01', 'Activo'::estado_general
                WHERE NOT EXISTS (SELECT 1 FROM ambiente);
                INSERT INTO ambiente (id_especialidad, nombre, estado)
                SELECT {idEsp}, 'Laboratorio de Redes y Conectividad', 'Activo'::estado_general
                WHERE (SELECT COUNT(*) FROM ambiente) < 2;
                INSERT INTO ambiente (id_especialidad, nombre, estado)
                SELECT {idEsp}, 'Aula 201 - Pabellón A', 'Activo'::estado_general
                WHERE (SELECT COUNT(*) FROM ambiente) < 3;";
            using (var cmd = new NpgsqlCommand(sqlAmb, conn)) { cmd.ExecuteNonQuery(); }

            int idAmb = Convert.ToInt32(new NpgsqlCommand("SELECT id_ambiente FROM ambiente ORDER BY id_ambiente LIMIT 1", conn).ExecuteScalar() ?? 1);

            // 4. Bienes (Activos de Hardware)
            string sqlBien = $@"
                INSERT INTO bien (id_almacen, cbi, descripcion, tipo_bien, marca, modelo, serie, procedencia, estado, observacion)
                SELECT {idAlm}, 'CBI-2024-001', 'Estación de Trabajo Core i7 16GB RAM', 'Computadora', 'Dell', 'OptiPlex 7090', 'SN-DELL-7090-01', 'MINEDU', 'Bueno'::estado_bien, 'Equipo principal de laboratorio'
                WHERE NOT EXISTS (SELECT 1 FROM bien WHERE cbi = 'CBI-2024-001');
                INSERT INTO bien (id_almacen, cbi, descripcion, tipo_bien, marca, modelo, serie, procedencia, estado, observacion)
                SELECT {idAlm}, 'CBI-2024-002', 'Proyector Multimedia Láser Full HD', 'Proyector', 'Epson', 'PowerLite L200W', 'SN-EPSON-L200-02', 'Compra Institucional', 'Bueno'::estado_bien, 'Instalado en techo Lab 01'
                WHERE NOT EXISTS (SELECT 1 FROM bien WHERE cbi = 'CBI-2024-002');
                INSERT INTO bien (id_almacen, cbi, descripcion, tipo_bien, marca, modelo, serie, procedencia, estado, observacion)
                SELECT {idAlm}, 'CBI-2024-003', 'Switch Administrable 24 Puertos Gigabit', 'Red', 'Cisco', 'Catalyst 2960X', 'SN-CISCO-2960-03', 'MINEDU', 'Regular'::estado_bien, 'Gabinete de comunicaciones'
                WHERE NOT EXISTS (SELECT 1 FROM bien WHERE cbi = 'CBI-2024-003');";
            using (var cmd = new NpgsqlCommand(sqlBien, conn)) { cmd.ExecuteNonQuery(); }

            int idBien = Convert.ToInt32(new NpgsqlCommand("SELECT id_bien FROM bien ORDER BY id_bien LIMIT 1", conn).ExecuteScalar() ?? 1);

            // 5. Botica Medicamentos (cumpliendo CHECK estado IN ('Disponible','Por vencer','Agotado'))
            const string sqlMed = @"
                INSERT INTO botica_medicamentos (nombre, presentacion, stock_actual, fecha_vencimiento, estado)
                SELECT 'Paracetamol 500mg', 'Tabletas x 100 unids', 85, CURRENT_DATE + INTERVAL '365 days', 'Disponible'
                WHERE NOT EXISTS (SELECT 1 FROM botica_medicamentos);
                INSERT INTO botica_medicamentos (nombre, presentacion, stock_actual, fecha_vencimiento, estado)
                SELECT 'Ibuprofeno 400mg', 'Blíster x 10 tabletas', 42, CURRENT_DATE + INTERVAL '240 days', 'Disponible'
                WHERE (SELECT COUNT(*) FROM botica_medicamentos) < 2;
                INSERT INTO botica_medicamentos (nombre, presentacion, stock_actual, fecha_vencimiento, estado)
                SELECT 'Alcohol Medicinal 70°', 'Frasco x 1000 ml', 12, CURRENT_DATE + INTERVAL '180 days', 'Disponible'
                WHERE (SELECT COUNT(*) FROM botica_medicamentos) < 3;
                INSERT INTO botica_medicamentos (nombre, presentacion, stock_actual, fecha_vencimiento, estado)
                SELECT 'Loratadina 10mg (Antihistamínico)', 'Caja x 20 tabletas', 4, CURRENT_DATE + INTERVAL '45 days', 'Por vencer'
                WHERE (SELECT COUNT(*) FROM botica_medicamentos) < 4;";
            using (var cmd = new NpgsqlCommand(sqlMed, conn)) { cmd.ExecuteNonQuery(); }

            // 6. Incidencia inicial si está vacía
            int idUsuario = Convert.ToInt32(new NpgsqlCommand("SELECT id_usuario FROM usuarios ORDER BY id_usuario LIMIT 1", conn).ExecuteScalar() ?? 1);
            string sqlInc = $@"
                INSERT INTO incidencias (id_bien, reportador_id, tecnico_id, tipo_falla, gravedad, descripcion, estado)
                SELECT {idBien}, {idUsuario}, {idUsuario}, 'Proyector Multimedia sin Imagen/Señal', 'Alta', '[Ambiente/Aula: Laboratorio de Cómputo 01] No detecta señal HDMI desde la PC del docente.', 'Abierta'
                WHERE NOT EXISTS (SELECT 1 FROM incidencias);";
            using (var cmd = new NpgsqlCommand(sqlInc, conn)) { cmd.ExecuteNonQuery(); }

            // 7. Requerimiento inicial si está vacío
            string sqlReq = $@"
                INSERT INTO requerimientos (solicitante_id, tecnico_id, id_ambiente, tipo_servicio, prioridad, descripcion, estado)
                SELECT {idUsuario}, {idUsuario}, {idAmb}, 'Instalación de Software Académico', 'Alta', 'Instalación de Visual Studio 2022, .NET SDK y PostgreSQL en las 25 estaciones del Laboratorio 01.', 'Pendiente'
                WHERE NOT EXISTS (SELECT 1 FROM requerimientos);";
            using (var cmd = new NpgsqlCommand(sqlReq, conn)) { cmd.ExecuteNonQuery(); }
        }
    }
}
