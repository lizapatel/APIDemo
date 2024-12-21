using APIDemo.DAL.Common.Interface;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace APIDemo.DAL.Common.Helpers
{
    public class PostgreSQLHelper : IDisposable,IDatabaseHelper
    {
        private readonly IConfiguration _configuration;
        private NpgsqlConnection _connection;
        private NpgsqlCommand _command;
        public PostgreSQLHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new NpgsqlConnection(_configuration.GetConnectionString("PostgresConnection"));
            _command = new NpgsqlCommand
            {
                CommandTimeout = 120,
                Connection = _connection
            };
        }
        public void Dispose()
        {
            _command.Parameters.Clear();
            _connection.Close();
            _connection.Dispose();
            _command.Dispose();
        }
        public IDbConnection GetConnection()
        {
            return _connection;
        }
    }
}
