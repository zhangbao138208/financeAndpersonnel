using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.System.DicType
{
    public class DicTypeCreateViewModel
    {
        public string Code { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        //// <summary>
        /// 使用值
        /// </summary>
        public string Value { get; set; }
    }
}
