using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FileHosting.Database
{
    public sealed class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        #region Constructor

        public GenericRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        #endregion

        #region GenericRepository<T> implementation

        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, IEnumerable<string> includes)
        {
            foreach (string include in includes)
            {
                _dbSet.Include(include);
            }

            return _dbSet.Where(predicate);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }
        
        public T First(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.First(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }        

        public T Single(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Single(predicate);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }        

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbSet.Add(entity);
        }

        public void Attach(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbSet.Attach(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbSet.Remove(entity);
        }        

        #endregion
    }
}