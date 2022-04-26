using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ShiftRepository : EfCoreRepository<Shift, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public ShiftRepository(BoxTrackDbContext context) : base(context)
        {
             _context = context;
        }
        /*Métodos específicos de la entidad Shift*/
        /// <summary>
        /// Obtiene un turno desde la BD mediante su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Shift> Get(string id)
        {
            return await _context.Set<Shift>().FindAsync(id);
        }
        /// <summary>
        /// Obtiene el turno actual tomando como parámetro la hora especificada
        /// </summary>
        /// <param name="currentTime">Hora especificada</param>
        /// <returns>Turno Actual</returns>
        public async Task<Shift> GetCurrentShift(TimeSpan currentTime)
        { 
            var shifts = await _context.Shifts.OrderBy(s=>s.Id).ToListAsync();
            var shift =  shifts.SingleOrDefault(s => s.HoraInicio <= currentTime && s.HoraFin > currentTime && s.EstaBorrado == false);
            if(shift == null)
            {
                shift = shifts.LastOrDefault(s=>s.EstaBorrado == false);
            }
            return shift;
        }
        /// <summary>
        /// Obtiene todos los turnos organizados por su id
        /// </summary>
        /// <returns>Listado de turnos</returns>
        public async Task<List<Shift>> GetShifts()
        { 
            return await _context.Shifts.Where(s=>s.EstaBorrado==false).OrderBy(s=>s.Id).ToListAsync();
        }
        /// <summary>
        /// Obtiene el último turno, el orden lo establece su id
        /// </summary>
        public async Task<Shift> GetLastShift()
        {
            var shifts = await GetShifts();
            return shifts.LastOrDefault();
        }
        
    }
}