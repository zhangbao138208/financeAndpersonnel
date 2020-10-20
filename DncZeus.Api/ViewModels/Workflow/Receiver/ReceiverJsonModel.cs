using System;

namespace DncZeus.Api.ViewModels.Workflow.Receiver
{
    public class ReceiverJsonModel
    {
        public int Id { get; set; }
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }

        public Guid User { get; set; }
        public string UserName { get; set; }
        public bool IsCheck { get; set; }
        public DateTime CheckDate { get; set; }
        public string ListType { get; set; }
        public string ListTypeName { get; set; }
        public string Status { get; set; }
        public string OldStatus { get; set; }

        public bool CanEdit { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Note { get; set; }
        public Guid CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }

        public string Additions { get; set; }
    }
}
