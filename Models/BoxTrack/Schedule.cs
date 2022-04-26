using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("Programacion")]
    public class Schedule
    {
        public int Id { get; set; }
        [Column(TypeName = "Date")]
        public DateTime FechaProduccion { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        [ForeignKey("Process")]
        public int IdProceso { get; set; }
        public Process Process { get; set; }
        [ForeignKey("Module")]
        public int IdModulo { get; set; }
        public Module Module { get; set; }
        [ForeignKey("Shift")]
        public int IdTurno { get; set; }
        public Shift Shift { get; set; }
        public string IdProducto { get; set; }
        [NotMapped]
        public Product Product { get; set; }
        public string UsuarioProgramacion { get; set; }
        public bool Finalizado { get; set; }
        public string UsuarioFinalizado { get; set; }
        public DateTime? FechaHoraFinalizado { get; set; }

    }
}