using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class WorkflowReceiver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//添加时自动增长
        public int Id { get; set; }
        public string WorkflowCode { get; set; }
        public string TemplateCode { get; set; }
        public string StepCode { get; set; }
        public Guid User { get; set; }
        public bool IsCheck { get; set; }
        public DateTime CheckDate { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Note { get; set; }
        public string Description { get; set; }
        public Guid CreateUser { get; set; }
        public DateTime CreateDate { get; set; }

        public string Additions { get; set; }
    }

}
