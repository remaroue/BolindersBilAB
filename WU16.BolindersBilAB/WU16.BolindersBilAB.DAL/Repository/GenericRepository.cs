using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.DAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace WU16.BolindersBilAB.DAL.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext _ctx;

        public GenericRepository(ApplicationDbContext Ctx)
        {
            _ctx = Ctx;
        }

        public void Delete(T entity)
        {
            _ctx.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            _ctx.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<T> Get()
        {
            return _ctx.Set<T>();
        }

        public T Insert(T entity)
        {
            _ctx.Set<T>().Add(entity);

            return entity;
        }

        public void Save()
        {
            _ctx.SaveChanges();
        }
    }
}
