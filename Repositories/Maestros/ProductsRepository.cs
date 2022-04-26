using System.Collections.Generic;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BoxTrackLabel.API.Repositories
{
    public class ProductsRepository 
    {
        private readonly MaestrosDbContext _context;
        public ProductsRepository(MaestrosDbContext context)
        {
             _context = context;
        }
        /// <summary>
        /// Obtiene el listado de todos los productos terminados
        /// </summary>
        public async Task<List<Product>> GetAll()
        {
            //return await _context.Products.FromSql("select * from V_ProductosTerminadosEan").ToListAsync(); //select top 500 * from V_ProductosTerminadosEan
            return await _context.Products.FromSql(GetSqlCmd()).ToListAsync();
        }
        /// <summary>
        /// Obtiene un producto terminado mediante su c�digo
        /// </summary>
        public async Task<Product> Get(string codigo)
        {
            // await _context.Products.FromSql("select * from V_ProductosTerminadosEan where Material ='"+codigo+"'").SingleOrDefaultAsync();
            return await _context.Products.FromSql(GetSqlCmd(codigo)).SingleOrDefaultAsync();
        }
        /// <summary>
        /// Obtiene el listado de todos los productos terminados que contengan el texto del par�metro name dentro de su descripci�n o que su c�digo sea igual a dicho par�metro.
        /// </summary>
        public async Task<List<Product>> GetByName(string name)
        {
            //return await _context.Products.FromSql("select * from V_ProductosTerminadosEan where [Texto breve de material] like '%" + name + "%' or Material = '"+name+"'").ToListAsync();
            return await _context.Products.FromSql(GetSqlCmd(null, name)).ToListAsync();
        }
        private string GetSqlCmd(string codigo = null, string name = null)
        {
            var filter = "";
            if(codigo != null)
            {
                filter = " AND (Material ='"+codigo+"')";
            }
            else if(name != null)
            {
                filter = " AND ([Texto breve de material] like '%" + name + "%' or Material = '"+name+"')";
            }
            var tempCigarsCodesCmd = @"
                 declare @productoTerminadosEanIndividuales table(Material nvarchar(25),CodEanUpc nvarchar(18), UMB nvarchar(3), IdUnidadMedidaInterna nvarchar(3))
                 insert into @productoTerminadosEanIndividuales
                 SELECT  dbo.V_ProductosTerminados.Material, dbo.MaterialEan.CodEanUpc, dbo.V_ProductosTerminados.UMB, dbo.MaterialEan.IdUnidadMedidaInterna
                 FROM    dbo.V_ProductosTerminados LEFT OUTER JOIN
                         dbo.MaterialEan ON CASE WHEN len(dbo.MaterialEan.IdMaterial) >= 18 
                                            THEN substring(dbo.MaterialEan.IdMaterial, 10, 9) 
                                            ELSE dbo.MaterialEan.IdMaterial END = dbo.V_ProductosTerminados.Material
                 WHERE   (dbo.V_ProductosTerminados.UMB = 'TH' OR dbo.V_ProductosTerminados.UMB = 'ST') AND (dbo.MaterialEan.IdUnidadMedidaInterna = N'ST') 
                         AND (dbo.V_ProductosTerminados.Centro = 'DO22')";

            var materialWithCodesCmd = @"
                SELECT  dbo.V_ProductosTerminados.Material, dbo.MaterialEan.CodEanUpc, dbo.V_ProductosTerminados.[Texto breve de material], 
                        dbo.V_ProductosTerminados.NºMat, dbo.V_ProductosTerminados.DenominStd, 
                        dbo.V_ProductosTerminados.DIN, dbo.V_ProductosTerminados.TpMt, dbo.V_ProductosTerminados.UMB, dbo.V_ProductosTerminados.Centro, 
                        CASE WHEN dbo.V_ProductosTerminados.UMB = 'TH' 
                        THEN dbo.V_ProductosTerminados.[Items Per Box] * 1000 
                        ELSE dbo.V_ProductosTerminados.[Items Per Box] END AS [Items Per Box], 
                        CASE WHEN dbo.V_ProductosTerminados.UMB = 'TH' 
                        THEN dbo.V_ProductosTerminados.[Items Per CS] * 1000 
                        ELSE dbo.V_ProductosTerminados.[Items Per CS] END AS [Items Per CS], dbo.V_ProductosTerminados.PesoNeto, 
                        dbo.V_ProductosTerminados.UnidadPeso, dbo.MaterialEan.IdUnidadMedidaInterna,
                        (select top 1 CodEanUpc from @productoTerminadosEanIndividuales as p where p.Material = dbo.V_ProductosTerminados.Material) as CodigoSku
                FROM    dbo.V_ProductosTerminados LEFT OUTER JOIN
                        dbo.MaterialEan ON CASE WHEN len(dbo.MaterialEan.IdMaterial) >= 18 
                                           THEN substring(dbo.MaterialEan.IdMaterial, 10, 9) 
                                           ELSE dbo.MaterialEan.IdMaterial END = dbo.V_ProductosTerminados.Material
                WHERE        (dbo.V_ProductosTerminados.UMB = 'TH' OR
                                        dbo.V_ProductosTerminados.UMB = 'ST') AND (dbo.MaterialEan.IdUnidadMedidaInterna = N'BX' OR
                                        dbo.MaterialEan.IdUnidadMedidaInterna IS NULL) AND (dbo.V_ProductosTerminados.Centro = 'DO22')";
            return tempCigarsCodesCmd + materialWithCodesCmd + filter;
            /*if(codigo != null)
            {
                filter = "where (Material ='"+codigo+"')";
            }
            else if(name != null)
            {
                filter = "where ([Texto breve de material] like '%" + name + "%' or Material = '"+name+"')";
            }
            return "select * from ProductosTerminadosTemp"+filter;*/
        }
    }
}