﻿using HBlog.Domain.Common;
using HBlog.Domain.Repositories;
using HBlog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HBlog.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DataContext _dataContext;
        protected readonly DbSet<TEntity> _dbSet;
        public Repository(DataContext context)
        {
            _dataContext = context;
            _dbSet = _dataContext.Set<TEntity>();
        }
        public virtual void Add(TEntity obj) => _dbSet.Add(obj);
        public virtual IQueryable<TEntity> GetAll() => _dbSet.AsNoTracking();
        public virtual IQueryable<TEntity> GetAllSoftDeleted()
            => _dbSet.IgnoreQueryFilters().Where(x => EF.Property<bool>(x, "IsDeleted"));
        public virtual async Task<TEntity> GetById(int id) => await _dbSet.FindAsync(id);

        public virtual void Remove(int id)
            => _dbSet.Remove(_dbSet.Find(id));
        public virtual async Task<int> SaveChangesAsync()
            => await _dataContext.SaveChangesAsync();
        public async Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate)
            => await Task.Run(() => _dbSet.Where(predicate)); 
    }
}
