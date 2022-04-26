using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
   [Table("DataMatrix_OrderSetting")]
   public class OrderSetting: IDataMatrixEntity
   {
      [Key]
      public int Id { get; set; }
      [Required]
      public string OmsId { get; set; }
      [Required]
      public string OmsUrl { get; set; }
      [Required]
      public string Token { get; set; }
      [Required]
      public string ConnectionId { get; set; }
      [Required]
      public string ContactPerson { get; set; }
      [Required]
      public string CreateMethodType { get; set; }
      [Required]
      public string FactoryAddress { get; set; }
      [Required]
      public string FactoryCountry { get; set; }
      public int FactoryId { get; set; }
      [Required]
      public string FactoryName { get; set; }
      public int ProductionLineId { get; set; }
      [Required]
      public string ReleaseMethodType { get; set; }
   }
}