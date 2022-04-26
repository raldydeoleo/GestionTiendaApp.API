using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class CustomerRepository 
    {
        private readonly MaestrosDbContext _context;
        public CustomerRepository(MaestrosDbContext context)
        {
             _context = context;
        }
        /// <summary>
        /// Obtiene los primeros 500 clientes
        /// </summary>
        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.Customers.Take(500).ToListAsync();
        }
        /// <summary>
        /// Obtiene todos los clientes que inician con el termino proporcionado
        /// </summary>
        public async Task<List<Customer>> GetCustomersByTerm(string term)
        {
            return await _context.Customers.Where(c=>c.Nombre1.StartsWith(term)).ToListAsync(); //colocar el centro DO22
        }
    }
}