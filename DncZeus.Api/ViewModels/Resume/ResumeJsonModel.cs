using DncZeus.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Refuse
{
    public class ResumeJsonModel
    {
        public string Code { get; set; }
        public Guid UserGuid { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        /// <summary>
        /// 简历类型
        /// </summary>
        public TypeID TypeID { get; set; }
        /// <summary>
        /// 人员状态
        /// </summary>
        public JobStatus JobStatus { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        public string Password { get; set; }
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 属相
        /// </summary>
        public string AnimalSign { get; set; }
        public int Age { get; set; }
        public Sex Sex { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        /// <summary>
        /// 工作年限
        /// </summary>
        public int Years { get; set; }
        public string Language { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string License { get; set; }
        /// <summary>
        /// 护照
        /// </summary>
        public string Comments { get; set; }
        public string LevelID { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }
        /// <summary>
        /// 教育经历
        /// </summary>
        public string EducationBackgrounds { get; set; }
        /// <summary>
        /// 工作经历
        /// </summary>
        public string Works { get; set; }
        /// <summary>
        /// 荣誉奖项
        /// </summary>
        public string Awards { get; set; }
        /// <summary>
        /// 自我评价
        /// </summary>
        public string SelfEvaluations { get; set; }
        /// <summary>
        /// 技能
        /// </summary>
        public string Interests { get; set; }
        /// <summary>
        /// 技能
        /// </summary>
        public string Skills { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 紧急联系人电话
        /// </summary>
        public string TEL { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        public string Wechat { get; set; }
        /// <summary>
        /// 纸飞机
        /// </summary>
        public string Telegram { get; set; }
        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 支付宝
        /// </summary>
        public string Alipay { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 家庭情况
        /// </summary>
        public HomeInfo HomeInfo { get; set; }
        public string Remark { get; set; }
        public string ImagePath { get; set; }
        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
