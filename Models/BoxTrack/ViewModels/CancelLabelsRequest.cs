using System.Collections.Generic;
namespace BoxTrackLabel.API.Models.BoxTrack
{
    public class CancelLabelsRequest
    {
        public List<Label> Labels { get; set; }
        public string UsuarioAnulacion { get; set; }
    }
}