using System.Linq.Expressions;

namespace Data.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
    }
}
