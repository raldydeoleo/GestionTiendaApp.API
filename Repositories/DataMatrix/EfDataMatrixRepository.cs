
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    /// <summary>
    /// Define los métodos básicos de una entidad de forma genérica
    /// </summary>
    public abstract class EfDataMatrixRepository<TEntity, TContext> : IDataMatrixRepository<TEntity>
        where TEntity : class, IDataMatrixEntity
        where TContext : DbContext
    {
        private readonly TContext context;
        public EfDataMatrixRepository(TContext context)
        {
            this.context = context;
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<List<TEntity>> BulkInsert(List<TEntity> entities)
        {
            await context.BulkInsertAsync(entities);
            return entities;
        }
        public async Task<TEntity> Get(int id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }
        public async Task<List<TEntity>> GetAll()
        {
            return await context.Set<TEntity>().ToListAsync();
        }
        public async Task<TEntity> Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Delete(int id)
        {
            var entity = await context.FindAsync<TEntity>(id);
            context.Remove<TEntity>(entity);
            await context.SaveChangesAsync();
            return entity;
        }

    }
}