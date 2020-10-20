using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Workflow.Receiver
{
    public class ReceiverEditViewModel
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
        public List<NextUser> NextUsers { get; set; }
        /// <summary>
        /// 只有最后一位审核人可以选择下一步审批
        /// </summary>
        public bool CanChoose { get; set; }

        public bool IsCounterSign { get; set; }

        public Guid User { get; set; }
        public string UserName { get; set; }
        public string ListType { get; set; }
        public string ListTypeName { get; set; }
        public List<ReceiverStatus> Statuses { get; set; }
        public string Description { get; set; }
        public string EndDate { get; set; }    

        public string Additions { get; set; }
    }
    public class NextUser
    {
        public Guid User { get; set; }
        public string UserName { get; set; }
    }
    public class ReceiverStatus{
        public string Name { get; set; }
        public string Vaue { get; set; }
    }
}
