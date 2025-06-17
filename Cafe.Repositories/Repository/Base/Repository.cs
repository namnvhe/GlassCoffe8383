using Cafe.BusinessObjects.Models;
using Cafe.Repositories.IRepository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly CoffeManagerContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(CoffeManagerContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public List<T> GetAll()
        {
            try
            {
                return _dbSet.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting all {typeof(T).Name}: {e.Message}");
            }
        }

        public List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return orderBy(query).ToList();
                }
                else
                {
                    return query.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting {typeof(T).Name} with filter: {e.Message}");
            }
        }

        public T GetById(object id)
        {
            try
            {
                return _dbSet.Find(id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting {typeof(T).Name} by ID: {e.Message}");
            }
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.FirstOrDefault(predicate);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding {typeof(T).Name}: {e.Message}");
            }
        }

        public List<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.Where(predicate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding all {typeof(T).Name}: {e.Message}");
            }
        }

        public void Add(T entity)
        {
            try
            {
                _dbSet.Add(entity);
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding {typeof(T).Name}: {e.Message}");
            }
        }

        public void AddRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.AddRange(entities);
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding range of {typeof(T).Name}: {e.Message}");
            }
        }

        public void Update(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating {typeof(T).Name}: {e.Message}");
            }
        }

        public void Remove(T entity)
        {
            try
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _dbSet.Remove(entity);
            }
            catch (Exception e)
            {
                throw new Exception($"Error removing {typeof(T).Name}: {e.Message}");
            }
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
            }
            catch (Exception e)
            {
                throw new Exception($"Error removing range of {typeof(T).Name}: {e.Message}");
            }
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.Any(predicate);
            }
            catch (Exception e)
            {
                throw new Exception($"Error checking existence of {typeof(T).Name}: {e.Message}");
            }
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                if (predicate == null)
                    return _dbSet.Count();
                else
                    return _dbSet.Count(predicate);
            }
            catch (Exception e)
            {
                throw new Exception($"Error counting {typeof(T).Name}: {e.Message}");
            }
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving changes: {e.Message}");
            }
        }
    }
}
