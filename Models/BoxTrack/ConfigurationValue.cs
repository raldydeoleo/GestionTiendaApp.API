using BoxTrackLabel.API.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Models
{
    [Table("ConfiguracionValor")]
    public class ConfigurationValue: IEntity
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string TextoConfiguracion { get; set; }
        public string ValorConfiguracion { get; set; }
        public bool EstaBorrado { get; set; }
        public DateTime? FechaHoraBorrado { get; set; }
        public DateTime? FechaHoraModificacion { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaHoraRegistro { get; set; } = DateTime.Now;
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public string  UsuarioEliminacion { get; set; }
    }
}
