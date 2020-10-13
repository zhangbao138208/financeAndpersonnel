using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Workflow.Receiver
{
    public class ReceiverCreateViewModel
    {
        public int Id { get; set; }
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }

        public string NextStepCode { get; set; }
        public string NextStepName { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }

        public Guid User { get; set; }
        public string UserName { get; set; }
        public bool IsCheck { get; set; }
        public DateTime CheckDate { get; set; }
        public string ListType { get; set; }
        public string ListTypeName { get; set; }
        [Required(ErrorMessage ="{0}该字段是必须的")]
        public Int16 Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Note { get; set; }
        public string Description { get; set; }
        public Guid CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
