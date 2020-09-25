using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class SystemDictionary
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string TypeCode { get; set; }
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 使用值
        /// </summary>
        public string Value { get; set; }
        public bool Fixed { get; set; }
    }
}
