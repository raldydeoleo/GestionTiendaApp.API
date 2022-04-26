
namespace BoxTrackLabel.API.Models.BoxTrack
{
    public class ProductRegistrationRequest
    {
        public Production Produccion { get; set; }
        public string Usuario { get; set; }
        public string ProductoPrevio { get; set; }
    }
}