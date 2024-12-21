using APIDemo.DAL.Common.Factory;
using APIDemo.DAL.Common.Interface;
using APIDemo.DAL.Enum;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APIDemo.DAL.Common.Helpers
{
    public class DapperService : IDapperService
    {
        private readonly IDatabaseHelper _IDatabaseHelper;
        private readonly IConfiguration _configuration;
        private readonly DatabaseType _databaseType;
        private readonly IDbConnection _dbConnection;

        public DapperService(IConfiguration configuration)
        {
            _configuration = configuration;
            _databaseType = _configuration["DatabaseType"] == "SQL" ? DatabaseType.SQL : DatabaseType.PostgreSQL;
            _IDatabaseHelper = DatabaseFactory.GetDatabaseHelper(_databaseType, _configuration);
            _dbConnection = _IDatabaseHelper.GetConnection();
        }
        public async Task<List<T>> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            var result = await _dbConnection.QueryAsync<T>(sp, parms, commandType: commandType);
            return result.ToList();
        }
        public async Task<int> ExecuteScalerAsync(string sp, DynamicParameters parms, CommandType commandType)
        {
            return await _dbConnection.ExecuteScalarAsync<int>(sp, parms, null, null, commandType: commandType);
        }
        public async Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            var result = await _dbConnection.QueryAsync<T>(sp, parms, commandType: commandType);
            return result.FirstOrDefault();
        }
        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            return _dbConnection.Execute(sp, parms, commandType: commandType);
        }
        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
    }
}
