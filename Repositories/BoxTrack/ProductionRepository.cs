
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Models.BoxTrack;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ProductionRepository 
    {
        private readonly BoxTrackDbContext _context;
        public ProductionRepository(BoxTrackDbContext context)
        {
             _context = context;
        }
        /// <summary>
        /// Inserta un registro nuevo de producci�n en la base de datos BoxTrackLabel 
        /// </summary>
        public async Task<Production> Add(Production production)
        {
            _context.Productions.Add(production);
            await _context.SaveChangesAsync();
            return production;
        }
        /// <summary>
        /// Cierra una producci�n de un determinado turno, as� como las producciones de productos finalizados en el mismo turno.
        /// </summary>
        public async Task<Production> CloseProduction(Production production)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var closeDate = DateTime.Now;
                /*var scheduleProduct = await GetScheduleProduct(production);
                if(scheduleProduct != null)
                {
                    scheduleProduct.Finalizado = true;
                    scheduleProduct.FechaHoraFinalizado = closeDate;
                    scheduleProduct.UsuarioFinalizado = production.UsuarioCierreTurno;
                    _context.Entry(scheduleProduct).State = EntityState.Modified;
                }*/
                //Finalizamos todas las programaciones   s.IdProceso == production.IdProceso &&
                await _context.Schedules.Where(s =>  s.IdTurno == production.IdTurno && s.FechaProduccion == production.FechaProduccion && s.Finalizado == false)
                    .BatchUpdateAsync(p => new Schedule { Finalizado = true, FechaHoraFinalizado = closeDate, UsuarioFinalizado = production.UsuarioCierreTurno });

                //Cerrar la producci�n del turno en todos los registros de producci�n incluyendo donde se hizo un change over    p.IdProceso == production.IdProceso && 
                await _context.Productions.Where(p => p.IdTurno == production.IdTurno && p.FechaProduccion == production.FechaProduccion && p.IdProceso == production.IdProceso) //&& p.ProductoFinalizado == true
                      .BatchUpdateAsync(p => new Production { FechaHoraCierreTurno = closeDate, UsuarioCierreTurno = production.UsuarioCierreTurno, TurnoAbierto = false, ProductoFinalizado = true}); //,   && p.IdModulo == production.IdModulo
                //production.FechaHoraCierreTurno = closeDate;
                //production.TurnoAbierto = false;
                //production.ProductoFinalizado = true;
                //_context.Entry(production).State = EntityState.Modified;
                //await _context.SaveChangesAsync();
                transaction.Commit();
                return production;
            }
        }
        /// <summary>
        /// Obtiene la producción del turno, fecha de producción y módulo de empaque especificado
        /// </summary>
        public async Task<Production> GetProduction(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion, Boolean isOpenShift)
        {
            var production = _context.Productions.Where(p=>p.IdProceso == idProceso && p.IdModulo == idModulo && p.IdTurno == idTurno && p.FechaProduccion == fechaProduccion && p.TurnoAbierto == isOpenShift && p.DataMatrixOrderId == 0).Include(s=>s.Shift).Include(l=>l.Labels).LastOrDefaultAsync();
            if(production.Result != null)
            {
                if(production.Result.Labels != null)
                {
                    production.Result.Labels = production.Result.Labels.OrderBy(l=>l.Id).ToList();
                }
            }
            return await production;
             
        }
        /// <summary>
        /// Obtiene la producción especificada validando si el producto está finalizado o no 
        /// </summary>
        public async Task<Production> GetProductionByChangeOver(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion, Boolean isChangeOver)
        {
            var production = _context.Productions.Where(p => p.IdProceso == idProceso && p.IdModulo == idModulo && p.IdTurno == idTurno && p.FechaProduccion == fechaProduccion && p.ProductoFinalizado == isChangeOver  && p.DataMatrixOrderId == 0).Include(s => s.Shift).Include(l => l.Labels).LastOrDefaultAsync();
            if (production.Result != null)
            {
                if (production.Result.Labels != null)
                {
                    production.Result.Labels = production.Result.Labels.OrderBy(l => l.Id).ToList();
                }
            }
            return await production;

        }
        /// <summary>
        /// Obtiene la producción mediante su id
        /// </summary>
        public async Task<Production> GetProductionById(int idProduction)
        {
            var production =  _context.Productions.Where(p=>p.Id == idProduction).Include(s=>s.Shift).Include(l=>l.Labels).FirstOrDefaultAsync();
            if(production.Result != null)
            {
                if(production.Result.Labels != null)
                {
                    production.Result.Labels = production.Result.Labels.OrderBy(l=>l.Id).ToList();
                }
            }
            return await production;
        }
        /// <summary>
        /// Obtiene el último registro de impresión  por producto y turno
        /// </summary>
        public async Task<Production> GetLastProductionByProduct(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion, string idProducto)
        {
            var production = _context.Productions.Where(p=>p.IdProceso == idProceso && p.IdModulo == idModulo && p.IdTurno == idTurno && p.FechaProduccion == fechaProduccion && p.IdProducto == idProducto  && p.DataMatrixOrderId == 0 && p.ProductoFinalizado == true).Include(s=>s.Shift).Include(l=>l.Labels).LastOrDefaultAsync();
            if(production.Result != null)
            {
                if(production.Result.Labels != null)
                {
                    production.Result.Labels = production.Result.Labels.OrderBy(l=>l.Id).ToList();
                }
            }
            return await production;
        }
        /// <summary>
        /// Obtiene los registros de impresión  por producto y turno
        /// </summary>
        public async Task<List<Label>> GetProductionsByProduct(int? idProceso, int? idModulo, int? idTurno, DateTime fechaProduccion, string idProducto)
        {
            var productionsBaseQuery = _context.Productions.Where(p=>p.FechaProduccion == fechaProduccion  && p.DataMatrixOrderId == 0 && p.ProductoFinalizado == true);
            var productionsQuery = productionsBaseQuery;
            if(idProducto != string.Empty && idProducto != "null")
            {
                productionsQuery = productionsQuery.Where(p=>p.IdProducto == idProducto);
            }
            if(idProceso != null)
            {
                productionsQuery = productionsQuery.Where(p=>p.IdProceso == idProceso);
            }
            if(idModulo != null)
            {
                productionsQuery = productionsQuery.Where(p=>p.IdModulo == idModulo);
            }
            if(idTurno != null)
            {
                productionsQuery = productionsQuery.Where(p=>p.IdTurno == idTurno);
            }
            var productions = await productionsQuery.Include(s=>s.Shift).Include(m => m.Module).Include(p => p.Process).Include(l=>l.Labels).ThenInclude(l=>l.LabelConfig).ToListAsync();
            List<Label> labels = new List<Label>();
            if(productions.Count > 0)
            {   
                foreach (var prod in productions)
                {
                    if(prod.Labels != null)
                    {
                        //labels.AddRange(prod.Labels);
                        var reprinted = prod.Labels.Where(l => l.EsReimpresion).ToList();
                        
                        prod.Labels = prod.Labels.Where(l => !l.EsReimpresion).ToList();
                        foreach (var label in prod.Labels)
                        {
                            var reprintToSum = reprinted.Where(r => r.IdEtiquetaReimpresa == label.Id).ToList();
                            var reprintedSum = reprintToSum.Sum(r => r.CantidadImpresa);
                            label.TotalReimpresiones = reprintedSum; 
                            label.CodigoSap = prod.IdProducto;
                            label.LabelConfig.Labels = null; 
                            label.Production.Shift = null;
                            label.Production.Labels = null;
                            label.Production.Module.Process = null;
                            label.Production.Process.Modules = null;
                            label.IdModulo = prod.IdModulo;
                            label.IdTurno = prod.IdTurno;
                            labels.Add(label);
                        }
                    }
                }
                if(labels.Count() > 0)
                {
                    labels = labels.OrderByDescending(l => l.FechaHoraCalendario).ToList();
                }
                // foreach (var prod in productions)
                // {
                //     if(prod.Labels != null)
                //     {
                //         var reprinted = prod.Labels.Where(l => l.EsReimpresion).ToList();
                //         var reprintedSum = reprinted.Sum(r => r.CantidadImpresa);
                //         prod.Labels = prod.Labels.Where(l => !l.EsReimpresion).OrderBy(l=>l.Id).ToList();
                //         var printedSum = prod.Labels.Sum(l => l.CantidadImpresa); 
                //         prod.Labels = prod.Labels.Take(1).ToList();   
                //         prod.Labels.ForEach( l=>{l.CodigoSap = prod.IdProducto;  l.IdModulo = prod.IdModulo; l.IdTurno = prod.IdTurno; l.TotalReimpresiones = reprintedSum; l.CantidadImpresa = printedSum; l.LabelConfig.Labels = null; l.Production.Shift = null;});
                //         labels.AddRange(prod.Labels);
                //     }
                // }
            }
            return labels;
        }
        public async Task<Production> Update(Production production)
        {
            _context.Productions.Update(production);
            await _context.SaveChangesAsync();
            return production;
        }
        /// <summary>
        /// Verifica si el producto está programado y puede utilizarse para generar etiquetas
        /// </summary>
        public async Task<bool> CanScanProduct(ProductRegistrationRequest req)
        {
            var schedule = await _context.Schedules.Where(s=>s.IdProceso == req.Produccion.IdProceso && s.IdModulo == req.Produccion.IdModulo && s.IdTurno == req.Produccion.IdTurno && s.IdProducto == req.Produccion.IdProducto && s.FechaProduccion == req.Produccion.FechaProduccion).SingleOrDefaultAsync();
            if(schedule != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Busca el registro de programación del producto para los parámetros dados 
        /// </summary>
        public async Task<Schedule> GetScheduleProduct(Production production)
        {
            var schedule = await _context.Schedules.Where(s => s.IdProceso == production.IdProceso && s.IdModulo == production.IdModulo && s.IdTurno == production.IdTurno && s.FechaProduccion == production.FechaProduccion && s.Finalizado == false).LastOrDefaultAsync();
            return schedule;
        }
        /// <summary>
        /// Detecta si ha ocurrido un cambio de producto y finaliza el registro de producción anterior y genera uno nuevo para el producto actual
        /// </summary>
        public async Task<Production> ChangeOverProduction(Production production, Schedule schedule)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                //si solo existe un solo schedule para ese turno solo modificar el producto, no crear otra producción.
                var schedules = await _context.Schedules.Where(s => s.IdProceso == production.IdProceso && s.IdModulo == production.IdModulo && s.IdTurno == production.IdTurno && s.FechaProduccion == production.FechaProduccion).ToListAsync();
                Production resultProduction;
                if(schedules.Count > 1)
                {
                    production.ProductoFinalizado = true;
                    await Update(production);
                    var newProduction = new Production
                    {
                        IdProceso = production.IdProceso,
                        IdModulo = production.IdModulo,
                        IdTurno = production.IdTurno,
                        IdProducto = schedule.IdProducto,
                        TurnoAbierto = true,
                        FechaHoraAperturaTurno = production.FechaHoraAperturaTurno,
                        UsuarioAperturaTurno = production.UsuarioAperturaTurno,
                        FechaProduccion = production.FechaProduccion
                    };
                    resultProduction = await Add(newProduction);
                }
                else
                {
                    production.IdProducto = schedule.IdProducto;
                    resultProduction = await Update(production);
                }
                
                transaction.Commit();
                return resultProduction;
            }
        }

        public async Task<Production> GetProductionByOrder(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion, Boolean isOpenShift, int orderId)
        {
            var production = _context.Productions.Where(p=>p.IdProceso == idProceso && p.IdModulo == idModulo && p.IdTurno == idTurno && p.FechaProduccion == fechaProduccion && p.TurnoAbierto == isOpenShift && p.DataMatrixOrderId == orderId).Include(s=>s.Shift).Include(l=>l.Labels).LastOrDefaultAsync();
            if(production.Result != null)
            {
                if(production.Result.Labels != null)
                {
                    production.Result.Labels = production.Result.Labels.OrderBy(l=>l.Id).ToList();
                }
            }
            return await production;
        }
        public async Task<Production> GetProductionByOrderId(int orderId)
        {
            var production = await _context.Productions.Where(p => p.DataMatrixOrderId == orderId && p.TurnoAbierto == true).Include(s=>s.Shift).Include(l=>l.Labels).SingleOrDefaultAsync();
            return production;
        }
    }
}