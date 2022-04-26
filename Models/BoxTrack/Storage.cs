using BoxTrackLabel.API.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Models
{
    [Table("Almacenamiento")]
    public class Storage:IEntity
    {
        public int Id { get; set; }
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public bool EstaBorrado { get; set; }
        public DateTime? FechaHoraBorrado { get; set; }
        public DateTime? FechaHoraModificacion { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraRegistro { get; set; } = DateTime.Now;
        [Required]
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public string UsuarioBorrado { get; set; }

    }
}
