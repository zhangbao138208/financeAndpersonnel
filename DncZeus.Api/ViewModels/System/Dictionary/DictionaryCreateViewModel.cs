using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.System.Dictionary
{
    public class DictionaryCreateViewModel
    {

        public string Code { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string TypeCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        /// <summary>
        /// 使用值
        /// </summary>
        public string Value { get; set; }
        public bool Fixed { get; set; }
    }
}
