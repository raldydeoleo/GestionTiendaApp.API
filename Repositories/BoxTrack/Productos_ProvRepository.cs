using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class Productos_ProvRepository 
    {
        private readonly BoxTrackDbContext _context;
        public Productos_ProvRepository(BoxTrackDbContext context) 
        {
             _context = context;
        }

        public async Task<Productos_Prov> Get(int id)
        {
            return await _context.Set<Productos_Prov>().FindAsync(id);
        }

        /// <summary>
        /// Obtiene el listado 
        /// </summary>
        public async Task<List<Productos_Prov>> GetAll()
        {
            return await _context.Productos_Prov.ToListAsync();
        }        

        /// <summary>
        /// Inserta un productos en la base de datos
        /// </summary>
        public async Task<Productos_Prov> CreateProducto(Productos_Prov productos_prov)
        {
            _context.Productos_Prov.Add(productos_prov);
            await _context.SaveChangesAsync();
            return productos_prov;
        }
        /// <summary>
        /// Actualiza un producto en la base de datos
        /// </summary>
        public async Task<Productos_Prov> UpdateProducto(Productos_Prov productos_prov)
        {            
            _context.Productos_Prov.Update(productos_prov);
            await _context.SaveChangesAsync();
            return productos_prov;
        }

        public async Task<Productos_Prov> Delete(Productos_Prov productos_prov)
        {
            _context.Productos_Prov.Remove(productos_prov);
            await _context.SaveChangesAsync();
            return productos_prov;
        }



        public async Task<List<Productos_Prov>> GetProcuctosPorCantidad(int id, int cantidad)
        {
            List<Productos_Prov> productos_prov_Result = new List<Productos_Prov>();            
            productos_prov_Result = await _context.Productos_Prov.Where(s => s.Id == id && s.Cantidad == cantidad).ToListAsync();            
            return productos_prov_Result;
        }

        public async Task<List<Productos_Prov>> GetProcuctosPorFecha(int id, int cantidad, DateTime fecha)
        {
            List<Productos_Prov> productos_prov_Result = new List<Productos_Prov>();
            productos_prov_Result = await _context.Productos_Prov.Where(s => s.Id == id && s.Cantidad == cantidad && s.Fecha_entrada == fecha).ToListAsync();
            return productos_prov_Result;            
        }
      
    }
}