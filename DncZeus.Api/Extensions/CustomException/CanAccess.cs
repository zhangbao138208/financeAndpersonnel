/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2019-01-08
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace DncZeus.Api.Extensions.CustomException
{
    /// <summary>
    /// 用户可以访问的控制器及操作权限
    /// </summary>
    public class CanAccess
    {
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>

        public string ControllerDisplayName { get; set; }
        /// <summary>
        /// Action集合
        /// </summary>
        public List<CanAccessAction> Actions { get; set; }
        
        
    }

    public class CanAccessAction
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 用户拥有的API操作权限
    /// </summary>
    public class OwnedApiPermission
    {
        /// <summary>
        /// 
        /// </summary>
        public OwnedApiPermission()
        {
            CanAccesses = new List<CanAccess>();
        }
        /// <summary>
        /// 可以访问的API控制器集合
        /// </summary>
        public List<CanAccess> CanAccesses { get; set; }

        /// <summary>
        /// 是否可以访问
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public bool Can( string controller,  string action,out string controllerName,out string actionName)
        {
            controllerName = "";
            actionName = "";
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
            {
                return false;
            }
            var ctrl = CanAccesses.
                FirstOrDefault(x=>x.Controller.Contains(controller,StringComparison.OrdinalIgnoreCase));
            if (ctrl == null) return false;
            controllerName = ctrl.ControllerDisplayName;
            var allow = ctrl.Actions.
                FirstOrDefault(a => 
                    string.Equals(a.Code, action, StringComparison.OrdinalIgnoreCase));
            if (allow == null)
                return false;
            actionName = allow.Name;
            return true;

        }
    }
}
