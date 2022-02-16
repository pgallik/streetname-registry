namespace StreetNameRegistry.Migrator.StreetName.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using StreetNameRegistry.Infrastructure;

    public class ProcessedIdsTable
    {
        private readonly string _connectionString;
        private readonly ILogger<ProcessedIdsTable> _logger;

        public ProcessedIdsTable(string connectionString, ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _logger = loggerFactory.CreateLogger<ProcessedIdsTable>();
        }

        private const string ProcessedIdsTableName = "ProcessedIds";

        public async Task CreateTableIfNotExists()
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@$"
IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = N'{Schema.MigrateStreetName}')
    EXEC('CREATE SCHEMA [{Schema.MigrateStreetName}]');

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{ProcessedIdsTableName}' and xtype='U')
CREATE TABLE [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}](
[Id] [nvarchar](1000) NOT NULL,
CONSTRAINT [PK_ProcessedIds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))");
        }

        public async Task Add(string streetNameStreamId)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.ExecuteAsync(@$"INSERT INTO [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}] VALUES ('{streetNameStreamId}')");
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, $"Failed to add Id '{streetNameStreamId}' to ProcessedIds table");
                throw;
            }
        }

        public async Task<IEnumerable<string>?> GetProcessedIds()
        {
            await using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<string>($"SELECT Id FROM [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}]");
            return result;
        }
    }
}
