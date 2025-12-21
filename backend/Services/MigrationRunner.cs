using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class MigrationRunner
    {
        private readonly string _connectionString;
        private readonly string _migrationsPath;

        public MigrationRunner(string connectionString, string migrationsPath)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _migrationsPath = migrationsPath ?? throw new ArgumentNullException(nameof(migrationsPath));
        }
        // Run all migrations
        public async Task RunAllAsync(CancellationToken token = default)
        {
            if (!Directory.Exists(_migrationsPath))
            {
                Console.WriteLine($"[Migration] Directory not found: {_migrationsPath}");
                return;
            }

            var files = Directory.GetFiles(_migrationsPath, "*.sql")
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (files.Length == 0) return;

            await EnsureHistoryTableAsync(token);

            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();

                var content = await File.ReadAllTextAsync(file, token);
                var hash = ComputeSha256Hash(content);
                var fileName = Path.GetFileName(file);

                if (await IsMigrationAppliedAsync(fileName, hash, token))
                {
                    Console.WriteLine($"[Migration] Skipping already applied: {fileName}");
                    continue;
                }

                Console.WriteLine($"[Migration] Applying: {fileName}");
                await ApplyMigrationAsync(fileName, content, hash, token);
                Console.WriteLine($"[Migration] Applied: {fileName}");
            }
        }
        // Ensure migration history table exists
        private async Task EnsureHistoryTableAsync(CancellationToken token)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '_MigrationsHistory')
                BEGIN
                    CREATE TABLE _MigrationsHistory (
                        MigrationId INT IDENTITY(1,1) PRIMARY KEY,
                        FileName NVARCHAR(255) NOT NULL,
                        FileHash NVARCHAR(64) NOT NULL,
                        AppliedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
                    );
                END";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(token);
            await using var cmd = new SqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync(token);
        }
        // Check if migration is already applied
        private async Task<bool> IsMigrationAppliedAsync(string fileName, string hash, CancellationToken token)
        {
            const string sql = "SELECT COUNT(1) FROM _MigrationsHistory WHERE FileName = @name AND FileHash = @hash";

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(token);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", fileName);
            cmd.Parameters.AddWithValue("@hash", hash);

            var count = (int)await cmd.ExecuteScalarAsync(token);
            return count > 0;
        }
        // Apply a single migration
        private async Task ApplyMigrationAsync(string fileName, string sqlContent, string hash, CancellationToken token)
        {
            var batches = SplitSqlBatches(sqlContent);

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(token);
            await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync(token);

            try
            {
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;

                    await using var cmd = new SqlCommand(batch, conn, transaction);
                    await cmd.ExecuteNonQueryAsync(token);
                }

                var logSql = "INSERT INTO _MigrationsHistory (FileName, FileHash) VALUES (@name, @hash)";
                await using var logCmd = new SqlCommand(logSql, conn, transaction);
                logCmd.Parameters.AddWithValue("@name", fileName);
                logCmd.Parameters.AddWithValue("@hash", hash);
                await logCmd.ExecuteNonQueryAsync(token);

                await transaction.CommitAsync(token);
            }
            catch
            {
                await transaction.RollbackAsync(token);
                throw;
            }
        }
        // Split SQL script into batches by "GO" statements
        private static string[] SplitSqlBatches(string sql)
        {
            var regex = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Split(sql);
        }
        // Compute SHA-256 hash of a string
        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}