
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("MaestroCliente")]
    public class Customer
    {
        [Key]
        public string IdCliente { get; set; }
        public string IdPais { get; set; }
        public string Nombre1 { get; set; }
    }
}
       