using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    /// <summary>
    /// 简历
    /// </summary>
    public class ResumeInfo:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        //public DncUser User { get; set; }
        //public UserDepartment Department { get; set; }
        //public UserPosition Position { get; set; }
        public Guid UserGuid { get; set; }
        public string DepartmentCode { get; set; }
        public string PositionCode { get; set; }
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
        
       
    }
    public enum Sex
    {
        All = 0,
        男 = 1,
        女 = 2,
        其他 = 3
    }
    public enum HomeInfo
    {
        All=0,
        已婚=1,
        未婚=2,
        离异=3,
        丧偶=4,
        其他=5
    }
    public enum JobStatus
    {
        All=0,
        未面试=1,
        隔离未上岗=2,
        隔离已上岗=3,
        已上岗=4,
        已离职=5,
        其他=6
    }
    public enum TypeID
    {
        All = 0,
        一般 = 1,
        推荐 = 2,
        黑名单 = 3,
        其他 = 4
    }
    

}
