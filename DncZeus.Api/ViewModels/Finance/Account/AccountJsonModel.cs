using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Finance.Account
{
    public class AccountJsonModel
    {
        public string Code { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
        public string Holder { get; set; }
        public string DepartmentCode { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
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
