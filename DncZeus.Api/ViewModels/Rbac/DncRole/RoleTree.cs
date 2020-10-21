using System.Collections.Generic;
using DncZeus.Api.Entities.Enums;

namespace DncZeus.Api.ViewModels.Rbac.DncRole
{
    public class RoleTree
    {
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Title { get; set; }
       
        /// <summary>
        /// 是否展开子节点
        /// </summary>
        public bool Expand { get; set; }
        /// <summary>
        /// 禁掉响应
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 禁掉 checkbox
        /// </summary>
        public bool DisableCheckbox { get; set; }
        /// <summary>
        /// 是否选中子节点	
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// 是否勾选(如果勾选，子节点也会全部勾选)
        /// </summary>
        public bool Checked { get; set; }
        public List<RoleTree> Children { get; set; }
    }
}