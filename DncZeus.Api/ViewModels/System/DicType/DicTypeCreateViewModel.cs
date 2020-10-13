using System.ComponentModel.DataAnnotations;

namespace DncZeus.Api.ViewModels.System.DicType
{
    public class DicTypeCreateViewModel
    {
        public string Code { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")] public string Name { get; set; }
        [Required(ErrorMessage = "{0}字段是必须的")]
        public string Value { get; set; }
    }
}
