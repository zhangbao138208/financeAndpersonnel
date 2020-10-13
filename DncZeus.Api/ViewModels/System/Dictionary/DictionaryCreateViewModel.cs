using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.System.Dictionary
{
    public class DictionaryCreateViewModel
    {

        public string Code { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string TypeCode { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")] public string Name { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")] public string Value { get; set; }
        public bool Fixed { get; set; }
    }
}
