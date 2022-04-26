
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public interface IDataMatrixRepository<T> where T : class, IDataMatrixEntity
    {
        Task<List<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Get(int id);
        Task<T> Delete(int id);
    }
}