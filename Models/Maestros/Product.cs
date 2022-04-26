
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    
    [NotMapped]
    public class Product
    {
        [Key]
        [Column("Material")]
        public string CodigoMaterial { get; set; }
        [Column("CodEanUpc")]
        public string CodigoEan { get; set; }
        [Column("Texto breve de material")]
        public string Descripcion { get; set; }
        public string Centro { get; set; }
        [Column("Items Per Box")]
        public double? CigarrosPorCaja { get; set; }
         [Column(TypeName = "decimal(13, 3)")]
        public decimal? PesoNeto { get; set; }
        public string UnidadPeso { get; set; }
        [Column("CodigoSku")]
        public string CodigoEanCigarro { get; set; }
    }
}