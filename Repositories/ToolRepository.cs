using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Client;
using sixxonAPI.Dtos;
using sixxonAPI.Services;
using SixxonAPI.Dtos;
using SixxonAPI.Interfaces;
using SixxonAPI.Models;
using System.Data;

namespace SixxonAPI.Repositories
{
    public class ToolRepository : IToolRepository
    {
        private readonly string _connectionString;

        public ToolRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }
        private IDbConnection Connection => new OracleConnection(_connectionString);
        public async Task<IEnumerable<Tool>> GetAllAsync()
        {
            using var conn = Connection;

            var sql = @"
            SELECT
                t.CATEGORY,
                t.BARCODE,
                t.TOOL_NAME,
                t.TOOL_CODE,
                t.BORROW_DAYS,
                t.REMARK,
                br.RECEIVER,
                br.EXPECTED_RETURN_DATE
            FROM TOOLTABLE t
            LEFT JOIN (
                SELECT *
                FROM BORROWRECORDS
                WHERE ACTUAL_RETURN_DATE IS NULL
            ) br ON t.BARCODE = br.BARCODE";

            return await conn.QueryAsync<Tool>(sql);
        }
        public async Task<bool> IsBarcodeExistsAsync(string barcode)
        {
            var sql = "SELECT COUNT(1) FROM TOOLTABLE WHERE BARCODE = :Barcode";
            using var conn = Connection;
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Barcode = barcode });
            return count > 0;
        }
        public async Task InsertAsync(CreateToolDto dto)
        {
            var sql = @"
            INSERT INTO TOOLTABLE (ID, TOOL_CODE, TOOL_NAME, CATEGORY, BORROW_DAYS, BARCODE, STATUS, REMARK)
            VALUES (:Id, :ToolCode, :ToolName, :Category, :BorrowDays, :Barcode, :Status, :Remark)";

            var parameters = new
            {
                Id = Guid.NewGuid().ToString(),
                dto.ToolCode,
                dto.ToolName,
                dto.Category,
                dto.BorrowDays,
                dto.Barcode,
                dto.Status,
                dto.Remark
            };

            using var conn = Connection;
            await conn.ExecuteAsync(sql, parameters);
        }
    }

}
