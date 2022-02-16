namespace StreetNameRegistry.Migrator.StreetName.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Data.SqlClient;
    using StreetNameRegistry.Infrastructure;

    public class SqlStreamsTable
    {
        private readonly string _connectionString;
        private int _pageIndex = -1;
        private readonly int _pageSize;

        public SqlStreamsTable(string connectionString, int pageSize = 500)
        {
            _connectionString = connectionString;
            _pageSize = pageSize;
        }

        public async Task<IEnumerable<string>?> ReadNextStreetNameStreamPage()
        {
            _pageIndex++;

            await using var conn = new SqlConnection(_connectionString);

            return await conn.QueryAsync<string>(@$"
SELECT IdOriginal FROM 
(SELECT
 IdOriginal,
 round(ROW_NUMBER() OVER(ORDER BY IdInternal ASC)/{_pageSize},0,1) AS PageIndex
 FROM [{Schema.Default}].[Streams]) a
WHERE IdOriginal not like 'municipality-%' AND PageIndex = {_pageIndex}", commandTimeout: 60);
        }
    }
}
