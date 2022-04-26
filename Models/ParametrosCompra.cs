using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Models
{
    public class ParametrosCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad_producto { get; set; }
    }
}
