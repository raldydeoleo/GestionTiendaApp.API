
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models.BoxTrack
{
    public class PrintLabelRequest
    {
        public int IdModulo {get; set;}
        public int IdProceso {get; set;} 
        public string IdAlmacenamiento {get; set;}
        public string IdProducto {get; set;}
        public string DescripcionProducto {get; set;}
        public int CantidadEtiquetas {get; set;}
        public int CantidadCigarros { get; set; }
        public string NumeroModulo { get; set; }
        public string TextoModulo { get; set; }
        public string Almacenamiento { get; set; }
        public string CodigoEan { get; set; }
        public string Centro { get; set; }
        public decimal PesoNeto { get; set; }
        public string UnidadPeso { get; set; }
        public string Usuario { get; set; }
        public int IdProduccion { get; set; }
        public Customer Cliente {get; set;}
        public bool EsUsa { get; set; }
        public string TipoEtiqueta { get; set; }
        public bool lleva_Logo_TextoInferior { get; set; }
        public int DataMatrixOrderId { get; set; }
        public bool EsReimpresion { get; set; }
        public DateTime FechaHoraReimpresion { get; set; }
        public int IdEtiquetaReimpresa { get; set; }
    }
}