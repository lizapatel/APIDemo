using APIDemo.DAL.Common.Factory;
using APIDemo.DAL.Common.Helpers;
using APIDemo.DAL.Common.Interface;
using APIDemo.DAL.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDemo.DAL.Common
{
    public class DBContext
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseType _databaseType;
        //private readonly IDbConnection _connection;
        public IDatabaseHelper IDatabaseHelper;
        public DBContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _databaseType = _configuration["DatabaseType"] == "SQL" ? DatabaseType.SQL : DatabaseType.PostgreSQL;
            IDatabaseHelper = DatabaseFactory.GetDatabaseHelper(_databaseType, _configuration);
        }
    }
}
