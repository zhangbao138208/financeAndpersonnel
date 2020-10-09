using System;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Workflow.Template
{
    public class TemplateJsonModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentCode { get; set; }
        public string ParentName { get; set; }
        public bool Visible { get; set; }
        public bool IsStepFree { get; set; }
        public IsDeleted IsDeleted { get; set; }
        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
