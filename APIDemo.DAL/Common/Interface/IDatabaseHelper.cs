using System.Data;

namespace APIDemo.DAL.Common.Interface
{
    public interface IDatabaseHelper
    {
        IDbConnection GetConnection();
    }
}
