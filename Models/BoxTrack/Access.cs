using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Models
{
    [Table("Acceso")]
    public class Access
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public DateTime FechaHoraAcceso { get; set; }
        public DateTime FechaHoraRefresh { get; set; }
    }
}
