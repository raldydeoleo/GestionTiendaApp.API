using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
   public class CodeResponse
   {
      public string omsId { get; set; }
      public string[] codes { get; set; } 
      public string blockId { get; set; }
   }
}