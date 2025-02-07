namespace ITnetworkProjekt.Interfaces
{

    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindById(int id);
        Task<bool> ExistsWithId(int id);
        Task<List<TEntity>> GetAll();
        Task<TEntity> Insert(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task Delete(TEntity entity);
        Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId);
    }
}
