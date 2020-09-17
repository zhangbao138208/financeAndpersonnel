/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.Entities
{
    public abstract class Entity
    {
        /// <summary>
        /// 是否可用(0:禁用,1:可用)
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 是否已删
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatedByUserGuid { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedByUserName { get; set; }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
        /// <summary>
        /// 最近修改者ID
        /// </summary>
        public Guid? ModifiedByUserGuid { get; set; }
        /// <summary>
        /// 最近修改者姓名
        /// </summary>
        public string ModifiedByUserName { get; set; }
    }
}
