using System;
using System.Reflection;
using DbUp;
using DbUp.Helpers;
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

            var migrationUpgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    name => name.Contains(".Scripts.Migrations.") &&
                            (name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) ||
                             name.EndsWith(".psql", StringComparison.OrdinalIgnoreCase))
                )
                .LogToConsole()
                .Build();

            int migrationResult = ExecuteUpgrader("Step 1: Migrations (Run-Once)", migrationUpgrader);
            if (migrationResult != 0) return migrationResult;

            var alwaysRunUpgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    name => name.Contains(".Scripts.AlwaysRun.") &&
                            (name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) ||
                             name.EndsWith(".psql", StringComparison.OrdinalIgnoreCase))
                )
                .JournalTo(new NullJournal()) // Execute every run without logging to journaling table
                .LogToConsole()
                .Build();

            int alwaysRunResult = ExecuteUpgrader("Step 2: AlwaysRun Scripts (Every Run)", alwaysRunUpgrader);
            if (alwaysRunResult != 0) return alwaysRunResult;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All database migrations completed successfully!");
            Console.ResetColor();
            return 0;
        }

        private static int ExecuteUpgrader(string stepName, DbUp.Engine.UpgradeEngine upgrader)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n=======================================================");
            Console.WriteLine($"Starting {stepName}...");
            Console.WriteLine("=======================================================");
            Console.ResetColor();

            var scriptsToExecute = upgrader.GetScriptsToExecute();
            if (scriptsToExecute.Count == 0)
            {
                Console.WriteLine("No scripts pending execution.");
                return 0;
            }

            Console.WriteLine($"Pending scripts ({scriptsToExecute.Count}):");
            foreach (var script in scriptsToExecute)
            {
                Console.WriteLine($"  - {script.Name}");
            }
            Console.WriteLine();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n*** Error occurred during {stepName}! ***");
                if (result.ErrorScript != null)
                {
                    Console.WriteLine($"Failed Script: {result.ErrorScript.Name}");
                }
                Console.WriteLine($"Error Message: {result.Error.Message}");
                Console.WriteLine($"Stack Trace:\n{result.Error}");
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== {stepName} completed successfully! ===");
            Console.ResetColor();

            Console.WriteLine("Successfully executed scripts:");
            foreach (var script in result.Scripts)
            {
                Console.WriteLine($"  [OK] {script.Name}");
            }
            Console.WriteLine();

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

            // Check if the database already exists before dropping it
            bool databaseExists = false;
            using (var existsCmd = new NpgsqlCommand($"SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname = '{targetDatabase}');", connection))
            {
                databaseExists = (bool)existsCmd.ExecuteScalar()!;
            }

            if (databaseExists)
            {
                // Force disconnect any open sessions to the database, then drop
                using (var revokeCmd = new NpgsqlCommand($"REVOKE CONNECT ON DATABASE {targetDatabase} FROM public;", connection))
                {
                    revokeCmd.ExecuteNonQuery();
                }

                using (var terminateCmd = new NpgsqlCommand($@"
                    SELECT pg_terminate_backend(pg_stat_activity.pid)
                    FROM pg_stat_activity
                    WHERE pg_stat_activity.datname = '{targetDatabase}'
                      AND pid <> pg_backend_pid();", connection))
                {
                    terminateCmd.ExecuteNonQuery();
                }

                using (var dropCmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS {targetDatabase};", connection))
                {
                    dropCmd.ExecuteNonQuery();
                    Console.WriteLine($"Database '{targetDatabase}' dropped.");
                }
            }
            else
            {
                Console.WriteLine($"Database '{targetDatabase}' does not exist. Skipping drop.");
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
