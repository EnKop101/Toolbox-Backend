using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Toolbox_Library.Context;
using Toolbox_Library.Repositories.Interfaces;

namespace Toolbox_Library.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ToolboxDbContext _context;

        public GenericRepository(ToolboxDbContext context)
        {
            _context = context;
        }

        public async Task<T?> Get(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> DoesValueExist(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>();

            query = query.Where(predicate);

            return await query.AnyAsync();
        }

        public async Task<bool> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<T, bool>> predicate)
        {
            bool success = await _context.Set<T>().Where(predicate).ExecuteDeleteAsync() > 0;
            _context.ChangeTracker.Clear();
            return success;
        }

        public async Task<bool> Update(T entity)
        {
            _context.Set<T>().Update(entity);

            // If nothing actually changed in the tracker, treat it as a success!
            if (!_context.ChangeTracker.HasChanges())
            {
                return true;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddRange(List<T> entities)
        {
            if (entities.Count == 0) return false; // Skip
            await _context.Set<T>().AddRangeAsync(entities);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRange(List<T> entities)
        {
            if (entities.Count == 0) return false; // Skip
            _context.Set<T>().RemoveRange(entities);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> Count(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null) return await _context.Set<T>().CountAsync();
            else return await _context.Set<T>().CountAsync(predicate);
        }
    }
}
