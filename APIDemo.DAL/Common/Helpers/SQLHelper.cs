using APIDemo.DAL.Common.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APIDemo.DAL.Common.Helpers
{
    public class SQLHelper : IDisposable, IDatabaseHelper
    {
        private readonly IConfiguration _configuration;
        private SqlConnection _connection;
        private SqlCommand objCommand;
        public SQLHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("SQLConnection"));
            objCommand = new SqlCommand();
            objCommand.CommandTimeout = 120;
            objCommand.Connection = _connection;
        }
        public void Dispose()
        {
            objCommand.Parameters.Clear();
            _connection.Close();
            _connection.Dispose();
            objCommand.Dispose();
        }
        public IDbConnection GetConnection()
        {
            return _connection;
        }
    }
}
