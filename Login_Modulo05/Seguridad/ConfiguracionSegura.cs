using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Login_Administracion_de_Modulos.Seguridad
{
    /// <summary>
    /// Centraliza la obtención y normalización de la cadena de conexión a PostgreSQL (Supabase)
    /// y las credenciales de Google OAuth / API Key.
    /// </summary>
    public static class ConfiguracionSegura
    {
        public const string DefaultPostgresUri = "postgresql://postgres:gfDnA*xx.B28wdB@db.oxmjxyywxksuwrcgewdg.supabase.co:5432/postgres";

        public static string ObtenerCadenaConexion(IConfiguration? configuration)
        {
            string? rawConn = configuration?.GetConnectionString("PostgreSQLConnection")
                           ?? configuration?.GetConnectionString("DefaultConnection")
                           ?? configuration?.GetConnectionString("PostgresUri")
                           ?? Environment.GetEnvironmentVariable("DATABASE_URL")
                           ?? DefaultPostgresUri;

            if (string.IsNullOrWhiteSpace(rawConn))
            {
                rawConn = DefaultPostgresUri;
            }

            return NormalizarCadenaPostgres(rawConn.Trim());
        }

        public static string NormalizarCadenaPostgres(string input)
        {
            if (input.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                input.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var uri = new Uri(input);
                    var userInfo = uri.UserInfo.Split(':', 2);
                    string username = Uri.UnescapeDataString(userInfo[0]);
                    string password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
                    string database = uri.AbsolutePath.Trim('/');
                    if (string.IsNullOrEmpty(database)) database = "postgres";
                    int port = uri.Port > 0 ? uri.Port : 5432;

                    var builder = new NpgsqlConnectionStringBuilder
                    {
                        Host = uri.Host,
                        Port = port,
                        Username = username,
                        Password = password,
                        Database = database,
                        SslMode = SslMode.Require,
                        TrustServerCertificate = true,
                        Pooling = true,
                        MinPoolSize = 1,
                        MaxPoolSize = 20,
                        Timeout = 15,
                        CommandTimeout = 30
                    };
                    return builder.ConnectionString;
                }
                catch
                {
                    return "Host=db.oxmjxyywxksuwrcgewdg.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=gfDnA*xx.B28wdB;SSL Mode=Require;Trust Server Certificate=true";
                }
            }

            return input;
        }

        public static string ObtenerGoogleClientId(IConfiguration? configuration)
        {
            return configuration?["Authentication:Google:ClientId"]
                ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
                ?? "COLOCAR_TU_GOOGLE_CLIENT_ID_AQUI.apps.googleusercontent.com";
        }

        public static string ObtenerGoogleApiKey(IConfiguration? configuration)
        {
            return configuration?["Authentication:Google:ApiKey"]
                ?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY")
                ?? "COLOCAR_TU_GOOGLE_API_KEY_AQUI";
        }

        public static bool GoogleEstaConfigurado(IConfiguration? configuration)
        {
            string clientId = ObtenerGoogleClientId(configuration);
            return !string.IsNullOrWhiteSpace(clientId)
                && !clientId.StartsWith("COLOCAR_", StringComparison.OrdinalIgnoreCase)
                && !clientId.StartsWith("TU_GOOGLE_", StringComparison.OrdinalIgnoreCase);
        }
    }
}
