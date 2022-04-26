using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
   public class OrderCreationResponse
   {
      public string omsId { get; set; }
      public string orderId { get; set; } 
      public int expectedCompleteTimestamp { get; set; }
   }
}