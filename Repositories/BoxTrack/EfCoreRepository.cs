
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    /// <summary>
    /// Define los métodos básicos de una entidad de forma genérica
    /// </summary>
    public abstract class EfCoreRepository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        private readonly TContext context;
        public EfCoreRepository(TContext context)
        {
            this.context = context;
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Get(int id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }
        public async Task<List<TEntity>> GetAll()
        {
            return await context.Set<TEntity>().Where(e=>e.EstaBorrado == false).ToListAsync();
        }
        public async Task<TEntity> Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> IsCodeUnavailable(TEntity entity)
        {
            entity.Codigo = entity.Codigo.TrimEnd();
            bool isCodeUnavailable = false;
            var storageInDb = await context.Set<TEntity>().Where(e => e.Id == entity.Id).AsNoTracking().SingleOrDefaultAsync();
            var posibleDuplicate = await context.Set<TEntity>().Where(e => e.Codigo == entity.Codigo && e.EstaBorrado == false).AsNoTracking().FirstOrDefaultAsync();
            if (storageInDb != null)
            {
                if (entity.Codigo != storageInDb.Codigo)
                {
                    if (posibleDuplicate != null)
                    {
                        isCodeUnavailable = true;
                    }
                }
            }
            else
            {
                if (posibleDuplicate != null)
                {
                    isCodeUnavailable = true;
                }
            }
            
            return isCodeUnavailable;
        }

    }
}