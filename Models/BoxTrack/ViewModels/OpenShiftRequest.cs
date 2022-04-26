
namespace BoxTrackLabel.API.Models.BoxTrack
{
    public class OpenShiftRequest
    {
         public int IdProceso { get; set; }
         public int IdModulo { get; set; }   
         public string UsuarioApertura { get; set; }
         public int DataMatrixOrderId { get; set; }
    }
   
}