using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class WageInfo : Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public Guid UserGuid { get; set; }
        public string RealName { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? BaseWage { get; set; }
        public int? WorkDays { get; set; }
        /// <summary>
        /// 加班工资
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? OTWage { get; set; }
        /// <summary>
        /// 加班天数
        /// </summary>
        public int? OTDays { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 绩效工资
        /// </summary>
        public decimal? PerformanceWage { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 补发工资
        /// </summary>
        public decimal? ReissueWage { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 提成
        /// </summary>
        public decimal? Commission { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 奖金
        /// </summary>
        public decimal? Bonus { get; set; }

        /// <summary>
        /// 额外
        /// </summary>
        public string Additions { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 补贴
        /// </summary>
        public decimal? Subsidy { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 社保
        /// </summary>
        public decimal? SocialSecurity { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        /// <summary>
        /// 公积金
        /// </summary>
        public decimal? AccumulationFund { get; set; }
        [Column(TypeName = "decimal(18,4)")] public decimal? IncomeTax { get; set; }
        /// <summary>
        /// 扣款
        /// </summary>
        public string Deductions { get; set; }
        [Column(TypeName = "decimal(18,4)")] public decimal? TotalWage { get; set; }

    }
}
