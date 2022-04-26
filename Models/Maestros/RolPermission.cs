using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace BoxTrackLabel.API.Models
{
    [Table("ZDeterminacionRoles")]
    public class RolPermission
    {
        public int IdRol { get; set; }
        [ForeignKey("Permission")]
        public int IdPermiso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeSpan HoraRegistro { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
