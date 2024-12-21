using APIDemo.BAL.Entity;
using APIDemo.BAL.Interface;
using APIDemo.DAL.Common;
using APIDemo.DAL.Common.Helpers;
using APIDemo.DAL.Common.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace APIDemo.DAL.Repositories.PgSQL.Master
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
            var query = "SELECT * FROM public.fn_user_get()";
            return await _dapperService.GetAllAsync<User>(query, null, commandType: CommandType.Text);
        }
        public async Task<long> Save(User user)
        {
            string inputJson = JsonConvert.SerializeObject(user);
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("p_input_json", inputJson, DbType.String);
            dynamicParameters.Add("p_is_update", false, DbType.Boolean);
            var query = "SELECT * FROM public.fn_user_AddEdit(@p_input_json::json, @p_is_update)";
            return await _dapperService.ExecuteScalerAsync(query, parms: dynamicParameters, commandType: CommandType.Text);
        }
        public async Task<long> Edit(User user)
        {
            string inputJson = JsonConvert.SerializeObject(user);
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("p_input_json", inputJson, DbType.String);
            dynamicParameters.Add("p_is_update", true, DbType.Boolean);
            var query = "SELECT * FROM public.fn_user_AddEdit(@p_input_json::json, @p_is_update)";
            return await _dapperService.ExecuteScalerAsync(query, parms: dynamicParameters, commandType: CommandType.Text);
        }
        public async Task<bool> Delete(Int64 UserId)
        {
            var query = "SELECT * FROM public.fn_user_delete(p_user_id := @p_user_id)";
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("p_user_id", UserId);

            int affectedRows = await _dapperService.ExecuteScalerAsync(query, parms: dynamicParameters, commandType: CommandType.Text);
            if (affectedRows > 0)
                return true;
            return false;
        }
        public async Task<User> GetDetail(Int64 UserId)
        {
            var query = "SELECT * FROM public.fn_user_get(p_user_id := @p_user_id)";
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("p_user_id", UserId);
            return await _dapperService.GetAsync<User>(query, parms: dynamicParameters, commandType: CommandType.Text);
        }
    }
}
