using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace BoxTrackLabel.API.Models
{
    [Table("ConfiguracionEtiqueta")]
    public class LabelConfig
    {
        public int Id { get; set; }
        public string IdPais { get; set; }
        public string ClienteEspecifico { get; set; }
        public string Direccion { get; set; }
        public string TextoCantidad { get; set; }
        public string Advertencia { get; set; }
        public string TextoPais { get; set; }
        public List<Label> Labels { get; set; }
        public string TipoEtiqueta { get; set; }
        public bool LlevaLogo { get; set; }
        public bool LlevaTextoInferior { get; set; }
    }
}