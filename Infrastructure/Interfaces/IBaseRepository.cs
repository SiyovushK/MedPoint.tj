namespace Infrastructure.Interfaces;

public interface IBaseRepository<T1, T2>
{
    Task<T2?> AddAsync(T1 entity);
    Task<T2?> UpdateAsync(T1 entity);
    Task<T2?> DeleteAsync(T1 entity);
    Task<T1?> GetByIdAsync(int id);
    IQueryable<T1> GetAll();
}