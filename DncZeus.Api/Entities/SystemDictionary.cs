using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DncZeus.Api.Entities
{
    public class SystemDictionary
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public SystemDicType systemDicType { get; set; }
        public string TypeCode { get; set; }
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 使用值
        /// </summary>
        public string Value { get; set; }
        public bool Fixed { get; set; }
    }
}
