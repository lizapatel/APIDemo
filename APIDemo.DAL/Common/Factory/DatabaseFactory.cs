using APIDemo.DAL.Common.Helpers;
using APIDemo.DAL.Common.Interface;
using APIDemo.DAL.Enum;
using Microsoft.Extensions.Configuration;

namespace APIDemo.DAL.Common.Factory
{
    public static class DatabaseFactory
    {
        public static IDatabaseHelper GetDatabaseHelper(DatabaseType databaseType, IConfiguration _configuration)
        {
            switch (databaseType)
            {
                case DatabaseType.SQL:
                    return new SQLHelper(_configuration);
                case DatabaseType.PostgreSQL:
                    return new PostgreSQLHelper(_configuration);
                default:
                    throw new ArgumentException("Invalid database type");
            };
        }
    }
}
