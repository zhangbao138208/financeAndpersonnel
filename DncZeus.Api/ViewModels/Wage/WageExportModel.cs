using System;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Wage
{
    public class WageExportModel
    {
        public string Code { get; set; }
        public Guid UserGuid { get; set; }
        public string RealName { get; set; }
        public string UserName { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public decimal? BaseWage { get; set; }
        public int? WorkDays { get; set; }
        /// <summary>
        /// 加班工资
        /// </summary>
        public decimal? OTWage { get; set; }
        /// <summary>
        /// 加班天数
        /// </summary>
        public int? OTDays { get; set; }
        /// <summary>
        /// 绩效工资
        /// </summary>
        public decimal? PerformanceWage { get; set; }
        /// <summary>
        /// 补发工资
        /// </summary>
        public decimal? ReissueWage { get; set; }
        /// <summary>
        /// 提成
        /// </summary>
        public decimal? Commission { get; set; }
        /// <summary>
        /// 奖金
        /// </summary>
        public decimal? Bonus { get; set; }
        /// <summary>
        /// 补贴
        /// </summary>
        public decimal? Subsidy { get; set; }
        /// <summary>
        /// 社保
        /// </summary>
        public decimal? SocialSecurity { get; set; }
        /// <summary>
        /// 公积金
        /// </summary>
        public decimal? AccumulationFund { get; set; }
        /// <summary>
        /// 个税
        /// </summary>
        public decimal? IncomeTax { get; set; }
        /// <summary>
        /// 扣款
        /// </summary>
        public string Deductions { get; set; }
        public string Additions { get; set; }
       
        /// <summary>
        /// 应发工资
        /// </summary>
        public decimal? TotalWage { get; set; }
        public Status Status { get; set; }
       
        public IsDeleted IsDeleted { get; set; }
    }
}
