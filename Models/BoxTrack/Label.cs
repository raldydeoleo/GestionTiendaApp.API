using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("Etiqueta")]
    public class Label
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Production")]
        public int ProduccionId { get; set; }
        public Production Production { get; set; }
        public string  CodigoBarra { get; set; }
        public string CodigoQr { get; set; }
        public string DescripcionProducto { get; set; }
        public int CantidadCigarros { get; set; }
        public int SecuenciaInicial { get; set; } 
        public int CantidadImpresa { get; set; }
        public DateTime FechaHoraCalendario { get; set; } 
        public string Almacenamiento { get; set; }
        [ForeignKey("LabelConfig")]
        public int ConfiguracionEtiquetaId { get; set; }
        public LabelConfig LabelConfig { get; set; }
        [NotMapped]
        public List<Code> DataMatrixCodes { get; set; }
        [NotMapped]
        public string CodigoSap { get; set; }
        [NotMapped]
        public int? IdModulo { get; set; }
        [NotMapped]
        public int? IdTurno { get; set; }
        [NotMapped]
        public int TotalReimpresiones { get; set; }
        public string UsuarioGeneracion { get; set; }
        public bool EsReimpresion { get; set; }
        public int IdEtiquetaReimpresa { get; set; }
    }
}