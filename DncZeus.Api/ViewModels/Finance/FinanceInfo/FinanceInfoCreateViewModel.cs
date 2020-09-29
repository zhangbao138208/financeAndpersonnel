using System;
using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.Finance.FinanceInfo
{
    public class FinanceInfoCreateViewModel
    {
        public string Code { get; set; }
        public string User { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string FinanceAccount { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Type { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string InfoStatus { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string HandleName { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public DateTime? HandleDate { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public decimal Amount { get; set; }
        public string ImagePath { get; set; }
        public string FilePath { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
