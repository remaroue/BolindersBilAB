using System;
using System.Collections.Generic;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Repository
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> Get();
        T Insert(T entity);
        void  Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}
