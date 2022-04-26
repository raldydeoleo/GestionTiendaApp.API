using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Repositories
{
    public class ScheduleRepository 
    {
        private readonly BoxTrackDbContext _context;
        private readonly ProductsRepository productsRepository;

        public ScheduleRepository(BoxTrackDbContext context, ProductsRepository productsRepository)
        {
             _context = context;
            this.productsRepository = productsRepository;
        }
        /// <summary>
        /// Obtiene un listado de programaciones de producto, de acuerdo al m�dulo de empaque, la fecha de producci�n y el estatus.
        /// </summary>
        public async Task<List<Schedule>> GetSchedulesByDate(DateTime fechaProduccion, string estatus, string idproceso, string idmodulo)
        {
            List<Schedule> schedulesResult = new List<Schedule>();
            if (estatus == "todos")
            {
                schedulesResult = await _context.Schedules.Where(s => s.FechaProduccion == fechaProduccion).ToListAsync();
            }
            else 
            {
                var isEnded = estatus == "finalizados";
                schedulesResult = await _context.Schedules.Where(s => s.FechaProduccion == fechaProduccion && s.Finalizado == isEnded).ToListAsync();
            }
            var products = await productsRepository.GetAll();
            var processes = await _context.Processes.AsNoTracking<Process>().ToListAsync();
            var modules = await _context.Modules.AsNoTracking<Module>().ToListAsync();
            var shifts = await _context.Shifts.AsNoTracking<Shift>().ToListAsync();
            var schedulesResultArray = schedulesResult.Select(
                s => { 
                    s.Product = products.FirstOrDefault(p=>p.CodigoMaterial == s.IdProducto);
                    s.Process = processes.FirstOrDefault(p=>p.Id == s.IdProceso);
                    s.Module = modules.FirstOrDefault(m=>m.Id == s.IdModulo);
                    s.Shift = shifts.FirstOrDefault(t=>t.Id == s.IdTurno);
                    return s; 
                });
            var schedulesResultList = schedulesResultArray.Where(result => result != null).ToList();
            schedulesResultList = filterSchedulesByModule(schedulesResultList, idproceso, idmodulo);
            return schedulesResultList;
        }
        /// <summary>
        /// Devuelve una lista de programaciones filtradas, de acuerdo al proceso y m�dulo de empaque.
        /// </summary>
        private List<Schedule> filterSchedulesByModule(List<Schedule> schedules, string idproceso, string idmodulo)
        {
            var resultList = schedules;
            if(!string.IsNullOrEmpty(idproceso)) 
            {
                int procesoId = int.Parse(idproceso);
                resultList = resultList.Where(s => s.IdProceso == procesoId).ToList();
                if(!string.IsNullOrEmpty(idmodulo))
                {
                    int moduloId = int.Parse(idmodulo);
                    resultList = resultList.Where(s => s.IdModulo == moduloId).ToList();
                }
            }
            return resultList;
        }
        /// <summary>
        /// Inserta una programaci�n de productos en la base de datos de BoxTrackLabel
        /// </summary>
        public async Task<Schedule> CreateSchedule(Schedule schedule)
        {
             _context.Schedules.Add(schedule);
             await _context.SaveChangesAsync();
             return schedule;
        }
        /// <summary>
        /// Actualiza una programaci�n de productos en la base de datos de BoxTrackLabel
        /// </summary>
        public async Task<Schedule> UpdateSchedule(Schedule schedule)
        {
            schedule.Process = null;
            schedule.Module = null;
            schedule.Shift = null;
             _context.Schedules.Update(schedule);
             await _context.SaveChangesAsync();
             return schedule;
        }
        /// <summary>
        /// Inserta un listado de programaciones de productos en la base de datos de BoxTrackLabel
        /// </summary>
        public async Task<bool> InsertScheduleList(List<Schedule> schedules)
        {
                await _context.BulkInsertAsync(schedules);
                return true;
        }
        /// <summary>
        /// Determina si ya existe una programaci�n con otro producto en el mod�lo de empaque para el turno y la fecha de producci�n dada.
        /// </summary>
        public async Task<bool> ScheduleExists(DateTime fechaProduccion, int idProceso, int idModulo, int idTurno, string idProducto)
        {
            var schedule = await _context.Schedules.Where(s => s.FechaProduccion == fechaProduccion && s.IdProceso == idProceso && s.IdModulo == idModulo && s.IdTurno == idTurno && s.IdProducto != idProducto && s.Finalizado == false ).FirstOrDefaultAsync();
            if(schedule != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determina si ya existe una programaci�n con el mismo producto en el mod�lo de empaque para el turno y la fecha de producci�n dada.
        /// </summary>
        public async Task<bool> SameProductScheduleExists(DateTime fechaProduccion, int idProceso, int idModulo, int idTurno, string idProducto)
        {
            var schedule = await _context.Schedules.Where(s => s.FechaProduccion == fechaProduccion && s.IdProceso == idProceso && s.IdModulo == idModulo && s.IdTurno == idTurno && s.IdProducto == idProducto && s.Finalizado == false).FirstOrDefaultAsync();
            if (schedule != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determina si ya existe una programaci�n en el m�dulo de cualquier producto, tomando en cuenta como siempre, el turno y la fecha de producci�n. 
        /// </summary>
        public async Task<bool> ScheduleExistsOnShift(DateTime fechaProduccion, int idProceso, int idModulo, int idTurno)
        {
            var schedule = await _context.Schedules.Where(s => s.FechaProduccion == fechaProduccion && s.IdProceso == idProceso && s.IdModulo == idModulo && s.IdTurno == idTurno && s.Finalizado == false).FirstOrDefaultAsync();
            if (schedule != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Trata de realizar un cambio de programaci�n, finalizando la anterior y insertanto una nueva, retorna de verdadero si la operaci�n es completada con �xito.
        /// </summary>
        public async Task<bool> TryChangeSchedule(Schedule schedule)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var scheduleToclose = await _context.Schedules.Where(s => s.FechaProduccion == schedule.FechaProduccion && s.IdProceso == schedule.IdProceso && s.IdModulo == schedule.IdModulo && s.IdTurno == schedule.IdTurno && s.Finalizado == false).FirstOrDefaultAsync();
                    scheduleToclose.Finalizado = true;
                    scheduleToclose.FechaHoraFinalizado = DateTime.Now;
                    scheduleToclose.UsuarioFinalizado = schedule.UsuarioProgramacion;
                    _context.Update(scheduleToclose);
                    if(schedule.Id != 0)
                    {
                        var newSchedule = new Schedule { IdProceso = schedule.IdProceso, IdModulo = schedule.IdModulo, FechaProduccion = schedule.FechaProduccion, IdTurno = schedule.IdTurno = schedule.IdTurno, IdProducto = schedule.IdProducto, UsuarioProgramacion = schedule.UsuarioProgramacion };
                        _context.Schedules.Add(newSchedule);
                    }
                    else
                    {
                        _context.Schedules.Add(schedule);
                    }
                    
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Determina si la programaci�n se puede editar verificando si ya se imprimieron etiquetas, si la programaci�n activa contiene etiquetas no se deben permitir modificaciones.
        /// </summary>
        public async Task<bool> CanEditSchedule(Schedule schedule)
        {   
            var production = await _context.Productions.Where(p => p.IdProceso == schedule.IdProceso && p.IdModulo == schedule.IdModulo && p.IdTurno == schedule.IdTurno  && p.ProductoFinalizado != true && p.TurnoAbierto == true && p.FechaProduccion == schedule.FechaProduccion).Include(l=>l.Labels).FirstOrDefaultAsync();
            if(production != null)
            {
                if(production.Labels != null)
                {
                    if(production.Labels.Count > 0)
                    {
                        var labelsWithScheduleProduct = from l in production.Labels where l.CodigoQr.Contains(production.IdProducto) select l;  
                        try
                        {
                            if (labelsWithScheduleProduct.Count() > 0)
                            {
                                return false;
                            }
                        }
                        catch (System.NullReferenceException)
                        {
                            labelsWithScheduleProduct = from l in production.Labels where l.Production.IdProducto == production.IdProducto select l; 
                            if (labelsWithScheduleProduct.Count() > 0)
                            {
                                return false;
                            }
                        }
                        
                    }
                }
            }
            return true;
        }



    }
}