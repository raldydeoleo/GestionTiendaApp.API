using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
    [Table("Turno")]
    public class Shift: IEntity
    {
        
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaHoraModificacion { get; set; }
        public DateTime? FechaHoraBorrado { get; set; }
        [Required]
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public string UsuarioBorrado { get; set;}
        public bool EstaBorrado { get; set; }
        [Required]
        public string LetraRepresentacion { get; set; }
        public List<Production> Productions { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}