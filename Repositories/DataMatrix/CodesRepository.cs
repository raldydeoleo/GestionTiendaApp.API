using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BoxTrackLabel.API.Repositories
{
    public class CodesRepository: EfDataMatrixRepository<Code, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public CodesRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Code>> ProcessOrder(List<Code> codes)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var order = await _context.Orders.Where(o => o.Id == codes[0].OrderId).SingleOrDefaultAsync();
                order.Status = "Procesada";
                try
                {
                    await _context.SaveChangesAsync();
                    await _context.BulkInsertAsync(codes);
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    await ex.Entries.Single().ReloadAsync();
                    await _context.SaveChangesAsync();
                    Log.Error(ex, ex.Message);
                }
                transaction.Commit();
                return codes;   
            } 
        }
        public async Task<Tuple<string,string>> ConfirmCode(string code, string user)
        {
            try
            {
                if(!IsValidDataMatrixCode(code))
                {
                    return Tuple.Create("400","Formato de código no admitido");
                }
                var existingCode = await _context.Codes.Where(c => c.CodeValue == code).SingleOrDefaultAsync();
                if(existingCode.IsConfirmed)
                {
                    return Tuple.Create("400","El código ya fue confirmado");
                }
                if(existingCode != null)
                {
                   existingCode.IsConfirmed = true;
                   existingCode.UserConfirm = user;
                   existingCode.ConfirmDate = DateTime.Now;
                }
                else
                {
                    throw new System.Exception("Código no encontrado");
                }
                await _context.SaveChangesAsync();
                return Tuple.Create("200", existingCode.CodeValue);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Code>> getAvailableOrderCodes(int OrderId)
        {
            var codes = await _context.Codes.Where(c=>c.OrderId == OrderId && c.IsPrinted == false).ToListAsync();
            return codes;
        }
        private bool IsValidDataMatrixCode(string code)
        {
            //Format: 0 + Ean13 (13 digits) + serial (7 chars) + MRP (AAAA) + security code (4 chars)
            return Regex.IsMatch(code, @"^[0][0-9]{13}\S{7}[A]{4}\S{4}$");
        }
    }
    
}