using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
    [Table("Suplidores")]
    public class Suplidores
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Nombre no puede tener mas de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "RNC es requirido")]
        public string RNC { get; set; }

        [Required(ErrorMessage = "Direccion es requirida")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Telefono es requirido")]
        public string Telefono { get; set; }

    }
}
