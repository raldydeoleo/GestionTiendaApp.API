using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("ZPermisos")]
    public class Permission
    {
        [Key]
        public int IdPermiso { get; set; }
        public string Permiso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeSpan HoraRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public List<RolPermission> RolsPermissions { get; set; }
    }
}
