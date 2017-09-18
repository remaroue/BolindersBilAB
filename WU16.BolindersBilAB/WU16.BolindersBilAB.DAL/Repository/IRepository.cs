using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> expression = null);
        T Insert(T entity);
        void  Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}
