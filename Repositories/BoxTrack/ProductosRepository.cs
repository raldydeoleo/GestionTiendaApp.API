using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ProductosRepository //: EfCoreRepository<BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public ProductosRepository(BoxTrackDbContext context) //: base(context)
        {
             _context = context;
        }

        public async Task<Productos> Get(int id)
        {
            return await _context.Set<Productos>().FindAsync(id);
        }

        /// <summary>
        /// Obtiene el listado 
        /// </summary>
        public async Task<List<Productos>> GetAll()
        {
            return await _context.Productos.Include(p=>p.suplidores).ToListAsync();
        }        

        /// <summary>
        /// Inserta un productos en la base de datos
        /// </summary>
        public async Task<Productos> CreateProducto(Productos productos)
        {
            _context.Productos.Add(productos);
            await _context.SaveChangesAsync();
            return productos;
        }
        /// <summary>
        /// Actualiza un producto en la base de datos
        /// </summary>
        public async Task<Productos> UpdateProducto(Productos productos)
        {            
            _context.Productos.Update(productos);
            await _context.SaveChangesAsync();
            return productos;
        }



        public async Task<Productos> Delete(Productos productos)
        {
            _context.Productos.Remove(productos);
            await _context.SaveChangesAsync();
            return productos;
        }

        public async Task<Productos> UpdateCantidadProducto(Productos productos)
        {
            //Productos productos = new Productos();
            //productos.Cantidad = parametrosReposicion.Cantidad;
            _context.Productos.Update(productos);
            await _context.SaveChangesAsync();
            return productos;
        }


        public async Task<Productos> Compra(Productos productos)
        {
            _context.Productos.Update(productos);
            await _context.SaveChangesAsync();
            return productos;
        }

    }
}