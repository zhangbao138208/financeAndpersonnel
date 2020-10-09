using System.Collections.Generic;

namespace DncZeus.Api.ViewModels.Workflow.Template
{
    public class TemplateTree
    {
        /// <summary>
        /// 
        /// </summary>
        public TemplateTree()
        {
            Children = new List<TemplateTree>();
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 子节点属性数组
        /// </summary>
        public List<TemplateTree> Children { get; set; }
    }
}
