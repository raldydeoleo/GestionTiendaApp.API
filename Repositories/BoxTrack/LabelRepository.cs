using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace BoxTrackLabel.API.Repositories
{
    public class LabelRepository 
    {
        private readonly BoxTrackDbContext _context;

        public LabelRepository(BoxTrackDbContext context)
        {
             _context = context;
        }
        /// <summary>
        /// Inserta un registro de etiquetas impresas
        /// </summary>
        public async Task<Label> Add(Label label) 
        {
            _context.Add<Label>(label);
            await _context.SaveChangesAsync();
            return label;
        }
        /// <summary>
        /// Inserta m�ltiples etiquetas a la vez
        /// </summary>
        public async Task<List<Label>> AddRange(List<Label> labels) 
        {
            await _context.BulkInsertAsync(labels);
            return labels;
        }
        /// <summary>
        /// Obtiene las etiquetas no nulas de acuerdo al proceso, módulo, turno y fecha de producción
        /// </summary>
        public async Task<List<Label>> GetLabels(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion)
        {
            return await _context.Labels.Where(l => l.Production.IdProceso == idProceso && l.Production.IdModulo == idModulo && l.Production.IdTurno == idTurno && l.Production.FechaProduccion == fechaProduccion).Include(l=>l.LabelConfig).ToListAsync();
        }
        /// <summary>
        /// Marca como nulas un listado de etiquetas
        /// </summary>
        // public async Task<List<Label>> CancelLabels(List<Label> labels, string usuarioAnulacion)
        // {
        //     foreach (var label in labels)
        //     {
        //         if(label.EsNula == false)
        //         {
        //             label.UsuarioAnulacion = usuarioAnulacion;
        //             label.FechaHoraAnulacion = DateTime.Now;
        //             label.EsNula = true;
        //         }
        //     }
        //     await _context.BulkUpdateAsync(labels); 
        //     //await _context.SaveChangesAsync();
        //     return labels;
        // }
        /// <summary>
        /// Obtiene el texto preconfigurado de la etiqueta dependiendo del país de destino
        /// </summary>
        public async Task<LabelConfig> getLabelText(bool esUsa, string tipoEtiqueta, bool lleva_Logo_TextoInferior) //Customer Cliente
        {
            List<LabelConfig> labelsConfig;
            if(tipoEtiqueta == "Box")
            {
                 labelsConfig = await _context.LabelConfigs.Where(l => l.TipoEtiqueta == tipoEtiqueta && l.LlevaTextoInferior == lleva_Logo_TextoInferior).ToListAsync();
            }
            else
            {
                 labelsConfig = await _context.LabelConfigs.Where(l => l.TipoEtiqueta == tipoEtiqueta && l.LlevaLogo == lleva_Logo_TextoInferior).ToListAsync();
            }
            if(!esUsa)
            {
                 return labelsConfig.Where(l=>l.IdPais == "DO" && l.ClienteEspecifico == null).FirstOrDefault();
            }
            else
            {
                 return labelsConfig.Where(l=>l.IdPais == "US" && l.ClienteEspecifico == null).FirstOrDefault();
            }
            /* if(Cliente == null)
            {
                return labelsConfig.Where(l=>l.IdPais == "DO" && l.ClienteEspecifico == null).FirstOrDefault();
            }
            var labelText = labelsConfig.Where(l=>l.ClienteEspecifico == Cliente.IdCliente).FirstOrDefault();
            if(labelText != null)
            {
                return labelText;
            }
            labelText = labelsConfig.Where(l=>l.IdPais == Cliente.IdPais && l.ClienteEspecifico == null).FirstOrDefault();
            if(labelText == null)
            {
                return labelsConfig.Where(l=>l.IdPais == "DO" && l.ClienteEspecifico == null).FirstOrDefault();
            } 
            return labelsConfig;*/
        }

        internal async Task AddWithDataMatrixOrder(Label labelObject, List<Code> availableCodes, string userName)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                await Add(labelObject);
                availableCodes.ForEach(c=>{c.IsPrinted = true; c.UserPrint = userName; c.LabelId = labelObject.Id; c.PrintDate = DateTime.Now;});
                await _context.BulkUpdateAsync(availableCodes);
                transaction.Commit(); 
                return;
            }
        }

        /// <summary>
        /// Marca como nula una etiqueta mediante su código Qr
        /// </summary>
        // internal async Task<Label> CancelLabelByQr(string qrCode, string usuario)
        // {
        //     var label = await _context.Labels.Where(l => l.CodigoQr == qrCode).FirstOrDefaultAsync();
        //     if(label != null)
        //     {
        //         if(label.EsNula == false)
        //         {
        //             label.UsuarioAnulacion = usuario;
        //             label.FechaHoraAnulacion = DateTime.Now;
        //             label.EsNula = true;
        //             _context.Update(label);
        //             await _context.SaveChangesAsync();
        //         }
        //     }
        //     return label;
        // }
        /*
public async Task<Label> Update(Label label)
{
    label.FechaHoraCierreTurno = DateTime.Now;
   _context.Entry(label).State = EntityState.Modified;
   await _context.SaveChangesAsync();
   return label;
} */
    }
}