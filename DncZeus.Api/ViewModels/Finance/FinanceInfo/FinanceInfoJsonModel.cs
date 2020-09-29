using System;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Finance.FinanceInfo
{
    public class FinanceInfoJsonModel
    {
        public string Code { get; set; }
        public string User { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string FinanceAccount { get; set; }
        public string FinanceAccountName { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string InfoStatus { get; set; }
        public string InfoStatusName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string HandleName { get; set; }
        public string HandleDate { get; set; }
        public decimal Amount { get; set; }
        public string ImagePath { get; set; }
        public string FilePath { get; set; }
        public IsDeleted IsDeleted { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
