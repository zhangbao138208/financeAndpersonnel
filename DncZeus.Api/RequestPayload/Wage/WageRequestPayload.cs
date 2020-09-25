using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.RequestPayload.Wage
{
    public class WageRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string realName { get; set; }
    }
}
