using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SanskritQuest.Database.Migrations
{
    class Program
    {
        static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            bool recreateDatabase = configuration.GetValue<bool>("RecreateDatabase");

            if (recreateDatabase)
            {
                Console.WriteLine("RecreateDatabase flag is true. Dropping and recreating database...");
                try
                {
                    RecreateDatabase(connectionString);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error recreating database: {ex.Message}");
                    Console.ResetColor();
                    return -1;
                }
            }

            // Ensure the database exists (creates it if missing and not already created by the drop/recreate logic)
            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) ||
                            name.EndsWith(".psql", StringComparison.OrdinalIgnoreCase)
                )
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database upgrade successful!");
            Console.ResetColor();
            return 0;
        }

        private static void RecreateDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            string? targetDatabase = builder.Database;

            if (string.IsNullOrWhiteSpace(targetDatabase))
            {
                throw new InvalidOperationException("Target database name cannot be empty.");
            }

            // Redirect connection to the default administrative database to perform DROP/CREATE
            builder.Database = "postgres";
            string adminConnectionString = builder.ConnectionString;

            using var connection = new NpgsqlConnection(adminConnectionString);
            connection.Open();

            // Force disconnect any open sessions to the database, then drop
            using (var dropCmd = new NpgsqlCommand($@"
                REVOKE CONNECT ON DATABASE {targetDatabase} FROM public;
                SELECT pg_terminate_backend(pg_stat_activity.pid)
                FROM pg_stat_activity
                WHERE pg_stat_activity.datname = '{targetDatabase}'
                  AND pid <> pg_backend_pid();
                DROP DATABASE IF EXISTS {targetDatabase};", connection))
            {
                dropCmd.ExecuteNonQuery();
                Console.WriteLine($"Database '{targetDatabase}' dropped.");
            }

            // Create fresh database
            using (var createCmd = new NpgsqlCommand($"CREATE DATABASE {targetDatabase};", connection))
            {
                createCmd.ExecuteNonQuery();
                Console.WriteLine($"Database '{targetDatabase}' created.");
            }
        }
    }
}
