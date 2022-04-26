using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BoxTrackLabel.API.Repositories;
using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
    [Table("Modulo")]
    public class Module: IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaHoraModificacion { get; set; }
        public DateTime? FechaHoraBorrado { get; set; }
        [Required]
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public string UsuarioEliminacion { get; set;}
        [ForeignKey("Process")]
        public int IdProceso { get; set; }
        public Process Process { get; set; }
        public bool EstaBorrado { get; set; }
        public string NumeroModulo { get; set; }
        public string TextoModulo { get; set; }
        public List<Production> Productions { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}