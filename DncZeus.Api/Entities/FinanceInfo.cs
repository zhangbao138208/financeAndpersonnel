using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class FinanceInfo:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string User { get; set; }
        public string DepartmentCode { get; set; }
        public string FinanceAccount { get; set; }
        public string Type { get; set; }
        public string InfoStatus { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string HandleName { get; set; }
        public string HandleDate { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public string ImagePath { get; set; }
        public string FilePath { get; set; }
        
    }

}
