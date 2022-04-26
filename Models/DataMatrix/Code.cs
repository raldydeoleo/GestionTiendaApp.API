
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
   [Table("DataMatrix_Code")]
   public class Code: IDataMatrixEntity
   {
      [Key]
      public int Id { get; set; }
      public string CisType{ get; set; }
      [Required]
      public string CodeValue { get; set; }
      public int LabelId { get; set; }
      public bool IsConfirmed { get; set; }
      public bool IsDropout { get; set; }
      public bool IsPrinted { get; set; }
      public DateTime? ConfirmDate { get; set; }
      public DateTime? DropoutDate { get; set; }
      public DateTime? PrintDate { get; set; }
      public string UserConfirm { get; set; }
      public string UserDropout { get; set; }
      public string UserPrint { get; set; }
      [Required]
      public int OrderId { get; set; }
      public Order Order { get; set; }
   }
}