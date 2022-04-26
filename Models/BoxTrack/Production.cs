using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoxTrackLabel.API.Models
{
    [Table("Produccion")]
    public class Production
    {
        public int Id { get; set; }
        public int DataMatrixOrderId { get; set; }
        [ForeignKey("Process")]
        public int  IdProceso { get; set; }
        public Process Process { get; set; }
        [ForeignKey("Shift")]
        public int IdTurno { get; set; }
        public Shift Shift { get; set; }
        [ForeignKey("Module")]
        public int IdModulo { get; set; }
        public Module Module { get; set; }
        public string IdProducto { get; set; }
        public DateTime? FechaHoraCierreTurno { get; set; } 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraAperturaTurno { get; set; }  = DateTime.Now;
        [Required]
        public string UsuarioAperturaTurno{ get; set; }
        public string UsuarioCierreTurno { get; set; }
        public bool TurnoAbierto { get; set; }
        public List<Label>  Labels { get; set; }
        [Column(TypeName = "Date")]
        public DateTime FechaProduccion { get; set; }
        public bool ProductoFinalizado { get; set; }
    }
}