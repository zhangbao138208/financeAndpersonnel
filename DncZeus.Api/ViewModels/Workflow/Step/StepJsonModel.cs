using System;
using System.Collections.Generic;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Workflow.Step
{
    public class StepJsonModel
    {
        public string Code { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string UserList { get; set; }
        public string UserListName { get; set; }
        public string Title { get; set; }
        public string SortID { get; set; }
        public bool IsCounterSign { get; set; }
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
