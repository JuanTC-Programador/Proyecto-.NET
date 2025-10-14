using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApi.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(int id);
    Task<T> Add(T entity);
    Task Update(T entity);
    Task Delete(int id);
}
