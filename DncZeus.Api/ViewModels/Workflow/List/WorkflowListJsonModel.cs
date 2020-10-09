using System;

namespace DncZeus.Api.ViewModels.Workflow.List
{
    public class WorkflowListJsonModel
    {
        public string Code { get; set; }
        public Guid User { get; set; }
        public string UserName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }

        public string Type { get; set; }
        public string TypeName { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string CurrentStepCode { get; set; }
        public string CurrentStepName { get; set; }
        public string NextStepCode { get; set; }
        public string NextStepName { get; set; }

        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid NotifyUser { get; set; }
    }
}
