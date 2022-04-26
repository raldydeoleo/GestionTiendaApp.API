using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
   public class OrderInfo
   {
      public string orderId { get; set; }
      public string orderStatus { get; set; } 
      public List<Buffer> buffers { get; set; }
      public long createdTimestamp { get; set; }
      public string declineReason { get; set; }
   }
}