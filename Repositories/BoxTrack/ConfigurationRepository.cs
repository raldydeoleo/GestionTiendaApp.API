using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public class ConfigurationRepository : EfCoreRepository<ConfigurationValue, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public ConfigurationRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Obtiene una configuración de la aplicación mediante su código
        /// </summary>
        public async Task<ConfigurationValue> GetByCode(string code)
        {
            return await _context.Set<ConfigurationValue>().Where(c=>c.Codigo == code).FirstOrDefaultAsync();
        }
    }
}
