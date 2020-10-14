using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class SystemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//添加时自动增长
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Application { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Levels { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Operatingtime { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Operatingaddress { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Logger { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Callsite { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Requesturl { get; set; }
        public string Referrerurl { get; set; }
        public string Action { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string Message { get; set; }
        public string Exception { get; set; }
    }

}
