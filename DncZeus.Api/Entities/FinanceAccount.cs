using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class FinanceAccount:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
        public string Holder { get; set; }
        public string DepartmentCode { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
       
    }

}
