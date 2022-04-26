using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
   public class BlocksResponse
   {
      public string omsId  { get; set; }
      public string gtin { get; set; } 
      public string orderId { get; set; }
      public List<Block> Blocks { get; set; }
   }
}