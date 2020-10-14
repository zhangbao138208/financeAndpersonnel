/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using DncZeus.Api.Entities;
using System;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.Rbac.DncUser
{
    /// <summary>
    /// 
    /// </summary>
    public class UserEditViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 纸飞机机器人令牌
        /// </summary>
        public string TelegramBotToken { get; set; }
        /// <summary>
        /// 纸飞机广播群地址
        /// </summary>
        public string TelegramChatId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IsLocked IsLocked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserStatus Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 用户描述信息
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 部门
        /// </summary>

        public string DepartmentCode { get; set; }
        /// <summary>
        /// 职位
        /// </summary>

        public string PositionCode { get; set; }    
    }
}
