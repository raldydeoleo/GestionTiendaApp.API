
using System.ComponentModel.DataAnnotations;

namespace BoxTrackLabel.API.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Clave { get; set; }
    }
}
