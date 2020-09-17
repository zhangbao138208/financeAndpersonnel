using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.RequestPayload.User.Department
{
    public class DepartmentRequestPaload: RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
    }
}
