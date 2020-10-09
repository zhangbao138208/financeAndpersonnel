using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class WorkflowList
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public Guid User { get; set; }
        public string DepartmentCode { get; set; }
        public string TemplateCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string CurrentStepCode { get; set; }
        public string NextStepCode { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid NotifyUser { get; set; }
    }

}
