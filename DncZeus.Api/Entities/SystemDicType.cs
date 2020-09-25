using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class SystemDicType
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 使用
        /// </summary>
        public string Value { get; set; }
    }
}
