using System.Collections.Generic;
using System.Data;

namespace Repository
{
    public interface IGenericRepository<T>
    {
        void AddOrUpdate(T entity);
        void Delete(int id);
        IEnumerable<T> GetAll();
        void Merge(IEnumerable<T> entities);
        T Get(int id);
    }
}