using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("ZUsuario")]
    public class User
    {
        [Key]
        public string NombreUsuario { get; set; }
        public int CodigoEmpleado { get; set; }
        public string Nombre { get; set; }
        public int IdRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeSpan HoraRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime ValidezClave { get; set; }
        public string ClaveAcceso { get; set; }
    }
}
