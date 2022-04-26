
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
   [Table("DataMatrix_Order")]
   public class Order: IDataMatrixEntity
   {
      [Key]
      public int Id { get; set; }
      public string OrderId { get; set; }
      public string ContactPerson { get; set; }
      [Required]
      public string CreateMethodType { get; set; }
      [Column(TypeName = "Date")]
      public DateTime? ExpectedStartDate { get; set; }
      public string FactoryAddress { get; set; }
      [Required]
      public string FactoryCountry { get; set; }
      [Required]
      public int FactoryId { get; set; }
      [Required]
      public string FactoryName { get; set; }
      public int? PoNumber { get; set; }
      [Required]
      public string ProductCode { get; set; }
      [Required]
      public string ProductDescription { get; set; }
      [Required]
      public int ProductionLineId { get; set; }
      public string ProductionOrderId { get; set; }
      [Required]
      public string ReleaseMethodType { get; set; }
      public string ServiceProviderId { get; set; }
      [ConcurrencyCheck]
      public string Status { get; set; }
      [Required]
      public string CisType  { get; set; }
      [Required]
      public string Gtin { get; set; }
      public string Mrp { get; set; }
      [Required]
      public int Quantity { get; set; }
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public string SerialNumberType { get; set; } = "OPERATOR";
      public int? StickerId { get; set; }
      [Required]
      public int TemplateId { get; set; }
      public int? ExpectedCompleteTimestamp { get; set; }
      [NotMapped]
      public string OmsUrl { get; set; }
      [NotMapped]
      public string OmsId { get; set; }
      [NotMapped]
      public string Token { get; set; }
      public bool IsPrintAuthorized{ get; set; }
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public DateTime CreationDate { get; set; }  = DateTime.Now;
      public string UserCreate { get; set; }
      public DateTime? PrintAuthorizedDate { get; set; }
      public string UserPrintAuthorized { get; set; }
      public bool IsClosed{ get; set; }
      public DateTime? CloseDate { get; set; }
      public string UserClose { get; set; }
      public string Remark { get; set; }
      public string SapOrderReference { get; set; }
      public List<Code> Codes { get; set; }
   }
}