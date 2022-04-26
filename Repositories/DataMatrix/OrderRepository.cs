using BoxTrackLabel.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace BoxTrackLabel.API.Repositories
{
    public class OrderRepository: EfDataMatrixRepository<Order, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public OrderRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetByDate(DateTime date)
        {
            return await _context.Orders.Where(o=>o.CreationDate.Date == date).Include(o => o.Codes).ToListAsync();
        }
        public async Task<Order> GetOrderById(string id)
        {
            return await _context.Orders.SingleOrDefaultAsync(o => o.OrderId == id);
        }
        public async Task<Order> UpdateOrderStatus(int orderId, string status, string remark = null)
        {
            var order = await Get(orderId);
            order.Status = status;
            if(remark != null)
            {
                order.Remark = remark;
            }
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order> AuthorizeOrder(int orderId, string userName)
        {
            var order = await Get(orderId);
            order.IsPrintAuthorized = true;
            order.UserPrintAuthorized = userName;
            order.PrintAuthorizedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order> CloseOrder(int orderId, string user) 
        {  
            using (var transaction = _context.Database.BeginTransaction())
            {
                await _context.Productions.Where(p=>p.DataMatrixOrderId == orderId && p.TurnoAbierto == true)
                    .BatchUpdateAsync(p=>new Production{FechaHoraCierreTurno = DateTime.Now, UsuarioCierreTurno = user, TurnoAbierto = false, ProductoFinalizado = true});
                var order = await Get(orderId);
                order.IsClosed = true;
                order.Status = "Cerrada";
                order.UserClose = user;
                order.CloseDate = DateTime.Now;
                try  
                {    
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException dbcex)
                {
                    await dbcex.Entries.Single().ReloadAsync();
                    await _context.SaveChangesAsync();
                }
                transaction.Commit();
                return order;
            }
            
        }
        public async Task<List<Order>> FetchPendingOrders()
        {
            return await _context.Orders.Where(o=>o.Status == "Creada").ToListAsync();
        }
    }
}