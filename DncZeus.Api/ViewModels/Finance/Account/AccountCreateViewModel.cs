using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Finance.Account
{
    public class AccountCreateViewModel
    {
        public string Code { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
       [Required(ErrorMessage = "{0}字段是必须的")]
        public string Holder { get; set; }
        public string DepartmentCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Type { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Account { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid? CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
