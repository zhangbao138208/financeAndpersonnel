using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class WorkflowTemplate:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentCode { get; set; }
        public bool Visible { get; set; }
        public bool IsStepFree { get; set; }
       
    }

}
