
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
    [Table("Proceso")]
    public class Process: IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public int CodigoPermiso { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaHoraModificacion { get; set; }
        public DateTime? FechaHoraBorrado { get; set; }
        [Required]
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public string UsuarioEliminacion { get; set;}
        public List<Module> Modules { get; set; }
        public bool EstaBorrado { get; set; }
        public List<Production> Productions { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}