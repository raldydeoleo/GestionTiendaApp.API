using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoxTrackLabel.API.Repositories;

namespace BoxTrackLabel.API.Models
{
   [Table("DataMatrix_EmailAccounts")]
   public class EmailAccount: IDataMatrixEntity
   {
      [Key]
      public int Id { get; set; }
      public string Email{ get; set; }
      public bool IsCopy { get; set; }
   }
}