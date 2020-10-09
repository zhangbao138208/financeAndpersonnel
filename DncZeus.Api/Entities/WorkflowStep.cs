using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class WorkflowStep:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string  TemplateCode{ get; set; }
        public string UserList { get; set; }
        public string Title { get; set; }
        public string SortID { get; set; }
        /// <summary>
        /// 会签
        /// </summary>
        public bool IsCounterSign { get; set; }
       
    }

}
