namespace StreetNameRegistry.Migrator.StreetName.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Data.SqlClient;
    using StreetNameRegistry.Infrastructure;

    public class ProcessedIdsTable
    {
        private readonly string _connectionString;

        public ProcessedIdsTable(string connectionString)
        {
            _connectionString = connectionString;
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
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{ProcessedIdsTableName}' and xtype='U')
CREATE TABLE [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}](
[Id] [char](42) NOT NULL,
CONSTRAINT [PK_ProcessedIds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))
GO");
        }

        public async Task<bool> Add(string streetNameStreamId)
        {
            await using var conn = new SqlConnection(_connectionString);
            var result = await conn.ExecuteAsync(@$"INSERT INTO [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}] VALUES ({streetNameStreamId})");
            return result > 0;
        }

        public async Task<IEnumerable<string>?> GetProcessedIds()
        {
            await using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<string>($"SELECT Id FROM [{Schema.MigrateStreetName}].[{ProcessedIdsTableName}]");
            return result;
        }
    }
}
