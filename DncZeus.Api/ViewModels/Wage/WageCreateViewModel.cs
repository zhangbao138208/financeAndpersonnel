using System;
using System.ComponentModel.DataAnnotations;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Wage
{
    public class WageCreateViewModel
    {
        public string Code { get; set; }
        public Guid UserGuid { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string RealName { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string PositionCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public decimal? BaseWage { get; set; }
        [Required(ErrorMessage ="{0}字段是必须的")]
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
        public string Additions { get; set; }
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
        public string Other { get; set; }
        public string OtherRemark { get; set; }
        /// <summary>
        /// 应发工资
        /// </summary>
        public decimal? TotalWage { get; set; }

        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
