using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ModuleRepository : EfCoreRepository<Module, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public ModuleRepository(BoxTrackDbContext context) : base(context)
        {
             _context = context;
        }




        /// <summary>
        /// Obtiene el listado de modulos con su objeto de proceso
        /// </summary>
        public async Task<List<Module>> GetAllWithProcess()
        {
            return await _context.Modules.Where(e => e.EstaBorrado == false).Include(p=>p.Process).ToListAsync();
        }



        public async Task<string> GetNextModuleCode(int idProceso)
        {
            string lastCode = "";
            var lastModule = await _context.Modules.Where(m => m.IdProceso == idProceso && m.EstaBorrado == false).OrderBy(m => m.Id).LastOrDefaultAsync();
            if (lastModule != null)
            {
                lastCode = lastModule.Codigo;
            }
            var nextCode = Regex.Replace(lastCode, @"(\d)(\D*)$", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
            return nextCode;
        }

    }
}