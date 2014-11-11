using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FileHosting.Database
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();

        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, IEnumerable<string> includes);

        int Count();        

        T GetById(int id);
        T First(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);        
        T Single(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);        

        void Add(T entity);
        void Attach(T entity);
        void Delete(T entity);        
    }
}