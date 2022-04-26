using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
    [Table("Productos")]
    public class Productos
    {        
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Nombre no puede tener mas de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Cantidad es requirida")]
        public int? Cantidad { get; set; }

        [Required(ErrorMessage = "Precio es requirido")]
        public double? Precio { get; set; }
        
        public ICollection<Suplidores> suplidores { get; set; }
    } 
}
