using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Workflow.Step
{
    public class StepCreateViewModel
    {
        public string Code { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string TemplateCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public List<string> UserList { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Title { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string SortID { get; set; }
        public bool IsCounterSign { get; set; }
        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
