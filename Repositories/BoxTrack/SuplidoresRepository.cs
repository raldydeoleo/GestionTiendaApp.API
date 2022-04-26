using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class SuplidoresRepository
    {
        private readonly BoxTrackDbContext _context;
        public SuplidoresRepository(BoxTrackDbContext context) 
        {
             _context = context;
        }

        public async Task<Suplidores> Get(int id)
        {
            return await _context.Set<Suplidores>().FindAsync(id);
        }

        /// <summary>
        /// Obtiene el listado 
        /// </summary>
        public async Task<List<Suplidores>> GetAll()
        {
            return await _context.Suplidores.ToListAsync();
        }        

        /// <summary>
        /// Inserta un suplidor o proveedor en la tabla Suplidores de la base de datos
        /// </summary>
        public async Task<Suplidores> CreateSuplidor(Suplidores suplidores)
        {
            _context.Suplidores.Add(suplidores);
            await _context.SaveChangesAsync();
            return suplidores;
        }
        /// <summary>
        /// Actualiza un producto en la base de datos
        /// </summary>
        public async Task<Suplidores> UpdateSuplidor(Suplidores suplidores)
        {            
            _context.Suplidores.Update(suplidores);
            await _context.SaveChangesAsync();
            return suplidores;
        }

        public async Task<Suplidores> Delete(Suplidores suplidores)
        {
            _context.Suplidores.Remove(suplidores);
            await _context.SaveChangesAsync();
            return suplidores;
        }
    }
}