using APIDemo.BAL.Entity;
using APIDemo.BAL.Interface;
using APIDemo.DAL.Common;
using APIDemo.DAL.Common.Helpers;
using APIDemo.DAL.Common.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using static Dapper.SqlMapper;

namespace APIDemo.DAL.Repositories.SQL.Master
{
    public class UserRepository : IMaster<User>
    {
        private DBContext _dbContext;
        private readonly IDapperService _dapperService;

        public UserRepository(IConfiguration configuration)
        {
            _dapperService = new DapperService(configuration);
        }
        public async Task<List<User>> GetList()
        {
            var query = "sp_user_Get";
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserId", null);
            return await _dapperService.GetAllAsync<User>(query, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<long> Save(User user)
        {
            EntityConverter<User> converter = new EntityConverter<User>();
            string xmlUser = converter.ConvertEntityToXml(user);

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("XMLInput", xmlUser, DbType.Xml);
            dynamicParameters.Add("IsUpdate", false, DbType.Boolean);
            dynamicParameters.Add("@ResultId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            _dapperService.Execute("[dbo].[sp_user_AddEdit]", dynamicParameters, commandType: CommandType.StoredProcedure);
            return dynamicParameters.Get<Int64>("@ResultId");
        }
        public async Task<long> Edit(User user)
        {
            EntityConverter<User> converter = new EntityConverter<User>();
            string xmlUser = converter.ConvertEntityToXml(user);

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("XMLInput", xmlUser, DbType.Xml);
            dynamicParameters.Add("IsUpdate", true, DbType.Boolean);
            dynamicParameters.Add("@ResultId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            _dapperService.Execute("[dbo].[sp_user_AddEdit]", dynamicParameters, commandType: CommandType.StoredProcedure);
            return dynamicParameters.Get<Int64>("@ResultId");
        }
        public async Task<bool> Delete(Int64 UserId)
        {
            var query = "sp_user_Delete";
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserId", UserId);
            int affectedRows = await _dapperService.ExecuteScalerAsync(query, parms: dynamicParameters, commandType: CommandType.StoredProcedure);
            if (affectedRows > 0)
                return true;
            return false;
        }
        public async Task<User> GetDetail(Int64 UserId)
        {
            var query = "sp_user_Get";
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@UserId", UserId);
            return await _dapperService.GetAsync<User>(query, parms: dynamicParameters, commandType: CommandType.StoredProcedure);
        }
    }
}
