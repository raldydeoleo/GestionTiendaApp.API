
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Get(int id);
        Task<bool> IsCodeUnavailable(T entity);
    }
}