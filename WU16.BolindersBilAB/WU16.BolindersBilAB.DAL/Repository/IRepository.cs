using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Get();
        T Insert(T entity);
        void  Delete(T entity);
        void Edit(T entity);
        void Save();
        void Delete(IEnumerable<T> entities);
    }
}
