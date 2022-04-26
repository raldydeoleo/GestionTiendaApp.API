using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ProcessRepository : EfCoreRepository<Process, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public ProcessRepository(BoxTrackDbContext context) : base(context)
        {
             _context = context;
        }
        /// <summary>
        /// Obtiene un listado de procesos filtrado por el rol del usuario.
        /// </summary>
        //public async Task<List<Process>> GetByRol(int rol)
        //{
        //    if(rol == (int)RolesEnum.PRINTER_MANUAL || rol == (int)RolesEnum.PRINTER_PLANNER_MANUAL)
        //    {
        //        return await _context.Processes.Where(e => e.EstaBorrado == false && e.Codigo == "EmpMan").ToListAsync();
        //    }
        //    else if (rol == (int)RolesEnum.PRINTER_MECANIZADO || rol == (int)RolesEnum.PRINTER_PLANNER_MECANIZADO)
        //    {
        //        return await _context.Processes.Where(e => e.EstaBorrado == false && e.Codigo == "EmpMec").ToListAsync();
        //    }
        //    else if (rol == (int)RolesEnum.PRINTER_POUCH || rol == (int)RolesEnum.PRINTER_PLANNER_POUCH)
        //    {
        //        return await _context.Processes.Where(e => e.EstaBorrado == false && e.Codigo == "EmpPou").ToListAsync();
        //    }
        //    else
        //    {
        //        return await _context.Processes.Where(e => e.EstaBorrado == false).ToListAsync();
        //    }
            
        //}

    }
}