using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DncZeus.Api.Entities
{
    public class SystemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//添加时自动增长
        public int Id { get; set; }
        public string Application { get; set; }
        public string Levels { get; set; }
        public string Operatingtime { get; set; }
        public string Operatingaddress { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Requesturl { get; set; }
        public string Referrerurl { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

}
