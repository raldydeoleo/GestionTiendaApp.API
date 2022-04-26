using System.Collections.Generic;

namespace BoxTrackLabel.API.Models
{
   public class OrderAvailabilityResponse
   {
      public string omsId { get; set; }
      public List<OrderInfo> orderInfos { get; set; } 
   }
}