namespace APIDemo.BAL.Interface
{
    public interface IMaster<T> where T : class
    {
        Task<List<T>> GetList();
        Task<Int64> Save(T entity);
        Task<Int64> Edit(T entity);
        Task<bool> Delete(Int64 Id);
        Task<T> GetDetail(Int64 Id);
    }
}
