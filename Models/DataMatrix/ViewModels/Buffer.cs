using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
   public class Buffer
   {
      public int leftInBuffer { get; set; }
      public bool poolsExhausted { get; set; } 
      public string bufferStatus { get; set; }
      public int totalCodes { get; set; }
      public int unavailableCodes { get; set; }
      public int availableCodes { get; set; }
      public string orderId { get; set; }
      public string gtin { get; set; }
      public int totalPassed { get; set; }
      public string omsId { get; set; }
   }
}