using DncZeus.Api.ViewModels.Workflow.Receiver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.Workflow.List
{
    public class WorkflowListCreateViewModel
    {
        public string Code { get; set; }
        //[Required(ErrorMessage = "{0}字段是必须的")]
        public Guid? User { get; set; }
       // [Required(ErrorMessage = "{0}字段是必须的")]
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string TemplateCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string CurrentStepCode { get; set; }
        public string NextStepCode { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }
        public string Number { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? NotifyUser { get; set; }

        public string Additions { get; set; }

        public List<Note> Notes { get; set; }
    }
}
